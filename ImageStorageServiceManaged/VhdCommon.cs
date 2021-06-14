using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000053 RID: 83
	internal static class VhdCommon
	{
		// Token: 0x060003C5 RID: 965 RVA: 0x00011707 File Offset: 0x0000F907
		public static uint Swap32(uint data)
		{
			return (uint)IPAddress.HostToNetworkOrder((int)data);
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x0001170F File Offset: 0x0000F90F
		public static ulong Swap64(ulong data)
		{
			return (ulong)IPAddress.HostToNetworkOrder((long)data);
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x00011718 File Offset: 0x0000F918
		public static uint CalculateChecksum<T>(ref T type) where T : struct
		{
			uint num = 0U;
			int num2 = Marshal.SizeOf(typeof(T));
			IntPtr intPtr = Marshal.AllocHGlobal(num2);
			byte[] array = new byte[num2];
			uint result;
			try
			{
				Marshal.StructureToPtr(type, intPtr, false);
				Marshal.Copy(intPtr, array, 0, num2);
				foreach (byte b in array)
				{
					num += (uint)b;
				}
				num = ~num;
				result = num;
			}
			finally
			{
				if (IntPtr.Zero != intPtr)
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
			return result;
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x000117B0 File Offset: 0x0000F9B0
		public static uint Round(uint number, uint roundTo)
		{
			return (number + roundTo - 1U) / roundTo * roundTo;
		}

		// Token: 0x040001FF RID: 511
		public static uint VHDSectorSize = 512U;

		// Token: 0x04000200 RID: 512
		public static uint DynamicVHDBlockSize = 2097152U;
	}
}
