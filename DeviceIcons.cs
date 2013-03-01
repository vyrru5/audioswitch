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

        internal static ImageList ActiveIcons;
        internal static ImageList NormalIcons;
        internal static ImageList DefaultIcons;

        internal static void InitImageLists(float dpifactor)
        {
            var size = new Size((int)(32 * dpifactor), (int)(32 * dpifactor));
            ActiveIcons = new ImageList
            {
                ImageSize = size,
                ColorDepth = ColorDepth.Depth32Bit
            };
            NormalIcons = new ImageList
            {
                ImageSize = size,
                ColorDepth = ColorDepth.Depth32Bit
            };
            DefaultIcons = new ImageList
            {
                ImageSize = size,
                ColorDepth = ColorDepth.Depth32Bit
            };
        }

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
                    canvas.DrawImage(overlay, 0, 0, original.Width, original.Height);
                    canvas.Save();
                    return bitmap;
                }
            }
        }
    }
}
