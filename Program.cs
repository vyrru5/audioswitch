using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AudioSwitch.CoreAudioApi;

namespace AudioSwitch
{
    internal static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool AttachConsole(int dwProcessId);

        internal static bool stfu;

        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var cmdArgsJoined = string.Join(" ", args);
                var cmdArgs = cmdArgsJoined.Split('-');

                foreach (var arg in cmdArgs)
                {
                    if (string.IsNullOrWhiteSpace(arg))
                        continue;

                    var purecmd = arg.Length > 1 ? arg.Substring(1, arg.Length - 1).Trim() : "";

                    switch (arg.Substring(0, 1))
                    {
                        case "s":
                            stfu = true;
                            break;

                        case "m":
                            Keys modifiers;
                            var mresult = Enum.TryParse(purecmd, true, out modifiers);
                            if (mresult) Hotkey.hotModifiers = modifiers;
                            break;

                        case "k":
                            Keys key;
                            var kresult = Enum.TryParse(purecmd, true, out key);
                            if (kresult) Hotkey.hotKey = key;
                            break;

                        case "i":
                            var devID = int.Parse(purecmd);
                            EndPoints.RefreshDevices(EDataFlow.eRender, false);
                            if (devID <= EndPoints.DeviceNames.Count - 1)
                                EndPoints.SetDefaultDevice(devID);
                            break;

                        case "l":
                            if (!AttachConsole(-1))
                                AllocConsole();
                            Console.WriteLine();
                            Console.WriteLine("Devices available:");
                            EndPoints.RefreshDevices(EDataFlow.eRender, false);

                            for (var i = 0; i < EndPoints.DeviceNames.Count; i++)
                                Console.WriteLine("  * " + i + ": " + EndPoints.DeviceNames[i]);
                            FreeConsole();
                            SendKeys.SendWait("{ENTER}");
                            break;

                        case "x":
                            return;
                    }
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var formSwitcher = new FormSwitcher();
            Application.Run();
        }
    }
}

