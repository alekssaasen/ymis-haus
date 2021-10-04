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
    public GameObject BackgroungButton;

    public CameraController cameraController;
    public Tilemap tilemap;
    public Tile[] tiles;
    public VisualEffect destroyEffect;
    public Gradient[] colors;

    public GameLoopState loopState;
    public ChessFigure figureSelected;
    public ChessBuiding buildingSelected;

    public Vector2Int newSelectedPosition = -Vector2Int.one;
    public Vector2Int oldSelectedPosition = -Vector2Int.one;

    public List<Vector2Int> validNewFigurePositions = new List<Vector2Int>();
    public List<Vector2Int> figuresThatCanMove = new List<Vector2Int>();
    public List<Vector2Int> buildingFoundations = new List<Vector2Int>();
    public List<Vector2Int> figureSpawnpoints = new List<Vector2Int>();

    public List<Vector2Int> ignoredFigures = new List<Vector2Int>();

    private void Awake()
    {
        if (PhotonNetwork.InRoom)
        {
            // Make GameLoop a singleton
            if (Main == null)
            {
                Main = this;
            }
            else
            {
                Main = this;
                Debug.LogWarning("There can only be one GameLoop!");
            }
        }
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
    }

    public void ChangeState(ChessFigure Figure, ChessBuiding Building)
    {
        if (GameManager.Main.turnID == GameManager.Main.localPlayerID)
        {
            if (Figure == ChessFigure.Empty && Building == ChessBuiding.Empty && figureSelected == ChessFigure.Empty && buildingSelected == ChessBuiding.Empty)
            {
                loopState = GameLoopState.Moving;
            }
            else if (Figure != ChessFigure.Empty && Building == ChessBuiding.Empty)
            {
                loopState = GameLoopState.Spawning;
                figureSelected = Figure;
                buildingSelected = Building;
            }
            else if (Figure == ChessFigure.Empty && Building != ChessBuiding.Empty)
            {
                loopState = GameLoopState.Building;
                figureSelected = Figure;
                buildingSelected = Building;
            }

            UpdateUI();
        }
    }

    public void NewPositionSelected(Vector2Int NewSelectedPosition)
    {
        if (GameManager.Main.turnID == GameManager.Main.localPlayerID)
        {
            oldSelectedPosition = newSelectedPosition;
            newSelectedPosition = NewSelectedPosition;

            switch (loopState)
            {
                case GameLoopState.Moving:
                    Move();
                    break;

                case GameLoopState.Building:
                    Build();
                    break;

                case GameLoopState.Spawning:
                    Spawn();
                    break;

                default:
                    break;
            }

            UpdateUI();
        }
    }

    private void Move()
    {
        if (figuresThatCanMove.Contains(newSelectedPosition) && newSelectedPosition != oldSelectedPosition && GameManager.Main.Board[newSelectedPosition.x, newSelectedPosition.y].figure != ChessFigure.Empty && GameManager.Main.Board[newSelectedPosition.x, newSelectedPosition.y].ownerID == GameManager.Main.localPlayerID)
        {
            validNewFigurePositions = FigureMovement.GetValidPositions(GameManager.Main.localPlayerID, GameManager.Main.Board[newSelectedPosition.x, newSelectedPosition.y], newSelectedPosition);
        }
        else
        {
            if (validNewFigurePositions.Contains(newSelectedPosition) && GameManager.Main.turnID == GameManager.Main.localPlayerID && GameManager.Main.turnPointsLeft - GameManager.GameSettingsInUse.GetMoveCost(GameManager.Main.Board[newSelectedPosition.x, newSelectedPosition.y].figure) >= 0)
            {
                PhotonView.Get(this).RpcSecure("MoveFigure", RpcTarget.AllBufferedViaServer, false, (Vector2)oldSelectedPosition, (Vector2)newSelectedPosition);
                GameManager.Main.turnPointsLeft -= GameManager.GameSettingsInUse.GetMoveCost(GameManager.Main.Board[oldSelectedPosition.x, oldSelectedPosition.y].figure);

                ignoredFigures.Add(newSelectedPosition);

                if (GameManager.Main.turnPointsLeft <= 0)
                {
                    FinishLocalTurn();
                }
            }
            if (newSelectedPosition == oldSelectedPosition)
            {
                newSelectedPosition = -Vector2Int.one;
            }
            validNewFigurePositions = new List<Vector2Int>();
        }
    }

    private void Build()
    {
        if (EconomySystem.CheckBuildingPrice(buildingSelected, out int price) && buildingFoundations.Contains(newSelectedPosition) && GameManager.Main.Board[newSelectedPosition.x, newSelectedPosition.y].building == ChessBuiding.Empty && GameManager.Main.Board[newSelectedPosition.x, newSelectedPosition.y].figure == ChessFigure.Empty)
        {
            EconomySystem.Money -= price;
            PhotonView.Get(this).RpcSecure("PlaceBuilding", RpcTarget.AllBufferedViaServer, false, (Vector2)newSelectedPosition, buildingSelected, GameManager.Main.localPlayerID);

            buildingSelected = ChessBuiding.Empty;
            ChangeState(ChessFigure.Empty, ChessBuiding.Empty);
        }
        else
        {
            buildingSelected = ChessBuiding.Empty;
            ChangeState(ChessFigure.Empty, ChessBuiding.Empty);
        }
    }

    private void Spawn()
    {
        if (EconomySystem.CheckFigurePrice(figureSelected, out int price) && figureSpawnpoints.Contains(newSelectedPosition) && GameManager.Main.Board[newSelectedPosition.x, newSelectedPosition.y].building == ChessBuiding.Empty && GameManager.Main.Board[newSelectedPosition.x, newSelectedPosition.y].figure == ChessFigure.Empty)
        {
            EconomySystem.Money -= price;
            PhotonView.Get(this).RpcSecure("SpawnFigure", RpcTarget.AllBufferedViaServer, false, (Vector2)newSelectedPosition, figureSelected, GameManager.Main.localPlayerID);
            if (!GameManager.GameSettingsInUse.FiguresCanMoveOnSpawn)
            {
                ignoredFigures.Add(newSelectedPosition);
            }

            figureSelected = ChessFigure.Empty;
            ChangeState(ChessFigure.Empty, ChessBuiding.Empty);
        }
        else
        {
            figureSelected = ChessFigure.Empty;
            ChangeState(ChessFigure.Empty, ChessBuiding.Empty);
        }
    }

    public void UpdateUI()
    {
        figuresThatCanMove = FigureMovement.GetMoveableFigures(GameManager.Main.localPlayerID);
        buildingFoundations = FigureBuilding.GetValidFoundations(GameManager.Main.localPlayerID);
        figureSpawnpoints = FigureBuilding.GetValidSpawnpoints(GameManager.Main.localPlayerID);

        for (int i = 0; i < figuresThatCanMove.Count; i++)
        {
            if (ignoredFigures.Contains(figuresThatCanMove[i]))
            {
                figuresThatCanMove.RemoveAt(i);
                i--;
            }
        }

        if (figuresThatCanMove.Count <= 0)
        {
            FinishLocalTurn();
        }

        UpdateTileset();
    }

    private void UpdateTileset()
    {
        tilemap.ClearAllTiles();
        if (figuresThatCanMove.Contains(newSelectedPosition) && GameManager.Main.Board[newSelectedPosition.x, newSelectedPosition.y].ownerID == GameManager.Main.localPlayerID && (GameManager.Main.Board[newSelectedPosition.x, newSelectedPosition.y].figure != ChessFigure.Empty || GameManager.Main.Board[newSelectedPosition.x, newSelectedPosition.y].building != ChessBuiding.Empty))
        {
            tilemap.SetTile((Vector3Int)newSelectedPosition, tiles[0]);
        }

        switch (loopState)
        {
            case GameLoopState.Moving:
                if (validNewFigurePositions.Count > 0)
                {
                    for (int i = 0; i < validNewFigurePositions.Count; i++)
                    {
                        if (GameManager.Main.Board[validNewFigurePositions[i].x, validNewFigurePositions[i].y].figure == ChessFigure.Empty && GameManager.Main.Board[validNewFigurePositions[i].x, validNewFigurePositions[i].y].building == ChessBuiding.Empty)
                        {
                            tilemap.SetTile(new Vector3Int(validNewFigurePositions[i].x, validNewFigurePositions[i].y, 0), tiles[2]);
                        }
                        else
                        {
                            tilemap.SetTile(new Vector3Int(validNewFigurePositions[i].x, validNewFigurePositions[i].y, 0), tiles[4]);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < figuresThatCanMove.Count; i++)
                    {
                        tilemap.SetTile(new Vector3Int(figuresThatCanMove[i].x, figuresThatCanMove[i].y, 0), tiles[0]);
                    }
                }
                break;

            case GameLoopState.Building:
                for (int i = 0; i < buildingFoundations.Count; i++)
                {
                    tilemap.SetTile(new Vector3Int(buildingFoundations[i].x, buildingFoundations[i].y, 0), tiles[3]);
                }
                break;

            case GameLoopState.Spawning:
                for (int i = 0; i < figureSpawnpoints.Count; i++)
                {
                    tilemap.SetTile(new Vector3Int(figureSpawnpoints[i].x, figureSpawnpoints[i].y, 0), tiles[7]);
                }
                break;
        }
    }



    public void StartLocalTurn()
    {
        if (GameManager.Main.turnID == GameManager.Main.localPlayerID)
        {
            EconomySystem.CalculateMoneyForTurn();
            GameManager.Main.turnPointsLeft = GameManager.GameSettingsInUse.MovePointsPerTurn;
            ignoredFigures = new List<Vector2Int>();

            tilemap.gameObject.SetActive(true);
        }
    }

    public void FinishLocalTurn()
    {
        if (GameManager.Main.turnID == GameManager.Main.localPlayerID)
        {
            PhotonView.Get(this).RpcSecure("FinishTurn", RpcTarget.AllBufferedViaServer, false);
            tilemap.gameObject.SetActive(false);
        }
    }
}

[System.Serializable]
public enum GameLoopState
{
    Moving, Building, Spawning
}
