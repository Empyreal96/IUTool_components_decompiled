using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Tools.IO;
using Microsoft.Win32.SafeHandles;

namespace System.Reflection.Adds
{
	// Token: 0x0200000B RID: 11
	internal class FileMapping : IDisposable
	{
		// Token: 0x06000067 RID: 103 RVA: 0x00003000 File Offset: 0x00001200
		public FileMapping(IntPtr baseAddress, long fileLength, string fileName)
		{
			this._fileName = fileName;
			this.Path = LongPathPath.GetFullPath(this._fileName);
			this.Length = fileLength;
			this.BaseAddress = baseAddress;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003030 File Offset: 0x00001230
		public FileMapping(string fileName)
		{
			this._fileName = fileName;
			this._fileHandle = NativeMethods.SafeOpenFile(fileName);
			this.Length = NativeMethods.FileSize(this._fileHandle);
			this.Path = LongPathPath.GetFullPath(this._fileName);
			this._fileMapping = NativeMethods.CreateFileMapping(this._fileHandle, IntPtr.Zero, NativeMethods.PageProtection.Readonly, 0U, 0U, null);
			bool isInvalid = this._fileMapping.IsInvalid;
			if (isInvalid)
			{
				int hrforLastWin32Error = Marshal.GetHRForLastWin32Error();
				Marshal.ThrowExceptionForHR(hrforLastWin32Error);
			}
			this._view = NativeMethods.MapViewOfFile(this._fileMapping, 4U, 0U, 0U, IntPtr.Zero);
			bool isInvalid2 = this._view.IsInvalid;
			if (isInvalid2)
			{
				int hrforLastWin32Error2 = Marshal.GetHRForLastWin32Error();
				Marshal.ThrowExceptionForHR(hrforLastWin32Error2);
			}
			this.BaseAddress = this._view.BaseAddress;
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000069 RID: 105 RVA: 0x000030FA File Offset: 0x000012FA
		public string Path { get; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00003102 File Offset: 0x00001302
		public long Length { get; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600006B RID: 107 RVA: 0x0000310A File Offset: 0x0000130A
		public IntPtr BaseAddress { get; }

		// Token: 0x0600006C RID: 108 RVA: 0x00003114 File Offset: 0x00001314
		public override string ToString()
		{
			bool flag = this.BaseAddress != IntPtr.Zero;
			string result;
			if (flag)
			{
				result = string.Format(CultureInfo.InvariantCulture, "{0} Addr=0x{1}, Length=0x{2}", new object[]
				{
					this._fileName,
					this.BaseAddress.ToString("x"),
					this.Length.ToString("x", CultureInfo.InvariantCulture)
				});
			}
			else
			{
				bool flag2 = this._view != null && this._view.IsInvalid;
				if (flag2)
				{
					result = this._fileName + " (closed)";
				}
				else
				{
					result = this._fileName;
				}
			}
			return result;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x000031C4 File Offset: 0x000013C4
		public void Dispose()
		{
			bool flag = this._view != null;
			if (flag)
			{
				this._view.Close();
			}
			bool flag2 = this._fileMapping != null;
			if (flag2)
			{
				this._fileMapping.Close();
			}
			bool flag3 = this._fileHandle != null;
			if (flag3)
			{
				this._fileHandle.Close();
			}
			GC.SuppressFinalize(this);
		}

		// Token: 0x04000032 RID: 50
		private readonly string _fileName;

		// Token: 0x04000033 RID: 51
		private readonly SafeFileHandle _fileHandle;

		// Token: 0x04000034 RID: 52
		private readonly NativeMethods.SafeWin32Handle _fileMapping;

		// Token: 0x04000035 RID: 53
		private readonly NativeMethods.SafeMapViewHandle _view;
	}
}
