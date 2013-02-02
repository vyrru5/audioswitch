using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AudioSwitch.Properties;

namespace AudioSwitch
{
    internal static class Hotkey
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [Flags]
        private enum HotModifierKeys : uint
        {
            Alt = 1,
            Control = 2,
            Shift = 4
            //, Win = 8
        }

        internal static bool isDown;
        internal static bool isRegd;
        internal static Keys hotModifiers;
        internal static Keys hotKey;
        internal static IntPtr handle;

        private const int HotKeyID = 875682524;
        private static DateTime firstPressed;
        private static readonly TimeSpan pressDuration = new TimeSpan(0, 0, 0, 2);

        static Hotkey()
        {
            hotModifiers = Settings.Default.ModifierKeys;
            hotKey = Settings.Default.Key;
        }

        internal static void UnregisterHotKey()
        {
            UnregisterHotKey(handle, HotKeyID);
        }

        internal static void RegisterHotKey(NotifyIcon notifyIcon)
        {
            if (hotModifiers == Keys.Delete || hotKey == Keys.Delete)
            {
                hotModifiers = Keys.None;
                hotKey = Keys.None;
                Settings.Default.ModifierKeys = hotModifiers;
                Settings.Default.Key = hotKey;
                Settings.Default.Save();
                notifyIcon.ShowBalloonTip(0, "Hot keys removed",
                                           "Hot keys have been successfully removed.", ToolTipIcon.Info);
                return;
            }

            HotModifierKeys modkeys = 0;
            if (hotModifiers.HasFlag(Keys.Control))
                modkeys |= HotModifierKeys.Control;
            if (hotModifiers.HasFlag(Keys.Alt))
                modkeys |= HotModifierKeys.Alt;
            if (hotModifiers.HasFlag(Keys.Shift))
                modkeys |= HotModifierKeys.Shift;
            if (modkeys == 0) return;
            
            if (RegisterHotKey(handle, HotKeyID, (uint)modkeys, (uint)hotKey))
            {
                Settings.Default.ModifierKeys = hotModifiers;
                Settings.Default.Key = hotKey;
                Settings.Default.Save();
                var modKeyStr = hotModifiers.ToString() == "0" ? "" : hotModifiers + " + ";
                notifyIcon.ShowBalloonTip(0, "AudioSwitch", string.Format(
                    "Hot key {0}{1} has been successfully set.", modKeyStr, hotKey), ToolTipIcon.Info);
            }
            else
                notifyIcon.ShowBalloonTip(0,
                                           "Hot key registration failed!",
                                           "This combination might be already in use. Please try again with a different combination.",
                                           ToolTipIcon.Error);
        }

        internal static void PollNewHotkey(NotifyIcon notifyIcon)
        {
            if (!isDown) return;

            if (!isRegd)
            {
                firstPressed = DateTime.Now;
                isRegd = true;
            }
            if (DateTime.Now - firstPressed < pressDuration) return;

            isDown = false;
            isRegd = false;
            UnregisterHotKey(handle, HotKeyID);
            RegisterHotKey(notifyIcon);
        }
    }
}
