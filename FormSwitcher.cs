using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AudioSwitch.CoreAudioApi;
using AudioSwitch.Properties;

namespace AudioSwitch
{
    public partial class FormSwitcher : Form
    {
        [DllImport("Shell32.dll")]
        private static extern int ExtractIconEx(string libName, int iconIndex, IntPtr[] largeIcon, IntPtr[] smallIcon, int nIcons);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int SetWindowTheme(IntPtr hWnd, string appName, string partList);

        private static readonly ImageList NormalIcons = new ImageList
        {
            ImageSize = new Size(32, 32),
            ColorDepth = ColorDepth.Depth32Bit
        };

        private static readonly ImageList DefaultIcons = new ImageList
        {
            ImageSize = new Size(32, 32),
            ColorDepth = ColorDepth.Depth32Bit
        };

        private readonly VolEventsHandler volEvents;
        private static MMDevice VolumeDevice;
        private static List<string> DeviceList = new List<string>();
        private static int CurrentDevice;
        private static EDataFlow RenderType = EDataFlow.eRender;

        private static DateTime LastScroll = DateTime.Now;
        private static readonly TimeSpan ShortInterval = new TimeSpan(0, 0, 0, 0, 70);

        protected override void WndProc(ref Message m)
        {
            if (m.Msg != 132)
                base.WndProc(ref m);
        }

        protected override void OnLoad(EventArgs e)
        {
            ShowInTaskbar = false;
            Hide();
            base.OnLoad(e);
        }

        public FormSwitcher()
        {
            InitializeComponent();
            SetWindowTheme(listView1.Handle, "explorer", null);
            volEvents = new VolEventsHandler(Volume);
        }

        private void FormSwitcher_Load(object sender, EventArgs e)
        {
            RefreshDevices(false);
            AddVolControls();

            Volume.TrackBarValueChanged += Volume_TrackBarValueChanged;
            Volume.MuteChanged += MuteChanged;
            listView1.ItemSelectionChanged += listView1_ItemSelectionChanged;
            listView1.Scroll += listView1_Scroll;
            listView1.LargeImageList = new ImageList
            {
                ImageSize = new Size(32, 32),
                ColorDepth = ColorDepth.Depth32Bit
            };
        }

        private void FormSwitcher_Deactivate(object sender, EventArgs e)
        {
            Hide();
            timer1.Enabled = false;
            RemoveVolControls();
            RenderType = EDataFlow.eRender;
            RefreshDevices(false);
            AddVolControls();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(SystemColors.ScrollBar))
                e.Graphics.DrawLine(pen, 0, 0, pictureBox1.Width, 0);
        }

        private void notifyIcon1_MouseDown(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Shift)
            {
                Close();
                return;
            }

            RemoveVolControls();
            RenderType = ModifierKeys.HasFlag(Keys.Control) ? EDataFlow.eCapture : EDataFlow.eRender;

            if (ModifierKeys.HasFlag(Keys.Alt))
            {
                RefreshDevices(false);
                AddVolControls();
                VolumeDevice.AudioEndpointVolume.Mute = !VolumeDevice.AudioEndpointVolume.Mute;
                Volume.Mute = VolumeDevice.AudioEndpointVolume.Mute;
                RemoveVolControls();

                if (RenderType == EDataFlow.eCapture)
                {
                    var mutetxt = string.Format("Device {0}muted", Volume.Mute ? "" : "un");
                    notifyIcon1.ShowBalloonTip(0, mutetxt, DeviceList[CurrentDevice], ToolTipIcon.Info);
                }
                
                RenderType = EDataFlow.eRender;
                RefreshDevices(false);
                AddVolControls();
                return;
            }
            
            RefreshDevices(true);
        }

        private void notifyIcon1_MouseUp(object sender, MouseEventArgs e)
        {
            if (ModifierKeys.HasFlag(Keys.Alt))
                return;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    Height = listView1.Items.Count * listView1.TileSize.Height + pictureBox1.Height + 15;
                    var point = WindowPosition.GetWindowPosition(notifyIcon1, Width, Height);
                    Left = point.X;
                    Top = point.Y;

                    timer1.Enabled = true;
                    Show();
                    Activate();
                    break;

                case MouseButtons.Right:
                    if (DeviceList.Count > 0)
                    {
                        CurrentDevice = CurrentDevice == DeviceList.Count - 1 ? 0 : CurrentDevice + 1;
                        EndPoints.SetDefaultDevice(CurrentDevice, RenderType);
                        notifyIcon1.ShowBalloonTip(0, "Audio device changed", DeviceList[CurrentDevice], ToolTipIcon.Info);
                    }
                    break;
            }
            AddVolControls();
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            listView1.BeginUpdate();

            if (!e.IsSelected)
            {
                listView1.LargeImageList.Images[CurrentDevice].Dispose();
                listView1.LargeImageList.Images[CurrentDevice] = NormalIcons.Images[CurrentDevice];
            }
            if (e.IsSelected && CurrentDevice != e.ItemIndex)
            {
                CurrentDevice = e.ItemIndex;
                EndPoints.SetDefaultDevice(CurrentDevice, RenderType);
                RemoveVolControls();
                AddVolControls();

                listView1.LargeImageList.Images[CurrentDevice].Dispose();
                listView1.LargeImageList.Images[CurrentDevice] = DefaultIcons.Images[CurrentDevice];
            }

            listView1.EndUpdate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ledLeft.SetValue(VolumeDevice.AudioMeterInformation.PeakValues[0]);
            ledRight.SetValue(VolumeDevice.AudioMeterInformation.PeakValues[1]);

            if (!listView1.Focused)
                listView1.Focus();
        }

        private void Volume_TrackBarValueChanged(object sender, EventArgs eventArgs)
        {
            if (VolumeDevice == null)
                return;

            VolumeDevice.AudioEndpointVolume.MasterVolumeLevelScalar = Volume.Value;
            SetIcon();
        }

        private void listView1_Scroll(object sender, ScrollEventArgs e)
        {
            var amount = DateTime.Now - LastScroll <= ShortInterval ? 0.10f : 0.05f;
            LastScroll = DateTime.Now;

            if (e.NewValue > 0)
                if (Volume.Value <= 1 - amount)
                    Volume.Value += amount;
                else
                    Volume.Value = 1;

            else if (e.NewValue < 0)
                if (Volume.Value >= amount)
                    Volume.Value -= amount;
                else
                    Volume.Value = 0;

            Volume_TrackBarValueChanged(null, null);
        }

        // ************* .oO(Functions)Oo. *************           

        private void RefreshDevices(bool UpdateListView)
        {
            DeviceList = EndPoints.GetDevices(RenderType);
            if (DeviceList.Count == 0) return;

            CurrentDevice = EndPoints.GetDefaultDevice(RenderType);
            if (!UpdateListView) return;

            listView1.BeginUpdate();

            listView1.Clear();
            listView1.LargeImageList.Images.Clear();
            NormalIcons.Images.Clear();
            DefaultIcons.Images.Clear();
            var hIconEx = new IntPtr[1];

            for (var i = 0; i < DeviceList.Count; i++)
            {
                var iconAdr = EndPoints.Icons[i].Split(',');
                ExtractIconEx(iconAdr[0], int.Parse(iconAdr[1]), hIconEx, null, 1);
                var icon = Icon.FromHandle(hIconEx[0]);
                listView1.LargeImageList.Images.Add(icon);
                NormalIcons.Images.Add(icon);
                DefaultIcons.Images.Add(AddOverlay(icon, Resources.defaultDevice));

                var item = new ListViewItem { ImageIndex = i, Text = DeviceList[i] };
                listView1.Items.Add(item);
            }

            if (DeviceList.Count > 0)
            {
                listView1.Items[CurrentDevice].Selected = true;
                listView1.LargeImageList.Images[CurrentDevice].Dispose();
                listView1.LargeImageList.Images[CurrentDevice] = DefaultIcons.Images[CurrentDevice];
            }

            listView1.EndUpdate();
        }

        private static Image AddOverlay(Icon originalIcon, Image overlay)
        {
            using (Image original = originalIcon.ToBitmap())
            {
                var bitmap = new Bitmap(originalIcon.Width, originalIcon.Height);
                using (var canvas = Graphics.FromImage(bitmap))
                {
                    canvas.CompositingQuality = CompositingQuality.HighQuality;
                    canvas.DrawImage(original, 0, 0);
                    canvas.DrawImage(overlay, 0, 0);
                    canvas.Save();
                    return bitmap;
                }
            }
        }

        private void RemoveVolControls()
        {
            if (DeviceList.Count == 0) return;

            VolumeDevice.AudioEndpointVolume.OnVolumeNotification -= VolNotify;
            try
            {
                VolumeDevice.AudioSessionManager.Sessions[0].UnregisterAudioSessionNotification(volEvents);
            }
            catch {}
        }

        private void AddVolControls()
        {
            if (DeviceList.Count == 0) return;
            
            VolumeDevice = EndPoints.pEnum.GetDefaultAudioEndpoint(RenderType, ERole.eMultimedia);
            Volume.Value = VolumeDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
            Volume.Mute = VolumeDevice.AudioEndpointVolume.Mute;
            VolumeDevice.AudioSessionManager.Sessions[0].RegisterAudioSessionNotification(volEvents);
            VolumeDevice.AudioEndpointVolume.OnVolumeNotification += VolNotify;
            SetIcon();
        }

        private void VolNotify(AudioVolumeNotificationData data)
        {
            if (InvokeRequired)
                Invoke(new AudioEndpointVolumeNotificationDelegate(VolNotify),
                       new object[] {data});
            else
            {
                if (!Volume.Moving)
                    Volume.Value = data.MasterVolume;
                Volume.Mute = data.Muted;
                SetIcon();
            }
        }

        private void MuteChanged(object sender, EventArgs eventArgs)
        {
            VolumeDevice.AudioEndpointVolume.Mute = Volume.Mute;
            SetIcon();
        }

        private void SetIcon()
        {
            if (Volume.Mute)
                notifyIcon1.Icon = Resources.mute;
            else if (Volume.Value == 0)
                notifyIcon1.Icon = Resources._0_25;
            else if (Volume.Value > 0 && Volume.Value < 0.33)
                notifyIcon1.Icon = Resources._25_50;
            else if (Volume.Value > 0.33 && Volume.Value < 0.66)
                notifyIcon1.Icon = Resources._50_75;
            else if (Volume.Value > 0.66 && Volume.Value <= 1)
                notifyIcon1.Icon = Resources._75_100;
        }
    }
}
