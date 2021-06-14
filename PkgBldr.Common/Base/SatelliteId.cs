using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x0200002C RID: 44
	public class SatelliteId
	{
		// Token: 0x060000AE RID: 174 RVA: 0x00006F2C File Offset: 0x0000512C
		private SatelliteId(SatelliteType type, string id)
		{
			this.Type = type;
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

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000AF RID: 175 RVA: 0x00006FED File Offset: 0x000051ED
		// (set) Token: 0x060000B0 RID: 176 RVA: 0x00006FF5 File Offset: 0x000051F5
		[SuppressMessage("Microsoft.Naming", "CA1721", Justification = "In the context Type is used, it's clear this not Object.GetType().")]
		public SatelliteType Type { get; private set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x00006FFE File Offset: 0x000051FE
		// (set) Token: 0x060000B2 RID: 178 RVA: 0x00007006 File Offset: 0x00005206
		public string Id { get; private set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x0000700F File Offset: 0x0000520F
		public string Culture
		{
			get
			{
				if (this.Type != SatelliteType.Language)
				{
					return null;
				}
				return this.Id;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x00007022 File Offset: 0x00005222
		public string Resolution
		{
			get
			{
				if (this.Type != SatelliteType.Resolution)
				{
					return null;
				}
				return this.Id;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x00007038 File Offset: 0x00005238
		public string MacroName
		{
			get
			{
				switch (this.Type)
				{
				case SatelliteType.Language:
					return "LANGID";
				case SatelliteType.Resolution:
					return "RESID";
				}
				return "NEUTRAL";
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x00007074 File Offset: 0x00005274
		public string MacroString
		{
			get
			{
				switch (this.Type)
				{
				case SatelliteType.Language:
					return "$(LANGID)";
				case SatelliteType.Resolution:
					return "$(RESID)";
				}
				return "NEUTRAL";
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x000070AD File Offset: 0x000052AD
		public string MacroValue
		{
			get
			{
				return this.Id;
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x000070B8 File Offset: 0x000052B8
		public string FileSuffix
		{
			get
			{
				switch (this.Type)
				{
				default:
					return string.Empty;
				case SatelliteType.Language:
					if (this.Id.Equals("*"))
					{
						return "Resources";
					}
					return "lang_" + this.Id;
				case SatelliteType.Resolution:
					return "res_" + this.Id;
				}
			}
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00007120 File Offset: 0x00005320
		public override string ToString()
		{
			switch (this.Type)
			{
			default:
				return "Neutral";
			case SatelliteType.Language:
				return "Language_" + this.Id;
			case SatelliteType.Resolution:
				return "Resolution_" + this.Id;
			}
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00007170 File Offset: 0x00005370
		public override bool Equals(object obj)
		{
			SatelliteId satelliteId = obj as SatelliteId;
			return this == satelliteId || (this.Type == satelliteId.Type && this.Id == satelliteId.Id);
		}

		// Token: 0x060000BB RID: 187 RVA: 0x000071B0 File Offset: 0x000053B0
		public override int GetHashCode()
		{
			return this.Type.GetHashCode() ^ this.Id.GetHashCode();
		}

		// Token: 0x060000BC RID: 188 RVA: 0x000071DD File Offset: 0x000053DD
		public static SatelliteId Create(SatelliteType type, string id)
		{
			if (type == SatelliteType.Neutral)
			{
				return SatelliteId.Neutral;
			}
			return new SatelliteId(type, id);
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000BD RID: 189 RVA: 0x000071EF File Offset: 0x000053EF
		public static SatelliteId Neutral
		{
			get
			{
				return SatelliteId.s_neutralId;
			}
		}

		// Token: 0x0400001F RID: 31
		private static SatelliteId s_neutralId = new SatelliteId(SatelliteType.Neutral, null);
	}
}
