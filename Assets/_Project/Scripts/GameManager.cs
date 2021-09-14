using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public static GameManager Main;

    public int localPlayerID = 0;
    public int turnID = 0;
    public TileInfo[,] Board;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile[] tiles;
    [SerializeField] private GameObject figurePrefab;
    [SerializeField] private ChessFigureSet chessFigureSet;
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
            newboard[i, 1] = new TileInfo(0, ChessFigure.Pawn, null);
            newboard[i, 6] = new TileInfo(1, ChessFigure.Pawn, null);
        }

        newboard[0, 0] = new TileInfo(0, ChessFigure.Rook, null);
        newboard[1, 0] = new TileInfo(0, ChessFigure.Knight, null);
        newboard[2, 0] = new TileInfo(0, ChessFigure.Bishop, null);

        newboard[3, 0] = new TileInfo(0, ChessFigure.Queen, null);
        newboard[4, 0] = new TileInfo(0, ChessFigure.King, null);

        newboard[5, 0] = new TileInfo(0, ChessFigure.Bishop, null);
        newboard[6, 0] = new TileInfo(0, ChessFigure.Knight, null);
        newboard[7, 0] = new TileInfo(0, ChessFigure.Rook, null);

        newboard[0, 7] = new TileInfo(1, ChessFigure.Rook, null);
        newboard[1, 7] = new TileInfo(1, ChessFigure.Knight, null);
        newboard[2, 7] = new TileInfo(1, ChessFigure.Bishop, null);

        newboard[3, 7] = new TileInfo(1, ChessFigure.Queen, null);
        newboard[4, 7] = new TileInfo(1, ChessFigure.King, null);

        newboard[5, 7] = new TileInfo(1, ChessFigure.Bishop, null);
        newboard[6, 7] = new TileInfo(1, ChessFigure.Knight, null);
        newboard[7, 7] = new TileInfo(1, ChessFigure.Rook, null);

        return newboard;
    }

    // -----------------------------------------------------------------------------------------------------

    public void PositionSelected(Vector2Int Position)
    {
        if (validPositions.Count == 0 && Board[Position.x, Position.y].figure != ChessFigure.Empty)
        {
            if (Board[Position.x, Position.y].ownerID == localPlayerID && turnID == localPlayerID)
            {
                validPositions = FigureMovement.GetValidPositions(localPlayerID, Board[Position.x, Position.y], Position);

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
        Board[OldPosition.x, OldPosition.y] = new TileInfo(-1, ChessFigure.Empty, null, false);

        validPositions = new List<Vector2Int>();
        selectedPosition = Vector2Int.zero;

        UpdateFigures();
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
                    GameObject obj = Instantiate(figurePrefab, new Vector3(x, 0, y), Quaternion.Euler(0, Board[x, y].ownerID * 180, 0));
                    obj.name = Board[x, y].figure.ToString() + " (" + Board[x, y].ownerID + ")";
                    obj.transform.parent = transform;

                    obj.GetComponent<MeshFilter>().mesh = chessFigureSet.Meshes[(int)Board[x, y].figure - 1];
                    obj.GetComponent<MeshRenderer>().material = chessFigureSet.PlayerMaterials[Board[x, y].ownerID];

                    Board[x, y].transform = obj.transform;
                }
                // If Chess figure is created update the position
                else if (Board[x, y].figure != ChessFigure.Empty)
                {
                    Board[x, y].transform.position = new Vector3(x, 0, y);
                }
            }
        }
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
    public int ownerID;
    public ChessFigure figure;
    public Transform transform;
    public bool hasMoved;

    public TileInfo(int NewOwnerID, ChessFigure Figure, Transform Object)
    {
        ownerID = NewOwnerID;
        figure = Figure;
        transform = Object;
        hasMoved = false;
    }
    public TileInfo(int NewOwnerID, ChessFigure Figure, Transform Object,bool HasMoved)
    {
        ownerID = NewOwnerID;
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