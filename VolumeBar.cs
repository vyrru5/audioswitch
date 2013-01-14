using System;
using System.Drawing;
using System.Windows.Forms;
using AudioSwitch.Properties;

namespace AudioSwitch
{
    public partial class VolumeBar : UserControl
    {
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

        public VolumeBar()
        {
            InitializeComponent();
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
