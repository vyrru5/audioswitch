using System;
using System.Collections.Generic;
using AudioSwitch.CoreAudioApi;

namespace AudioSwitch
{
    static class EndPoints
    {
        internal static readonly List<string> Icons = new List<string>(); 
        internal static readonly MMDeviceEnumerator pEnum = new MMDeviceEnumerator();
        private static readonly PolicyConfigClient pPolicyConfig = new PolicyConfigClient();
        
        internal static List<string> GetDevices(EDataFlow renderType)
        {
            var list = new List<string>();
            Icons.Clear();

            var pDevices = pEnum.EnumerateAudioEndPoints(renderType, EDeviceState.DEVICE_STATE_ACTIVE);
            var devCount = pDevices.Count;

            for (var i = 0; i < devCount; i++)
            {
                list.Add(pDevices[i].FriendlyName);
                var path = Environment.ExpandEnvironmentVariables(pDevices[i].IconPath);
                Icons.Add(path);
            }
            return list;
        }

        internal static int GetDefaultDevice(EDataFlow renderType)
        {
            var pDevice = pEnum.GetDefaultAudioEndpoint(renderType, ERole.eMultimedia);
            var pDevices = pEnum.EnumerateAudioEndPoints(renderType, EDeviceState.DEVICE_STATE_ACTIVE);
            var count = pDevices.Count;
            var defaultID = pDevice.ID;

            for (var i = 0; i < count; i++)
                if (pDevices[i].ID == defaultID)
                    return i;
            return -1;
        }

        internal static void SetDefaultDevice(int devID, EDataFlow renderType)
        {
            var pDevices = pEnum.EnumerateAudioEndPoints(renderType, EDeviceState.DEVICE_STATE_ACTIVE);
            pPolicyConfig.SetDefaultEndpoint(pDevices[devID].ID, ERole.eMultimedia);
        }
    }
}
