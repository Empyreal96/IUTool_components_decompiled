using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.Imaging.WimInterop
{
	// Token: 0x02000003 RID: 3
	public sealed class WindowsImageContainer : IDisposable
	{
		// Token: 0x06000006 RID: 6 RVA: 0x00002050 File Offset: 0x00000250
		public WindowsImageContainer(string imageFilePath, WindowsImageContainer.CreateFileMode mode, WindowsImageContainer.CreateFileAccess access, WindowsImageContainer.CreateFileCompression compress)
		{
			WindowsImageContainer.CreateFileAccessPrivate mappedFileAccess = this.GetMappedFileAccess(access);
			if (mappedFileAccess == (WindowsImageContainer.CreateFileAccessPrivate)2147483648U && (!File.Exists(imageFilePath) || WindowsImageContainer.CreateFileMode.OpenExisting != mode))
			{
				throw new UnauthorizedAccessException(string.Format(CultureInfo.CurrentCulture, "Read access can be specified only with OpenExisting mode or OpenAlways mode when the .wim file does not exist.", new object[0]));
			}
			try
			{
				this._imageContainerHandle = WindowsImageContainer.NativeMethods.CreateFile(imageFilePath, (uint)mappedFileAccess, (uint)mode, (uint)compress);
				this._windowsImageFilePath = imageFilePath;
			}
			catch (DllNotFoundException ex)
			{
				throw new DllNotFoundException(string.Format(CultureInfo.CurrentCulture, "Unable to load WIM libraries. Make sure the correct DLLs are present (Wimgapi.dll and Xmlrw.dll).", new object[0]), ex.InnerException);
			}
			if (this._imageContainerHandle.Equals(IntPtr.Zero))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unable to open  the .wim file {0}.", new object[]
				{
					imageFilePath
				}));
			}
			string environmentVariable = Environment.GetEnvironmentVariable("BUILD_PRODUCT");
			string text = Environment.GetEnvironmentVariable("OBJECT_ROOT");
			if ((!string.IsNullOrEmpty(environmentVariable) && environmentVariable.Equals("nt", StringComparison.OrdinalIgnoreCase)) || string.IsNullOrEmpty(text))
			{
				text = Path.GetTempPath();
			}
			WindowsImageContainer.NativeMethods.SetTemporaryPath(this._imageContainerHandle, text);
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < this.ImageCount; i++)
			{
				arrayList.Add(new WindowsImageContainer.WindowsImage(this._imageContainerHandle, this._windowsImageFilePath, i + 1));
			}
			this._images = (WindowsImageContainer.WindowsImage[])arrayList.ToArray(typeof(WindowsImageContainer.WindowsImage));
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021BC File Offset: 0x000003BC
		~WindowsImageContainer()
		{
			this.DisposeInner();
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000021E8 File Offset: 0x000003E8
		public void Dispose()
		{
			this.DisposeInner();
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000021F8 File Offset: 0x000003F8
		private void DisposeInner()
		{
			if (this._images != null)
			{
				foreach (WindowsImageContainer.WindowsImage windowsImage in this._images)
				{
					if (windowsImage != null)
					{
						windowsImage.Dispose();
					}
				}
			}
			if (this._imageContainerHandle != IntPtr.Zero)
			{
				WindowsImageContainer.NativeMethods.CloseHandle(this._imageContainerHandle);
				this._imageContainerHandle = IntPtr.Zero;
			}
			GC.KeepAlive(this);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000225D File Offset: 0x0000045D
		public IEnumerator GetEnumerator()
		{
			if (this._images == null)
			{
				return new ArrayList().GetEnumerator();
			}
			return this._images.GetEnumerator();
		}

		// Token: 0x17000002 RID: 2
		public IImage this[int imageIndex]
		{
			get
			{
				if (this._images == null || this._images[imageIndex] == null)
				{
					this._images = (WindowsImageContainer.WindowsImage[])new ArrayList
					{
						new WindowsImageContainer.WindowsImage(this._imageContainerHandle, this._windowsImageFilePath, imageIndex + 1)
					}.ToArray(typeof(WindowsImageContainer.WindowsImage));
				}
				GC.KeepAlive(this);
				return this._images[imageIndex];
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000022E9 File Offset: 0x000004E9
		public void SetBootImage(int imageIndex)
		{
			WindowsImageContainer.NativeMethods.SetBootImage(this._imageContainerHandle, imageIndex);
			GC.KeepAlive(this);
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000D RID: 13 RVA: 0x000022FE File Offset: 0x000004FE
		public int ImageCount
		{
			get
			{
				if (this._imageCount == 0)
				{
					this._imageCount = WindowsImageContainer.NativeMethods.GetImageCount(this._imageContainerHandle);
				}
				GC.KeepAlive(this);
				return this._imageCount;
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002328 File Offset: 0x00000528
		public void CaptureImage(string pathToCapture)
		{
			int imageIndex = this.ImageCount + 1;
			WindowsImageContainer.NativeMethods.CloseHandle(WindowsImageContainer.NativeMethods.CaptureImage(this._imageContainerHandle, pathToCapture));
			GC.KeepAlive(this);
			ArrayList arrayList = new ArrayList();
			if (this._images != null)
			{
				foreach (WindowsImageContainer.WindowsImage value in this._images)
				{
					arrayList.Add(value);
				}
			}
			arrayList.Add(new WindowsImageContainer.WindowsImage(this._imageContainerHandle, this._windowsImageFilePath, imageIndex));
			this._images = (WindowsImageContainer.WindowsImage[])arrayList.ToArray(typeof(WindowsImageContainer.WindowsImage));
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000023BC File Offset: 0x000005BC
		private WindowsImageContainer.CreateFileAccessPrivate GetMappedFileAccess(WindowsImageContainer.CreateFileAccess access)
		{
			WindowsImageContainer.CreateFileAccessPrivate result;
			if (access != WindowsImageContainer.CreateFileAccess.Read)
			{
				if (access != WindowsImageContainer.CreateFileAccess.Write)
				{
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "No file access level specified.", new object[0]));
				}
				result = (WindowsImageContainer.CreateFileAccessPrivate)3758096384U;
			}
			else
			{
				result = (WindowsImageContainer.CreateFileAccessPrivate)2684354560U;
			}
			return result;
		}

		// Token: 0x04000001 RID: 1
		private IntPtr _imageContainerHandle;

		// Token: 0x04000002 RID: 2
		private string _windowsImageFilePath;

		// Token: 0x04000003 RID: 3
		private WindowsImageContainer.WindowsImage[] _images;

		// Token: 0x04000004 RID: 4
		private int _imageCount;

		// Token: 0x02000006 RID: 6
		public enum CreateFileAccess
		{
			// Token: 0x0400000C RID: 12
			Read,
			// Token: 0x0400000D RID: 13
			Write
		}

		// Token: 0x02000007 RID: 7
		public enum CreateFileMode
		{
			// Token: 0x0400000F RID: 15
			None,
			// Token: 0x04000010 RID: 16
			CreateNew,
			// Token: 0x04000011 RID: 17
			CreateAlways,
			// Token: 0x04000012 RID: 18
			OpenExisting,
			// Token: 0x04000013 RID: 19
			OpenAlways
		}

		// Token: 0x02000008 RID: 8
		public enum CreateFileCompression
		{
			// Token: 0x04000015 RID: 21
			WIM_COMPRESS_NONE,
			// Token: 0x04000016 RID: 22
			WIM_COMPRESS_XPRESS,
			// Token: 0x04000017 RID: 23
			WIM_COMPRESS_LZX,
			// Token: 0x04000018 RID: 24
			WIM_COMPRESS_LZMS
		}

		// Token: 0x02000009 RID: 9
		private class WindowsImage : IImage, IDisposable
		{
			// Token: 0x06000019 RID: 25 RVA: 0x000024A4 File Offset: 0x000006A4
			public WindowsImage(IntPtr imageContainerHandle, string imageContainerFilePath, int imageIndex)
			{
				this._parentWindowsImageHandle = imageContainerHandle;
				this._parentWindowsImageFilePath = imageContainerFilePath;
				this._index = imageIndex;
				this._imageHandle = WindowsImageContainer.NativeMethods.LoadImage(imageContainerHandle, imageIndex);
			}

			// Token: 0x0600001A RID: 26 RVA: 0x000024E4 File Offset: 0x000006E4
			~WindowsImage()
			{
				this.DisposeInner();
			}

			// Token: 0x0600001B RID: 27 RVA: 0x00002510 File Offset: 0x00000710
			public void Dispose()
			{
				this.DisposeInner();
				GC.SuppressFinalize(this);
			}

			// Token: 0x0600001C RID: 28 RVA: 0x00002520 File Offset: 0x00000720
			private void DisposeInner()
			{
				if (this._mounted)
				{
					this.DismountImage();
					this._mounted = false;
				}
				if (this._imageHandle != IntPtr.Zero)
				{
					WindowsImageContainer.NativeMethods.CloseHandle(this._imageHandle);
					this._imageHandle = IntPtr.Zero;
				}
				GC.KeepAlive(this);
			}

			// Token: 0x17000009 RID: 9
			// (get) Token: 0x0600001D RID: 29 RVA: 0x00002570 File Offset: 0x00000770
			public string MountedPath
			{
				get
				{
					if (this._mountedPath == null)
					{
						return null;
					}
					return this._mountedPath;
				}
			}

			// Token: 0x0600001E RID: 30 RVA: 0x00002584 File Offset: 0x00000784
			public void Apply(string pathToApplyTo)
			{
				this._mountedPath = pathToApplyTo;
				uint applyFlags = 0U;
				WindowsImageContainer.NativeMethods.ApplyImage(this._imageHandle, pathToApplyTo, applyFlags);
			}

			// Token: 0x0600001F RID: 31 RVA: 0x000025A8 File Offset: 0x000007A8
			public void Mount(string pathToMountTo, bool isReadOnly)
			{
				this._mountedPath = pathToMountTo;
				uint num = 0U;
				if (isReadOnly)
				{
					num |= 512U;
				}
				WindowsImageContainer.NativeMethods.MountImage(this._imageHandle, pathToMountTo, num);
				this._mounted = true;
			}

			// Token: 0x06000020 RID: 32 RVA: 0x000025DD File Offset: 0x000007DD
			public void DismountImage()
			{
				if (this._mounted)
				{
					WindowsImageContainer.NativeMethods.DismountImage(this._imageHandle);
					this._mountedPath = null;
					this._mounted = false;
				}
			}

			// Token: 0x06000021 RID: 33 RVA: 0x00002600 File Offset: 0x00000800
			public void DismountImage(bool saveChanges)
			{
				if (this._mounted)
				{
					if (saveChanges)
					{
						WindowsImageContainer.NativeMethods.WIMCommitImageHandle(this._imageHandle, 0U);
					}
					this.DismountImage();
					this._mountedPath = null;
					this._mounted = false;
				}
			}

			// Token: 0x04000019 RID: 25
			private IntPtr _parentWindowsImageHandle = IntPtr.Zero;

			// Token: 0x0400001A RID: 26
			private string _parentWindowsImageFilePath;

			// Token: 0x0400001B RID: 27
			private IntPtr _imageHandle = IntPtr.Zero;

			// Token: 0x0400001C RID: 28
			private int _index;

			// Token: 0x0400001D RID: 29
			private string _mountedPath;

			// Token: 0x0400001E RID: 30
			private bool _mounted;

			// Token: 0x0400001F RID: 31
			private const string UNICODE_FILE_MARKER = "﻿";
		}

		// Token: 0x0200000A RID: 10
		private class NativeMethods
		{
			// Token: 0x06000022 RID: 34 RVA: 0x0000262E File Offset: 0x0000082E
			private NativeMethods()
			{
			}

			// Token: 0x06000023 RID: 35
			[DllImport("Wimgapi.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WIMCreateFile", ExactSpelling = true, SetLastError = true)]
			private static extern IntPtr WimCreateFile([MarshalAs(UnmanagedType.LPWStr)] string WimPath, uint DesiredAccess, uint CreationDisposition, uint FlagsAndAttributes, uint CompressionType, out IntPtr CreationResult);

			// Token: 0x06000024 RID: 36 RVA: 0x00002638 File Offset: 0x00000838
			public static IntPtr CreateFile(string imageFile, uint access, uint mode, uint compress)
			{
				IntPtr zero = IntPtr.Zero;
				IntPtr intPtr = IntPtr.Zero;
				uint num = 2U;
				if (compress == 3U)
				{
					num |= 536870912U;
				}
				intPtr = WindowsImageContainer.NativeMethods.WimCreateFile(imageFile, access, mode, num, compress, out zero);
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (intPtr == IntPtr.Zero)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unable to open/create .wim file {0}. Error = {1}", new object[]
					{
						imageFile,
						lastWin32Error
					}));
				}
				return intPtr;
			}

			// Token: 0x06000025 RID: 37
			[DllImport("Wimgapi.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WIMCloseHandle", ExactSpelling = true, SetLastError = true)]
			private static extern bool WimCloseHandle(IntPtr Handle);

			// Token: 0x06000026 RID: 38 RVA: 0x000026AC File Offset: 0x000008AC
			public static void CloseHandle(IntPtr handle)
			{
				bool flag = WindowsImageContainer.NativeMethods.WimCloseHandle(handle);
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (!flag)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unable to close image handle. Error = {0}", new object[]
					{
						lastWin32Error
					}));
				}
			}

			// Token: 0x06000027 RID: 39
			[DllImport("Wimgapi.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WIMSetTemporaryPath", ExactSpelling = true, SetLastError = true)]
			private static extern bool WimSetTemporaryPath(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string TemporaryPath);

			// Token: 0x06000028 RID: 40 RVA: 0x000026EC File Offset: 0x000008EC
			public static void SetTemporaryPath(IntPtr handle, string temporaryPath)
			{
				bool flag = WindowsImageContainer.NativeMethods.WimSetTemporaryPath(handle, temporaryPath);
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (!flag)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unable to set temporary path. Error = {0}", new object[]
					{
						lastWin32Error
					}));
				}
			}

			// Token: 0x06000029 RID: 41
			[DllImport("Wimgapi.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WIMLoadImage", ExactSpelling = true, SetLastError = true)]
			private static extern IntPtr WimLoadImage(IntPtr Handle, uint ImageIndex);

			// Token: 0x0600002A RID: 42 RVA: 0x0000272C File Offset: 0x0000092C
			public static IntPtr LoadImage(IntPtr handle, int imageIndex)
			{
				IntPtr intPtr = WindowsImageContainer.NativeMethods.WimLoadImage(handle, (uint)imageIndex);
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (intPtr == IntPtr.Zero)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unable to load image. Error = {0}", new object[]
					{
						lastWin32Error
					}));
				}
				return intPtr;
			}

			// Token: 0x0600002B RID: 43
			[DllImport("Wimgapi.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WIMCaptureImage", ExactSpelling = true, SetLastError = true)]
			private static extern IntPtr WimCaptureImage(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string Path, uint CaptureFlags);

			// Token: 0x0600002C RID: 44 RVA: 0x0000277C File Offset: 0x0000097C
			public static IntPtr CaptureImage(IntPtr handle, string path)
			{
				IntPtr intPtr = WindowsImageContainer.NativeMethods.WimCaptureImage(handle, path, 0U);
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (intPtr == IntPtr.Zero)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Failed to capture image from {0}. Error = {1}", new object[]
					{
						path,
						lastWin32Error
					}));
				}
				return intPtr;
			}

			// Token: 0x0600002D RID: 45
			[DllImport("Wimgapi.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WIMSetBootImage", ExactSpelling = true, SetLastError = true)]
			private static extern IntPtr WimSetBootImage(IntPtr Handle, uint index);

			// Token: 0x0600002E RID: 46 RVA: 0x000027D0 File Offset: 0x000009D0
			public static IntPtr SetBootImage(IntPtr handle, int index)
			{
				if (index != 0 && (index > WindowsImageContainer.NativeMethods.GetImageCount(handle) || index < 0))
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Index is out of range.  Current image count is {0}.", new object[]
					{
						WindowsImageContainer.NativeMethods.GetImageCount(handle)
					}));
				}
				IntPtr intPtr = WindowsImageContainer.NativeMethods.WimSetBootImage(handle, (uint)index);
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (intPtr == IntPtr.Zero)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Failed to set boot image with index {0}. Error = {1}", new object[]
					{
						index,
						lastWin32Error
					}));
				}
				return intPtr;
			}

			// Token: 0x0600002F RID: 47
			[DllImport("Wimgapi.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WIMGetImageCount", ExactSpelling = true, SetLastError = true)]
			private static extern int WimGetImageCount(IntPtr Handle);

			// Token: 0x06000030 RID: 48 RVA: 0x00002860 File Offset: 0x00000A60
			public static int GetImageCount(IntPtr windowsImageHandle)
			{
				int num = WindowsImageContainer.NativeMethods.WimGetImageCount(windowsImageHandle);
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (num == -1)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unable to get image count. Error = {0}", new object[]
					{
						lastWin32Error
					}));
				}
				return num;
			}

			// Token: 0x06000031 RID: 49
			[DllImport("Wimgapi.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WIMMountImage", ExactSpelling = true, SetLastError = true)]
			private static extern bool WimMountImage([MarshalAs(UnmanagedType.LPWStr)] string MountPath, [MarshalAs(UnmanagedType.LPWStr)] string WimFileName, uint ImageIndex, [MarshalAs(UnmanagedType.LPWStr)] string TemporaryPath);

			// Token: 0x06000032 RID: 50 RVA: 0x000028A4 File Offset: 0x00000AA4
			public static void MountImage(string mountPath, string windowsImageFileName, int imageIndex)
			{
				bool flag = false;
				int lastWin32Error;
				try
				{
					flag = WindowsImageContainer.NativeMethods.WimMountImage(mountPath, windowsImageFileName, (uint)imageIndex, Environment.GetEnvironmentVariable("temp"));
					lastWin32Error = Marshal.GetLastWin32Error();
				}
				catch (StackOverflowException)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unable to mount image {0} to {1}.", new object[]
					{
						windowsImageFileName,
						mountPath
					}));
				}
				if (!flag)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unable to mount image {0} to {1}. Error = {2}", new object[]
					{
						windowsImageFileName,
						mountPath,
						lastWin32Error
					}));
				}
			}

			// Token: 0x06000033 RID: 51
			[DllImport("Wimgapi.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WIMApplyImage", ExactSpelling = true, SetLastError = true)]
			private static extern bool WimApplyImage(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string MountPath, uint ApplyFlags);

			// Token: 0x06000034 RID: 52 RVA: 0x00002934 File Offset: 0x00000B34
			public static void ApplyImage(IntPtr handle, string applyPath, uint applyFlags)
			{
				bool flag = false;
				int lastWin32Error;
				try
				{
					flag = WindowsImageContainer.NativeMethods.WimApplyImage(handle, applyPath, applyFlags);
					lastWin32Error = Marshal.GetLastWin32Error();
				}
				catch (StackOverflowException)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unable to mount image from handle to {0}.", new object[]
					{
						applyPath
					}));
				}
				if (!flag)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unable to mount image from handle to {0}. Error = {1:X}", new object[]
					{
						applyPath,
						lastWin32Error
					}));
				}
			}

			// Token: 0x06000035 RID: 53
			[DllImport("Wimgapi.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WIMMountImageHandle", ExactSpelling = true, SetLastError = true)]
			private static extern bool WimMountImageHandle(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string MountPath, uint MountFlags);

			// Token: 0x06000036 RID: 54 RVA: 0x000029B0 File Offset: 0x00000BB0
			public static void MountImage(IntPtr handle, string mountPath, uint mountFlags)
			{
				bool flag = false;
				int lastWin32Error;
				try
				{
					flag = WindowsImageContainer.NativeMethods.WimMountImageHandle(handle, mountPath, mountFlags);
					lastWin32Error = Marshal.GetLastWin32Error();
				}
				catch (StackOverflowException)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unable to mount image from handle to {0}.", new object[]
					{
						mountPath
					}));
				}
				if (!flag)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unable to mount image from handle to {0}. Error = {1:X}", new object[]
					{
						mountPath,
						lastWin32Error
					}));
				}
			}

			// Token: 0x06000037 RID: 55
			[DllImport("Wimgapi.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WIMUnmountImage", ExactSpelling = true, SetLastError = true)]
			private static extern bool WimUnmountImage([MarshalAs(UnmanagedType.LPWStr)] string MountPath, [MarshalAs(UnmanagedType.LPWStr)] string WimFileName, uint ImageIndex, bool CommitChanges);

			// Token: 0x06000038 RID: 56 RVA: 0x00002A2C File Offset: 0x00000C2C
			public static void DismountImage(string mountPath, string wimdowsImageFileName, int imageIndex, bool commitChanges)
			{
				bool flag = false;
				int lastWin32Error;
				try
				{
					flag = WindowsImageContainer.NativeMethods.WimUnmountImage(mountPath, wimdowsImageFileName, (uint)imageIndex, commitChanges);
					lastWin32Error = Marshal.GetLastWin32Error();
				}
				catch (StackOverflowException ex)
				{
					throw new StackOverflowException(string.Format(CultureInfo.CurrentCulture, "Unable to unmount image {0} from {1}.", new object[]
					{
						wimdowsImageFileName,
						mountPath
					}), ex.InnerException);
				}
				if (!flag)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unable to unmount image {0} from {1}. Error = {2}", new object[]
					{
						wimdowsImageFileName,
						mountPath,
						lastWin32Error
					}));
				}
			}

			// Token: 0x06000039 RID: 57
			[DllImport("Wimgapi.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WIMUnmountImageHandle", ExactSpelling = true, SetLastError = true)]
			private static extern bool WimUnmountImage(IntPtr ImageHandle, uint UnmountFlags);

			// Token: 0x0600003A RID: 58 RVA: 0x00002AB8 File Offset: 0x00000CB8
			public static void DismountImage(IntPtr imageHandle)
			{
				bool flag = false;
				int lastWin32Error;
				try
				{
					flag = WindowsImageContainer.NativeMethods.WimUnmountImage(imageHandle, 0U);
					lastWin32Error = Marshal.GetLastWin32Error();
				}
				catch (StackOverflowException ex)
				{
					throw new StackOverflowException(string.Format(CultureInfo.CurrentCulture, "Unable to unmount image from handle {0}.", new object[]
					{
						imageHandle
					}), ex.InnerException);
				}
				if (!flag)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unable to unmount image from handle {0}. Error = {1}", new object[]
					{
						imageHandle,
						lastWin32Error
					}));
				}
			}

			// Token: 0x0600003B RID: 59
			[DllImport("Wimgapi.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
			private static extern bool WIMCommitImageHandle(IntPtr ImageHandle, uint UnmountFlags, IntPtr phNewImageHandle);

			// Token: 0x0600003C RID: 60 RVA: 0x00002B44 File Offset: 0x00000D44
			public static bool WIMCommitImageHandle(IntPtr ImageHandle, uint UnmountFlags)
			{
				return WindowsImageContainer.NativeMethods.WIMCommitImageHandle(ImageHandle, UnmountFlags, IntPtr.Zero);
			}

			// Token: 0x04000020 RID: 32
			public const uint WIM_FLAG_VERIFY = 2U;

			// Token: 0x04000021 RID: 33
			public const uint WIM_FLAG_INDEX = 4U;
		}

		// Token: 0x0200000B RID: 11
		[Flags]
		private enum CreateFileAccessPrivate : uint
		{
			// Token: 0x04000023 RID: 35
			Read = 2147483648U,
			// Token: 0x04000024 RID: 36
			Write = 1073741824U,
			// Token: 0x04000025 RID: 37
			Mount = 536870912U
		}
	}
}
