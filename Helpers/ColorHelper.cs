using System;
using System.Drawing;

namespace DoublonManager.Helpers
{
    public static class ColorHelper
    {
        public static Color Primary = ColorTranslator.FromHtml("#2C3E50");
        public static Color Success = ColorTranslator.FromHtml("#27AE60");
        public static Color Warning = ColorTranslator.FromHtml("#F39C12");
        public static Color Danger = ColorTranslator.FromHtml("#E74C3C");
        public static Color Neutral = ColorTranslator.FromHtml("#ECF0F1");
        public static Color Blue = ColorTranslator.FromHtml("#3498DB");
        public static Color Gray = ColorTranslator.FromHtml("#95A5A6");
        
        // Nuances pour les interactions
        public static Color BlueDark = ColorTranslator.FromHtml("#2980B9");
        public static Color GrayDark = ColorTranslator.FromHtml("#7F8C8D");
    }
}
