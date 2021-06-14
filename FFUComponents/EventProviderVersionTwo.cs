using System;
using System.Diagnostics.Eventing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FFUComponents
{
	// Token: 0x0200004A RID: 74
	internal class EventProviderVersionTwo : EventProvider
	{
		// Token: 0x0600021E RID: 542 RVA: 0x00004A4A File Offset: 0x00002C4A
		internal EventProviderVersionTwo(Guid id) : base(id)
		{
		}

		// Token: 0x0600021F RID: 543 RVA: 0x00009948 File Offset: 0x00007B48
		internal unsafe bool TemplateDeviceSpecificEventWithString(ref EventDescriptor eventDescriptor, Guid DeviceId, string DeviceFriendlyName, string AssemblyFileVersion)
		{
			int num = 3;
			bool result = true;
			if (base.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
			{
				byte* ptr = stackalloc byte[checked(unchecked((UIntPtr)(sizeof(EventProviderVersionTwo.EventData) * num)) * 1)];
				EventProviderVersionTwo.EventData* ptr2 = (EventProviderVersionTwo.EventData*)ptr;
				ptr2->DataPointer = &DeviceId;
				ptr2->Size = (uint)sizeof(Guid);
				ptr2[1].Size = (uint)((DeviceFriendlyName.Length + 1) * 2);
				ptr2[2].Size = (uint)((AssemblyFileVersion.Length + 1) * 2);
				fixed (string text = DeviceFriendlyName)
				{
					char* ptr3 = text;
					if (ptr3 != null)
					{
						ptr3 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text2 = AssemblyFileVersion)
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

		// Token: 0x06000220 RID: 544 RVA: 0x00009A30 File Offset: 0x00007C30
		internal unsafe bool TemplateDeviceSpecificEvent(ref EventDescriptor eventDescriptor, Guid DeviceId, string DeviceFriendlyName)
		{
			int num = 2;
			bool result = true;
			if (base.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
			{
				byte* ptr = stackalloc byte[checked(unchecked((UIntPtr)(sizeof(EventProviderVersionTwo.EventData) * num)) * 1)];
				EventProviderVersionTwo.EventData* ptr2 = (EventProviderVersionTwo.EventData*)ptr;
				ptr2->DataPointer = &DeviceId;
				ptr2->Size = (uint)sizeof(Guid);
				ptr2[1].Size = (uint)((DeviceFriendlyName.Length + 1) * 2);
				fixed (string text = DeviceFriendlyName)
				{
					char* ptr3 = text;
					if (ptr3 != null)
					{
						ptr3 += RuntimeHelpers.OffsetToStringData / 2;
					}
					ptr2[1].DataPointer = ptr3;
					result = base.WriteEvent(ref eventDescriptor, num, (IntPtr)((void*)ptr));
				}
			}
			return result;
		}

		// Token: 0x06000221 RID: 545 RVA: 0x00009ACC File Offset: 0x00007CCC
		internal unsafe bool TemplateDeviceEventWithErrorCode(ref EventDescriptor eventDescriptor, Guid DeviceId, string DeviceFriendlyName, int ErrorCode)
		{
			int num = 3;
			bool result = true;
			if (base.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
			{
				byte* ptr = stackalloc byte[checked(unchecked((UIntPtr)(sizeof(EventProviderVersionTwo.EventData) * num)) * 1)];
				EventProviderVersionTwo.EventData* ptr2 = (EventProviderVersionTwo.EventData*)ptr;
				ptr2->DataPointer = &DeviceId;
				ptr2->Size = (uint)sizeof(Guid);
				ptr2[1].Size = (uint)((DeviceFriendlyName.Length + 1) * 2);
				ptr2[2].DataPointer = &ErrorCode;
				ptr2[2].Size = 4U;
				fixed (string text = DeviceFriendlyName)
				{
					char* ptr3 = text;
					if (ptr3 != null)
					{
						ptr3 += RuntimeHelpers.OffsetToStringData / 2;
					}
					ptr2[1].DataPointer = ptr3;
					result = base.WriteEvent(ref eventDescriptor, num, (IntPtr)((void*)ptr));
				}
			}
			return result;
		}

		// Token: 0x06000222 RID: 546 RVA: 0x00009B90 File Offset: 0x00007D90
		internal unsafe bool TemplateNotifyException(ref EventDescriptor eventDescriptor, string DevicePath, string Exception)
		{
			int num = 2;
			bool result = true;
			if (base.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
			{
				byte* ptr = stackalloc byte[checked(unchecked((UIntPtr)(sizeof(EventProviderVersionTwo.EventData) * num)) * 1)];
				EventProviderVersionTwo.EventData* ptr2 = (EventProviderVersionTwo.EventData*)ptr;
				ptr2->Size = (uint)((DevicePath.Length + 1) * 2);
				ptr2[1].Size = (uint)((Exception.Length + 1) * 2);
				fixed (string text = DevicePath)
				{
					char* ptr3 = text;
					if (ptr3 != null)
					{
						ptr3 += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (string text2 = Exception)
					{
						char* ptr4 = text2;
						if (ptr4 != null)
						{
							ptr4 += RuntimeHelpers.OffsetToStringData / 2;
						}
						ptr2->DataPointer = ptr3;
						ptr2[1].DataPointer = ptr4;
						result = base.WriteEvent(ref eventDescriptor, num, (IntPtr)((void*)ptr));
						text = null;
					}
				}
			}
			return result;
		}

		// Token: 0x06000223 RID: 547 RVA: 0x00009C4C File Offset: 0x00007E4C
		internal unsafe bool TemplateDeviceSpecificEventWithSize(ref EventDescriptor eventDescriptor, Guid DeviceId, string DeviceFriendlyName, int TransferSize)
		{
			int num = 3;
			bool result = true;
			if (base.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
			{
				byte* ptr = stackalloc byte[checked(unchecked((UIntPtr)(sizeof(EventProviderVersionTwo.EventData) * num)) * 1)];
				EventProviderVersionTwo.EventData* ptr2 = (EventProviderVersionTwo.EventData*)ptr;
				ptr2->DataPointer = &DeviceId;
				ptr2->Size = (uint)sizeof(Guid);
				ptr2[1].Size = (uint)((DeviceFriendlyName.Length + 1) * 2);
				ptr2[2].DataPointer = &TransferSize;
				ptr2[2].Size = 4U;
				fixed (string text = DeviceFriendlyName)
				{
					char* ptr3 = text;
					if (ptr3 != null)
					{
						ptr3 += RuntimeHelpers.OffsetToStringData / 2;
					}
					ptr2[1].DataPointer = ptr3;
					result = base.WriteEvent(ref eventDescriptor, num, (IntPtr)((void*)ptr));
				}
			}
			return result;
		}

		// Token: 0x06000224 RID: 548 RVA: 0x00009D10 File Offset: 0x00007F10
		internal unsafe bool TemplateDeviceFlashParameters(ref EventDescriptor eventDescriptor, int USBTransactionSize, int PacketDataSize)
		{
			int num = 2;
			bool result = true;
			if (base.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
			{
				byte* ptr = stackalloc byte[checked(unchecked((UIntPtr)(sizeof(EventProviderVersionTwo.EventData) * num)) * 1)];
				EventProviderVersionTwo.EventData* ptr2 = (EventProviderVersionTwo.EventData*)ptr;
				ptr2->DataPointer = &USBTransactionSize;
				ptr2->Size = 4U;
				ptr2[1].DataPointer = &PacketDataSize;
				ptr2[1].Size = 4U;
				result = base.WriteEvent(ref eventDescriptor, num, (IntPtr)((void*)ptr));
			}
			return result;
		}

		// Token: 0x02000056 RID: 86
		[StructLayout(LayoutKind.Explicit, Size = 16)]
		private struct EventData
		{
			// Token: 0x040001C1 RID: 449
			[FieldOffset(0)]
			internal ulong DataPointer;

			// Token: 0x040001C2 RID: 450
			[FieldOffset(8)]
			internal uint Size;

			// Token: 0x040001C3 RID: 451
			[FieldOffset(12)]
			internal int Reserved;
		}
	}
}
