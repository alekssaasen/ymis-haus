using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public static GameManager Main;

    [HideInInspector] public int localPlayerID = -1;
    [HideInInspector] public int turnID = 0;
    public int turnPointsLeft = 0;
    public TileInfo[,] Board;

    public GameObject figurePrefab;
    public static ChessFigureSet ChessFigureSetInUse;
    public static GameSettings GameSettingsInUse;
    public static Texture2D MapTexture2D;

    private void Awake()
    {
        if (!PhotonNetwork.InRoom)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            // Make GameManager a singleton
            if (Main == null)
            {
                Main = this;
            }
            else
            {
                Main = this;
                Debug.LogWarning("There can only be one GameManager!");
            }

            Board = BoardMaster.InitializeBoard(GameSettingsInUse.BoardSetup);
            Instantiate(GameSettingsInUse.BoardSetup.Prefab);

            UpdateFigures();
            turnPointsLeft = GameSettingsInUse.MovePointsPerTurn;
            EconomySystem.Initialize();
        }
    }

    private void Start()
    {
        GameLoop.Main.StartLocalTurn();
    }

    // -----------------------------------------------------------------------------------------------------

    public void UpdateFigures()
    {
        // For each tile on the board
        for (int x = 0; x < Board.GetLength(0); x++)
        {
            for (int y = 0; y < Board.GetLength(1); y++)
            {
                if (Board[x, y].ownerID >= PhotonNetwork.PlayerList.Length)
                {
                    Board[x, y] = new TileInfo(-1);
                }

                if (Board[x, y].figureTransform == null && Board[x, y].figure != ChessFigure.Empty && Board[x, y].building == ChessBuiding.Empty)
                {
                    // Create figure
                    Debug.Log("Create figure: (" + x + "," + y + ")");
                    GameObject obj = Instantiate(figurePrefab, new Vector3(x, 0, y), Quaternion.Euler(0, GameSettingsInUse.CameraRotationOffsets[Board[x, y].ownerID], 0));
                    obj.name = Board[x, y].figure.ToString() + " (" + Board[x, y].ownerID + ")";
                    obj.transform.parent = transform;

                    obj.GetComponent<MeshFilter>().mesh = ChessFigureSetInUse.GetMesh(Board[x, y].figure, Board[x, y].building);
                    obj.GetComponent<MeshRenderer>().material = ChessFigureSetInUse.MaterialsByID[Board[x, y].ownerID + 1];

                    Board[x, y].figureTransform = obj.transform;
                }
                else if (Board[x, y].figureTransform != null && Board[x, y].figure != ChessFigure.Empty && Board[x, y].building == ChessBuiding.Empty)
                {
                    // Update figure
                    Debug.Log("Update figure: (" + x + "," + y + ")");
                    Board[x, y].figureTransform.position = new Vector3(x, 0, y);
                }

                else if (Board[x, y].buildingTransform == null && Board[x, y].building != ChessBuiding.Empty && Board[x, y].figure == ChessFigure.Empty)
                {
                    // Create building
                    Debug.Log("Create building: (" + x + "," + y + ")");
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
                    Debug.Log("Update building: (" + x + "," + y + ")");
                    Board[x, y].buildingTransform.GetComponent<MeshRenderer>().material = ChessFigureSetInUse.MaterialsByID[Board[x, y].ownerID + 1];
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------------------------

    public void MovePiece(Vector2Int OldPosition, Vector2Int NewPosition)
    {
        if (Board[NewPosition.x, NewPosition.y].building == ChessBuiding.Mine)
        {
            // Claim gold mine
            Board[NewPosition.x, NewPosition.y].ownerID = Board[OldPosition.x, OldPosition.y].ownerID;
            UpdateFigures();
        }
        else if (Board[NewPosition.x, NewPosition.y].building != ChessBuiding.Empty)
        {
            // Destroy building
            if (Board[NewPosition.x, NewPosition.y].buildingTransform != null)
            {
                Destroy(Board[NewPosition.x, NewPosition.y].buildingTransform.gameObject);
                GameLoop.Main.destroyEffect.transform.position = new Vector3(NewPosition.x, 0, NewPosition.y);
                GameLoop.Main.destroyEffect.SetGradient("Gradient", GameLoop.Main.colors[Board[NewPosition.x, NewPosition.y].ownerID]);
                GameLoop.Main.destroyEffect.Play();
            }

            // Set ownership
            Board[NewPosition.x, NewPosition.y].ownerID = Board[OldPosition.x, OldPosition.y].ownerID;
            Board[OldPosition.x, OldPosition.y].ownerID = Board[OldPosition.x, OldPosition.y].defaultID;

            // Set figure
            Board[NewPosition.x, NewPosition.y].building = ChessBuiding.Empty;
            Board[NewPosition.x, NewPosition.y].figure = Board[OldPosition.x, OldPosition.y].figure;
            Board[OldPosition.x, OldPosition.y].figure = ChessFigure.Empty;

            // Move transform referance
            Board[NewPosition.x, NewPosition.y].buildingTransform = null;
            Board[NewPosition.x, NewPosition.y].figureTransform = Board[OldPosition.x, OldPosition.y].figureTransform;
            Board[OldPosition.x, OldPosition.y].figureTransform = null;

            // Updating other values
            Board[NewPosition.x, NewPosition.y].hasMoved = true;
            Board[OldPosition.x, OldPosition.y].hasMoved = false;

            UpdateFigures();
        }
        else
        {
            // Destroy figure
            if (Board[NewPosition.x, NewPosition.y].figureTransform != null)
            {
                Destroy(Board[NewPosition.x, NewPosition.y].figureTransform.gameObject);
                GameLoop.Main.destroyEffect.transform.position = new Vector3(NewPosition.x, 0, NewPosition.y);
                GameLoop.Main.destroyEffect.SetGradient("Gradient", GameLoop.Main.colors[Board[NewPosition.x, NewPosition.y].ownerID]);
                GameLoop.Main.destroyEffect.Play();
            }

            // Set ownership
            Board[NewPosition.x, NewPosition.y].ownerID = Board[OldPosition.x, OldPosition.y].ownerID;
            Board[OldPosition.x, OldPosition.y].ownerID = Board[OldPosition.x, OldPosition.y].defaultID;

            // Set figure
            Board[NewPosition.x, NewPosition.y].figure = Board[OldPosition.x, OldPosition.y].figure;
            Board[OldPosition.x, OldPosition.y].figure = ChessFigure.Empty;

            // Move transform referance
            Board[NewPosition.x, NewPosition.y].figureTransform = Board[OldPosition.x, OldPosition.y].figureTransform;
            Board[OldPosition.x, OldPosition.y].figureTransform = null;

            // Updating other values
            Board[NewPosition.x, NewPosition.y].hasMoved = true;
            Board[OldPosition.x, OldPosition.y].hasMoved = false;

            UpdateFigures();
        }
        GameLoop.Main.NewPositionSelected(NewPosition);
    }

    public void BuildBuilding(Vector2Int NewPosition, ChessBuiding Building, int NewID)
    {
        Board[NewPosition.x, NewPosition.y].building = Building;
        Board[NewPosition.x, NewPosition.y].buildingTransform = null;
        Board[NewPosition.x, NewPosition.y].ownerID = NewID;
        UpdateFigures();
    }

    public void SpawnFigure(Vector2Int NewPosition, ChessFigure Figure, int NewID)
    {
        Board[NewPosition.x, NewPosition.y].figure = Figure;
        Board[NewPosition.x, NewPosition.y].figureTransform = null;
        Board[NewPosition.x, NewPosition.y].ownerID = NewID;
        UpdateFigures();
    }
}