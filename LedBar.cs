using System;
using System.Drawing;
using System.Windows.Forms;

namespace AudioSwitch
{
    public partial class LedBar : UserControl
    {
        private readonly Label[] LED;
        private int lastValue;

        private static readonly Color[] pgOnColors = new[]
                                                  {
                                                      Color.Black, Color.Lime, Color.Lime, Color.Lime, Color.Lime,
                                                      Color.Lime, Color.Lime, Color.Lime, Color.Lime, Color.Lime,
                                                      Color.Yellow, Color.Yellow, Color.Yellow, Color.Red
                                                  };

        private static readonly Color[] pgOffColors = new[]
                                                   {
                                                       Color.Black, Color.Green, Color.Green, Color.Green, Color.Green,
                                                       Color.Green, Color.Green, Color.Green, Color.Green, Color.Green,
                                                       Color.Olive, Color.Olive, Color.Olive, Color.Maroon
                                                   };

        public LedBar()
        {
            InitializeComponent();
            LED = new[] { new Label(), l1, l2, l3, l4, l5, l6, l7, l8, l9, l10, l11, l12, l13 };
        }

        public void SetValue(float value)
        {
            var val = (int)Math.Ceiling(value * 14);
            if (lastValue == val) return;

            for (var i = 0; i < 14; i++)
                LED[i].BackColor = val >= i ? pgOnColors[i] : pgOffColors[i];
            lastValue = val;
        }
    }
}
