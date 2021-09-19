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
        EconomySystem.Initialize();
    }

    // -----------------------------------------------------------------------------------------------------
    
    public void UpdateFigures()
    {
        // For each tile on the board
        for (int x = 0; x < Board.GetLength(0); x++)
        {
            for (int y = 0; y < Board.GetLength(1); y++)
            {
                if (Board[x, y].figureTransform == null && Board[x, y].figure != ChessFigure.Empty && Board[x, y].building == ChessBuiding.Empty)
                {
                    // Create figure
                    GameObject obj = Instantiate(figurePrefab, new Vector3(x, 0, y), Quaternion.Euler(0, Board[x, y].ownerID * 180, 0));
                    obj.name = Board[x, y].figure.ToString() + " (" + Board[x, y].ownerID + ")";
                    obj.transform.parent = transform;

                    obj.GetComponent<MeshFilter>().mesh = ChessFigureSetInUse.GetMesh(Board[x, y].figure, Board[x, y].building);
                    obj.GetComponent<MeshRenderer>().material = ChessFigureSetInUse.MaterialsByID[Board[x, y].ownerID + 1];

                    Board[x, y].figureTransform = obj.transform;
                }
                else if (Board[x, y].figureTransform != null && Board[x, y].figure != ChessFigure.Empty && Board[x, y].building == ChessBuiding.Empty)
                {
                    // Update figure
                    Board[x, y].figureTransform.position = new Vector3(x, 0, y);
                }

                else if (Board[x, y].buildingTransform == null && Board[x, y].building != ChessBuiding.Empty && Board[x, y].figure == ChessFigure.Empty)
                {
                    // Create building
                    GameObject obj = Instantiate(figurePrefab, new Vector3(x, 0, y), Quaternion.identity);
                    obj.name = Board[x, y].building.ToString() + " (" + Board[x, y].ownerID + ")";
                    obj.transform.parent = transform;

                    obj.GetComponent<MeshFilter>().mesh = ChessFigureSetInUse.GetMesh(Board[x, y].figure, Board[x, y].building);
                    obj.GetComponent<MeshRenderer>().material = ChessFigureSetInUse.MaterialsByID[Board[x, y].ownerID + 1];

                    Board[x, y].buildingTransform = obj.transform;
                }
                else if (Board[x, y].buildingTransform != null && Board[x, y].building != ChessBuiding.Empty && Board[x, y].figure == ChessFigure.Empty)
                {
                    // Update building
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------------------------

    public void MovePiece(Vector2Int OldPosition, Vector2Int NewPosition)
    {
        if (Board[NewPosition.x, NewPosition.y].figureTransform != null)
        {
            Destroy(Board[NewPosition.x, NewPosition.y].figureTransform.gameObject);
            GameLoop.Main.destroyEffect.transform.position = new Vector3(NewPosition.x, 0, NewPosition.y);
            GameLoop.Main.destroyEffect.SetGradient("Gradient", GameLoop.Main.colors[Board[NewPosition.x, NewPosition.y].ownerID]);
            GameLoop.Main.destroyEffect.Play();
        }

        Board[NewPosition.x, NewPosition.y].ownerID = Board[OldPosition.x, OldPosition.y].ownerID;
        Board[OldPosition.x, OldPosition.y].ownerID = 0;

        Board[NewPosition.x, NewPosition.y].figure = Board[OldPosition.x, OldPosition.y].figure;
        Board[OldPosition.x, OldPosition.y].figure = ChessFigure.Empty;

        Board[NewPosition.x, NewPosition.y].figureTransform = Board[OldPosition.x, OldPosition.y].figureTransform;
        Board[OldPosition.x, OldPosition.y].figureTransform = null;

        Board[NewPosition.x, NewPosition.y].hasMoved = true;
        Board[OldPosition.x, OldPosition.y].hasMoved = false;

        UpdateFigures();
    }

    public void BuildBuilding(Vector2Int NewPosition, ChessBuiding Building)
    {
        Board[NewPosition.x, NewPosition.y].building = Building;

        UpdateFigures();
    }
}