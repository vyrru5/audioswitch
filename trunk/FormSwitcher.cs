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
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int SetWindowTheme(IntPtr hWnd, string appName, string partList);

        private readonly VolEventsHandler volEvents;
        private static MMDevice VolumeDevice;
        private static List<string> DeviceList = new List<string>();
        private static int CurrentDevice;
        private static EDataFlow RenderType = EDataFlow.eRender;

        private byte lastL;
        private byte lastR;
        
        public FormSwitcher()
        {
            InitializeComponent();
            SetWindowTheme(listView1.Handle, "explorer", null);
            volEvents = new VolEventsHandler(tbMaster);
            notifyIcon1.Icon = Resources._0_25;
        }

        private void FormSwitcher_Load(object sender, EventArgs e)
        {
            RefreshDevices(false);
            AddVolControls();
            timer1.Enabled = true;

            tbMaster.TrackBarValueChanged += tbMaster_TrackBarValueChanged;
            tbMaster.MuteChanged += MuteChanged;
            listView1.ItemSelectionChanged += DeviceListOnSelectionChanged;
        }

        private void RefreshDevices(bool UpdateListView)
        {
            DeviceList = EndPoints.GetDevices(RenderType);
            if (DeviceList.Count == 0) return;

            CurrentDevice = EndPoints.GetDefaultDevice(RenderType);
            if (!UpdateListView) return;

            listView1.Clear();
            listView1.BeginUpdate();

            if (listView1.LargeImageList != null)
                listView1.LargeImageList.Dispose();
            listView1.LargeImageList = IconExtract.Extract(EndPoints.Icons);

            for (var i = 0; i < DeviceList.Count; i++)
            {
                var item = new ListViewItem { ImageIndex = i, Text = DeviceList[i] };
                listView1.Items.Add(item);
            }
            if (DeviceList.Count > 0)
                listView1.Items[CurrentDevice].Selected = true;

            listView1.EndUpdate();
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
                EndPoints.SetDefaultDevice(CurrentDevice, RenderType);
                RemoveVolControls();
                AddVolControls();
            }
        }

        private void OnDeactivated(object sender, EventArgs e)
        {
            Hide();
            timer1.Enabled = false;
            RemoveVolControls();
            RenderType = EDataFlow.eRender;
            RefreshDevices(false);
            AddVolControls();
        }

        protected override void OnLoad(EventArgs e)
        {
            ShowInTaskbar = false;
            Hide();
            base.OnLoad(e);
        }

        private void notifyIcon1_MouseMove(object sender, MouseEventArgs e)
        {
            notifyIcon1.Text = DeviceList.Count > 0 ? DeviceList[CurrentDevice] : "No audio devices connected";
        }

        private void notifyIcon1_MouseDown(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Shift)
            {
                Close();
                return;
            }

            RemoveVolControls();
            RenderType = ModifierKeys == Keys.Control ? EDataFlow.eCapture : EDataFlow.eRender;
            RefreshDevices(true);
        }

        private void notifyIcon1_MouseUp(object sender, MouseEventArgs e)
        {
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
            tbMaster.Value = (int)(VolumeDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
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
                if (!tbMaster.Moving)
                    tbMaster.Value = (int) (data.MasterVolume*100);
                tbMaster.Mute = data.Muted;
                SetIcon();
            }
        }

        private void MuteChanged(object sender, EventArgs eventArgs)
        {
            VolumeDevice.AudioEndpointVolume.Mute = tbMaster.Mute;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(SystemColors.ScrollBar))
                e.Graphics.DrawLine(pen, 0, 0, pictureBox1.Width, 0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var newL = (byte)Math.Ceiling(VolumeDevice.AudioMeterInformation.PeakValues[0] * 14);
            var newR = (byte)Math.Ceiling(VolumeDevice.AudioMeterInformation.PeakValues[1] * 14);

            if (lastL != newL)
            {
                tbMaster.SetLeftChannel(newL);
                lastL = newL;
            }
            if (lastR != newR)
            {
                tbMaster.SetRightChannel(newR);
                lastR = newR;
            }
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
            if (tbMaster.Mute)
                notifyIcon1.Icon = Resources.mute;
            else if (tbMaster.Value == 0)
                notifyIcon1.Icon = Resources._0_25;
            else if (tbMaster.Value > 0 && tbMaster.Value < 33)
                notifyIcon1.Icon = Resources._25_50;
            else if (tbMaster.Value > 33 && tbMaster.Value < 66)
                notifyIcon1.Icon = Resources._50_75;
            else if (tbMaster.Value > 66 && tbMaster.Value <= 100)
                notifyIcon1.Icon = Resources._75_100;
        }
    }
}
