using System;
using System.Text.RegularExpressions;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x0200004A RID: 74
	public class SatelliteId
	{
		// Token: 0x06000131 RID: 305 RVA: 0x00005D00 File Offset: 0x00003F00
		private SatelliteId(SatelliteType type, string id)
		{
			this.SatType = type;
			switch (type)
			{
			case SatelliteType.Neutral:
				this.Id = string.Empty;
				return;
			case SatelliteType.Language:
				if (id == null || !Regex.Match(id, PkgConstants.c_strCultureStringPattern).Success)
				{
					throw new PkgGenException("Invalid language identifier string: {0}", new object[]
					{
						id
					});
				}
				this.Id = id.ToLowerInvariant();
				return;
			case SatelliteType.Resolution:
				if (id == null || !Regex.Match(id, PkgConstants.c_strResolutionStringPattern).Success)
				{
					throw new PkgGenException("Invalid resolution identifier string: {0}", new object[]
					{
						id
					});
				}
				this.Id = id.ToLowerInvariant();
				return;
			default:
				throw new PkgGenException("Unexpected satellite type: {0}", new object[]
				{
					type
				});
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000132 RID: 306 RVA: 0x00005DC1 File Offset: 0x00003FC1
		// (set) Token: 0x06000133 RID: 307 RVA: 0x00005DC9 File Offset: 0x00003FC9
		public SatelliteType SatType { get; private set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000134 RID: 308 RVA: 0x00005DD2 File Offset: 0x00003FD2
		// (set) Token: 0x06000135 RID: 309 RVA: 0x00005DDA File Offset: 0x00003FDA
		public string Id { get; private set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000136 RID: 310 RVA: 0x00005DE3 File Offset: 0x00003FE3
		public string Culture
		{
			get
			{
				if (this.SatType != SatelliteType.Language)
				{
					return null;
				}
				return this.Id;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000137 RID: 311 RVA: 0x00005DF6 File Offset: 0x00003FF6
		public string Resolution
		{
			get
			{
				if (this.SatType != SatelliteType.Resolution)
				{
					return null;
				}
				return this.Id;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000138 RID: 312 RVA: 0x00005E0C File Offset: 0x0000400C
		public string MacroName
		{
			get
			{
				SatelliteType satType = this.SatType;
				if (satType == SatelliteType.Language)
				{
					return "LANGID";
				}
				if (satType != SatelliteType.Resolution)
				{
					throw new NotSupportedException("Unsupported satellite type '" + this.SatType + "' for property MacroName");
				}
				return "RESID";
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000139 RID: 313 RVA: 0x00005E58 File Offset: 0x00004058
		public string MacroString
		{
			get
			{
				SatelliteType satType = this.SatType;
				if (satType == SatelliteType.Language)
				{
					return "$(LANGID)";
				}
				if (satType != SatelliteType.Resolution)
				{
					throw new NotSupportedException("Unsupported satellite type '" + this.SatType + "' for property MacroString");
				}
				return "$(RESID)";
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x0600013A RID: 314 RVA: 0x00005EA1 File Offset: 0x000040A1
		public string MacroValue
		{
			get
			{
				if (this.SatType != SatelliteType.Neutral)
				{
					return this.Id;
				}
				throw new NotSupportedException("Unsupported satellite type '" + this.SatType + "' for property MacroValue");
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x0600013B RID: 315 RVA: 0x00005ED4 File Offset: 0x000040D4
		public string FileSuffix
		{
			get
			{
				switch (this.SatType)
				{
				case SatelliteType.Neutral:
					return string.Empty;
				case SatelliteType.Language:
					return "lang_" + this.Id;
				case SatelliteType.Resolution:
					return "res_" + this.Id;
				default:
					throw new NotSupportedException("Unsupported satellite type '" + this.SatType + "' for property FileSuffix");
				}
			}
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00005F44 File Offset: 0x00004144
		public override string ToString()
		{
			switch (this.SatType)
			{
			case SatelliteType.Neutral:
				return "Neutral";
			case SatelliteType.Language:
				return "Language_" + this.Id;
			case SatelliteType.Resolution:
				return "Resolution_" + this.Id;
			default:
				return "Unexpected satellite type '" + this.SatType + "'";
			}
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00005FB0 File Offset: 0x000041B0
		public override bool Equals(object obj)
		{
			SatelliteId satelliteId = obj as SatelliteId;
			return this == satelliteId || (this.SatType == satelliteId.SatType && this.Id == satelliteId.Id);
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00005FF0 File Offset: 0x000041F0
		public override int GetHashCode()
		{
			return this.SatType.GetHashCode() ^ this.Id.GetHashCode();
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000601D File Offset: 0x0000421D
		public static SatelliteId Create(SatelliteType type, string id)
		{
			if (type == SatelliteType.Neutral)
			{
				return SatelliteId.Neutral;
			}
			return new SatelliteId(type, id);
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000140 RID: 320 RVA: 0x0000602F File Offset: 0x0000422F
		public static SatelliteId Neutral
		{
			get
			{
				return SatelliteId.s_neutralId;
			}
		}

		// Token: 0x040000FB RID: 251
		private static SatelliteId s_neutralId = new SatelliteId(SatelliteType.Neutral, null);
	}
}
