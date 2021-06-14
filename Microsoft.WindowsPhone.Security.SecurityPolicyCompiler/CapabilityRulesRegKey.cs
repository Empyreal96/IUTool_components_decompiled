using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200002A RID: 42
	public class CapabilityRulesRegKey : RuleWithPathInput
	{
		// Token: 0x0600014A RID: 330 RVA: 0x000078D1 File Offset: 0x00005AD1
		public CapabilityRulesRegKey()
		{
			base.RuleType = "RegKey";
			base.RuleInheritanceInfo = true;
			base.Inheritance = "CI";
		}

		// Token: 0x0600014B RID: 331 RVA: 0x000078F6 File Offset: 0x00005AF6
		public CapabilityRulesRegKey(CapabilityOwnerType value) : this()
		{
			base.OwnerType = value;
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00007905 File Offset: 0x00005B05
		protected sealed override void AddAttributes(XmlElement BasicRuleXmlElement)
		{
			base.AddAttributes(BasicRuleXmlElement);
			base.Flags |= 515U;
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00007920 File Offset: 0x00005B20
		protected sealed override void ValidateOutPath()
		{
			if ((base.NormalizedPath.StartsWith("HKEY_USERS", GlobalVariables.GlobalStringComparison) && !base.NormalizedPath.StartsWith("HKEY_USERS\\.DEFAULT", GlobalVariables.GlobalStringComparison)) || base.NormalizedPath.StartsWith("HKEY_CURRENT_USER", GlobalVariables.GlobalStringComparison))
			{
				throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "It is not allowed to define a capability rule or private resource for registry key '{0}' under HKEY_USERS or HKEY_CURRENT_USER.", new object[]
				{
					base.Path
				}));
			}
			if (ConstantStrings.BlockedRegPathRegexes.Any((Regex blockedPathRegex) => blockedPathRegex.IsMatch(base.NormalizedPath)))
			{
				bool flag = false;
				foreach (string value in ConstantStrings.AllowedRegPaths)
				{
					if (base.NormalizedPath.Equals(value, GlobalVariables.GlobalStringComparison))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					uint num = 1343029254U;
					if ((AccessRightHelper.MergeAccessRight(base.ResolvedRights, base.Path, base.RuleType) & num) != 0U)
					{
						throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "It is not allowed to define a capability rule or private resource for write access on registry key '{0}'.", new object[]
						{
							base.Path
						}));
					}
				}
			}
		}
	}
}
