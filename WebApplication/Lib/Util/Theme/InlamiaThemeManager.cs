namespace WebApplication.Lib.Util.Theme
{
    public class InlamiaThemeManager
    {

        private ThemeName _themeName;

        public InlamiaThemeManager()
        {
            _themeName = new ThemeName(ThemeName.ThemeNames.Default);
        }

        public void SetTheme(ThemeName.ThemeNames themeNames)
        {
            _themeName.SetThemeName(themeNames);
        }

        public ThemeName GetTheme()
        {
            return _themeName;
        }
    }
}