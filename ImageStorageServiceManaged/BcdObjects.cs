using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000002 RID: 2
	public static class BcdObjects
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static Guid IdFromName(string objectName)
		{
			Guid guid = Guid.Empty;
			foreach (Guid guid2 in BcdObjects.BootObjectList.Keys)
			{
				if (string.Compare(objectName, BcdObjects.BootObjectList[guid2].Name, true, CultureInfo.InvariantCulture) == 0)
				{
					guid = guid2;
					break;
				}
			}
			if (guid == Guid.Empty)
			{
				throw new ImageStorageException(string.Format("{0}: '{1}' doesn't have an associated Id.", MethodBase.GetCurrentMethod().Name, objectName));
			}
			return guid;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020F4 File Offset: 0x000002F4
		[CLSCompliant(false)]
		public static uint ObjectTypeFromName(string objectName)
		{
			uint num = 0U;
			foreach (Guid key in BcdObjects.BootObjectList.Keys)
			{
				if (string.Compare(objectName, BcdObjects.BootObjectList[key].Name, true, CultureInfo.InvariantCulture) == 0)
				{
					num = BcdObjects.BootObjectList[key].Type;
					break;
				}
			}
			if (num == 0U)
			{
				throw new ImageStorageException(string.Format("{0}: '{1}' doesn't have a known type.", MethodBase.GetCurrentMethod().Name, objectName));
			}
			return num;
		}

		// Token: 0x04000001 RID: 1
		public static readonly Guid BootManager = new Guid("{9dea862c-5cdd-4e70-acc1-f32b344d4795}");

		// Token: 0x04000002 RID: 2
		public static readonly Guid WindowsLoader = new Guid("{7619dcc9-fafe-11d9-b411-000476eba25f}");

		// Token: 0x04000003 RID: 3
		public static readonly Guid FullFlashUpdateApp = new Guid("{0ff5f24a-3785-4aeb-b8fe-4226215b88c4}");

		// Token: 0x04000004 RID: 4
		public static readonly Guid MobileStartupApp = new Guid("{01de5a27-8705-40db-bad6-96fa5187d4a6}");

		// Token: 0x04000005 RID: 5
		public static readonly Guid UpdateOSWim = new Guid("{311b88b5-9b30-491d-bad9-167ca3e2d417}");

		// Token: 0x04000006 RID: 6
		public static readonly Guid BootManagerSettingsGroup = new Guid("{6efb52bf-1766-41db-a6b3-0ee5eff72bd7}");

		// Token: 0x04000007 RID: 7
		public static readonly Guid FirmwareBootMgr = new Guid("{a5a30fa2-3d06-4e9f-b5f4-a01df9d1fcba}");

		// Token: 0x04000008 RID: 8
		public static readonly Guid WindowsMemoryTester = new Guid("{b2721d73-1db4-4c62-bf78-c548a880142d}");

		// Token: 0x04000009 RID: 9
		public static readonly Guid WindowsResumeApp = new Guid("{147aa509-0358-4473-b83b-d950dda00615}");

		// Token: 0x0400000A RID: 10
		public static readonly Guid WindowsResumeTargetTemplateEfi = new Guid("{0c334284-9a41-4de1-99b3-a7e87e8ff07e}");

		// Token: 0x0400000B RID: 11
		public static readonly Guid WindowsResumeTargetTemplatePcat = new Guid("{98b02a23-0674-4ce7-bdad-e0a15a8ff97b}");

		// Token: 0x0400000C RID: 12
		public static readonly Guid WindowsLegacyNtLdr = new Guid("{466f5a88-0af2-4f76-9038-095b170dc21c}");

		// Token: 0x0400000D RID: 13
		public static readonly Guid WindowsSetupPcat = new Guid("{cbd971bf-b7b8-4885-951a-fa03044f5d71}");

		// Token: 0x0400000E RID: 14
		public static readonly Guid WindowsSetupEfi = new Guid("{7254a080-1510-4e85-ac0f-e7fb3d444736}");

		// Token: 0x0400000F RID: 15
		public static readonly Guid WindowsOsTargetTemplatePcat = new Guid("{a1943bbc-ea85-487c-97c7-c9ede908a38a}");

		// Token: 0x04000010 RID: 16
		public static readonly Guid WindowsOsTargetTemplateEfi = new Guid("{b012b84d-c47c-4ed5-b722-c0c42163e569}");

		// Token: 0x04000011 RID: 17
		public static readonly Guid WindowsSetupRamdiskOptions = new Guid("{ae5534e0-a924-466c-b836-758539a3ee3a}");

		// Token: 0x04000012 RID: 18
		public static readonly Guid CurrentBootEntry = new Guid("{fa926493-6f1c-4193-a414-58f0b2456d1e}");

		// Token: 0x04000013 RID: 19
		public static readonly Guid DefaultBootEntry = new Guid("{1cae1eb7-a0df-4d4d-9851-4860e34ef535}");

		// Token: 0x04000014 RID: 20
		public static readonly Guid BadMemoryGroup = new Guid("{5189b25c-5558-4bf2-bca4-289b11bd29e2}");

		// Token: 0x04000015 RID: 21
		public static readonly Guid DebuggerSettingsGroup = new Guid("{4636856e-540f-4170-a130-a84776f4c654}");

		// Token: 0x04000016 RID: 22
		public static readonly Guid EmsSettingsGroup = new Guid("{0ce4991b-e6b3-4b16-b23c-5e0d9250e5d9}");

		// Token: 0x04000017 RID: 23
		public static readonly Guid GlobalSettingsGroup = new Guid("{7ea2e1ac-2e61-4728-aaa3-896d9d0a9f0e}");

		// Token: 0x04000018 RID: 24
		public static readonly Guid ResumeLoaderSettingsGroup = new Guid("{1afa9c49-16ab-4a5c-901b-212802da9460}");

		// Token: 0x04000019 RID: 25
		public static readonly Guid HypervisorSettingsGroup = new Guid("{7ff607e0-4395-11db-b0de-0800200c9a66}");

		// Token: 0x0400001A RID: 26
		public static readonly Guid ResetPhoneApp = new Guid("{BD8951C4-EABD-4c6f-AAFB-4DDB4EB0469B}");

		// Token: 0x0400001B RID: 27
		public static readonly Guid PrebootCrashDumpApp = new Guid("{012cdeb8-68fe-4fb9-be98-dbf20d98c261}");

		// Token: 0x0400001C RID: 28
		public static readonly Guid MMOSLoader = new Guid("{874EF8BB-D20F-4364-B545-A36E88EC40B0}");

		// Token: 0x0400001D RID: 29
		public static readonly Guid MMOSWim = new Guid("{A5935FF2-32BA-4617-BF36-5AC314B3F9BF}");

		// Token: 0x0400001E RID: 30
		public static readonly Guid DeveloperMenuApp = new Guid("{0D1B5E40-42F1-41e7-A690-8DD3CE23CC11}");

		// Token: 0x0400001F RID: 31
		public static readonly Guid RelockApp = new Guid("{F8E167D7-BEDF-41FF-A32E-27043A83CC89}");

		// Token: 0x04000020 RID: 32
		public static readonly Guid HoloLensDisplayApp = new Guid("{17B80A47-C57B-460F-AF39-9BD2D4080A26}");

		// Token: 0x04000021 RID: 33
		public static readonly Guid PhoneOsBoot = new Guid("{00000000-0000-0000-0000-000000000001}");

		// Token: 0x04000022 RID: 34
		public static readonly Dictionary<Guid, BcdObjects.BootObjectInfo> BootObjectList = new Dictionary<Guid, BcdObjects.BootObjectInfo>
		{
			{
				BcdObjects.BootManager,
				new BcdObjects.BootObjectInfo("Windows Boot Manager", 269484034U)
			},
			{
				BcdObjects.WindowsLoader,
				new BcdObjects.BootObjectInfo("Windows Loader", 270532611U)
			},
			{
				BcdObjects.FirmwareBootMgr,
				new BcdObjects.BootObjectInfo("Firmware BootMgr", 270532611U)
			},
			{
				BcdObjects.UpdateOSWim,
				new BcdObjects.BootObjectInfo("Windows Phone Update OS", 270532611U)
			},
			{
				BcdObjects.MMOSWim,
				new BcdObjects.BootObjectInfo("MMOS", 270532611U)
			},
			{
				BcdObjects.MobileStartupApp,
				new BcdObjects.BootObjectInfo("Mobile Startup App", 270532618U)
			},
			{
				BcdObjects.FullFlashUpdateApp,
				new BcdObjects.BootObjectInfo("Full Flash Update", 270532618U)
			},
			{
				BcdObjects.ResetPhoneApp,
				new BcdObjects.BootObjectInfo("Reset My Phone App", 270532618U)
			},
			{
				BcdObjects.PrebootCrashDumpApp,
				new BcdObjects.BootObjectInfo("Preboot Crash Dump Application", 270532618U)
			},
			{
				BcdObjects.MMOSLoader,
				new BcdObjects.BootObjectInfo("MMOS Launcher App", 270532618U)
			},
			{
				BcdObjects.DeveloperMenuApp,
				new BcdObjects.BootObjectInfo("Developer Menu App", 270532618U)
			},
			{
				BcdObjects.RelockApp,
				new BcdObjects.BootObjectInfo("Relock App", 270532618U)
			},
			{
				BcdObjects.HoloLensDisplayApp,
				new BcdObjects.BootObjectInfo("HoloLens Display Initialization App", 270532618U)
			},
			{
				BcdObjects.WindowsMemoryTester,
				new BcdObjects.BootObjectInfo("Windows Memory Tester", 270532613U)
			},
			{
				BcdObjects.WindowsResumeApp,
				new BcdObjects.BootObjectInfo("Windows Resume App", 270532612U)
			},
			{
				BcdObjects.DebuggerSettingsGroup,
				new BcdObjects.BootObjectInfo("Debugger Settings Group", 537919488U)
			},
			{
				BcdObjects.EmsSettingsGroup,
				new BcdObjects.BootObjectInfo("EMS Settings Group", 537919488U)
			},
			{
				BcdObjects.GlobalSettingsGroup,
				new BcdObjects.BootObjectInfo("Global Settings Group", 537919488U)
			},
			{
				BcdObjects.HypervisorSettingsGroup,
				new BcdObjects.BootObjectInfo("Hypervisor Settings Group", 538968067U)
			},
			{
				BcdObjects.BootManagerSettingsGroup,
				new BcdObjects.BootObjectInfo("Boot Loader Settings Group", 538968067U)
			},
			{
				BcdObjects.WindowsSetupRamdiskOptions,
				new BcdObjects.BootObjectInfo("Ramdisk Options", 805306372U)
			}
		};

		// Token: 0x02000075 RID: 117
		public struct BootObjectInfo
		{
			// Token: 0x060004D0 RID: 1232 RVA: 0x00014DA7 File Offset: 0x00012FA7
			[CLSCompliant(false)]
			public BootObjectInfo(string name, uint type)
			{
				this.Name = name;
				this.Type = type;
			}

			// Token: 0x0400029F RID: 671
			public string Name;

			// Token: 0x040002A0 RID: 672
			[CLSCompliant(false)]
			public uint Type;
		}
	}
}
