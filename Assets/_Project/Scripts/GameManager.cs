using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Main;

    public bool whitesTurn = true;
    public TileInfo[,] Board;

    public GameObject[] figurePrefabs;
    public List<Vector2Int> validPositions;
    public Vector2Int selectedPosition;

    public List<Vector2Int> directions = new List<Vector2Int>() {
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(1, 0),
        new Vector2Int(1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1),};

    public List<Vector2Int> cardinals = new List<Vector2Int>() {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0), };

    public List<Vector2Int> intermediates = new List<Vector2Int>() {
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 1),};

    private void Awake()
    {
        // Make GameManager a singleton
        if (Main == null)
        {
            Main = this;
        }
        else
        {
            Destroy(this);
            Debug.LogWarning("There can only be one GameManager!");
        }

        // Setup the game
        Board = CreateBoard();
        UpdateFigures();
    }

    public static TileInfo[,] CreateBoard()
    {
        // This creates a default Chess board with Figures (does not instantiate so you need to call "UpdateFigures()" later)
        TileInfo[,] newboard = new TileInfo[8, 8];

        for (int i = 0; i < 8; i++)
        {
            newboard[i, 1] = new TileInfo(true, ChessFigure.Pawn, null);
            newboard[i, 6] = new TileInfo(false, ChessFigure.Pawn, null);
        }

        newboard[0, 0] = new TileInfo(true, ChessFigure.Rook, null);
        newboard[1, 0] = new TileInfo(true, ChessFigure.Knight, null);
        newboard[2, 0] = new TileInfo(true, ChessFigure.Bishop, null);

        newboard[3, 0] = new TileInfo(true, ChessFigure.Queen, null);
        newboard[4, 0] = new TileInfo(true, ChessFigure.King, null);

        newboard[5, 0] = new TileInfo(true, ChessFigure.Bishop, null);
        newboard[6, 0] = new TileInfo(true, ChessFigure.Knight, null);
        newboard[7, 0] = new TileInfo(true, ChessFigure.Rook, null);

        newboard[0, 7] = new TileInfo(false, ChessFigure.Rook, null);
        newboard[1, 7] = new TileInfo(false, ChessFigure.Knight, null);
        newboard[2, 7] = new TileInfo(false, ChessFigure.Bishop, null);

        newboard[3, 7] = new TileInfo(false, ChessFigure.Queen, null);
        newboard[4, 7] = new TileInfo(false, ChessFigure.King, null);

        newboard[5, 7] = new TileInfo(false, ChessFigure.Bishop, null);
        newboard[6, 7] = new TileInfo(false, ChessFigure.Knight, null);
        newboard[7, 7] = new TileInfo(false, ChessFigure.Rook, null);

        return newboard;
    }

    private void UpdateFigures()
    {
        // For each tile on the board
        for (int x = 0; x < Board.GetLength(0); x++)
        {
            for (int y = 0; y < Board.GetLength(1); y++)
            {
                // Check if Chess figure is created / referenced on the board and if not create it
                if (Board[x, y].transform == null && Board[x, y].figure != ChessFigure.Empty)
                {
                    GameObject obj = Instantiate(figurePrefabs[(int)Board[x, y].figure - 1 + ((Board[x, y].isWhite ? 0 : 1) * 6)], new Vector3(x, 0, y), Quaternion.identity);
                    Board[x, y].transform = obj.transform;
                    obj.transform.parent = transform;
                }
                // If Chess figure is created update the position
                else if (Board[x, y].figure != ChessFigure.Empty)
                {
                    Board[x, y].transform.position = new Vector3(x, 0, y);
                }
            }
        }
    }

    // ----------------------------------------------- Needs reworking! ------------------------------------
    public void PositionSelected(Vector2Int Position)
    {
        if (validPositions.Count == 0 && Board[Position.x, Position.y].figure != ChessFigure.Empty)
        {
            if (Board[Position.x, Position.y].isWhite && whitesTurn)
            {
                validPositions = GetValidPositions(true, Board[Position.x, Position.y].figure, Position, Board[Position.x, Position.y].hasMoved);
                selectedPosition = Position;
            }
            else if (!Board[Position.x, Position.y].isWhite && !whitesTurn)
            {
                validPositions = GetValidPositions(false, Board[Position.x, Position.y].figure, Position, Board[Position.x, Position.y].hasMoved);
                selectedPosition = Position;
            }
        }
        else if (validPositions.Contains(Position))
        {
            MovePiece(selectedPosition, Position);
        }
        else
        {
            validPositions = new List<Vector2Int>();
            selectedPosition = Position;
        }
    }

    private List<Vector2Int> GetValidPositions(bool IsWhite, ChessFigure Figure, Vector2Int Position, bool HasMoved)
    {
        List<Vector2Int> validpositions = new List<Vector2Int>();
        //Calculates distances from the map edge
        int distancefromleft = Position.x;
        int distancefromright = Board.GetLength(0) - Position.x -1;
        int distancefrombottom = Position.y;
        int distancefromtop = Board.GetLength(1) - Position.y -1;
        int distancefromtopright = Mathf.Min(distancefromtop, distancefromright);
        int distancefrombottomright = Mathf.Min(distancefrombottom, distancefromright);
        int distancefromtopleft = Mathf.Min(distancefromtop, distancefromleft);
        int distancefrombottomleft = Mathf.Min(distancefrombottom, distancefromleft);

        Debug.Log("Distance from edge L,R,T,B,TR,BR,TL,BL:" + distancefromleft + distancefromright
            + distancefromtop + distancefrombottom + distancefromtopright + distancefrombottomright + distancefromtopleft + distancefrombottomleft);
        
        switch (Figure)
        {
            case ChessFigure.King:
                //King movement, good

                for (int i = 0; i <= 7; i++)
                {
                    Vector2Int Tile = Position + directions[i];

                    if (Tile.x < Board.GetLength(0) && Tile.x >= 0 &&
                        Tile.y < Board.GetLength(1) && Tile.y >= 0)
                    {
                        if (Board[Tile.x, Tile.y].figure == ChessFigure.Empty)
                        {
                            validpositions.Add(Tile);
                        }
                        else if (Board[Tile.x, Tile.y].figure != ChessFigure.Empty && Board[Tile.x, Tile.y].isWhite != IsWhite)
                        {
                            validpositions.Add(Tile);
                        }
                    }
                }

                break;

            case ChessFigure.Queen:
                //Queen movement, good

                for (int dir = 0; dir <= 7; dir++)
                {
                    for (int i = 1; i <= Mathf.Max(Board.GetLength(0), Board.GetLength(1)); i++)
                    {
                        Vector2Int Tile = Position + (directions[dir] * i);
                        if (Tile.x < Board.GetLength(0) && Tile.x >= 0 &&
                            Tile.y < Board.GetLength(1) && Tile.y >= 0)
                        {
                            if (Board[Tile.x, Tile.y].figure == ChessFigure.Empty)
                            {
                                validpositions.Add(Tile);
                            }
                            else if (Board[Tile.x, Tile.y].figure != ChessFigure.Empty)
                            {
                                if (Board[Tile.x, Tile.y].isWhite != IsWhite)
                                {
                                    validpositions.Add(Tile);
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                break;

            case ChessFigure.Bishop:
                //Bishop movement, good

                for (int dir = 0; dir <= 3; dir++)
                {
                    for (int i = 1; i <= Mathf.Max(Board.GetLength(0), Board.GetLength(1)); i++)
                    {
                        Vector2Int Tile = Position + (intermediates[dir] * i);
                        if (Tile.x < Board.GetLength(0) && Tile.x >= 0 &&
                            Tile.y < Board.GetLength(1) && Tile.y >= 0)
                        {
                            if (Board[Tile.x, Tile.y].figure == ChessFigure.Empty)
                            {
                                validpositions.Add(Tile);
                            }
                            else if (Board[Tile.x, Tile.y].figure != ChessFigure.Empty)
                            {
                                if (Board[Tile.x, Tile.y].isWhite != IsWhite)
                                {
                                    validpositions.Add(Tile);
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                break;

            case ChessFigure.Knight:
                Debug.Log(Figure);
                break;

            case ChessFigure.Rook:
                //Rook movement, good

                for (int dir = 0; dir <= 3; dir++)
                {
                    for (int i = 1; i <= Mathf.Max(Board.GetLength(0), Board.GetLength(1)); i++)
                    {
                        Vector2Int Tile = Position + (cardinals[dir] * i);
                        if (Tile.x < Board.GetLength(0) && Tile.x >= 0 &&
                            Tile.y < Board.GetLength(1) && Tile.y >= 0)
                        {
                            if (Board[Tile.x, Tile.y].figure == ChessFigure.Empty)
                            {
                                validpositions.Add(Tile);
                            }
                            else if (Board[Tile.x, Tile.y].figure != ChessFigure.Empty)
                            {
                                if (Board[Tile.x, Tile.y].isWhite != IsWhite)
                                {
                                    validpositions.Add(Tile);
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                break;

                //Pawn movement, works as expected. however can be redone to use a new list of pawn directions like the rook/bishop/queen/king 
                //but you will still have edge cases that require its own if statement that it is unlikely worth it
            case ChessFigure.Pawn:
                if (distancefromtop >0 &&
                    IsWhite && Board[Position.x, Position.y + 1].figure == ChessFigure.Empty)
                {
                    validpositions.Add(Position + Vector2Int.up);

                    if (distancefromtop > 0 &&
                    IsWhite && Board[Position.x, Position.y + 2].figure == ChessFigure.Empty && !HasMoved)
                    {
                        validpositions.Add(Position + Vector2Int.up * 2);
                    }
                }
                if (distancefromtop > 0 && distancefromright >0 &&
                    IsWhite && Board[Position.x + 1, Position.y + 1].figure != ChessFigure.Empty && !Board[Position.x + 1, Position.y + 1].isWhite)
                {
                    validpositions.Add(Position + Vector2Int.up + Vector2Int.right);
                }
                if (distancefromtop > 0 && distancefromleft >0 &&
                    IsWhite && Board[Position.x - 1, Position.y + 1].figure != ChessFigure.Empty && !Board[Position.x - 1, Position.y + 1].isWhite)
                {
                    validpositions.Add(Position + Vector2Int.up + Vector2Int.left);
                }


                if (distancefrombottom >0 &&
                    !IsWhite && Board[Position.x, Position.y - 1].figure == ChessFigure.Empty)
                {
                    validpositions.Add(Position + Vector2Int.down);

                    if (distancefrombottom > 0 &&
                        !IsWhite && Board[Position.x, Position.y - 2].figure == ChessFigure.Empty && !HasMoved)
                    {
                        validpositions.Add(Position + Vector2Int.down * 2);
                    }
                }
                if (distancefrombottom > 0 && distancefromright >0 &&
                    !IsWhite && Board[Position.x + 1, Position.y - 1].figure != ChessFigure.Empty && Board[Position.x + 1, Position.y - 1].isWhite)
                {
                    validpositions.Add(Position + Vector2Int.down + Vector2Int.right);
                }
                if (distancefrombottom > 0 && distancefromleft >0 &&
                    !IsWhite && Board[Position.x - 1, Position.y - 1].figure != ChessFigure.Empty && Board[Position.x - 1, Position.y - 1].isWhite)
                {
                    validpositions.Add(Position + Vector2Int.down + Vector2Int.left);
                }
                break;

            default:
                Debug.LogError("Trying to move air!");
                break;
        }

        return validpositions;
    }

    private void MovePiece(Vector2Int OldPosition, Vector2Int NewPosition)
    {
        if (Board[NewPosition.x, NewPosition.y].transform != null)
        {
            Destroy(Board[NewPosition.x, NewPosition.y].transform.gameObject);
        }
        Board[NewPosition.x, NewPosition.y] = Board[OldPosition.x, OldPosition.y];
        Board[NewPosition.x, NewPosition.y].hasMoved = true;
        Board[OldPosition.x, OldPosition.y] = new TileInfo(false, ChessFigure.Empty, null,false);

        validPositions = new List<Vector2Int>();
        selectedPosition = Vector2Int.zero;

        UpdateFigures();
        whitesTurn = !whitesTurn;
    }
    // -----------------------------------------------------------------------------------------------------

    public void OnDrawGizmos()
    {
        // For showing valid positions in editor for debuging
        Gizmos.color = Color.green;
        for (int i = 0; i < validPositions.Count; i++)
        {
            Gizmos.DrawCube(new Vector3(validPositions[i].x, 0, validPositions[i].y), new Vector3(1, 0.03f, 1));
        }
    }
}

[System.Serializable]
public struct TileInfo
{
    public bool isWhite;
    public ChessFigure figure;
    public Transform transform;
    public bool hasMoved;

    public TileInfo(bool IsWhite, ChessFigure Figure, Transform Object)
    {
        isWhite = IsWhite;
        figure = Figure;
        transform = Object;
        hasMoved = false;
    }
    public TileInfo(bool IsWhite, ChessFigure Figure, Transform Object,bool HasMoved)
    {
        isWhite = IsWhite;
        figure = Figure;
        transform = Object;
        hasMoved = HasMoved;
    }
}

[System.Serializable]
public enum ChessFigure
{
    Empty, King, Queen, Bishop, Knight, Rook, Pawn,
}