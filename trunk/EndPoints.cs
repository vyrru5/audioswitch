using System;
using System.Collections.Generic;
using AudioSwitch.CoreAudioApi;
using AudioSwitch.Properties;

namespace AudioSwitch
{
    internal static class EndPoints
    {
        internal static readonly MMDeviceEnumerator pEnum = new MMDeviceEnumerator();
        internal static readonly List<string> DeviceNames = new List<string>();

        private static readonly BiDictionary<int, string> DeviceIDs = new BiDictionary<int, string>(); 
        private static readonly PolicyConfigClient pPolicyConfig = new PolicyConfigClient();
        
        internal static void RefreshDevices(EDataFlow renderType, bool updateIcons)
        {
            DeviceNames.Clear();
            DeviceIDs.Clear();

            if (updateIcons)
                DeviceIcons.Clear();

            var pDevices = pEnum.EnumerateAudioEndPoints(renderType, EDeviceState.DEVICE_STATE_ACTIVE);
            var devCount = pDevices.Count;

            for (var i = 0; i < devCount; i++)
            {
                var device = pDevices[i];
                DeviceNames.Add(device.FriendlyName);
                DeviceIDs.Add(i, device.ID);

                if (updateIcons)
                    DeviceIcons.Add(device.IconPath);
            }           
        }

        internal static int GetDefaultDevice(EDataFlow renderType)
        {
            return DeviceIDs.GetBySecond(pEnum.GetDefaultAudioEndpoint(renderType, ERole.eMultimedia).ID);
        }

        internal static void SetDefaultDevice(int devID)
        {
            try
            {
                pPolicyConfig.SetDefaultEndpoint(DeviceIDs.GetByFirst(devID), ERole.eMultimedia);
            }
            catch (Exception)
            {
                if (!Settings.Default.IgnoreErrors)
                    throw;
            }
        }
    }
}
