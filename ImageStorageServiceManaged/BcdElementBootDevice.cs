using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000013 RID: 19
	public class BcdElementBootDevice
	{
		// Token: 0x06000065 RID: 101 RVA: 0x00004C66 File Offset: 0x00002E66
		public static BcdElementBootDevice CreateBaseBootDevice()
		{
			return new BcdElementBootDevice
			{
				Type = BcdElementBootDevice.DeviceType.Boot,
				Size = BcdElementBootDevice.BaseBootDeviceSizeInBytes,
				Flags = 0U,
				Identifier = new BootIdentifier()
			};
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00004C91 File Offset: 0x00002E91
		public static BcdElementBootDevice CreateBaseRamdiskDevice(string filePath, BcdElementBootDevice parentDevice)
		{
			return new BcdElementBootDevice
			{
				Type = BcdElementBootDevice.DeviceType.BlockIo,
				Size = 33U,
				Flags = 1U,
				Identifier = new RamDiskIdentifier(filePath, parentDevice)
			};
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000067 RID: 103 RVA: 0x00004CBB File Offset: 0x00002EBB
		// (set) Token: 0x06000068 RID: 104 RVA: 0x00004CC3 File Offset: 0x00002EC3
		[CLSCompliant(false)]
		public BcdElementBootDevice.DeviceType Type { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000069 RID: 105 RVA: 0x00004CCC File Offset: 0x00002ECC
		// (set) Token: 0x0600006A RID: 106 RVA: 0x00004CD4 File Offset: 0x00002ED4
		[CLSCompliant(false)]
		public uint Flags { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600006B RID: 107 RVA: 0x00004CDD File Offset: 0x00002EDD
		internal static uint BaseSize
		{
			get
			{
				return 16U;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600006C RID: 108 RVA: 0x00004CE1 File Offset: 0x00002EE1
		// (set) Token: 0x0600006D RID: 109 RVA: 0x00004CE9 File Offset: 0x00002EE9
		[CLSCompliant(false)]
		public uint Size { get; set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600006E RID: 110 RVA: 0x00004CF4 File Offset: 0x00002EF4
		[CLSCompliant(false)]
		public uint CalculatedSize
		{
			get
			{
				uint num = BcdElementBootDevice.BaseSize;
				if (this.Identifier != null)
				{
					num += this.Identifier.Size;
				}
				return num;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600006F RID: 111 RVA: 0x00004D1E File Offset: 0x00002F1E
		// (set) Token: 0x06000070 RID: 112 RVA: 0x00004D26 File Offset: 0x00002F26
		[CLSCompliant(false)]
		public IDeviceIdentifier Identifier
		{
			get
			{
				return this._identifier;
			}
			protected set
			{
				this._identifier = value;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000071 RID: 113 RVA: 0x00004D2F File Offset: 0x00002F2F
		// (set) Token: 0x06000072 RID: 114 RVA: 0x00004D37 File Offset: 0x00002F37
		internal long OriginalStreamPosition { get; set; }

		// Token: 0x06000073 RID: 115 RVA: 0x00004D40 File Offset: 0x00002F40
		[CLSCompliant(false)]
		public void ReplaceIdentifier(IDeviceIdentifier identifier)
		{
			this.Identifier = identifier;
			if (this._identifier.GetType() == typeof(PartitionIdentifierEx))
			{
				this.Type = BcdElementBootDevice.DeviceType.PartitionEx;
			}
			this.Size = BcdElementBootDevice.BaseSize + identifier.Size;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00004D80 File Offset: 0x00002F80
		public void ReadFromStream(BinaryReader reader)
		{
			this.OriginalStreamPosition = reader.BaseStream.Position;
			this.Type = (BcdElementBootDevice.DeviceType)reader.ReadUInt32();
			this.Flags = reader.ReadUInt32();
			this.Size = reader.ReadUInt32();
			reader.ReadUInt32();
			switch (this.Type)
			{
			case BcdElementBootDevice.DeviceType.BlockIo:
				this.Identifier = BlockIoIdentifierFactory.CreateFromStream(reader);
				goto IL_C9;
			case BcdElementBootDevice.DeviceType.Partition:
				this.Identifier = new PartitionIdentifier();
				goto IL_C9;
			case BcdElementBootDevice.DeviceType.SerialPort:
				this.Identifier = new SerialPortIdentifier();
				goto IL_C9;
			case BcdElementBootDevice.DeviceType.Udp:
				this.Identifier = new UdpIdentifier();
				goto IL_C9;
			case BcdElementBootDevice.DeviceType.Boot:
				goto IL_C9;
			case BcdElementBootDevice.DeviceType.PartitionEx:
				this.Identifier = new PartitionIdentifierEx();
				goto IL_C9;
			case BcdElementBootDevice.DeviceType.Locate:
				this.Identifier = new LocateIdentifier();
				goto IL_C9;
			}
			throw new ImageStorageException("Unknown Device Identifier type.");
			IL_C9:
			if (this.Identifier != null)
			{
				this.Identifier.Parent = this;
				this.Identifier.ReadFromStream(reader);
				if (reader.BaseStream.Position - this.OriginalStreamPosition < (long)((ulong)this.Size))
				{
					uint count = this.Size - (uint)(reader.BaseStream.Position - this.OriginalStreamPosition);
					byte[] array = reader.ReadBytes((int)count);
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] != 0)
						{
							throw new ImageStorageException(string.Format("{0}: Non-zero data was found at the end of a boot device object.", MethodBase.GetCurrentMethod().Name));
						}
					}
				}
			}
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00004EE4 File Offset: 0x000030E4
		public void WriteToStream(BinaryWriter writer)
		{
			writer.Write((uint)this.Type);
			writer.Write(this.Flags);
			writer.Write(BcdElementBootDevice.BaseSize + this.Identifier.Size);
			writer.Write(0U);
			this.Identifier.WriteToStream(writer);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00004F34 File Offset: 0x00003134
		[CLSCompliant(false)]
		public void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			logger.LogInfo(str + string.Format("Boot Device:  {0}", this.Type), new object[0]);
			logger.LogInfo(str + string.Format("Device Flags: 0x{0:x}", this.Flags), new object[0]);
			logger.LogInfo(str + string.Format("Device Size:  0x{0:x}", this.Size), new object[0]);
			if (this.Identifier != null)
			{
				logger.LogInfo("", new object[0]);
				this.Identifier.LogInfo(logger, checked(indentLevel + 2));
			}
		}

		// Token: 0x040000BD RID: 189
		[CLSCompliant(false)]
		public static readonly uint BaseBootDeviceSizeInBytes = 72U;

		// Token: 0x040000C1 RID: 193
		private IDeviceIdentifier _identifier;

		// Token: 0x02000076 RID: 118
		[CLSCompliant(false)]
		public enum DeviceType : uint
		{
			// Token: 0x040002A2 RID: 674
			BlockIo,
			// Token: 0x040002A3 RID: 675
			Unused,
			// Token: 0x040002A4 RID: 676
			Partition,
			// Token: 0x040002A5 RID: 677
			SerialPort,
			// Token: 0x040002A6 RID: 678
			Udp,
			// Token: 0x040002A7 RID: 679
			Boot,
			// Token: 0x040002A8 RID: 680
			PartitionEx,
			// Token: 0x040002A9 RID: 681
			Locate = 8U
		}
	}
}
