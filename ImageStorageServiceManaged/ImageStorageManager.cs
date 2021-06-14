using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Win32.SafeHandles;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000049 RID: 73
	[CLSCompliant(false)]
	public class ImageStorageManager
	{
		// Token: 0x06000352 RID: 850 RVA: 0x0000F978 File Offset: 0x0000DB78
		public ImageStorageManager() : this(new IULogger())
		{
		}

		// Token: 0x06000353 RID: 851 RVA: 0x0000F985 File Offset: 0x0000DB85
		public ImageStorageManager(IULogger logger) : this(logger, null)
		{
		}

		// Token: 0x06000354 RID: 852 RVA: 0x0000F98F File Offset: 0x0000DB8F
		public ImageStorageManager(IULogger logger, IList<string> partitionsTargeted)
		{
			this._logger = logger;
			this._partitionsTargeted = partitionsTargeted;
			this._storages = new Dictionary<FullFlashUpdateImage.FullFlashUpdateStore, ImageStorage>();
			this._virtualHardDiskSectorSize = ImageConstants.DefaultVirtualHardDiskSectorSize;
			this.MountManagerScrubRegistry();
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06000355 RID: 853 RVA: 0x0000F9C1 File Offset: 0x0000DBC1
		public IULogger Logger
		{
			get
			{
				return this._logger;
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x06000356 RID: 854 RVA: 0x0000F9C9 File Offset: 0x0000DBC9
		public FullFlashUpdateImage Image
		{
			get
			{
				return this._image;
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06000357 RID: 855 RVA: 0x0000F9D1 File Offset: 0x0000DBD1
		public ReadOnlyCollection<ImageStorage> Storages
		{
			get
			{
				return this._storages.Values.ToList<ImageStorage>().AsReadOnly();
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000358 RID: 856 RVA: 0x0000F9E8 File Offset: 0x0000DBE8
		public ImageStorage MainOSStorage
		{
			get
			{
				FullFlashUpdateImage.FullFlashUpdateStore key = this._storages.Keys.Single((FullFlashUpdateImage.FullFlashUpdateStore s) => s.IsMainOSStore);
				return this._storages[key];
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000359 RID: 857 RVA: 0x0000FA31 File Offset: 0x0000DC31
		// (set) Token: 0x0600035A RID: 858 RVA: 0x0000FA3C File Offset: 0x0000DC3C
		public uint VirtualHardDiskSectorSize
		{
			get
			{
				return this._virtualHardDiskSectorSize;
			}
			set
			{
				this._virtualHardDiskSectorSize = value;
				foreach (ImageStorage imageStorage in this._storages.Values)
				{
					imageStorage.VirtualHardDiskSectorSize = value;
				}
			}
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x0600035B RID: 859 RVA: 0x0000FA9C File Offset: 0x0000DC9C
		// (set) Token: 0x0600035C RID: 860 RVA: 0x0000FAA4 File Offset: 0x0000DCA4
		public bool IsDesktopImage { get; set; }

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x0600035D RID: 861 RVA: 0x0000FAAD File Offset: 0x0000DCAD
		// (set) Token: 0x0600035E RID: 862 RVA: 0x0000FAB5 File Offset: 0x0000DCB5
		public bool RandomizeDiskIds { get; set; }

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x0600035F RID: 863 RVA: 0x0000FABE File Offset: 0x0000DCBE
		// (set) Token: 0x06000360 RID: 864 RVA: 0x0000FAC6 File Offset: 0x0000DCC6
		public bool RandomizePartitionIDs { get; set; }

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000361 RID: 865 RVA: 0x0000D433 File Offset: 0x0000B633
		internal uint BytesPerBlock
		{
			get
			{
				return ImageConstants.PAYLOAD_BLOCK_SIZE;
			}
		}

		// Token: 0x06000362 RID: 866 RVA: 0x0000FAD0 File Offset: 0x0000DCD0
		public void SetFullFlashImage(FullFlashUpdateImage image)
		{
			this._image = image;
			foreach (ImageStorage imageStorage in this._storages.Values)
			{
				imageStorage.SetFullFlashImage(image);
			}
		}

		// Token: 0x06000363 RID: 867 RVA: 0x0000FB30 File Offset: 0x0000DD30
		public void CreateFullFlashImage(FullFlashUpdateImage image)
		{
			if (this._image != null)
			{
				bool saveChanges = false;
				this.DismountFullFlashImage(saveChanges);
			}
			int tickCount = Environment.TickCount;
			image.DisplayImageInformation(this._logger);
			ImageStorageManager.CheckForDuplicateNames(image);
			ImageStorageManager.ValidateMainOsInImage(image);
			if (image.DefaultPartitionAlignmentInBytes < ImageConstants.PAYLOAD_BLOCK_SIZE)
			{
				image.DefaultPartitionAlignmentInBytes = ImageConstants.PAYLOAD_BLOCK_SIZE;
			}
			ImageStructures.STORE_ID[] array = new ImageStructures.STORE_ID[image.Stores.Count<FullFlashUpdateImage.FullFlashUpdateStore>()];
			for (int i = 0; i < image.Stores.Count<FullFlashUpdateImage.FullFlashUpdateStore>(); i++)
			{
				ImageStructures.STORE_ID store_ID = default(ImageStructures.STORE_ID);
				FullFlashUpdateImage.FullFlashUpdateStore fullFlashUpdateStore = image.Stores[i];
				store_ID.StoreType = image.ImageStyle;
				if (this.RandomizeDiskIds)
				{
					store_ID.StoreId_GPT = Guid.NewGuid();
				}
				else if (store_ID.StoreType == ImageConstants.PartitionTypeGpt)
				{
					store_ID.StoreId_GPT = (fullFlashUpdateStore.IsMainOSStore ? ImageConstants.SYSTEM_STORE_GUID : Guid.Parse(fullFlashUpdateStore.Id));
				}
				else
				{
					store_ID.StoreId_MBR = (fullFlashUpdateStore.IsMainOSStore ? ImageConstants.SYSTEM_STORE_SIGNATURE : Convert.ToUInt32(fullFlashUpdateStore.Id));
				}
				array[i] = store_ID;
			}
			for (int j = 0; j < image.Stores.Count<FullFlashUpdateImage.FullFlashUpdateStore>(); j++)
			{
				FullFlashUpdateImage.FullFlashUpdateStore fullFlashUpdateStore2 = image.Stores[j];
				if (fullFlashUpdateStore2.SectorSize > this.BytesPerBlock)
				{
					throw new ImageStorageException(string.Format("The sector size (0x{0:x} bytes) is greater than the image block size (0x{1x} bytes)", fullFlashUpdateStore2.SectorSize, this.BytesPerBlock));
				}
				if (this.BytesPerBlock % fullFlashUpdateStore2.SectorSize != 0U)
				{
					throw new ImageStorageException(string.Format("The block size (0x{0:x} bytes) is not a mulitple of the sector size (0x{1x} bytes)", this.BytesPerBlock, fullFlashUpdateStore2.SectorSize));
				}
				ulong num = (ulong)fullFlashUpdateStore2.SectorCount;
				if (num == 0UL)
				{
					throw new ImageStorageException("Please specify an image size using the MinSectorCount field in the device platform information file.");
				}
				if (num * (ulong)fullFlashUpdateStore2.SectorSize % (ulong)this.BytesPerBlock != 0UL)
				{
					throw new ImageStorageException(string.Format("The image size, specified by MinSectorCount, needs to be a multiple of {0} (0x{0:x}) sectors.", this.BytesPerBlock / fullFlashUpdateStore2.SectorSize));
				}
				for (int k = 0; k < fullFlashUpdateStore2.Partitions.Count; k++)
				{
					foreach (FullFlashUpdateImage.FullFlashUpdatePartition fullFlashUpdatePartition in fullFlashUpdateStore2.Partitions)
					{
						if (fullFlashUpdatePartition.ByteAlignment > 0U && fullFlashUpdatePartition.ByteAlignment < ImageConstants.PAYLOAD_BLOCK_SIZE)
						{
							fullFlashUpdatePartition.ByteAlignment = ImageConstants.PAYLOAD_BLOCK_SIZE;
						}
					}
				}
				this.CreateVirtualHardDisk(fullFlashUpdateStore2, null, image.ImageStyle, true, array[j], array);
			}
			int num2 = Environment.TickCount - tickCount;
			this._logger.LogInfo("Storage Service: Created a new image in {0:F1} seconds.", new object[]
			{
				(double)num2 / 1000.0
			});
			this._image = image;
		}

		// Token: 0x06000364 RID: 868 RVA: 0x0000FE0C File Offset: 0x0000E00C
		public uint MountFullFlashImage(FullFlashUpdateImage image, bool randomizeGptIds)
		{
			if (this._image != null)
			{
				bool saveChanges = false;
				this.DismountFullFlashImage(saveChanges);
			}
			uint result = 1U;
			using (FileStream imageStream = image.GetImageStream())
			{
				PayloadReader payloadReader = new PayloadReader(imageStream);
				if (payloadReader.Payloads.Count<StorePayload>() != image.StoreCount)
				{
					throw new ImageStorageException("Store counts in metadata and store header do not match");
				}
				for (int i = 0; i < image.StoreCount; i++)
				{
					FullFlashUpdateImage.FullFlashUpdateStore fullFlashUpdateStore = image.Stores[i];
					StorePayload storePayload = payloadReader.Payloads[i];
					payloadReader.ValidatePayloadPartitions((int)fullFlashUpdateStore.SectorSize, (long)((ulong)fullFlashUpdateStore.SectorCount * (ulong)fullFlashUpdateStore.SectorSize), storePayload, image.ImageStyle, fullFlashUpdateStore.IsMainOSStore, this._logger);
					ImageStorage imageStorage = new ImageStorage(this._logger, this);
					imageStorage.VirtualHardDiskSectorSize = this.VirtualHardDiskSectorSize;
					imageStorage.MountFullFlashImageStore(fullFlashUpdateStore, payloadReader, storePayload, randomizeGptIds);
					this._storages.Add(fullFlashUpdateStore, imageStorage);
					result = (uint)storePayload.StoreHeader.MajorVersion;
				}
			}
			this._image = image;
			return result;
		}

		// Token: 0x06000365 RID: 869 RVA: 0x0000FF2C File Offset: 0x0000E12C
		public void DismountFullFlashImage(bool saveChanges)
		{
			OutputWrapper outputWrapper = null;
			if (this._image == null || this._storages.Count == 0)
			{
				return;
			}
			try
			{
				outputWrapper = new OutputWrapper(this._image.Stores[0].BackingFile);
				this.DismountFullFlashImage(saveChanges, outputWrapper, true, 1U);
			}
			finally
			{
				if (outputWrapper != null)
				{
					outputWrapper.FinalizeWrapper();
				}
			}
		}

		// Token: 0x06000366 RID: 870 RVA: 0x0000FF94 File Offset: 0x0000E194
		public void DismountFullFlashImage(bool saveChanges, IPayloadWrapper payloadWrapper)
		{
			this.DismountFullFlashImage(saveChanges, payloadWrapper, true);
		}

		// Token: 0x06000367 RID: 871 RVA: 0x0000FF9F File Offset: 0x0000E19F
		public void DismountFullFlashImage(bool saveChanges, IPayloadWrapper payloadWrapper, bool deleteFile)
		{
			this.DismountFullFlashImage(saveChanges, payloadWrapper, deleteFile, 1U);
		}

		// Token: 0x06000368 RID: 872 RVA: 0x0000FFAC File Offset: 0x0000E1AC
		public void DismountFullFlashImage(bool saveChanges, IPayloadWrapper payloadWrapper, bool deleteFile, uint storeHeaderVersion)
		{
			int tickCount = Environment.TickCount;
			if (this._image == null && saveChanges)
			{
				throw new ImageStorageException(string.Format("{0}: Cannot save changes because the full flash update image is null.", MethodBase.GetCurrentMethod().Name));
			}
			if (this._storages.Keys.Count((FullFlashUpdateImage.FullFlashUpdateStore s) => s.IsMainOSStore) != 1)
			{
				throw new ImageStorageException(string.Format("{0}: One and only one storage can be the MainOS storage.", MethodBase.GetCurrentMethod().Name));
			}
			foreach (ImageStorage imageStorage in this._storages.Values)
			{
				if (imageStorage.SafeStoreHandle == null)
				{
					this._logger.DebugLogger("{0}: This function was called when no image is mounted.", new object[]
					{
						MethodBase.GetCurrentMethod().Name
					});
					return;
				}
				if (imageStorage.SafeStoreHandle.IsInvalid)
				{
					throw new ImageStorageException(string.Format("{0}: This function was called without a mounted image.", MethodBase.GetCurrentMethod().Name));
				}
				if (imageStorage.Image == null && saveChanges)
				{
					throw new ImageStorageException(string.Format("{0}: Cannot save changes because the full flash update image is null.", MethodBase.GetCurrentMethod().Name));
				}
			}
			if (saveChanges)
			{
				foreach (ImageStorage imageStorage2 in this._storages.Values)
				{
					if (this._image.ImageStyle == ImageConstants.PartitionTypeMbr)
					{
						this._logger.LogInfo("{0}:[{1}] Updating the BCD to fix partition offsets.", new object[]
						{
							MethodBase.GetCurrentMethod().Name,
							(double)(Environment.TickCount - tickCount) / 1000.0
						});
						imageStorage2.UpdateBootConfigurationDatabase(ImageConstants.EFI_BCD_FILE_PATH, ImageConstants.SYSTEM_STORE_SIGNATURE);
					}
					if (imageStorage2.IsMainOSStorage)
					{
						try
						{
							string partitionPath = this.GetPartitionPath("CrashDump");
							if (!string.IsNullOrEmpty(partitionPath))
							{
								string partitionFileSystem = this.GetPartitionFileSystem("CrashDump");
								if (string.Compare("NTFS", partitionFileSystem, true, CultureInfo.InvariantCulture) != 0)
								{
									using (FileStream fileStream = File.Create(Path.Combine(partitionPath, "readme.txt")))
									{
										StreamWriter streamWriter = new StreamWriter(fileStream);
										streamWriter.WriteLine("This is a workaround for bug #48031. Please use NTFS file system for CrashDump partition to avoid this file.");
										streamWriter.Flush();
									}
								}
							}
						}
						catch (Exception)
						{
						}
						if (!this.IsDesktopImage)
						{
							this._logger.LogInfo("{0}:[{1}] Enabling USN journal on partition {2}.", new object[]
							{
								MethodBase.GetCurrentMethod().Name,
								(double)(Environment.TickCount - tickCount) / 1000.0,
								ImageConstants.MAINOS_PARTITION_NAME
							});
							this.CreateUsnJournal(ImageConstants.MAINOS_PARTITION_NAME);
							this._logger.LogInfo("{0}:[{1}] Enabling USN journal on partition {2}.", new object[]
							{
								MethodBase.GetCurrentMethod().Name,
								(double)(Environment.TickCount - tickCount) / 1000.0,
								ImageConstants.DATA_PARTITION_NAME
							});
							this.CreateUsnJournal(ImageConstants.DATA_PARTITION_NAME);
						}
					}
					if (imageStorage2.IsMainOSStorage)
					{
						NativeImaging.WriteMountManagerRegistry2(imageStorage2.ServiceHandle, imageStorage2.StoreId, true);
						NativeImaging.NormalizeVolumeMountPoints(imageStorage2.ServiceHandle, imageStorage2.StoreId, this.GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME));
					}
				}
				this._logger.LogInfo("{0}:[{1}] Flushing all volumes.", new object[]
				{
					MethodBase.GetCurrentMethod().Name,
					(double)(Environment.TickCount - tickCount) / 1000.0
				});
				this.FlushVolumesForDismount();
				using (VirtualDiskPayloadManager virtualDiskPayloadManager = new VirtualDiskPayloadManager(this._logger, (ushort)storeHeaderVersion, (ushort)this._storages.Count<KeyValuePair<FullFlashUpdateImage.FullFlashUpdateStore, ImageStorage>>()))
				{
					foreach (ImageStorage storage in this._storages.Values)
					{
						virtualDiskPayloadManager.AddStore(storage);
					}
					virtualDiskPayloadManager.Write(payloadWrapper);
				}
				using (Dictionary<FullFlashUpdateImage.FullFlashUpdateStore, ImageStorage>.ValueCollection.Enumerator enumerator = this._storages.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ImageStorage imageStorage3 = enumerator.Current;
						imageStorage3.SafeStoreHandle.Close();
						this._logger.LogInfo("{0}:[{1}] Final VHD dismount.", new object[]
						{
							MethodBase.GetCurrentMethod().Name,
							(double)(Environment.TickCount - tickCount) / 1000.0
						});
						NativeImaging.DismountVirtualHardDisk(imageStorage3.ServiceHandle, imageStorage3.StoreId, true, deleteFile, false);
						this._logger.LogInfo("{0}:[{1}] Cleaning up temporary paths.", new object[]
						{
							MethodBase.GetCurrentMethod().Name,
							(double)(Environment.TickCount - tickCount) / 1000.0
						});
						imageStorage3.Cleanup();
					}
					goto IL_58A;
				}
			}
			foreach (ImageStorage imageStorage4 in this._storages.Values)
			{
				if (imageStorage4.SafeStoreHandle != null)
				{
					imageStorage4.SafeStoreHandle.Close();
				}
				NativeImaging.DismountVirtualHardDiskByName(imageStorage4.ServiceHandle, imageStorage4.VirtualDiskFilePath, deleteFile);
				this._logger.LogInfo("{0}:[{1}] Cleaning up temporary paths.", new object[]
				{
					MethodBase.GetCurrentMethod().Name,
					(double)(Environment.TickCount - tickCount) / 1000.0
				});
				imageStorage4.Cleanup();
			}
			IL_58A:
			int num = Environment.TickCount - tickCount;
			this._logger.LogInfo("Storage Service: Dismounting the image in {0:F1} seconds.", new object[]
			{
				(double)num / 1000.0
			});
			this._image = null;
			this._storages.Clear();
		}

		// Token: 0x06000369 RID: 873 RVA: 0x0001064C File Offset: 0x0000E84C
		public string CreateVirtualHardDisk(FullFlashUpdateImage.FullFlashUpdateStore store, string imagePath, uint partitionStyle, bool preparePartitions)
		{
			ImageStructures.STORE_ID store_ID = default(ImageStructures.STORE_ID);
			store_ID.StoreType = partitionStyle;
			if (this.RandomizeDiskIds)
			{
				store_ID.StoreId_GPT = Guid.NewGuid();
			}
			else if (partitionStyle == ImageConstants.PartitionTypeGpt)
			{
				store_ID.StoreId_GPT = (store.IsMainOSStore ? ImageConstants.SYSTEM_STORE_GUID : Guid.Parse(store.Id));
			}
			else
			{
				store_ID.StoreId_MBR = (store.IsMainOSStore ? ImageConstants.SYSTEM_STORE_SIGNATURE : Convert.ToUInt32(store.Id));
			}
			return this.CreateVirtualHardDisk(store, imagePath, partitionStyle, preparePartitions, store_ID, new ImageStructures.STORE_ID[]
			{
				store_ID
			});
		}

		// Token: 0x0600036A RID: 874 RVA: 0x000106E8 File Offset: 0x0000E8E8
		public string CreateVirtualHardDisk(FullFlashUpdateImage.FullFlashUpdateStore store, string imagePath, uint partitionStyle, bool preparePartitions, ImageStructures.STORE_ID storeId, ImageStructures.STORE_ID[] storeIds)
		{
			if (this._image != null)
			{
				this.DismountFullFlashImage(false);
			}
			ImageStorage imageStorage = new ImageStorage(this._logger, this, storeId);
			imageStorage.VirtualHardDiskSectorSize = this.VirtualHardDiskSectorSize;
			imageStorage.CreateVirtualHardDiskFromStore(store, imagePath, partitionStyle, preparePartitions, storeIds);
			this._storages.Add(store, imageStorage);
			return imageStorage.VirtualDiskFilePath;
		}

		// Token: 0x0600036B RID: 875 RVA: 0x00010740 File Offset: 0x0000E940
		public void MountExistingVirtualHardDisk(string imagePath, bool readOnly)
		{
			ImageStorage imageStorage = new ImageStorage(this._logger, this);
			imageStorage.VirtualHardDiskSectorSize = this.VirtualHardDiskSectorSize;
			imageStorage.MountExistingVirtualHardDisk(imagePath, readOnly);
			this.CreateFullFlashObjectFromAttachedImage(imageStorage);
		}

		// Token: 0x0600036C RID: 876 RVA: 0x00010776 File Offset: 0x0000E976
		public ImageStorage MountDesktopVirtualHardDisk(string imagePath, bool readOnly)
		{
			ImageStorage imageStorage = new ImageStorage(this._logger, this);
			imageStorage.VirtualHardDiskSectorSize = this.VirtualHardDiskSectorSize;
			imageStorage.MountExistingVirtualHardDisk(imagePath, readOnly);
			return imageStorage;
		}

		// Token: 0x0600036D RID: 877 RVA: 0x00010798 File Offset: 0x0000E998
		public void DismountVirtualHardDisk()
		{
			this.DismountVirtualHardDisk(false, false, false);
		}

		// Token: 0x0600036E RID: 878 RVA: 0x000107A3 File Offset: 0x0000E9A3
		public void DismountVirtualHardDisk(bool skipPostProcessing, bool deleteFile)
		{
			this.DismountVirtualHardDisk(skipPostProcessing, deleteFile, false);
		}

		// Token: 0x0600036F RID: 879 RVA: 0x000107B0 File Offset: 0x0000E9B0
		public void DismountVirtualHardDisk(bool skipPostProcessing, bool deleteFile, bool normalizeDiskSignature)
		{
			foreach (ImageStorage imageStorage in this._storages.Values)
			{
				imageStorage.DismountVirtualHardDisk(skipPostProcessing, normalizeDiskSignature);
				if (deleteFile)
				{
					LongPathFile.Delete(imageStorage.VirtualDiskFilePath);
				}
			}
			this._storages.Clear();
		}

		// Token: 0x06000370 RID: 880 RVA: 0x00010824 File Offset: 0x0000EA24
		public FullFlashUpdateImage CreateFullFlashObjectFromAttachedImage(ImageStorage storage)
		{
			return this.CreateFullFlashObjectFromAttachedImage(new List<ImageStorage>
			{
				storage
			});
		}

		// Token: 0x06000371 RID: 881 RVA: 0x00010848 File Offset: 0x0000EA48
		public FullFlashUpdateImage CreateFullFlashObjectFromAttachedImage(List<ImageStorage> storages)
		{
			string deviceLayoutPath = null;
			string platformInfoPath = null;
			try
			{
				ImageStorage imageStorage = storages.Single((ImageStorage s) => s.IsMainOSStorage);
				deviceLayoutPath = Path.Combine(imageStorage.GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME), DevicePaths.DeviceLayoutFilePath);
				platformInfoPath = Path.Combine(imageStorage.GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME), DevicePaths.OemDevicePlatformFilePath);
			}
			catch (Exception)
			{
				throw new ImageStorageException("Unable to find MainOS store or there are more than one.");
			}
			return this.CreateFullFlashObjectFromAttachedImage(storages, deviceLayoutPath, platformInfoPath);
		}

		// Token: 0x06000372 RID: 882 RVA: 0x000108D0 File Offset: 0x0000EAD0
		public FullFlashUpdateImage CreateFullFlashObjectFromAttachedImage(List<ImageStorage> storages, string deviceLayoutPath, string platformInfoPath)
		{
			ImageGenerator imageGenerator = new ImageGenerator();
			ImageGeneratorParameters imageGeneratorParameters = new ImageGeneratorParameters();
			try
			{
				imageGeneratorParameters.Initialize(this._logger);
				imageGeneratorParameters.ProcessInputXML(deviceLayoutPath, platformInfoPath);
				for (int i = 0; i < imageGeneratorParameters.Stores.Count; i++)
				{
					InputStore inputStore = imageGeneratorParameters.Stores[i];
					ImageStorage imageStorage = storages[i];
					imageStorage.VirtualHardDiskSectorSize = imageGeneratorParameters.VirtualHardDiskSectorSize;
					foreach (InputPartition inputPartition in inputStore.Partitions)
					{
						if (inputPartition.MinFreeSectors != 0U)
						{
							inputPartition.TotalSectors = (uint)imageStorage.GetPartitionSize(inputPartition.Name);
						}
					}
				}
				imageGenerator.Initialize(imageGeneratorParameters, this._logger, this.IsDesktopImage);
				FullFlashUpdateImage fullFlashUpdateImage = imageGenerator.CreateFFU();
				if (storages.Count != fullFlashUpdateImage.StoreCount)
				{
					throw new ImageStorageException("Number of ImageStorage objects and stores in device layout do not match");
				}
				for (int k = 0; k < storages.Count; k++)
				{
					storages[k].SetFullFlashUpdateStore(fullFlashUpdateImage.Stores[k]);
					this._storages.Add(fullFlashUpdateImage.Stores[k], storages[k]);
				}
				this._image = fullFlashUpdateImage;
			}
			catch (Exception innerException)
			{
				throw new ImageStorageException("Unable to create a FullFlashImage object.", innerException);
			}
			return this._image;
		}

		// Token: 0x06000373 RID: 883 RVA: 0x00010A34 File Offset: 0x0000EC34
		public ImageStorage AttachToMountedVirtualHardDisk(string physicalDiskPath, bool readOnly)
		{
			return this.AttachToMountedVirtualHardDisk(physicalDiskPath, readOnly, true);
		}

		// Token: 0x06000374 RID: 884 RVA: 0x00010A3F File Offset: 0x0000EC3F
		public ImageStorage AttachToMountedVirtualHardDisk(string physicalDiskPath, bool readOnly, bool isMainOSStore)
		{
			ImageStorage imageStorage = new ImageStorage(this._logger, this);
			imageStorage.VirtualHardDiskSectorSize = this.VirtualHardDiskSectorSize;
			imageStorage.AttachToMountedVirtualHardDisk(physicalDiskPath, readOnly, isMainOSStore);
			return imageStorage;
		}

		// Token: 0x06000375 RID: 885 RVA: 0x00010A64 File Offset: 0x0000EC64
		public void DetachVirtualHardDisk(bool deleteFile)
		{
			foreach (ImageStorage imageStorage in this._storages.Values)
			{
				imageStorage.DetachVirtualHardDisk(deleteFile);
			}
			this._storages.Clear();
		}

		// Token: 0x06000376 RID: 886 RVA: 0x00010AC8 File Offset: 0x0000ECC8
		public ImageStorage GetImageStorage(FullFlashUpdateImage.FullFlashUpdateStore store)
		{
			return this._storages[store];
		}

		// Token: 0x06000377 RID: 887 RVA: 0x00010AD8 File Offset: 0x0000ECD8
		public bool IsPartitionTargeted(string partition)
		{
			return this._partitionsTargeted == null || this._partitionsTargeted.Any((string p) => string.Compare(partition, p, true, CultureInfo.InvariantCulture) == 0);
		}

		// Token: 0x06000378 RID: 888 RVA: 0x00010B14 File Offset: 0x0000ED14
		private static void CheckForDuplicateNames(FullFlashUpdateImage image)
		{
			List<string> list = new List<string>();
			foreach (FullFlashUpdateImage.FullFlashUpdateStore fullFlashUpdateStore in image.Stores)
			{
				foreach (FullFlashUpdateImage.FullFlashUpdatePartition fullFlashUpdatePartition in fullFlashUpdateStore.Partitions)
				{
					if (list.Contains(fullFlashUpdatePartition.Name))
					{
						throw new ImageStorageException(string.Format("Partition {0} is included more than once.", fullFlashUpdatePartition.Name));
					}
					list.Add(fullFlashUpdatePartition.Name);
				}
			}
			list = null;
		}

		// Token: 0x06000379 RID: 889 RVA: 0x00010BD4 File Offset: 0x0000EDD4
		private static void ValidateMainOsInImage(FullFlashUpdateImage image)
		{
			bool flag = false;
			foreach (FullFlashUpdateImage.FullFlashUpdatePartition fullFlashUpdatePartition in image.Stores.Single((FullFlashUpdateImage.FullFlashUpdateStore s) => s.IsMainOSStore).Partitions)
			{
				if (string.Compare(fullFlashUpdatePartition.Name, ImageConstants.MAINOS_PARTITION_NAME, true, CultureInfo.InvariantCulture) == 0)
				{
					if (string.IsNullOrEmpty(fullFlashUpdatePartition.FileSystem))
					{
						throw new ImageStorageException(string.Format("{0}: Partition '{1}' must have a valid file system.", MethodBase.GetCurrentMethod().Name, ImageConstants.MAINOS_PARTITION_NAME));
					}
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				throw new ImageStorageException(string.Format("{0}: The full flash update image must contain a partition '{1}'.", MethodBase.GetCurrentMethod().Name, ImageConstants.MAINOS_PARTITION_NAME));
			}
		}

		// Token: 0x0600037A RID: 890 RVA: 0x00010CB8 File Offset: 0x0000EEB8
		public Guid GetPartitionTypeGpt(string partitionName)
		{
			return this.GetImageStorageByPartitionName(partitionName).GetPartitionTypeGpt(partitionName);
		}

		// Token: 0x0600037B RID: 891 RVA: 0x00010CC7 File Offset: 0x0000EEC7
		public byte GetPartitionTypeMbr(string partitionName)
		{
			return this.GetImageStorageByPartitionName(partitionName).GetPartitionTypeMbr(partitionName);
		}

		// Token: 0x0600037C RID: 892 RVA: 0x00010CD6 File Offset: 0x0000EED6
		public string GetPartitionPath(string partitionName)
		{
			return this.GetImageStorageByPartitionName(partitionName).GetPartitionPath(partitionName);
		}

		// Token: 0x0600037D RID: 893 RVA: 0x00010CE5 File Offset: 0x0000EEE5
		public ulong GetPartitionSize(string partitionName)
		{
			return this.GetImageStorageByPartitionName(partitionName).GetPartitionSize(partitionName);
		}

		// Token: 0x0600037E RID: 894 RVA: 0x00010CF4 File Offset: 0x0000EEF4
		public void SetPartitionType(string partitionName, Guid partitionType)
		{
			this.GetImageStorageByPartitionName(partitionName).SetPartitionType(partitionName, partitionType);
		}

		// Token: 0x0600037F RID: 895 RVA: 0x00010D04 File Offset: 0x0000EF04
		public void SetPartitionType(string partitionName, byte partitionType)
		{
			this.GetImageStorageByPartitionName(partitionName).SetPartitionType(partitionName, partitionType);
		}

		// Token: 0x06000380 RID: 896 RVA: 0x00010D14 File Offset: 0x0000EF14
		public string GetPartitionFileSystem(string partitionName)
		{
			return this.GetImageStorageByPartitionName(partitionName).GetPartitionFileSystem(partitionName);
		}

		// Token: 0x06000381 RID: 897 RVA: 0x00010D23 File Offset: 0x0000EF23
		public bool PartitionIsMountedRaw(string partitionName)
		{
			return this.GetImageStorageByPartitionName(partitionName).PartitionIsMountedRaw(partitionName);
		}

		// Token: 0x06000382 RID: 898 RVA: 0x00010D32 File Offset: 0x0000EF32
		public void FormatPartition(string partitionName, string fileSsytem, uint cbClusterSize)
		{
			this.GetImageStorageByPartitionName(partitionName).FormatPartition(partitionName, fileSsytem, cbClusterSize);
		}

		// Token: 0x06000383 RID: 899 RVA: 0x00010D43 File Offset: 0x0000EF43
		public SafeFileHandle OpenVolumeHandle(string partitionName)
		{
			return this.GetImageStorageByPartitionName(partitionName).OpenVolumeHandle(partitionName);
		}

		// Token: 0x06000384 RID: 900 RVA: 0x00010D52 File Offset: 0x0000EF52
		public void WaitForVolume(string volumeName)
		{
			this.GetImageStorageByPartitionName(volumeName).WaitForVolume(volumeName);
		}

		// Token: 0x06000385 RID: 901 RVA: 0x00010D64 File Offset: 0x0000EF64
		public void FlushVolumesForDismount()
		{
			foreach (FullFlashUpdateImage.FullFlashUpdateStore fullFlashUpdateStore in this._storages.Keys)
			{
				ImageStorage imageStorage = this._storages[fullFlashUpdateStore];
				foreach (FullFlashUpdateImage.FullFlashUpdatePartition fullFlashUpdatePartition in fullFlashUpdateStore.Partitions)
				{
					imageStorage.WaitForVolume(fullFlashUpdatePartition.Name);
					if (!imageStorage.PartitionIsMountedRaw(fullFlashUpdatePartition.Name))
					{
						using (SafeVolumeHandle safeVolumeHandle = new SafeVolumeHandle(imageStorage, fullFlashUpdatePartition.Name))
						{
							Win32Exports.FlushFileBuffers(safeVolumeHandle.VolumeHandle);
						}
					}
				}
			}
		}

		// Token: 0x06000386 RID: 902 RVA: 0x00010E58 File Offset: 0x0000F058
		public ulong GetFreeBytesOnVolume(string partitionName)
		{
			return this.GetImageStorageByPartitionName(partitionName).GetFreeBytesOnVolume(partitionName);
		}

		// Token: 0x06000387 RID: 903 RVA: 0x00010E67 File Offset: 0x0000F067
		public void CreateJunction(string sourceName, string targetPartition, string targetPath)
		{
			this.CreateJunction(sourceName, targetPartition, targetPath, false);
		}

		// Token: 0x06000388 RID: 904 RVA: 0x00010E73 File Offset: 0x0000F073
		public void CreateJunction(string sourceName, string targetPartition, string targetPath, bool useWellKnownGuids)
		{
			this.GetImageStorageByPartitionName(targetPartition).CreateJunction(sourceName, targetPartition, targetPath, useWellKnownGuids);
		}

		// Token: 0x06000389 RID: 905 RVA: 0x00010E86 File Offset: 0x0000F086
		public void CreateUsnJournal(string partitionName)
		{
			this.GetImageStorageByPartitionName(partitionName).CreateUsnJournal(partitionName);
		}

		// Token: 0x0600038A RID: 906 RVA: 0x00010E95 File Offset: 0x0000F095
		public void AttachWOFToVolume(string partitionName)
		{
			this.GetImageStorageByPartitionName(partitionName).AttachWOFToVolume(partitionName);
		}

		// Token: 0x0600038B RID: 907 RVA: 0x00010EA4 File Offset: 0x0000F0A4
		public void LockAndDismountVolume(string partitionName)
		{
			this.LockAndDismountVolume(partitionName, false);
		}

		// Token: 0x0600038C RID: 908 RVA: 0x00010EAE File Offset: 0x0000F0AE
		public void LockAndDismountVolume(string partitionName, bool forceDismount)
		{
			this.GetImageStorageByPartitionName(partitionName).LockAndDismountVolume(partitionName, forceDismount);
		}

		// Token: 0x0600038D RID: 909 RVA: 0x00010EBE File Offset: 0x0000F0BE
		public void UnlockVolume(string partitionName)
		{
			this.GetImageStorageByPartitionName(partitionName).UnlockVolume(partitionName);
		}

		// Token: 0x0600038E RID: 910 RVA: 0x00010ED0 File Offset: 0x0000F0D0
		private ImageStorage GetImageStorageByPartitionName(string partitionName)
		{
			Predicate<FullFlashUpdateImage.FullFlashUpdatePartition> <>9__1;
			FullFlashUpdateImage.FullFlashUpdateStore key = this._storages.Keys.Single(delegate(FullFlashUpdateImage.FullFlashUpdateStore s)
			{
				List<FullFlashUpdateImage.FullFlashUpdatePartition> partitions = s.Partitions;
				Predicate<FullFlashUpdateImage.FullFlashUpdatePartition> match;
				if ((match = <>9__1) == null)
				{
					match = (<>9__1 = ((FullFlashUpdateImage.FullFlashUpdatePartition p) => string.Compare(partitionName, p.Name, true, CultureInfo.InvariantCulture) == 0));
				}
				return partitions.Exists(match);
			});
			return this._storages[key];
		}

		// Token: 0x0600038F RID: 911 RVA: 0x00010F14 File Offset: 0x0000F114
		private void MountManagerScrubRegistry()
		{
			using (SafeFileHandle safeFileHandle = Win32Exports.CreateFile(Win32Exports.MountManagerPath, (Win32Exports.DesiredAccess)3221225472U, Win32Exports.ShareMode.FILE_SHARE_READ | Win32Exports.ShareMode.FILE_SHARE_WRITE, Win32Exports.CreationDisposition.OPEN_EXISTING, Win32Exports.FlagsAndAttributes.FILE_ATTRIBUTE_NORMAL))
			{
				int num = 0;
				Win32Exports.DeviceIoControl(safeFileHandle.DangerousGetHandle(), 7192632U, null, 0, null, 0, out num);
			}
		}

		// Token: 0x040001C1 RID: 449
		private IULogger _logger;

		// Token: 0x040001C2 RID: 450
		private FullFlashUpdateImage _image;

		// Token: 0x040001C3 RID: 451
		private Dictionary<FullFlashUpdateImage.FullFlashUpdateStore, ImageStorage> _storages;

		// Token: 0x040001C4 RID: 452
		private uint _virtualHardDiskSectorSize;

		// Token: 0x040001C5 RID: 453
		private IList<string> _partitionsTargeted;
	}
}
