using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AudioSwitch
{
    internal static class WindowPosition
    {
        [DllImport("Shell32", SetLastError = true)]
        private static extern IntPtr SHAppBarMessage(ABMsg dwMessage, ref APPBARDATA pData);

        [DllImport("Shell32", SetLastError = true)]
        private static extern int Shell_NotifyIconGetRect(ref NOTIFYICONIDENTIFIER identifier, out RECT iconLocation);

        private enum ABEdge
        {
            ABE_LEFT,
            ABE_TOP,
            ABE_RIGHT,
            ABE_BOTTOM
        }

        private enum ABMsg
        {
            ABM_NEW,
            ABM_REMOVE,
            ABM_QUERYPOS,
            ABM_SETPOS,
            ABM_GETSTATE,
            ABM_GETTASKBARPOS,
            ABM_ACTIVATE,
            ABM_GETAUTOHIDEBAR,
            ABM_SETAUTOHIDEBAR,
            ABM_WINDOWPOSCHANGED,
            ABM_SETSTATE
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public ABEdge uEdge;
            public RECT rc;
            public IntPtr lParam;
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
            public static implicit operator Rectangle(RECT rect)
            {
                return rect.right - rect.left >= 0 && rect.bottom - rect.top >= 0
                           ? new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top)
                           : new Rectangle(rect.left, rect.top, 0, 0);
            }

            public static implicit operator RECT(Rectangle rect)
            {
                return new RECT { left = rect.Left, top = rect.Top, right = rect.Right, bottom = rect.Bottom };
            }
        }

        private enum TaskBarAlignment
        {
            Bottom,
            Top,
            Left,
            Right
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TaskBarInfo
        {
            public Rectangle Position;
            public TaskBarAlignment Alignment;
        }

        private static Rectangle GetNotifyIconRectangle(IDisposable notifyicon)
        {
            var field = notifyicon.GetType().GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);
            var num = (int) field.GetValue(notifyicon);
            var fieldInfo = notifyicon.GetType().GetField("window", BindingFlags.NonPublic | BindingFlags.Instance);
            var window = (NativeWindow) fieldInfo.GetValue(notifyicon);
            var notifyiconidentifier2 = new NOTIFYICONIDENTIFIER {hWnd = window.Handle, uID = (uint) num};
            notifyiconidentifier2.cbSize = (uint) Marshal.SizeOf(notifyiconidentifier2);

            RECT iconLocation;
            Shell_NotifyIconGetRect(ref notifyiconidentifier2, out iconLocation);
            return iconLocation;
        }

        private static TaskBarInfo GetTaskBarInfo()
        {
            TaskBarAlignment left;
            var appbardata = new APPBARDATA {hWnd = IntPtr.Zero};
            appbardata.cbSize = (uint)Marshal.SizeOf(appbardata);
            if (SHAppBarMessage(ABMsg.ABM_GETTASKBARPOS, ref appbardata) == IntPtr.Zero)
                throw new Exception("Could not retrieve taskbar information.");

            switch (appbardata.uEdge)
            {
                case ABEdge.ABE_LEFT:
                    left = TaskBarAlignment.Left;
                    break;

                case ABEdge.ABE_TOP:
                    left = TaskBarAlignment.Top;
                    break;

                case ABEdge.ABE_RIGHT:
                    left = TaskBarAlignment.Right;
                    break;

                case ABEdge.ABE_BOTTOM:
                    left = TaskBarAlignment.Bottom;
                    break;

                default:
                    throw new Exception("Couldn't retrieve location of taskbar.");
            }
            return new TaskBarInfo { Position = appbardata.rc, Alignment = left };
        }

        public static Point GetWindowPosition(NotifyIcon notifyicon, int windowwidth, int windowheight)
        {
            int left;
            int top;
            var tBarInf = GetTaskBarInfo();
            var iconRect = GetNotifyIconRectangle(notifyicon);
            var point = new Point(iconRect.Left + iconRect.Width / 2, iconRect.Top + iconRect.Height / 2);
            var flag = iconRect.Left > tBarInf.Position.Right || iconRect.Right < tBarInf.Position.Left || iconRect.Bottom < tBarInf.Position.Top || iconRect.Top > tBarInf.Position.Bottom;
            
            switch (tBarInf.Alignment)
            {
                case TaskBarAlignment.Top:
                    left = point.X - windowwidth / 2;
                    top = !flag ? tBarInf.Position.Bottom + 8 : iconRect.Bottom + 8;
                    break;

                case TaskBarAlignment.Left:
                    if (!flag)
                    {
                        left = tBarInf.Position.Right + 8;
                        top = point.Y - windowheight / 2;
                    }
                    else
                    {
                        left = point.X - windowwidth / 2;
                        top = iconRect.Top - windowheight - 8;
                    }
                    break;

                case TaskBarAlignment.Right:
                    if (!flag)
                    {
                        left = tBarInf.Position.Left - windowwidth - 8;
                        top = point.Y - windowheight / 2;
                    }
                    else
                    {
                        left = point.X - windowwidth / 2;
                        top = iconRect.Top - windowheight - 8;
                    }
                    break;

                default:
                    left = point.X - windowwidth / 2;
                    top = flag ? iconRect.Top - windowheight - 8 : tBarInf.Position.Top - windowheight - 8;
                    break;
            }

            if (windowwidth + left > Screen.PrimaryScreen.WorkingArea.Width)
                left = Screen.PrimaryScreen.WorkingArea.Width - windowwidth;
            else if (left < 0)
                left = 0;

            if (windowheight + top > Screen.PrimaryScreen.WorkingArea.Height)
                top = Screen.PrimaryScreen.WorkingArea.Height - windowheight;
            else if (top < 0)
                top = 0;

            return new Point(left, top);
        }
    }
}

