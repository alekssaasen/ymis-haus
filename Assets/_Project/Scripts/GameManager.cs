using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Main;

    [HideInInspector] public int localPlayerID = 0;
    [HideInInspector] public int turnID = 0;
    public int turnPointsLeft = 5;
    public TileInfo[,] Board;

    public GameObject figurePrefab;
    public ChessFigureSet chessFigureSet;
    public GameSettings ChessGameSettings;

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

        if (ChessGameSettings.CurrentGameMode == ChessGameModes.ChessEmpires)
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
    }

    // -----------------------------------------------------------------------------------------------------
    
    public static void UpdateFigures()
    {
        // For each tile on the board
        for (int x = 0; x < Main.Board.GetLength(0); x++)
        {
            for (int y = 0; y < Main.Board.GetLength(1); y++)
            {
                // Check if Chess figure is created / referenced on the board and if not create it
                if (Main.Board[x, y].figureTransform == null && Main.Board[x, y].figure != ChessFigure.Empty)
                {
                    GameObject obj = Instantiate(Main.figurePrefab, new Vector3(x, 0, y), Quaternion.Euler(0, Main.Board[x, y].ownerID * 180, 0));
                    obj.name = Main.Board[x, y].figure.ToString() + " (" + Main.Board[x, y].ownerID + ")";
                    obj.transform.parent = Main.transform;

                    obj.GetComponent<MeshFilter>().mesh = Main.chessFigureSet.PlayerMeshes[(int)Main.Board[x, y].figure - 1];
                    obj.GetComponent<MeshRenderer>().material = Main.chessFigureSet.PlayerMaterials[Main.Board[x, y].ownerID];

                    Main.Board[x, y].figureTransform = obj.transform;
                }
                else if (Main.Board[x, y].buildingTransform == null && Main.Board[x, y].building != ChessBuilding.Empty)
                {
                    GameObject obj = Instantiate(Main.figurePrefab, new Vector3(x, 0, y), Quaternion.identity);
                    obj.name = Main.Board[x, y].building.ToString() + " (" + Main.Board[x, y].ownerID + ")";
                    obj.transform.parent = Main.transform;

                    obj.GetComponent<MeshFilter>().mesh = Main.chessFigureSet.BuildingMeshes[(int)Main.Board[x, y].building - 1];
                    obj.GetComponent<MeshRenderer>().material = Main.chessFigureSet.BuildingMaterials[Main.Board[x, y].ownerID + 1];

                    Main.Board[x, y].buildingTransform = obj.transform;
                }
                // If Chess figure is created update the position
                else if (Main.Board[x, y].figure != ChessFigure.Empty)
                {
                    Main.Board[x, y].figureTransform.position = new Vector3(x, 0, y);
                }
            }
        }
    }
}