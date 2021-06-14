using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000010 RID: 16
	internal static class LongPathCommon
	{
		// Token: 0x0600007D RID: 125 RVA: 0x00003702 File Offset: 0x00001902
		internal static Exception GetExceptionFromLastWin32Error()
		{
			return LongPathCommon.GetExceptionFromLastWin32Error("path");
		}

		// Token: 0x0600007E RID: 126 RVA: 0x0000370E File Offset: 0x0000190E
		internal static Exception GetExceptionFromLastWin32Error(string parameterName)
		{
			return LongPathCommon.GetExceptionFromWin32Error(Marshal.GetLastWin32Error(), parameterName);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x0000371B File Offset: 0x0000191B
		internal static Exception GetExceptionFromWin32Error(int errorCode)
		{
			return LongPathCommon.GetExceptionFromWin32Error(errorCode, "path");
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00003728 File Offset: 0x00001928
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

		// Token: 0x06000081 RID: 129 RVA: 0x000037B8 File Offset: 0x000019B8
		[SuppressMessage("Microsoft.Usage", "CA1806")]
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

		// Token: 0x06000082 RID: 130 RVA: 0x000037F4 File Offset: 0x000019F4
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

		// Token: 0x06000083 RID: 131 RVA: 0x00003848 File Offset: 0x00001A48
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

		// Token: 0x06000084 RID: 132 RVA: 0x0000387E File Offset: 0x00001A7E
		public static FileAttributes GetAttributes(string path)
		{
			FileAttributes fileAttributes = NativeMethods.GetFileAttributes(LongPathCommon.NormalizeLongPath(path));
			if (fileAttributes == (FileAttributes)(-1))
			{
				throw LongPathCommon.GetExceptionFromLastWin32Error();
			}
			return fileAttributes;
		}

		// Token: 0x0400002A RID: 42
		private static int MAX_LONG_PATH = 32000;
	}
}
