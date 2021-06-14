using System;
using System.IO;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000023 RID: 35
	public class SerialPortIdentifier : BaseIdentifier, IDeviceIdentifier
	{
		// Token: 0x060000F1 RID: 241 RVA: 0x000060E4 File Offset: 0x000042E4
		public void ReadFromStream(BinaryReader reader)
		{
			this.Type = reader.ReadUInt32();
			byte[] array = reader.ReadBytes(12);
			this.PortNumber = (uint)((int)array[0] << 24 | (int)array[1] << 16 | (int)array[2] << 8 | (int)array[3]);
			this.GenericAddressSpaceId = array[0];
			this.GenericAddressWidth = array[1];
			this.GenericAddressBitOffset = array[2];
			this.GenericAddressAccessSize = array[3];
			this.GenericAddressPhysicalAddress = (ulong)((long)((int)array[4] << 24 | (int)array[5] << 16 | (int)array[6] << 8 | (int)array[7] | (int)array[8] << 24 | (int)array[9] << 16 | (int)array[10] << 8 | (int)array[11]));
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00006180 File Offset: 0x00004380
		public void WriteToStream(BinaryWriter writer)
		{
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00006184 File Offset: 0x00004384
		[CLSCompliant(false)]
		public void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			logger.LogInfo(str + "Identifier: Serial Port", new object[0]);
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x00005269 File Offset: 0x00003469
		[CLSCompliant(false)]
		public uint Size
		{
			get
			{
				return 0U;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x000061BB File Offset: 0x000043BB
		// (set) Token: 0x060000F6 RID: 246 RVA: 0x000061C3 File Offset: 0x000043C3
		[CLSCompliant(false)]
		public uint Type { get; set; }

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x000061CC File Offset: 0x000043CC
		// (set) Token: 0x060000F8 RID: 248 RVA: 0x000061D4 File Offset: 0x000043D4
		[CLSCompliant(false)]
		public uint PortNumber { get; set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x000061DD File Offset: 0x000043DD
		// (set) Token: 0x060000FA RID: 250 RVA: 0x000061E5 File Offset: 0x000043E5
		public byte GenericAddressSpaceId { get; set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000FB RID: 251 RVA: 0x000061EE File Offset: 0x000043EE
		// (set) Token: 0x060000FC RID: 252 RVA: 0x000061F6 File Offset: 0x000043F6
		public byte GenericAddressWidth { get; set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000FD RID: 253 RVA: 0x000061FF File Offset: 0x000043FF
		// (set) Token: 0x060000FE RID: 254 RVA: 0x00006207 File Offset: 0x00004407
		public byte GenericAddressBitOffset { get; set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000FF RID: 255 RVA: 0x00006210 File Offset: 0x00004410
		// (set) Token: 0x06000100 RID: 256 RVA: 0x00006218 File Offset: 0x00004418
		public byte GenericAddressAccessSize { get; set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000101 RID: 257 RVA: 0x00006221 File Offset: 0x00004421
		// (set) Token: 0x06000102 RID: 258 RVA: 0x00006229 File Offset: 0x00004429
		[CLSCompliant(false)]
		public ulong GenericAddressPhysicalAddress { get; set; }
	}
}
