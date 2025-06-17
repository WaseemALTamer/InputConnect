using Avalonia.Media;
using Avalonia;



namespace InputConnect.Setting{
    static class Themes{

        public static LinearGradientBrush Backgruond = new LinearGradientBrush{
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
            GradientStops = new GradientStops{
                new GradientStop { Color = Color.FromUInt32(0xff001357), Offset = 0.0 }, // top left
                new GradientStop { Color = Color.FromUInt32(0xff001357), Offset = 0.2 }, // top left section
                new GradientStop { Color = Color.FromUInt32(0xff173bb6), Offset = 0.5 }, // middle (lighter)
                new GradientStop { Color = Color.FromUInt32(0xff001357), Offset = 0.8 }, // bottom right section
                new GradientStop { Color = Color.FromUInt32(0xff001357), Offset = 1.0 }, // bottom right
            }
        };

        public static SolidColorBrush Holders = new SolidColorBrush(Color.FromUInt32(0x7f1f1f1f));
        public static SolidColorBrush DimOverlay = new SolidColorBrush(Color.FromUInt32(0x7f1f1f1f));
        public static SolidColorBrush InWindowPopup = new SolidColorBrush(Color.FromUInt32(0xff1a1a1a));
        public static SolidColorBrush Advertisement = new SolidColorBrush(Color.FromUInt32(0x7f1f1f1f));
        public static SolidColorBrush Button = new SolidColorBrush(Color.FromUInt32(0x7f2f2f2f));
        public static SolidColorBrush ConfigProperty = new SolidColorBrush(Color.FromUInt32(0x7f1f1f1f));
        public static SolidColorBrush Test = new SolidColorBrush(Color.FromUInt32(0xffffffff));
        public static SolidColorBrush Entry = new SolidColorBrush(Color.FromUInt32(0xff000000));
        public static SolidColorBrush WrongToken = new SolidColorBrush(Color.FromUInt32(0xff990000));
        public static SolidColorBrush Connection = new SolidColorBrush(Color.FromUInt32(0x7f1f1f1f));

    }
}