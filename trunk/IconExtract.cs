using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AudioSwitch
{
    internal static class IconExtract
    {
        [DllImport("Shell32.dll")]
        private static extern int ExtractIconEx(string libName, int iconIndex, IntPtr[] largeIcon, IntPtr[] smallIcon, int nIcons);

        internal static ImageList Extract(IEnumerable<string> addresses)
        {
            var LargeImageList = new ImageList
            {
                ImageSize = new Size(32, 32),
                ColorDepth = ColorDepth.Depth32Bit
            };

            foreach (var address in addresses)
            {
                var iconAdr = address.Split(',');
                var hIconEx = new IntPtr[1];
                ExtractIconEx(iconAdr[0], int.Parse(iconAdr[1]), hIconEx, null, 1);
                LargeImageList.Images.Add(Icon.FromHandle(hIconEx[0]));
            }
            return LargeImageList;
        }
    }
}
