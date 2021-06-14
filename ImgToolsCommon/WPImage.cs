using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Images
{
	// Token: 0x02000005 RID: 5
	public class WPImage : IDisposable
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00004D28 File Offset: 0x00002F28
		public MobileCoreImage MCImage
		{
			get
			{
				return this._mcImage;
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00004D30 File Offset: 0x00002F30
		public WPImage(IULogger logger)
		{
			this._logger = logger;
			this._store = new WPStore(this);
			this._tempDirectoryPath = BuildPaths.GetImagingTempPath("");
			Directory.CreateDirectory(this._tempDirectoryPath);
			this._tempDirectoryPath = FileUtils.GetShortPathName(this._tempDirectoryPath);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00004D9C File Offset: 0x00002F9C
		~WPImage()
		{
			this.Dispose(false);
		}

		// Token: 0x06000031 RID: 49
		[DllImport("kernel32.dll")]
		private static extern bool DeleteVolumeMountPoint(string lpszVolumeMountPoint);

		// Token: 0x06000032 RID: 50
		[DllImport("kernel32.dll")]
		private static extern bool SetVolumeMountPoint(string lpszVolumeMountPoint, string lpszVolumeName);

		// Token: 0x06000033 RID: 51 RVA: 0x00004DCC File Offset: 0x00002FCC
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00004DDC File Offset: 0x00002FDC
		protected virtual void Dispose(bool isDisposing)
		{
			if (this._alreadyDisposed)
			{
				return;
			}
			if (this._store != null)
			{
				this._store.Dispose();
				this._store = null;
			}
			if (this._storageManager != null)
			{
				if (this._isFFU)
				{
					this._storageManager.DismountFullFlashImage(false);
				}
				else if (this._isVHD)
				{
					this._storageManager.DismountVirtualHardDisk(true, true);
				}
				this._storageManager = null;
			}
			foreach (WPPartition wppartition in this.Partitions)
			{
				wppartition.Dispose();
			}
			this._wpMetadata = null;
			if (!string.IsNullOrEmpty(this.MainOSMountPoint))
			{
				WPImage.DeleteVolumeMountPoint(this.MainOSMountPoint);
				this.MainOSMountPoint = null;
			}
			if (this._mcImage != null)
			{
				if (this._mcImage.IsMounted)
				{
					this._mcImage.Unmount();
				}
				this._mcImage = null;
			}
			if (!string.IsNullOrEmpty(this._tempDirectoryPath))
			{
				FileUtils.DeleteTree(this._tempDirectoryPath);
			}
			this._alreadyDisposed = true;
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00004EF8 File Offset: 0x000030F8
		public WPMetadata Metadata
		{
			get
			{
				return this._wpMetadata;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00004F00 File Offset: 0x00003100
		public bool IsFFU
		{
			get
			{
				return this._isFFU;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00004F08 File Offset: 0x00003108
		public bool IsVHD
		{
			get
			{
				return this._isVHD;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00004F10 File Offset: 0x00003110
		public bool IsWIM
		{
			get
			{
				return this._isWIM;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000039 RID: 57 RVA: 0x00004F18 File Offset: 0x00003118
		public string TempDirectoryPath
		{
			get
			{
				return this._tempDirectoryPath;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00004F20 File Offset: 0x00003120
		public int PartitionCount
		{
			get
			{
				return this._partitions.Count;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00004F2D File Offset: 0x0000312D
		public List<WPPartition> Partitions
		{
			get
			{
				return this._partitions;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00004F35 File Offset: 0x00003135
		public WPStore Store
		{
			get
			{
				return this._store;
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00004F3D File Offset: 0x0000313D
		public void LoadImage(string fileName)
		{
			this.LoadImage(fileName, true, false);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00004F48 File Offset: 0x00003148
		public void LoadImage(string fileName, bool readOnly, bool randomizeGptIds)
		{
			if (!File.Exists(fileName))
			{
				throw new ImagesException("Tools.ImgCommon!LoadImage: File could not be found: '" + fileName + "'.");
			}
			string a = Path.GetExtension(fileName).ToUpper(CultureInfo.InvariantCulture);
			if (a == ".FFU")
			{
				this._isFFU = true;
				this.LoadFFU(fileName, randomizeGptIds);
				return;
			}
			if (a == ".VHD")
			{
				this._isVHD = true;
				this.LoadVHD(fileName, readOnly);
				return;
			}
			if (!(a == ".WIM"))
			{
				throw new ImagesException("Tools.ImgCommon!LoadImage: Unrecognized file type '" + Path.GetExtension(fileName).ToUpper(CultureInfo.InvariantCulture) + "' not supported.");
			}
			this._isWIM = true;
			this.LoadWIM(fileName, readOnly);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00005004 File Offset: 0x00003204
		private void GetMainOSPath()
		{
			if (this._storageManager == null || !string.IsNullOrEmpty(this.MainOSPath))
			{
				return;
			}
			this.MainOSPath = this._storageManager.GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME);
			if (this.MainOSPath.StartsWith("\\\\.\\", StringComparison.OrdinalIgnoreCase) && WPImage.SetVolumeMountPoint(this.MainOSMountPoint, this.MainOSPath.Replace("\\\\.\\", "\\\\?\\", StringComparison.OrdinalIgnoreCase)))
			{
				this.MainOSPath = this.MainOSMountPoint;
			}
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00005080 File Offset: 0x00003280
		private void LoadFFU(string fileName, bool randomizeGptIds)
		{
			try
			{
				this._wpMetadata = new WPMetadata();
				this._wpMetadata.Initialize(fileName);
			}
			catch (Exception ex)
			{
				throw new ImagesException("Tools.ImgCommon!LoadImage: Failed to Initialize WPMetadata with '" + fileName + "': " + ex.Message, ex.InnerException);
			}
			this.DevicePlatformIDs = this._wpMetadata.DevicePlatformIDs.ToList<string>();
			this._store.Initialize(this._wpMetadata.Stores[0]);
			try
			{
				this._storageManager = new ImageStorageManager(new IULogger
				{
					ErrorLogger = null,
					DebugLogger = null,
					WarningLogger = null,
					InformationLogger = null
				});
				this._storageManager.MountFullFlashImage(this._wpMetadata, randomizeGptIds);
			}
			catch (Exception ex2)
			{
				throw new ImagesException("Tools.ImgCommon!LoadImage: Failed to Mount '" + fileName + "': " + ex2.Message, ex2.InnerException);
			}
			this.GetMainOSPath();
			try
			{
				foreach (FullFlashUpdateImage.FullFlashUpdatePartition partition in this._wpMetadata.Stores[0].Partitions)
				{
					this.AddPartition(partition);
				}
				this.FindBinaryPartitions();
				this.AddWIMs();
			}
			catch (Exception ex3)
			{
				throw new ImagesException("Tools.ImgCommon!LoadImage: Failed while adding partitions: " + ex3.Message, ex3.InnerException);
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00005214 File Offset: 0x00003414
		private void FindBinaryPartitions()
		{
			using (List<IPkgInfo>.Enumerator enumerator = this.Partitions.Find((WPPartition part) => part.Name.Equals(ImageConstants.MAINOS_PARTITION_NAME, StringComparison.OrdinalIgnoreCase)).Packages.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IPkgInfo pkg = enumerator.Current;
					if (pkg.IsBinaryPartition)
					{
						this.Partitions.Find((WPPartition part) => part.Name.Equals(pkg.Partition, StringComparison.OrdinalIgnoreCase)).IsBinaryPartition = true;
					}
				}
			}
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000052C0 File Offset: 0x000034C0
		private void LoadWIM(string fileName, bool readOnly)
		{
			MobileCoreImage mobileCoreImage = MobileCoreImage.Create(fileName);
			mobileCoreImage.Mount();
			this._mcImage = mobileCoreImage;
			this.PopulatePartitionsFromMCImage();
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000052E8 File Offset: 0x000034E8
		private void LoadVHD(string fileName, bool readOnly)
		{
			bool flag = false;
			try
			{
				this._storageManager = new ImageStorageManager(new IULogger
				{
					ErrorLogger = null,
					DebugLogger = null,
					WarningLogger = null,
					InformationLogger = null
				});
				this._storageManager.MountExistingVirtualHardDisk(fileName, readOnly);
				this.GetMainOSPath();
				flag = true;
			}
			catch
			{
				this._storageManager = null;
			}
			if (!flag)
			{
				MobileCoreImage mobileCoreImage = MobileCoreImage.Create(fileName);
				if (readOnly)
				{
					mobileCoreImage.MountReadOnly();
				}
				else
				{
					mobileCoreImage.Mount();
				}
				this._mcImage = mobileCoreImage;
			}
			try
			{
				this.PopulatePartitionsFromVHD();
				this.PopulateStoreFromVHD();
			}
			catch (Exception ex)
			{
				throw new ImagesException("Tools.ImgCommon!LoadImage: Failed while adding partitions: " + ex.Message, ex.InnerException);
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000053B0 File Offset: 0x000035B0
		private void AddPartition(FullFlashUpdateImage.FullFlashUpdatePartition partition)
		{
			WPPartition item = new WPPartition(this, partition, this._storageManager, this._logger);
			this._partitions.Add(item);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000053E0 File Offset: 0x000035E0
		private void AddWIM(string wimPath)
		{
			WPPartition item = new WPPartition(this, wimPath, this._storageManager, this._logger);
			this._partitions.Add(item);
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00005410 File Offset: 0x00003610
		private void AddPartition(InputPartition partition)
		{
			WPPartition item = new WPPartition(this, partition, this._storageManager, this._logger);
			this._partitions.Add(item);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00005440 File Offset: 0x00003640
		public static DeviceLayoutInputv2 GetDeviceLayoutv2(ImageStorageManager storageManager)
		{
			DeviceLayoutInputv2 result = new DeviceLayoutInputv2();
			string partitionPath = storageManager.GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME);
			if (string.IsNullOrEmpty(partitionPath))
			{
				return null;
			}
			string text = Path.Combine(partitionPath, DevicePaths.DeviceLayoutFilePath);
			if (!File.Exists(text))
			{
				return null;
			}
			XsdValidator xsdValidator = new XsdValidator();
			try
			{
				using (Stream deviceLayoutXSD = ImageGeneratorParameters.GetDeviceLayoutXSD(text))
				{
					xsdValidator.ValidateXsd(deviceLayoutXSD, text, storageManager.Logger);
				}
			}
			catch (XsdValidatorException)
			{
				return null;
			}
			TextReader textReader = new StreamReader(text);
			try
			{
				if (!ImageGeneratorParameters.IsDeviceLayoutV2(text))
				{
					return null;
				}
				result = (DeviceLayoutInputv2)new XmlSerializer(typeof(DeviceLayoutInputv2)).Deserialize(textReader);
			}
			catch
			{
				return null;
			}
			finally
			{
				textReader.Close();
				textReader = null;
			}
			return result;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00005538 File Offset: 0x00003738
		[CLSCompliant(false)]
		public static DeviceLayoutInput GetDeviceLayout(ImageStorageManager storageManager)
		{
			DeviceLayoutInput result = new DeviceLayoutInput();
			string partitionPath = storageManager.GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME);
			if (string.IsNullOrEmpty(partitionPath))
			{
				return null;
			}
			string text = Path.Combine(partitionPath, DevicePaths.DeviceLayoutFilePath);
			if (!File.Exists(text))
			{
				return null;
			}
			XsdValidator xsdValidator = new XsdValidator();
			try
			{
				using (Stream deviceLayoutXSD = ImageGeneratorParameters.GetDeviceLayoutXSD(text))
				{
					xsdValidator.ValidateXsd(deviceLayoutXSD, text, storageManager.Logger);
				}
			}
			catch (XsdValidatorException)
			{
				return null;
			}
			TextReader textReader = new StreamReader(text);
			try
			{
				if (ImageGeneratorParameters.IsDeviceLayoutV2(text))
				{
					return null;
				}
				result = (DeviceLayoutInput)new XmlSerializer(typeof(DeviceLayoutInput)).Deserialize(textReader);
			}
			catch
			{
				return null;
			}
			finally
			{
				textReader.Close();
				textReader = null;
			}
			return result;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000562C File Offset: 0x0000382C
		public void PopulatePartitionsFromMCImage()
		{
			if (this._mcImage == null)
			{
				return;
			}
			if (this._mcImage.Partitions.Count<ImagePartition>() > 0)
			{
				this._partitions = new List<WPPartition>();
			}
			foreach (ImagePartition partition in this._mcImage.Partitions)
			{
				WPPartition item = new WPPartition(this, partition, this._logger);
				this._partitions.Add(item);
			}
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000056B8 File Offset: 0x000038B8
		[CLSCompliant(false)]
		public void PopulatePartitionsFromVHD()
		{
			if (this._storageManager == null && this._mcImage != null)
			{
				this.PopulatePartitionsFromMCImage();
				return;
			}
			DeviceLayoutInput deviceLayoutInput = null;
			DeviceLayoutInputv2 deviceLayoutInputv = null;
			if (deviceLayoutInput == null && deviceLayoutInputv == null)
			{
				throw new ImagesException("Tools.ImgCommon!PopulatePartitionsFromVHD: Unable to find DeviceLayout file and thus unable to extract metadata from VHD.");
			}
			if (deviceLayoutInputv != null)
			{
				this.Store.SectorSize = deviceLayoutInputv.SectorSize;
				foreach (InputPartition partition in deviceLayoutInputv.MainOSStore.Partitions)
				{
					this.AddPartition(partition);
				}
			}
			else
			{
				this.Store.SectorSize = deviceLayoutInput.SectorSize;
				foreach (InputPartition partition2 in deviceLayoutInput.Partitions)
				{
					this.AddPartition(partition2);
				}
			}
			this.AddWIMs();
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00005768 File Offset: 0x00003968
		private void AddWIMs()
		{
			string text = Path.Combine(this._storageManager.GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME), DevicePaths.UpdateOSWIMFilePath);
			if (Directory.Exists(Path.GetDirectoryName(text)) && File.Exists(text))
			{
				this.AddWIM(text);
			}
			try
			{
				string text2 = Path.Combine(this._storageManager.GetPartitionPath(ImageConstants.MMOS_PARTITION_NAME), DevicePaths.MMOSWIMFilePath);
				if (File.Exists(text2))
				{
					this.AddWIM(text2);
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000057EC File Offset: 0x000039EC
		[CLSCompliant(false)]
		public void PopulateStoreFromVHD()
		{
			OEMDevicePlatformInput oemdevicePlatformInput = null;
			if (this._storageManager == null)
			{
				return;
			}
			string text = Path.Combine(this._storageManager.GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME), DevicePaths.OemDevicePlatformFilePath);
			if (!File.Exists(text))
			{
				throw new ImagesException("Tools.ImgCommon!PopulateStoreFromVHD: Unable to find OEM Device Platform file and thus unable to extract metadata from VHD.");
			}
			XsdValidator xsdValidator = new XsdValidator();
			try
			{
				using (Stream oemdevicePlatformXSD = ImageGeneratorParameters.GetOEMDevicePlatformXSD())
				{
					xsdValidator.ValidateXsd(oemdevicePlatformXSD, text, this._logger);
				}
			}
			catch (XsdValidatorException inner)
			{
				throw new ImagesException("Tools.ImgCommon!PopulateStoreFromVHD: Unable to validate OEM Device Platform XSD.", inner);
			}
			TextReader textReader = new StreamReader(text);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(OEMDevicePlatformInput));
			try
			{
				oemdevicePlatformInput = (OEMDevicePlatformInput)xmlSerializer.Deserialize(textReader);
			}
			catch (Exception inner2)
			{
				throw new ImagesException("Tools.ImgCommon!PopulateStoreFromVHD: Unable to parse OEM Device Platform XML.", inner2);
			}
			finally
			{
				textReader.Close();
				textReader = null;
			}
			this._store.Initialize(oemdevicePlatformInput.MinSectorCount, this._store.SectorSize);
			this.DevicePlatformIDs = oemdevicePlatformInput.DevicePlatformIDs.ToList<string>();
		}

		// Token: 0x04000018 RID: 24
		private ImageStorageManager _storageManager;

		// Token: 0x04000019 RID: 25
		private IULogger _logger;

		// Token: 0x0400001A RID: 26
		private List<WPPartition> _partitions = new List<WPPartition>();

		// Token: 0x0400001B RID: 27
		private WPStore _store;

		// Token: 0x0400001C RID: 28
		private bool _isFFU;

		// Token: 0x0400001D RID: 29
		private bool _isVHD;

		// Token: 0x0400001E RID: 30
		private bool _isWIM;

		// Token: 0x0400001F RID: 31
		public const string SystemVolumeInfo = "SYSTEM VOLUME INFORMATION";

		// Token: 0x04000020 RID: 32
		private MobileCoreImage _mcImage;

		// Token: 0x04000021 RID: 33
		private bool _alreadyDisposed;

		// Token: 0x04000022 RID: 34
		private WPMetadata _wpMetadata;

		// Token: 0x04000023 RID: 35
		private string _tempDirectoryPath = string.Empty;

		// Token: 0x04000024 RID: 36
		public List<string> DevicePlatformIDs;

		// Token: 0x04000025 RID: 37
		public string MainOSPath;

		// Token: 0x04000026 RID: 38
		public string MainOSMountPoint;

		// Token: 0x04000027 RID: 39
		public bool Win32Accessible;
	}
}
