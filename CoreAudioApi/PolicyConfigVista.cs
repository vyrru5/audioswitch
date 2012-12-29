using System.Runtime.InteropServices;
using AudioSwitch.CoreAudioApi.Interfaces;

namespace AudioSwitch.CoreAudioApi
{
    public class PolicyConfigVista
    {
        [ComImport, Guid("294935CE-F637-4E7C-A41B-AB255460B862")]
        private class CPolicyConfigVistaClient
        {
        }

        private readonly IPolicyConfigVista _IPolicyConfigVista = new CPolicyConfigVistaClient() as IPolicyConfigVista;

        public void SetDefaultEndpoint(string wszDeviceId, ERole eRole)
        {
            Marshal.ThrowExceptionForHR(_IPolicyConfigVista.SetDefaultEndpoint(wszDeviceId, eRole));
        }
    }
}
