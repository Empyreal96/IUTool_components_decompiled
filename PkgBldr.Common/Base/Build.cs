using System;
using System.Collections.Generic;
using Microsoft.CompPlat.PkgBldr.Base.Tools;
using Microsoft.CompPlat.PkgBldr.Tools;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x0200002A RID: 42
	public class Build
	{
		// Token: 0x060000A2 RID: 162 RVA: 0x00006E48 File Offset: 0x00005048
		public Build()
		{
			this.m_wowTypeList.Add(Build.WowType.host);
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00006E67 File Offset: 0x00005067
		public List<Build.WowType> GetWowTypes()
		{
			return this.m_wowTypeList;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00006E6F File Offset: 0x0000506F
		public void AddGuest()
		{
			if (!this.m_wowTypeList.Contains(Build.WowType.guest))
			{
				this.m_wowTypeList.Add(Build.WowType.guest);
			}
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00006E8B File Offset: 0x0000508B
		public bool BuildGuest()
		{
			return this.m_wowTypeList.Contains(Build.WowType.guest);
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x00006E9E File Offset: 0x0000509E
		// (set) Token: 0x060000A7 RID: 167 RVA: 0x00006EA8 File Offset: 0x000050A8
		public string WowDir
		{
			get
			{
				return this.m_wowdir;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.m_wowdir = null;
					return;
				}
				this.m_wowdir = LongPath.GetFullPath(value.TrimEnd(new char[]
				{
					'\\'
				}));
				if (!LongPathDirectory.Exists(this.m_wowdir))
				{
					LongPathDirectory.CreateDirectory(this.m_wowdir);
				}
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x00006EF9 File Offset: 0x000050F9
		// (set) Token: 0x060000A9 RID: 169 RVA: 0x00006F01 File Offset: 0x00005101
		public Build.WowType wow
		{
			get
			{
				return this.m_wow;
			}
			set
			{
				this.m_wow = value;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000AA RID: 170 RVA: 0x00006F0A File Offset: 0x0000510A
		// (set) Token: 0x060000AB RID: 171 RVA: 0x00006F12 File Offset: 0x00005112
		public WowBuildType? WowBuilds { get; set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000AC RID: 172 RVA: 0x00006F1B File Offset: 0x0000511B
		// (set) Token: 0x060000AD RID: 173 RVA: 0x00006F23 File Offset: 0x00005123
		public SatelliteId satellite
		{
			get
			{
				return this.m_satellite;
			}
			set
			{
				this.m_satellite = value;
			}
		}

		// Token: 0x04000016 RID: 22
		private List<Build.WowType> m_wowTypeList = new List<Build.WowType>();

		// Token: 0x04000017 RID: 23
		private string m_wowdir;

		// Token: 0x04000018 RID: 24
		private Build.WowType m_wow;

		// Token: 0x04000019 RID: 25
		private SatelliteId m_satellite;

		// Token: 0x02000063 RID: 99
		public enum WowType
		{
			// Token: 0x0400015F RID: 351
			host,
			// Token: 0x04000160 RID: 352
			guest
		}
	}
}
