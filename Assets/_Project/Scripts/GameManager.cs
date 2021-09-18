using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Main;

    [HideInInspector] public int localPlayerID = 0;
    [HideInInspector] public int turnID = 0;
    public int turnPointsLeft = 0;
    public TileInfo[,] Board;

    public GameObject figurePrefab;
    public static ChessFigureSet ChessFigureSetInUse;
    public static GameSettings GameSettingsInUse;

    [SerializeField] private GameObject smallBoard;
    [SerializeField] private GameObject bigBoard;

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

        if (GameSettingsInUse.MapSize == new Vector2Int(14, 14))
        {
            Board = BoardMaster.CreateChessEmpiresBoard();
            smallBoard.SetActive(false);
            bigBoard.SetActive(true);
        }
        else
        {
            Board = BoardMaster.CreateClasicChessBoard();
            smallBoard.SetActive(true);
            bigBoard.SetActive(false);
        }

        UpdateFigures();
        turnPointsLeft = GameSettingsInUse.MovePointsPerTurn;
    }

    // -----------------------------------------------------------------------------------------------------
    
    public void UpdateFigures()
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

                    obj.GetComponent<MeshFilter>().mesh = ChessFigureSetInUse.GetMesh(Board[x, y].figure);
                    obj.GetComponent<MeshRenderer>().material = ChessFigureSetInUse.MaterialsByID[Board[x, y].ownerID + 1];

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

    public void MovePiece(Vector2Int OldPosition, Vector2Int NewPosition)
    {
        if (Board[NewPosition.x, NewPosition.y].transform != null)
        {
            Destroy(Board[NewPosition.x, NewPosition.y].transform.gameObject);
            GameLoop.Main.destroyEffect.transform.position = new Vector3(NewPosition.x, 0, NewPosition.y);
            GameLoop.Main.destroyEffect.SetGradient("Gradient", GameLoop.Main.colors[Board[NewPosition.x, NewPosition.y].ownerID]);
            GameLoop.Main.destroyEffect.Play();
        }

        Board[NewPosition.x, NewPosition.y].ownerID = Board[OldPosition.x, OldPosition.y].ownerID;
        Board[OldPosition.x, OldPosition.y].ownerID = 0;

        Board[NewPosition.x, NewPosition.y].figure = Board[OldPosition.x, OldPosition.y].figure;
        Board[OldPosition.x, OldPosition.y].figure = ChessFigure.Empty;

        Board[NewPosition.x, NewPosition.y].transform = Board[OldPosition.x, OldPosition.y].transform;
        Board[OldPosition.x, OldPosition.y].transform = null;

        Board[NewPosition.x, NewPosition.y].hasMoved = true;
        Board[OldPosition.x, OldPosition.y].hasMoved = false;

        UpdateFigures();
    }
}