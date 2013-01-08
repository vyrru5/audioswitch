using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AudioSwitch
{
    internal static class IconExtract
    {
        [DllImport("Shell32", CharSet = CharSet.Auto)]
        private static extern int ExtractIconEx(string lpszFile, int nIndex, IntPtr[] iLarge, IntPtr[] iSmall, int nIcons);

        internal static Icon Extract(string address)
        {
            var hIconEx = new IntPtr[1];
            var iconAdr = address.Split(',');

            ExtractIconEx(iconAdr[0], int.Parse(iconAdr[1]), hIconEx, null, 1);
            return Icon.FromHandle(hIconEx[0]);
        }
    }
}
