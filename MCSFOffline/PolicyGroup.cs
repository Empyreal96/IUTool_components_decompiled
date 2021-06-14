using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.WindowsPhone.MCSF.Offline
{
	// Token: 0x02000003 RID: 3
	public class PolicyGroup
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000008 RID: 8 RVA: 0x000021AE File Offset: 0x000003AE
		// (set) Token: 0x06000009 RID: 9 RVA: 0x000021B6 File Offset: 0x000003B6
		public string Path { get; private set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000021BF File Offset: 0x000003BF
		// (set) Token: 0x0600000B RID: 11 RVA: 0x000021C7 File Offset: 0x000003C7
		public bool Atomic { get; private set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000021D0 File Offset: 0x000003D0
		// (set) Token: 0x0600000D RID: 13 RVA: 0x000021D8 File Offset: 0x000003D8
		public bool ImageTimeOnly { get; private set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000021E1 File Offset: 0x000003E1
		// (set) Token: 0x0600000F RID: 15 RVA: 0x000021E9 File Offset: 0x000003E9
		public bool FirstVariationOnly { get; private set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000010 RID: 16 RVA: 0x000021F2 File Offset: 0x000003F2
		// (set) Token: 0x06000011 RID: 17 RVA: 0x000021FA File Offset: 0x000003FA
		public bool CriticalSettings { get; private set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000012 RID: 18 RVA: 0x00002203 File Offset: 0x00000403
		public List<string> OEMMacros
		{
			get
			{
				if (this._oemMacros == null)
				{
					this._oemMacros = PolicyMacroTable.OEMMacroList(this.Path);
				}
				return this._oemMacros;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000013 RID: 19 RVA: 0x00002224 File Offset: 0x00000424
		public bool HasOEMMacros
		{
			get
			{
				return this.OEMMacros.Count<string>() > 0;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000014 RID: 20 RVA: 0x00002234 File Offset: 0x00000434
		public IEnumerable<PolicySetting> Settings
		{
			get
			{
				return this.settingsList;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000015 RID: 21 RVA: 0x0000223C File Offset: 0x0000043C
		public IEnumerable<PolicyAssetInfo> Assets
		{
			get
			{
				return this.assetInfoList.Values;
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x0000224C File Offset: 0x0000044C
		public PolicySetting SettingByName(string name)
		{
			IEnumerable<PolicySetting> source = from x in this.settingsList
			where PolicyMacroTable.IsMatch(x.Name, name, StringComparison.OrdinalIgnoreCase)
			select x;
			if (source.Count<PolicySetting>() > 1)
			{
				source = from x in source
				where x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
				select x;
			}
			if (source.Count<PolicySetting>() == 0)
			{
				return null;
			}
			return source.First<PolicySetting>();
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000022AC File Offset: 0x000004AC
		public PolicyAssetInfo AssetByName(string name)
		{
			if (this.assetInfoList.ContainsKey(name))
			{
				return this.assetInfoList[name];
			}
			if (this.assetInfoList.Values.Any((PolicyAssetInfo asstInfo) => asstInfo.IsMatch(name)))
			{
				return this.assetInfoList.Values.First((PolicyAssetInfo asstInfo) => asstInfo.IsMatch(name));
			}
			return null;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002327 File Offset: 0x00000527
		public PolicyMacroTable GetMacroTable(string path)
		{
			return new PolicyMacroTable(this.Path, path);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002335 File Offset: 0x00000535
		public PolicyGroup(XElement policyGroupElement) : this(policyGroupElement, null, null)
		{
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002340 File Offset: 0x00000540
		public PolicyGroup(XElement policyGroupElement, string definedIn) : this(policyGroupElement, definedIn, null)
		{
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000234C File Offset: 0x0000054C
		public PolicyGroup(XElement policyGroupElement, string definedIn, string partition)
		{
			this.settingsList = new List<PolicySetting>();
			this.Path = (string)policyGroupElement.LocalAttribute("Path");
			this.CriticalSettings = (policyGroupElement.LocalAttribute("CriticalSettings") != null && policyGroupElement.LocalAttribute("CriticalSettings").Value.Equals("Yes"));
			this.DefinedIn = definedIn;
			XElement xelement = policyGroupElement.LocalElement("Constraints");
			if (xelement != null)
			{
				this.ImageTimeOnly = (xelement.LocalAttribute("ImageTimeOnly") != null && xelement.LocalAttribute("ImageTimeOnly").Value.Equals("Yes"));
				this.FirstVariationOnly = (xelement.LocalAttribute("FirstVariationOnly") != null && xelement.LocalAttribute("FirstVariationOnly").Value.Equals("Yes"));
				this.Atomic = (xelement.LocalAttribute("Atomic") != null && xelement.LocalAttribute("Atomic").Value.Equals("Yes"));
			}
			foreach (XElement assetElement in policyGroupElement.LocalElements("Asset"))
			{
				PolicyAssetInfo policyAssetInfo = new PolicyAssetInfo(assetElement, definedIn);
				this.assetInfoList.Add(policyAssetInfo.Name, policyAssetInfo);
			}
			foreach (XElement settingElement in policyGroupElement.LocalElements("Setting"))
			{
				PolicySetting item = new PolicySetting(settingElement, this, definedIn, partition);
				this.settingsList.Add(item);
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002510 File Offset: 0x00000710
		public override string ToString()
		{
			return this.Path;
		}

		// Token: 0x04000001 RID: 1
		private List<PolicySetting> settingsList;

		// Token: 0x04000002 RID: 2
		private Dictionary<string, PolicyAssetInfo> assetInfoList = new Dictionary<string, PolicyAssetInfo>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x04000008 RID: 8
		private List<string> _oemMacros;

		// Token: 0x04000009 RID: 9
		public string DefinedIn;
	}
}
