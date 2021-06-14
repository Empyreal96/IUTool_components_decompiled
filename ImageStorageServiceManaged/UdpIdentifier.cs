using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000024 RID: 36
	public class UdpIdentifier : BaseIdentifier, IDeviceIdentifier
	{
		// Token: 0x06000104 RID: 260 RVA: 0x00006232 File Offset: 0x00004432
		public void ReadFromStream(BinaryReader reader)
		{
			this.HardwareType = reader.ReadUInt32();
			this._hardwareAddress = new List<byte>(reader.ReadBytes(16));
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00005402 File Offset: 0x00003602
		public void WriteToStream(BinaryWriter writer)
		{
			throw new ImageStorageException(string.Format("{0}: This function isn't implemented.", MethodBase.GetCurrentMethod().Name));
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00006254 File Offset: 0x00004454
		[CLSCompliant(false)]
		public void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			logger.LogInfo(str + "Identifier: UDP", new object[0]);
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000107 RID: 263 RVA: 0x00005269 File Offset: 0x00003469
		[CLSCompliant(false)]
		public uint Size
		{
			get
			{
				return 0U;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000108 RID: 264 RVA: 0x0000628B File Offset: 0x0000448B
		// (set) Token: 0x06000109 RID: 265 RVA: 0x00006293 File Offset: 0x00004493
		[CLSCompliant(false)]
		public uint HardwareType { get; set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x0600010A RID: 266 RVA: 0x0000629C File Offset: 0x0000449C
		public List<byte> HardwareAddress
		{
			get
			{
				return this._hardwareAddress;
			}
		}

		// Token: 0x040000ED RID: 237
		private List<byte> _hardwareAddress;
	}
}
