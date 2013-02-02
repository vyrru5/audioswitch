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

        private readonly List<Icon> TrayIcons = new List<Icon>();
        private static int CurrentDevice;
        private static EDataFlow RenderType = EDataFlow.eRender;
        
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312) // WM_HOTKEY
                HotKeyPressed();
            else if (m.Msg != 0x84) // resize window
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

            TrayIcons.Add(getIcon(Resources.mute));
            TrayIcons.Add(getIcon(Resources.zero));
            TrayIcons.Add(getIcon(Resources._1_33));
            TrayIcons.Add(getIcon(Resources._33_66));
            TrayIcons.Add(getIcon(Resources._66_100));
        }

        private static Icon getIcon(Bitmap source)
        {
            return Icon.FromHandle(source.GetHicon());
        }

        ~FormSwitcher()
        {
            Hotkey.UnregisterHotKey();
        }

        private void FormSwitcher_Load(object sender, EventArgs e)
        {
            RefreshDevices(false);
            Volume.VolumeMuteChanged += IconChanged;
            Volume.RegisterDevice(RenderType);
            
            listView1.ItemSelectionChanged += listView1_ItemSelectionChanged;
            listView1.Scroll += Volume.DoScroll;
            listView1.LargeImageList = DeviceIcons.ActiveIcons;

            Hotkey.handle = Handle;
            Hotkey.RegisterHotKey(notifyIcon1);
        }

        private void FormSwitcher_Deactivate(object sender, EventArgs e)
        {
            Hide();
            timer1.Enabled = false;
            Volume.UnregisterDevice();
            RenderType = EDataFlow.eRender;
            RefreshDevices(false);
            Volume.RegisterDevice(RenderType);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(SystemColors.ScrollBar))
                e.Graphics.DrawLine(pen, 0, 0, pictureBox1.Width, 0);
        }

        private void IconChanged(object sender, EventArgs eventArgs)
        {
            if (Volume.Mute)
                notifyIcon1.Icon = TrayIcons[0];
            else if (Volume.Value == 0)
                notifyIcon1.Icon = TrayIcons[1];
            else if (Volume.Value > 0 && Volume.Value < 0.33)
                notifyIcon1.Icon = TrayIcons[2];
            else if (Volume.Value > 0.33 && Volume.Value < 0.66)
                notifyIcon1.Icon = TrayIcons[3];
            else if (Volume.Value > 0.66 && Volume.Value <= 1)
                notifyIcon1.Icon = TrayIcons[4];
        }

        private void HotKeyPressed()
        {
            if (Visible) return; // Let's not make it complicated here...

            Volume.UnregisterDevice();
            RenderType = EDataFlow.eRender;
            RefreshDevices(false);
            if (EndPoints.DeviceNames.Count == 0) return;

            CurrentDevice = CurrentDevice == EndPoints.DeviceNames.Count - 1 ? 0 : CurrentDevice + 1;
            EndPoints.SetDefaultDevice(CurrentDevice);
            if (!Program.stfu)
                notifyIcon1.ShowBalloonTip(0, "Audio device changed", EndPoints.DeviceNames[CurrentDevice], ToolTipIcon.Info);
        }

        private void notifyIcon1_MouseMove(object sender, MouseEventArgs e)
        {
            var text = EndPoints.DeviceNames[CurrentDevice];
            notifyIcon1.Text = text.Length > 63 ? text.Substring(0, 63) : text;
        }

        private void notifyIcon1_MouseDown(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Shift)
            {
                Close();
                return;
            }

            Volume.UnregisterDevice();
            RenderType = ModifierKeys.HasFlag(Keys.Control) ? EDataFlow.eCapture : EDataFlow.eRender;

            if (ModifierKeys.HasFlag(Keys.Alt))
            {
                RefreshDevices(false);
                Volume.RegisterDevice(RenderType);
                Volume.ChangeMute();
                Volume.UnregisterDevice();

                if (RenderType == EDataFlow.eCapture && !Program.stfu)
                {
                    var mutetxt = String.Format("Device {0}muted", Volume.Mute ? "" : "un");
                    notifyIcon1.ShowBalloonTip(0, mutetxt, EndPoints.DeviceNames[CurrentDevice], ToolTipIcon.Info);
                }
                
                RenderType = EDataFlow.eRender;
                RefreshDevices(false);
                Volume.RegisterDevice(RenderType);
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
                    if (!Program.stfu)
                    {
                        Height = listView1.Items.Count * listView1.TileSize.Height + pictureBox1.Height + 15;
                        var point = WindowPosition.GetWindowPosition(notifyIcon1, Width, Height);
                        Left = point.X;
                        Top = point.Y;

                        timer1.Enabled = true;
                        Show();
                        Activate();
                    }
                    break;

                case MouseButtons.Right:
                    if (EndPoints.DeviceNames.Count > 0)
                    {
                        CurrentDevice = CurrentDevice == EndPoints.DeviceNames.Count - 1 ? 0 : CurrentDevice + 1;
                        EndPoints.SetDefaultDevice(CurrentDevice);
                        if (!Program.stfu)
                            notifyIcon1.ShowBalloonTip(0, "Audio device changed", EndPoints.DeviceNames[CurrentDevice], ToolTipIcon.Info);
                    }
                    break;
            }
            Volume.RegisterDevice(RenderType);
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            listView1.BeginUpdate();

            if (!e.IsSelected)
            {
                listView1.LargeImageList.Images[CurrentDevice].Dispose();
                listView1.LargeImageList.Images[CurrentDevice] = DeviceIcons.NormalIcons.Images[CurrentDevice];
            }
            if (e.IsSelected && CurrentDevice != e.ItemIndex)
            {
                CurrentDevice = e.ItemIndex;
                EndPoints.SetDefaultDevice(CurrentDevice);
                Volume.UnregisterDevice();
                Volume.RegisterDevice(RenderType);

                listView1.LargeImageList.Images[CurrentDevice].Dispose();
                listView1.LargeImageList.Images[CurrentDevice] = DeviceIcons.DefaultIcons.Images[CurrentDevice];
            }

            listView1.EndUpdate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var peaks = Volume.Device.AudioMeterInformation.PeakValues.GetPeaks();
            ledLeft.SetValue(peaks[0]);
            ledRight.SetValue(peaks[Volume.Stereo ? 1 : 0]);

            if (!listView1.Focused)
                listView1.Focus();

            Hotkey.PollNewHotkey(notifyIcon1);
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            Hotkey.isDown = true;
            Hotkey.hotModifiers = e.Modifiers;
            Hotkey.hotKey = e.KeyCode;
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            Hotkey.isDown = false;
            Hotkey.isRegd = false;
        }

        private void RefreshDevices(bool UpdateListView)
        {
            EndPoints.RefreshDevices(RenderType, UpdateListView);
            if (EndPoints.DeviceNames.Count == 0) return;

            CurrentDevice = EndPoints.GetDefaultDevice(RenderType);
            if (!UpdateListView) return;

            listView1.BeginUpdate();
            listView1.Clear();
            
            for (var i = 0; i < EndPoints.DeviceNames.Count; i++)
            {
                var item = new ListViewItem { ImageIndex = i, Text = EndPoints.DeviceNames[i], Selected = i == CurrentDevice };
                listView1.Items.Add(item);
            }
            
            listView1.LargeImageList.Images[CurrentDevice].Dispose();
            listView1.LargeImageList.Images[CurrentDevice] = DeviceIcons.DefaultIcons.Images[CurrentDevice];
            listView1.EndUpdate();
        }
    }
}
