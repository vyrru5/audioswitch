using System;
using System.Drawing;
using System.Windows.Forms;
using AudioSwitch.Properties;

namespace AudioSwitch
{
    public partial class ProgressTrackBar : UserControl
    {
        private readonly Label[] pgLeft;
        private readonly Label[] pgRight;

        private readonly Color[] pgOnColors;
        private readonly Color[] pgOffColors;

        public EventHandler TrackBarValueChanged;
        public EventHandler MuteChanged;

        private Point pMousePosition = Point.Empty;
        public bool Moving;

        private bool _mute;
        public bool Mute
        {
            get { return _mute; }
            set
            {
                Thumb.Image.Dispose();
                Thumb.Image = value ? Resources.ThumbMute : Resources.ThumbNormal;
                _mute = value;
            }
        }

        private int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                MoveThumb();
            }
        }

        public ProgressTrackBar()
        {
            InitializeComponent();

            pgLeft = new[] { new Label(), l1, l2, l3, l4, l5, l6, l7, l8, l9, l10, l11, l12, l13 };
            pgRight = new[] { new Label(), r1, r2, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r13 };
            pgOnColors = new[]
                             {
                                 Color.Black, Color.Lime, Color.Lime, Color.Lime, Color.Lime,
                                 Color.Lime, Color.Lime, Color.Lime, Color.Lime, Color.Lime,
                                 Color.Yellow, Color.Yellow, Color.Yellow, Color.Red
                             };

            pgOffColors = new[]
                             {
                                 Color.Black, Color.Green, Color.Green, Color.Green, Color.Green,
                                 Color.Green, Color.Green, Color.Green, Color.Green, Color.Green,
                                 Color.Olive, Color.Olive, Color.Olive, Color.Maroon
                             };
        }

        public void SetLeftChannel(byte value)
        {
            for (byte i = 0; i < 14; i++)
                pgLeft[i].BackColor = value >= i ? pgOnColors[i] : pgOffColors[i];
        }

        public void SetRightChannel(byte value)
        {
            for (byte i = 0; i < 14; i++)
                pgRight[i].BackColor = value >= i ? pgOnColors[i] : pgOffColors[i];
        }

        private void MoveThumb()
        {
            var trackDistance = ClientSize.Width - Thumb.Width;
            var fractionMoved = (float)_value / 100;
            Thumb.Left = (int)(fractionMoved * trackDistance);
        }

        private void Thumb_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pMousePosition = Thumb.PointToClient(MousePosition);
                Moving = true;
            }
            else
            {
                Mute = !Mute;
                if (MuteChanged != null)
                    MuteChanged(this, null);
            }
        }

        private void Thumb_MouseUp(object sender, MouseEventArgs e)
        {
            Moving = false;
        }

        private void Thumb_MouseMove(object sender, MouseEventArgs e)
        {
            if (Moving && e.Button == MouseButtons.Left)
            {
                var theFormPosition = PointToClient(MousePosition);
                theFormPosition.X -= pMousePosition.X;

                if (theFormPosition.X > Width - Thumb.Width)
                    theFormPosition.X = Width - Thumb.Width;

                if (theFormPosition.X < 0)
                    theFormPosition.X = 0;

                Thumb.Left = theFormPosition.X;
                Thumb.Refresh();

                _value = (int)(theFormPosition.X / (float)(ClientSize.Width - Thumb.Width) * 100);
                if (TrackBarValueChanged != null)
                    TrackBarValueChanged(this, null);
            }
        }
        
        private void lblGraph_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var theFormPosition = PointToClient(MousePosition);
                theFormPosition.X -= Thumb.Width / 2;

                if (theFormPosition.X > Width - Thumb.Width)
                    theFormPosition.X = Width - Thumb.Width;

                if (theFormPosition.X < 0)
                    theFormPosition.X = 0;

                Thumb.Left = theFormPosition.X;

                Moving = true;
            }
            else
            {
                Mute = !Mute;
                if (MuteChanged != null)
                    MuteChanged(this, null);
            }
        }

        private void lblGraph_MouseMove(object sender, MouseEventArgs e)
        {
            if (Moving && e.Button == MouseButtons.Left)
            {
                var theFormPosition = PointToClient(MousePosition);
                theFormPosition.X -= Thumb.Width / 2;

                if (theFormPosition.X > Width - Thumb.Width)
                    theFormPosition.X = Width - Thumb.Width;

                if (theFormPosition.X < 0)
                    theFormPosition.X = 0;

                Thumb.Left = theFormPosition.X;

                _value = (int)(theFormPosition.X / (float)(ClientSize.Width - Thumb.Width) * 100);
                if (TrackBarValueChanged != null)
                    TrackBarValueChanged(this, null);
            }
        }

        private void lblGraph_MouseUp(object sender, MouseEventArgs e)
        {
            Moving = false;
        }

        private void Thumb_Move(object sender, EventArgs e)
        {
            Thumb.Refresh();
            lblGraph.Refresh();
        }

        private void Thumb_MouseEnter(object sender, EventArgs e)
        {
            if (_mute) return;
            Thumb.Image.Dispose();
            Thumb.Image = Resources.ThumbHover;
        }

        private void Thumb_MouseLeave(object sender, EventArgs e)
        {
            if (_mute) return;
            Thumb.Image.Dispose();
            Thumb.Image = Resources.ThumbNormal;
        }
    }
}
