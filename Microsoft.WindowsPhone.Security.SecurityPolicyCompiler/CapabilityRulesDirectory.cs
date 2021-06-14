using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000023 RID: 35
	[XmlInclude(typeof(CapabilityRulesInstallationFolder))]
	[XmlInclude(typeof(CapabilityRulesChamberProfileDefaultDataFolder))]
	[XmlInclude(typeof(CapabilityRulesChamberProfileShellContentFolder))]
	[XmlInclude(typeof(CapabilityRulesChamberProfileMediaFolder))]
	[XmlInclude(typeof(CapabilityRulesChamberProfilePlatformDataFolder))]
	[XmlInclude(typeof(CapabilityRulesChamberProfileLiveTilesFolder))]
	public class CapabilityRulesDirectory : RuleWithPathInput
	{
		// Token: 0x0600012F RID: 303 RVA: 0x00006BC9 File Offset: 0x00004DC9
		public CapabilityRulesDirectory()
		{
			base.RuleType = "Directory";
			base.RuleInheritanceInfo = true;
			base.Inheritance = "CIOI";
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00006BEE File Offset: 0x00004DEE
		public CapabilityRulesDirectory(CapabilityOwnerType value) : this()
		{
			base.OwnerType = value;
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00006BFD File Offset: 0x00004DFD
		protected override void AddAttributes(XmlElement BasicRuleXmlElement)
		{
			base.AddAttributes(BasicRuleXmlElement);
			base.Flags |= 259U;
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00006C18 File Offset: 0x00004E18
		protected override PathInheritanceType ResolvePathMacroAndInheritance()
		{
			PathInheritanceType pathInheritanceType = base.ResolvePathMacroAndInheritance();
			if (PathInheritanceType.ContainerObjectInheritable == pathInheritanceType)
			{
				base.Inheritance = "CIOI";
			}
			else if (PathInheritanceType.ContainerObjectInheritable_InheritOnly == pathInheritanceType)
			{
				base.Inheritance = "IOCIOI";
			}
			return pathInheritanceType;
		}

		// Token: 0x06000133 RID: 307 RVA: 0x00006BC0 File Offset: 0x00004DC0
		protected override void ValidateOutPath()
		{
			base.ValidateFileOutPath(false);
		}
	}
}
