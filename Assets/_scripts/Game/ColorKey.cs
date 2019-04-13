using System;
using UnityEngine;

namespace Game
{
    public static class ColorKey
    {
        // UI Color Palette
        public static Color32 UIStarTBD = new Color32(255, 255, 255, 150);
        public static Color32 UIStarSuccess = new Color32(255, 255, 0, 255);
        public static Color32 UITextInactive = UIStarTBD;

        public static Color GetBuildingColor(Building.BuildingColors buildingColor)
        {
            switch (buildingColor)
            {
                case Building.BuildingColors.Red:
                    return new Color32(163, 108, 110, 255);
                case Building.BuildingColors.Green:
                    return new Color32(127, 184, 138, 255);
                case Building.BuildingColors.Blue:
                    return new Color32(130, 169, 179, 255);
                case Building.BuildingColors.Yellow:
                    return new Color32(255, 165, 0, 255);
                case Building.BuildingColors.Purple:
                    return new Color32(150, 125, 171, 255);
                case Building.BuildingColors.Orange:
                    return new Color32(196, 165, 114, 255);
                case Building.BuildingColors.Pink:
                    return new Color32(191, 119, 162, 255);
                default:
                    throw new ArgumentOutOfRangeException(nameof(buildingColor), buildingColor, null);
            }
        }
    }
}