using System;
using System.Collections.Generic;
using System.Drawing;
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

        private readonly TimeSpan tooltipRefreshRate = new TimeSpan(0, 0, 0, 0, 300);
        private DateTime tooltipLastRefresh = DateTime.Now;
        private readonly VolEventsHandler volEvents;
        private static MMDevice VolumeDevice;
        private static List<string> DeviceList = new List<string>();
        private static int CurrentDevice;
        
        public FormSwitcher()
        {
            InitializeComponent();
            SetWindowTheme(listView1.Handle, "explorer", null);

            var icons = new IntPtr[1];
            ExtractIconEx("mmres.dll", 0, icons, null, 1);
            listView1.LargeImageList = new ImageList
                                           {
                                               ImageSize = new Size(32, 32),
                                               ColorDepth = ColorDepth.Depth32Bit
                                           };
            
            listView1.LargeImageList.Images.Add(Icon.FromHandle(icons[0]));
            notifyIcon1.Icon = Resources._0_25;

            volEvents = new VolEventsHandler(tbMaster);
            tbMaster.TrackBarValueChanged += tbMaster_TrackBarValueChanged;
            RefreshDevices();
            UpdateListView();
            UpdateVolControls();
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
            if (ModifierKeys == Keys.Control)
                Close();

            RefreshDevices();
            if (e.Button == MouseButtons.Left)
                UpdateListView();
        }

        private void NotifyIconMouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    Height = listView1.Items.Count * listView1.TileSize.Height + pictureBox1.Height + 15;
                    var point = WindowPosition.GetWindowPosition(notifyIcon1, Width, Height);
                    Left = point.X;
                    Top = point.Y;

                    UpdateVolControls();
                    Show();
                    Activate();
                    break;

                case MouseButtons.Right:
                    if (DeviceList.Count > 0)
                    {
                        CurrentDevice = CurrentDevice == DeviceList.Count - 1 ? 0 : CurrentDevice + 1;
                        EndPointControl.SetDefaultDevice(CurrentDevice);
                        notifyIcon1.ShowBalloonTip(0, "Audio output changed", DeviceList[CurrentDevice], ToolTipIcon.Info);
                    }
                    break;
            }
        }

        private void UpdateListView()
        {
            listView1.Clear();
            listView1.BeginUpdate();
            foreach (var audioDevice in DeviceList)
            {
                var item = new ListViewItem { ImageIndex = 0, Text = audioDevice };
                listView1.Items.Add(item);
            }
            if (DeviceList.Count > 0)
                listView1.Items[CurrentDevice].Selected = true;
            listView1.EndUpdate();
        }

        private void UpdateVolControls()
        {
            if (DeviceList.Count > 0)
            {
                if (VolumeDevice != null)
                    VolumeDevice.AudioEndpointVolume.OnVolumeNotification -= VolNotify;

                VolumeDevice = EndPointControl.pEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

                if (!VolumeDevice.AudioEndpointVolume.Mute)
                    tbMaster.Value = (int)(VolumeDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
                else
                    tbMaster.Value = 0;
                
                VolumeDevice.AudioSessionManager.Sessions[0].RegisterAudioSessionNotification(volEvents);
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
            {
                if (!tbMaster.Moving)
                    tbMaster.Value = VolumeDevice.AudioEndpointVolume.Mute ? 0 : (int) (data.MasterVolume*100);
                SetIcon();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(SystemColors.ScrollBar))
                e.Graphics.DrawLine(pen, 0, 0, pictureBox1.Width, 0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tbMaster.SetLeftChannel((int)Math.Ceiling(VolumeDevice.AudioMeterInformation.PeakValues[0] * 13));
            tbMaster.SetRightChannel((int)Math.Ceiling(VolumeDevice.AudioMeterInformation.PeakValues[1] * 13));
        }

        private void tbMaster_TrackBarValueChanged(object sender, EventArgs eventArgs)
        {
            if (VolumeDevice == null)
                return;
            if (VolumeDevice.AudioEndpointVolume.Mute)
                VolumeDevice.AudioEndpointVolume.Mute = false;

            VolumeDevice.AudioEndpointVolume.MasterVolumeLevelScalar = tbMaster.Value / 100.0f;
            SetIcon();
        }

        private void SetIcon()
        {
            if (VolumeDevice.AudioEndpointVolume.Mute)
                notifyIcon1.Icon = Resources.mute;
            else if (tbMaster.Value >= 0 && tbMaster.Value <= 25)
                notifyIcon1.Icon = Resources._0_25;
            else if (tbMaster.Value > 25 && tbMaster.Value < 50)
                notifyIcon1.Icon = Resources._25_50;
            else if (tbMaster.Value > 50 && tbMaster.Value < 75)
                notifyIcon1.Icon = Resources._50_75;
            else if (tbMaster.Value > 75 && tbMaster.Value <= 100)
                notifyIcon1.Icon = Resources._75_100;
        }
    }
}
