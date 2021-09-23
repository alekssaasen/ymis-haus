using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;
using TMPro;
using UnityEngine.VFX;

public class GameLoop : MonoBehaviour
{
    public static GameLoop Main;

    public CameraController cameraController;
    public TMP_Text turnCountText;
    public TMP_Text goldCountText;
    public TMP_Dropdown figureSelection;
    public TMP_Dropdown buildingSelection;
    public Tilemap tilemap;
    public Tile[] tiles;
    public VisualEffect destroyEffect;
    public Gradient[] colors;

    [SerializeField] private GameToolSelection gameTool;

    public Vector2Int selectedFigurePosition = -Vector2Int.one;
    public List<Vector2Int> validNewFigurePositions = new List<Vector2Int>();
    public List<Vector2Int> figuresThatCanMove = new List<Vector2Int>();
    public List<Vector2Int> buildingFoundations = new List<Vector2Int>();
    public List<Vector2Int> figureSpawnpoints = new List<Vector2Int>();

    private void Awake()
    {
        // Make GameLoop a singleton
        if (Main == null)
        {
            Main = this;
        }
        else
        {
            Destroy(this);
            Debug.LogWarning("There can only be one GameLoop!");
        }
        NewPositionSelected(-Vector2Int.one);
    }

    public void Update()
    {
        if (GameManager.Main.localPlayerID == GameManager.Main.turnID)
        {
            tilemap.gameObject.SetActive(true);
        }
        else
        {
            tilemap.gameObject.SetActive(false);
        }
        goldCountText.text = "Gold: " + EconomySystem.Money;
        turnCountText.text = "TP: " + GameManager.Main.turnPointsLeft;
    }

    public void UpdateTool(int Tool)
    {
        gameTool = (GameToolSelection)Tool;
        ResetUI();
    }



    public void NewPositionSelected(Vector2Int NewSelectedPosition)
    {
        switch (gameTool)
        {
            case GameToolSelection.Select:
                Select(NewSelectedPosition);
                break;

            case GameToolSelection.Move:
                Move(NewSelectedPosition);
                break;

            case GameToolSelection.Build:
                Build(NewSelectedPosition);
                break;

            case GameToolSelection.Spawn:
                Spawn(NewSelectedPosition);
                break;

            default:
                break;
        }

        UpdateUI();
    }



    private void Select(Vector2Int NewSelectedPosition)
    {
        if (NewSelectedPosition != -Vector2Int.one && figuresThatCanMove.Count != 0 && GameManager.Main.Board[NewSelectedPosition.x, NewSelectedPosition.y].ownerID == GameManager.Main.localPlayerID && GameManager.Main.turnID == GameManager.Main.localPlayerID && GameManager.Main.Board[NewSelectedPosition.x, NewSelectedPosition.y].figure != ChessFigure.Empty && figuresThatCanMove.Contains(NewSelectedPosition))
        {
            selectedFigurePosition = NewSelectedPosition;
            validNewFigurePositions = FigureMovement.GetValidPositions(GameManager.Main.localPlayerID, GameManager.Main.Board[NewSelectedPosition.x, NewSelectedPosition.y], NewSelectedPosition);
            gameTool = GameToolSelection.Move;
            figuresThatCanMove = new List<Vector2Int>();
            UpdateUI();
        }
        else if (figuresThatCanMove.Count == 0)
        {
            for (int x = 0; x < GameManager.Main.Board.GetLength(0); x++)
            {
                for (int y = 0; y < GameManager.Main.Board.GetLength(1); y++)
                {
                    if (GameManager.Main.Board[x, y].figure != ChessFigure.Empty && GameManager.Main.Board[x, y].ownerID == GameManager.Main.localPlayerID && FigureMovement.GetValidPositions(GameManager.Main.localPlayerID, GameManager.Main.Board[x, y], new Vector2Int(x, y)).Count > 0)
                    {
                        if (GameManager.GameSettingsInUse.GetMoveCost(GameManager.Main.Board[x, y].figure) <= GameManager.Main.turnPointsLeft)
                        {
                            figuresThatCanMove.Add(new Vector2Int(x, y));
                        }
                    }
                }
            }
        }
        else
        {
            ResetUI();
        }
    }

    private void Move(Vector2Int NewSelectedPosition)
    {
        if (validNewFigurePositions.Contains(NewSelectedPosition) && GameManager.Main.turnPointsLeft - GameManager.GameSettingsInUse.GetMoveCost(GameManager.Main.Board[selectedFigurePosition.x, selectedFigurePosition.y].figure) >= 0)
        {
            PhotonView.Get(this).RpcSecure("MoveFigure", RpcTarget.AllBufferedViaServer, false, (Vector2)selectedFigurePosition, (Vector2)NewSelectedPosition);
            GameManager.Main.turnPointsLeft -= GameManager.GameSettingsInUse.GetMoveCost(GameManager.Main.Board[selectedFigurePosition.x, selectedFigurePosition.y].figure);

            if (GameManager.Main.turnPointsLeft <= 0)
            {
                FinishLocalTurn();
            }
        }
        else
        {
            gameTool = GameToolSelection.Select;
            ResetUI();
        }
    }

    private void Build(Vector2Int NewSelectedPosition)
    {
        ChessBuiding NewChessBuiding = ChessBuiding.Empty;

        if (buildingSelection.value == 0)
        {
            NewChessBuiding = ChessBuiding.Farm;
        }
        else if(buildingSelection.value == 1)
        {
            NewChessBuiding = ChessBuiding.Barracks;
        }

        if (buildingFoundations.Count != 0)
        {
            if (EconomySystem.CheckBuildingPrice(NewChessBuiding, out int price) && buildingFoundations.Contains(NewSelectedPosition) && GameManager.Main.Board[NewSelectedPosition.x, NewSelectedPosition.y].building == ChessBuiding.Empty)
            {
                EconomySystem.Money -= price;
                PhotonView.Get(this).RpcSecure("PlaceBuilding", RpcTarget.AllBufferedViaServer, false, (Vector2)NewSelectedPosition, NewChessBuiding);
            }
            else
            {
                ResetUI();
            }
        }
        else if (buildingFoundations.Count == 0)
        {
            buildingFoundations = FigureBuilding.GetValidFoundations(GameManager.Main.localPlayerID, NewChessBuiding);
        }
        else
        {
            ResetUI();
        }
    }

    private void Spawn(Vector2Int NewSelectedPosition)
    {
        ChessFigure NewChessFigure = (ChessFigure)(2 + figureSelection.value);

        if (figureSpawnpoints.Count != 0)
        {
            if (EconomySystem.CheckFigurePrice(NewChessFigure, out int price) && figureSpawnpoints.Contains(NewSelectedPosition) && GameManager.Main.Board[NewSelectedPosition.x, NewSelectedPosition.y].building == ChessBuiding.Empty)
            {
                EconomySystem.Money -= price;
                PhotonView.Get(this).RpcSecure("SpawnFigure", RpcTarget.AllBufferedViaServer, false, (Vector2)NewSelectedPosition, NewChessFigure);
            }
            else
            {
                ResetUI();
            }
        }
        else if (figureSpawnpoints.Count == 0)
        {
            figureSpawnpoints = FigureBuilding.GetValidSpawnpoints(GameManager.Main.localPlayerID, NewChessFigure);
        }
        else
        {
            ResetUI();
        }
    }

    public void ResetUI()
    {
        selectedFigurePosition = -Vector2Int.one;
        validNewFigurePositions = new List<Vector2Int>();
        figuresThatCanMove = new List<Vector2Int>();
        buildingFoundations = new List<Vector2Int>();
        figureSpawnpoints = new List<Vector2Int>();

        NewPositionSelected(-Vector2Int.one);
    }

    public void UpdateUI()
    {
        tilemap.ClearAllTiles();
        if (selectedFigurePosition != -Vector2Int.one)
        {
            tilemap.SetTile((Vector3Int)selectedFigurePosition, tiles[0]);
        }

        for (int i = 0; i < validNewFigurePositions.Count; i++)
        {
            if (GameManager.Main.Board[validNewFigurePositions[i].x, validNewFigurePositions[i].y].figure == ChessFigure.Empty)
            {
                tilemap.SetTile(new Vector3Int(validNewFigurePositions[i].x, validNewFigurePositions[i].y, 0), tiles[2]);
            }
            else
            {
                tilemap.SetTile(new Vector3Int(validNewFigurePositions[i].x, validNewFigurePositions[i].y, 0), tiles[4]);
            }
        }

        for (int i = 0; i < figuresThatCanMove.Count; i++)
        {
            tilemap.SetTile(new Vector3Int(figuresThatCanMove[i].x, figuresThatCanMove[i].y, 0), tiles[0]);
        }

        for (int i = 0; i < buildingFoundations.Count; i++)
        {
            tilemap.SetTile(new Vector3Int(buildingFoundations[i].x, buildingFoundations[i].y, 0), tiles[3]);
        }

        for (int i = 0; i < figureSpawnpoints.Count; i++)
        {
            tilemap.SetTile(new Vector3Int(figureSpawnpoints[i].x, figureSpawnpoints[i].y, 0), tiles[7]);
        }
    }



    public void FinishLocalTurn()
    {
        if (GameManager.Main.turnID == GameManager.Main.localPlayerID)
        {
            EconomySystem.CalculateMoneyForTurn();
            GameManager.Main.turnPointsLeft = GameManager.GameSettingsInUse.MovePointsPerTurn;
            PhotonView.Get(this).RpcSecure("FinishTurn", RpcTarget.AllBufferedViaServer, false);
        }
    }

    public void test(int num)
    {
        Debug.Log(num);
    }
}

[System.Serializable]
public enum GameToolSelection
{
    Select, Move, Build, Spawn
}
