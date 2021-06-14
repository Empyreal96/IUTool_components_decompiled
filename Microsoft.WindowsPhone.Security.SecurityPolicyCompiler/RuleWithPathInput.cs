using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200002C RID: 44
	public abstract class RuleWithPathInput : BaseRule
	{
		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000154 RID: 340 RVA: 0x00007B4D File Offset: 0x00005D4D
		// (set) Token: 0x06000155 RID: 341 RVA: 0x00007B55 File Offset: 0x00005D55
		protected bool RuleInheritanceInfo
		{
			get
			{
				return this.pathContainsInheritanceInfo;
			}
			set
			{
				this.pathContainsInheritanceInfo = value;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000156 RID: 342 RVA: 0x00007B5E File Offset: 0x00005D5E
		// (set) Token: 0x06000157 RID: 343 RVA: 0x00007B66 File Offset: 0x00005D66
		[XmlAttribute(AttributeName = "Path")]
		public string Path
		{
			get
			{
				return this.path;
			}
			set
			{
				this.path = value;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000158 RID: 344 RVA: 0x00007B6F File Offset: 0x00005D6F
		[XmlIgnore]
		protected string NormalizedPath
		{
			get
			{
				return NormalizedString.Get(this.path);
			}
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00007B8F File Offset: 0x00005D8F
		public override void Add(IXPathNavigable BasicRuleXmlElement, string appCapSID, string svcCapSID)
		{
			this.AddAttributes((XmlElement)BasicRuleXmlElement);
			this.CompileAttributes(appCapSID, svcCapSID);
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00007BA5 File Offset: 0x00005DA5
		protected override void AddAttributes(XmlElement BasicRuleXmlElement)
		{
			base.AddAttributes(BasicRuleXmlElement);
			this.inPath = BasicRuleXmlElement.GetAttribute("Path");
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00007BBF File Offset: 0x00005DBF
		protected override void CompileAttributes(string appCapSID, string svcCapSID)
		{
			this.ResolvePathMacroAndInheritance();
			base.CompileAttributes(appCapSID, svcCapSID);
			this.ValidateOutPath();
			base.CalculateElementId(this.NormalizedPath);
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00007BE4 File Offset: 0x00005DE4
		protected virtual PathInheritanceType ResolvePathMacroAndInheritance()
		{
			string text = this.inPath;
			PathInheritanceType result = PathInheritanceType.NotInheritable;
			if (this.pathContainsInheritanceInfo)
			{
				int num = text.IndexOf("\\(*)", GlobalVariables.GlobalStringComparison);
				if (num >= 0 && num == text.Length - "\\(*)".Length)
				{
					result = PathInheritanceType.ContainerObjectInheritable;
				}
				else
				{
					if (num != -1)
					{
						throw new PolicyCompilerInternalException("Compile path does not have required inheritance info: type= " + base.RuleType);
					}
					num = text.IndexOf("\\(+)", GlobalVariables.GlobalStringComparison);
					if (num == -1 || num != text.Length - "\\(+)".Length)
					{
						throw new PolicyCompilerInternalException("Compile path does not have required inheritance info: type= " + base.RuleType);
					}
					result = PathInheritanceType.ContainerObjectInheritable_InheritOnly;
				}
				text = text.Substring(0, num);
			}
			this.path = base.ResolveMacro(text, "Path");
			return result;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x00007CAC File Offset: 0x00005EAC
		protected void ValidateFileOutPath(bool IsChamberProfile)
		{
			if (!IsChamberProfile && ((this.NormalizedPath.StartsWith("\\DATA\\USERS\\", GlobalVariables.GlobalStringComparison) && !this.NormalizedPath.StartsWith("\\DATA\\USERS\\PUBLIC", GlobalVariables.GlobalStringComparison)) || this.NormalizedPath.Equals("\\DATA\\USERS", GlobalVariables.GlobalStringComparison)))
			{
				throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "'{0}' should not be defined in capability rule or private resource.", new object[]
				{
					this.Path
				}));
			}
			if (ConstantStrings.BlockedFilePathRegexes.Any((Regex blockedPathRegex) => blockedPathRegex.IsMatch(this.NormalizedPath)))
			{
				throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "'{0}' should not be defined in capability rule or private resource.", new object[]
				{
					this.Path
				}));
			}
			if (this.NormalizedPath.StartsWith("\\DATA\\", GlobalVariables.GlobalStringComparison) || this.NormalizedPath.Equals("\\DATA", GlobalVariables.GlobalStringComparison))
			{
				if (base.RuleType == "File")
				{
					throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "It is not allowed to define a capability rule for a file '{0}' under \\DATA, only Directory rule is allowed.", new object[]
					{
						this.Path
					}));
				}
			}
			else if (!GlobalVariables.IsInPackageAllowList && !IsChamberProfile)
			{
				uint num = 1343029526U;
				if ((AccessRightHelper.MergeAccessRight(base.ResolvedRights, this.Path, base.RuleType) & num) != 0U)
				{
					throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "It is not allowed to grants write access on '{0}'. Only the folders under \\DATA can be granted write access.", new object[]
					{
						this.Path
					}));
				}
			}
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00005340 File Offset: 0x00003540
		protected virtual void ValidateOutPath()
		{
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00007E14 File Offset: 0x00006014
		public override string GetAttributesString()
		{
			return base.GetAttributesString() + this.NormalizedPath;
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00007E27 File Offset: 0x00006027
		public override void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel4, base.RuleType + "Rule");
			base.Print();
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel5, "Path", this.Path);
		}

		// Token: 0x04000101 RID: 257
		private string inPath;

		// Token: 0x04000102 RID: 258
		private bool pathContainsInheritanceInfo;

		// Token: 0x04000103 RID: 259
		private string path = "Not Calculated";
	}
}
