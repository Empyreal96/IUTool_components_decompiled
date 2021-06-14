using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000026 RID: 38
	public class ImageGeneratorParameters
	{
		// Token: 0x06000188 RID: 392 RVA: 0x0000DB00 File Offset: 0x0000BD00
		public ImageGeneratorParameters()
		{
			this.Stores = new List<InputStore>();
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000DB30 File Offset: 0x0000BD30
		public void Initialize(IULogger logger)
		{
			if (logger == null)
			{
				this._logger = new IULogger();
				return;
			}
			this._logger = logger;
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x0600018A RID: 394 RVA: 0x0000DB48 File Offset: 0x0000BD48
		// (set) Token: 0x0600018B RID: 395 RVA: 0x0000DB50 File Offset: 0x0000BD50
		public string Description { get; set; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x0600018C RID: 396 RVA: 0x0000DB59 File Offset: 0x0000BD59
		public InputStore MainOSStore
		{
			get
			{
				return this.Stores.FirstOrDefault((InputStore x) => x.IsMainOSStore());
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x0600018D RID: 397 RVA: 0x0000DB85 File Offset: 0x0000BD85
		// (set) Token: 0x0600018E RID: 398 RVA: 0x0000DB8D File Offset: 0x0000BD8D
		[CLSCompliant(false)]
		public uint ChunkSize
		{
			get
			{
				return this._chunkSize;
			}
			set
			{
				this._chunkSize = ((value != 0U) ? value : 256U);
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x0600018F RID: 399 RVA: 0x0000DBA0 File Offset: 0x0000BDA0
		// (set) Token: 0x06000190 RID: 400 RVA: 0x0000DBA8 File Offset: 0x0000BDA8
		[CLSCompliant(false)]
		public uint DefaultPartitionByteAlignment { get; set; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000191 RID: 401 RVA: 0x0000DBB1 File Offset: 0x0000BDB1
		// (set) Token: 0x06000192 RID: 402 RVA: 0x0000DBB9 File Offset: 0x0000BDB9
		[CLSCompliant(false)]
		public uint VirtualHardDiskSectorSize { get; set; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000193 RID: 403 RVA: 0x0000DBC2 File Offset: 0x0000BDC2
		// (set) Token: 0x06000194 RID: 404 RVA: 0x0000DBCA File Offset: 0x0000BDCA
		[CLSCompliant(false)]
		public uint SectorSize { get; set; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000195 RID: 405 RVA: 0x0000DBD3 File Offset: 0x0000BDD3
		// (set) Token: 0x06000196 RID: 406 RVA: 0x0000DBDB File Offset: 0x0000BDDB
		[CLSCompliant(false)]
		public uint MinSectorCount { get; set; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000197 RID: 407 RVA: 0x0000DBE4 File Offset: 0x0000BDE4
		// (set) Token: 0x06000198 RID: 408 RVA: 0x0000DBEC File Offset: 0x0000BDEC
		[CLSCompliant(false)]
		public uint Algid
		{
			get
			{
				return this._algid;
			}
			set
			{
				this._algid = ((value != 0U) ? value : 32780U);
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000199 RID: 409 RVA: 0x0000DBFF File Offset: 0x0000BDFF
		public uint DeviceLayoutVersion
		{
			get
			{
				return this._deviceLayoutVersion;
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x0600019A RID: 410 RVA: 0x0000DC07 File Offset: 0x0000BE07
		// (set) Token: 0x0600019B RID: 411 RVA: 0x0000DC0F File Offset: 0x0000BE0F
		public InputRules Rules { get; set; }

		// Token: 0x0600019C RID: 412 RVA: 0x0000DC18 File Offset: 0x0000BE18
		private bool VerifyPartitionSizes()
		{
			uint num = 0U;
			if (this.Stores == null)
			{
				return true;
			}
			foreach (InputPartition inputPartition in this.MainOSStore.Partitions)
			{
				if (inputPartition.UseAllSpace)
				{
					num += 1U;
				}
				else
				{
					num += inputPartition.TotalSectors;
				}
			}
			if (num > this.MinSectorCount)
			{
				ulong num2 = (ulong)num * (ulong)this.SectorSize / 1024UL / 1024UL;
				ulong num3 = (ulong)this.MinSectorCount * (ulong)this.SectorSize / 1024UL / 1024UL;
				this._logger.LogError(string.Format("ImageCommon!ImageGeneratorParameters::VerifyPartitionSizes: The total sectors used by all the partitions ({0} sectors/{1} MB) is larger than the MinSectorCount ({2} sectors/{3} MB). This means the image would not flash to a device with only {4} sectors/{5} MB. Either remove image content, or increase MinSectorCount.", new object[]
				{
					num,
					num2,
					this.MinSectorCount,
					num3,
					this.MinSectorCount,
					num3
				}), new object[0]);
				return false;
			}
			return true;
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000DD14 File Offset: 0x0000BF14
		public bool VerifyInputParameters()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (4294967295U / this.ChunkSize < 1024U)
			{
				this._logger.LogError("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The chunk size is specified in Kilobytes and the total size must be under 4GB.", new object[0]);
				return false;
			}
			if (this.SectorSize < 512U)
			{
				this._logger.LogError("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The sector size must be at least 512 bytes: {0} bytes.", new object[]
				{
					this.SectorSize
				});
				return false;
			}
			if (!InputHelpers.IsPowerOfTwo(this.SectorSize))
			{
				this._logger.LogError("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The sector size must be a multiple of 2: {0} bytes.", new object[]
				{
					this.SectorSize
				});
				return false;
			}
			if (this.ChunkSize * 1024U < this.SectorSize)
			{
				this._logger.LogError("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The chunk size is specified in Kilobytes and the total size must be under larger the sector size: {0} bytes.", new object[]
				{
					this.SectorSize
				});
				return false;
			}
			if (this.ChunkSize * 1024U % this.SectorSize != 0U)
			{
				this._logger.LogError("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The chunk size is specified in Kilobytes and must be divisible by the sector size: {0}.", new object[]
				{
					this.SectorSize
				});
				return false;
			}
			if (this.DefaultPartitionByteAlignment != 0U && !InputHelpers.IsPowerOfTwo(this.DefaultPartitionByteAlignment))
			{
				this._logger.LogError("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The default partition byte alignment must be a multiple of 2: {0} bytes.", new object[]
				{
					this.DefaultPartitionByteAlignment
				});
				return false;
			}
			if (this.Stores == null || this.Stores.Count == 0)
			{
				this._logger.LogError("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: For Generating a FFU image, at least one store must be specified.", new object[0]);
				return false;
			}
			if (this.Stores.Count((InputStore x) => x.IsMainOSStore()) != 1)
			{
				this._logger.LogError("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: For Generating a FFU image, one and only one of the stores must be MainOS.", new object[0]);
				return false;
			}
			if (this.MainOSStore.Partitions == null || this.MainOSStore.Partitions.Count<InputPartition>() == 0)
			{
				this._logger.LogError("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: For Generating a FFU image, at least one partition must be specified.", new object[0]);
				return false;
			}
			if (this.SectorSize == 0U)
			{
				this._logger.LogError("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The SectorSize cannot be 0. Please provide a valid SectorSize.", new object[0]);
				return false;
			}
			if (this.ChunkSize == 0U)
			{
				this._logger.LogError("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The ChunkSize cannot be 0. Please provide a valid ChunkSize between 1-1024.", new object[0]);
				return false;
			}
			if (this.ChunkSize < 1U || this.ChunkSize > 1024U)
			{
				this._logger.LogError("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The ChunkSize must between 1-1024.", new object[0]);
				return false;
			}
			int num = 0;
			if (this.DevicePlatformIDs != null)
			{
				foreach (string text in this.DevicePlatformIDs)
				{
					num += text.Length + 1;
				}
			}
			if ((long)num > 191L)
			{
				this._logger.LogError("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: parameter DevicePlatformID larger than {0}.", new object[]
				{
					192U.ToString()
				});
				return false;
			}
			foreach (InputStore inputStore in this.Stores)
			{
				foreach (InputPartition inputPartition in inputStore.Partitions)
				{
					if (dictionary.ContainsKey(inputPartition.Name))
					{
						this._logger.LogError("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: A partition '" + inputPartition.Name + "' is defined twice in the DeviceLayout.", new object[0]);
						return false;
					}
					dictionary.Add(inputPartition.Name, "Partitions");
				}
			}
			InputPartition inputPartition2 = null;
			foreach (InputPartition inputPartition3 in this.MainOSStore.Partitions)
			{
				if (inputPartition2 != null)
				{
					this._logger.LogError("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: Partitions that specify UseAllSpace must be at the end.  See partition '{0}' and '{1}' for conflict.", new object[]
					{
						inputPartition2.Name,
						inputPartition3.Name
					});
					return false;
				}
				if (inputPartition3.UseAllSpace)
				{
					inputPartition2 = inputPartition3;
					if (inputPartition3.TotalSectors != 0U)
					{
						this._logger.LogError("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: A partition cannot use all available space and have total sectors set.  See partition " + inputPartition3.Name, new object[0]);
						return false;
					}
				}
				if (inputPartition3.ByteAlignment != 0U)
				{
					if (inputPartition3.SingleSectorAlignment && inputPartition3.ByteAlignment != this.SectorSize)
					{
						this._logger.LogError("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: Partition '{0}' has both a byte alignment and SingleSectorAlignment set.", new object[]
						{
							inputPartition3.Name
						});
						return false;
					}
					if (!InputHelpers.IsPowerOfTwo(inputPartition3.ByteAlignment))
					{
						this._logger.LogError("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The byte alignment for partition '{0}' must be a multiple of 2: {1} bytes.", new object[]
						{
							inputPartition3.Name,
							inputPartition3.ByteAlignment
						});
						return false;
					}
				}
				if (inputPartition3.SingleSectorAlignment)
				{
					inputPartition3.ByteAlignment = this.SectorSize;
				}
				if (!string.IsNullOrEmpty(inputPartition3.PrimaryPartition) && this.FindPartition(inputPartition3.PrimaryPartition) == null)
				{
					this._logger.LogError(string.Format("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The primary partition for partition '{0}' is not found Primary: '{1}'.", inputPartition3.Name, inputPartition3.PrimaryPartition), new object[0]);
					return false;
				}
			}
			return this.VerifyPartitionSizes();
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000E21C File Offset: 0x0000C41C
		private InputPartition FindPartition(string PartitionName)
		{
			ImageGeneratorParameters.<>c__DisplayClass54_0 CS$<>8__locals1 = new ImageGeneratorParameters.<>c__DisplayClass54_0();
			CS$<>8__locals1.PartitionName = PartitionName;
			foreach (InputStore inputStore in this.Stores)
			{
				IEnumerable<InputPartition> partitions = inputStore.Partitions;
				Func<InputPartition, bool> predicate;
				if ((predicate = CS$<>8__locals1.<>9__0) == null)
				{
					ImageGeneratorParameters.<>c__DisplayClass54_0 CS$<>8__locals2 = CS$<>8__locals1;
					predicate = (CS$<>8__locals2.<>9__0 = ((InputPartition x) => x.Name.Equals(CS$<>8__locals2.PartitionName, StringComparison.OrdinalIgnoreCase)));
				}
				IEnumerable<InputPartition> source = partitions.Where(predicate);
				if (source.ToArray<InputPartition>().Length != 0)
				{
					return source.First<InputPartition>();
				}
			}
			return null;
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000E2B8 File Offset: 0x0000C4B8
		public static bool IsDeviceLayoutV2(string DeviceLayoutXMLFile)
		{
			XPathNavigator xpathNavigator = new XPathDocument(DeviceLayoutXMLFile).CreateNavigator();
			xpathNavigator.MoveToFollowing(XPathNodeType.Element);
			return xpathNavigator.GetNamespacesInScope(XmlNamespaceScope.All).Values.Any((string x) => string.CompareOrdinal(x, "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate/v2") == 0);
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000E307 File Offset: 0x0000C507
		public static Stream GetDeviceLayoutXSD(string deviceLayoutXMLFile)
		{
			if (ImageGeneratorParameters.IsDeviceLayoutV2(deviceLayoutXMLFile))
			{
				return ImageGeneratorParameters.GetXSDStream(DevicePaths.DeviceLayoutSchema2);
			}
			return ImageGeneratorParameters.GetXSDStream(DevicePaths.DeviceLayoutSchema);
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000E326 File Offset: 0x0000C526
		public static Stream GetOEMDevicePlatformXSD()
		{
			return ImageGeneratorParameters.GetXSDStream(DevicePaths.OEMDevicePlatformSchema);
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000E334 File Offset: 0x0000C534
		public static Stream GetXSDStream(string xsdID)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string[] manifestResourceNames = executingAssembly.GetManifestResourceNames();
			string text = string.Empty;
			foreach (string text2 in manifestResourceNames)
			{
				if (text2.Contains(xsdID))
				{
					text = text2;
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::GetXSDStream: XSD resource was not found: " + xsdID);
			}
			return executingAssembly.GetManifestResourceStream(text);
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000E398 File Offset: 0x0000C598
		public void ProcessInputXML(string deviceLayoutXMLFile, string oemDevicePlatformXMLFile)
		{
			OEMDevicePlatformInput oemdevicePlatformInput = null;
			XsdValidator xsdValidator = new XsdValidator();
			try
			{
				using (Stream deviceLayoutXSD = ImageGeneratorParameters.GetDeviceLayoutXSD(deviceLayoutXMLFile))
				{
					xsdValidator.ValidateXsd(deviceLayoutXSD, deviceLayoutXMLFile, this._logger);
				}
			}
			catch (XsdValidatorException innerException)
			{
				throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::ProcessInputXML: Unable to validate Device Layout XSD.", innerException);
			}
			this._logger.LogInfo("ImageCommon: Successfully validated the Device Layout XML", new object[0]);
			if (ImageGeneratorParameters.IsDeviceLayoutV2(deviceLayoutXMLFile))
			{
				this.InitializeV2DeviceLayout(deviceLayoutXMLFile);
			}
			else
			{
				this.InitializeV1DeviceLayout(deviceLayoutXMLFile);
			}
			xsdValidator = new XsdValidator();
			try
			{
				using (Stream oemdevicePlatformXSD = ImageGeneratorParameters.GetOEMDevicePlatformXSD())
				{
					xsdValidator.ValidateXsd(oemdevicePlatformXSD, oemDevicePlatformXMLFile, this._logger);
				}
			}
			catch (XsdValidatorException innerException2)
			{
				throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::ProcessInputXML: Unable to validate OEM Device Platform XSD.", innerException2);
			}
			this._logger.LogInfo("ImageCommon: Successfully validated the OEM Device Platform XML", new object[0]);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(OEMDevicePlatformInput));
			using (StreamReader streamReader = new StreamReader(oemDevicePlatformXMLFile))
			{
				try
				{
					oemdevicePlatformInput = (OEMDevicePlatformInput)xmlSerializer.Deserialize(streamReader);
				}
				catch (Exception innerException3)
				{
					throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::ProcessInputXML: Unable to parse OEM Device Platform XML.", innerException3);
				}
			}
			try
			{
				this.DevicePlatformIDs = oemdevicePlatformInput.DevicePlatformIDs;
				this.MinSectorCount = oemdevicePlatformInput.MinSectorCount;
				foreach (InputStore inputStore in this.Stores)
				{
					foreach (InputPartition inputPartition in from x in inputStore.Partitions
					where !string.IsNullOrEmpty(x.FileSystem) && x.FileSystem.Equals("NTFS", StringComparison.OrdinalIgnoreCase)
					select x)
					{
						inputPartition.Compressed = true;
					}
				}
				foreach (string text in oemdevicePlatformInput.UncompressedPartitions ?? new string[0])
				{
					InputPartition inputPartition2 = this.FindPartition(text);
					if (inputPartition2 == null)
					{
						throw new ImageCommonException("Partition " + text + " was marked in the OEMDevicePlatform as uncompressed, but the partition doesn't exist in the device layout. Please ensure the spelling of the partition is correct in OEMDevicePlatform and that the partition is defined in the OEMDeviceLayout.");
					}
					this._logger.LogInfo("ImageCommon: Marking partition " + text + " uncompressed as requested by device plaform.", new object[0]);
					inputPartition2.Compressed = false;
				}
				this.AddSectorsToMainOs(oemdevicePlatformInput.AdditionalMainOSFreeSectorsRequest, oemdevicePlatformInput.MainOSRTCDataReservedSectors);
				if (oemdevicePlatformInput.MMOSPartitionTotalSectorsOverride != 0U)
				{
					InputPartition inputPartition3 = this.FindPartition("MMOS");
					if (inputPartition3 == null)
					{
						throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::ProcessInputXML: The OEM Device Platform XML specifies that the MMOS should have total sectors set but no MMOS partition was found.");
					}
					inputPartition3.TotalSectors = oemdevicePlatformInput.MMOSPartitionTotalSectorsOverride;
				}
				if (oemdevicePlatformInput.Rules != null)
				{
					this.Rules = oemdevicePlatformInput.Rules;
				}
			}
			catch (ImageCommonException)
			{
				throw;
			}
			catch (Exception innerException4)
			{
				throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::ProcessInputXML: There was a problem parsing the OEM Device Platform input", innerException4);
			}
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x0000E714 File Offset: 0x0000C914
		private void InitializeV1DeviceLayout(string DeviceLayoutXMLFile)
		{
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.Load(DeviceLayoutXMLFile);
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::InitializeV1DeviceLayout: Unable to validate Device Layout XSD.", innerException);
			}
			DeviceLayoutInput deviceLayoutInput = null;
			using (StreamReader streamReader = new StreamReader(DeviceLayoutXMLFile))
			{
				try
				{
					deviceLayoutInput = (DeviceLayoutInput)new XmlSerializer(typeof(DeviceLayoutInput)).Deserialize(streamReader);
				}
				catch (Exception innerException2)
				{
					throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::InitializeV1DeviceLayout: Unable to parse Device Layout XML.", innerException2);
				}
			}
			try
			{
				InputStore inputStore = new InputStore("MainOSStore");
				if (deviceLayoutInput.Partitions != null)
				{
					inputStore.Partitions = deviceLayoutInput.Partitions;
				}
				this.SectorSize = deviceLayoutInput.SectorSize;
				this.ChunkSize = deviceLayoutInput.ChunkSize;
				this.VirtualHardDiskSectorSize = deviceLayoutInput.SectorSize;
				this.DefaultPartitionByteAlignment = deviceLayoutInput.DefaultPartitionByteAlignment;
				foreach (InputPartition inputPartition in inputStore.Partitions)
				{
					if (inputPartition.MinFreeSectors != 0U)
					{
						if (inputPartition.TotalSectors != 0U || inputPartition.UseAllSpace)
						{
							throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::InitializeV1DeviceLayout: MinFreeSectors cannot be set for partition '" + inputPartition.Name + "' when either TotalSectors or UseAllSpace is set.");
						}
						if (inputPartition.MinFreeSectors < 8192U)
						{
							throw new ImageCommonException(string.Concat(new object[]
							{
								"ImageCommon!ImageGeneratorParameters::InitializeV1DeviceLayout: MinFreeSectors cannot be set for partition '",
								inputPartition.Name,
								"' less than ",
								8192U,
								" sectors."
							}));
						}
					}
					if (inputPartition.GeneratedFileOverheadSectors != 0U && inputPartition.MinFreeSectors == 0U)
					{
						throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::InitializeV1DeviceLayout: GeneratedFileOverheadSectors cannot be set for partition '" + inputPartition.Name + "' without MinFreeSectors being set.");
					}
				}
				this.Stores.Add(inputStore);
			}
			catch (ImageCommonException)
			{
				throw;
			}
			catch (Exception innerException3)
			{
				throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::InitializeV1DeviceLayout: There was a problem parsing the Device Layout input", innerException3);
			}
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000E948 File Offset: 0x0000CB48
		private void InitializeV2DeviceLayout(string DeviceLayoutXMLFile)
		{
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.Load(DeviceLayoutXMLFile);
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: Unable to validate Device Layout XSD.", innerException);
			}
			DeviceLayoutInputv2 deviceLayoutInputv = null;
			using (StreamReader streamReader = new StreamReader(DeviceLayoutXMLFile))
			{
				try
				{
					deviceLayoutInputv = (DeviceLayoutInputv2)new XmlSerializer(typeof(DeviceLayoutInputv2)).Deserialize(streamReader);
				}
				catch (Exception innerException2)
				{
					throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: Unable to parse Device Layout XML.", innerException2);
				}
			}
			try
			{
				if (deviceLayoutInputv.Stores != null)
				{
					this.Stores = new List<InputStore>(deviceLayoutInputv.Stores);
				}
				this.SectorSize = deviceLayoutInputv.SectorSize;
				this.ChunkSize = deviceLayoutInputv.ChunkSize;
				this.VirtualHardDiskSectorSize = deviceLayoutInputv.SectorSize;
				this.DefaultPartitionByteAlignment = deviceLayoutInputv.DefaultPartitionByteAlignment;
				foreach (InputStore inputStore in this.Stores)
				{
					if (inputStore.IsMainOSStore())
					{
						if (inputStore.SizeInSectors != 0U)
						{
							throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: SizeInSector cannot be set for MainOS store.'");
						}
					}
					else
					{
						if (string.IsNullOrEmpty(inputStore.Id))
						{
							throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: Id needs to be set for individual stores.'");
						}
						if (inputStore.SizeInSectors == 0U)
						{
							throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: SizeInSector needs to be set for non-MainOS store '" + inputStore.Id + "'.");
						}
						if ((ulong)inputStore.SizeInSectors * (ulong)this.SectorSize < 3145728UL)
						{
							throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: Minimum size of a store '" + inputStore.Id + "' must be 3MB or larger.");
						}
					}
					if (string.IsNullOrEmpty(inputStore.StoreType))
					{
						throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: StoreType needs to be set for store '" + inputStore.Id + "'.");
					}
					if (string.IsNullOrEmpty(inputStore.DevicePath))
					{
						throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: DevicePath needs to be set for store '" + inputStore.Id + "'.");
					}
					if (inputStore.OnlyAllocateDefinedGptEntries && inputStore.Partitions.Count<InputPartition>() > 32)
					{
						throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: Cannot use shortened GPT as it has more than 32 partitions for store '" + inputStore.Id + "'.");
					}
					foreach (InputPartition inputPartition in inputStore.Partitions)
					{
						if (inputPartition.MinFreeSectors != 0U)
						{
							if (inputPartition.TotalSectors != 0U || inputPartition.UseAllSpace)
							{
								throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: MinFreeSectors cannot be set for partition '" + inputPartition.Name + "' when either TotalSectors or UseAllSpace is set.");
							}
							if (inputPartition.MinFreeSectors < 8192U)
							{
								throw new ImageCommonException(string.Concat(new object[]
								{
									"ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: MinFreeSectors cannot be set for partition '",
									inputPartition.Name,
									"' less than ",
									8192U,
									" sectors."
								}));
							}
						}
						if (inputPartition.GeneratedFileOverheadSectors != 0U && inputPartition.MinFreeSectors == 0U)
						{
							throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: GeneratedFileOverheadSectors cannot be set for partition '" + inputPartition.Name + "' without MinFreeSectors being set.");
						}
					}
				}
			}
			catch (ImageCommonException)
			{
				throw;
			}
			catch (Exception innerException3)
			{
				throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: There was a problem parsing the Device Layout input", innerException3);
			}
			this._deviceLayoutVersion = 2U;
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000ECD4 File Offset: 0x0000CED4
		private void AddSectorsToMainOs(uint additionalFreeSectors, uint runtimeConfigurationDataSectors)
		{
			InputPartition inputPartition = this.FindPartition("MainOS");
			if (inputPartition == null)
			{
				throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::AddSectorsToMainOs: No MainOS partition found for additional free sectors.");
			}
			if ((additionalFreeSectors != 0U || runtimeConfigurationDataSectors != 0U) && inputPartition.MinFreeSectors == 0U)
			{
				throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::AddSectorsToMainOs: The OEM Device Platform XML specifies that the MainOS should have additional free sectors but the MainOS partition is not using MinFreeSectors.");
			}
			if (runtimeConfigurationDataSectors > 104857600U / this.SectorSize)
			{
				throw new ImageCommonException("ImageCommon!ImageGeneratorParameters::AddSectorsToMainOs: Runtime configuration data reservation is limited to 100MB. Please reduce the number of sectors requested in 'MainOSMVDataReservedSectors' in the OEM device platform input.");
			}
			if (additionalFreeSectors != 0U)
			{
				this._logger.LogInfo("OEM device platform input requested {0} additional free sectors in the MainOS partition.", new object[]
				{
					additionalFreeSectors
				});
			}
			if (runtimeConfigurationDataSectors != 0U)
			{
				this._logger.LogInfo("OEM device platform input requested {0} additional sectors for runtime configuration data be reserved in the MainOS partition.", new object[]
				{
					runtimeConfigurationDataSectors
				});
			}
			inputPartition.MinFreeSectors += additionalFreeSectors;
			inputPartition.MinFreeSectors += runtimeConfigurationDataSectors;
		}

		// Token: 0x04000101 RID: 257
		[CLSCompliant(false)]
		public const uint DefaultChunkSize = 256U;

		// Token: 0x04000102 RID: 258
		[CLSCompliant(false)]
		public const uint DevicePlatformIDSize = 192U;

		// Token: 0x04000103 RID: 259
		private const uint _OneKiloBtye = 1024U;

		// Token: 0x04000104 RID: 260
		private const uint _MinimumSectorSize = 512U;

		// Token: 0x04000105 RID: 261
		private const uint _MinimumSectorFreeCount = 8192U;

		// Token: 0x04000106 RID: 262
		private IULogger _logger;

		// Token: 0x04000107 RID: 263
		private const uint ALG_CLASS_HASH = 32768U;

		// Token: 0x04000108 RID: 264
		private const uint ALG_TYPE_ANY = 0U;

		// Token: 0x04000109 RID: 265
		private const uint ALG_SID_SHA_256 = 12U;

		// Token: 0x0400010A RID: 266
		private const uint CALG_SHA_256 = 32780U;

		// Token: 0x0400010C RID: 268
		public List<InputStore> Stores;

		// Token: 0x0400010D RID: 269
		public string[] DevicePlatformIDs;

		// Token: 0x0400010E RID: 270
		private uint _chunkSize = 256U;

		// Token: 0x04000113 RID: 275
		private uint _algid = 32780U;

		// Token: 0x04000114 RID: 276
		private uint _deviceLayoutVersion = 1U;

		// Token: 0x02000079 RID: 121
		[CLSCompliant(false)]
		public enum FFUHashAlgorithm : uint
		{
			// Token: 0x040002B9 RID: 697
			Ffuhsha256 = 32780U
		}
	}
}
