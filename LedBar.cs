using System;
using System.Drawing;
using System.Windows.Forms;

namespace AudioSwitch
{
    internal partial class LedBar : UserControl
    {
        private readonly Label[] LED;
        private int lastValue;

        internal LedBar()
        {
            InitializeComponent();
            LED = new[] { new Label(), led1, led2, led3, led4, led5, led6, led7, led8, led9, led10, led11, led12, led13 };
        }

        internal void SetValue(float value)
        {
            var val = (int)Math.Ceiling(value * 14);
            if (lastValue == val) return;
            lastValue = val;

            for (var i = 0; i < 14; i++)
                LED[i].BackColor = val >= i ? SystemColors.ControlDarkDark : SystemColors.ScrollBar;
        }
    }
}
