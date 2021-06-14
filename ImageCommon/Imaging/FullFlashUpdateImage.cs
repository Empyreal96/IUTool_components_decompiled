using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000019 RID: 25
	public class FullFlashUpdateImage
	{
		// Token: 0x060000E9 RID: 233 RVA: 0x0000BD34 File Offset: 0x00009F34
		public void Initialize(string imagePath)
		{
			if (!File.Exists(imagePath))
			{
				throw new ImageCommonException("ImageCommon!FullFlashUpdateImage::Initialize: The FFU file '" + imagePath + "' does not exist.");
			}
			this._imagePath = Path.GetFullPath(imagePath);
			using (FileStream imageStream = this.GetImageStream())
			{
				using (BinaryReader binaryReader = new BinaryReader(imageStream))
				{
					uint num = binaryReader.ReadUInt32();
					byte[] signature = binaryReader.ReadBytes(12);
					if (num != FullFlashUpdateHeaders.SecurityHeaderSize || !FullFlashUpdateImage.SecurityHeader.ValidateSignature(signature))
					{
						throw new ImageCommonException("ImageCommon!FullFlashUpdateImage::Initialize: Unable to load image because the security header is invalid.");
					}
					this._securityHeader.ByteCount = num;
					this._securityHeader.ChunkSize = binaryReader.ReadUInt32();
					this._securityHeader.HashAlgorithmID = binaryReader.ReadUInt32();
					this._securityHeader.CatalogSize = binaryReader.ReadUInt32();
					this._securityHeader.HashTableSize = binaryReader.ReadUInt32();
					this._catalogData = binaryReader.ReadBytes((int)this._securityHeader.CatalogSize);
					this._hashTableData = binaryReader.ReadBytes((int)this._securityHeader.HashTableSize);
					binaryReader.ReadBytes((int)this.SecurityPadding);
					num = binaryReader.ReadUInt32();
					signature = binaryReader.ReadBytes(12);
					if (num != FullFlashUpdateHeaders.ImageHeaderSize || !FullFlashUpdateImage.ImageHeader.ValidateSignature(signature))
					{
						throw new ImageCommonException("ImageCommon!FullFlashUpdateImage::Initialize: Unable to load image because the image header is invalid.");
					}
					this._imageHeader.ByteCount = num;
					this._imageHeader.ManifestLength = binaryReader.ReadUInt32();
					this._imageHeader.ChunkSize = binaryReader.ReadUInt32();
					StreamReader streamReader = new StreamReader(new MemoryStream(binaryReader.ReadBytes((int)this._imageHeader.ManifestLength)), Encoding.ASCII);
					try
					{
						this._manifest = new FullFlashUpdateImage.FullFlashUpdateManifest(this, streamReader);
					}
					finally
					{
						streamReader.Close();
						streamReader = null;
					}
					this.ValidateManifest();
					if (this._imageHeader.ChunkSize > 0U)
					{
						binaryReader.ReadBytes((int)this.CalculateAlignment((uint)imageStream.Position, this._imageHeader.ChunkSize * 1024U));
					}
					this._payloadOffset = imageStream.Position;
				}
			}
		}

		// Token: 0x060000EA RID: 234 RVA: 0x0000BF74 File Offset: 0x0000A174
		public FileStream GetImageStream()
		{
			FileStream fileStream = File.OpenRead(this._imagePath);
			fileStream.Position = this._payloadOffset;
			return fileStream;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x0000BF8D File Offset: 0x0000A18D
		public void AddStore(FullFlashUpdateImage.FullFlashUpdateStore store)
		{
			if (store == null)
			{
				throw new ArgumentNullException("store");
			}
			this._stores.Add(store);
		}

		// Token: 0x060000EC RID: 236 RVA: 0x0000BFAC File Offset: 0x0000A1AC
		private void AddStore(FullFlashUpdateImage.ManifestCategory category)
		{
			uint sectorSize = uint.Parse(category["SectorSize"], CultureInfo.InvariantCulture);
			uint minSectorCount = 0U;
			if (category["MinSectorCount"] != null)
			{
				minSectorCount = uint.Parse(category["MinSectorCount"], CultureInfo.InvariantCulture);
			}
			string storeId = null;
			if (category["StoreId"] != null)
			{
				storeId = category["StoreId"];
			}
			bool isMainOSStore = true;
			if (category["IsMainOSStore"] != null)
			{
				isMainOSStore = bool.Parse(category["IsMainOSStore"]);
			}
			string devicePath = null;
			if (category["DevicePath"] != null)
			{
				devicePath = category["DevicePath"];
			}
			bool onlyAllocateDefinedGptEntries = false;
			if (category["OnlyAllocateDefinedGptEntries"] != null)
			{
				onlyAllocateDefinedGptEntries = bool.Parse(category["OnlyAllocateDefinedGptEntries"]);
			}
			FullFlashUpdateImage.FullFlashUpdateStore fullFlashUpdateStore = new FullFlashUpdateImage.FullFlashUpdateStore();
			fullFlashUpdateStore.Initialize(this, storeId, isMainOSStore, devicePath, onlyAllocateDefinedGptEntries, minSectorCount, sectorSize);
			this._stores.Add(fullFlashUpdateStore);
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000ED RID: 237 RVA: 0x0000C092 File Offset: 0x0000A292
		public FullFlashUpdateImage.SecurityHeader GetSecureHeader
		{
			get
			{
				return this._securityHeader;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000EE RID: 238 RVA: 0x0000C09C File Offset: 0x0000A29C
		public static int SecureHeaderSize
		{
			get
			{
				return Marshal.SizeOf(default(FullFlashUpdateImage.SecurityHeader));
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000EF RID: 239 RVA: 0x0000C0BC File Offset: 0x0000A2BC
		// (set) Token: 0x060000F0 RID: 240 RVA: 0x0000C0C4 File Offset: 0x0000A2C4
		public byte[] CatalogData
		{
			get
			{
				return this._catalogData;
			}
			set
			{
				this._catalogData = value;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000F1 RID: 241 RVA: 0x0000C0CD File Offset: 0x0000A2CD
		// (set) Token: 0x060000F2 RID: 242 RVA: 0x0000C0D5 File Offset: 0x0000A2D5
		public byte[] HashTableData
		{
			get
			{
				return this._hashTableData;
			}
			set
			{
				this._hashTableData = value;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x0000C0DE File Offset: 0x0000A2DE
		public FullFlashUpdateImage.ImageHeader GetImageHeader
		{
			get
			{
				return this._imageHeader;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x0000C0E8 File Offset: 0x0000A2E8
		public static int ImageHeaderSize
		{
			get
			{
				return Marshal.SizeOf(default(FullFlashUpdateImage.ImageHeader));
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x0000C108 File Offset: 0x0000A308
		// (set) Token: 0x060000F6 RID: 246 RVA: 0x0000C115 File Offset: 0x0000A315
		public uint ChunkSize
		{
			get
			{
				return this._imageHeader.ChunkSize;
			}
			set
			{
				this._imageHeader.ChunkSize = value;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x0000C123 File Offset: 0x0000A323
		public uint ChunkSizeInBytes
		{
			get
			{
				return this.ChunkSize * 1024U;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x0000C131 File Offset: 0x0000A331
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x0000C13E File Offset: 0x0000A33E
		public uint HashAlgorithmID
		{
			get
			{
				return this._securityHeader.HashAlgorithmID;
			}
			set
			{
				this._securityHeader.HashAlgorithmID = value;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000FA RID: 250 RVA: 0x0000C14C File Offset: 0x0000A34C
		// (set) Token: 0x060000FB RID: 251 RVA: 0x0000C159 File Offset: 0x0000A359
		public uint ManifestLength
		{
			get
			{
				return this._imageHeader.ManifestLength;
			}
			set
			{
				this._imageHeader.ManifestLength = value;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000FC RID: 252 RVA: 0x0000C167 File Offset: 0x0000A367
		public int StoreCount
		{
			get
			{
				return this._stores.Count;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000FD RID: 253 RVA: 0x0000C174 File Offset: 0x0000A374
		public List<FullFlashUpdateImage.FullFlashUpdateStore> Stores
		{
			get
			{
				return this._stores;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000FE RID: 254 RVA: 0x0000C17C File Offset: 0x0000A37C
		public uint ImageStyle
		{
			get
			{
				bool flag = true;
				if (this.Stores[0].Partitions != null && this.Stores[0].Partitions.Count<FullFlashUpdateImage.FullFlashUpdatePartition>() > 0)
				{
					flag = FullFlashUpdateImage.IsGPTPartitionType(this.Stores[0].Partitions[0].PartitionType);
				}
				if (!flag)
				{
					return FullFlashUpdateImage.PartitionTypeMbr;
				}
				return FullFlashUpdateImage.PartitionTypeGpt;
			}
		}

		// Token: 0x060000FF RID: 255 RVA: 0x0000C1E8 File Offset: 0x0000A3E8
		public static bool IsGPTPartitionType(string partitionType)
		{
			Guid guid;
			return Guid.TryParse(partitionType, out guid);
		}

		// Token: 0x17000030 RID: 48
		public FullFlashUpdateImage.FullFlashUpdatePartition this[string name]
		{
			get
			{
				foreach (FullFlashUpdateImage.FullFlashUpdateStore fullFlashUpdateStore in this.Stores)
				{
					foreach (FullFlashUpdateImage.FullFlashUpdatePartition fullFlashUpdatePartition in fullFlashUpdateStore.Partitions)
					{
						if (string.CompareOrdinal(fullFlashUpdatePartition.Name, name) == 0)
						{
							return fullFlashUpdatePartition;
						}
					}
				}
				return null;
			}
		}

		// Token: 0x06000101 RID: 257 RVA: 0x0000C29C File Offset: 0x0000A49C
		public void DisplayImageInformation(IULogger logger)
		{
			foreach (string text in this.DevicePlatformIDs)
			{
				logger.LogInfo("\tDevice Platform ID: {0}", new object[]
				{
					text
				});
			}
			logger.LogInfo("\tChunk Size: 0x{0:X}", new object[]
			{
				this.ChunkSize
			});
			logger.LogInfo(" ", new object[0]);
			foreach (FullFlashUpdateImage.FullFlashUpdateStore fullFlashUpdateStore in this.Stores)
			{
				logger.LogInfo("Store", new object[0]);
				logger.LogInfo("\tSector Size: 0x{0:X}", new object[]
				{
					fullFlashUpdateStore.SectorSize
				});
				logger.LogInfo("\tID: {0}", new object[]
				{
					fullFlashUpdateStore.Id
				});
				logger.LogInfo("\tDevice Path: {0}", new object[]
				{
					fullFlashUpdateStore.DevicePath
				});
				logger.LogInfo("\tContains MainOS: {0}", new object[]
				{
					fullFlashUpdateStore.IsMainOSStore
				});
				if (fullFlashUpdateStore.IsMainOSStore)
				{
					logger.LogInfo("\tMinimum Sector Count: 0x{0:X}", new object[]
					{
						fullFlashUpdateStore.SectorCount
					});
				}
				logger.LogInfo(" ", new object[0]);
				logger.LogInfo("There are {0} partitions in the store.", new object[]
				{
					fullFlashUpdateStore.Partitions.Count
				});
				logger.LogInfo(" ", new object[0]);
				foreach (FullFlashUpdateImage.FullFlashUpdatePartition fullFlashUpdatePartition in fullFlashUpdateStore.Partitions)
				{
					logger.LogInfo("\tPartition", new object[0]);
					logger.LogInfo("\t\tName: {0}", new object[]
					{
						fullFlashUpdatePartition.Name
					});
					logger.LogInfo("\t\tPartition Type: {0}", new object[]
					{
						fullFlashUpdatePartition.PartitionType
					});
					logger.LogInfo("\t\tTotal Sectors: 0x{0:X}", new object[]
					{
						fullFlashUpdatePartition.TotalSectors
					});
					logger.LogInfo("\t\tSectors In Use: 0x{0:X}", new object[]
					{
						fullFlashUpdatePartition.SectorsInUse
					});
					logger.LogInfo(" ", new object[0]);
				}
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000102 RID: 258 RVA: 0x0000C560 File Offset: 0x0000A760
		public uint StartOfImageHeader
		{
			get
			{
				uint num = 0U;
				if (this._manifest != null)
				{
					num += FullFlashUpdateHeaders.SecurityHeaderSize;
					num += this._securityHeader.CatalogSize;
					num += this._securityHeader.HashTableSize;
				}
				return num;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000103 RID: 259 RVA: 0x0000C59C File Offset: 0x0000A79C
		public FullFlashUpdateImage.FullFlashUpdateManifest GetManifest
		{
			get
			{
				return this._manifest;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000104 RID: 260 RVA: 0x0000C5A4 File Offset: 0x0000A7A4
		// (set) Token: 0x06000105 RID: 261 RVA: 0x0000C5AC File Offset: 0x0000A7AC
		public uint DefaultPartitionAlignmentInBytes
		{
			get
			{
				return this._defaultPartititionByteAlignment;
			}
			set
			{
				if (InputHelpers.IsPowerOfTwo(value))
				{
					this._defaultPartititionByteAlignment = value;
				}
			}
		}

		// Token: 0x06000106 RID: 262 RVA: 0x0000C5C0 File Offset: 0x0000A7C0
		private uint CalculateAlignment(uint currentPosition, uint blockSize)
		{
			uint result = 0U;
			uint num = currentPosition % blockSize;
			if (num > 0U)
			{
				result = blockSize - num;
			}
			return result;
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000107 RID: 263 RVA: 0x0000C5DC File Offset: 0x0000A7DC
		public uint SecurityPadding
		{
			get
			{
				uint num = 1024U;
				if (this._imageHeader.ChunkSize != 0U)
				{
					num *= this._imageHeader.ChunkSize;
				}
				else
				{
					if (this._securityHeader.ChunkSize == 0U)
					{
						throw new ImageCommonException("ImageCommon!FullFlashUpdateImage::SecurityPadding: Neither the of the headers have been initialized with a chunk size.");
					}
					num *= this._securityHeader.ChunkSize;
				}
				return this.CalculateAlignment(FullFlashUpdateHeaders.SecurityHeaderSize + (uint)((this.CatalogData != null) ? this.CatalogData.Length : 0) + (uint)((this.HashTableData != null) ? this.HashTableData.Length : 0), num);
			}
		}

		// Token: 0x06000108 RID: 264 RVA: 0x0000C668 File Offset: 0x0000A868
		public byte[] GetSecurityHeader(byte[] catalogData, byte[] hashData)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(FullFlashUpdateHeaders.SecurityHeaderSize);
			binaryWriter.Write(FullFlashUpdateHeaders.GetSecuritySignature());
			binaryWriter.Write(this.ChunkSize);
			binaryWriter.Write(this.HashAlgorithmID);
			binaryWriter.Write(catalogData.Length);
			binaryWriter.Write(hashData.Length);
			binaryWriter.Write(catalogData);
			binaryWriter.Write(hashData);
			binaryWriter.Flush();
			if (memoryStream.Length % (long)((ulong)this.ChunkSizeInBytes) != 0L)
			{
				long num = (long)((ulong)this.ChunkSizeInBytes - (ulong)(memoryStream.Length % (long)((ulong)this.ChunkSizeInBytes)));
				MemoryStream memoryStream2 = memoryStream;
				memoryStream2.SetLength(memoryStream2.Length + num);
			}
			return memoryStream.ToArray();
		}

		// Token: 0x06000109 RID: 265 RVA: 0x0000C710 File Offset: 0x0000A910
		public byte[] GetManifestRegion()
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(FullFlashUpdateHeaders.ImageHeaderSize);
			binaryWriter.Write(FullFlashUpdateHeaders.GetImageSignature());
			binaryWriter.Write(this._manifest.Length);
			binaryWriter.Write(this.ChunkSize);
			binaryWriter.Flush();
			this._manifest.WriteToStream(memoryStream);
			if (memoryStream.Length % (long)((ulong)this.ChunkSizeInBytes) != 0L)
			{
				long num = (long)((ulong)this.ChunkSizeInBytes - (ulong)(memoryStream.Length % (long)((ulong)this.ChunkSizeInBytes)));
				MemoryStream memoryStream2 = memoryStream;
				memoryStream2.SetLength(memoryStream2.Length + num);
			}
			return memoryStream.ToArray();
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600010A RID: 266 RVA: 0x0000C7A8 File Offset: 0x0000A9A8
		// (set) Token: 0x0600010B RID: 267 RVA: 0x0000C80C File Offset: 0x0000AA0C
		public string Description
		{
			get
			{
				if (this._manifest != null && this._manifest["FullFlash"] != null && this._manifest["FullFlash"]["Description"] != null)
				{
					return this._manifest["FullFlash"]["Description"];
				}
				return "";
			}
			set
			{
				if (this._manifest != null)
				{
					if (this._manifest["FullFlash"] == null)
					{
						this._manifest.AddCategory("FullFlash", "FullFlash");
					}
					this._manifest["FullFlash"]["Description"] = value;
				}
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600010C RID: 268 RVA: 0x0000C864 File Offset: 0x0000AA64
		// (set) Token: 0x0600010D RID: 269 RVA: 0x0000C904 File Offset: 0x0000AB04
		public List<string> DevicePlatformIDs
		{
			get
			{
				List<string> list = new List<string>();
				if (this._manifest == null || this._manifest["FullFlash"] == null)
				{
					return list;
				}
				int num = 0;
				for (;;)
				{
					string name = string.Format("DevicePlatformId{0}", num);
					if (this._manifest["FullFlash"][name] == null)
					{
						break;
					}
					num++;
				}
				for (int i = 0; i < num; i++)
				{
					string name2 = string.Format("DevicePlatformId{0}", i);
					list.Add(this._manifest["FullFlash"][name2]);
				}
				return list;
			}
			set
			{
				if (this._manifest != null)
				{
					if (this._manifest["FullFlash"] == null)
					{
						this._manifest.AddCategory("FullFlash", "FullFlash");
					}
					for (int i = 0; i < value.Count; i++)
					{
						string name = string.Format("DevicePlatformId{0}", i);
						this._manifest["FullFlash"][name] = value[i];
					}
				}
			}
		}

		// Token: 0x0600010E RID: 270 RVA: 0x0000C980 File Offset: 0x0000AB80
		public void WriteManifest(Stream stream)
		{
			this._manifest.WriteToStream(stream);
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000C990 File Offset: 0x0000AB90
		private void ValidateManifest()
		{
			if (this._manifest["FullFlash"] == null)
			{
				throw new ImageCommonException("ImageCommon!FullFlashUpdateImage::ValidateManifest: Missing 'FullFlash' or 'Image' category in the manifest");
			}
			string text = this._manifest["FullFlash"]["Version"];
			if (text == null)
			{
				throw new ImageCommonException("ImageCommon!FullFlashUpdateImage::ValidateManifest: Missing 'Version' name/value pair in the 'FullFlash' category.");
			}
			if (!text.Equals("2.0", StringComparison.OrdinalIgnoreCase))
			{
				throw new ImageCommonException("ImageCommon!FullFlashUpdateImage::ValidateManifest: 'Version' value (" + text + ") does not match current version of 2.0.");
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000110 RID: 272 RVA: 0x0000CA0C File Offset: 0x0000AC0C
		// (set) Token: 0x06000111 RID: 273 RVA: 0x0000CA70 File Offset: 0x0000AC70
		public string Version
		{
			get
			{
				if (this._manifest != null && this._manifest["FullFlash"] != null && this._manifest["FullFlash"]["Version"] != null)
				{
					return this._manifest["FullFlash"]["Version"];
				}
				return string.Empty;
			}
			private set
			{
				if (this._manifest != null)
				{
					if (this._manifest["FullFlash"] == null)
					{
						this._manifest.AddCategory("FullFlash", "FullFlash");
					}
					this._manifest["FullFlash"]["Version"] = value;
				}
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000112 RID: 274 RVA: 0x0000CAC8 File Offset: 0x0000ACC8
		// (set) Token: 0x06000113 RID: 275 RVA: 0x0000CB2C File Offset: 0x0000AD2C
		public string OSVersion
		{
			get
			{
				if (this._manifest != null && this._manifest["FullFlash"] != null && this._manifest["FullFlash"]["OSVersion"] != null)
				{
					return this._manifest["FullFlash"]["OSVersion"];
				}
				return string.Empty;
			}
			set
			{
				if (this._manifest != null)
				{
					if (this._manifest["FullFlash"] == null)
					{
						this._manifest.AddCategory("FullFlash", "FullFlash");
					}
					this._manifest["FullFlash"]["OSVersion"] = value;
				}
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000114 RID: 276 RVA: 0x0000CB84 File Offset: 0x0000AD84
		// (set) Token: 0x06000115 RID: 277 RVA: 0x0000CBE8 File Offset: 0x0000ADE8
		public string CanFlashToRemovableMedia
		{
			get
			{
				if (this._manifest != null && this._manifest["FullFlash"] != null && this._manifest["FullFlash"]["CanFlashToRemovableMedia"] != null)
				{
					return this._manifest["FullFlash"]["CanFlashToRemovableMedia"];
				}
				return string.Empty;
			}
			set
			{
				if (this._manifest != null)
				{
					if (this._manifest["FullFlash"] == null)
					{
						this._manifest.AddCategory("FullFlash", "FullFlash");
					}
					this._manifest["FullFlash"]["CanFlashToRemovableMedia"] = value;
				}
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000116 RID: 278 RVA: 0x0000CC40 File Offset: 0x0000AE40
		// (set) Token: 0x06000117 RID: 279 RVA: 0x0000CCA4 File Offset: 0x0000AEA4
		public string AntiTheftVersion
		{
			get
			{
				if (this._manifest != null && this._manifest["FullFlash"] != null && this._manifest["FullFlash"]["AntiTheftVersion"] != null)
				{
					return this._manifest["FullFlash"]["AntiTheftVersion"];
				}
				return string.Empty;
			}
			set
			{
				if (this._manifest != null)
				{
					if (this._manifest["FullFlash"] == null)
					{
						this._manifest.AddCategory("FullFlash", "FullFlash");
					}
					this._manifest["FullFlash"]["AntiTheftVersion"] = value;
				}
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000118 RID: 280 RVA: 0x0000CCFC File Offset: 0x0000AEFC
		// (set) Token: 0x06000119 RID: 281 RVA: 0x0000CD60 File Offset: 0x0000AF60
		public string RulesVersion
		{
			get
			{
				if (this._manifest != null && this._manifest["FullFlash"] != null && this._manifest["FullFlash"]["RulesVersion"] != null)
				{
					return this._manifest["FullFlash"]["RulesVersion"];
				}
				return string.Empty;
			}
			set
			{
				if (this._manifest != null)
				{
					if (this._manifest["FullFlash"] == null)
					{
						this._manifest.AddCategory("FullFlash", "FullFlash");
					}
					this._manifest["FullFlash"]["RulesVersion"] = value;
				}
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x0600011A RID: 282 RVA: 0x0000CDB8 File Offset: 0x0000AFB8
		// (set) Token: 0x0600011B RID: 283 RVA: 0x0000CE1C File Offset: 0x0000B01C
		public string RulesData
		{
			get
			{
				if (this._manifest != null && this._manifest["FullFlash"] != null && this._manifest["FullFlash"]["RulesData"] != null)
				{
					return this._manifest["FullFlash"]["RulesData"];
				}
				return string.Empty;
			}
			set
			{
				if (this._manifest != null)
				{
					if (this._manifest["FullFlash"] == null)
					{
						this._manifest.AddCategory("FullFlash", "FullFlash");
					}
					this._manifest["FullFlash"]["RulesData"] = value;
				}
			}
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000CE74 File Offset: 0x0000B074
		public void Initialize()
		{
			this._manifest = new FullFlashUpdateImage.FullFlashUpdateManifest(this);
			this.Version = "2.0";
		}

		// Token: 0x040000BA RID: 186
		private FullFlashUpdateImage.FullFlashUpdateManifest _manifest;

		// Token: 0x040000BB RID: 187
		private const uint _OneKiloByte = 1024U;

		// Token: 0x040000BC RID: 188
		private const string _version = "2.0";

		// Token: 0x040000BD RID: 189
		private const uint _DefaultPartitionByteAlignment = 65536U;

		// Token: 0x040000BE RID: 190
		private List<FullFlashUpdateImage.FullFlashUpdateStore> _stores = new List<FullFlashUpdateImage.FullFlashUpdateStore>();

		// Token: 0x040000BF RID: 191
		private string _imagePath;

		// Token: 0x040000C0 RID: 192
		private long _payloadOffset;

		// Token: 0x040000C1 RID: 193
		private FullFlashUpdateImage.ImageHeader _imageHeader;

		// Token: 0x040000C2 RID: 194
		private FullFlashUpdateImage.SecurityHeader _securityHeader;

		// Token: 0x040000C3 RID: 195
		private byte[] _catalogData;

		// Token: 0x040000C4 RID: 196
		private byte[] _hashTableData;

		// Token: 0x040000C5 RID: 197
		private uint _defaultPartititionByteAlignment = 65536U;

		// Token: 0x040000C6 RID: 198
		public static readonly uint PartitionTypeMbr = 0U;

		// Token: 0x040000C7 RID: 199
		public static readonly uint PartitionTypeGpt = 1U;

		// Token: 0x02000072 RID: 114
		public struct SecurityHeader
		{
			// Token: 0x06000320 RID: 800 RVA: 0x000149A8 File Offset: 0x00012BA8
			public static bool ValidateSignature(byte[] signature)
			{
				byte[] securitySignature = FullFlashUpdateHeaders.GetSecuritySignature();
				for (int i = 0; i < securitySignature.Length; i++)
				{
					if (signature[i] != securitySignature[i])
					{
						return false;
					}
				}
				return true;
			}

			// Token: 0x1700007B RID: 123
			// (get) Token: 0x06000321 RID: 801 RVA: 0x000149D4 File Offset: 0x00012BD4
			// (set) Token: 0x06000322 RID: 802 RVA: 0x000149DC File Offset: 0x00012BDC
			public uint ByteCount
			{
				get
				{
					return this._byteCount;
				}
				set
				{
					this._byteCount = value;
				}
			}

			// Token: 0x1700007C RID: 124
			// (get) Token: 0x06000323 RID: 803 RVA: 0x000149E5 File Offset: 0x00012BE5
			// (set) Token: 0x06000324 RID: 804 RVA: 0x000149ED File Offset: 0x00012BED
			public uint ChunkSize
			{
				get
				{
					return this._chunkSize;
				}
				set
				{
					this._chunkSize = value;
				}
			}

			// Token: 0x1700007D RID: 125
			// (get) Token: 0x06000325 RID: 805 RVA: 0x000149F6 File Offset: 0x00012BF6
			// (set) Token: 0x06000326 RID: 806 RVA: 0x000149FE File Offset: 0x00012BFE
			public uint HashAlgorithmID
			{
				get
				{
					return this._algid;
				}
				set
				{
					this._algid = value;
				}
			}

			// Token: 0x1700007E RID: 126
			// (get) Token: 0x06000327 RID: 807 RVA: 0x00014A07 File Offset: 0x00012C07
			// (set) Token: 0x06000328 RID: 808 RVA: 0x00014A0F File Offset: 0x00012C0F
			public uint CatalogSize
			{
				get
				{
					return this._catalogSize;
				}
				set
				{
					this._catalogSize = value;
				}
			}

			// Token: 0x1700007F RID: 127
			// (get) Token: 0x06000329 RID: 809 RVA: 0x00014A18 File Offset: 0x00012C18
			// (set) Token: 0x0600032A RID: 810 RVA: 0x00014A20 File Offset: 0x00012C20
			public uint HashTableSize
			{
				get
				{
					return this._hashTableSize;
				}
				set
				{
					this._hashTableSize = value;
				}
			}

			// Token: 0x0400028B RID: 651
			private uint _byteCount;

			// Token: 0x0400028C RID: 652
			private uint _chunkSize;

			// Token: 0x0400028D RID: 653
			private uint _algid;

			// Token: 0x0400028E RID: 654
			private uint _catalogSize;

			// Token: 0x0400028F RID: 655
			private uint _hashTableSize;
		}

		// Token: 0x02000073 RID: 115
		public struct ImageHeader
		{
			// Token: 0x0600032B RID: 811 RVA: 0x00014A2C File Offset: 0x00012C2C
			public static bool ValidateSignature(byte[] signature)
			{
				byte[] imageSignature = FullFlashUpdateHeaders.GetImageSignature();
				for (int i = 0; i < imageSignature.Length; i++)
				{
					if (signature[i] != imageSignature[i])
					{
						return false;
					}
				}
				return true;
			}

			// Token: 0x17000080 RID: 128
			// (get) Token: 0x0600032C RID: 812 RVA: 0x00014A58 File Offset: 0x00012C58
			// (set) Token: 0x0600032D RID: 813 RVA: 0x00014A60 File Offset: 0x00012C60
			public uint ByteCount
			{
				get
				{
					return this._byteCount;
				}
				set
				{
					this._byteCount = value;
				}
			}

			// Token: 0x17000081 RID: 129
			// (get) Token: 0x0600032E RID: 814 RVA: 0x00014A69 File Offset: 0x00012C69
			// (set) Token: 0x0600032F RID: 815 RVA: 0x00014A71 File Offset: 0x00012C71
			public uint ManifestLength
			{
				get
				{
					return this._manifestLength;
				}
				set
				{
					this._manifestLength = value;
				}
			}

			// Token: 0x17000082 RID: 130
			// (get) Token: 0x06000330 RID: 816 RVA: 0x00014A7A File Offset: 0x00012C7A
			// (set) Token: 0x06000331 RID: 817 RVA: 0x00014A82 File Offset: 0x00012C82
			public uint ChunkSize
			{
				get
				{
					return this._chunkSize;
				}
				set
				{
					this._chunkSize = value;
				}
			}

			// Token: 0x04000290 RID: 656
			private uint _byteCount;

			// Token: 0x04000291 RID: 657
			private uint _manifestLength;

			// Token: 0x04000292 RID: 658
			private uint _chunkSize;
		}

		// Token: 0x02000074 RID: 116
		public class FullFlashUpdatePartition
		{
			// Token: 0x06000332 RID: 818 RVA: 0x00014A8C File Offset: 0x00012C8C
			public void Initialize(uint usedSectors, uint totalSectors, string partitionType, string partitionId, string name, FullFlashUpdateImage.FullFlashUpdateStore store, bool useAllSpace)
			{
				this.Initialize(usedSectors, totalSectors, partitionType, partitionId, name, store, useAllSpace, false);
			}

			// Token: 0x06000333 RID: 819 RVA: 0x00014AAC File Offset: 0x00012CAC
			public void Initialize(uint usedSectors, uint totalSectors, string partitionType, string partitionId, string name, FullFlashUpdateImage.FullFlashUpdateStore store, bool useAllSpace, bool isDesktopImage)
			{
				this._sectorsInUse = usedSectors;
				this._totalSectors = totalSectors;
				this._type = partitionType;
				this._id = partitionId;
				this._name = name;
				this._store = store;
				this._useAllSpace = useAllSpace;
				if (!isDesktopImage && this._useAllSpace && !name.Equals("Data", StringComparison.InvariantCultureIgnoreCase))
				{
					throw new ImageCommonException(string.Format("ImageCommon!FullFlashUpdatePartition::Initialize: Partition {0} cannot specify UseAllSpace.", this._name));
				}
				if (this._totalSectors == 4294967295U)
				{
					throw new ImageCommonException(string.Concat(new object[]
					{
						"ImageCommon!FullFlashUpdatePartition::Initialize: Partition ",
						name,
						" is too large (",
						this._totalSectors,
						" sectors)"
					}));
				}
				this.ReadOnly = false;
				this.Bootable = false;
				this.Hidden = false;
				this.AttachDriveLetter = false;
				this.RequiredToFlash = false;
				this.SectorAlignment = 0U;
				this._fileSystem = string.Empty;
				this._byteAlignment = 0U;
				this._clusterSize = 0U;
			}

			// Token: 0x06000334 RID: 820 RVA: 0x00014BA8 File Offset: 0x00012DA8
			public void ToCategory(FullFlashUpdateImage.ManifestCategory category)
			{
				category.Clean();
				category["Name"] = this._name;
				category["Type"] = this._type;
				if (!string.IsNullOrEmpty(this._id))
				{
					category["Id"] = this._id;
				}
				category["Primary"] = this.PrimaryPartition;
				if (!string.IsNullOrEmpty(this._fileSystem))
				{
					category["FileSystem"] = this._fileSystem;
				}
				if (this.ReadOnly)
				{
					category["ReadOnly"] = this.ReadOnly.ToString(CultureInfo.InvariantCulture);
				}
				if (this.Hidden)
				{
					category["Hidden"] = this.Hidden.ToString(CultureInfo.InvariantCulture);
				}
				if (this.AttachDriveLetter)
				{
					category["AttachDriveLetter"] = this.AttachDriveLetter.ToString(CultureInfo.InvariantCulture);
				}
				if (this.Bootable)
				{
					category["Bootable"] = this.Bootable.ToString(CultureInfo.InvariantCulture);
				}
				if (this._useAllSpace)
				{
					category["UseAllSpace"] = "true";
				}
				else
				{
					category["TotalSectors"] = this._totalSectors.ToString(CultureInfo.InvariantCulture);
					category["UsedSectors"] = this._sectorsInUse.ToString(CultureInfo.InvariantCulture);
				}
				if (this._byteAlignment != 0U)
				{
					category["ByteAlignment"] = this._byteAlignment.ToString(CultureInfo.InvariantCulture);
				}
				if (this._clusterSize != 0U)
				{
					category["ClusterSize"] = this._clusterSize.ToString(CultureInfo.InvariantCulture);
				}
				if (this.SectorAlignment != 0U)
				{
					category["SectorAlignment"] = this.SectorAlignment.ToString(CultureInfo.InvariantCulture);
				}
				if (this.RequiredToFlash)
				{
					category["RequiredToFlash"] = this.RequiredToFlash.ToString(CultureInfo.InvariantCulture);
				}
			}

			// Token: 0x06000335 RID: 821 RVA: 0x00014DA4 File Offset: 0x00012FA4
			public override string ToString()
			{
				return this.Name;
			}

			// Token: 0x17000083 RID: 131
			// (get) Token: 0x06000336 RID: 822 RVA: 0x00014DAC File Offset: 0x00012FAC
			// (set) Token: 0x06000337 RID: 823 RVA: 0x00014DB4 File Offset: 0x00012FB4
			public string Name
			{
				get
				{
					return this._name;
				}
				set
				{
					this._name = value;
				}
			}

			// Token: 0x17000084 RID: 132
			// (get) Token: 0x06000338 RID: 824 RVA: 0x00014DBD File Offset: 0x00012FBD
			// (set) Token: 0x06000339 RID: 825 RVA: 0x00014DC5 File Offset: 0x00012FC5
			public uint TotalSectors
			{
				get
				{
					return this._totalSectors;
				}
				set
				{
					this._totalSectors = value;
				}
			}

			// Token: 0x17000085 RID: 133
			// (get) Token: 0x0600033A RID: 826 RVA: 0x00014DCE File Offset: 0x00012FCE
			// (set) Token: 0x0600033B RID: 827 RVA: 0x00014DD6 File Offset: 0x00012FD6
			public string PartitionType
			{
				get
				{
					return this._type;
				}
				set
				{
					this._type = value;
				}
			}

			// Token: 0x17000086 RID: 134
			// (get) Token: 0x0600033C RID: 828 RVA: 0x00014DDF File Offset: 0x00012FDF
			// (set) Token: 0x0600033D RID: 829 RVA: 0x00014DE7 File Offset: 0x00012FE7
			public string PartitionId
			{
				get
				{
					return this._id;
				}
				set
				{
					this._id = value;
				}
			}

			// Token: 0x17000087 RID: 135
			// (get) Token: 0x0600033E RID: 830 RVA: 0x00014DF0 File Offset: 0x00012FF0
			// (set) Token: 0x0600033F RID: 831 RVA: 0x00014DF8 File Offset: 0x00012FF8
			public bool Bootable { get; set; }

			// Token: 0x17000088 RID: 136
			// (get) Token: 0x06000340 RID: 832 RVA: 0x00014E01 File Offset: 0x00013001
			// (set) Token: 0x06000341 RID: 833 RVA: 0x00014E09 File Offset: 0x00013009
			public bool ReadOnly { get; set; }

			// Token: 0x17000089 RID: 137
			// (get) Token: 0x06000342 RID: 834 RVA: 0x00014E12 File Offset: 0x00013012
			// (set) Token: 0x06000343 RID: 835 RVA: 0x00014E1A File Offset: 0x0001301A
			public bool Hidden { get; set; }

			// Token: 0x1700008A RID: 138
			// (get) Token: 0x06000344 RID: 836 RVA: 0x00014E23 File Offset: 0x00013023
			// (set) Token: 0x06000345 RID: 837 RVA: 0x00014E2B File Offset: 0x0001302B
			public bool AttachDriveLetter { get; set; }

			// Token: 0x1700008B RID: 139
			// (get) Token: 0x06000346 RID: 838 RVA: 0x00014E34 File Offset: 0x00013034
			// (set) Token: 0x06000347 RID: 839 RVA: 0x00014E3C File Offset: 0x0001303C
			public string PrimaryPartition { get; set; }

			// Token: 0x1700008C RID: 140
			// (get) Token: 0x06000348 RID: 840 RVA: 0x00014550 File Offset: 0x00012750
			public bool Contiguous
			{
				get
				{
					return true;
				}
			}

			// Token: 0x1700008D RID: 141
			// (get) Token: 0x06000349 RID: 841 RVA: 0x00014E45 File Offset: 0x00013045
			// (set) Token: 0x0600034A RID: 842 RVA: 0x00014E4D File Offset: 0x0001304D
			public string FileSystem
			{
				get
				{
					return this._fileSystem;
				}
				set
				{
					this._fileSystem = value;
				}
			}

			// Token: 0x1700008E RID: 142
			// (get) Token: 0x0600034B RID: 843 RVA: 0x00014E56 File Offset: 0x00013056
			// (set) Token: 0x0600034C RID: 844 RVA: 0x00014E5E File Offset: 0x0001305E
			public uint ByteAlignment
			{
				get
				{
					return this._byteAlignment;
				}
				set
				{
					if (InputHelpers.IsPowerOfTwo(value))
					{
						this._byteAlignment = value;
					}
				}
			}

			// Token: 0x1700008F RID: 143
			// (get) Token: 0x0600034D RID: 845 RVA: 0x00014E6F File Offset: 0x0001306F
			// (set) Token: 0x0600034E RID: 846 RVA: 0x00014E77 File Offset: 0x00013077
			public uint ClusterSize
			{
				get
				{
					return this._clusterSize;
				}
				set
				{
					if (InputHelpers.IsPowerOfTwo(value))
					{
						this._clusterSize = value;
					}
				}
			}

			// Token: 0x17000090 RID: 144
			// (get) Token: 0x0600034F RID: 847 RVA: 0x00014E88 File Offset: 0x00013088
			public uint LastUsedSector
			{
				get
				{
					if (this._sectorsInUse > 0U)
					{
						return this._sectorsInUse - 1U;
					}
					return 0U;
				}
			}

			// Token: 0x17000091 RID: 145
			// (get) Token: 0x06000350 RID: 848 RVA: 0x00014E9D File Offset: 0x0001309D
			// (set) Token: 0x06000351 RID: 849 RVA: 0x00014EA5 File Offset: 0x000130A5
			public uint SectorsInUse
			{
				get
				{
					return this._sectorsInUse;
				}
				set
				{
					this._sectorsInUse = value;
				}
			}

			// Token: 0x17000092 RID: 146
			// (get) Token: 0x06000352 RID: 850 RVA: 0x00014EAE File Offset: 0x000130AE
			// (set) Token: 0x06000353 RID: 851 RVA: 0x00014EB6 File Offset: 0x000130B6
			public bool UseAllSpace
			{
				get
				{
					return this._useAllSpace;
				}
				set
				{
					this._useAllSpace = value;
				}
			}

			// Token: 0x17000093 RID: 147
			// (get) Token: 0x06000354 RID: 852 RVA: 0x00014EBF File Offset: 0x000130BF
			// (set) Token: 0x06000355 RID: 853 RVA: 0x00014EC7 File Offset: 0x000130C7
			public bool RequiredToFlash { get; set; }

			// Token: 0x17000094 RID: 148
			// (get) Token: 0x06000356 RID: 854 RVA: 0x00014ED0 File Offset: 0x000130D0
			// (set) Token: 0x06000357 RID: 855 RVA: 0x00014ED8 File Offset: 0x000130D8
			public uint SectorAlignment { get; set; }

			// Token: 0x04000293 RID: 659
			private uint _sectorsInUse;

			// Token: 0x04000294 RID: 660
			private uint _totalSectors;

			// Token: 0x04000295 RID: 661
			private string _type;

			// Token: 0x04000296 RID: 662
			private string _id;

			// Token: 0x04000297 RID: 663
			private string _name;

			// Token: 0x04000298 RID: 664
			private FullFlashUpdateImage.FullFlashUpdateStore _store;

			// Token: 0x04000299 RID: 665
			private bool _useAllSpace;

			// Token: 0x0400029A RID: 666
			private string _fileSystem;

			// Token: 0x0400029B RID: 667
			private uint _byteAlignment;

			// Token: 0x0400029C RID: 668
			private uint _clusterSize;
		}

		// Token: 0x02000075 RID: 117
		public class FullFlashUpdateStore : IDisposable
		{
			// Token: 0x06000359 RID: 857 RVA: 0x00014EE4 File Offset: 0x000130E4
			~FullFlashUpdateStore()
			{
				this.Dispose(false);
			}

			// Token: 0x0600035A RID: 858 RVA: 0x00014F14 File Offset: 0x00013114
			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			// Token: 0x0600035B RID: 859 RVA: 0x00014F24 File Offset: 0x00013124
			protected virtual void Dispose(bool isDisposing)
			{
				if (this._alreadyDisposed)
				{
					return;
				}
				if (isDisposing)
				{
					this._partitions = null;
				}
				if (File.Exists(this._tempBackingStoreFile))
				{
					try
					{
						File.Delete(this._tempBackingStoreFile);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Warning: ImageCommon!Dispose: Failed to delete temporary backing store '" + this._tempBackingStoreFile + "' with exception: " + ex.Message);
					}
				}
				if (Directory.Exists(this._tempBackingStorePath))
				{
					try
					{
						Directory.Delete(this._tempBackingStorePath, true);
					}
					catch (Exception ex2)
					{
						Console.WriteLine("Warning: ImageCommon!Dispose: Failed to delete temporary backing store directory '" + this._tempBackingStorePath + "' with exception: " + ex2.Message);
					}
				}
				this._alreadyDisposed = true;
			}

			// Token: 0x17000095 RID: 149
			// (get) Token: 0x0600035C RID: 860 RVA: 0x00014FE4 File Offset: 0x000131E4
			public FullFlashUpdateImage Image
			{
				get
				{
					return this._image;
				}
			}

			// Token: 0x17000096 RID: 150
			// (get) Token: 0x0600035D RID: 861 RVA: 0x00014FEC File Offset: 0x000131EC
			public string Id
			{
				get
				{
					return this._storeId;
				}
			}

			// Token: 0x17000097 RID: 151
			// (get) Token: 0x0600035E RID: 862 RVA: 0x00014FF4 File Offset: 0x000131F4
			public bool IsMainOSStore
			{
				get
				{
					return this._isMainOSStore;
				}
			}

			// Token: 0x17000098 RID: 152
			// (get) Token: 0x0600035F RID: 863 RVA: 0x00014FFC File Offset: 0x000131FC
			public string DevicePath
			{
				get
				{
					return this._devicePath;
				}
			}

			// Token: 0x17000099 RID: 153
			// (get) Token: 0x06000360 RID: 864 RVA: 0x00015004 File Offset: 0x00013204
			public bool OnlyAllocateDefinedGptEntries
			{
				get
				{
					return this._onlyAllocateDefinedGptEntries;
				}
			}

			// Token: 0x1700009A RID: 154
			// (get) Token: 0x06000361 RID: 865 RVA: 0x0001500C File Offset: 0x0001320C
			// (set) Token: 0x06000362 RID: 866 RVA: 0x00015014 File Offset: 0x00013214
			public uint SectorCount
			{
				get
				{
					return this._minSectorCount;
				}
				set
				{
					this._minSectorCount = value;
				}
			}

			// Token: 0x1700009B RID: 155
			// (get) Token: 0x06000363 RID: 867 RVA: 0x0001500C File Offset: 0x0001320C
			// (set) Token: 0x06000364 RID: 868 RVA: 0x00015014 File Offset: 0x00013214
			public uint MinSectorCount
			{
				get
				{
					return this._minSectorCount;
				}
				set
				{
					this._minSectorCount = value;
				}
			}

			// Token: 0x1700009C RID: 156
			// (get) Token: 0x06000365 RID: 869 RVA: 0x0001501D File Offset: 0x0001321D
			// (set) Token: 0x06000366 RID: 870 RVA: 0x00015025 File Offset: 0x00013225
			public uint SectorSize
			{
				get
				{
					return this._sectorSize;
				}
				set
				{
					this._sectorSize = value;
				}
			}

			// Token: 0x1700009D RID: 157
			// (get) Token: 0x06000367 RID: 871 RVA: 0x0001502E File Offset: 0x0001322E
			public int PartitionCount
			{
				get
				{
					return this._partitions.Count;
				}
			}

			// Token: 0x1700009E RID: 158
			// (get) Token: 0x06000368 RID: 872 RVA: 0x0001503B File Offset: 0x0001323B
			public List<FullFlashUpdateImage.FullFlashUpdatePartition> Partitions
			{
				get
				{
					return this._partitions;
				}
			}

			// Token: 0x1700009F RID: 159
			public FullFlashUpdateImage.FullFlashUpdatePartition this[string name]
			{
				get
				{
					foreach (FullFlashUpdateImage.FullFlashUpdatePartition fullFlashUpdatePartition in this._partitions)
					{
						if (string.CompareOrdinal(fullFlashUpdatePartition.Name, name) == 0)
						{
							return fullFlashUpdatePartition;
						}
					}
					return null;
				}
			}

			// Token: 0x0600036A RID: 874 RVA: 0x000150A8 File Offset: 0x000132A8
			public void Initialize(FullFlashUpdateImage image, string storeId, bool isMainOSStore, string devicePath, bool onlyAllocateDefinedGptEntries, uint minSectorCount, uint sectorSize)
			{
				this._tempBackingStorePath = BuildPaths.GetImagingTempPath(Directory.GetCurrentDirectory());
				Directory.CreateDirectory(this._tempBackingStorePath);
				this._tempBackingStoreFile = FileUtils.GetTempFile(this._tempBackingStorePath) + "FFUBackingStore";
				this._image = image;
				this._storeId = storeId;
				this._isMainOSStore = isMainOSStore;
				this._devicePath = devicePath;
				this._onlyAllocateDefinedGptEntries = onlyAllocateDefinedGptEntries;
				this._minSectorCount = minSectorCount;
				this._sectorSize = sectorSize;
				this._sectorsUsed = 0U;
			}

			// Token: 0x0600036B RID: 875 RVA: 0x00015128 File Offset: 0x00013328
			public void AddPartition(FullFlashUpdateImage.FullFlashUpdatePartition partition)
			{
				if (this[partition.Name] != null)
				{
					throw new ImageCommonException("ImageCommon!FullFlashUpdateStore::AddPartition: Two partitions in a store have the same name (" + partition.Name + ").");
				}
				if (this._isMainOSStore)
				{
					if (this._minSectorCount != 0U && partition.TotalSectors > this._minSectorCount)
					{
						throw new ImageCommonException("ImageCommon!FullFlashUpdateStore::AddPartition: The partition " + partition.Name + " is too large for the store.");
					}
					if (partition.UseAllSpace)
					{
						using (List<FullFlashUpdateImage.FullFlashUpdatePartition>.Enumerator enumerator = this._partitions.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (enumerator.Current.UseAllSpace)
								{
									throw new ImageCommonException("ImageCommon!FullFlashUpdateStore::AddPartition: Two partitions in the same store have the UseAllSpace flag set.");
								}
							}
							goto IL_109;
						}
					}
					if (partition.SectorsInUse > partition.TotalSectors)
					{
						throw new ImageCommonException(string.Concat(new object[]
						{
							"ImageCommon!FullFlashUpdateStore::AddPartition: The partition data is invalid.  There are more used sectors (",
							partition.SectorsInUse,
							") than total sectors (",
							partition.TotalSectors,
							") for partition:",
							partition.Name
						}));
					}
					IL_109:
					if (this._minSectorCount != 0U)
					{
						if (partition.UseAllSpace)
						{
							this._sectorsUsed += 1U;
						}
						else
						{
							this._sectorsUsed += partition.TotalSectors;
						}
						if (this._sectorsUsed > this._minSectorCount)
						{
							throw new ImageCommonException(string.Concat(new object[]
							{
								"ImageCommon!FullFlashUpdateStore::AddPartition: Partition (",
								partition.Name,
								") on the Store does not fit. SectorsUsed = ",
								this._sectorsUsed,
								" > MinSectorCount = ",
								this._minSectorCount
							}));
						}
					}
				}
				this._partitions.Add(partition);
			}

			// Token: 0x0600036C RID: 876 RVA: 0x000152EC File Offset: 0x000134EC
			internal void AddPartition(FullFlashUpdateImage.ManifestCategory category)
			{
				uint usedSectors = 0U;
				uint num = 0U;
				string partitionType = category["Type"];
				string text = category["Name"];
				string partitionId = category["Id"];
				bool flag = false;
				if (this._isMainOSStore)
				{
					if (category["UsedSectors"] != null)
					{
						usedSectors = uint.Parse(category["UsedSectors"], CultureInfo.InvariantCulture);
					}
					if (category["TotalSectors"] != null)
					{
						num = uint.Parse(category["TotalSectors"], CultureInfo.InvariantCulture);
					}
					if (category["UseAllSpace"] != null)
					{
						flag = bool.Parse(category["UseAllSpace"]);
					}
					if (!flag && num == 0U)
					{
						throw new ImageCommonException(string.Format("ImageCommon!FullFlashUpdateImage::AddPartition: The partition category for partition {0} must contain either a 'TotalSectors' or 'UseAllSpace' key/value pair.", text));
					}
					if (flag && num > 0U)
					{
						throw new ImageCommonException(string.Format("ImageCommon!FullFlashUpdateImage::AddPartition: The partition category for partition {0} cannot contain both a 'TotalSectors' and a 'UseAllSpace' key/value pair.", text));
					}
				}
				FullFlashUpdateImage.FullFlashUpdatePartition fullFlashUpdatePartition = new FullFlashUpdateImage.FullFlashUpdatePartition();
				fullFlashUpdatePartition.Initialize(usedSectors, num, partitionType, partitionId, text, this, flag);
				if (category["Hidden"] != null)
				{
					fullFlashUpdatePartition.Hidden = bool.Parse(category["Hidden"]);
				}
				if (category["AttachDriveLetter"] != null)
				{
					fullFlashUpdatePartition.AttachDriveLetter = bool.Parse(category["AttachDriveLetter"]);
				}
				if (category["ReadOnly"] != null)
				{
					fullFlashUpdatePartition.ReadOnly = bool.Parse(category["ReadOnly"]);
				}
				if (category["Bootable"] != null)
				{
					fullFlashUpdatePartition.Bootable = bool.Parse(category["Bootable"]);
				}
				if (category["FileSystem"] != null)
				{
					fullFlashUpdatePartition.FileSystem = category["FileSystem"];
				}
				fullFlashUpdatePartition.PrimaryPartition = category["Primary"];
				if (category["ByteAlignment"] != null)
				{
					fullFlashUpdatePartition.ByteAlignment = uint.Parse(category["ByteAlignment"], CultureInfo.InvariantCulture);
				}
				if (category["ClusterSize"] != null)
				{
					fullFlashUpdatePartition.ClusterSize = uint.Parse(category["ClusterSize"], CultureInfo.InvariantCulture);
				}
				if (category["SectorAlignment"] != null)
				{
					fullFlashUpdatePartition.SectorAlignment = uint.Parse(category["SectorAlignment"], CultureInfo.InvariantCulture);
				}
				if (category["RequiredToFlash"] != null)
				{
					fullFlashUpdatePartition.RequiredToFlash = bool.Parse(category["RequiredToFlash"]);
				}
				this.AddPartition(fullFlashUpdatePartition);
			}

			// Token: 0x0600036D RID: 877 RVA: 0x00015548 File Offset: 0x00013748
			public void TransferLocation(Stream sourceStream, Stream destinationStream)
			{
				byte[] array = new byte[1048576];
				int num2;
				for (long num = sourceStream.Length - sourceStream.Position; num > 0L; num -= (long)num2)
				{
					num2 = (int)Math.Min(num, (long)array.Length);
					sourceStream.Read(array, 0, num2);
					destinationStream.Write(array, 0, num2);
				}
			}

			// Token: 0x0600036E RID: 878 RVA: 0x0001559C File Offset: 0x0001379C
			public void ToCategory(FullFlashUpdateImage.ManifestCategory category)
			{
				category["SectorSize"] = this._sectorSize.ToString(CultureInfo.InvariantCulture);
				if (this._minSectorCount != 0U)
				{
					category["MinSectorCount"] = this._minSectorCount.ToString(CultureInfo.InvariantCulture);
				}
				if (!string.IsNullOrEmpty(this._storeId))
				{
					category["StoreId"] = this._storeId;
				}
				category["IsMainOSStore"] = this._isMainOSStore.ToString(CultureInfo.InvariantCulture);
				if (!string.IsNullOrEmpty(this._devicePath))
				{
					category["DevicePath"] = this._devicePath;
				}
				category["OnlyAllocateDefinedGptEntries"] = this._onlyAllocateDefinedGptEntries.ToString(CultureInfo.InvariantCulture);
			}

			// Token: 0x170000A0 RID: 160
			// (get) Token: 0x0600036F RID: 879 RVA: 0x00015659 File Offset: 0x00013859
			public string BackingFile
			{
				get
				{
					return this._tempBackingStoreFile;
				}
			}

			// Token: 0x040002A4 RID: 676
			private List<FullFlashUpdateImage.FullFlashUpdatePartition> _partitions = new List<FullFlashUpdateImage.FullFlashUpdatePartition>();

			// Token: 0x040002A5 RID: 677
			private FullFlashUpdateImage _image;

			// Token: 0x040002A6 RID: 678
			private string _storeId;

			// Token: 0x040002A7 RID: 679
			private bool _isMainOSStore;

			// Token: 0x040002A8 RID: 680
			private string _devicePath;

			// Token: 0x040002A9 RID: 681
			private bool _onlyAllocateDefinedGptEntries;

			// Token: 0x040002AA RID: 682
			private uint _minSectorCount;

			// Token: 0x040002AB RID: 683
			private uint _sectorSize;

			// Token: 0x040002AC RID: 684
			private uint _sectorsUsed;

			// Token: 0x040002AD RID: 685
			private string _tempBackingStoreFile = string.Empty;

			// Token: 0x040002AE RID: 686
			private string _tempBackingStorePath = string.Empty;

			// Token: 0x040002AF RID: 687
			private bool _alreadyDisposed;
		}

		// Token: 0x02000076 RID: 118
		public class ManifestCategory
		{
			// Token: 0x170000A1 RID: 161
			public string this[string name]
			{
				get
				{
					return (string)this._keyValues[name];
				}
				set
				{
					if (this._keyValues.ContainsKey(name))
					{
						this._keyValues[name] = value;
						return;
					}
					if (name.Length > this._maxKeySize)
					{
						this._maxKeySize = name.Length;
					}
					this._keyValues.Add(name, value);
				}
			}

			// Token: 0x170000A2 RID: 162
			// (get) Token: 0x06000373 RID: 883 RVA: 0x000156F0 File Offset: 0x000138F0
			// (set) Token: 0x06000374 RID: 884 RVA: 0x000156F8 File Offset: 0x000138F8
			public string Category
			{
				get
				{
					return this._category;
				}
				set
				{
					this._category = value;
				}
			}

			// Token: 0x170000A3 RID: 163
			// (get) Token: 0x06000375 RID: 885 RVA: 0x00015701 File Offset: 0x00013901
			public string Name
			{
				get
				{
					return this._name;
				}
			}

			// Token: 0x06000376 RID: 886 RVA: 0x00015709 File Offset: 0x00013909
			public void RemoveNameValue(string name)
			{
				if (this._keyValues.ContainsKey(name))
				{
					this._keyValues.Remove(name);
				}
			}

			// Token: 0x06000377 RID: 887 RVA: 0x00015725 File Offset: 0x00013925
			public ManifestCategory(string name)
			{
				this._name = name;
			}

			// Token: 0x06000378 RID: 888 RVA: 0x00015755 File Offset: 0x00013955
			public ManifestCategory(string name, string categoryValue)
			{
				this._name = name;
				this._category = categoryValue;
			}

			// Token: 0x06000379 RID: 889 RVA: 0x0001578C File Offset: 0x0001398C
			public void WriteToStream(Stream targetStream)
			{
				ASCIIEncoding asciiencoding = new ASCIIEncoding();
				byte[] bytes = asciiencoding.GetBytes("[" + this._category + "]\r\n");
				targetStream.Write(bytes, 0, bytes.Count<byte>());
				foreach (object obj in this._keyValues)
				{
					string text = ((DictionaryEntry)obj).Key as string;
					bytes = asciiencoding.GetBytes(text);
					targetStream.Write(bytes, 0, bytes.Count<byte>());
					for (int i = 0; i < this._maxKeySize + 1 - text.Length; i++)
					{
						targetStream.Write(asciiencoding.GetBytes(" "), 0, 1);
					}
					bytes = asciiencoding.GetBytes("= " + this._keyValues[text] + "\r\n");
					targetStream.Write(bytes, 0, bytes.Count<byte>());
				}
				targetStream.Write(asciiencoding.GetBytes("\r\n"), 0, 2);
			}

			// Token: 0x0600037A RID: 890 RVA: 0x000158B4 File Offset: 0x00013AB4
			public void WriteToFile(TextWriter targetStream)
			{
				targetStream.WriteLine("[{0}]", this._category);
				foreach (object obj in this._keyValues)
				{
					string text = ((DictionaryEntry)obj).Key as string;
					targetStream.Write("{0}", text);
					for (int i = 0; i < this._maxKeySize + 1 - text.Length; i++)
					{
						targetStream.Write(" ");
					}
					targetStream.WriteLine("= {0}", this._keyValues[text]);
				}
				targetStream.WriteLine("");
			}

			// Token: 0x0600037B RID: 891 RVA: 0x0001597C File Offset: 0x00013B7C
			public void Clean()
			{
				this._keyValues.Clear();
			}

			// Token: 0x040002B0 RID: 688
			private string _name = string.Empty;

			// Token: 0x040002B1 RID: 689
			private string _category = string.Empty;

			// Token: 0x040002B2 RID: 690
			private int _maxKeySize;

			// Token: 0x040002B3 RID: 691
			private Hashtable _keyValues = new Hashtable();
		}

		// Token: 0x02000077 RID: 119
		public class FullFlashUpdateManifest
		{
			// Token: 0x0600037C RID: 892 RVA: 0x00015989 File Offset: 0x00013B89
			public FullFlashUpdateManifest(FullFlashUpdateImage image)
			{
				this._image = image;
			}

			// Token: 0x0600037D RID: 893 RVA: 0x000159A8 File Offset: 0x00013BA8
			public FullFlashUpdateManifest(FullFlashUpdateImage image, StreamReader manifestStream)
			{
				Regex regex = new Regex("^\\s*\\[(?<category>[^\\]]+)\\]\\s*$");
				Regex regex2 = new Regex("^\\s*(?<key>[^=\\s]+)\\s*=\\s*(?<value>.*)(\\s*$)");
				Match match = null;
				this._image = image;
				FullFlashUpdateImage.ManifestCategory manifestCategory = null;
				while (!manifestStream.EndOfStream)
				{
					string text = manifestStream.ReadLine();
					if (regex.IsMatch(text))
					{
						match = null;
						string value = regex.Match(text).Groups["category"].Value;
						this.ProcessCategory(manifestCategory);
						if (string.Compare(value, "Store", StringComparison.Ordinal) == 0)
						{
							manifestCategory = new FullFlashUpdateImage.ManifestCategory("Store", "Store");
						}
						else if (string.Compare(value, "Partition", StringComparison.Ordinal) == 0)
						{
							manifestCategory = new FullFlashUpdateImage.ManifestCategory("Partition", "Partition");
						}
						else
						{
							string text2 = value;
							manifestCategory = this.AddCategory(text2, text2);
						}
					}
					else if (manifestCategory != null && regex2.IsMatch(text))
					{
						match = null;
						Match match2 = regex2.Match(text);
						manifestCategory[match2.Groups["key"].Value] = match2.Groups["value"].Value;
						if (match2.Groups["key"].ToString() == "Description")
						{
							match = match2;
						}
					}
					else if (match != null)
					{
						FullFlashUpdateImage.ManifestCategory manifestCategory2 = manifestCategory;
						string value2 = match.Groups["key"].Value;
						manifestCategory2[value2] = manifestCategory2[value2] + Environment.NewLine + text;
					}
				}
				this.ProcessCategory(manifestCategory);
			}

			// Token: 0x0600037E RID: 894 RVA: 0x00015B44 File Offset: 0x00013D44
			private void ProcessCategory(FullFlashUpdateImage.ManifestCategory category)
			{
				if (category != null)
				{
					if (string.CompareOrdinal(category.Name, "Store") == 0)
					{
						this._image.AddStore(category);
						category = null;
						return;
					}
					if (string.CompareOrdinal(category.Name, "Partition") == 0)
					{
						this._image.Stores.Last<FullFlashUpdateImage.FullFlashUpdateStore>().AddPartition(category);
						category = null;
					}
				}
			}

			// Token: 0x0600037F RID: 895 RVA: 0x00015BA4 File Offset: 0x00013DA4
			public FullFlashUpdateImage.ManifestCategory AddCategory(string name, string categoryValue)
			{
				if (this[name] != null)
				{
					throw new ImageCommonException("ImageCommon!FullFlashUpdateManifest::AddCategory: Cannot add duplicate categories to a manifest.");
				}
				FullFlashUpdateImage.ManifestCategory manifestCategory = new FullFlashUpdateImage.ManifestCategory(name, categoryValue);
				this._categories.Add(manifestCategory);
				return manifestCategory;
			}

			// Token: 0x06000380 RID: 896 RVA: 0x00015BDC File Offset: 0x00013DDC
			public void RemoveCategory(string name)
			{
				if (this[name] == null)
				{
					return;
				}
				FullFlashUpdateImage.ManifestCategory obj = this[name];
				this._categories.Remove(obj);
			}

			// Token: 0x170000A4 RID: 164
			public FullFlashUpdateImage.ManifestCategory this[string categoryName]
			{
				get
				{
					foreach (object obj in this._categories)
					{
						FullFlashUpdateImage.ManifestCategory manifestCategory = (FullFlashUpdateImage.ManifestCategory)obj;
						if (string.Compare(manifestCategory.Name, categoryName, StringComparison.Ordinal) == 0)
						{
							return manifestCategory;
						}
					}
					return null;
				}
			}

			// Token: 0x06000382 RID: 898 RVA: 0x00015C70 File Offset: 0x00013E70
			public void WriteToStream(Stream targetStream)
			{
				foreach (object obj in this._categories)
				{
					((FullFlashUpdateImage.ManifestCategory)obj).WriteToStream(targetStream);
				}
				foreach (FullFlashUpdateImage.FullFlashUpdateStore fullFlashUpdateStore in this._image.Stores)
				{
					FullFlashUpdateImage.ManifestCategory manifestCategory = new FullFlashUpdateImage.ManifestCategory("Store", "Store");
					fullFlashUpdateStore.ToCategory(manifestCategory);
					manifestCategory.WriteToStream(targetStream);
					foreach (FullFlashUpdateImage.FullFlashUpdatePartition fullFlashUpdatePartition in fullFlashUpdateStore.Partitions)
					{
						FullFlashUpdateImage.ManifestCategory manifestCategory2 = new FullFlashUpdateImage.ManifestCategory("Partition", "Partition");
						fullFlashUpdatePartition.ToCategory(manifestCategory2);
						manifestCategory2.WriteToStream(targetStream);
					}
				}
			}

			// Token: 0x06000383 RID: 899 RVA: 0x00015D80 File Offset: 0x00013F80
			public void WriteToFile(string fileName)
			{
				try
				{
					if (File.Exists(fileName))
					{
						File.Delete(fileName);
					}
				}
				catch (Exception innerException)
				{
					throw new ImageCommonException("ImageCommon!FullFlashUpdateManifest::WriteToFile: Unable to delete the existing image file", innerException);
				}
				StreamWriter streamWriter = File.CreateText(fileName);
				this.WriteToStream(streamWriter.BaseStream);
				streamWriter.Close();
			}

			// Token: 0x170000A5 RID: 165
			// (get) Token: 0x06000384 RID: 900 RVA: 0x00015DD4 File Offset: 0x00013FD4
			public uint Length
			{
				get
				{
					MemoryStream memoryStream = new MemoryStream();
					this.WriteToStream(memoryStream);
					return (uint)memoryStream.Position;
				}
			}

			// Token: 0x040002B4 RID: 692
			private ArrayList _categories = new ArrayList(20);

			// Token: 0x040002B5 RID: 693
			private FullFlashUpdateImage _image;
		}
	}
}
