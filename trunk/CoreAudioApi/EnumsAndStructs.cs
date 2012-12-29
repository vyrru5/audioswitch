/*
  LICENSE
  -------
  Copyright (C) 2007-2010 Ray Molenkamp

  This source code is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this source code or the software it produces.

  Permission is granted to anyone to use this source code for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this source code must not be misrepresented; you must not
     claim that you wrote the original source code.  If you use this source code
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original source code.
  3. This notice may not be removed or altered from any source distribution.
*/

using System;

namespace AudioSwitch.CoreAudioApi
{
    public static class PKEY
    {
        public static readonly PropertyKey PKEY_DeviceInterface_FriendlyName = new PropertyKey { fmtid = new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), pid = 14 };
    }

    public struct PropertyKey
    {
        public Guid fmtid;
        public int pid;
    }

    public enum ERole
    {
        eConsole = 0,
        eMultimedia = 1,
        eCommunications = 2,
        ERole_enum_count = 3
    }

    [Flags]
    public enum EDeviceState : uint
    {
        DEVICE_STATE_ACTIVE = 0x00000001,
        DEVICE_STATE_UNPLUGGED = 0x00000002,
        DEVICE_STATE_NOTPRESENT = 0x00000004,
        DEVICE_STATEMASK_ALL = 0x00000007
    }

    public enum EDataFlow
    {
        eRender = 0,
        eCapture = 1,
        eAll = 2,
        EDataFlow_enum_count = 3
    }

    internal struct AUDIO_VOLUME_NOTIFICATION_DATA
    {
        public Guid guidEventContext;
        public bool bMuted;
        public float fMasterVolume;
        public uint nChannels;
        public float ChannelVolume;
    }

    internal struct Blob
    {
        public int Length;
        public IntPtr Data;
    }

    [Flags]
    internal enum CLSCTX : uint
    {
        INPROC_SERVER = 0x1,
        INPROC_HANDLER = 0x2,
        LOCAL_SERVER = 0x4,
        INPROC_SERVER16 = 0x8,
        REMOTE_SERVER = 0x10,
        INPROC_HANDLER16 = 0x20,
        RESERVED1 = 0x40,
        RESERVED2 = 0x80,
        RESERVED3 = 0x100,
        RESERVED4 = 0x200,
        NO_CODE_DOWNLOAD = 0x400,
        RESERVED5 = 0x800,
        NO_CUSTOM_MARSHAL = 0x1000,
        ENABLE_CODE_DOWNLOAD = 0x2000,
        NO_FAILURE_LOG = 0x4000,
        DISABLE_AAA = 0x8000,
        ENABLE_AAA = 0x10000,
        FROM_DEFAULT_CONTEXT = 0x20000,
        INPROC = INPROC_SERVER | INPROC_HANDLER,
        SERVER = INPROC_SERVER | LOCAL_SERVER | REMOTE_SERVER,
        ALL = SERVER | INPROC_HANDLER
    }

    internal enum EStgmAccess
    {
        STGM_READ = 0x00000000,
        STGM_WRITE = 0x00000001,
        STGM_READWRITE = 0x00000002
    }
}
