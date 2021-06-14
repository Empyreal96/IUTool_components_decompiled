using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.Tools.IO;

namespace Microsoft.Phone.Test.TestMetadata.Helper
{
	// Token: 0x0200000D RID: 13
	public class PortableExecutable : IDisposable
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600001B RID: 27 RVA: 0x0000237E File Offset: 0x0000057E
		// (set) Token: 0x0600001C RID: 28 RVA: 0x00002386 File Offset: 0x00000586
		public string FileName { get; private set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600001D RID: 29 RVA: 0x0000238F File Offset: 0x0000058F
		// (set) Token: 0x0600001E RID: 30 RVA: 0x00002397 File Offset: 0x00000597
		public string FullFileName { get; private set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600001F RID: 31 RVA: 0x000023A0 File Offset: 0x000005A0
		// (set) Token: 0x06000020 RID: 32 RVA: 0x000023A8 File Offset: 0x000005A8
		internal ImageNtHeaders NtHeaders { get; private set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000021 RID: 33 RVA: 0x000023B1 File Offset: 0x000005B1
		// (set) Token: 0x06000022 RID: 34 RVA: 0x000023B9 File Offset: 0x000005B9
		public List<string> Imports { get; private set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000023 RID: 35 RVA: 0x000023C2 File Offset: 0x000005C2
		// (set) Token: 0x06000024 RID: 36 RVA: 0x000023CA File Offset: 0x000005CA
		public List<string> DelayLoadImports { get; private set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000025 RID: 37 RVA: 0x000023D3 File Offset: 0x000005D3
		// (set) Token: 0x06000026 RID: 38 RVA: 0x000023DB File Offset: 0x000005DB
		public List<string> Exports { get; private set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000027 RID: 39 RVA: 0x000023E4 File Offset: 0x000005E4
		public bool IsPortableExecutableBinary
		{
			get
			{
				ushort num = (ushort)Marshal.ReadInt16(this._imageBase);
				bool flag = num != 23117;
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					bool flag2 = this.NtHeaders.Signature != 17744U;
					result = !flag2;
				}
				return result;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000028 RID: 40 RVA: 0x00002438 File Offset: 0x00000638
		public bool IsManaged
		{
			get
			{
				bool flag = !this.IsPortableExecutableBinary;
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					uint num = 0U;
					IntPtr zero = IntPtr.Zero;
					bool flag2 = NativeMethods.ImageDirectoryEntryToDataEx(this._imageBase, 0, 14, ref num, ref zero).ToInt32() == 0;
					result = !flag2;
				}
				return result;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00002494 File Offset: 0x00000694
		public bool IsNative
		{
			get
			{
				return !this.IsManaged;
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000024AF File Offset: 0x000006AF
		public PortableExecutable(string fileName)
		{
			this._fileName = "\\\\?\\" + fileName;
			this.Load();
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000024D4 File Offset: 0x000006D4
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Va")]
		public IntPtr ImageRvaToVa(uint relativeVirtualAddress)
		{
			return NativeMethods.ImageRvaToVa(this._imageNtHeader, this._imageBase, relativeVirtualAddress, IntPtr.Zero);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002500 File Offset: 0x00000700
		public void CopyData(uint relativeVirtualAddress, byte[] buffer, int startIndex, int length)
		{
			IntPtr source = NativeMethods.ImageRvaToVa(this._imageNtHeader, this._imageBase, relativeVirtualAddress, IntPtr.Zero);
			Marshal.Copy(source, buffer, startIndex, length);
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002534 File Offset: 0x00000734
		private void Load()
		{
			this._fileHandle = PortableExecutable.OpenFile(this._fileName);
			this.FullFileName = this._fileName;
			this.FileName = LongPathPath.GetFileName(this._fileName);
			uint dwMaximumSizeHigh = 0U;
			uint fileSize = PortableExecutable.GetFileSize(this._fileHandle, ref dwMaximumSizeHigh);
			this._mapFileHandle = NativeMethods.CreateFileMapping(this._fileHandle, IntPtr.Zero, 2U, dwMaximumSizeHigh, fileSize, null);
			int lastWin32Error = Marshal.GetLastWin32Error();
			bool flag = this._mapFileHandle.ToInt32() == 0 || this._mapFileHandle.ToInt32() == -1;
			if (flag)
			{
				throw new Win32Exception(lastWin32Error);
			}
			this._imageBase = NativeMethods.MapViewOfFile(this._mapFileHandle, 4U, 0U, 0U, IntPtr.Zero);
			lastWin32Error = Marshal.GetLastWin32Error();
			bool flag2 = this._imageBase.ToInt32() == 0;
			if (flag2)
			{
				throw new Win32Exception(lastWin32Error);
			}
			this._imageNtHeader = NativeMethods.ImageNtHeader(this._imageBase);
			lastWin32Error = Marshal.GetLastWin32Error();
			bool flag3 = this._imageNtHeader.ToInt32() == 0;
			if (flag3)
			{
				throw new Win32Exception(lastWin32Error);
			}
			this.NtHeaders = (ImageNtHeaders)Marshal.PtrToStructure(this._imageNtHeader, typeof(ImageNtHeaders));
			this.LoadImports();
			this.LoadDelayLoadImports();
			this.LoadExports();
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002670 File Offset: 0x00000870
		private void LoadImports()
		{
			bool flag = this.Imports == null;
			if (flag)
			{
				this.Imports = new List<string>();
			}
			else
			{
				this.Imports.Clear();
			}
			uint num = 0U;
			IntPtr zero = IntPtr.Zero;
			IntPtr intPtr = NativeMethods.ImageDirectoryEntryToDataEx(this._imageBase, 0, 1, ref num, ref zero);
			bool flag2 = intPtr.ToInt32() == 0;
			if (!flag2)
			{
				ImageImportDescriptor imageImportDescriptor = (ImageImportDescriptor)Marshal.PtrToStructure(intPtr, typeof(ImageImportDescriptor));
				for (;;)
				{
					bool flag3 = imageImportDescriptor.Name == 0U;
					if (flag3)
					{
						break;
					}
					IntPtr ptr = NativeMethods.ImageRvaToVa(this._imageNtHeader, this._imageBase, imageImportDescriptor.Name, IntPtr.Zero);
					string item = Marshal.PtrToStringAnsi(ptr);
					this.Imports.Add(item);
					intPtr = (IntPtr)((int)intPtr + Marshal.SizeOf(imageImportDescriptor));
					imageImportDescriptor = (ImageImportDescriptor)Marshal.PtrToStructure(intPtr, typeof(ImageImportDescriptor));
				}
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000276C File Offset: 0x0000096C
		private void LoadDelayLoadImports()
		{
			bool flag = this.DelayLoadImports == null;
			if (flag)
			{
				this.DelayLoadImports = new List<string>();
			}
			else
			{
				this.DelayLoadImports.Clear();
			}
			uint num = 0U;
			IntPtr zero = IntPtr.Zero;
			IntPtr intPtr = NativeMethods.ImageDirectoryEntryToDataEx(this._imageBase, 0, 13, ref num, ref zero);
			bool flag2 = intPtr.ToInt32() == 0;
			if (!flag2)
			{
				DelayLoadImportDescriptor delayLoadImportDescriptor = (DelayLoadImportDescriptor)Marshal.PtrToStructure(intPtr, typeof(DelayLoadImportDescriptor));
				for (;;)
				{
					bool flag3 = delayLoadImportDescriptor.DllName == 0U;
					if (flag3)
					{
						break;
					}
					IntPtr ptr = NativeMethods.ImageRvaToVa(this._imageNtHeader, this._imageBase, delayLoadImportDescriptor.DllName, IntPtr.Zero);
					string item = Marshal.PtrToStringAnsi(ptr);
					this.DelayLoadImports.Add(item);
					intPtr = (IntPtr)((int)intPtr + Marshal.SizeOf(delayLoadImportDescriptor));
					delayLoadImportDescriptor = (DelayLoadImportDescriptor)Marshal.PtrToStructure(intPtr, typeof(DelayLoadImportDescriptor));
				}
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x0000286C File Offset: 0x00000A6C
		private void LoadExports()
		{
			bool flag = this.Exports == null;
			if (flag)
			{
				this.Exports = new List<string>();
			}
			else
			{
				this.Exports.Clear();
			}
			uint num = 0U;
			IntPtr zero = IntPtr.Zero;
			IntPtr ptr = NativeMethods.ImageDirectoryEntryToDataEx(this._imageBase, 0, 0, ref num, ref zero);
			bool flag2 = ptr.ToInt32() == 0;
			if (!flag2)
			{
				ImageExportDirectory imageExportDirectory = (ImageExportDirectory)Marshal.PtrToStructure(ptr, typeof(ImageExportDirectory));
				IntPtr ptr2 = NativeMethods.ImageRvaToVa(this._imageNtHeader, this._imageBase, imageExportDirectory.AddressOfNames, IntPtr.Zero);
				bool flag3 = ptr2.ToInt32() == 0;
				if (!flag3)
				{
					int num2 = 0;
					while ((long)num2 < (long)((ulong)imageExportDirectory.NumberOfNames))
					{
						IntPtr ptr3 = NativeMethods.ImageRvaToVa(this._imageNtHeader, this._imageBase, (uint)Marshal.ReadInt32(ptr2), IntPtr.Zero);
						string item = Marshal.PtrToStringAnsi(ptr3);
						this.Exports.Add(item);
						ptr2 = (IntPtr)(ptr2.ToInt32() + Marshal.SizeOf(typeof(uint)));
						num2++;
					}
				}
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002994 File Offset: 0x00000B94
		private static uint GetFileSize(IntPtr fileHandle, ref uint fileSizeHigh)
		{
			uint fileSize = NativeMethods.GetFileSize(fileHandle, ref fileSizeHigh);
			bool flag = fileSize == uint.MaxValue || fileSizeHigh > 0U;
			if (flag)
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			return fileSize;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000029CC File Offset: 0x00000BCC
		private static IntPtr OpenFile(string fileName)
		{
			IntPtr result = NativeMethods.CreateFile(LongPathPath.GetFullPath(fileName), 2147483648U, 1U, IntPtr.Zero, 3U, 0U, IntPtr.Zero);
			int lastWin32Error = Marshal.GetLastWin32Error();
			bool flag = result.ToInt32() == -1;
			if (flag)
			{
				throw new Win32Exception(lastWin32Error);
			}
			return result;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002A1C File Offset: 0x00000C1C
		private static void CloseHandle(IntPtr fileHandle)
		{
			int num = NativeMethods.CloseHandle(fileHandle);
			bool flag = num != 1;
			if (flag)
			{
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002A3E File Offset: 0x00000C3E
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002A50 File Offset: 0x00000C50
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				bool flag = this._imageBase.ToInt32() != 0;
				if (flag)
				{
					int num = NativeMethods.UnmapViewOfFile(this._imageBase);
					bool flag2 = num != 1;
					if (flag2)
					{
					}
					this._imageBase = IntPtr.Zero;
				}
				bool flag3 = this._mapFileHandle.ToInt32() != 0 && -1 != this._mapFileHandle.ToInt32();
				if (flag3)
				{
					PortableExecutable.CloseHandle(this._mapFileHandle);
				}
				bool flag4 = this._fileHandle.ToInt32() != 0 && -1 != this._fileHandle.ToInt32();
				if (flag4)
				{
					PortableExecutable.CloseHandle(this._fileHandle);
				}
				this._imageNtHeader = IntPtr.Zero;
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002B18 File Offset: 0x00000D18
		~PortableExecutable()
		{
			this.Dispose(false);
		}

		// Token: 0x04000062 RID: 98
		private const ushort DosSignature = 23117;

		// Token: 0x04000063 RID: 99
		private const uint NtSignature = 17744U;

		// Token: 0x04000064 RID: 100
		private readonly string _fileName;

		// Token: 0x04000065 RID: 101
		private IntPtr _fileHandle;

		// Token: 0x04000066 RID: 102
		[SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
		private IntPtr _mapFileHandle;

		// Token: 0x04000067 RID: 103
		[SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
		private IntPtr _imageBase;

		// Token: 0x04000068 RID: 104
		[SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
		private IntPtr _imageNtHeader;
	}
}
