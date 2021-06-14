using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200002F RID: 47
	public class FMCollectionItem
	{
		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060001E9 RID: 489 RVA: 0x000111E8 File Offset: 0x0000F3E8
		// (set) Token: 0x060001EA RID: 490 RVA: 0x00011214 File Offset: 0x0000F414
		[XmlAttribute("OwnerName")]
		[DefaultValue(null)]
		public string Owner
		{
			get
			{
				if (this.ownerType == OwnerType.Microsoft)
				{
					return OwnerType.Microsoft.ToString();
				}
				return this._owner;
			}
			set
			{
				if (this.ownerType == OwnerType.Microsoft)
				{
					this._owner = null;
					return;
				}
				this._owner = value;
			}
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0001122E File Offset: 0x0000F42E
		public bool ShouldSerializeMicrosoftFMGUID()
		{
			return this.MicrosoftFMGUID != Guid.Empty;
		}

		// Token: 0x060001EC RID: 492 RVA: 0x00011240 File Offset: 0x0000F440
		public override string ToString()
		{
			return this.Path;
		}

		// Token: 0x060001ED RID: 493 RVA: 0x00011248 File Offset: 0x0000F448
		public string ResolveFMPath(string fmDirectory)
		{
			return FMCollection.ResolveFMPath(this.Path, fmDirectory);
		}

		// Token: 0x04000145 RID: 325
		[XmlAttribute]
		public string Path;

		// Token: 0x04000146 RID: 326
		[XmlAttribute("ReleaseType")]
		[DefaultValue(ReleaseType.Production)]
		public ReleaseType releaseType = ReleaseType.Production;

		// Token: 0x04000147 RID: 327
		[XmlAttribute]
		[DefaultValue(false)]
		public bool UserInstallable;

		// Token: 0x04000148 RID: 328
		[XmlAttribute("OwnerType")]
		[DefaultValue(OwnerType.OEM)]
		public OwnerType ownerType = OwnerType.OEM;

		// Token: 0x04000149 RID: 329
		private string _owner;

		// Token: 0x0400014A RID: 330
		[XmlAttribute]
		[DefaultValue(CpuId.Invalid)]
		public CpuId CPUType;

		// Token: 0x0400014B RID: 331
		[XmlAttribute]
		[DefaultValue(false)]
		public bool SkipForPublishing;

		// Token: 0x0400014C RID: 332
		[XmlAttribute]
		[DefaultValue(false)]
		public bool SkipForPRSSigning;

		// Token: 0x0400014D RID: 333
		[XmlAttribute]
		[DefaultValue(false)]
		public bool ValidateAsMicrosoftPhoneFM;

		// Token: 0x0400014E RID: 334
		[XmlAttribute]
		[DefaultValue(false)]
		public bool Critical;

		// Token: 0x0400014F RID: 335
		[XmlAttribute]
		public string ID;

		// Token: 0x04000150 RID: 336
		[XmlAttribute]
		public Guid MicrosoftFMGUID = Guid.Empty;
	}
}
