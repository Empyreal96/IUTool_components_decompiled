using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000048 RID: 72
	internal static class LongPathCommon
	{
		// Token: 0x060001E3 RID: 483 RVA: 0x00009999 File Offset: 0x00007B99
		internal static Exception GetExceptionFromLastWin32Error()
		{
			return LongPathCommon.GetExceptionFromLastWin32Error("path");
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x000099A5 File Offset: 0x00007BA5
		internal static Exception GetExceptionFromLastWin32Error(string parameterName)
		{
			return LongPathCommon.GetExceptionFromWin32Error(Marshal.GetLastWin32Error(), parameterName);
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x000099B2 File Offset: 0x00007BB2
		internal static Exception GetExceptionFromWin32Error(int errorCode)
		{
			return LongPathCommon.GetExceptionFromWin32Error(errorCode, "path");
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x000099C0 File Offset: 0x00007BC0
		internal static Exception GetExceptionFromWin32Error(int errorCode, string parameterName)
		{
			string messageFromErrorCode = LongPathCommon.GetMessageFromErrorCode(errorCode);
			if (errorCode <= 15)
			{
				switch (errorCode)
				{
				case 2:
					return new FileNotFoundException(messageFromErrorCode);
				case 3:
					return new DirectoryNotFoundException(messageFromErrorCode);
				case 4:
					break;
				case 5:
					return new UnauthorizedAccessException(messageFromErrorCode);
				default:
					if (errorCode == 15)
					{
						return new DriveNotFoundException(messageFromErrorCode);
					}
					break;
				}
			}
			else
			{
				if (errorCode == 123)
				{
					return new ArgumentException(messageFromErrorCode, parameterName);
				}
				if (errorCode == 206)
				{
					return new PathTooLongException(messageFromErrorCode);
				}
				if (errorCode == 995)
				{
					return new OperationCanceledException(messageFromErrorCode);
				}
			}
			return new IOException(messageFromErrorCode, NativeMethods.MakeHRFromErrorCode(errorCode));
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00009A50 File Offset: 0x00007C50
		private static string GetMessageFromErrorCode(int errorCode)
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			int dwFlags = 12800;
			IntPtr zero = IntPtr.Zero;
			int dwLanguageId = 0;
			StringBuilder stringBuilder2 = stringBuilder;
			NativeMethods.FormatMessage(dwFlags, zero, errorCode, dwLanguageId, stringBuilder2, stringBuilder2.Capacity, IntPtr.Zero);
			return stringBuilder.ToString();
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x00009A8C File Offset: 0x00007C8C
		internal static string[] ConvertPtrArrayToStringArray(IntPtr strPtrArray, int cStrings)
		{
			IntPtr[] array = new IntPtr[cStrings];
			if (strPtrArray != IntPtr.Zero)
			{
				Marshal.Copy(strPtrArray, array, 0, cStrings);
			}
			List<string> list = new List<string>(cStrings);
			for (int i = 0; i < cStrings; i++)
			{
				list.Add(Marshal.PtrToStringUni(array[i]));
			}
			return list.ToArray();
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x00009AE0 File Offset: 0x00007CE0
		public static string NormalizeLongPath(string path)
		{
			StringBuilder stringBuilder = new StringBuilder(LongPathCommon.MAX_LONG_PATH);
			StringBuilder stringBuilder2 = stringBuilder;
			int num = NativeMethods.IU_GetCanonicalUNCPath(path, stringBuilder2, stringBuilder2.Capacity);
			if (num != 0)
			{
				throw LongPathCommon.GetExceptionFromWin32Error(num);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060001EA RID: 490 RVA: 0x00009B16 File Offset: 0x00007D16
		public static FileAttributes GetAttributes(string path)
		{
			FileAttributes fileAttributes = NativeMethods.GetFileAttributes(LongPathCommon.NormalizeLongPath(path));
			if (fileAttributes == (FileAttributes)(-1))
			{
				throw LongPathCommon.GetExceptionFromLastWin32Error();
			}
			return fileAttributes;
		}

		// Token: 0x040000FA RID: 250
		private static int MAX_LONG_PATH = 32000;
	}
}
