using System;
using System.Drawing;
using System.Windows.Forms;

namespace AudioSwitch
{
    public partial class ProgressTrackBar : UserControl
    {
        private readonly Label[] pgLeft;
        private readonly Label[] pgRight;

        private readonly Color[] pgOnColors;
        private readonly Color[] pgOffColors;

        public EventHandler TrackBarValueChanged;

        private Point pMousePosition = Point.Empty;
        private int _TrackBarValue;
        public bool Moving;

        public int Value
        {
            get
            {
                return _TrackBarValue;
            }
            set
            {
                _TrackBarValue = value;
                MoveThumb();
            }
        }

        public ProgressTrackBar()
        {
            InitializeComponent();

            pgLeft = new[] { l1, l2, l3, l4, l5, l6, l7, l8, l9, l10, l11, l12, l13 };
            pgRight = new[] { r1, r2, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r13 };
            pgOnColors = new[]
                             {
                                 Color.Lime, Color.Lime, Color.Lime, Color.Lime, Color.Lime,
                                 Color.Lime, Color.Lime, Color.Lime, Color.Lime, Color.Yellow,
                                 Color.Yellow, Color.Yellow, Color.Red
                             };

            pgOffColors = new[]
                             {
                                 Color.Green, Color.Green, Color.Green, Color.Green, Color.Green,
                                 Color.Green, Color.Green, Color.Green, Color.Green, Color.Olive,
                                 Color.Olive, Color.Olive, Color.Maroon
                             };
        }

        public void SetLeftChannel(byte value)
        {
            for (byte i = 0; i < 13; i++)
                pgLeft[i].BackColor = value >= i ? pgOnColors[i] : pgOffColors[i];
        }

        public void SetRightChannel(byte value)
        {
            for (byte i = 0; i < 13; i++)
                pgRight[i].BackColor = value >= i ? pgOnColors[i] : pgOffColors[i];
        }

        private void MoveThumb()
        {
            var trackDistance = ClientSize.Width - Beat.Width;
            var fractionMoved = (float)_TrackBarValue / 100;
            Beat.Left = (int)(fractionMoved * trackDistance);
        }

        private void btnThumb_MouseDown(object sender, MouseEventArgs e)
        {
            pMousePosition = Beat.PointToClient(MousePosition);
            Moving = true;
        }

        private void btnThumb_MouseUp(object sender, MouseEventArgs e)
        {
            Moving = false;
        }

        private void btnThumb_MouseMove(object sender, MouseEventArgs e)
        {
            if (Moving && e.Button == MouseButtons.Left)
            {
                var theFormPosition = PointToClient(MousePosition);
                theFormPosition.X -= pMousePosition.X;

                if (theFormPosition.X > Width - Beat.Width)
                    theFormPosition.X = Width - Beat.Width;

                if (theFormPosition.X < 0)
                    theFormPosition.X = 0;

                Beat.Left = theFormPosition.X;

                _TrackBarValue = (int)(theFormPosition.X / (float)(ClientSize.Width - Beat.Width) * 100);
                if (TrackBarValueChanged != null)
                    TrackBarValueChanged(this, null);
            }
        }

        private void Dragger_MouseEnter(object sender, EventArgs e)
        {
            Beat.BackColor = SystemColors.GrayText;
        }

        private void Dragger_MouseLeave(object sender, EventArgs e)
        {
            Beat.BackColor = SystemColors.ControlDark;
        }
    }
}
