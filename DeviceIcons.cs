using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AudioSwitch.Properties;

namespace AudioSwitch
{
    internal static class DeviceIcons
    {
        [DllImport("Shell32.dll")]
        private static extern int ExtractIconEx(string libName, int iconIndex, IntPtr[] largeIcon, IntPtr[] smallIcon, int nIcons);

        internal static readonly ImageList ActiveIcons = new ImageList
        {
            ImageSize = new Size(32, 32),
            ColorDepth = ColorDepth.Depth32Bit
        };

        internal static readonly ImageList NormalIcons = new ImageList
        {
            ImageSize = new Size(32, 32),
            ColorDepth = ColorDepth.Depth32Bit
        };

        internal static readonly ImageList DefaultIcons = new ImageList
        {
            ImageSize = new Size(32, 32),
            ColorDepth = ColorDepth.Depth32Bit
        };

        internal static void Add(string iconPath)
        {
            var path = Environment.ExpandEnvironmentVariables(iconPath);
            var iconAdr = path.Split(',');

            var hIconEx = new IntPtr[1];
            ExtractIconEx(iconAdr[0], int.Parse(iconAdr[1]), hIconEx, null, 1);
            var icon = Icon.FromHandle(hIconEx[0]);

            NormalIcons.Images.Add(icon);
            ActiveIcons.Images.Add(icon);
            DefaultIcons.Images.Add(AddOverlay(icon, Resources.defaultDevice));
        }

        internal static void Clear()
        {
            ActiveIcons.Images.Clear();
            NormalIcons.Images.Clear();
            DefaultIcons.Images.Clear();
        }

        private static Image AddOverlay(Icon originalIcon, Image overlay)
        {
            using (Image original = originalIcon.ToBitmap())
            {
                var bitmap = new Bitmap(originalIcon.Width, originalIcon.Height);
                using (var canvas = Graphics.FromImage(bitmap))
                {
                    canvas.DrawImage(original, 0, 0);
                    canvas.DrawImage(overlay, 0, 0);
                    canvas.Save();
                    return bitmap;
                }
            }
        }
    }
}
