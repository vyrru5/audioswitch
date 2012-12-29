using System.Collections.Generic;
using AudioSwitch.CoreAudioApi;

namespace AudioSwitch
{
    static class EndPointControl
    {
        internal static readonly MMDeviceEnumerator pEnum = new MMDeviceEnumerator();
        private static readonly PolicyConfigVista pPolicyConfig = new PolicyConfigVista();

        internal static int GetDefaultDevice()
        {
            var pDevice = pEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            var pDevices = pEnum.EnumerateAudioEndPoints(EDataFlow.eRender, EDeviceState.DEVICE_STATE_ACTIVE);
            var count = pDevices.Count;
            var defaultID = pDevice.ID;

            for (var i = 0; i < count; i++)
                if (pDevices[i].ID == defaultID)
                    return i;
            return -1;
        }

        internal static void SetDefaultDevice(int devID)
        {
            var pDevices = pEnum.EnumerateAudioEndPoints(EDataFlow.eRender, EDeviceState.DEVICE_STATE_ACTIVE);
            pPolicyConfig.SetDefaultEndpoint(pDevices[devID].ID, ERole.eMultimedia);
        }

        internal static List<string> GetDevices()
        {
            var list = new List<string>();
            var pDevices = pEnum.EnumerateAudioEndPoints(EDataFlow.eRender, EDeviceState.DEVICE_STATE_ACTIVE);
            var devCount = pDevices.Count;

            for (var i = 0; i < devCount; i++)
                list.Add(pDevices[i].FriendlyName);
            return list;
        }
    }
}
