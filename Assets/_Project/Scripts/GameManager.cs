using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public static GameManager Main;

    public bool localPlayerIsWhite = true;
    public bool localPlayersTurn = true;
    public TileInfo[,] Board;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile[] tiles;
    [SerializeField] private GameObject[] figurePrefabs;
    private List<Vector2Int> validPositions = new List<Vector2Int>();
    private Vector2Int selectedPosition;


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
            if (Board[Position.x, Position.y].isWhite == localPlayerIsWhite && localPlayersTurn)
            {
                validPositions = FigureMovement.GetValidPositions(localPlayerIsWhite, Board[Position.x, Position.y], Position);

                tilemap.ClearAllTiles();
                tilemap.SetTile((Vector3Int)Position, tiles[0]);
                for (int i = 0; i < validPositions.Count; i++)
                {
                    if (Board[validPositions[i].x, validPositions[i].y].figure != ChessFigure.Empty)
                    {
                        tilemap.SetTile((Vector3Int)validPositions[i], tiles[2]);
                    }
                    else
                    {
                        tilemap.SetTile((Vector3Int)validPositions[i], tiles[1]);
                    }
                }

                selectedPosition = Position;
            }
        }
        else if (validPositions.Contains(Position))
        {
            PhotonView.Get(this).RpcSecure("FinishTurn", RpcTarget.AllBufferedViaServer, false, (Vector2)selectedPosition, (Vector2)Position);
            validPositions = new List<Vector2Int>();
            tilemap.ClearAllTiles();
        }
        else
        {
            selectedPosition = Position;
            validPositions = new List<Vector2Int>();
            tilemap.ClearAllTiles();
        }
    }

    public void MovePiece(Vector2Int OldPosition, Vector2Int NewPosition)
    {
        if (Board[NewPosition.x, NewPosition.y].transform != null)
        {
            Destroy(Board[NewPosition.x, NewPosition.y].transform.gameObject);
        }

        Board[NewPosition.x, NewPosition.y] = Board[OldPosition.x, OldPosition.y];
        Board[NewPosition.x, NewPosition.y].hasMoved = true;
        Board[OldPosition.x, OldPosition.y] = new TileInfo(false, ChessFigure.Empty, null, false);

        validPositions = new List<Vector2Int>();
        selectedPosition = Vector2Int.zero;

        UpdateFigures();
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