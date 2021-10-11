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

    private static int CheckFigurePrice(ChessFigure Figure)
    {
        int price = 0;
        switch (Figure)
        {
            case ChessFigure.Queen:
                price = GameManager.GameSettingsInUse.QueenSpawnCost;
                break;

            case ChessFigure.Bishop:
                price = GameManager.GameSettingsInUse.BishopSpawnCost;
                break;

            case ChessFigure.Knight:
                price = GameManager.GameSettingsInUse.KnightSpawnCost;
                break;

            case ChessFigure.Rook:
                price = GameManager.GameSettingsInUse.RookSpawnCost;
                break;

            case ChessFigure.Pawn:
                price = GameManager.GameSettingsInUse.PawnSpawnCost;
                break;

        }
        return Mathf.RoundToInt((float)price * FigureMultiplayer(Figure, GameManager.Main.localPlayerID));
    }

    private static int CheckBuildingPrice(ChessBuiding Building)
    {
        int price = 0;
        switch (Building)
        {
            case ChessBuiding.Farm:
                price = GameManager.GameSettingsInUse.FarmCreationCost;
                break;

            case ChessBuiding.Barracks:
                price = GameManager.GameSettingsInUse.BarracksCreationCost;
                break;
        }
        return Mathf.RoundToInt((float)price * BuildingMultiplayer(Building, GameManager.Main.localPlayerID));
    }

    public static bool CanBuyBuilding(ChessBuiding Building, out string ErrorMessage, out int Price)
    {
        if (GameManager.Main.turnPointsLeft - GameManager.GameSettingsInUse.BuildingBuildTurnCost >= 0)
        {
            switch (Building)
            {
                case ChessBuiding.Farm:
                    Price = CheckBuildingPrice(Building);
                    if (Price <= Money)
                    {
                        ErrorMessage = "";
                        return true;
                    }
                    else
                    {
                        ErrorMessage = "Not enough money";
                        return false;
                    }

                case ChessBuiding.Barracks:
                    Price = CheckBuildingPrice(Building);
                    if (Price <= Money)
                    {
                        ErrorMessage = "";
                        return true;
                    }
                    else
                    {
                        ErrorMessage = "Not enough money";
                        return false;
                    }

                default:
                    Price = 0;
                    ErrorMessage = "<BuildingNotAvailable>";
                    return false;
            }
        }
        else
        {
            Price = 0;
            ErrorMessage = "Not enough turn Points";
            return false;
        }
    }

    public static bool CanBuyFigure(ChessFigure Figure, out string ErrorMessage, out int Price)
    {
        Price = CheckFigurePrice(Figure);
        if (Price <= Money)
        {
            if (GameManager.Main.turnPointsLeft - GameManager.GameSettingsInUse.FigureSpawnTurnCost >= 0)
            {
                ErrorMessage = "";
                return true;
            }
            else
            {
                ErrorMessage = "Not enough turn Points";
                return false;
            }
        }
        else
        {
            ErrorMessage = "Not enough money";
            return false;
        }
    }

    private static float FigureMultiplayer(ChessFigure Figure, int ID)
    {
        float multiplayer = 1;
        int figurecount = 0;

        for (int x = 0; x < GameManager.Main.Board.GetLength(0); x++)
        {
            for (int y = 0; y < GameManager.Main.Board.GetLength(0); y++)
            {
                if (GameManager.Main.Board[x, y].figure == Figure && GameManager.Main.Board[x, y].ownerID == ID)
                {
                    figurecount++;
                }
            }
        }

        if (GameManager.GameSettingsInUse.AlternativeMultiplayerFormula)
        {
            multiplayer = Mathf.Pow(GameManager.GameSettingsInUse.FigureCostMultiplayer, figurecount);
        }
        else
        {

        }

        return multiplayer;
    }

    private static float BuildingMultiplayer(ChessBuiding Buiding, int ID)
    {
        float multiplayer = 1;
        int buildingcount = 0;

        for (int x = 0; x < GameManager.Main.Board.GetLength(0); x++)
        {
            for (int y = 0; y < GameManager.Main.Board.GetLength(0); y++)
            {
                if (GameManager.Main.Board[x,y].building == Buiding && GameManager.Main.Board[x, y].ownerID == ID)
                {
                    buildingcount++;
                }
            }
        }

        if (GameManager.GameSettingsInUse.AlternativeMultiplayerFormula)
        {
            multiplayer = Mathf.Pow(GameManager.GameSettingsInUse.BuildingCostMultiplayer, buildingcount);
        }
        else
        {

        }

        return multiplayer;
    }
}
