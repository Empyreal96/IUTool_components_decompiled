using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000025 RID: 37
	public class LocateIdentifier : BaseIdentifier, IDeviceIdentifier
	{
		// Token: 0x0600010C RID: 268 RVA: 0x000062A4 File Offset: 0x000044A4
		public void ReadFromStream(BinaryReader reader)
		{
			this.Type = (LocateIdentifier.LocateType)reader.ReadUInt32();
			this.ElementType = reader.ReadUInt32();
			this.ParentOffset = reader.ReadUInt32();
			this.Path = reader.ReadString();
			if (this.Type == LocateIdentifier.LocateType.BootElement)
			{
				throw new ImageStorageException("Not supported.");
			}
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00005402 File Offset: 0x00003602
		public void WriteToStream(BinaryWriter writer)
		{
			throw new ImageStorageException(string.Format("{0}: This function isn't implemented.", MethodBase.GetCurrentMethod().Name));
		}

		// Token: 0x0600010E RID: 270 RVA: 0x000062F4 File Offset: 0x000044F4
		[CLSCompliant(false)]
		public void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			logger.LogInfo(str + "Identifier: Locate", new object[0]);
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600010F RID: 271 RVA: 0x00005269 File Offset: 0x00003469
		[CLSCompliant(false)]
		public uint Size
		{
			get
			{
				return 0U;
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000110 RID: 272 RVA: 0x0000632B File Offset: 0x0000452B
		// (set) Token: 0x06000111 RID: 273 RVA: 0x00006333 File Offset: 0x00004533
		[CLSCompliant(false)]
		public LocateIdentifier.LocateType Type { get; set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000112 RID: 274 RVA: 0x0000633C File Offset: 0x0000453C
		// (set) Token: 0x06000113 RID: 275 RVA: 0x00006344 File Offset: 0x00004544
		[CLSCompliant(false)]
		public uint ElementType { get; set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000114 RID: 276 RVA: 0x0000634D File Offset: 0x0000454D
		// (set) Token: 0x06000115 RID: 277 RVA: 0x00006355 File Offset: 0x00004555
		[CLSCompliant(false)]
		public uint ParentOffset { get; set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000116 RID: 278 RVA: 0x0000635E File Offset: 0x0000455E
		// (set) Token: 0x06000117 RID: 279 RVA: 0x00006366 File Offset: 0x00004566
		public string Path { get; set; }

		// Token: 0x02000077 RID: 119
		[CLSCompliant(false)]
		public enum LocateType : uint
		{
			// Token: 0x040002AB RID: 683
			BootElement,
			// Token: 0x040002AC RID: 684
			String
		}
	}
}
