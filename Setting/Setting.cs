using InputConnect.SettingStruct;


namespace InputConnect
{
    public static class Setting{
        // this has been changed from being a namespace to being a public
        // static class which contains a  config structure  and the Theme
        // structure this enable us to save it and  load it on the run so
        // so we can keep the user prefrence on the go


        public static ConfigStruct Config = new ConfigStruct();
        public static ThemesStruct Themes = new ThemesStruct();

    }
}
