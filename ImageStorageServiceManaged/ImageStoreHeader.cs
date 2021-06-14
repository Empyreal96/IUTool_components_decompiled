using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000030 RID: 48
	public class ImageStoreHeader
	{
		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060001B8 RID: 440 RVA: 0x000085F2 File Offset: 0x000067F2
		// (set) Token: 0x060001B9 RID: 441 RVA: 0x000085FA File Offset: 0x000067FA
		[StructVersion(Version = 1)]
		public FullFlashUpdateType UpdateType { get; set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060001BA RID: 442 RVA: 0x00008603 File Offset: 0x00006803
		// (set) Token: 0x060001BB RID: 443 RVA: 0x0000860B File Offset: 0x0000680B
		[StructVersion(Version = 1)]
		public ushort MajorVersion { get; set; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060001BC RID: 444 RVA: 0x00008614 File Offset: 0x00006814
		// (set) Token: 0x060001BD RID: 445 RVA: 0x0000861C File Offset: 0x0000681C
		[StructVersion(Version = 1)]
		public ushort MinorVersion { get; set; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060001BE RID: 446 RVA: 0x00008625 File Offset: 0x00006825
		// (set) Token: 0x060001BF RID: 447 RVA: 0x0000862D File Offset: 0x0000682D
		[StructVersion(Version = 1)]
		public ushort FullFlashMajorVersion { get; set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060001C0 RID: 448 RVA: 0x00008636 File Offset: 0x00006836
		// (set) Token: 0x060001C1 RID: 449 RVA: 0x0000863E File Offset: 0x0000683E
		[StructVersion(Version = 1)]
		public ushort FullFlashMinorVersion { get; set; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060001C2 RID: 450 RVA: 0x00008647 File Offset: 0x00006847
		// (set) Token: 0x060001C3 RID: 451 RVA: 0x0000864F File Offset: 0x0000684F
		[StructVersion(Version = 1)]
		public byte[] PlatformIdentifier
		{
			get
			{
				return this._platformId;
			}
			set
			{
				this._platformId = value;
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060001C4 RID: 452 RVA: 0x00008658 File Offset: 0x00006858
		// (set) Token: 0x060001C5 RID: 453 RVA: 0x00008660 File Offset: 0x00006860
		[StructVersion(Version = 1)]
		public uint BytesPerBlock { get; set; }

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060001C6 RID: 454 RVA: 0x00008669 File Offset: 0x00006869
		// (set) Token: 0x060001C7 RID: 455 RVA: 0x00008671 File Offset: 0x00006871
		[StructVersion(Version = 1)]
		public uint StoreDataEntryCount { get; set; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060001C8 RID: 456 RVA: 0x0000867A File Offset: 0x0000687A
		// (set) Token: 0x060001C9 RID: 457 RVA: 0x00008682 File Offset: 0x00006882
		[StructVersion(Version = 1)]
		public uint StoreDataSizeInBytes { get; set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060001CA RID: 458 RVA: 0x0000868B File Offset: 0x0000688B
		// (set) Token: 0x060001CB RID: 459 RVA: 0x00008693 File Offset: 0x00006893
		[StructVersion(Version = 1)]
		public uint ValidationEntryCount { get; set; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060001CC RID: 460 RVA: 0x0000869C File Offset: 0x0000689C
		// (set) Token: 0x060001CD RID: 461 RVA: 0x000086A4 File Offset: 0x000068A4
		[StructVersion(Version = 1)]
		public uint ValidationDataSizeInBytes { get; set; }

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060001CE RID: 462 RVA: 0x000086AD File Offset: 0x000068AD
		// (set) Token: 0x060001CF RID: 463 RVA: 0x000086B5 File Offset: 0x000068B5
		[StructVersion(Version = 1)]
		public uint InitialPartitionTableBlockIndex { get; set; }

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060001D0 RID: 464 RVA: 0x000086BE File Offset: 0x000068BE
		// (set) Token: 0x060001D1 RID: 465 RVA: 0x000086C6 File Offset: 0x000068C6
		[StructVersion(Version = 1)]
		public uint InitialPartitionTableBlockCount { get; set; }

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060001D2 RID: 466 RVA: 0x000086CF File Offset: 0x000068CF
		// (set) Token: 0x060001D3 RID: 467 RVA: 0x000086D7 File Offset: 0x000068D7
		[StructVersion(Version = 1)]
		public uint FlashOnlyPartitionTableBlockIndex { get; set; }

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060001D4 RID: 468 RVA: 0x000086E0 File Offset: 0x000068E0
		// (set) Token: 0x060001D5 RID: 469 RVA: 0x000086E8 File Offset: 0x000068E8
		[StructVersion(Version = 1)]
		public uint FlashOnlyPartitionTableBlockCount { get; set; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060001D6 RID: 470 RVA: 0x000086F1 File Offset: 0x000068F1
		// (set) Token: 0x060001D7 RID: 471 RVA: 0x000086F9 File Offset: 0x000068F9
		[StructVersion(Version = 1)]
		public uint FinalPartitionTableBlockIndex { get; set; }

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x00008702 File Offset: 0x00006902
		// (set) Token: 0x060001D9 RID: 473 RVA: 0x0000870A File Offset: 0x0000690A
		[StructVersion(Version = 1)]
		public uint FinalPartitionTableBlockCount { get; set; }

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060001DA RID: 474 RVA: 0x00008713 File Offset: 0x00006913
		// (set) Token: 0x060001DB RID: 475 RVA: 0x0000872F File Offset: 0x0000692F
		[StructVersion(Version = 2)]
		public ushort NumberOfStores
		{
			get
			{
				if (this.MajorVersion < 2)
				{
					throw new NotImplementedException("NumberOfStores");
				}
				return this._numberOfStores;
			}
			set
			{
				this._numberOfStores = value;
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060001DC RID: 476 RVA: 0x00008738 File Offset: 0x00006938
		// (set) Token: 0x060001DD RID: 477 RVA: 0x00008754 File Offset: 0x00006954
		[StructVersion(Version = 2)]
		public ushort StoreIndex
		{
			get
			{
				if (this.MajorVersion < 2)
				{
					throw new NotImplementedException("StoreIndex");
				}
				return this._storeIndex;
			}
			set
			{
				this._storeIndex = value;
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060001DE RID: 478 RVA: 0x0000875D File Offset: 0x0000695D
		// (set) Token: 0x060001DF RID: 479 RVA: 0x00008779 File Offset: 0x00006979
		[StructVersion(Version = 2)]
		public ulong StorePayloadSize
		{
			get
			{
				if (this.MajorVersion < 2)
				{
					throw new NotImplementedException("StorePayloadSize");
				}
				return this._storePayloadSize;
			}
			set
			{
				this._storePayloadSize = value;
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060001E0 RID: 480 RVA: 0x00008782 File Offset: 0x00006982
		// (set) Token: 0x060001E1 RID: 481 RVA: 0x0000879E File Offset: 0x0000699E
		[StructVersion(Version = 2)]
		public ushort DevicePathLength
		{
			get
			{
				if (this.MajorVersion < 2)
				{
					throw new NotImplementedException("DevicePathLength");
				}
				return this._devicePathLength;
			}
			set
			{
				this._devicePathLength = value;
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060001E2 RID: 482 RVA: 0x000087A7 File Offset: 0x000069A7
		// (set) Token: 0x060001E3 RID: 483 RVA: 0x000087C3 File Offset: 0x000069C3
		[StructVersion(Version = 2)]
		public byte[] DevicePath
		{
			get
			{
				if (this.MajorVersion < 2)
				{
					throw new NotImplementedException("DevicePath");
				}
				return this._devicePath;
			}
			set
			{
				this._devicePath = value;
			}
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x000087CC File Offset: 0x000069CC
		private void SetFullFlashVersion(FullFlashUpdateImage fullFlashImage)
		{
			Regex regex = new Regex("(?<MajorVersion>\\d+)\\.(?<MinorVersion>\\d+)");
			if (!regex.IsMatch(fullFlashImage.Version))
			{
				throw new ImageStorageException(string.Format("{0}: The full flash update version isn't valid. '{1}'", MethodBase.GetCurrentMethod().Name, fullFlashImage.Version));
			}
			Match match = regex.Match(fullFlashImage.Version);
			try
			{
				ushort fullFlashMajorVersion = ushort.Parse(match.Groups["MajorVersion"].Value);
				ushort fullFlashMinorVersion = ushort.Parse(match.Groups["MinorVersion"].Value);
				this.FullFlashMajorVersion = fullFlashMajorVersion;
				this.FullFlashMinorVersion = fullFlashMinorVersion;
			}
			catch (Exception innerException)
			{
				throw new ImageStorageException(string.Format("{0}: The full flash image version number is invalid. '{1}'", MethodBase.GetCurrentMethod().Name, fullFlashImage.Version), innerException);
			}
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x00008898 File Offset: 0x00006A98
		public void Initialize(FullFlashUpdateType updateType, uint blockSizeInBytes, FullFlashUpdateImage fullFlashImage)
		{
			this.BytesPerBlock = blockSizeInBytes;
			this.UpdateType = updateType;
			this.MajorVersion = 1;
			int num = 0;
			foreach (string s in fullFlashImage.DevicePlatformIDs)
			{
				byte[] bytes = Encoding.ASCII.GetBytes(s);
				int num2 = bytes.Length + 1;
				if (num + num2 > ImageStoreHeader.PlatformIdSizeInBytes - 1)
				{
					throw new ImageStorageException(string.Format("{0}: The platform ID group is too large.", MethodBase.GetCurrentMethod().Name));
				}
				bytes.CopyTo(this.PlatformIdentifier, num);
				num += num2;
			}
			this.SetFullFlashVersion(fullFlashImage);
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000894C File Offset: 0x00006B4C
		public void Initialize2(FullFlashUpdateType updateType, uint blockSizeInBytes, FullFlashUpdateImage fullFlashImage, ushort numberOfStores, ushort storeIndex, string devicePath)
		{
			this.Initialize(updateType, blockSizeInBytes, fullFlashImage);
			this.MajorVersion = 2;
			this.NumberOfStores = numberOfStores;
			this.StoreIndex = storeIndex;
			this.DevicePathLength = (ushort)devicePath.Length;
			UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
			this.DevicePath = unicodeEncoding.GetBytes(devicePath.ToCharArray());
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x000089A0 File Offset: 0x00006BA0
		public void WriteToStream(Stream stream)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			long position = stream.Position;
			binaryWriter.Write((uint)this.UpdateType);
			binaryWriter.Write(this.MajorVersion);
			binaryWriter.Write(this.MinorVersion);
			binaryWriter.Write(this.FullFlashMajorVersion);
			binaryWriter.Write(this.FullFlashMinorVersion);
			binaryWriter.Write(this.PlatformIdentifier);
			binaryWriter.Write(this.BytesPerBlock);
			binaryWriter.Write(this.StoreDataEntryCount);
			binaryWriter.Write(this.StoreDataSizeInBytes);
			binaryWriter.Write(this.ValidationEntryCount);
			binaryWriter.Write(this.ValidationDataSizeInBytes);
			binaryWriter.Write(this.InitialPartitionTableBlockIndex);
			binaryWriter.Write(this.InitialPartitionTableBlockCount);
			binaryWriter.Write(this.FlashOnlyPartitionTableBlockIndex);
			binaryWriter.Write(this.FlashOnlyPartitionTableBlockCount);
			binaryWriter.Write(this.FinalPartitionTableBlockIndex);
			binaryWriter.Write(this.FinalPartitionTableBlockCount);
			if (this.MajorVersion >= 2)
			{
				binaryWriter.Write(this.NumberOfStores);
				binaryWriter.Write(this.StoreIndex);
				binaryWriter.Write(this.StorePayloadSize);
				binaryWriter.Write(this.DevicePathLength);
				binaryWriter.Write(this.DevicePath);
			}
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x00008AD0 File Offset: 0x00006CD0
		public static ImageStoreHeader ReadFromStream(Stream stream)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			long position = stream.Position;
			ImageStoreHeader imageStoreHeader = new ImageStoreHeader();
			imageStoreHeader.UpdateType = (FullFlashUpdateType)binaryReader.ReadUInt32();
			imageStoreHeader.MajorVersion = binaryReader.ReadUInt16();
			imageStoreHeader.MinorVersion = binaryReader.ReadUInt16();
			imageStoreHeader.FullFlashMajorVersion = binaryReader.ReadUInt16();
			imageStoreHeader.FullFlashMinorVersion = binaryReader.ReadUInt16();
			imageStoreHeader.PlatformIdentifier = binaryReader.ReadBytes(ImageStoreHeader.PlatformIdSizeInBytes);
			imageStoreHeader.BytesPerBlock = binaryReader.ReadUInt32();
			imageStoreHeader.StoreDataEntryCount = binaryReader.ReadUInt32();
			imageStoreHeader.StoreDataSizeInBytes = binaryReader.ReadUInt32();
			imageStoreHeader.ValidationEntryCount = binaryReader.ReadUInt32();
			imageStoreHeader.ValidationDataSizeInBytes = binaryReader.ReadUInt32();
			imageStoreHeader.InitialPartitionTableBlockIndex = binaryReader.ReadUInt32();
			imageStoreHeader.InitialPartitionTableBlockCount = binaryReader.ReadUInt32();
			imageStoreHeader.FlashOnlyPartitionTableBlockIndex = binaryReader.ReadUInt32();
			imageStoreHeader.FlashOnlyPartitionTableBlockCount = binaryReader.ReadUInt32();
			imageStoreHeader.FinalPartitionTableBlockIndex = binaryReader.ReadUInt32();
			imageStoreHeader.FinalPartitionTableBlockCount = binaryReader.ReadUInt32();
			if (imageStoreHeader.MajorVersion >= 2)
			{
				imageStoreHeader.NumberOfStores = binaryReader.ReadUInt16();
				imageStoreHeader.StoreIndex = binaryReader.ReadUInt16();
				imageStoreHeader.StorePayloadSize = binaryReader.ReadUInt64();
				imageStoreHeader.DevicePathLength = binaryReader.ReadUInt16();
				imageStoreHeader.DevicePath = binaryReader.ReadBytes((int)(imageStoreHeader.DevicePathLength * 2));
			}
			return imageStoreHeader;
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x00008C12 File Offset: 0x00006E12
		public void LogInfo(IULogger logger)
		{
			this.LogInfo(logger, 0);
		}

		// Token: 0x060001EA RID: 490 RVA: 0x00008C1C File Offset: 0x00006E1C
		public void LogInfo(IULogger logger, int indentLevel)
		{
			string @string = Encoding.ASCII.GetString(this.PlatformIdentifier);
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			logger.LogInfo(str + "Image Store Payload Header", new object[0]);
			checked
			{
				indentLevel += 2;
				str = new StringBuilder().Append(' ', indentLevel).ToString();
				logger.LogInfo(str + "UpdateType                       : {0}", new object[]
				{
					this.UpdateType
				});
				logger.LogInfo(str + "MajorVersion                     : 0x{0:x}", new object[]
				{
					this.MajorVersion
				});
				logger.LogInfo(str + "MinorVersion                     : 0x{0:x}", new object[]
				{
					this.MinorVersion
				});
				logger.LogInfo(str + "FullFlashMajorVersion            : 0x{0:x}", new object[]
				{
					this.FullFlashMajorVersion
				});
				logger.LogInfo(str + "FullFlashMinorVersion            : 0x{0:x}", new object[]
				{
					this.FullFlashMinorVersion
				});
				logger.LogInfo(str + "PlatformIdentifier               : {0}", new object[]
				{
					@string.Substring(0, @string.IndexOf('\0'))
				});
				logger.LogInfo(str + "BytesPerBlock                    : 0x{0:x}", new object[]
				{
					this.BytesPerBlock
				});
				logger.LogInfo(str + "StoreDataEntryCount              : 0x{0:x}", new object[]
				{
					this.StoreDataEntryCount
				});
				logger.LogInfo(str + "StoreDataSizeInBytes             : 0x{0:x}", new object[]
				{
					this.StoreDataSizeInBytes
				});
				logger.LogInfo(str + "ValidationEntryCount             : 0x{0:x}", new object[]
				{
					this.ValidationEntryCount
				});
				logger.LogInfo(str + "ValidationDataSizeInBytes        : 0x{0:x}", new object[]
				{
					this.ValidationDataSizeInBytes
				});
				logger.LogInfo(str + "InitialPartitionTableBlockIndex  : 0x{0:x}", new object[]
				{
					this.InitialPartitionTableBlockIndex
				});
				logger.LogInfo(str + "InitialPartitionTableBlockCount  : 0x{0:x}", new object[]
				{
					this.InitialPartitionTableBlockCount
				});
				logger.LogInfo(str + "FlashOnlyPartitionTableBlockIndex: 0x{0:x}", new object[]
				{
					this.FlashOnlyPartitionTableBlockIndex
				});
				logger.LogInfo(str + "FlashOnlyPartitionTableBlockCount: 0x{0:x}", new object[]
				{
					this.FlashOnlyPartitionTableBlockCount
				});
				logger.LogInfo(str + "FinalPartitionTableBlockIndex    : 0x{0:x}", new object[]
				{
					this.FinalPartitionTableBlockIndex
				});
				logger.LogInfo(str + "FinalPartitionTableBlockCount    : 0x{0:x}", new object[]
				{
					this.FinalPartitionTableBlockCount
				});
				if (this.MajorVersion >= 2)
				{
					string string2 = Encoding.ASCII.GetString(this.DevicePath);
					logger.LogInfo(str + "NumberOfStores                   : 0x{0:x}", new object[]
					{
						this.NumberOfStores
					});
					logger.LogInfo(str + "StoreIndex                       : 0x{0:x}", new object[]
					{
						this.StoreIndex
					});
					logger.LogInfo(str + "StorePayloadSize                 : 0x{0:x}", new object[]
					{
						this.StorePayloadSize
					});
					logger.LogInfo(str + "DevicePathLength                 : 0x{0:x}", new object[]
					{
						this.DevicePathLength
					});
					logger.LogInfo(str + "DevicePath                       : {0}", new object[]
					{
						string2.Substring(0, string2.IndexOf('\0'))
					});
				}
			}
		}

		// Token: 0x060001EB RID: 491 RVA: 0x00008FD0 File Offset: 0x000071D0
		public int GetStructureSize()
		{
			int num = 0;
			foreach (PropertyInfo propertyInfo in typeof(ImageStoreHeader).GetProperties())
			{
				MethodInfo[] accessors = propertyInfo.GetAccessors();
				int j = 0;
				while (j < accessors.Length)
				{
					MethodInfo methodInfo = accessors[j];
					if (methodInfo.IsStatic || propertyInfo.PropertyType.IsArray)
					{
						break;
					}
					if (methodInfo.IsPublic)
					{
						StructVersionAttribute[] array = propertyInfo.GetCustomAttributes(typeof(StructVersionAttribute), false) as StructVersionAttribute[];
						if (array == null || (this.MajorVersion < 2 && this.IsVersion2Field(array)))
						{
							break;
						}
						if (propertyInfo.PropertyType.IsEnum)
						{
							num += Marshal.SizeOf(Enum.GetUnderlyingType(propertyInfo.PropertyType));
							break;
						}
						num += Marshal.SizeOf(propertyInfo.PropertyType);
						break;
					}
					else
					{
						j++;
					}
				}
			}
			num += ImageStoreHeader.PlatformIdSizeInBytes;
			if (this.MajorVersion >= 2)
			{
				num += (int)this.DevicePathLength;
			}
			return num;
		}

		// Token: 0x060001EC RID: 492 RVA: 0x000090CD File Offset: 0x000072CD
		private bool IsVersion2Field(StructVersionAttribute[] structVersions)
		{
			return structVersions[0].Version == 2;
		}

		// Token: 0x04000138 RID: 312
		public static readonly int PlatformIdSizeInBytes = 192;

		// Token: 0x04000139 RID: 313
		private byte[] _platformId = new byte[ImageStoreHeader.PlatformIdSizeInBytes];

		// Token: 0x0400013A RID: 314
		private ushort _numberOfStores = 1;

		// Token: 0x0400013B RID: 315
		private ushort _storeIndex = 1;

		// Token: 0x0400013C RID: 316
		private ulong _storePayloadSize;

		// Token: 0x0400013D RID: 317
		private ushort _devicePathLength;

		// Token: 0x0400013E RID: 318
		private byte[] _devicePath;
	}
}
