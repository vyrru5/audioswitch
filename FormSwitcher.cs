using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AudioSwitch.CoreAudioApi;

namespace AudioSwitch
{
    public partial class FormSwitcher : Form
    {
        [DllImport("Shell32.dll")]
        private static extern int ExtractIconEx(string libName, int iconIndex, IntPtr[] largeIcon, IntPtr[] smallIcon, int nIcons);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int SetWindowTheme(IntPtr hWnd, string appName, string partList);

        private readonly TimeSpan tooltipRefreshRate = new TimeSpan(0, 0, 0, 0, 300);
        private DateTime tooltipLastRefresh = DateTime.Now;
        private static MMDevice VolumeDevice;
        private static List<string> DeviceList = new List<string>();
        private static int CurrentDevice;

        public FormSwitcher()
        {
            InitializeComponent();
            SetWindowTheme(listView1.Handle, "explorer", null);

            var icon = new IntPtr[1];
            ExtractIconEx("SndVol.exe", 1, null, icon, 1);
            notifyIcon1.Icon = Icon.FromHandle(icon[0]);
            
            ExtractIconEx("mmres.dll", 0, icon, null, 1);
            listView1.LargeImageList = new ImageList
                                           {
                                               ImageSize = new Size(32, 32),
                                               ColorDepth = ColorDepth.Depth32Bit
                                           };
            listView1.LargeImageList.Images.Add(Icon.FromHandle(icon[0]));
        }

        private static void RefreshDevices()
        {
            DeviceList = EndPointControl.GetDevices();
            if (DeviceList.Count > 0)
                CurrentDevice = EndPointControl.GetDefaultDevice();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg != 132)
                base.WndProc(ref m);
        }

        private void DeviceListOnSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected && CurrentDevice != e.ItemIndex)
            {
                CurrentDevice = e.ItemIndex;
                EndPointControl.SetDefaultDevice(CurrentDevice);
                UpdateVolControls();
            }
        }

        private void ExitOnMouseClick(object sender, MouseEventArgs e)
        {
            Close();
        }

        private void ExitOnMouseEnter(object sender, EventArgs e)
        {
            SetHover(true, label1);
        }

        private void ExitOnMouseLeave(object sender, EventArgs e)
        {
            SetHover(false, label1);
        }

        private void SetHover(bool isMouseOver, Control label)
        {
            var newFont = new Font(label.Font.FontFamily,
                label.Font.Size,
                isMouseOver ? FontStyle.Underline : FontStyle.Regular,
                label.Font.Unit,
                0);
            label.Font.Dispose();
            label.Font = newFont;
        }

        private void OnDeactivated(object sender, EventArgs e)
        {
            Hide();
            timer1.Enabled = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            ShowInTaskbar = false;
            Hide();
            base.OnLoad(e);
        }

        private void notifyIcon1_MouseMove(object sender, MouseEventArgs e)
        {
            if (DateTime.Now - tooltipLastRefresh > tooltipRefreshRate)
            {
                RefreshDevices();
                tooltipLastRefresh = DateTime.Now;
            }
            notifyIcon1.Text = DeviceList.Count > 0 ? DeviceList[CurrentDevice] : "No audio devices connected";
        }

        private void notifyIcon1_MouseDown(object sender, MouseEventArgs e)
        {
            RefreshDevices();
            if (e.Button != MouseButtons.Right) return;

            listView1.Clear();
            listView1.BeginUpdate();
            foreach (var audioDevice in DeviceList)
            {
                var devName = audioDevice;
                if (audioDevice.Length > 30)
                    devName = audioDevice.Substring(0, 30) + Environment.NewLine +
                                  audioDevice.Substring(30, audioDevice.Length - 30);

                var item = new ListViewItem { ImageIndex = 0, Text = devName };
                listView1.Items.Add(item);
            }
            if (DeviceList.Count > 0)
                listView1.Items[CurrentDevice].Selected = true;
            listView1.EndUpdate();
        }

        private void NotifyIconMouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    Height = listView1.Items.Count * listView1.TileSize.Height + pictureBox1.Height + 15;
                    var point = WindowPosition.GetWindowPosition(notifyIcon1, Width, Height);
                    Left = point.X;
                    Top = point.Y;

                    UpdateVolControls();
                    Show();
                    Activate();
                    break;

                case MouseButtons.Left:
                    if (DeviceList.Count > 0)
                    {
                        CurrentDevice = CurrentDevice == DeviceList.Count - 1 ? 0 : CurrentDevice + 1;
                        EndPointControl.SetDefaultDevice(CurrentDevice);
                        notifyIcon1.ShowBalloonTip(0, "Audio output changed", DeviceList[CurrentDevice], ToolTipIcon.Info);
                    }
                    break;
            }
        }

        private void UpdateVolControls()
        {
            if (DeviceList.Count > 0)
            {
                var selItemHeight = listView1.TileSize.Height*listView1.SelectedItems[0].Index;
                pgLeft.Top = selItemHeight + 5;
                pgRight.Top = selItemHeight + 5;
                label2.Top = selItemHeight + 4;

                if (VolumeDevice != null)
                    VolumeDevice.AudioEndpointVolume.OnVolumeNotification -= VolNotify;

                VolumeDevice = EndPointControl.pEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

                if (!VolumeDevice.AudioEndpointVolume.Mute)
                    tbMaster.Value = (int)(VolumeDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
                else
                    tbMaster.Value = 0;

                VolumeDevice.AudioEndpointVolume.OnVolumeNotification += VolNotify;
                timer1.Enabled = true;
            }
        }

        private void VolNotify(AudioVolumeNotificationData data)
        {
            if (InvokeRequired)
                Invoke(new AudioEndpointVolumeNotificationDelegate(VolNotify),
                       new object[] {data});
            else
                tbMaster.Value = VolumeDevice.AudioEndpointVolume.Mute ? 0 : (int) (data.MasterVolume*100);
        }

        private void tbMaster_Scroll(object sender, EventArgs e)
        {
            if (VolumeDevice.AudioEndpointVolume.Mute)
                VolumeDevice.AudioEndpointVolume.Mute = false;

            VolumeDevice.AudioEndpointVolume.MasterVolumeLevelScalar = tbMaster.Value / 100.0f;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(SystemColors.ScrollBar))
                e.Graphics.DrawLine(pen, 0, 0, pictureBox1.Width, 0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pgLeft.Value = (int)(VolumeDevice.AudioMeterInformation.PeakValues[0] * 100);
            pgRight.Value = (int)(VolumeDevice.AudioMeterInformation.PeakValues[1] * 100);
        }

        private void tbMaster_MouseUp(object sender, MouseEventArgs e)
        {
            listView1.Focus();
        }
    }
}
