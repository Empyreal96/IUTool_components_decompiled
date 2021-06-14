using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.WindowsPhone.MCSF.Offline
{
	// Token: 0x0200000A RID: 10
	public class PolicySettingDestination
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000075 RID: 117 RVA: 0x0000370E File Offset: 0x0000190E
		// (set) Token: 0x06000076 RID: 118 RVA: 0x00003716 File Offset: 0x00001916
		public PolicySettingDestinationType Destination { get; private set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000077 RID: 119 RVA: 0x0000371F File Offset: 0x0000191F
		// (set) Token: 0x06000078 RID: 120 RVA: 0x00003727 File Offset: 0x00001927
		public string Type { get; private set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000079 RID: 121 RVA: 0x00003730 File Offset: 0x00001930
		// (set) Token: 0x0600007A RID: 122 RVA: 0x00003738 File Offset: 0x00001938
		public string Path { get; private set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600007B RID: 123 RVA: 0x00003741 File Offset: 0x00001941
		// (set) Token: 0x0600007C RID: 124 RVA: 0x00003749 File Offset: 0x00001949
		public string RegistryKey { get; private set; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600007D RID: 125 RVA: 0x00003752 File Offset: 0x00001952
		// (set) Token: 0x0600007E RID: 126 RVA: 0x0000375A File Offset: 0x0000195A
		public string RegistryValueName { get; private set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600007F RID: 127 RVA: 0x00003763 File Offset: 0x00001963
		// (set) Token: 0x06000080 RID: 128 RVA: 0x0000376B File Offset: 0x0000196B
		public IEnumerable<string> ProvisioningPath { get; private set; }

		// Token: 0x06000081 RID: 129 RVA: 0x00003774 File Offset: 0x00001974
		public PolicySettingDestination(XElement destinationElement, PolicySetting parent, PolicyGroup grandparent)
		{
			this.Destination = (string.Equals("RegistrySource", destinationElement.Name.LocalName, StringComparison.OrdinalIgnoreCase) ? PolicySettingDestinationType.Registry : PolicySettingDestinationType.CSP);
			this.Type = (string)destinationElement.LocalAttribute("Type");
			this.Path = (string)destinationElement.LocalAttribute("Path");
			this.InitProvisionPath(parent.Name, grandparent);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000037E2 File Offset: 0x000019E2
		public PolicySettingDestination(PolicySetting parent, PolicyGroup grandParent)
		{
			this.Destination = PolicySettingDestinationType.None;
			this.Type = null;
			this.Path = null;
			this.InitProvisionPath(parent.Name, grandParent);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x0000380C File Offset: 0x00001A0C
		public PolicySettingDestination(string leafNode, PolicyGroup grandParent)
		{
			this.Destination = PolicySettingDestinationType.None;
			this.Type = null;
			this.Path = null;
			this.InitProvisionPath(leafNode, grandParent);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00003831 File Offset: 0x00001A31
		public string GetResolvedRegistryKey(PolicyMacroTable macroTable)
		{
			return macroTable.ReplaceMacros(this.RegistryKey);
		}

		// Token: 0x06000085 RID: 133 RVA: 0x0000383F File Offset: 0x00001A3F
		public string GetResolvedRegistryValueName(PolicyMacroTable macroTable)
		{
			return macroTable.ReplaceMacros(this.RegistryValueName);
		}

		// Token: 0x06000086 RID: 134 RVA: 0x0000384D File Offset: 0x00001A4D
		public IEnumerable<string> GetResolvedProvisioningPath(PolicyMacroTable macroTable)
		{
			foreach (string inputString in this.ProvisioningPath)
			{
				yield return PolicyMacroTable.MacroTildeToDollar(macroTable.ReplaceMacros(inputString));
			}
			IEnumerator<string> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00003864 File Offset: 0x00001A64
		private void SetRegistryPath(string registryPath)
		{
			int num = registryPath.LastIndexOf('\\');
			if (num < 0)
			{
				throw new ArgumentException("registryPath doesn't appear to be a valid registry path. Could not find separator character.");
			}
			this.RegistryKey = registryPath.Substring(0, num);
			this.RegistryValueName = registryPath.Substring(num + 1);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000038A8 File Offset: 0x00001AA8
		private void InitProvisionPath(string leafNode, PolicyGroup grandParent)
		{
			if (this.Destination == PolicySettingDestinationType.CSP)
			{
				this.ProvisioningPath = this.Path.Split(new char[]
				{
					'/'
				}).AsEnumerable<string>();
				return;
			}
			List<string> list = new List<string>();
			list.Add("MCSF");
			if (grandParent != null)
			{
				list.Add(grandParent.Path);
			}
			if (leafNode != null)
			{
				list.Add(leafNode);
			}
			this.ProvisioningPath = list;
			if (this.Destination == PolicySettingDestinationType.Registry)
			{
				this.SetRegistryPath(this.Path);
			}
		}

		// Token: 0x0400003E RID: 62
		public const string RegistryIntegerType = "REG_DWORD";

		// Token: 0x0400003F RID: 63
		public const string RegistryStringType = "REG_SZ";

		// Token: 0x04000040 RID: 64
		public const string RegistryExpandType = "REG_EXPAND_SZ";

		// Token: 0x04000041 RID: 65
		public const string RegistryMultistring = "REG_MULTI_SZ";

		// Token: 0x04000042 RID: 66
		public const string RegistryBinary = "REG_BINARY";

		// Token: 0x04000043 RID: 67
		public const string CspIntegerType = "CFG_DATATYPE_INTEGER";

		// Token: 0x04000044 RID: 68
		public const string CspStringType = "CFG_DATATYPE_STRING";

		// Token: 0x04000045 RID: 69
		public const string CspMultistringType = "CFG_DATATYPE_MULTIPLE_STRING";

		// Token: 0x04000046 RID: 70
		public const string CspBooleanType = "CFG_DATATYPE_BOOLEAN";

		// Token: 0x04000047 RID: 71
		public const string CspBinaryType = "CFG_DATATYPE_BINARY";

		// Token: 0x04000048 RID: 72
		public const string CspUnknownType = "CFG_DATATYPE_UNKNOWN";

		// Token: 0x04000049 RID: 73
		private const string RegistrySourceName = "RegistrySource";

		// Token: 0x0400004A RID: 74
		private const string DestinationType = "Type";

		// Token: 0x0400004B RID: 75
		private const string DestinationPath = "Path";

		// Token: 0x0400004C RID: 76
		private const string McsfNode = "MCSF";
	}
}
