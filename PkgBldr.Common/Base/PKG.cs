using System;
using System.Xml.Linq;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x0200003C RID: 60
	public class PKG
	{
		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000107 RID: 263 RVA: 0x00008302 File Offset: 0x00006502
		// (set) Token: 0x06000108 RID: 264 RVA: 0x0000830A File Offset: 0x0000650A
		public string OwnerType
		{
			get
			{
				return this._ownerType;
			}
			set
			{
				this._ownerType = value;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000109 RID: 265 RVA: 0x00008313 File Offset: 0x00006513
		// (set) Token: 0x0600010A RID: 266 RVA: 0x0000831B File Offset: 0x0000651B
		public string Partition
		{
			get
			{
				return this._partition;
			}
			set
			{
				this._partition = value;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x0600010B RID: 267 RVA: 0x00008324 File Offset: 0x00006524
		// (set) Token: 0x0600010C RID: 268 RVA: 0x0000832C File Offset: 0x0000652C
		public string Component
		{
			get
			{
				return this._component;
			}
			set
			{
				this._component = value;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x0600010D RID: 269 RVA: 0x00008335 File Offset: 0x00006535
		// (set) Token: 0x0600010E RID: 270 RVA: 0x0000833D File Offset: 0x0000653D
		public string SubComponent
		{
			get
			{
				return this._subComponent;
			}
			set
			{
				this._subComponent = value;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x0600010F RID: 271 RVA: 0x00008346 File Offset: 0x00006546
		// (set) Token: 0x06000110 RID: 272 RVA: 0x0000834E File Offset: 0x0000654E
		public string ReleaseType
		{
			get
			{
				return this._releaseType;
			}
			set
			{
				this._releaseType = value;
			}
		}

		// Token: 0x04000065 RID: 101
		private string _partition;

		// Token: 0x04000066 RID: 102
		private string _ownerType;

		// Token: 0x04000067 RID: 103
		private string _component;

		// Token: 0x04000068 RID: 104
		private string _subComponent;

		// Token: 0x04000069 RID: 105
		private string _releaseType;

		// Token: 0x0400006A RID: 106
		public XElement Root;
	}
}
