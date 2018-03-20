using System.Collections.Generic;

namespace AbpCompanyName.AbpProjectName.Configuration.Ui
{
    public static class UiThemes
    {
        public static List<UiThemeInfo> All { get; }

        static UiThemes()
        {
            All = new List<UiThemeInfo>
            {
                new UiThemeInfo("Rojo", "red"),
                new UiThemeInfo("Rosado", "pink"),
                new UiThemeInfo("Morado", "purple"),
                new UiThemeInfo("Morado oscuro", "deep-purple"),
                new UiThemeInfo("Indigo", "indigo"),
                new UiThemeInfo("Azul", "blue"),
                new UiThemeInfo("Azul claro", "light-blue"),
                new UiThemeInfo("Cian", "cyan"),
                new UiThemeInfo("Verde azulado", "teal"),
                new UiThemeInfo("Verde", "green"),
                new UiThemeInfo("Verde claro", "light-green"),
                new UiThemeInfo("Limón", "lime"),
                new UiThemeInfo("Amarillo", "yellow"),
                new UiThemeInfo("Ambar", "amber"),
                new UiThemeInfo("Anaranjado", "orange"),
                new UiThemeInfo("Anaranjado oscuro", "deep-orange"),
                new UiThemeInfo("Marrón", "brown"),
                new UiThemeInfo("Gris", "grey"),
                new UiThemeInfo("Azul gris", "blue-grey"),
                new UiThemeInfo("Negro", "black")
            };
        }
    }
}
