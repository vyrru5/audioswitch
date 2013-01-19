using System;
using System.Windows.Forms;

namespace AudioSwitch
{
    public partial class LedBar : UserControl
    {
        private readonly Label[] LEDon;
        private int lastValue;

        public LedBar()
        {
            InitializeComponent();
            LEDon = new[] { new Label(), lon1, lon2, lon3, lon4, lon5, lon6, lon7, lon8, lon9, lon10, lon11, lon12, lon13 };
        }

        public void SetValue(float value)
        {
            var val = (int)Math.Ceiling(value * 14);
            if (lastValue == val) return;
            lastValue = val;

            for (var i = 0; i < 14; i++)
                LEDon[i].Visible = val >= i;
        }
    }
}
