using System;
using System.Runtime.InteropServices;
using AudioSwitch.CoreAudioApi.Interfaces;

namespace AudioSwitch.CoreAudioApi
{
    class VolEventsHandler : IAudioSessionEvents
    {
        readonly ProgressTrackBar volumebar;

        public VolEventsHandler(ProgressTrackBar VolumeBar)
        {
            volumebar = VolumeBar;
        }

        delegate void MethodWithFloatArg(float Float);

        private void SafeSet(float Volume)
        {
            if (volumebar.InvokeRequired) 
                volumebar.Invoke(new MethodWithFloatArg(SafeSet), Volume);
            else
            {
                volumebar.Value = (int)(Volume * 100);
            }
        }

        [PreserveSig]
        public int OnDisplayNameChanged([MarshalAs(UnmanagedType.LPWStr)] string NewDisplayName, Guid EventContext) { return 0; }
        [PreserveSig]
        public int OnIconPathChanged([MarshalAs(UnmanagedType.LPWStr)] string NewIconPath, Guid EventContext) { return 0; }
        [PreserveSig]
        public int OnSimpleVolumeChanged(float NewVolume, bool newMute, Guid EventContext) { SafeSet(NewVolume); return 0; }
        [PreserveSig]
        public int OnChannelVolumeChanged(UInt32 ChannelCount, IntPtr NewChannelVolumeArray, UInt32 ChangedChannel, Guid EventContext) { return 0; }
        [PreserveSig]
        public int OnGroupingParamChanged(Guid NewGroupingParam, Guid EventContext) { return 0; }
        [PreserveSig]
        public int OnStateChanged(AudioSessionState NewState) { return 0; }
        [PreserveSig]
        public int OnSessionDisconnected(AudioSessionDisconnectReason DisconnectReason) { return 0; }
    }
}
