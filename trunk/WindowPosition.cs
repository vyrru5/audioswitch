using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AudioSwitch
{
    internal static class WindowPosition
    {
        [DllImport("shell32.dll", SetLastError = true)]
        private static extern IntPtr SHAppBarMessage(ABM dwMessage, [In] ref APPBARDATA pData);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("Shell32", SetLastError = true)]
        private static extern int Shell_NotifyIconGetRect(ref NOTIFYICONIDENTIFIER identifier, out RECT iconLocation);

        private enum TaskbarPosition
        {
            Unknown = -1,
            Left,
            Top,
            Right,
            Bottom,
        }

        private enum ABM : uint
        {
            New = 0x00000000,
            Remove = 0x00000001,
            QueryPos = 0x00000002,
            SetPos = 0x00000003,
            GetState = 0x00000004,
            GetTaskbarPos = 0x00000005,
            Activate = 0x00000006,
            GetAutoHideBar = 0x00000007,
            SetAutoHideBar = 0x00000008,
            WindowPosChanged = 0x00000009,
            SetState = 0x0000000A,
        }

        private enum ABE : uint
        {
            Left = 0,
            Top = 1,
            Right = 2,
            Bottom = 3
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public ABE uEdge;
            public RECT rc;
            public int lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NOTIFYICONIDENTIFIER
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uID;
            public Guid guidItem;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private static TaskbarPosition GetPosition(out Rectangle bounds)
        {
            var taskbarHandle = FindWindow("Shell_TrayWnd", null);

            var data = new APPBARDATA { cbSize = (uint)Marshal.SizeOf(typeof(APPBARDATA)), hWnd = taskbarHandle };
            var result = SHAppBarMessage(ABM.GetTaskbarPos, ref data);
            if (result == IntPtr.Zero)
                throw new InvalidOperationException();

            bounds = Rectangle.FromLTRB(data.rc.left, data.rc.top, data.rc.right, data.rc.bottom);
            return (TaskbarPosition)data.uEdge;
        }

        private static Point GetNotifyIconRectangle(IDisposable notifyicon)
        {
            var field = notifyicon.GetType().GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);
            var num = (int) field.GetValue(notifyicon);
            var fieldInfo = notifyicon.GetType().GetField("window", BindingFlags.NonPublic | BindingFlags.Instance);
            var window = (NativeWindow) fieldInfo.GetValue(notifyicon);
            var notifyiconidentifier2 = new NOTIFYICONIDENTIFIER {hWnd = window.Handle, uID = (uint) num};
            notifyiconidentifier2.cbSize = (uint) Marshal.SizeOf(notifyiconidentifier2);

            RECT rect;
            Shell_NotifyIconGetRect(ref notifyiconidentifier2, out rect);
            return new Point(rect.left + (rect.right - rect.left) / 2, rect.top + (rect.bottom - rect.top) / 2);
        }

        public static Point GetWindowPosition(NotifyIcon notifyicon, int windowwidth, int windowheight)
        {
            int left;
            int top;
            Rectangle taskbar;
            var position = GetPosition(out taskbar);
            
            if (position == TaskbarPosition.Unknown)
                return new Point((Screen.PrimaryScreen.WorkingArea.Width + windowwidth)/2,
                                 (Screen.PrimaryScreen.WorkingArea.Height + windowheight)/2);

            var point = GetNotifyIconRectangle(notifyicon);

            switch (position)
            {
                case TaskbarPosition.Top:
                    left = point.X - windowwidth/2;
                    if (left > taskbar.Left + taskbar.Width - windowwidth)
                        left = taskbar.Left + taskbar.Width - windowwidth;

                    top = taskbar.Top + taskbar.Height + 8;
                    break;

                case TaskbarPosition.Left:
                    left = taskbar.Left + taskbar.Width + 8;

                    top = point.Y - windowheight / 2;
                    if (top > taskbar.Top + taskbar.Height - windowheight)
                        top = taskbar.Top + taskbar.Height - windowheight;
                    break;

                case TaskbarPosition.Right:
                    left = taskbar.Left - windowwidth - 8;

                    top = point.Y - windowheight / 2;
                    if (top > taskbar.Top + taskbar.Height - windowheight)
                        top = taskbar.Top + taskbar.Height - windowheight;
                    break;

                default: 
                    left = point.X - windowwidth/2;
                    if (left > taskbar.Left + taskbar.Width - windowwidth)
                        left = taskbar.Left + taskbar.Width - windowwidth;

                    top = taskbar.Top - windowheight - 8;
                    break;
            }
            return new Point(left, top);
        }
    }
}

