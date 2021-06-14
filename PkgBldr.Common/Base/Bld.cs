using System;
using System.Collections.Generic;
using Microsoft.CompPlat.PkgBldr.Base.Tools;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x0200003D RID: 61
	public class Bld
	{
		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000112 RID: 274 RVA: 0x00008357 File Offset: 0x00006557
		public List<SatelliteId> AllResolutions
		{
			get
			{
				return this._resolutions;
			}
		}

		// Token: 0x06000113 RID: 275 RVA: 0x0000835F File Offset: 0x0000655F
		public void SetAllResolutions(List<SatelliteId> resolutions)
		{
			this._resolutions = resolutions;
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000114 RID: 276 RVA: 0x00008368 File Offset: 0x00006568
		public bool IsPhoneBuild
		{
			get
			{
				return this._isPhoneBuild;
			}
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00008370 File Offset: 0x00006570
		public Bld()
		{
			if (!Environment.ExpandEnvironmentVariables("%_WINPHONEROOT%").StartsWith("%", StringComparison.OrdinalIgnoreCase))
			{
				this._isPhoneBuild = true;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000117 RID: 279 RVA: 0x0000841C File Offset: 0x0000661C
		// (set) Token: 0x06000116 RID: 278 RVA: 0x000083C4 File Offset: 0x000065C4
		public CpuType Arch
		{
			get
			{
				return this._arch;
			}
			set
			{
				this._arch = value;
				switch (value)
				{
				case CpuType.x86:
				case CpuType.amd64:
					this.HostArch = "amd64";
					this.GuestArch = "x86";
					return;
				case CpuType.arm:
				case CpuType.arm64:
					this.HostArch = "arm64";
					this.GuestArch = "arm";
					return;
				default:
					return;
				}
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000118 RID: 280 RVA: 0x00008424 File Offset: 0x00006624
		public string ArchAsString
		{
			get
			{
				switch (this._arch)
				{
				case CpuType.x86:
					return "x86";
				case CpuType.amd64:
					return "amd64";
				case CpuType.arm:
					return "arm";
				case CpuType.arm64:
					return "arm64";
				default:
					return null;
				}
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600011A RID: 282 RVA: 0x000084CE File Offset: 0x000066CE
		// (set) Token: 0x06000119 RID: 281 RVA: 0x0000846C File Offset: 0x0000666C
		public string GuestArch
		{
			get
			{
				return this._guestArch;
			}
			set
			{
				if (value == null)
				{
					throw new PkgGenException("GuestArch cannot be null");
				}
				this._guestArch = value.ToLowerInvariant();
				string guestArch = this._guestArch;
				if (!(guestArch == "x86") && !(guestArch == "arm"))
				{
					throw new PkgGenException("Invalid arch = {0}", new object[]
					{
						value.ToLowerInvariant()
					});
				}
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600011C RID: 284 RVA: 0x0000853A File Offset: 0x0000673A
		// (set) Token: 0x0600011B RID: 283 RVA: 0x000084D8 File Offset: 0x000066D8
		public string HostArch
		{
			get
			{
				return this._hostArch;
			}
			set
			{
				if (value == null)
				{
					throw new PkgGenException("HostArch cannot be null");
				}
				this._hostArch = value.ToLowerInvariant();
				string hostArch = this._hostArch;
				if (!(hostArch == "amd64") && !(hostArch == "arm64"))
				{
					throw new PkgGenException("Invalid arch = {0}", new object[]
					{
						value.ToLowerInvariant()
					});
				}
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600011E RID: 286 RVA: 0x0000855B File Offset: 0x0000675B
		// (set) Token: 0x0600011D RID: 285 RVA: 0x00008542 File Offset: 0x00006742
		public string Product
		{
			get
			{
				return this._product;
			}
			set
			{
				if (value != null)
				{
					this._product = value.ToLowerInvariant();
					return;
				}
				this._product = null;
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000120 RID: 288 RVA: 0x0000857C File Offset: 0x0000677C
		// (set) Token: 0x0600011F RID: 287 RVA: 0x00008563 File Offset: 0x00006763
		public string Version
		{
			get
			{
				return this._version;
			}
			set
			{
				if (value != null)
				{
					this._version = value.ToLowerInvariant();
					return;
				}
				this._version = null;
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000122 RID: 290 RVA: 0x0000859D File Offset: 0x0000679D
		// (set) Token: 0x06000121 RID: 289 RVA: 0x00008584 File Offset: 0x00006784
		public string JsonDepot
		{
			get
			{
				return this._jsonDepot;
			}
			set
			{
				if (value != null)
				{
					this._jsonDepot = value.ToLowerInvariant();
					return;
				}
				this._jsonDepot = null;
			}
		}

		// Token: 0x0400006B RID: 107
		private CpuType _arch;

		// Token: 0x0400006C RID: 108
		private string _hostArch;

		// Token: 0x0400006D RID: 109
		private string _guestArch;

		// Token: 0x0400006E RID: 110
		private string _version;

		// Token: 0x0400006F RID: 111
		private string _product;

		// Token: 0x04000070 RID: 112
		private bool _isPhoneBuild;

		// Token: 0x04000071 RID: 113
		private string _jsonDepot;

		// Token: 0x04000072 RID: 114
		private List<SatelliteId> _resolutions;

		// Token: 0x04000073 RID: 115
		public string Lang;

		// Token: 0x04000074 RID: 116
		public string Resolution;

		// Token: 0x04000075 RID: 117
		public bool IsGuest;

		// Token: 0x04000076 RID: 118
		public MacroResolver BuildMacros;

		// Token: 0x04000077 RID: 119
		public CSI CSI = new CSI();

		// Token: 0x04000078 RID: 120
		public PKG PKG = new PKG();

		// Token: 0x04000079 RID: 121
		public WM WM = new WM();
	}
}
