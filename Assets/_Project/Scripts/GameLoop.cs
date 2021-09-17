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

    public TMP_Text turnCountText;
    public Tilemap tilemap;
    public Tile[] tiles;
    public VisualEffect destroyEffect;
    public Gradient[] colors;

    private List<Vector2Int> validPositions = new List<Vector2Int>();
    private Vector2Int selectedPosition;

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
    }

    public void  NewPositionSelected(Vector2Int Position)
    {
        if (validPositions.Count == 0 && GameManager.Main.Board[Position.x, Position.y].figure != ChessFigure.Empty)
        {
            if (GameManager.Main.Board[Position.x, Position.y].ownerID == GameManager.Main.localPlayerID && GameManager.Main.turnID == GameManager.Main.localPlayerID)
            {
                validPositions = FigureMovement.GetValidPositions(GameManager.Main.localPlayerID, GameManager.Main.Board[Position.x, Position.y], Position);

                tilemap.ClearAllTiles();
                tilemap.SetTile((Vector3Int)Position, tiles[0]);
                for (int i = 0; i < validPositions.Count; i++)
                {
                    if (GameManager.Main.Board[validPositions[i].x, validPositions[i].y].figure != ChessFigure.Empty)
                    {
                        tilemap.SetTile((Vector3Int)validPositions[i], tiles[4]);
                    }
                    else
                    {
                        tilemap.SetTile((Vector3Int)validPositions[i], tiles[1]);
                    }
                }

                selectedPosition = Position;
            }
        }
        else if (validPositions.Contains(Position) && GameManager.Main.turnPointsLeft - GameManager.Main.ChessGameSettings.GetMoveCost(GameManager.Main.Board[selectedPosition.x, selectedPosition.y].figure) >= 0)
        {
            Debug.Log("TP: " + GameManager.Main.turnPointsLeft);
            PhotonView.Get(this).RpcSecure("MoveFigure", RpcTarget.AllBufferedViaServer, false, (Vector2)selectedPosition, (Vector2)Position);
            GameManager.Main.turnPointsLeft -= GameManager.Main.ChessGameSettings.GetMoveCost(GameManager.Main.Board[selectedPosition.x, selectedPosition.y].figure);
            if (GameManager.Main.turnPointsLeft <= 0)
            {
                FinishLocalTurn();
            }
            turnCountText.text = "TP: " + GameManager.Main.turnPointsLeft;
            validPositions = new List<Vector2Int>();
            tilemap.ClearAllTiles();
        }
        else if (validPositions.Contains(Position))
        {
            selectedPosition = Position;
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

    public void FinishLocalTurn()
    {
        GameManager.Main.turnPointsLeft = GameManager.Main.ChessGameSettings.MaxMovePoints;
        turnCountText.text = "TP: " + GameManager.Main.turnPointsLeft;
        PhotonView.Get(this).RpcSecure("FinishTurn", RpcTarget.AllBufferedViaServer, false);
    }

    public void MovePiece(Vector2Int OldPosition, Vector2Int NewPosition)
    {
        if (GameManager.Main.Board[NewPosition.x, NewPosition.y].figureTransform != null)
        {
            Destroy(GameManager.Main.Board[NewPosition.x, NewPosition.y].figureTransform.gameObject);
            destroyEffect.transform.position = new Vector3(NewPosition.x, 0, NewPosition.y);
            destroyEffect.SetGradient("Gradient", colors[GameManager.Main.Board[NewPosition.x, NewPosition.y].ownerID]);
            destroyEffect.Play();
        }

        GameManager.Main.Board[NewPosition.x, NewPosition.y] = GameManager.Main.Board[OldPosition.x, OldPosition.y];
        GameManager.Main.Board[NewPosition.x, NewPosition.y].hasMoved = true;
        GameManager.Main.Board[OldPosition.x, OldPosition.y] = new TileInfo(-1, ChessFigure.Empty, null, false);

        validPositions = new List<Vector2Int>();
        selectedPosition = Vector2Int.zero;

        GameManager.UpdateFigures();
    }
}
