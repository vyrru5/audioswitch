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

        public FormSwitcher()
        {
            InitializeComponent();
            SetWindowTheme(listDevices.Handle, "explorer", null);

            TrayIcons.Add(getIcon(Resources.mute));
            TrayIcons.Add(getIcon(Resources.zero));
            TrayIcons.Add(getIcon(Resources._1_33));
            TrayIcons.Add(getIcon(Resources._33_66));
            TrayIcons.Add(getIcon(Resources._66_100));

            RefreshDevices(false);
            VolBar.VolumeMuteChanged += IconChanged;
            VolBar.RegisterDevice(RenderType);

            listDevices.Scroll += VolBar.DoScroll;
            listDevices.LargeImageList = DeviceIcons.ActiveIcons;

            Hotkey.handle = Handle;
            Hotkey.RegisterHotKey(notifyIcon);
        }

        private static Icon getIcon(Bitmap source)
        {
            return Icon.FromHandle(source.GetHicon());
        }

        ~FormSwitcher()
        {
            Hotkey.UnregisterHotKey();
        }

        private void FormSwitcher_Deactivate(object sender, EventArgs e)
        {
            Hide();
            listDevices.ItemSelectionChanged -= listDevices_ItemSelectionChanged;
            timer1.Enabled = false;
            VolBar.UnregisterDevice();
            RenderType = EDataFlow.eRender;
            RefreshDevices(false);
            VolBar.RegisterDevice(RenderType);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(SystemColors.ScrollBar))
                e.Graphics.DrawLine(pen, 0, 0, pictureItemsBack.Width, 0);
        }

        private void IconChanged(object sender, EventArgs eventArgs)
        {
            if (VolBar.Mute)
                notifyIcon.Icon = TrayIcons[0];
            else if (VolBar.Value == 0)
                notifyIcon.Icon = TrayIcons[1];
            else if (VolBar.Value > 0 && VolBar.Value < 0.33)
                notifyIcon.Icon = TrayIcons[2];
            else if (VolBar.Value > 0.33 && VolBar.Value < 0.66)
                notifyIcon.Icon = TrayIcons[3];
            else if (VolBar.Value > 0.66 && VolBar.Value <= 1)
                notifyIcon.Icon = TrayIcons[4];
        }

        private void HotKeyPressed()
        {
            if (Visible) return; // Let's not make it complicated here...

            VolBar.UnregisterDevice();
            RenderType = EDataFlow.eRender;
            RefreshDevices(false);
            if (EndPoints.DeviceNames.Count == 0) return;

            CurrentDevice = CurrentDevice == EndPoints.DeviceNames.Count - 1 ? 0 : CurrentDevice + 1;
            EndPoints.SetDefaultDevice(CurrentDevice);
            if (!Program.stfu)
                notifyIcon.ShowBalloonTip(0, "Audio device changed", EndPoints.DeviceNames[CurrentDevice], ToolTipIcon.Info);
        }

        private void notifyIcon1_MouseMove(object sender, MouseEventArgs e)
        {
            var text = EndPoints.DeviceNames[CurrentDevice];
            notifyIcon.Text = text.Length > 63 ? text.Substring(0, 63) : text;
        }

        private void notifyIcon1_MouseDown(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Shift)
            {
                Close();
                Application.Exit();
                return;
            }

            VolBar.UnregisterDevice();
            RenderType = ModifierKeys.HasFlag(Keys.Control) ? EDataFlow.eCapture : EDataFlow.eRender;

            if (ModifierKeys.HasFlag(Keys.Alt))
            {
                RefreshDevices(false);
                VolBar.RegisterDevice(RenderType);
                VolBar.ChangeMute();
                VolBar.UnregisterDevice();

                if (RenderType == EDataFlow.eCapture && !Program.stfu)
                {
                    var mutetxt = String.Format("Device {0}muted", VolBar.Mute ? "" : "un");
                    notifyIcon.ShowBalloonTip(0, mutetxt, EndPoints.DeviceNames[CurrentDevice], ToolTipIcon.Info);
                }
                
                RenderType = EDataFlow.eRender;
                RefreshDevices(false);
                VolBar.RegisterDevice(RenderType);
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
                        Height = listDevices.Items.Count * listDevices.TileSize.Height + pictureItemsBack.Height + 15;
                        var point = WindowPosition.GetWindowPosition(notifyIcon, Width, Height);
                        Left = point.X;
                        Top = point.Y;

                        timer1.Enabled = true;
                        Show();
                        Activate();
                        listDevices.ItemSelectionChanged += listDevices_ItemSelectionChanged;
                    }
                    break;

                case MouseButtons.Right:
                    if (EndPoints.DeviceNames.Count > 0)
                    {
                        CurrentDevice = CurrentDevice == EndPoints.DeviceNames.Count - 1 ? 0 : CurrentDevice + 1;
                        EndPoints.SetDefaultDevice(CurrentDevice);
                        if (!Program.stfu)
                            notifyIcon.ShowBalloonTip(0, "Audio device changed", EndPoints.DeviceNames[CurrentDevice], ToolTipIcon.Info);
                    }
                    break;
            }
            VolBar.RegisterDevice(RenderType);
        }

        private void listDevices_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            listDevices.BeginUpdate();

            if (!e.IsSelected)
            {
                listDevices.LargeImageList.Images[CurrentDevice].Dispose();
                listDevices.LargeImageList.Images[CurrentDevice] = DeviceIcons.NormalIcons.Images[CurrentDevice];
            }
            else if (CurrentDevice != e.ItemIndex)
            {
                CurrentDevice = e.ItemIndex;
                EndPoints.SetDefaultDevice(CurrentDevice);
                VolBar.UnregisterDevice();
                VolBar.RegisterDevice(RenderType);

                listDevices.LargeImageList.Images[CurrentDevice].Dispose();
                listDevices.LargeImageList.Images[CurrentDevice] = DeviceIcons.DefaultIcons.Images[CurrentDevice];
            }

            listDevices.EndUpdate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var peaks = VolBar.Device.AudioMeterInformation.Channels.GetPeaks();
            ledLeft.SetValue(peaks[0]);
            ledRight.SetValue(peaks[VolBar.Stereo ? 1 : 0]);

            if (!listDevices.Focused)
                listDevices.Focus();

            Hotkey.PollNewHotkey(notifyIcon);
        }

        private void listDevices_KeyDown(object sender, KeyEventArgs e)
        {
            Hotkey.isDown = true;
            Hotkey.hotModifiers = e.Modifiers;
            Hotkey.hotKey = e.KeyCode;
        }

        private void listDevices_KeyUp(object sender, KeyEventArgs e)
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

            listDevices.BeginUpdate();
            listDevices.Clear();
            
            for (var i = 0; i < EndPoints.DeviceNames.Count; i++)
            {
                var item = new ListViewItem { ImageIndex = i, Text = EndPoints.DeviceNames[i], Selected = i == CurrentDevice };
                listDevices.Items.Add(item);
            }
            
            listDevices.LargeImageList.Images[CurrentDevice].Dispose();
            listDevices.LargeImageList.Images[CurrentDevice] = DeviceIcons.DefaultIcons.Images[CurrentDevice];
            listDevices.EndUpdate();
        }
    }
}
