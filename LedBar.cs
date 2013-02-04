using System;
using System.Drawing;
using System.Windows.Forms;

namespace AudioSwitch
{
    public partial class LedBar : UserControl
    {
        private Color[] LEDoff;
        private Color[] LEDon;
        private readonly Label[] LED;
        private int lastValue;

        public LedBar()
        {
            InitializeComponent();
            LED = new[] { new Label(), lon1, lon2, lon3, lon4, lon5, lon6, lon7, lon8, lon9, lon10, lon11, lon12, lon13 };

            foreach (var control in LED)
                control.DoubleClick += (sender, args) => OnDoubleClick(args);
        }

        public void SetValue(float value)
        {
            var val = (int)Math.Ceiling(value * 14);
            if (lastValue == val) return;
            lastValue = val;

            for (var i = 0; i < 14; i++)
                LED[i].BackColor = val >= i ? LEDon[i] : LEDoff[i];
        }

        internal void SetColors(bool NewLEDs)
        {
            if (NewLEDs)
            {
                BackColor = SystemColors.Control;
                LEDoff = new[]
                    {
                        Color.Black,
                        SystemColors.ScrollBar, SystemColors.ScrollBar, SystemColors.ScrollBar,
                        SystemColors.ScrollBar, SystemColors.ScrollBar, SystemColors.ScrollBar,
                        SystemColors.ScrollBar, SystemColors.ScrollBar, SystemColors.ScrollBar,
                        SystemColors.ScrollBar, SystemColors.ScrollBar, SystemColors.ScrollBar,
                        SystemColors.ScrollBar
                    };
                LEDon = new[]
                    {
                        Color.Black,
                        SystemColors.ControlDarkDark, SystemColors.ControlDarkDark, SystemColors.ControlDarkDark,
                        SystemColors.ControlDarkDark, SystemColors.ControlDarkDark, SystemColors.ControlDarkDark,
                        SystemColors.ControlDarkDark, SystemColors.ControlDarkDark, SystemColors.ControlDarkDark,
                        SystemColors.ControlDarkDark, SystemColors.ControlDarkDark, SystemColors.ControlDarkDark,
                        SystemColors.ControlDarkDark
                    };
            }
            else
            {
                BackColor = Color.FromArgb(64, 64, 64);
                LEDoff = new[]
                    {
                        Color.Black,
                        Color.Green, Color.Green, Color.Green,
                        Color.Green, Color.Green, Color.Green,
                        Color.Green, Color.Green, Color.Green,
                        Color.Olive, Color.Olive, Color.Olive,
                        Color.Maroon
                    };
                LEDon = new[]
                    {
                        Color.Black,
                        Color.Lime, Color.Lime, Color.Lime,
                        Color.Lime, Color.Lime, Color.Lime,
                        Color.Lime, Color.Lime, Color.Lime,
                        Color.Yellow, Color.Yellow, Color.Yellow,
                        Color.Red
                    };
            }
            lastValue = 1;
            SetValue(0);
        }
    }
}
