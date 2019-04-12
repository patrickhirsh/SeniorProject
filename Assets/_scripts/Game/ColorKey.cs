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
                    return new Color(163, 108, 110);
                case Building.BuildingColors.Green:
                    return new Color(127, 184, 138);
                case Building.BuildingColors.Blue:
                    return new Color(130, 169, 179);
                case Building.BuildingColors.Yellow:
                    return new Color(255, 165, 0);
                case Building.BuildingColors.Purple:
                    return new Color(150, 125, 171);
                case Building.BuildingColors.Orange:
                    return new Color(196, 165, 114);
                case Building.BuildingColors.Pink:
                    return new Color(191, 119, 162);
                default:
                    throw new ArgumentOutOfRangeException(nameof(buildingColor), buildingColor, null);
            }
        }
    }
}