using System.Runtime.InteropServices;
using AudioSwitch.CoreAudioApi.Interfaces.CoreAudioApi.Interfaces;

namespace AudioSwitch.CoreAudioApi
{
    public class PolicyConfigClient
    {
        [ComImport, Guid("870af99c-171d-4f9e-af0d-e63df40c2bc9")]
        private class _PolicyConfigClient
        {
        }

        private readonly IPolicyConfig _PolicyConfig = new _PolicyConfigClient() as IPolicyConfig;

        public void SetDefaultEndpoint(string devID, ERole eRole)
        {
            Marshal.ThrowExceptionForHR(_PolicyConfig.SetDefaultEndpoint(devID, eRole));
        }
    }
}