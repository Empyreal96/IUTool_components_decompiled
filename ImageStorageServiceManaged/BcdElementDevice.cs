using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000012 RID: 18
	public class BcdElementDevice : BcdElement
	{
		// Token: 0x06000056 RID: 86 RVA: 0x00004954 File Offset: 0x00002B54
		public static BcdElementDevice CreateBaseBootDevice()
		{
			return new BcdElementDevice(BcdElementBootDevice.CreateBaseBootDevice(), new BcdElementDataType(285212673U));
		}

		// Token: 0x06000057 RID: 87 RVA: 0x0000496A File Offset: 0x00002B6A
		public static BcdElementDevice CreateBaseRamdiskDevice(string filePath, BcdElementBootDevice parentDevice)
		{
			return new BcdElementDevice(BcdElementBootDevice.CreateBaseRamdiskDevice(filePath, parentDevice), BcdElementDataTypes.OsLoaderDevice, BcdObjects.WindowsSetupRamdiskOptions);
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004984 File Offset: 0x00002B84
		public BcdElementDevice(byte[] binaryData, BcdElementDataType dataType) : base(dataType)
		{
			base.SetBinaryData(binaryData);
			MemoryStream stream = new MemoryStream(binaryData);
			this.ReadFromStream(stream);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000049B0 File Offset: 0x00002BB0
		public BcdElementDevice(BcdElementBootDevice bootDevice, BcdElementDataType dataType) : base(dataType)
		{
			this.AdditionalFlags = Guid.Empty;
			this.BootDevice = bootDevice;
			byte[] array = new byte[this.BinarySize];
			using (MemoryStream memoryStream = new MemoryStream(array))
			{
				this.WriteToStream(memoryStream);
				base.SetBinaryData(array);
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00004A14 File Offset: 0x00002C14
		public BcdElementDevice(BcdElementBootDevice bootDevice, BcdElementDataType dataType, Guid Flags) : base(dataType)
		{
			this.AdditionalFlags = Flags;
			this.BootDevice = bootDevice;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.WriteToStream(memoryStream);
				base.SetBinaryData(memoryStream.ToArray());
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600005B RID: 91 RVA: 0x00004A6C File Offset: 0x00002C6C
		// (set) Token: 0x0600005C RID: 92 RVA: 0x00004A74 File Offset: 0x00002C74
		public Guid AdditionalFlags { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600005D RID: 93 RVA: 0x00004A7D File Offset: 0x00002C7D
		// (set) Token: 0x0600005E RID: 94 RVA: 0x00004A85 File Offset: 0x00002C85
		public BcdElementBootDevice BootDevice { get; set; }

		// Token: 0x0600005F RID: 95 RVA: 0x00004A90 File Offset: 0x00002C90
		public void ReadFromStream(Stream stream)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			byte[] b = binaryReader.ReadBytes(16);
			this.AdditionalFlags = new Guid(b);
			this.BootDevice = new BcdElementBootDevice();
			this.BootDevice.ReadFromStream(binaryReader);
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00004AD4 File Offset: 0x00002CD4
		public void WriteToStream(Stream stream)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			byte[] buffer = this.AdditionalFlags.ToByteArray();
			binaryWriter.Write(buffer);
			this.BootDevice.WriteToStream(binaryWriter);
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000061 RID: 97 RVA: 0x00004B0C File Offset: 0x00002D0C
		[CLSCompliant(false)]
		public uint BinarySize
		{
			get
			{
				return 16U + this.BootDevice.Size;
			}
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00004B1C File Offset: 0x00002D1C
		[CLSCompliant(false)]
		public override void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			base.LogInfo(logger, indentLevel);
			logger.LogInfo(str + "Additional Flags:   {{{0}}}", new object[]
			{
				this.AdditionalFlags
			});
			logger.LogInfo("", new object[0]);
			this.BootDevice.LogInfo(logger, checked(indentLevel + 2));
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00004B8C File Offset: 0x00002D8C
		[CLSCompliant(false)]
		public void ReplaceRamDiskDeviceIdentifier(IDeviceIdentifier identifier)
		{
			RamDiskIdentifier ramDiskIdentifier = (RamDiskIdentifier)this.BootDevice.Identifier;
			if (ramDiskIdentifier == null)
			{
				throw new ImageStorageException(string.Format("{0}: The device's identifier is not a ramdisk.", MethodBase.GetCurrentMethod().Name));
			}
			ramDiskIdentifier.ReplaceParentDeviceIdentifier(identifier);
			this.BootDevice.ReplaceIdentifier(ramDiskIdentifier);
			byte[] array = new byte[this.BinarySize];
			using (MemoryStream memoryStream = new MemoryStream(array))
			{
				this.WriteToStream(memoryStream);
				memoryStream.Flush();
				memoryStream.Close();
			}
			base.SetBinaryData(array);
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00004C24 File Offset: 0x00002E24
		[CLSCompliant(false)]
		public void ReplaceBootDeviceIdentifier(IDeviceIdentifier identifier)
		{
			this.BootDevice.ReplaceIdentifier(identifier);
			byte[] array = new byte[this.BinarySize];
			MemoryStream memoryStream = new MemoryStream(array);
			this.WriteToStream(memoryStream);
			base.SetBinaryData(array);
			memoryStream.Close();
		}
	}
}
