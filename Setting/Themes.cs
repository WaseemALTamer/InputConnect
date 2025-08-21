using Avalonia.Media;
using Avalonia;



namespace InputConnect.Setting{
    static class Themes{

        // this is the defult background
        public static LinearGradientBrush Backgruond = new LinearGradientBrush{
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
            GradientStops = new GradientStops{
                new GradientStop { Color = Color.FromUInt32(0xff001357), Offset = 0.0 },
                new GradientStop { Color = Color.FromUInt32(0xff001357), Offset = 0.2 },
                new GradientStop { Color = Color.FromUInt32(0xff173bb6), Offset = 0.5 },
                new GradientStop { Color = Color.FromUInt32(0xff001357), Offset = 0.8 },
                new GradientStop { Color = Color.FromUInt32(0xff001357), Offset = 1.0 },
            }
        };

        public static SolidColorBrush Page = new SolidColorBrush(Color.FromUInt32(0x7f1f1f1f));
        public static SolidColorBrush Holder = new SolidColorBrush(Color.FromUInt32(0x7f1f1f1f));
        public static SolidColorBrush DimOverlay = new SolidColorBrush(Color.FromUInt32(0x7f1f1f1f));
        public static SolidColorBrush InWindowPopup = new SolidColorBrush(Color.FromUInt32(0xff1a1a1a));
        public static SolidColorBrush Advertisement = new SolidColorBrush(Color.FromUInt32(0x7f1f1f1f));
        public static SolidColorBrush Button = new SolidColorBrush(Color.FromUInt32(0x7f2f2f2f));
        public static SolidColorBrush ConfigProperty = new SolidColorBrush(Color.FromUInt32(0x7f1f1f1f));
        public static SolidColorBrush Test = new SolidColorBrush(Color.FromUInt32(0xffffffff));
        public static SolidColorBrush Entry = new SolidColorBrush(Color.FromUInt32(0xff000000));
        public static SolidColorBrush WrongToken = new SolidColorBrush(Color.FromUInt32(0xff990000));
        public static SolidColorBrush Connection = new SolidColorBrush(Color.FromUInt32(0x7f1f1f1f));
        public static SolidColorBrush Text = new SolidColorBrush(Color.FromUInt32(0xffffffff));

        public static SolidColorBrush MonitorEdges = new SolidColorBrush(Color.FromUInt32(0xff000000));

        
        public static SolidColorBrush Timer = new SolidColorBrush(Color.FromUInt32(0xffffffff));



        public static LinearGradientBrush[] Backgrounds = new LinearGradientBrush[]{
            // 1. Blue
            new LinearGradientBrush {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops = new GradientStops {
                    new GradientStop { Color = Color.FromUInt32(0xff001357), Offset = 0.0 },
                    new GradientStop { Color = Color.FromUInt32(0xff001357), Offset = 0.2 },
                    new GradientStop { Color = Color.FromUInt32(0xff173bb6), Offset = 0.5 },
                    new GradientStop { Color = Color.FromUInt32(0xff001357), Offset = 0.8 },
                    new GradientStop { Color = Color.FromUInt32(0xff001357), Offset = 1.0 },
                }
            },

            // 2. Green
            new LinearGradientBrush {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops = new GradientStops {
                    new GradientStop { Color = Color.FromUInt32(0xff003300), Offset = 0.0 },
                    new GradientStop { Color = Color.FromUInt32(0xff003300), Offset = 0.2 },
                    new GradientStop { Color = Color.FromUInt32(0xff00b347), Offset = 0.5 },
                    new GradientStop { Color = Color.FromUInt32(0xff003300), Offset = 0.8 },
                    new GradientStop { Color = Color.FromUInt32(0xff003300), Offset = 1.0 },
                }
            },

            // 3. Orange
            new LinearGradientBrush {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops = new GradientStops {
                    new GradientStop { Color = Color.FromUInt32(0xff4d1f00), Offset = 0.0 },
                    new GradientStop { Color = Color.FromUInt32(0xff4d1f00), Offset = 0.2 },
                    new GradientStop { Color = Color.FromUInt32(0xffff8000), Offset = 0.5 },
                    new GradientStop { Color = Color.FromUInt32(0xff4d1f00), Offset = 0.8 },
                    new GradientStop { Color = Color.FromUInt32(0xff4d1f00), Offset = 1.0 },
                }
            },

            // 4. Red
            new LinearGradientBrush {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops = new GradientStops {
                    new GradientStop { Color = Color.FromUInt32(0xff330000), Offset = 0.0 },
                    new GradientStop { Color = Color.FromUInt32(0xff330000), Offset = 0.2 },
                    new GradientStop { Color = Color.FromUInt32(0xffcc0000), Offset = 0.5 },
                    new GradientStop { Color = Color.FromUInt32(0xff330000), Offset = 0.8 },
                    new GradientStop { Color = Color.FromUInt32(0xff330000), Offset = 1.0 },
                }
            },

            // 5. Purple
            new LinearGradientBrush {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops = new GradientStops {
                    new GradientStop { Color = Color.FromUInt32(0xff1a0033), Offset = 0.0 },
                    new GradientStop { Color = Color.FromUInt32(0xff1a0033), Offset = 0.2 },
                    new GradientStop { Color = Color.FromUInt32(0xff8000ff), Offset = 0.5 },
                    new GradientStop { Color = Color.FromUInt32(0xff1a0033), Offset = 0.8 },
                    new GradientStop { Color = Color.FromUInt32(0xff1a0033), Offset = 1.0 },
                }
            },

            // 6. Pink
            new LinearGradientBrush {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops = new GradientStops {
                    new GradientStop { Color = Color.FromUInt32(0xff33001a), Offset = 0.0 },
                    new GradientStop { Color = Color.FromUInt32(0xff33001a), Offset = 0.2 },
                    new GradientStop { Color = Color.FromUInt32(0xffff3399), Offset = 0.5 },
                    new GradientStop { Color = Color.FromUInt32(0xff33001a), Offset = 0.8 },
                    new GradientStop { Color = Color.FromUInt32(0xff33001a), Offset = 1.0 },
                }
            },

            // 7. Cyan
            new LinearGradientBrush {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops = new GradientStops {
                    new GradientStop { Color = Color.FromUInt32(0xff003333), Offset = 0.0 },
                    new GradientStop { Color = Color.FromUInt32(0xff003333), Offset = 0.2 },
                    new GradientStop { Color = Color.FromUInt32(0xff00e6e6), Offset = 0.5 },
                    new GradientStop { Color = Color.FromUInt32(0xff003333), Offset = 0.8 },
                    new GradientStop { Color = Color.FromUInt32(0xff003333), Offset = 1.0 },
                }
            },

            // 8. Yellow
            new LinearGradientBrush {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops = new GradientStops {
                    new GradientStop { Color = Color.FromUInt32(0xff333300), Offset = 0.0 },
                    new GradientStop { Color = Color.FromUInt32(0xff333300), Offset = 0.2 },
                    new GradientStop { Color = Color.FromUInt32(0xffffff00), Offset = 0.5 },
                    new GradientStop { Color = Color.FromUInt32(0xff333300), Offset = 0.8 },
                    new GradientStop { Color = Color.FromUInt32(0xff333300), Offset = 1.0 },
                }
            },

            // 9. Teal
            new LinearGradientBrush {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops = new GradientStops {
                    new GradientStop { Color = Color.FromUInt32(0xff002633), Offset = 0.0 },
                    new GradientStop { Color = Color.FromUInt32(0xff002633), Offset = 0.2 },
                    new GradientStop { Color = Color.FromUInt32(0xff009999), Offset = 0.5 },
                    new GradientStop { Color = Color.FromUInt32(0xff002633), Offset = 0.8 },
                    new GradientStop { Color = Color.FromUInt32(0xff002633), Offset = 1.0 },
                }
            },

            // 10. Gold
            new LinearGradientBrush {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops = new GradientStops {
                    new GradientStop { Color = Color.FromUInt32(0xff332600), Offset = 0.0 },
                    new GradientStop { Color = Color.FromUInt32(0xff332600), Offset = 0.2 },
                    new GradientStop { Color = Color.FromUInt32(0xffffcc00), Offset = 0.5 },
                    new GradientStop { Color = Color.FromUInt32(0xff332600), Offset = 0.8 },
                    new GradientStop { Color = Color.FromUInt32(0xff332600), Offset = 1.0 },
                }
            }
        };
    }
}