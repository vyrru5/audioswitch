using System;
using System.Drawing;
using System.Windows.Forms;

namespace AudioSwitch
{
    public class VerticalProgressBar : ProgressBar
    {
        public VerticalProgressBar()
        {
            SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (ProgressBarRenderer.IsSupported)
            {
                var barHeight = Math.Min(Value * Height / 100, Height);
                ProgressBarRenderer.DrawVerticalChunks(e.Graphics,
                                                       new Rectangle(0, Height - barHeight, Width, barHeight));
            }
            base.OnPaint(e);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.Style |= 0x04;
                return cp;
            }
        }
    }
}
