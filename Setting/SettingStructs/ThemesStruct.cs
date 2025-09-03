using System.Text.Json.Serialization;
using Avalonia.Media;
using Avalonia;


namespace InputConnect.SettingStruct
{
    public class ThemesStruct
    {
        // The default background theme
        [JsonPropertyName("Background")]
        public LinearGradientBrush Background { get; set; } = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
            GradientStops = new GradientStops
            {
                new GradientStop { Color = Color.FromUInt32(0xff001357), Offset = 0.0 },
                new GradientStop { Color = Color.FromUInt32(0xff001357), Offset = 0.2 },
                new GradientStop { Color = Color.FromUInt32(0xff173bb6), Offset = 0.5 },
                new GradientStop { Color = Color.FromUInt32(0xff001357), Offset = 0.8 },
                new GradientStop { Color = Color.FromUInt32(0xff001357), Offset = 1.0 },
            }
        };

        [JsonPropertyName("Page")]
        public SolidColorBrush Page { get; set; } = new SolidColorBrush(Color.FromUInt32(0x7f1f1f1f));

        [JsonPropertyName("Holder")]
        public SolidColorBrush Holder { get; set; } = new SolidColorBrush(Color.FromUInt32(0x7f1f1f1f));

        [JsonPropertyName("DimOverlay")]
        public SolidColorBrush DimOverlay { get; set; } = new SolidColorBrush(Color.FromUInt32(0x7f1f1f1f));

        [JsonPropertyName("InWindowPopup")]
        public SolidColorBrush InWindowPopup { get; set; } = new SolidColorBrush(Color.FromUInt32(0xff1a1a1a));

        [JsonPropertyName("Advertisement")]
        public SolidColorBrush Advertisement { get; set; } = new SolidColorBrush(Color.FromUInt32(0x7f1f1f1f));

        [JsonPropertyName("Button")]
        public SolidColorBrush Button { get; set; } = new SolidColorBrush(Color.FromUInt32(0x7f2f2f2f));

        [JsonPropertyName("ConfigProperty")]
        public SolidColorBrush ConfigProperty { get; set; } = new SolidColorBrush(Color.FromUInt32(0x7f1f1f1f));

        [JsonPropertyName("Test")]
        public SolidColorBrush Test { get; set; } = new SolidColorBrush(Color.FromUInt32(0xffffffff));

        [JsonPropertyName("Entry")]
        public SolidColorBrush Entry { get; set; } = new SolidColorBrush(Color.FromUInt32(0xff000000));

        [JsonPropertyName("WrongToken")]
        public SolidColorBrush WrongToken { get; set; } = new SolidColorBrush(Color.FromUInt32(0xff990000));

        [JsonPropertyName("Connection")]
        public SolidColorBrush Connection { get; set; } = new SolidColorBrush(Color.FromUInt32(0x7f1f1f1f));

        [JsonPropertyName("Text")]
        public SolidColorBrush Text { get; set; } = new SolidColorBrush(Color.FromUInt32(0xffffffff));

        [JsonPropertyName("MonitorEdges")]
        public SolidColorBrush MonitorEdges { get; set; } = new SolidColorBrush(Color.FromUInt32(0xff000000));

        [JsonPropertyName("Timer")]
        public SolidColorBrush Timer { get; set; } = new SolidColorBrush(Color.FromUInt32(0xffffffff));

        [JsonPropertyName("TrashBin")]
        public SolidColorBrush TrashBin { get; set; } = new SolidColorBrush(Color.FromUInt32(0xff992726));

        [JsonPropertyName("DropDown")]
        public SolidColorBrush DropDown { get; set; } = new SolidColorBrush(Color.FromUInt32(0xff000000));

        [JsonPropertyName("WaterMark")]
        public SolidColorBrush WaterMark { get; set; } = new SolidColorBrush(Color.FromUInt32(0xff888888));

        [JsonPropertyName("Offline")]
        public SolidColorBrush Offline { get; set; } = new SolidColorBrush(Color.FromUInt32(0x55ffffff));

        [JsonPropertyName("Online")]
        public SolidColorBrush Online { get; set; } = new SolidColorBrush(Color.FromUInt32(0xff00ff00));

        [JsonPropertyName("Connected")]
        public SolidColorBrush Connected { get; set; } = new SolidColorBrush(Color.FromUInt32(0xff00ff00));

        [JsonPropertyName("Pending")]
        public SolidColorBrush Pending { get; set; } = new SolidColorBrush(Color.FromUInt32(0xffffff00));

        // All gradient backgrounds
        [JsonPropertyName("Backgrounds")]
        public LinearGradientBrush[] Backgrounds { get; set; } = new LinearGradientBrush[]{
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
