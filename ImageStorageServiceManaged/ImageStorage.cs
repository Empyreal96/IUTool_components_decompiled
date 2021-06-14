using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000048 RID: 72
	[CLSCompliant(false)]
	public class ImageStorage
	{
		// Token: 0x06000302 RID: 770 RVA: 0x0000D2F8 File Offset: 0x0000B4F8
		public ImageStorage(IULogger logger, ImageStorageManager manager)
		{
			this._logger = logger;
			this._manager = manager;
			this._logError = new LogFunction(this.LogError);
			this._service = new NativeServiceHandle(this._logError);
			this._storeId = default(ImageStructures.STORE_ID);
			this._pathsToRemove = new List<string>();
			this._isMainOSStorage = true;
			this.PrepareLogging();
		}

		// Token: 0x06000303 RID: 771 RVA: 0x0000D360 File Offset: 0x0000B560
		public ImageStorage(IULogger logger, ImageStorageManager manager, ImageStructures.STORE_ID storeId)
		{
			this._logger = logger;
			this._manager = manager;
			this._logError = new LogFunction(this.LogError);
			this._service = new NativeServiceHandle(this._logError);
			this._storeId = storeId;
			this._pathsToRemove = new List<string>();
			this._isMainOSStorage = true;
			this.PrepareLogging();
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000304 RID: 772 RVA: 0x0000D3C3 File Offset: 0x0000B5C3
		public IntPtr StoreHandle
		{
			get
			{
				return this._storeHandle.DangerousGetHandle();
			}
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000305 RID: 773 RVA: 0x0000D3D0 File Offset: 0x0000B5D0
		public SafeFileHandle SafeStoreHandle
		{
			get
			{
				return this._storeHandle;
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000306 RID: 774 RVA: 0x0000D3D8 File Offset: 0x0000B5D8
		public bool IsMainOSStorage
		{
			get
			{
				return this._isMainOSStorage;
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000307 RID: 775 RVA: 0x0000D3E0 File Offset: 0x0000B5E0
		// (set) Token: 0x06000308 RID: 776 RVA: 0x0000D3E8 File Offset: 0x0000B5E8
		public ImageStructures.STORE_ID StoreId
		{
			get
			{
				return this._storeId;
			}
			set
			{
				this._storeId = value;
			}
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000309 RID: 777 RVA: 0x0000D3F1 File Offset: 0x0000B5F1
		// (set) Token: 0x0600030A RID: 778 RVA: 0x0000D3F9 File Offset: 0x0000B5F9
		public uint VirtualHardDiskSectorSize { get; set; }

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x0600030B RID: 779 RVA: 0x0000D402 File Offset: 0x0000B602
		// (set) Token: 0x0600030C RID: 780 RVA: 0x0000D40A File Offset: 0x0000B60A
		public string VirtualDiskFilePath { get; private set; }

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x0600030D RID: 781 RVA: 0x0000D413 File Offset: 0x0000B613
		public IULogger Logger
		{
			get
			{
				return this._logger;
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x0600030E RID: 782 RVA: 0x0000D41B File Offset: 0x0000B61B
		internal FullFlashUpdateImage Image
		{
			get
			{
				return this._image;
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x0600030F RID: 783 RVA: 0x0000D423 File Offset: 0x0000B623
		internal FullFlashUpdateImage.FullFlashUpdateStore Store
		{
			get
			{
				return this._store;
			}
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000310 RID: 784 RVA: 0x0000D42B File Offset: 0x0000B62B
		internal NativeServiceHandle ServiceHandle
		{
			get
			{
				return this._service;
			}
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000311 RID: 785 RVA: 0x0000D433 File Offset: 0x0000B633
		internal uint BytesPerBlock
		{
			get
			{
				return ImageConstants.PAYLOAD_BLOCK_SIZE;
			}
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000312 RID: 786 RVA: 0x0000D43A File Offset: 0x0000B63A
		// (set) Token: 0x06000313 RID: 787 RVA: 0x0000D442 File Offset: 0x0000B642
		internal bool ReadOnlyVirtualDisk { get; private set; }

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000314 RID: 788 RVA: 0x0000D44B File Offset: 0x0000B64B
		// (set) Token: 0x06000315 RID: 789 RVA: 0x0000D453 File Offset: 0x0000B653
		private uint ImageSectorCount { get; set; }

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000316 RID: 790 RVA: 0x0000D45C File Offset: 0x0000B65C
		// (set) Token: 0x06000317 RID: 791 RVA: 0x0000D464 File Offset: 0x0000B664
		private bool PostProcessVHD { get; set; }

		// Token: 0x06000318 RID: 792 RVA: 0x0000D46D File Offset: 0x0000B66D
		public void Cleanup()
		{
			this.CleanupTemporaryPaths();
			this._image = null;
			this._store = null;
			this._storeHandle = null;
		}

		// Token: 0x06000319 RID: 793 RVA: 0x0000D48A File Offset: 0x0000B68A
		public void LogError(string message)
		{
			this.Logger.LogError("{0}", new object[]
			{
				message
			});
		}

		// Token: 0x0600031A RID: 794 RVA: 0x0000D4A6 File Offset: 0x0000B6A6
		public void LogWarning(string message)
		{
			this.Logger.LogWarning("{0}", new object[]
			{
				message
			});
		}

		// Token: 0x0600031B RID: 795 RVA: 0x0000D4C2 File Offset: 0x0000B6C2
		public void LogInfo(string message)
		{
			this.Logger.LogInfo("{0}", new object[]
			{
				message
			});
		}

		// Token: 0x0600031C RID: 796 RVA: 0x0000D4DE File Offset: 0x0000B6DE
		public void LogDebug(string message)
		{
			this.Logger.LogDebug("{0}", new object[]
			{
				message
			});
		}

		// Token: 0x0600031D RID: 797 RVA: 0x0000D4FA File Offset: 0x0000B6FA
		public void SetFullFlashImage(FullFlashUpdateImage image)
		{
			this._image = image;
		}

		// Token: 0x0600031E RID: 798 RVA: 0x0000D503 File Offset: 0x0000B703
		public void CreateJunction(string sourceName, string targetPartition, string targetPath)
		{
			this.CreateJunction(sourceName, targetPartition, targetPath, false);
		}

		// Token: 0x0600031F RID: 799 RVA: 0x0000D50F File Offset: 0x0000B70F
		public void CreateJunction(string sourceName, string targetPartition, string targetPath, bool useWellKnownGuids)
		{
			NativeImaging.CreateJunction(this.ServiceHandle, this.StoreId, sourceName, targetPartition, targetPath, useWellKnownGuids);
		}

		// Token: 0x06000320 RID: 800 RVA: 0x0000D52C File Offset: 0x0000B72C
		public void SetFullFlashUpdateStore(FullFlashUpdateImage.FullFlashUpdateStore store)
		{
			if (this._image != null)
			{
				throw new ImageStorageException("ImageStorage already has a FullFlashUpdateImage.");
			}
			if (this._store != null)
			{
				throw new ImageStorageException("ImageStorage already has a FullFlashUpdateStore.");
			}
			this._image = store.Image;
			this._store = store;
		}

		// Token: 0x06000321 RID: 801 RVA: 0x0000D567 File Offset: 0x0000B767
		public void DetachVirtualHardDisk(bool deleteFile)
		{
			NativeImaging.DismountVirtualHardDisk(this._service, this._storeId, true, deleteFile, true);
		}

		// Token: 0x06000322 RID: 802 RVA: 0x0000D584 File Offset: 0x0000B784
		public void CreateVirtualHardDiskFromStore(FullFlashUpdateImage.FullFlashUpdateStore store, string imagePath, uint partitionStyle, bool preparePartitions, ImageStructures.STORE_ID[] storeIds)
		{
			this._image = store.Image;
			this._store = store;
			this._isMainOSStorage = store.IsMainOSStore;
			if (string.IsNullOrEmpty(imagePath))
			{
				imagePath = this.CreateBackingVhdFileName(store.SectorSize);
			}
			this.VirtualDiskFilePath = imagePath;
			List<FullFlashUpdateImage.FullFlashUpdatePartition> partitions = store.Partitions;
			List<string> list = new List<string>();
			if (store.MinSectorCount > 0U)
			{
				this.ImageSectorCount = store.MinSectorCount;
			}
			else
			{
				this.ImageSectorCount = (uint)(10737418240UL / (ulong)store.SectorCount);
			}
			int num = partitions.Count;
			if (partitionStyle == ImageConstants.PartitionTypeMbr)
			{
				num++;
			}
			uint num2 = 1U;
			if (ImageConstants.MINIMUM_PARTITION_SIZE > store.SectorSize)
			{
				num2 = ImageConstants.MINIMUM_PARTITION_SIZE / store.SectorSize;
			}
			foreach (FullFlashUpdateImage.FullFlashUpdatePartition fullFlashUpdatePartition in store.Partitions)
			{
				if (fullFlashUpdatePartition.TotalSectors < num2 && !fullFlashUpdatePartition.UseAllSpace)
				{
					fullFlashUpdatePartition.TotalSectors = num2;
				}
				if (string.Compare(fullFlashUpdatePartition.PrimaryPartition, fullFlashUpdatePartition.Name, true, CultureInfo.InvariantCulture) != 0)
				{
					if (list.Contains(fullFlashUpdatePartition.Name))
					{
						throw new ImageStorageException(string.Format("{0}: A duplicate partition cannot be used as a primary partition for another duplicate partition.", MethodBase.GetCurrentMethod().Name));
					}
					if (!list.Contains(fullFlashUpdatePartition.PrimaryPartition))
					{
						list.Add(fullFlashUpdatePartition.PrimaryPartition);
					}
					list.Add(fullFlashUpdatePartition.Name);
				}
			}
			int num3 = -1;
			ImageStructures.PARTITION_ENTRY[] array = new ImageStructures.PARTITION_ENTRY[num];
			int i = 0;
			while (i < array.Length)
			{
				int num4 = i;
				if (i != array.Length - 1 || partitionStyle != ImageConstants.PartitionTypeMbr)
				{
					goto IL_21A;
				}
				if (num3 != -1)
				{
					num4 = num3;
					goto IL_21A;
				}
				array[i].PartitionName = ImageConstants.MBR_METADATA_PARTITION_NAME;
				array[i].FileSystem = "";
				array[i].SectorCount = (ulong)(ImageConstants.MBR_METADATA_PARTITION_SIZE / store.SectorSize);
				array[i].MBRFlags = 0;
				array[i].MBRType = ImageConstants.MBR_METADATA_PARTITION_TYPE;
				array[i].AlignmentSizeInBytes = store.SectorSize;
				IL_3AF:
				i++;
				continue;
				IL_21A:
				FullFlashUpdateImage.FullFlashUpdatePartition fullFlashUpdatePartition2 = partitions[num4];
				if (!fullFlashUpdatePartition2.UseAllSpace || partitionStyle != ImageConstants.PartitionTypeMbr || num4 == num3)
				{
					this.ValidatePartitionStrings(fullFlashUpdatePartition2);
					uint alignmentInBytes = store.SectorSize;
					if (fullFlashUpdatePartition2.ByteAlignment != 0U)
					{
						if (fullFlashUpdatePartition2.ByteAlignment < store.SectorSize)
						{
							throw new ImageStorageException(string.Format("{0}: The alignment for partition '{1}' is smaller than the sector size: 0x{2:x}/0x{3:x}.", new object[]
							{
								MethodBase.GetCurrentMethod().Name,
								fullFlashUpdatePartition2.Name,
								fullFlashUpdatePartition2.ByteAlignment,
								store.SectorSize
							}));
						}
						alignmentInBytes = fullFlashUpdatePartition2.ByteAlignment;
					}
					else if (this._image.DefaultPartitionAlignmentInBytes > store.SectorSize)
					{
						alignmentInBytes = this._image.DefaultPartitionAlignmentInBytes;
					}
					this.PreparePartitionEntry(ref array[i], store, fullFlashUpdatePartition2, partitionStyle, alignmentInBytes);
					goto IL_3AF;
				}
				if (num3 != -1)
				{
					throw new ImageStorageException(string.Format("There are two partition set to use all remaining space on disk: {0} and {1}", store.Partitions[num4].Name, store.Partitions[num3].Name));
				}
				num3 = num4;
				array[i].PartitionName = ImageConstants.MBR_METADATA_PARTITION_NAME;
				array[i].FileSystem = "";
				array[i].SectorCount = (ulong)(ImageConstants.MBR_METADATA_PARTITION_SIZE / store.SectorSize);
				array[i].MBRFlags = 0;
				array[i].MBRType = ImageConstants.MBR_METADATA_PARTITION_TYPE;
				array[i].AlignmentSizeInBytes = store.SectorSize;
				goto IL_3AF;
			}
			ulong num5 = 0UL;
			ImageStructures.PARTITION_ENTRY[] array2 = array;
			if (array2[array2.Length - 1].SectorCount == (ulong)-1)
			{
				for (int j = 0; j < array.Length; j++)
				{
					if (partitionStyle == ImageConstants.PartitionTypeMbr && num3 != -1 && j >= 3)
					{
						num5 += (ulong)(65536U / store.SectorSize);
					}
					uint num6 = array[j].AlignmentSizeInBytes / store.SectorSize;
					if (num5 == 0UL || num5 % (ulong)num6 != 0UL)
					{
						num5 += (ulong)num6 - num5 % (ulong)num6;
					}
					if (j == array.Length - 1)
					{
						break;
					}
					num5 += array[j].SectorCount;
				}
				uint num7 = 0U;
				if (partitionStyle == ImageConstants.PartitionTypeGpt)
				{
					num7 += 2U * ImageConstants.PARTITION_TABLE_METADATA_SIZE;
				}
				else
				{
					num7 = ImageConstants.PAYLOAD_BLOCK_SIZE;
					if (array.Length > 3)
					{
						num7 += (uint)((array.Length - 3) * (int)ImageConstants.PAYLOAD_BLOCK_SIZE);
					}
				}
				num5 += (ulong)(num7 / store.SectorSize);
			}
			if (num5 > (ulong)this.ImageSectorCount)
			{
				throw new ImageStorageException("The store's minSectorCount is less than the count of sectors in its partitions.");
			}
			int k = 0;
			while (k < array.Length)
			{
				if (array[k].SectorCount == (ulong)-1)
				{
					array[k].SectorCount = (ulong)this.ImageSectorCount - num5;
					ulong num8 = array[k].SectorCount * (ulong)store.SectorSize;
					if (num8 % (ulong)ImageConstants.PAYLOAD_BLOCK_SIZE == 0UL)
					{
						break;
					}
					ulong num9 = num8;
					num8 = num9 - num9 % (ulong)ImageConstants.PAYLOAD_BLOCK_SIZE;
					array[k].SectorCount = num8 / (ulong)store.SectorSize;
					if (array[k].SectorCount == 0UL)
					{
						throw new ImageStorageException("The store's minSectorCount is less than the count of sectors in its partitions.");
					}
					break;
				}
				else
				{
					k++;
				}
			}
			try
			{
				ulong maxSizeInBytes = (ulong)this.ImageSectorCount * (ulong)store.SectorSize;
				this.CleanupAllMountedDisks();
				IntPtr preexistingHandle = NativeImaging.InitializeVirtualHardDisk(this._service, this.VirtualDiskFilePath, maxSizeInBytes, ref this._storeId, array, preparePartitions, store.IsMainOSStore, this.VirtualHardDiskSectorSize, storeIds);
				this._storeHandle = new SafeFileHandle(preexistingHandle, true);
			}
			catch (ImageStorageException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new ImageStorageException(string.Format("{0}: Unable to create the VHD.", MethodBase.GetCurrentMethod().Name), innerException);
			}
			if (store.IsMainOSStore)
			{
				this._pathsToRemove.Add(this.GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME));
			}
			this.PostProcessVHD = true;
		}

		// Token: 0x06000323 RID: 803 RVA: 0x0000DBC4 File Offset: 0x0000BDC4
		public void MountExistingVirtualHardDisk(string imagePath, bool readOnly)
		{
			this._storeId = default(ImageStructures.STORE_ID);
			this.ReadOnlyVirtualDisk = readOnly;
			this.VirtualDiskFilePath = imagePath;
			try
			{
				using (DynamicHardDisk dynamicHardDisk = new DynamicHardDisk(imagePath, false))
				{
					using (VirtualDiskStream virtualDiskStream = new VirtualDiskStream(dynamicHardDisk))
					{
						MasterBootRecord masterBootRecord = new MasterBootRecord(this.Logger, (int)dynamicHardDisk.SectorSize, this._manager.IsDesktopImage);
						masterBootRecord.ReadFromStream(virtualDiskStream, MasterBootRecord.MbrParseType.Normal);
						if (masterBootRecord.IsValidProtectiveMbr())
						{
							GuidPartitionTable guidPartitionTable = new GuidPartitionTable((int)dynamicHardDisk.SectorSize, this.Logger);
							guidPartitionTable.ReadFromStream(virtualDiskStream, true, this._manager.IsDesktopImage);
							bool flag = false;
							using (List<GuidPartitionTableEntry>.Enumerator enumerator = guidPartitionTable.Entries.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									if (string.Compare(enumerator.Current.PartitionName, ImageConstants.MAINOS_PARTITION_NAME, true, CultureInfo.InvariantCulture) == 0)
									{
										flag = true;
									}
								}
							}
							if (!flag)
							{
								throw new ImageStorageException(string.Format("{0}: The given VHD does not contain the partition '{1}'.", MethodBase.GetCurrentMethod().Name, ImageConstants.MAINOS_PARTITION_NAME));
							}
							this._storeId.StoreType = ImageConstants.PartitionTypeGpt;
							this._storeId.StoreId_GPT = guidPartitionTable.Header.DiskId;
						}
						else
						{
							if (masterBootRecord.FindPartitionByName(ImageConstants.MAINOS_PARTITION_NAME) == null)
							{
								throw new ImageStorageException(string.Format("{0}: The given VHD does not contain the partition '{1}'.", MethodBase.GetCurrentMethod().Name, ImageConstants.MAINOS_PARTITION_NAME));
							}
							this._storeId.StoreType = ImageConstants.PartitionTypeMbr;
							this._storeId.StoreId_MBR = masterBootRecord.DiskSignature;
						}
					}
				}
			}
			catch (ImageStorageException)
			{
			}
			NativeImaging.DismountVirtualHardDisk(this.ServiceHandle, this.StoreId, false, false, false);
			IntPtr preexistingHandle = NativeImaging.OpenVirtualHardDisk(this._service, imagePath, out this._storeId, readOnly);
			this._storeHandle = new SafeFileHandle(preexistingHandle, true);
			string text = string.Empty;
			try
			{
				text = BuildPaths.GetImagingTempPath(Path.GetTempPath());
				text += ".mnt\\";
				Directory.CreateDirectory(text);
				this._pathsToRemove.Add(text);
			}
			catch (SecurityException innerException)
			{
				throw new ImageStorageException("Unable to retrieve a temporary path.", innerException);
			}
			NativeImaging.AddAccessPath(this._service, this._storeId, ImageConstants.MAINOS_PARTITION_NAME, text);
		}

		// Token: 0x06000324 RID: 804 RVA: 0x0000DE78 File Offset: 0x0000C078
		public void DismountVirtualHardDisk(bool skipPostProcessing)
		{
			this.DismountVirtualHardDisk(skipPostProcessing, false);
		}

		// Token: 0x06000325 RID: 805 RVA: 0x0000DE84 File Offset: 0x0000C084
		public void DismountVirtualHardDisk(bool skipPostProcessing, bool normalizeDiskSignature)
		{
			int tickCount = Environment.TickCount;
			if (this._storeHandle == null)
			{
				this.Logger.DebugLogger("{0}: This function was called when no image is mounted.", new object[]
				{
					MethodBase.GetCurrentMethod().Name
				});
				return;
			}
			if (!this.ReadOnlyVirtualDisk && !skipPostProcessing)
			{
				this.Logger.LogInfo("{0}:[{1}] Enabling USN journal on partition {2}.", new object[]
				{
					MethodBase.GetCurrentMethod().Name,
					(double)(Environment.TickCount - tickCount) / 1000.0,
					ImageConstants.MAINOS_PARTITION_NAME
				});
				NativeImaging.CreateUsnJournal(this._service, this.StoreId, ImageConstants.MAINOS_PARTITION_NAME);
				this.Logger.LogInfo("{0}:[{1}] Enabling USN journal on partition {2}.", new object[]
				{
					MethodBase.GetCurrentMethod().Name,
					(double)(Environment.TickCount - tickCount) / 1000.0,
					ImageConstants.DATA_PARTITION_NAME
				});
				NativeImaging.CreateUsnJournal(this._service, this.StoreId, ImageConstants.DATA_PARTITION_NAME);
				if (this.StoreId.StoreType == ImageConstants.PartitionTypeMbr)
				{
					this.Logger.LogInfo("{0}:[{1}] Updating the BCD.", new object[]
					{
						MethodBase.GetCurrentMethod().Name,
						(double)(Environment.TickCount - tickCount) / 1000.0
					});
					this.UpdateBootConfigurationDatabase(ImageConstants.BCD_FILE_PATH, ImageConstants.SYSTEM_STORE_SIGNATURE);
				}
				if (normalizeDiskSignature)
				{
					NativeImaging.WriteMountManagerRegistry2(this._service, this._storeId, true);
					NativeImaging.NormalizeVolumeMountPoints(this._service, this._storeId, this.GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME));
				}
			}
			this._storeHandle.Close();
			this._storeHandle = null;
			NativeImaging.DismountVirtualHardDisk(this._service, this._storeId, false, false, true);
			if (!this.ReadOnlyVirtualDisk && !skipPostProcessing && this.PostProcessVHD && this._image != null)
			{
				string fileSystem = null;
				string bootPartitionName = null;
				foreach (FullFlashUpdateImage.FullFlashUpdatePartition fullFlashUpdatePartition in this._store.Partitions)
				{
					if (fullFlashUpdatePartition.Bootable)
					{
						fileSystem = fullFlashUpdatePartition.FileSystem;
						bootPartitionName = fullFlashUpdatePartition.Name;
						break;
					}
				}
				this.Logger.LogInfo("{0}:[{1}] Making the virtual disk bootable.", new object[]
				{
					MethodBase.GetCurrentMethod().Name,
					(double)(Environment.TickCount - tickCount) / 1000.0
				});
				ImageStorage.PostProcessVirtualHardDisk(this.VirtualDiskFilePath, this.Logger, bootPartitionName, fileSystem, normalizeDiskSignature);
			}
			this.Logger.LogInfo("{0}:[{1}] Cleaning up temporary paths.", new object[]
			{
				MethodBase.GetCurrentMethod().Name,
				(double)(Environment.TickCount - tickCount) / 1000.0
			});
			this.CleanupTemporaryPaths();
			this._image = null;
			this._store = null;
			this._storeHandle = null;
			this._storeId = default(ImageStructures.STORE_ID);
			int num = Environment.TickCount - tickCount;
			this.Logger.LogInfo("Storage Service: Dismounting the image in {0:F1} seconds.", new object[]
			{
				(double)num / 1000.0
			});
		}

		// Token: 0x06000326 RID: 806 RVA: 0x0000E1D8 File Offset: 0x0000C3D8
		public string GetPartitionPath(string partitionName)
		{
			StringBuilder stringBuilder = new StringBuilder("path", 1024);
			if (this._storeId.StoreId_GPT == Guid.Empty && this._storeId.StoreId_MBR == 0U)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				NativeImaging.GetPartitionPathNoContext(partitionName, stringBuilder2, (uint)stringBuilder2.Capacity);
			}
			else
			{
				IntPtr serviceHandle = this._service;
				ImageStructures.STORE_ID storeId = this._storeId;
				StringBuilder stringBuilder3 = stringBuilder;
				NativeImaging.GetPartitionPath(serviceHandle, storeId, partitionName, stringBuilder3, (uint)stringBuilder3.Capacity);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000327 RID: 807 RVA: 0x0000E24C File Offset: 0x0000C44C
		public static string GetPartitionPathNoContext(string partitionName)
		{
			StringBuilder stringBuilder = new StringBuilder("path", 1024);
			StringBuilder stringBuilder2 = stringBuilder;
			NativeImaging.GetPartitionPathNoContext(partitionName, stringBuilder2, (uint)stringBuilder2.Capacity);
			return stringBuilder.ToString();
		}

		// Token: 0x06000328 RID: 808 RVA: 0x0000E27C File Offset: 0x0000C47C
		public string GetDiskName()
		{
			return NativeImaging.GetDiskName(this.ServiceHandle, this.StoreId);
		}

		// Token: 0x06000329 RID: 809 RVA: 0x0000E294 File Offset: 0x0000C494
		public void SetDiskAttributes(ImageStructures.DiskAttributes attributes, ImageStructures.DiskAttributes attributesMask, bool persist)
		{
			ImageStructures.SetDiskAttributes setDiskAttributes = default(ImageStructures.SetDiskAttributes);
			setDiskAttributes.Version = (uint)Marshal.SizeOf(setDiskAttributes);
			setDiskAttributes.Persist = (persist ? 1 : 0);
			setDiskAttributes.AttributesMask = attributesMask;
			setDiskAttributes.Attributes = attributes;
			NativeImaging.SetDiskAttributes(this._service, this.StoreHandle, setDiskAttributes);
		}

		// Token: 0x0600032A RID: 810 RVA: 0x0000E2F0 File Offset: 0x0000C4F0
		public void FormatPartition(string partitionName, string fileSystem, uint cbClusterSize)
		{
			NativeImaging.FormatPartition(this._service, this._storeId, partitionName, fileSystem, cbClusterSize);
		}

		// Token: 0x0600032B RID: 811 RVA: 0x0000E30C File Offset: 0x0000C50C
		public void AttachWOFToVolume(string partitionName)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			IntPtr serviceHandle = this._service;
			ImageStructures.STORE_ID storeId = this._storeId;
			StringBuilder stringBuilder2 = stringBuilder;
			NativeImaging.GetPartitionPath(serviceHandle, storeId, partitionName, stringBuilder2, (uint)stringBuilder2.Capacity);
			NativeImaging.AttachWOFToVolume(this._service, stringBuilder.ToString());
		}

		// Token: 0x0600032C RID: 812 RVA: 0x0000E358 File Offset: 0x0000C558
		public Guid GetPartitionTypeGpt(string partitionName)
		{
			return NativeImaging.GetPartitionType(this._service, this._storeId, partitionName).gptType;
		}

		// Token: 0x0600032D RID: 813 RVA: 0x0000E376 File Offset: 0x0000C576
		public byte GetPartitionTypeMbr(string partitionName)
		{
			return NativeImaging.GetPartitionType(this._service, this._storeId, partitionName).mbrType;
		}

		// Token: 0x0600032E RID: 814 RVA: 0x0000E394 File Offset: 0x0000C594
		public void SetPartitionType(string partitionName, Guid partitionType)
		{
			ImageStructures.PartitionType partitionType2 = default(ImageStructures.PartitionType);
			partitionType2.gptType = partitionType;
			NativeImaging.SetPartitionType(this._service, this._storeId, partitionName, partitionType2);
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0000E3CC File Offset: 0x0000C5CC
		public void SetPartitionType(string partitionName, byte partitionType)
		{
			ImageStructures.PartitionType partitionType2 = default(ImageStructures.PartitionType);
			partitionType2.mbrType = partitionType;
			NativeImaging.SetPartitionType(this._service, this._storeId, partitionName, partitionType2);
		}

		// Token: 0x06000330 RID: 816 RVA: 0x0000E401 File Offset: 0x0000C601
		public ulong GetPartitionAttributesGpt(string partitionName)
		{
			if (this._storeId.StoreType != ImageConstants.PartitionTypeGpt)
			{
				throw new ImageStorageException("UInt64 GetPartitionAttributes(string) can only be called on an GPT style disk.");
			}
			return NativeImaging.GetPartitionAttributes(this._service, this._storeId, partitionName).gptAttributes;
		}

		// Token: 0x06000331 RID: 817 RVA: 0x0000E43C File Offset: 0x0000C63C
		public byte GetPartitionAttributesMbr(string partitionName)
		{
			if (this._storeId.StoreType != ImageConstants.PartitionTypeMbr)
			{
				throw new ImageStorageException("byte GetPartitionAttributes(string) can only be called on an MBR style disk.");
			}
			return NativeImaging.GetPartitionAttributes(this._service, this._storeId, partitionName).mbrAttributes;
		}

		// Token: 0x06000332 RID: 818 RVA: 0x0000E478 File Offset: 0x0000C678
		public void SetPartitionAttributes(string partitionName, ulong attributes)
		{
			ImageStructures.PartitionAttributes attributes2 = default(ImageStructures.PartitionAttributes);
			attributes2.gptAttributes = attributes;
			NativeImaging.SetPartitionAttributes(this._service, this._storeId, partitionName, attributes2);
		}

		// Token: 0x06000333 RID: 819 RVA: 0x0000E4AD File Offset: 0x0000C6AD
		public ulong GetPartitionSize(string partitionName)
		{
			return NativeImaging.GetPartitionSize(this._service, this._storeId, partitionName);
		}

		// Token: 0x06000334 RID: 820 RVA: 0x0000E4C6 File Offset: 0x0000C6C6
		public string GetPartitionFileSystem(string partitionName)
		{
			return NativeImaging.GetPartitionFileSystem(this._service, this.StoreId, partitionName);
		}

		// Token: 0x06000335 RID: 821 RVA: 0x0000E4DF File Offset: 0x0000C6DF
		public bool IsPartitionTargeted(string partition)
		{
			return this._manager.IsPartitionTargeted(partition);
		}

		// Token: 0x06000336 RID: 822 RVA: 0x0000E4ED File Offset: 0x0000C6ED
		public bool IsBackingFileVhdx()
		{
			return this.VirtualDiskFilePath.EndsWith(".vhdx", true, CultureInfo.InvariantCulture);
		}

		// Token: 0x06000337 RID: 823 RVA: 0x0000E505 File Offset: 0x0000C705
		public ulong GetFreeBytesOnVolume(string partitionName)
		{
			return NativeImaging.GetFreeBytesOnVolume(this._service, this._storeId, partitionName);
		}

		// Token: 0x06000338 RID: 824 RVA: 0x0000E51E File Offset: 0x0000C71E
		public SafeFileHandle OpenVolumeHandle(string partitionName)
		{
			return NativeImaging.OpenVolumeHandle(this._service, this._storeId, partitionName, FileAccess.ReadWrite, FileShare.ReadWrite);
		}

		// Token: 0x06000339 RID: 825 RVA: 0x0000E539 File Offset: 0x0000C739
		public SafeFileHandle OpenVolumeHandle(string partitionName, FileAccess access, FileShare share)
		{
			return NativeImaging.OpenVolumeHandle(this._service, this._storeId, partitionName, access, share);
		}

		// Token: 0x0600033A RID: 826 RVA: 0x0000E554 File Offset: 0x0000C754
		public void WaitForVolume(string strVolumeName)
		{
			this.WaitForVolume(strVolumeName, int.MaxValue);
		}

		// Token: 0x0600033B RID: 827 RVA: 0x0000E562 File Offset: 0x0000C762
		public void WaitForVolume(string strVolumeName, int timeout)
		{
			NativeImaging.WaitForVolumeArrival(this._service, this._storeId, strVolumeName, timeout);
		}

		// Token: 0x0600033C RID: 828 RVA: 0x0000E57C File Offset: 0x0000C77C
		public void LockAndDismountVolume(string partitionName)
		{
			this.LockAndDismountVolume(partitionName, false);
		}

		// Token: 0x0600033D RID: 829 RVA: 0x0000E588 File Offset: 0x0000C788
		public void LockAndDismountVolume(string partitionName, bool forceDismount)
		{
			using (SafeVolumeHandle safeVolumeHandle = new SafeVolumeHandle(this, partitionName))
			{
				NativeImaging.LockAndDismountVolume(this._service, safeVolumeHandle.VolumeHandle, forceDismount);
			}
		}

		// Token: 0x0600033E RID: 830 RVA: 0x0000E5D0 File Offset: 0x0000C7D0
		public void UnlockVolume(string partitionName)
		{
			using (SafeVolumeHandle safeVolumeHandle = new SafeVolumeHandle(this, partitionName))
			{
				NativeImaging.UnlockVolume(this._service, safeVolumeHandle.VolumeHandle);
			}
		}

		// Token: 0x0600033F RID: 831 RVA: 0x0000E618 File Offset: 0x0000C818
		public bool PartitionIsMountedRaw(string partitionName)
		{
			string partitionFileSystem = this.GetPartitionFileSystem(partitionName);
			return string.Compare("RAW", partitionFileSystem, true, CultureInfo.InvariantCulture) == 0;
		}

		// Token: 0x06000340 RID: 832 RVA: 0x0000E643 File Offset: 0x0000C843
		public void CreateUsnJournal(string partitionName)
		{
			NativeImaging.CreateUsnJournal(this._service, this.StoreId, partitionName);
		}

		// Token: 0x06000341 RID: 833 RVA: 0x0000E65C File Offset: 0x0000C85C
		internal void MountFullFlashImageStore(FullFlashUpdateImage.FullFlashUpdateStore store, PayloadReader payloadReader, StorePayload payload, bool randomizeGptIds)
		{
			this.VirtualDiskFilePath = this.CreateBackingVhdFileName(store.SectorSize);
			this._image = store.Image;
			this._store = store;
			int tickCount = Environment.TickCount;
			if (store.SectorSize > this.BytesPerBlock)
			{
				throw new ImageStorageException(string.Format("The sector size (0x{0:x} bytes) is greater than the image block size (0x{1x} bytes)", store.SectorSize, this.BytesPerBlock));
			}
			if (this.BytesPerBlock % store.SectorSize != 0U)
			{
				throw new ImageStorageException(string.Format("The block size (0x{0:x} bytes) is not a mulitple of the sector size (0x{1x} bytes)", this.BytesPerBlock, store.SectorSize));
			}
			ulong num = (ulong)store.SectorCount;
			if (this._image == null)
			{
				throw new ImageStorageException(string.Format("{0}: The full flash update image has not been set.", MethodBase.GetCurrentMethod().Name));
			}
			if (num == 0UL)
			{
				num = 10737418240UL / (ulong)store.SectorSize;
			}
			this._storeId.StoreType = store.Image.ImageStyle;
			if (store.Id != null)
			{
				if (this._storeId.StoreType == ImageConstants.PartitionTypeGpt)
				{
					this._storeId.StoreId_GPT = Guid.Parse(store.Id);
				}
				else
				{
					this._storeId.StoreId_MBR = Convert.ToUInt32(store.Id);
				}
			}
			IntPtr preexistingHandle = NativeImaging.CreateEmptyVirtualDisk(this._service, this.VirtualDiskFilePath, ref this._storeId, num * (ulong)store.SectorSize, this.VirtualHardDiskSectorSize);
			this._storeHandle = new SafeFileHandle(preexistingHandle, true);
			int num2 = Environment.TickCount - tickCount;
			this.Logger.LogInfo("Storage Service: Created a new image in {0:F1} seconds.", new object[]
			{
				(double)num2 / 1000.0
			});
			string text = string.Empty;
			string text2 = string.Empty;
			bool flag = false;
			foreach (FullFlashUpdateImage.FullFlashUpdatePartition fullFlashUpdatePartition in store.Partitions)
			{
				if (fullFlashUpdatePartition.Name.Equals(ImageConstants.SYSTEM_PARTITION_NAME, StringComparison.OrdinalIgnoreCase))
				{
					if (!ImageStorage.IsPartitionHidden(fullFlashUpdatePartition))
					{
						flag = true;
						break;
					}
					break;
				}
			}
			try
			{
				string imagingTempPath = BuildPaths.GetImagingTempPath(Path.GetTempPath());
				text = imagingTempPath + ".mnt\\";
				Directory.CreateDirectory(text);
				this._pathsToRemove.Add(text);
				if (flag)
				{
					text2 = imagingTempPath + ".efiesp.mnt\\";
					Directory.CreateDirectory(text2);
					this._pathsToRemove.Add(text2);
				}
			}
			catch (SecurityException innerException)
			{
				throw new ImageStorageException("Unable to retrieve a temporary path.", innerException);
			}
			try
			{
				this.MountFullFlashImageStoreInternal(store, payloadReader, payload, randomizeGptIds);
			}
			catch (Win32ExportException innerException2)
			{
				throw new ImageStorageException("Unable to mount the existing full flash update image.", innerException2);
			}
			if (store.IsMainOSStore)
			{
				NativeImaging.AddAccessPath(this._service, this._storeId, ImageConstants.MAINOS_PARTITION_NAME, text);
				if (flag)
				{
					NativeImaging.AddAccessPath(this._service, this._storeId, ImageConstants.SYSTEM_PARTITION_NAME, text2);
				}
				else
				{
					this.Logger.LogDebug("{0}: Not mounting the system partition because it is absent or hidden", new object[]
					{
						MethodBase.GetCurrentMethod().Name
					});
				}
				if (this.IsImageCompressed(text))
				{
					try
					{
						this.AttachWOFToVolume(ImageConstants.MAINOS_PARTITION_NAME);
						this.AttachWOFToVolume(ImageConstants.DATA_PARTITION_NAME);
					}
					catch (Exception)
					{
						this.Logger.LogWarning(string.Format("{0}: Unable to attach WOF to a volume.", MethodBase.GetCurrentMethod().Name), new object[0]);
					}
				}
			}
		}

		// Token: 0x06000342 RID: 834 RVA: 0x0000E9DC File Offset: 0x0000CBDC
		internal void AttachToMountedVirtualHardDisk(string physicalDiskPath, bool readOnly, bool isMainOSStore)
		{
			string empty = string.Empty;
			this._storeId = default(ImageStructures.STORE_ID);
			IntPtr zero = IntPtr.Zero;
			NativeImaging.AttachToMountedImage(this._service, physicalDiskPath, readOnly, out empty, out this._storeId, out zero);
			this._storeHandle = new SafeFileHandle(zero, true);
			if (isMainOSStore)
			{
				this.WaitForVolume(ImageConstants.MAINOS_PARTITION_NAME);
			}
			this._isMainOSStorage = isMainOSStore;
			this.VirtualDiskFilePath = empty;
		}

		// Token: 0x06000343 RID: 835 RVA: 0x0000EA48 File Offset: 0x0000CC48
		private string CreateBackingVhdFileName(uint sectorSize)
		{
			string result;
			try
			{
				string path;
				if (sectorSize == 512U)
				{
					path = Guid.NewGuid().ToString("N") + ".vhd";
				}
				else
				{
					path = Guid.NewGuid().ToString("N") + ".vhdx";
				}
				string text = Environment.GetEnvironmentVariable("VHDTMP");
				if (text == null)
				{
					text = Path.GetDirectoryName(BuildPaths.GetImagingTempPath(Path.GetTempPath()));
				}
				result = Path.Combine(text, path);
			}
			catch (SecurityException innerException)
			{
				throw new ImageStorageException("Unable to retrieve a temporary path.", innerException);
			}
			return result;
		}

		// Token: 0x06000344 RID: 836 RVA: 0x0000EAE4 File Offset: 0x0000CCE4
		private static void PostProcessVirtualHardDisk(string virtualImagePath, IULogger logger, string bootPartitionName, string fileSystem, bool normalizeDiskSignature)
		{
			bool flag = false;
			using (DynamicHardDisk dynamicHardDisk = new DynamicHardDisk(virtualImagePath, true))
			{
				using (VirtualDiskStream virtualDiskStream = new VirtualDiskStream(dynamicHardDisk))
				{
					MasterBootRecord masterBootRecord = new MasterBootRecord(logger, (int)dynamicHardDisk.SectorSize);
					masterBootRecord.ReadFromStream(virtualDiskStream, MasterBootRecord.MbrParseType.Normal);
					if (!masterBootRecord.IsValidProtectiveMbr() && !string.IsNullOrEmpty(fileSystem) && !string.IsNullOrEmpty(bootPartitionName))
					{
						if (masterBootRecord.FindPartitionByName(bootPartitionName) == null)
						{
							throw new ImageStorageException(string.Format("{0}: No bootable partition was found in the image.", MethodBase.GetCurrentMethod().Name));
						}
						flag = true;
					}
					if (normalizeDiskSignature && masterBootRecord.DiskSignature != ImageConstants.SYSTEM_STORE_SIGNATURE)
					{
						masterBootRecord.DiskSignature = ImageConstants.SYSTEM_STORE_SIGNATURE;
						flag = true;
					}
					if (flag)
					{
						masterBootRecord.WriteToStream(virtualDiskStream, true);
					}
				}
			}
		}

		// Token: 0x06000345 RID: 837 RVA: 0x0000EBB4 File Offset: 0x0000CDB4
		private void PrepareLogging()
		{
			this._logWarning = new LogFunction(this.LogWarning);
			this._logInfo = new LogFunction(this.LogInfo);
			this._logDebug = new LogFunction(this.LogDebug);
			NativeImaging.SetLoggingFunction(this._service, NativeImaging.LogLevel.levelWarning, this._logWarning);
			NativeImaging.SetLoggingFunction(this._service, NativeImaging.LogLevel.levelInfo, this._logInfo);
			NativeImaging.SetLoggingFunction(this._service, NativeImaging.LogLevel.levelDebug, this._logDebug);
			string etwlogPath = NativeImaging.GetETWLogPath(this._service);
			this.LogInfo(string.Format("ETW Log Path: {0}", etwlogPath));
			OperatingSystem osversion = Environment.OSVersion;
			this.LogInfo(string.Format("OS Version: {0}", osversion.VersionString));
		}

		// Token: 0x06000346 RID: 838 RVA: 0x0000EC7C File Offset: 0x0000CE7C
		private void CleanupTemporaryPaths()
		{
			foreach (string text in this._pathsToRemove)
			{
				this.Logger.LogInfo("{0}: Cleaning up temporary path {1}.", new object[]
				{
					MethodBase.GetCurrentMethod().Name,
					text
				});
				FileUtils.DeleteTree(text);
			}
			this._pathsToRemove.Clear();
		}

		// Token: 0x06000347 RID: 839 RVA: 0x0000ED00 File Offset: 0x0000CF00
		public void UpdateBootConfigurationDatabase(string bcdFile, uint diskSignature)
		{
			bool save = false;
			ulong num = NativeImaging.GetPartitionOffset(this._service, this._storeId, ImageConstants.MAINOS_PARTITION_NAME);
			ulong num2 = NativeImaging.GetPartitionOffset(this._service, this._storeId, ImageConstants.SYSTEM_PARTITION_NAME);
			if (this._image == null)
			{
				num *= (ulong)this.VirtualHardDiskSectorSize;
				num2 *= (ulong)this.VirtualHardDiskSectorSize;
			}
			else
			{
				num *= (ulong)this._image.Stores[0].SectorSize;
				num2 *= (ulong)this._image.Stores[0].SectorSize;
			}
			string text = null;
			try
			{
				text = this.GetPartitionPath(ImageConstants.SYSTEM_PARTITION_NAME) + bcdFile;
			}
			catch (ImageStorageException)
			{
				this.Logger.LogInfo("{0}: Not updating the BCD - unable to find the '{1}' partition.", new object[]
				{
					MethodBase.GetCurrentMethod().Name,
					ImageConstants.SYSTEM_PARTITION_NAME
				});
				return;
			}
			if (!File.Exists(text))
			{
				this.Logger.LogInfo("{0}: Not updating the BCD - unable to find the path: {1}", new object[]
				{
					MethodBase.GetCurrentMethod().Name,
					text
				});
				return;
			}
			PartitionIdentifierEx identifier = PartitionIdentifierEx.CreateSimpleMbr(num, diskSignature);
			PartitionIdentifierEx identifier2 = PartitionIdentifierEx.CreateSimpleMbr(num2, diskSignature);
			using (BootConfigurationDatabase bootConfigurationDatabase = new BootConfigurationDatabase(text))
			{
				bootConfigurationDatabase.Mount();
				BcdObject @object = bootConfigurationDatabase.GetObject(BcdObjects.WindowsLoader);
				BcdObject object2 = bootConfigurationDatabase.GetObject(BcdObjects.BootManager);
				BcdObject object3 = bootConfigurationDatabase.GetObject(BcdObjects.UpdateOSWim);
				BcdObject object4 = bootConfigurationDatabase.GetObject(BcdObjects.WindowsSetupRamdiskOptions);
				if (object2 == null)
				{
					throw new ImageStorageException(string.Format("{0}: The Boot Manager Object was not found.", MethodBase.GetCurrentMethod().Name));
				}
				for (int i = 0; i < bootConfigurationDatabase.Objects.Count; i++)
				{
					BcdObject bcdObject = bootConfigurationDatabase.Objects[i];
					for (int j = 0; j < bcdObject.Elements.Count; j++)
					{
						BcdElement bcdElement = bcdObject.Elements[j];
						if (bcdElement.DataType.Format == ElementFormat.Device)
						{
							BcdElementDevice bcdElementDevice = bcdElement as BcdElementDevice;
							if (bcdElementDevice == null)
							{
								throw new ImageStorageException(string.Format("{0}: The default application's device element is invalid.", MethodBase.GetCurrentMethod().Name));
							}
							if (bcdElementDevice.BootDevice.Type == BcdElementBootDevice.DeviceType.BlockIo)
							{
								if (object3 != null && bcdObject.Id == object3.Id)
								{
									bcdElementDevice.ReplaceRamDiskDeviceIdentifier(identifier);
									bootConfigurationDatabase.SaveElementValue(bcdObject, bcdElement);
									save = true;
								}
							}
							else if (bcdElementDevice.BootDevice.Type == BcdElementBootDevice.DeviceType.Boot && (bcdElementDevice.DataType.Equals(BcdElementDataTypes.OsLoaderDevice) || bcdElementDevice.DataType.Equals(BcdElementDataTypes.OsLoaderType) || bcdElementDevice.DataType.Equals(BcdElementDataTypes.RamDiskSdiDevice)))
							{
								if (bcdObject.Id == object2.Id)
								{
									bcdElementDevice.ReplaceBootDeviceIdentifier(identifier2);
									bootConfigurationDatabase.SaveElementValue(bcdObject, bcdElement);
									save = true;
								}
								else if (@object != null && bcdObject.Id == @object.Id)
								{
									bcdElementDevice.ReplaceBootDeviceIdentifier(identifier);
									bootConfigurationDatabase.SaveElementValue(bcdObject, bcdElement);
									save = true;
								}
								else if (object4 != null && bcdObject.Id == object4.Id)
								{
									bcdElementDevice.ReplaceBootDeviceIdentifier(identifier2);
									bootConfigurationDatabase.SaveElementValue(bcdObject, bcdElement);
									save = true;
								}
								else
								{
									this.Logger.LogInfo("{0}: Modifying unknown object device elements to point to the system partition. ID is {1}", new object[]
									{
										MethodBase.GetCurrentMethod().Name,
										bcdObject.Id
									});
									bcdElementDevice.ReplaceBootDeviceIdentifier(identifier2);
									bootConfigurationDatabase.SaveElementValue(bcdObject, bcdElement);
									save = true;
								}
							}
						}
					}
				}
				bootConfigurationDatabase.DismountHive(save);
			}
		}

		// Token: 0x06000348 RID: 840 RVA: 0x0000F0DC File Offset: 0x0000D2DC
		private void CleanupAllMountedDisks()
		{
			this.Logger.LogInfo("{0}: Cleaning up all mounted disks.", new object[]
			{
				MethodBase.GetCurrentMethod().Name
			});
			try
			{
				for (int i = 0; i < 10; i++)
				{
					NativeImaging.DismountVirtualHardDisk(this._service, this._storeId, true, false, false);
				}
			}
			catch (ImageStorageException)
			{
			}
		}

		// Token: 0x06000349 RID: 841 RVA: 0x0000F148 File Offset: 0x0000D348
		private void MountFullFlashImageStoreInternal(FullFlashUpdateImage.FullFlashUpdateStore store, PayloadReader payloadReader, StorePayload payload, bool randomizeGptIds)
		{
			uint imageStyle = this._image.ImageStyle;
			payloadReader.WriteToDisk(this.SafeStoreHandle, payload);
			using (DiskStreamSource diskStreamSource = new DiskStreamSource(this.SafeStoreHandle, payload.StoreHeader.BytesPerBlock))
			{
				using (DataBlockStream dataBlockStream = new DataBlockStream(diskStreamSource, payload.StoreHeader.BytesPerBlock))
				{
					if (imageStyle == ImageConstants.PartitionTypeGpt && store.IsMainOSStore)
					{
						GuidPartitionTable guidPartitionTable = new GuidPartitionTable((int)NativeImaging.GetSectorSize(this.ServiceHandle, this.SafeStoreHandle), this._logger);
						guidPartitionTable.ReadFromStream(dataBlockStream, true);
						guidPartitionTable.GetEntry(ImageConstants.MAINOS_PARTITION_NAME).Attributes |= ImageConstants.GPT_ATTRIBUTE_NO_DRIVE_LETTER;
						if (randomizeGptIds)
						{
							guidPartitionTable.RandomizeGptIds();
						}
						guidPartitionTable.FixCrcs();
						guidPartitionTable.WriteToStream(dataBlockStream, true, false);
						using (VirtualMemoryPtr virtualMemoryPtr = new VirtualMemoryPtr(payload.StoreHeader.BytesPerBlock))
						{
							foreach (DataBlockEntry dataBlockEntry in dataBlockStream.BlockEntries)
							{
								if (dataBlockEntry.DataSource.Source == DataBlockSource.DataSource.Memory)
								{
									long distanceToMove = (long)((ulong)dataBlockEntry.BlockLocationsOnDisk[0].BlockIndex * (ulong)payload.StoreHeader.BytesPerBlock);
									long num = 0L;
									uint num2 = 0U;
									Marshal.Copy(dataBlockEntry.DataSource.GetMemoryData(), 0, virtualMemoryPtr.AllocatedPointer, (int)payload.StoreHeader.BytesPerBlock);
									Win32Exports.SetFilePointerEx(this.SafeStoreHandle, distanceToMove, out num, Win32Exports.MoveMethod.FILE_BEGIN);
									Win32Exports.WriteFile(this.SafeStoreHandle, virtualMemoryPtr.AllocatedPointer, payload.StoreHeader.BytesPerBlock, out num2);
								}
							}
						}
					}
				}
			}
			NativeImaging.UpdateDiskLayout(this._service, this._storeHandle);
			this._storeId = NativeImaging.GetDiskId(this._service, this._storeHandle);
			List<ImageStructures.PARTITION_ENTRY> list = new List<ImageStructures.PARTITION_ENTRY>();
			for (int i = 0; i < store.PartitionCount; i++)
			{
				FullFlashUpdateImage.FullFlashUpdatePartition partition = store.Partitions[i];
				ImageStructures.PARTITION_ENTRY item = default(ImageStructures.PARTITION_ENTRY);
				this.PreparePartitionEntry(ref item, store, partition, imageStyle, 1U);
				list.Add(item);
			}
			list.TrimExcess();
			NativeImaging.WaitForPartitions(this._service, this._storeId, list.ToArray());
		}

		// Token: 0x0600034A RID: 842 RVA: 0x0000F408 File Offset: 0x0000D608
		private void ValidatePartitionStrings(FullFlashUpdateImage.FullFlashUpdatePartition partition)
		{
			if (partition.Name.Length > 32)
			{
				throw new ImageStorageException(string.Format("The partition name is too long: {0}.", partition.Name));
			}
			if (!string.IsNullOrEmpty(partition.FileSystem) && partition.FileSystem.Length > 32)
			{
				throw new ImageStorageException(string.Format("Partition {0}'s file system is too long.", partition.Name));
			}
		}

		// Token: 0x0600034B RID: 843 RVA: 0x0000F46C File Offset: 0x0000D66C
		private ulong FlagsFromPartition(FullFlashUpdateImage.FullFlashUpdatePartition partition)
		{
			ulong num = 0UL;
			if (partition.Hidden)
			{
				num |= 4611686018427387904UL;
			}
			if (partition.ReadOnly)
			{
				num |= 1152921504606846976UL;
			}
			if (!partition.AttachDriveLetter)
			{
				num |= 9223372036854775808UL;
			}
			return num;
		}

		// Token: 0x0600034C RID: 844 RVA: 0x0000F4BC File Offset: 0x0000D6BC
		private void PreparePartitionEntry(ref ImageStructures.PARTITION_ENTRY partitionEntry, FullFlashUpdateImage.FullFlashUpdateStore store, FullFlashUpdateImage.FullFlashUpdatePartition partition, uint partitionStyle, uint alignmentInBytes)
		{
			if (partitionStyle == ImageConstants.PartitionTypeGpt)
			{
				Guid partitionType;
				try
				{
					partitionType = new Guid(partition.PartitionType);
				}
				catch (Exception ex)
				{
					throw new ImageStorageException(string.Format("Partition {0}'s TYPE is invalid: {1}: {2}", partition.Name, partition.PartitionType, ex.Message));
				}
				partitionEntry.PartitionType = partitionType;
				Guid guid = Guid.Empty;
				bool flag = false;
				if (!string.IsNullOrEmpty(partition.PartitionId))
				{
					try
					{
						guid = new Guid(partition.PartitionId);
					}
					catch (Exception ex2)
					{
						throw new ImageStorageException(string.Format("Partition {0}'s ID is invalid: {1}: {2}", partition.Name, partition.PartitionId, ex2.Message));
					}
					flag = true;
				}
				if (this._manager.RandomizePartitionIDs)
				{
					partitionEntry.PartitionId = Guid.NewGuid();
				}
				else if (string.Compare(ImageConstants.MAINOS_PARTITION_NAME, partition.Name, true, CultureInfo.InvariantCulture) == 0)
				{
					partitionEntry.PartitionId = ImageConstants.MAINOS_PARTITION_ID;
					if (flag)
					{
						throw new ImageStorageException(string.Format("Unable to override protected partition {0}'s ID with {1}", partition.Name, partition.PartitionId));
					}
				}
				else if (string.Compare(ImageConstants.SYSTEM_PARTITION_NAME, partition.Name, true, CultureInfo.InvariantCulture) == 0)
				{
					partitionEntry.PartitionId = ImageConstants.SYSTEM_PARTITION_ID;
					if (flag)
					{
						throw new ImageStorageException(string.Format("Unable to override protected partition {0}'s ID with {1}", partition.Name, partition.PartitionId));
					}
				}
				else if (string.Compare(ImageConstants.MMOS_PARTITION_NAME, partition.Name, true, CultureInfo.InvariantCulture) == 0)
				{
					partitionEntry.PartitionId = ImageConstants.MMOS_PARTITION_ID;
					if (flag)
					{
						throw new ImageStorageException(string.Format("Unable to override protected partition {0}'s ID with {1}", partition.Name, partition.PartitionId));
					}
				}
				else
				{
					partitionEntry.PartitionId = (flag ? guid : Guid.NewGuid());
				}
				partitionEntry.PartitionFlags = this.FlagsFromPartition(partition);
			}
			else
			{
				if (partition.Bootable)
				{
					partitionEntry.MBRFlags = 128;
				}
				string partitionType2 = partition.PartitionType;
				byte mbrtype = 0;
				if (partitionType2.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
				{
					if (!byte.TryParse(partitionType2.Substring(2, partitionType2.Length - 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out mbrtype))
					{
						throw new ImageStorageException(string.Format("Partition MBR style {0}'s type cannot be parsed.", partition.Name));
					}
				}
				else if (!byte.TryParse(partitionType2, NumberStyles.Integer, CultureInfo.InvariantCulture, out mbrtype))
				{
					throw new ImageStorageException(string.Format("Partition GPT style {0}'s type cannot be parsed.", partition.Name));
				}
				partitionEntry.MBRType = mbrtype;
			}
			if (string.IsNullOrEmpty(partition.FileSystem))
			{
				partitionEntry.FileSystem = string.Empty;
			}
			else
			{
				partitionEntry.FileSystem = partition.FileSystem;
			}
			partitionEntry.PartitionName = partition.Name;
			partitionEntry.AlignmentSizeInBytes = alignmentInBytes;
			partitionEntry.ClusterSize = partition.ClusterSize;
			if (partition.UseAllSpace)
			{
				partitionEntry.SectorCount = (ulong)-1;
				return;
			}
			if (store.SectorSize != this.VirtualHardDiskSectorSize)
			{
				partitionEntry.SectorCount = (ulong)(partition.TotalSectors * (store.SectorSize / this.VirtualHardDiskSectorSize));
				return;
			}
			partitionEntry.SectorCount = (ulong)partition.TotalSectors;
		}

		// Token: 0x0600034D RID: 845 RVA: 0x0000F7A0 File Offset: 0x0000D9A0
		private bool IsImageCompressed(string accessPath)
		{
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			try
			{
				intPtr = OfflineRegUtils.OpenHive(Path.Combine(accessPath, "Windows\\system32\\config\\SYSTEM"));
				intPtr2 = OfflineRegUtils.OpenKey(intPtr, "Setup");
				return BitConverter.ToUInt32(OfflineRegUtils.GetValue(intPtr2, "Compact"), 0) == 1U;
			}
			catch (Win32Exception)
			{
			}
			catch (Exception)
			{
				this.Logger.LogWarning(string.Format("{0}: Unable to get Compact regkey value.", MethodBase.GetCurrentMethod().Name), new object[0]);
			}
			finally
			{
				if (intPtr2 != IntPtr.Zero)
				{
					OfflineRegUtils.CloseKey(intPtr2);
					intPtr2 = IntPtr.Zero;
				}
				if (intPtr != IntPtr.Zero)
				{
					OfflineRegUtils.CloseHive(intPtr);
					intPtr = IntPtr.Zero;
				}
			}
			return false;
		}

		// Token: 0x0600034E RID: 846 RVA: 0x0000F878 File Offset: 0x0000DA78
		private static bool IsPartitionHidden(FullFlashUpdateImage.FullFlashUpdatePartition partition)
		{
			Guid a;
			return partition.Hidden || (Guid.TryParse(partition.PartitionType, out a) && a == ImageConstants.PARTITION_SYSTEM_GUID);
		}

		// Token: 0x0600034F RID: 847 RVA: 0x0000F8B0 File Offset: 0x0000DAB0
		[Conditional("DEBUG")]
		internal void TestMountVirtualDisk(string existingDisk)
		{
			string tempFileName = Path.GetTempFileName();
			File.Copy(existingDisk, tempFileName, true);
			this.MountExistingVirtualHardDisk(tempFileName, true);
		}

		// Token: 0x06000350 RID: 848 RVA: 0x0000F8D3 File Offset: 0x0000DAD3
		[Conditional("DEBUG")]
		internal void TestDismountVirtualDisk()
		{
			if (!this._storeHandle.IsInvalid)
			{
				this.DismountVirtualHardDisk(false);
			}
		}

		// Token: 0x06000351 RID: 849 RVA: 0x0000F8EC File Offset: 0x0000DAEC
		[Conditional("DEBUG")]
		internal void TestValidateFileBuffer(byte[] fileBuffer, ulong diskOffset)
		{
			using (VirtualMemoryPtr virtualMemoryPtr = new VirtualMemoryPtr((uint)fileBuffer.Length))
			{
				uint value = 0U;
				long num = 0L;
				Win32Exports.SetFilePointerEx(this._storeHandle, (long)diskOffset, out num, Win32Exports.MoveMethod.FILE_BEGIN);
				Win32Exports.ReadFile(this._storeHandle, virtualMemoryPtr, virtualMemoryPtr.MemorySize, out value);
				if (Win32Exports.memcmp(fileBuffer, virtualMemoryPtr, (UIntPtr)value) != 0)
				{
					throw new ImageStorageException(string.Format("TEST: ValidateFileBuffer failed at disk offset {0}", diskOffset));
				}
			}
		}

		// Token: 0x040001AF RID: 431
		private ImageStorageManager _manager;

		// Token: 0x040001B0 RID: 432
		private SafeFileHandle _storeHandle;

		// Token: 0x040001B1 RID: 433
		private FullFlashUpdateImage _image;

		// Token: 0x040001B2 RID: 434
		private FullFlashUpdateImage.FullFlashUpdateStore _store;

		// Token: 0x040001B3 RID: 435
		private NativeServiceHandle _service;

		// Token: 0x040001B4 RID: 436
		private ImageStructures.STORE_ID _storeId;

		// Token: 0x040001B5 RID: 437
		private IULogger _logger;

		// Token: 0x040001B6 RID: 438
		private LogFunction _logError;

		// Token: 0x040001B7 RID: 439
		private LogFunction _logWarning;

		// Token: 0x040001B8 RID: 440
		private LogFunction _logInfo;

		// Token: 0x040001B9 RID: 441
		private LogFunction _logDebug;

		// Token: 0x040001BA RID: 442
		private List<string> _pathsToRemove;

		// Token: 0x040001BB RID: 443
		private bool _isMainOSStorage;

		// Token: 0x02000080 RID: 128
		private class PartitionInfo
		{
			// Token: 0x17000125 RID: 293
			// (get) Token: 0x060004DC RID: 1244 RVA: 0x00014E18 File Offset: 0x00013018
			// (set) Token: 0x060004DD RID: 1245 RVA: 0x00014E20 File Offset: 0x00013020
			public byte MbrType { get; set; }

			// Token: 0x17000126 RID: 294
			// (get) Token: 0x060004DE RID: 1246 RVA: 0x00014E29 File Offset: 0x00013029
			// (set) Token: 0x060004DF RID: 1247 RVA: 0x00014E31 File Offset: 0x00013031
			public Guid GptType { get; set; }

			// Token: 0x17000127 RID: 295
			// (get) Token: 0x060004E0 RID: 1248 RVA: 0x00014E3A File Offset: 0x0001303A
			// (set) Token: 0x060004E1 RID: 1249 RVA: 0x00014E42 File Offset: 0x00013042
			public byte MbrAttributes { get; set; }

			// Token: 0x17000128 RID: 296
			// (get) Token: 0x060004E2 RID: 1250 RVA: 0x00014E4B File Offset: 0x0001304B
			// (set) Token: 0x060004E3 RID: 1251 RVA: 0x00014E53 File Offset: 0x00013053
			public ulong GptAttributes { get; set; }
		}
	}
}
