using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000045 RID: 69
	internal sealed class NativeImaging
	{
		// Token: 0x060002A7 RID: 679
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "CreateImageStorageService")]
		private static extern int CreateImageStorageServiceNative(out IntPtr serviceHandle, [MarshalAs(UnmanagedType.FunctionPtr)] LogFunction logError);

		// Token: 0x060002A8 RID: 680 RVA: 0x0000C690 File Offset: 0x0000A890
		public static IntPtr CreateImageStorageService(LogFunction logError)
		{
			IntPtr zero = IntPtr.Zero;
			int num = NativeImaging.CreateImageStorageServiceNative(out zero, logError);
			if (Win32Exports.FAILED(num) || zero == IntPtr.Zero)
			{
				throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name, num));
			}
			return zero;
		}

		// Token: 0x060002A9 RID: 681
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern void CloseImageStorageService(IntPtr service);

		// Token: 0x060002AA RID: 682
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern void SetLoggingFunction(IntPtr service, NativeImaging.LogLevel level, [MarshalAs(UnmanagedType.FunctionPtr)] LogFunction logFunction);

		// Token: 0x060002AB RID: 683
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "GetETWLogPath")]
		private static extern int GetETWLogPath_Native(IntPtr serviceHandle, StringBuilder logPath, uint pathLength);

		// Token: 0x060002AC RID: 684 RVA: 0x0000C6E4 File Offset: 0x0000A8E4
		public static string GetETWLogPath(IntPtr serviceHandle)
		{
			StringBuilder stringBuilder = new StringBuilder("etwLogPath", 1024);
			StringBuilder stringBuilder2 = stringBuilder;
			int etwlogPath_Native = NativeImaging.GetETWLogPath_Native(serviceHandle, stringBuilder2, (uint)stringBuilder2.Capacity);
			if (Win32Exports.FAILED(etwlogPath_Native))
			{
				throw new ImageStorageException(string.Format("{0} failed: {1:x}", MethodBase.GetCurrentMethod().Name, etwlogPath_Native));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060002AD RID: 685
		[DllImport("ImageStorageService.dll", EntryPoint = "UpdateDiskLayout")]
		private static extern int UpdateDiskLayout_Native(IntPtr service, SafeFileHandle diskHandle);

		// Token: 0x060002AE RID: 686 RVA: 0x0000C740 File Offset: 0x0000A940
		public static void UpdateDiskLayout(IntPtr service, SafeFileHandle diskHandle)
		{
			int num = NativeImaging.UpdateDiskLayout_Native(service, diskHandle);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}", MethodBase.GetCurrentMethod().Name, num));
			}
		}

		// Token: 0x060002AF RID: 687
		[DllImport("ImageStorageService.dll", CharSet = CharSet.Unicode, EntryPoint = "InitializeVirtualHardDisk")]
		private static extern int InitializeVirtualHardDisk_Native(IntPtr service, string fileName, [MarshalAs(UnmanagedType.Bool)] bool preparePartitions, ulong maxSizeInBytes, ref ImageStructures.STORE_ID storeId, uint partitionCount, uint sectorSize, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] ImageStructures.PARTITION_ENTRY[] partitions, bool fAssignMountPoints, uint storeIdsCount, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 10)] ImageStructures.STORE_ID[] storeIds, out IntPtr storeHandle);

		// Token: 0x060002B0 RID: 688 RVA: 0x0000C780 File Offset: 0x0000A980
		public static IntPtr InitializeVirtualHardDisk(IntPtr service, string fileName, ulong maxSizeInBytes, ref ImageStructures.STORE_ID storeId, ImageStructures.PARTITION_ENTRY[] partitions, bool preparePartitions, bool fAssignMountPoints, uint sectorSize, ImageStructures.STORE_ID[] storeIds)
		{
			IntPtr zero = IntPtr.Zero;
			int num = NativeImaging.InitializeVirtualHardDisk_Native(service, fileName, preparePartitions, maxSizeInBytes, ref storeId, (uint)partitions.Length, sectorSize, partitions, fAssignMountPoints, (uint)storeIds.Length, storeIds, out zero);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}", MethodBase.GetCurrentMethod().Name, num));
			}
			return zero;
		}

		// Token: 0x060002B1 RID: 689
		[DllImport("ImageStorageService.dll", CharSet = CharSet.Unicode, EntryPoint = "_NormalizeVolumeMountPoints@28")]
		private static extern int NormalizeVolumeMountPoints_Native(IntPtr service, ImageStructures.STORE_ID storeId, string mountPath);

		// Token: 0x060002B2 RID: 690 RVA: 0x0000C7DC File Offset: 0x0000A9DC
		public static void NormalizeVolumeMountPoints(IntPtr service, ImageStructures.STORE_ID storeId, string mountPath)
		{
			IntPtr zero = IntPtr.Zero;
			int num = NativeImaging.NormalizeVolumeMountPoints_Native(service, storeId, mountPath);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}", MethodBase.GetCurrentMethod().Name, num));
			}
		}

		// Token: 0x060002B3 RID: 691
		[DllImport("ImageStorageService.dll", CharSet = CharSet.Unicode, EntryPoint = "WriteMountManagerRegistry2")]
		private static extern int WriteMountManagerRegistry2_Native(IntPtr service, ImageStructures.STORE_ID storeId, bool useWellKnownGuids);

		// Token: 0x060002B4 RID: 692 RVA: 0x0000C824 File Offset: 0x0000AA24
		public static void WriteMountManagerRegistry2(IntPtr service, ImageStructures.STORE_ID storeId, bool useWellKnownGuids)
		{
			IntPtr zero = IntPtr.Zero;
			int num = NativeImaging.WriteMountManagerRegistry2_Native(service, storeId, useWellKnownGuids);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}", MethodBase.GetCurrentMethod().Name, num));
			}
		}

		// Token: 0x060002B5 RID: 693
		[DllImport("ImageStorageService.dll", CharSet = CharSet.Unicode, EntryPoint = "CreateEmptyVirtualDisk")]
		private static extern int CreateEmptyVirtualDisk_Native(IntPtr service, string fileName, ref ImageStructures.STORE_ID storeId, ulong maxSizeInBytes, uint sectorSize, out IntPtr storeHandle);

		// Token: 0x060002B6 RID: 694 RVA: 0x0000C86C File Offset: 0x0000AA6C
		public static IntPtr CreateEmptyVirtualDisk(IntPtr service, string fileName, ref ImageStructures.STORE_ID storeId, ulong maxSizeInBytes, uint sectorSize)
		{
			IntPtr zero = IntPtr.Zero;
			int num = NativeImaging.CreateEmptyVirtualDisk_Native(service, fileName, ref storeId, maxSizeInBytes, sectorSize, out zero);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}", MethodBase.GetCurrentMethod().Name, num));
			}
			return zero;
		}

		// Token: 0x060002B7 RID: 695
		[DllImport("ImageStorageService.dll", CharSet = CharSet.Unicode, EntryPoint = "OpenVirtualHardDisk")]
		private static extern int OpenVirtualHardDisk_Native(IntPtr service, string fileName, [MarshalAs(UnmanagedType.Bool)] bool readOnly, out ImageStructures.STORE_ID storeId, out IntPtr storeHandle);

		// Token: 0x060002B8 RID: 696 RVA: 0x0000C8B8 File Offset: 0x0000AAB8
		public static IntPtr OpenVirtualHardDisk(IntPtr service, string fileName, out ImageStructures.STORE_ID storeId, bool readOnly)
		{
			IntPtr zero = IntPtr.Zero;
			int num = NativeImaging.OpenVirtualHardDisk_Native(service, fileName, readOnly, out storeId, out zero);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0}({1}) failed with error code: {2:x}", MethodBase.GetCurrentMethod().Name, fileName, num));
			}
			return zero;
		}

		// Token: 0x060002B9 RID: 697
		[DllImport("ImageStorageService.dll", CharSet = CharSet.Unicode, EntryPoint = "AttachToMountedImage")]
		private static extern int AttachToMountedImage_Native(IntPtr service, string mountedPath, [MarshalAs(UnmanagedType.Bool)] bool openWithWriteAccess, StringBuilder imagePath, uint imagePathCharacterCount, out ImageStructures.STORE_ID storeId, out IntPtr storeHandle);

		// Token: 0x060002BA RID: 698 RVA: 0x0000C904 File Offset: 0x0000AB04
		public static void AttachToMountedImage(IntPtr service, string mountedDrivePath, bool readOnly, out string imagePath, out ImageStructures.STORE_ID storeId, out IntPtr storeHandle)
		{
			StringBuilder stringBuilder = new StringBuilder("imagePath", 32768);
			bool openWithWriteAccess = !readOnly;
			StringBuilder stringBuilder2 = stringBuilder;
			int num = NativeImaging.AttachToMountedImage_Native(service, mountedDrivePath, openWithWriteAccess, stringBuilder2, (uint)stringBuilder2.Capacity, out storeId, out storeHandle);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0}({1}) failed with error code: {2:x}", MethodBase.GetCurrentMethod().Name, mountedDrivePath, num));
			}
			imagePath = stringBuilder.ToString();
		}

		// Token: 0x060002BB RID: 699
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "GetPartitionPath")]
		private static extern int GetPartitionPath_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId, string partitionName, StringBuilder path, uint pathSizeInCharacters);

		// Token: 0x060002BC RID: 700 RVA: 0x0000C96C File Offset: 0x0000AB6C
		public static void GetPartitionPath(IntPtr serviceHandle, ImageStructures.STORE_ID storeId, string partitionName, StringBuilder path, uint pathSizeInCharacters)
		{
			int partitionPath_Native = NativeImaging.GetPartitionPath_Native(serviceHandle, storeId, partitionName, path, pathSizeInCharacters);
			if (Win32Exports.FAILED(partitionPath_Native))
			{
				throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}", MethodBase.GetCurrentMethod().Name, partitionName, partitionPath_Native));
			}
		}

		// Token: 0x060002BD RID: 701
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "GetPartitionPathNoContext")]
		private static extern int GetPartitionPathNoContext_Native(string partitionName, StringBuilder path, uint pathSizeInCharacters);

		// Token: 0x060002BE RID: 702 RVA: 0x0000C9B0 File Offset: 0x0000ABB0
		public static void GetPartitionPathNoContext(string partitionName, StringBuilder path, uint pathSizeInCharacters)
		{
			int partitionPathNoContext_Native = NativeImaging.GetPartitionPathNoContext_Native(partitionName, path, pathSizeInCharacters);
			if (Win32Exports.FAILED(partitionPathNoContext_Native))
			{
				throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}", MethodBase.GetCurrentMethod().Name, partitionName, partitionPathNoContext_Native));
			}
		}

		// Token: 0x060002BF RID: 703
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "GetPartitionFileSystem")]
		private static extern int GetPartitionFileSystem_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId, string partitionName, StringBuilder fileSystem, uint fileSystemSizeInCharacters);

		// Token: 0x060002C0 RID: 704 RVA: 0x0000C9F0 File Offset: 0x0000ABF0
		public static string GetPartitionFileSystem(IntPtr serviceHandle, ImageStructures.STORE_ID storeId, string partitionName)
		{
			StringBuilder stringBuilder = new StringBuilder("fileSystem", 260);
			StringBuilder stringBuilder2 = stringBuilder;
			int partitionFileSystem_Native = NativeImaging.GetPartitionFileSystem_Native(serviceHandle, storeId, partitionName, stringBuilder2, (uint)stringBuilder2.Capacity);
			if (Win32Exports.FAILED(partitionFileSystem_Native))
			{
				throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}", MethodBase.GetCurrentMethod().Name, partitionName, partitionFileSystem_Native));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060002C1 RID: 705
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "GetDiskName")]
		private static extern int GetDiskName_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId, StringBuilder fileSystem, uint fileSystemSizeInCharacters);

		// Token: 0x060002C2 RID: 706 RVA: 0x0000CA4C File Offset: 0x0000AC4C
		public static string GetDiskName(IntPtr serviceHandle, ImageStructures.STORE_ID storeId)
		{
			StringBuilder stringBuilder = new StringBuilder("diskName", 32768);
			StringBuilder stringBuilder2 = stringBuilder;
			int diskName_Native = NativeImaging.GetDiskName_Native(serviceHandle, storeId, stringBuilder2, (uint)stringBuilder2.Capacity);
			if (Win32Exports.FAILED(diskName_Native))
			{
				throw new ImageStorageException(string.Format("{0} failed: {1:x}", MethodBase.GetCurrentMethod().Name, diskName_Native));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060002C3 RID: 707
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "GetVirtualHardDiskFileName")]
		private static extern int GetVhdFileName_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId, StringBuilder imagePath, uint imagePathSizeInCharacters);

		// Token: 0x060002C4 RID: 708 RVA: 0x0000CAA8 File Offset: 0x0000ACA8
		public static string GetVhdFileName(IntPtr serviceHandle, ImageStructures.STORE_ID storeId)
		{
			StringBuilder stringBuilder = new StringBuilder("imagePath", 32768);
			StringBuilder stringBuilder2 = stringBuilder;
			int vhdFileName_Native = NativeImaging.GetVhdFileName_Native(serviceHandle, storeId, stringBuilder2, (uint)stringBuilder2.Capacity);
			if (Win32Exports.FAILED(vhdFileName_Native))
			{
				throw new ImageStorageException(string.Format("{0} failed: {1:x}", MethodBase.GetCurrentMethod().Name, vhdFileName_Native));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060002C5 RID: 709
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetDiskId")]
		private static extern int GetDiskId_Native(IntPtr serviceHandle, SafeFileHandle diskHandle, out ImageStructures.STORE_ID storeId);

		// Token: 0x060002C6 RID: 710 RVA: 0x0000CB04 File Offset: 0x0000AD04
		public static ImageStructures.STORE_ID GetDiskId(IntPtr serviceHandle, SafeFileHandle diskHandle)
		{
			ImageStructures.STORE_ID result = default(ImageStructures.STORE_ID);
			int diskId_Native = NativeImaging.GetDiskId_Native(serviceHandle, diskHandle, out result);
			if (Win32Exports.FAILED(diskId_Native))
			{
				throw new ImageStorageException(string.Format("{0} failed: {1:x}", MethodBase.GetCurrentMethod().Name, diskId_Native));
			}
			return result;
		}

		// Token: 0x060002C7 RID: 711
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "GetPartitionType")]
		private static extern int GetPartitionType_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId, string partitionName, out ImageStructures.PartitionType partitionType);

		// Token: 0x060002C8 RID: 712 RVA: 0x0000CB4C File Offset: 0x0000AD4C
		public static ImageStructures.PartitionType GetPartitionType(IntPtr serviceHandle, ImageStructures.STORE_ID storeId, string partitionName)
		{
			ImageStructures.PartitionType result = default(ImageStructures.PartitionType);
			int partitionType_Native = NativeImaging.GetPartitionType_Native(serviceHandle, storeId, partitionName, out result);
			if (Win32Exports.FAILED(partitionType_Native))
			{
				throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}", MethodBase.GetCurrentMethod().Name, partitionName, partitionType_Native));
			}
			return result;
		}

		// Token: 0x060002C9 RID: 713
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "SetPartitionType")]
		private static extern int SetPartitionType_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId, string partitionName, ImageStructures.PartitionType partitionType);

		// Token: 0x060002CA RID: 714 RVA: 0x0000CB98 File Offset: 0x0000AD98
		public static void SetPartitionType(IntPtr serviceHandle, ImageStructures.STORE_ID storeId, string partitionName, ImageStructures.PartitionType partitionType)
		{
			int num = NativeImaging.SetPartitionType_Native(serviceHandle, storeId, partitionName, partitionType);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}", MethodBase.GetCurrentMethod().Name, partitionName, num));
			}
		}

		// Token: 0x060002CB RID: 715
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "SetPartitionAttributes")]
		private static extern int SetPartitionAttributes_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId, string partitionName, ImageStructures.PartitionAttributes attributes);

		// Token: 0x060002CC RID: 716 RVA: 0x0000CBD8 File Offset: 0x0000ADD8
		public static void SetPartitionAttributes(IntPtr serviceHandle, ImageStructures.STORE_ID storeId, string partitionName, ImageStructures.PartitionAttributes attributes)
		{
			int num = NativeImaging.SetPartitionAttributes_Native(serviceHandle, storeId, partitionName, attributes);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}", MethodBase.GetCurrentMethod().Name, partitionName, num));
			}
		}

		// Token: 0x060002CD RID: 717
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "GetPartitionAttributes")]
		private static extern int GetPartitionAttributes_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId, string partitionName, out ImageStructures.PartitionAttributes attributes);

		// Token: 0x060002CE RID: 718 RVA: 0x0000CC18 File Offset: 0x0000AE18
		public static ImageStructures.PartitionAttributes GetPartitionAttributes(IntPtr serviceHandle, ImageStructures.STORE_ID storeId, string partitionName)
		{
			ImageStructures.PartitionAttributes result = default(ImageStructures.PartitionAttributes);
			int partitionAttributes_Native = NativeImaging.GetPartitionAttributes_Native(serviceHandle, storeId, partitionName, out result);
			if (Win32Exports.FAILED(partitionAttributes_Native))
			{
				throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}", MethodBase.GetCurrentMethod().Name, partitionName, partitionAttributes_Native));
			}
			return result;
		}

		// Token: 0x060002CF RID: 719
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "SetDiskAttributes")]
		private static extern int SetDiskAttributes_Native(IntPtr serviceHandle, IntPtr diskHandle, out ImageStructures.SetDiskAttributes attributes);

		// Token: 0x060002D0 RID: 720 RVA: 0x0000CC64 File Offset: 0x0000AE64
		public static void SetDiskAttributes(IntPtr serviceHandle, IntPtr diskHandle, ImageStructures.SetDiskAttributes attributes)
		{
			ImageStructures.SetDiskAttributes setDiskAttributes = attributes;
			int num = NativeImaging.SetDiskAttributes_Native(serviceHandle, diskHandle, out setDiskAttributes);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0} failed: {1:x}", MethodBase.GetCurrentMethod().Name, num));
			}
		}

		// Token: 0x060002D1 RID: 721
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "DismountVirtualHardDisk")]
		private static extern int DismountVirtualHardDisk_Native(IntPtr service, ImageStructures.STORE_ID storeId, [MarshalAs(UnmanagedType.Bool)] bool removeAccessPaths, [MarshalAs(UnmanagedType.Bool)] bool deleteFile, [MarshalAs(UnmanagedType.Bool)] bool fFailIfDiskMissing);

		// Token: 0x060002D2 RID: 722 RVA: 0x0000CCA8 File Offset: 0x0000AEA8
		public static void DismountVirtualHardDisk(IntPtr service, ImageStructures.STORE_ID storeId, bool removeAccessPaths, bool deleteFile, bool failIfDiskMissing = false)
		{
			int num = NativeImaging.DismountVirtualHardDisk_Native(service, storeId, removeAccessPaths, deleteFile, failIfDiskMissing);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name, num));
			}
		}

		// Token: 0x060002D3 RID: 723
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "DismountVirtualHardDiskByFileName")]
		private static extern int DismountVirtualHardDiskByFileName_Native(IntPtr service, string fileName, [MarshalAs(UnmanagedType.Bool)] bool deleteFile);

		// Token: 0x060002D4 RID: 724 RVA: 0x0000CCEC File Offset: 0x0000AEEC
		public static void DismountVirtualHardDiskByName(IntPtr service, string fileName, bool deleteFile)
		{
			int num = NativeImaging.DismountVirtualHardDiskByFileName_Native(service, fileName, deleteFile);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name, num));
			}
		}

		// Token: 0x060002D5 RID: 725
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetSectorSize")]
		private static extern int GetSectorSize_Native(IntPtr service, ImageStructures.STORE_ID storeId, out uint bytesPerSector);

		// Token: 0x060002D6 RID: 726 RVA: 0x0000CD2C File Offset: 0x0000AF2C
		public static uint GetSectorSize(IntPtr service, ImageStructures.STORE_ID storeId)
		{
			uint result = 0U;
			int sectorSize_Native = NativeImaging.GetSectorSize_Native(service, storeId, out result);
			if (Win32Exports.FAILED(sectorSize_Native))
			{
				throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name, sectorSize_Native));
			}
			return result;
		}

		// Token: 0x060002D7 RID: 727
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "GetPartitionOffset")]
		private static extern int GetPartitionOffset_Native(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName, out ulong startingSector);

		// Token: 0x060002D8 RID: 728 RVA: 0x0000CD70 File Offset: 0x0000AF70
		public static ulong GetPartitionOffset(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName)
		{
			ulong result = 0UL;
			int partitionOffset_Native = NativeImaging.GetPartitionOffset_Native(service, storeId, partitionName, out result);
			if (Win32Exports.FAILED(partitionOffset_Native))
			{
				throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name, partitionOffset_Native));
			}
			return result;
		}

		// Token: 0x060002D9 RID: 729
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "GetPartitionSize")]
		private static extern int GetPartitionSize_Native(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName, out ulong sectorCount);

		// Token: 0x060002DA RID: 730 RVA: 0x0000CDB4 File Offset: 0x0000AFB4
		public static ulong GetPartitionSize(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName)
		{
			ulong result = 0UL;
			int partitionSize_Native = NativeImaging.GetPartitionSize_Native(service, storeId, partitionName, out result);
			if (Win32Exports.FAILED(partitionSize_Native))
			{
				throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name, partitionSize_Native));
			}
			return result;
		}

		// Token: 0x060002DB RID: 731
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "GetFreeBytesOnVolume")]
		private static extern int GetFreeBytesOnVolume_Native(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName, out ulong freeBytes);

		// Token: 0x060002DC RID: 732 RVA: 0x0000CDF8 File Offset: 0x0000AFF8
		public static ulong GetFreeBytesOnVolume(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName)
		{
			ulong result = 0UL;
			int freeBytesOnVolume_Native = NativeImaging.GetFreeBytesOnVolume_Native(service, storeId, partitionName, out result);
			if (Win32Exports.FAILED(freeBytesOnVolume_Native))
			{
				throw new ImageStorageException(string.Format("{0} failed for partition {1}: {2:x}.", MethodBase.GetCurrentMethod().Name, partitionName, freeBytesOnVolume_Native));
			}
			return result;
		}

		// Token: 0x060002DD RID: 733
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "OpenVolume")]
		private static extern int OpenVolumeHandle_Native(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName, uint requestedAccess, uint shareMode, out IntPtr volumeHandle);

		// Token: 0x060002DE RID: 734 RVA: 0x0000CE40 File Offset: 0x0000B040
		public static SafeFileHandle OpenVolumeHandle(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName, FileAccess access, FileShare share)
		{
			IntPtr zero = IntPtr.Zero;
			uint num = 0U;
			uint num2 = 0U;
			if ((access & FileAccess.Read) != (FileAccess)0)
			{
				num2 |= 2147483648U;
			}
			if ((access & FileAccess.Write) != (FileAccess)0)
			{
				num2 |= 1073741824U;
			}
			if ((access & FileAccess.ReadWrite) != (FileAccess)0)
			{
				num2 |= 2147483648U;
				num2 |= 1073741824U;
			}
			if ((share & FileShare.Read) != FileShare.None)
			{
				num |= 1U;
			}
			if ((share & FileShare.Write) != FileShare.None)
			{
				num |= 2U;
			}
			if ((share & FileShare.ReadWrite) != FileShare.None)
			{
				num |= 1U;
				num |= 2U;
			}
			if ((share & FileShare.Delete) != FileShare.None)
			{
				num |= 4U;
			}
			int num3 = NativeImaging.OpenVolumeHandle_Native(service, storeId, partitionName, num2, num, out zero);
			if (Win32Exports.FAILED(num3))
			{
				throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name, num3));
			}
			SafeFileHandle safeFileHandle = new SafeFileHandle(zero, true);
			if (safeFileHandle.IsInvalid)
			{
				throw new ImageStorageException(string.Format("{0} returned an invalid handle.", MethodBase.GetCurrentMethod().Name));
			}
			return safeFileHandle;
		}

		// Token: 0x060002DF RID: 735
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "CloseVolumeHandle")]
		private static extern int CloseVolumeHandle_Native(IntPtr service, IntPtr volumeHandle);

		// Token: 0x060002E0 RID: 736 RVA: 0x0000CF10 File Offset: 0x0000B110
		public static void CloseVolumeHandle(IntPtr service, IntPtr volumeHandle)
		{
			int num = NativeImaging.CloseVolumeHandle_Native(service, volumeHandle);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name, num));
			}
		}

		// Token: 0x060002E1 RID: 737
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "LockAndDismountVolumeByHandle")]
		private static extern int LockAndDismountVolumeByHandle_Native(IntPtr service, SafeFileHandle volumeHandle, [MarshalAs(UnmanagedType.Bool)] bool forceDismount);

		// Token: 0x060002E2 RID: 738 RVA: 0x0000CF50 File Offset: 0x0000B150
		public static void LockAndDismountVolume(IntPtr service, SafeFileHandle volumeHandle, bool forceDismount)
		{
			int num = NativeImaging.LockAndDismountVolumeByHandle_Native(service, volumeHandle, forceDismount);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name, num));
			}
		}

		// Token: 0x060002E3 RID: 739
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UnlockVolumeByHandle")]
		private static extern int UnlockVolumeByHandle_Native(IntPtr service, IntPtr volumeHandle);

		// Token: 0x060002E4 RID: 740 RVA: 0x0000CF90 File Offset: 0x0000B190
		public static void UnlockVolume(IntPtr service, SafeHandle volumeHandle)
		{
			int num = NativeImaging.UnlockVolumeByHandle_Native(service, volumeHandle.DangerousGetHandle());
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name, num));
			}
		}

		// Token: 0x060002E5 RID: 741
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "FormatPartition")]
		private static extern int FormatPartition_Native(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName, string fileSystem, uint cbClusterSize);

		// Token: 0x060002E6 RID: 742 RVA: 0x0000CFD4 File Offset: 0x0000B1D4
		public static void FormatPartition(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName, string fileSystem, uint cbClusterSize)
		{
			int num = NativeImaging.FormatPartition_Native(service, storeId, partitionName, fileSystem, cbClusterSize);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}.", MethodBase.GetCurrentMethod().Name, partitionName, num));
			}
		}

		// Token: 0x060002E7 RID: 743
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "AttachWOFToVolume")]
		private static extern int AttachWOFToVolume_Native(IntPtr service, string volumePath);

		// Token: 0x060002E8 RID: 744 RVA: 0x0000D018 File Offset: 0x0000B218
		public static void AttachWOFToVolume(IntPtr service, string volumePath)
		{
			int num = NativeImaging.AttachWOFToVolume_Native(service, volumePath);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}.", MethodBase.GetCurrentMethod().Name, volumePath, num));
			}
		}

		// Token: 0x060002E9 RID: 745
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "AddAccessPath")]
		private static extern int AddAccessPath_Native(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName, string accessPath);

		// Token: 0x060002EA RID: 746 RVA: 0x0000D058 File Offset: 0x0000B258
		public static void AddAccessPath(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName, string accessPath)
		{
			int num = NativeImaging.AddAccessPath_Native(service, storeId, partitionName, accessPath);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}.", MethodBase.GetCurrentMethod().Name, partitionName, num));
			}
		}

		// Token: 0x060002EB RID: 747
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "WaitForVolumeArrival")]
		private static extern int WaitForVolumeArrival_Native(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName, int timeout);

		// Token: 0x060002EC RID: 748 RVA: 0x0000D098 File Offset: 0x0000B298
		public static void WaitForVolumeArrival(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName, int timeout)
		{
			int num = NativeImaging.WaitForVolumeArrival_Native(service, storeId, partitionName, timeout);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}.", MethodBase.GetCurrentMethod().Name, partitionName, num));
			}
		}

		// Token: 0x060002ED RID: 749
		[DllImport("ImageStorageService.dll", CharSet = CharSet.Unicode, EntryPoint = "ReadFromDisk")]
		private static extern int ReadFromDisk_Native(IntPtr service, ImageStructures.STORE_ID storeId, ulong diskOffset, uint byteCount, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] buffer);

		// Token: 0x060002EE RID: 750 RVA: 0x0000D0D8 File Offset: 0x0000B2D8
		public static void ReadFromDisk(IntPtr service, ImageStructures.STORE_ID storeId, ulong diskOffset, byte[] buffer)
		{
			int num = NativeImaging.ReadFromDisk_Native(service, storeId, diskOffset, (uint)buffer.Length, buffer);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}", MethodBase.GetCurrentMethod().Name, num));
			}
		}

		// Token: 0x060002EF RID: 751
		[DllImport("ImageStorageService.dll", CharSet = CharSet.Unicode, EntryPoint = "GetPartitionStyle")]
		private static extern int GetPartitionStyle_Native(IntPtr service, SafeFileHandle hStore, out uint partitionStyle);

		// Token: 0x060002F0 RID: 752 RVA: 0x0000D11C File Offset: 0x0000B31C
		public static uint GetPartitionStyle(IntPtr service, SafeFileHandle storeHandle)
		{
			uint result = 0U;
			int partitionStyle_Native = NativeImaging.GetPartitionStyle_Native(service, storeHandle, out result);
			if (Win32Exports.FAILED(partitionStyle_Native))
			{
				throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name, partitionStyle_Native));
			}
			return result;
		}

		// Token: 0x060002F1 RID: 753
		[DllImport("ImageStorageService.dll", CharSet = CharSet.Unicode, EntryPoint = "GetSectorCount")]
		private static extern int GetSectorCount_Native(IntPtr service, SafeFileHandle hStore, out ulong sectorCount);

		// Token: 0x060002F2 RID: 754 RVA: 0x0000D160 File Offset: 0x0000B360
		public static ulong GetSectorCount(IntPtr service, SafeFileHandle storeHandle)
		{
			ulong result = 0UL;
			int sectorCount_Native = NativeImaging.GetSectorCount_Native(service, storeHandle, out result);
			if (Win32Exports.FAILED(sectorCount_Native))
			{
				throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name, sectorCount_Native));
			}
			return result;
		}

		// Token: 0x060002F3 RID: 755
		[DllImport("ImageStorageService.dll", CharSet = CharSet.Unicode, EntryPoint = "GetSectorSizeFromHandle")]
		private static extern int GetSectorSizeFromHandle_Native(IntPtr service, SafeFileHandle hStore, out uint sectorCount);

		// Token: 0x060002F4 RID: 756 RVA: 0x0000D1A4 File Offset: 0x0000B3A4
		public static uint GetSectorSize(IntPtr service, SafeFileHandle storeHandle)
		{
			uint result = 0U;
			int sectorSizeFromHandle_Native = NativeImaging.GetSectorSizeFromHandle_Native(service, storeHandle, out result);
			if (Win32Exports.FAILED(sectorSizeFromHandle_Native))
			{
				throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name, sectorSizeFromHandle_Native));
			}
			return result;
		}

		// Token: 0x060002F5 RID: 757
		[DllImport("ImageStorageService.dll", CharSet = CharSet.Unicode, EntryPoint = "GetBlockAllocationBitmap")]
		private static extern int GetBlockAllocationBitmap_Native(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName, uint blockSize, byte[] blockBitmapBuffer, uint bitmapBufferSize);

		// Token: 0x060002F6 RID: 758 RVA: 0x0000D1E8 File Offset: 0x0000B3E8
		public static void GetBlockAllocationBitmap(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName, uint blockSize, byte[] blockBitmapBuffer)
		{
			int blockAllocationBitmap_Native = NativeImaging.GetBlockAllocationBitmap_Native(service, storeId, partitionName, blockSize, blockBitmapBuffer, (uint)blockBitmapBuffer.Length);
			if (Win32Exports.FAILED(blockAllocationBitmap_Native))
			{
				throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name, blockAllocationBitmap_Native));
			}
		}

		// Token: 0x060002F7 RID: 759
		[DllImport("ImageStorageService.dll", CharSet = CharSet.Unicode, EntryPoint = "WaitForPartitions")]
		private static extern int WaitForPartitions_Native(IntPtr service, ImageStructures.STORE_ID storeId, uint partitionCount, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] ImageStructures.PARTITION_ENTRY[] partitions);

		// Token: 0x060002F8 RID: 760 RVA: 0x0000D22C File Offset: 0x0000B42C
		public static IntPtr WaitForPartitions(IntPtr service, ImageStructures.STORE_ID storeId, ImageStructures.PARTITION_ENTRY[] partitions)
		{
			IntPtr zero = IntPtr.Zero;
			int num = NativeImaging.WaitForPartitions_Native(service, storeId, (uint)partitions.Length, partitions);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}", MethodBase.GetCurrentMethod().Name, num));
			}
			return zero;
		}

		// Token: 0x060002F9 RID: 761
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "CreateUsnJournal")]
		private static extern int CreateUsnJournal_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId, string partitionName);

		// Token: 0x060002FA RID: 762 RVA: 0x0000D274 File Offset: 0x0000B474
		public static void CreateUsnJournal(IntPtr serviceHandle, ImageStructures.STORE_ID storeId, string partitionName)
		{
			int num = NativeImaging.CreateUsnJournal_Native(serviceHandle, storeId, partitionName);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0}({1} failed with error code: {2:x}", MethodBase.GetCurrentMethod().Name, partitionName, num));
			}
		}

		// Token: 0x060002FB RID: 763
		[DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "CreateJunction")]
		private static extern int CreateJunction_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId, string sourceName, string targetPartition, string targetPath, [MarshalAs(UnmanagedType.U1)] bool useWellKnownGuids);

		// Token: 0x060002FC RID: 764 RVA: 0x0000D2B4 File Offset: 0x0000B4B4
		public static void CreateJunction(IntPtr serviceHandle, ImageStructures.STORE_ID storeId, string sourceName, string targetPartition, string targetName, bool useWellKnownGuids = false)
		{
			int num = NativeImaging.CreateJunction_Native(serviceHandle, storeId, sourceName, targetPartition, targetName, useWellKnownGuids);
			if (Win32Exports.FAILED(num))
			{
				throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}", MethodBase.GetCurrentMethod().Name, num));
			}
		}

		// Token: 0x0200007F RID: 127
		public enum LogLevel
		{
			// Token: 0x040002C3 RID: 707
			levelError,
			// Token: 0x040002C4 RID: 708
			levelWarning,
			// Token: 0x040002C5 RID: 709
			levelInfo,
			// Token: 0x040002C6 RID: 710
			levelDebug,
			// Token: 0x040002C7 RID: 711
			levelInvalid
		}
	}
}
