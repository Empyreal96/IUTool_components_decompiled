using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000054 RID: 84
	internal static class VhdExtensions
	{
		// Token: 0x060003CA RID: 970 RVA: 0x000117D4 File Offset: 0x0000F9D4
		public static void WriteStruct<T>(this FileStream writer, ref T structure) where T : struct
		{
			int num = Marshal.SizeOf(typeof(T));
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			byte[] array = new byte[num];
			try
			{
				Marshal.StructureToPtr(structure, intPtr, false);
				Marshal.Copy(intPtr, array, 0, num);
				writer.Write(array, 0, num);
			}
			finally
			{
				if (IntPtr.Zero != intPtr)
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
		}

		// Token: 0x060003CB RID: 971 RVA: 0x0001184C File Offset: 0x0000FA4C
		public static T ReadStruct<T>(this FileStream reader) where T : struct
		{
			int num = Marshal.SizeOf(typeof(T));
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			byte[] array = new byte[num];
			T result;
			try
			{
				reader.Read(array, 0, num);
				Marshal.Copy(array, 0, intPtr, num);
				result = (T)((object)Marshal.PtrToStructure(intPtr, typeof(T)));
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
	}
}
