using System;
using UnityEngine;

namespace Game
{
    public static class ColorKey
    {
        public static Color GetColor(Building.BuildingColors buildingColor)
        {
            switch (buildingColor)
            {
                case Building.BuildingColors.Red:
                    return Color.red;
                case Building.BuildingColors.Green:
                    return Color.green;
                case Building.BuildingColors.Blue:
                    return Color.blue;
                case Building.BuildingColors.Yellow:
                    return Color.yellow;
                case Building.BuildingColors.Purple:
                    return new Color(128, 0, 128);
                case Building.BuildingColors.Orange:
                    return new Color(255, 165, 0);
                default:
                    throw new ArgumentOutOfRangeException(nameof(buildingColor), buildingColor, null);
            }
        }
    }
}