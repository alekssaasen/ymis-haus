using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EconomySystem
{
    public static int Money;

    public static void Initialize()
    {
        Money = GameManager.GameSettingsInUse.StartingGold;
    }

    public static void CalculateMoneyForTurn()
    {
        if (GameManager.Main.localPlayerID == GameManager.Main.turnID)
        {
            Money += GameManager.GameSettingsInUse.GoldPerTurn;
            for (int x = 0; x < GameManager.Main.Board.GetLength(0); x++)
            {
                for (int y = 0; y < GameManager.Main.Board.GetLength(1); y++)
                {
                    if (GameManager.Main.Board[x, y].ownerID == GameManager.Main.localPlayerID)
                    {
                        switch (GameManager.Main.Board[x, y].building)
                        {
                            case ChessBuiding.Farm:
                                Money += GameManager.GameSettingsInUse.GoldPerFarm;
                                break;

                            case ChessBuiding.Mine:
                                Money += GameManager.GameSettingsInUse.GoldPerGoldMine;
                                break;
                        }
                    }
                }
            }
        }
    }

    public static bool CheckFigurePrice(ChessFigure Figure, out int Price)
    {
        Price = 0;
        switch (Figure)
        {
            case ChessFigure.Queen:
                Price = GameManager.GameSettingsInUse.QueenSpawnCost;
                break;

            case ChessFigure.Bishop:
                Price = GameManager.GameSettingsInUse.BishopSpawnCost;
                break;

            case ChessFigure.Knight:
                Price = GameManager.GameSettingsInUse.KnightSpawnCost;
                break;

            case ChessFigure.Rook:
                Price = GameManager.GameSettingsInUse.RookSpawnCost;
                break;

            case ChessFigure.Pawn:
                Price = GameManager.GameSettingsInUse.PawnSpawnCost;
                break;

        }
        return Money >= Price;
    }

    public static bool CheckBuildingPrice(ChessBuiding Building, out int Price)
    {
        Price = 0;
        switch (Building)
        {
            case ChessBuiding.Farm:
                Price = GameManager.GameSettingsInUse.FarmCreationCost;
                break;

            case ChessBuiding.Barracks:
                Price = GameManager.GameSettingsInUse.BarracksCreationCost;
                break;
        }
        return Money >= Price;
    }
    public static bool CanBuyBuilding()
    {
        return Money >= Mathf.Min(GameManager.GameSettingsInUse.FarmCreationCost, GameManager.GameSettingsInUse.BarracksCreationCost);
    }
}
