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

    [HideInInspector] public List<Vector2Int> validPositions = new List<Vector2Int>();
    [HideInInspector] public List<Vector2Int> moveableFigures = new List<Vector2Int>();
    [HideInInspector] public Vector2Int selectedPosition;

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
        Deselect();
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

    public void NewPositionSelected(Vector2Int NewSelectedPosition)
    {
        if (NewSelectedPosition == new Vector2Int(-1, -1))
        {
            Deselect();
        }
        else if (validPositions.Count == 0 && GameManager.Main.Board[NewSelectedPosition.x, NewSelectedPosition.y].figure != ChessFigure.Empty && moveableFigures.Contains(NewSelectedPosition))
        {
            Select(NewSelectedPosition);
        }
        else if (validPositions.Contains(NewSelectedPosition) && GameManager.Main.turnPointsLeft - GameManager.Main.ChessGameSettings.GetMoveCost(GameManager.Main.Board[selectedPosition.x, selectedPosition.y].figure) >= 0)
        {
            Move(NewSelectedPosition);
        }
        else
        {
            Deselect();
        }
    }

    public void Deselect()
    {
        selectedPosition = -Vector2Int.one;
        validPositions = new List<Vector2Int>();
        moveableFigures = new List<Vector2Int>();
        tilemap.ClearAllTiles();

        for (int x = 0; x < GameManager.Main.Board.GetLength(0); x++)
        {
            for (int y = 0; y < GameManager.Main.Board.GetLength(1); y++)
            {
                if (GameManager.Main.Board[x, y].figure != ChessFigure.Empty && GameManager.Main.Board[x, y].ownerID == GameManager.Main.localPlayerID)
                {
                    List<Vector2Int> validpos = FigureMovement.GetValidPositions(GameManager.Main.localPlayerID, GameManager.Main.Board[x, y], new Vector2Int(x, y));
                    if (validpos.Count > 0)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), tiles[0]);
                        moveableFigures.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
    }

    private void Select(Vector2Int NewSelectedPosition)
    {
        if (GameManager.Main.Board[NewSelectedPosition.x, NewSelectedPosition.y].ownerID == GameManager.Main.localPlayerID && GameManager.Main.turnID == GameManager.Main.localPlayerID)
        {
            validPositions = FigureMovement.GetValidPositions(GameManager.Main.localPlayerID, GameManager.Main.Board[NewSelectedPosition.x, NewSelectedPosition.y], NewSelectedPosition);

            tilemap.ClearAllTiles();
            tilemap.SetTile((Vector3Int)NewSelectedPosition, tiles[0]);
            for (int i = 0; i < validPositions.Count; i++)
            {
                if (GameManager.Main.Board[validPositions[i].x, validPositions[i].y].figure != ChessFigure.Empty)
                {
                    tilemap.SetTile((Vector3Int)validPositions[i], tiles[4]);
                }
                else
                {
                    tilemap.SetTile((Vector3Int)validPositions[i], tiles[2]);
                }
            }

            selectedPosition = NewSelectedPosition;
        }
    }

    private void Move(Vector2Int NewSelectedPosition)
    {
        PhotonView.Get(this).RpcSecure("MoveFigure", RpcTarget.AllBufferedViaServer, false, (Vector2)selectedPosition, (Vector2)NewSelectedPosition);
        GameManager.Main.turnPointsLeft -= GameManager.Main.ChessGameSettings.GetMoveCost(GameManager.Main.Board[selectedPosition.x, selectedPosition.y].figure);
        if (GameManager.Main.turnPointsLeft <= 0)
        {
            FinishLocalTurn();
        }
        turnCountText.text = "TP: " + GameManager.Main.turnPointsLeft;
        Deselect();
    }

    public void FinishLocalTurn()
    {
        if (GameManager.Main.turnID == GameManager.Main.localPlayerID)
        {
            GameManager.Main.turnPointsLeft = GameManager.Main.ChessGameSettings.MovePointsPerTurn;
            turnCountText.text = "TP: " + GameManager.Main.turnPointsLeft;
            PhotonView.Get(this).RpcSecure("FinishTurn", RpcTarget.AllBufferedViaServer, false);
        }
    }
}
