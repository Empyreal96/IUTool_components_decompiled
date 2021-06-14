using System;
using System.Diagnostics.Eventing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FFUComponents
{
	// Token: 0x02000025 RID: 37
	internal class DeviceEventProvider : EventProvider
	{
		// Token: 0x060000EB RID: 235 RVA: 0x00004A4A File Offset: 0x00002C4A
		internal DeviceEventProvider(Guid id) : base(id)
		{
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00004A54 File Offset: 0x00002C54
		internal unsafe bool TemplateDeviceEvent(ref EventDescriptor eventDescriptor, Guid DeviceUniqueId, string DeviceFriendlyName, string AdditionalInfoString)
		{
			int num = 3;
			bool result = true;
			if (base.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
			{
				byte* ptr = stackalloc byte[checked(unchecked((UIntPtr)(sizeof(DeviceEventProvider.EventData) * num)) * 1)];
				DeviceEventProvider.EventData* ptr2 = (DeviceEventProvider.EventData*)ptr;
				ptr2->DataPointer = &DeviceUniqueId;
				ptr2->Size = (uint)sizeof(Guid);
				ptr2[1].Size = (uint)((DeviceFriendlyName.Length + 1) * 2);
				ptr2[2].Size = (uint)((AdditionalInfoString.Length + 1) * 2);
				fixed (string text = DeviceFriendlyName)
				{
					char* ptr3 = text;
					if (ptr3 != null)
					{
						ptr3 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text2 = AdditionalInfoString)
					{
						char* ptr4 = text2;
						if (ptr4 != null)
						{
							ptr4 += RuntimeHelpers.OffsetToStringData / 2;
						}
						ptr2[1].DataPointer = ptr3;
						ptr2[2].DataPointer = ptr4;
						result = base.WriteEvent(ref eventDescriptor, num, (IntPtr)((void*)ptr));
						text = null;
					}
				}
			}
			return result;
		}

		// Token: 0x02000052 RID: 82
		[StructLayout(LayoutKind.Explicit, Size = 16)]
		private struct EventData
		{
			// Token: 0x0400019B RID: 411
			[FieldOffset(0)]
			internal ulong DataPointer;

			// Token: 0x0400019C RID: 412
			[FieldOffset(8)]
			internal uint Size;

			// Token: 0x0400019D RID: 413
			[FieldOffset(12)]
			internal int Reserved;
		}
	}
}
