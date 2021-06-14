using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.CompPlat.PkgBldr.Base;

namespace Microsoft.CompPlat.PkgBldr.Plugins.CsiToPkg
{
	// Token: 0x0200000F RID: 15
	internal class Security
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00003977 File Offset: 0x00001B77
		public bool HavePolicyData
		{
			get
			{
				return this.m_havePolicyData;
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000397F File Offset: 0x00001B7F
		public Security()
		{
			this.m_lookup = new Dictionary<string, SDDL>();
			this.m_fileSddlList = new Dictionary<string, SDDL>();
			this.m_dirSddlList = new Dictionary<string, SDDL>();
			this.m_regSddlList = new Dictionary<string, SDDL>();
			this.m_policyMacros = new MacroResolver();
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000039BE File Offset: 0x00001BBE
		public void LoadPolicyMacros(XmlReader Macros)
		{
			this.m_policyMacros.Load(Macros);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000039CC File Offset: 0x00001BCC
		public void AddToLookupTable(string name, SDDL ace)
		{
			if (!this.m_lookup.ContainsKey(name.ToUpperInvariant()))
			{
				this.m_lookup.Add(name.ToUpperInvariant(), ace);
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000039F3 File Offset: 0x00001BF3
		public SDDL Lookup(string name)
		{
			name = name.ToUpperInvariant();
			if (this.m_lookup.ContainsKey(name))
			{
				return this.m_lookup[name];
			}
			return null;
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00003A19 File Offset: 0x00001C19
		// (set) Token: 0x06000034 RID: 52 RVA: 0x00003A21 File Offset: 0x00001C21
		public string PolicyID
		{
			get
			{
				return this.m_policyId;
			}
			set
			{
				this.m_policyId = value;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00003A2A File Offset: 0x00001C2A
		public MacroResolver Macros
		{
			get
			{
				return this.m_policyMacros;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00003A32 File Offset: 0x00001C32
		public Dictionary<string, SDDL> FileACLs
		{
			get
			{
				return this.m_fileSddlList;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00003A3A File Offset: 0x00001C3A
		public Dictionary<string, SDDL> DirACLs
		{
			get
			{
				return this.m_dirSddlList;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00003A42 File Offset: 0x00001C42
		public Dictionary<string, SDDL> RegACLs
		{
			get
			{
				return this.m_regSddlList;
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003A4A File Offset: 0x00001C4A
		public void AddFileAce(string path, SDDL ace)
		{
			if (!this.m_fileSddlList.ContainsKey(path.ToUpperInvariant()))
			{
				this.m_fileSddlList.Add(path.ToUpperInvariant(), ace);
				this.m_havePolicyData = true;
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003A78 File Offset: 0x00001C78
		public void AddDirAce(string path, SDDL ace)
		{
			if (!this.m_dirSddlList.ContainsKey(path.ToUpperInvariant()))
			{
				this.m_dirSddlList.Add(path.ToUpperInvariant(), ace);
				this.m_havePolicyData = true;
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003AA6 File Offset: 0x00001CA6
		public void AddRegAce(string path, SDDL ace)
		{
			if (!this.m_regSddlList.ContainsKey(path.ToUpperInvariant()))
			{
				this.m_regSddlList.Add(path.ToUpperInvariant(), ace);
				this.m_havePolicyData = true;
			}
		}

		// Token: 0x04000009 RID: 9
		private string m_policyId;

		// Token: 0x0400000A RID: 10
		private Dictionary<string, SDDL> m_lookup;

		// Token: 0x0400000B RID: 11
		private Dictionary<string, SDDL> m_fileSddlList;

		// Token: 0x0400000C RID: 12
		private Dictionary<string, SDDL> m_dirSddlList;

		// Token: 0x0400000D RID: 13
		private Dictionary<string, SDDL> m_regSddlList;

		// Token: 0x0400000E RID: 14
		private MacroResolver m_policyMacros;

		// Token: 0x0400000F RID: 15
		private bool m_havePolicyData;
	}
}
