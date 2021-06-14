using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.WindowsPhone.MCSF.Offline
{
	// Token: 0x02000008 RID: 8
	public class PolicyAssetInfo
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000059 RID: 89 RVA: 0x0000336A File Offset: 0x0000156A
		public IEnumerable<string> FileTypes
		{
			get
			{
				return this.fileTypes;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00003372 File Offset: 0x00001572
		// (set) Token: 0x0600005B RID: 91 RVA: 0x0000337A File Offset: 0x0000157A
		public string Name { get; private set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00003383 File Offset: 0x00001583
		// (set) Token: 0x0600005D RID: 93 RVA: 0x0000338B File Offset: 0x0000158B
		public string Description { get; private set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00003394 File Offset: 0x00001594
		// (set) Token: 0x0600005F RID: 95 RVA: 0x0000339C File Offset: 0x0000159C
		public string TargetDir { get; private set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000060 RID: 96 RVA: 0x000033A5 File Offset: 0x000015A5
		// (set) Token: 0x06000061 RID: 97 RVA: 0x000033AD File Offset: 0x000015AD
		public string TargetPackage { get; private set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000062 RID: 98 RVA: 0x000033B6 File Offset: 0x000015B6
		// (set) Token: 0x06000063 RID: 99 RVA: 0x000033BE File Offset: 0x000015BE
		public List<PolicyEnum> Presets { get; private set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000064 RID: 100 RVA: 0x000033C7 File Offset: 0x000015C7
		// (set) Token: 0x06000065 RID: 101 RVA: 0x000033CF File Offset: 0x000015CF
		public Dictionary<string, string> PresetsAltDir { get; private set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000066 RID: 102 RVA: 0x000033D8 File Offset: 0x000015D8
		// (set) Token: 0x06000067 RID: 103 RVA: 0x000033E0 File Offset: 0x000015E0
		public string OemRegKey { get; private set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000068 RID: 104 RVA: 0x000033E9 File Offset: 0x000015E9
		// (set) Token: 0x06000069 RID: 105 RVA: 0x000033F1 File Offset: 0x000015F1
		public string OemRegValue { get; private set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600006A RID: 106 RVA: 0x000033FA File Offset: 0x000015FA
		// (set) Token: 0x0600006B RID: 107 RVA: 0x00003402 File Offset: 0x00001602
		public bool FileNameOnly { get; private set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600006C RID: 108 RVA: 0x0000340B File Offset: 0x0000160B
		// (set) Token: 0x0600006D RID: 109 RVA: 0x00003413 File Offset: 0x00001613
		public string MORegKey { get; private set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600006E RID: 110 RVA: 0x0000341C File Offset: 0x0000161C
		public bool GenerateAssetProvXML
		{
			get
			{
				return this.OemRegKey != null || this.MORegKey != null;
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00003431 File Offset: 0x00001631
		public PolicyAssetInfo(XElement assetElement) : this(assetElement, null)
		{
		}

		// Token: 0x06000070 RID: 112 RVA: 0x0000343C File Offset: 0x0000163C
		public PolicyAssetInfo(XElement assetElement, string definedIn)
		{
			string text = (string)assetElement.LocalAttribute("Type");
			this.fileTypes = (from x in text.Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries)
			select x.Trim()).ToList<string>();
			this.TargetDir = (string)assetElement.LocalAttribute("Path");
			this.Name = (string)assetElement.LocalAttribute("Name");
			this.Description = (string)assetElement.LocalAttribute("Description");
			this.TargetPackage = (string)assetElement.LocalAttribute("TargetPackage");
			if (string.IsNullOrEmpty(this.TargetPackage) || this.TargetPackage.Equals("Default"))
			{
				this.TargetPackage = "";
			}
			XElement xelement = assetElement.LocalElement("ValueList") ?? assetElement.LocalElement("MultiStringList");
			if (xelement == null)
			{
				this.OemRegKey = (this.OemRegValue = (this.MORegKey = null));
			}
			else
			{
				this.OemRegKey = (string)(xelement.LocalAttribute("OEMKey") ?? xelement.LocalAttribute("Key"));
				this.OemRegValue = (string)xelement.LocalAttribute("Value");
				this.MORegKey = (string)xelement.LocalAttribute("MOKey");
				this.FileNameOnly = string.Equals((string)xelement.LocalAttribute("FileNamesOnly"), "YES", StringComparison.OrdinalIgnoreCase);
			}
			this.Presets = new List<PolicyEnum>();
			this.PresetsAltDir = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			IEnumerable<XElement> enumerable = assetElement.LocalElements("Preset");
			if (enumerable != null)
			{
				foreach (XElement source in enumerable)
				{
					string text2 = (string)source.LocalAttribute("TargetFileName");
					this.Presets.Add(new PolicyEnum((string)source.LocalAttribute("DisplayName"), text2));
					string value = (string)source.LocalAttribute("AlternatePath");
					if (!string.IsNullOrEmpty(value))
					{
						this.PresetsAltDir.Add(text2, value);
					}
				}
			}
			this.DefinedIn = definedIn;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000036A0 File Offset: 0x000018A0
		public bool IsValidFileType(string filename)
		{
			return this.FileTypes.Any((string type) => filename.EndsWith(type, StringComparison.OrdinalIgnoreCase));
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000072 RID: 114 RVA: 0x000036D1 File Offset: 0x000018D1
		public List<string> OEMMacros
		{
			get
			{
				if (this._oemMacros == null)
				{
					this._oemMacros = PolicyMacroTable.OEMMacroList(this.Name);
				}
				return this._oemMacros;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000073 RID: 115 RVA: 0x000036F2 File Offset: 0x000018F2
		public bool HasOEMMacros
		{
			get
			{
				return this.OEMMacros.Any<string>();
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000036FF File Offset: 0x000018FF
		public bool IsMatch(string value)
		{
			return PolicyMacroTable.IsMatch(this.Name, value, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x0400002D RID: 45
		private List<string> fileTypes;

		// Token: 0x04000038 RID: 56
		public string DefinedIn;

		// Token: 0x04000039 RID: 57
		private List<string> _oemMacros;
	}
}
