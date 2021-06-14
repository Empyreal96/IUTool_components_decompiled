using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200002D RID: 45
	public class CapabilityRulesSDRegValue : BaseRule
	{
		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000163 RID: 355 RVA: 0x00007E64 File Offset: 0x00006064
		// (set) Token: 0x06000164 RID: 356 RVA: 0x00007E6C File Offset: 0x0000606C
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

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000165 RID: 357 RVA: 0x00007E75 File Offset: 0x00006075
		// (set) Token: 0x06000166 RID: 358 RVA: 0x00007E7D File Offset: 0x0000607D
		[XmlAttribute(AttributeName = "Type")]
		public string SDRegValueType
		{
			get
			{
				return this.sdRegValueType;
			}
			set
			{
				this.sdRegValueType = value;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000167 RID: 359 RVA: 0x00007E86 File Offset: 0x00006086
		// (set) Token: 0x06000168 RID: 360 RVA: 0x00007E8E File Offset: 0x0000608E
		[XmlAttribute(AttributeName = "SaveAsString")]
		public string SaveAsString
		{
			get
			{
				return this.saveAsString;
			}
			set
			{
				this.saveAsString = value;
			}
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00007E97 File Offset: 0x00006097
		public bool ShouldSerializeSaveAsString()
		{
			return this.saveAsString != "Not Calculated";
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00007EA9 File Offset: 0x000060A9
		public CapabilityRulesSDRegValue()
		{
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00007EE0 File Offset: 0x000060E0
		public CapabilityRulesSDRegValue(string value, CapabilityOwnerType type)
		{
			base.RuleType = value;
			base.OwnerType = type;
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00007B8F File Offset: 0x00005D8F
		public override void Add(IXPathNavigable BasicRuleXmlElement, string appCapSID, string svcCapSID)
		{
			this.AddAttributes((XmlElement)BasicRuleXmlElement);
			this.CompileAttributes(appCapSID, svcCapSID);
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00007F30 File Offset: 0x00006130
		protected override void AddAttributes(XmlElement BasicRuleXmlElement)
		{
			base.AddAttributes(BasicRuleXmlElement);
			string ruleType = base.RuleType;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(ruleType);
			if (num <= 1114269721U)
			{
				if (num <= 601173383U)
				{
					if (num != 439927768U)
					{
						if (num == 601173383U)
						{
							if (ruleType == "WinRTLaunchPermission")
							{
								this.SetWinRTRights(BasicRuleXmlElement.GetAttribute("LaunchPermission"));
								this.inputAttributeName = "ServerName";
								base.Flags |= 262660U;
								goto IL_36A;
							}
						}
					}
					else if (ruleType == "ETWProvider")
					{
						this.inputAttributeName = "Guid";
						base.Flags |= 516U;
						goto IL_36A;
					}
				}
				else if (num != 735153999U)
				{
					if (num == 1114269721U)
					{
						if (ruleType == "SDRegValue")
						{
							this.inputAttributeName = "Path";
							base.Flags |= 4U;
							this.SaveAsString = BasicRuleXmlElement.GetAttribute("SaveAsString");
							string attribute = BasicRuleXmlElement.GetAttribute("SetOwner");
							if (string.IsNullOrEmpty(attribute) || !attribute.Equals("No", GlobalVariables.GlobalStringComparison))
							{
								base.Flags |= 512U;
								goto IL_36A;
							}
							goto IL_36A;
						}
					}
				}
				else if (ruleType == "COMAccessPermission")
				{
					this.SetCOMRights(BasicRuleXmlElement.GetAttribute("AccessPermission"));
					this.inputAttributeName = "AppId";
					base.Flags |= 262660U;
					goto IL_36A;
				}
			}
			else if (num <= 1411464970U)
			{
				if (num != 1127826844U)
				{
					if (num == 1411464970U)
					{
						if (ruleType == "WNF")
						{
							this.inputAttribute = CapabilityRulesSDRegValue.GenerateWnfId(BasicRuleXmlElement.GetAttribute("Scope"), BasicRuleXmlElement.GetAttribute("Tag"), BasicRuleXmlElement.GetAttribute("Sequence"), BasicRuleXmlElement.GetAttribute("DataPermanent"));
							base.Flags |= 4U;
							return;
						}
					}
				}
				else if (ruleType == "DeviceSetupClass")
				{
					this.inputAttributeName = "Guid";
					base.Flags = ((base.Flags & 4294902015U) | 2147483648U | 4U);
					goto IL_36A;
				}
			}
			else if (num != 3562219862U)
			{
				if (num != 3715409434U)
				{
					if (num == 4128555198U)
					{
						if (ruleType == "ServiceAccess")
						{
							this.inputAttributeName = "Name";
							this.SetServiceAccessRights(BasicRuleXmlElement.GetAttribute(this.inputAttributeName), BasicRuleXmlElement.GetAttribute("Rights"));
							base.Flags |= 518U;
							goto IL_36A;
						}
					}
				}
				else if (ruleType == "WinRTAccessPermission")
				{
					this.SetWinRTRights(BasicRuleXmlElement.GetAttribute("AccessPermission"));
					this.inputAttributeName = "ServerName";
					base.Flags |= 262660U;
					goto IL_36A;
				}
			}
			else if (ruleType == "COMLaunchPermission")
			{
				this.SetCOMRights(BasicRuleXmlElement.GetAttribute("LaunchPermission"));
				this.inputAttributeName = "AppId";
				base.Flags |= 262660U;
				goto IL_36A;
			}
			throw new PolicyCompilerInternalException("Undefined rule type: " + base.RuleType);
			IL_36A:
			this.inputAttribute = BasicRuleXmlElement.GetAttribute(this.inputAttributeName);
		}

		// Token: 0x0600016E RID: 366 RVA: 0x000082BC File Offset: 0x000064BC
		private void SetCOMRights(string value)
		{
			if (base.OwnerType != CapabilityOwnerType.Application && base.OwnerType != CapabilityOwnerType.Service)
			{
				base.Rights = value;
				return;
			}
			if (base.RuleType == "COMAccessPermission")
			{
				base.Rights = "$(COM_LOCAL_ACCESS)";
				return;
			}
			base.Rights = "$(COM_LOCAL_LAUNCH)";
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000830C File Offset: 0x0000650C
		private void SetWinRTRights(string value)
		{
			if (base.OwnerType != CapabilityOwnerType.Application && base.OwnerType != CapabilityOwnerType.Service)
			{
				base.Rights = value;
				return;
			}
			if (base.RuleType == "WinRTAccessPermission")
			{
				base.Rights = "$(COM_LOCAL_ACCESS)";
				return;
			}
			base.Rights = "$(COM_LOCAL_LAUNCH)";
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000835C File Offset: 0x0000655C
		private void SetServiceAccessRights(string serviceName, string value)
		{
			if (base.OwnerType != CapabilityOwnerType.Application && base.OwnerType != CapabilityOwnerType.Service)
			{
				base.Rights = value;
				return;
			}
			if (base.ReadOnlyRights)
			{
				base.Rights = "$(GENERIC_READ)";
				return;
			}
			base.Rights = "$(SERVICE_PRIVATE_RESOURCE_ACCESS)";
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00008398 File Offset: 0x00006598
		private static string GenerateWnfId(string scope, string tag, string sequence, string dataPermanent)
		{
			UTF32Encoding utf32Encoding = new UTF32Encoding();
			uint num = 0U;
			uint num2 = 0U;
			uint num3;
			if (!(scope == "System"))
			{
				if (!(scope == "Session"))
				{
					if (!(scope == "User"))
					{
						if (!(scope == "Process"))
						{
							throw new PolicyCompilerInternalException("Undefined WNF scope value: " + scope);
						}
						num3 = 3U;
					}
					else
					{
						num3 = 2U;
					}
				}
				else
				{
					num3 = 1U;
				}
			}
			else
			{
				num3 = 0U;
			}
			byte[] array = new byte[8];
			for (int i = tag.Length - 1; i >= 0; i--)
			{
				Array.Clear(array, 0, 8);
				utf32Encoding.GetBytes(tag, i, 1, array, 0);
				num2 = (num2 << 8) + (uint)BitConverter.ToInt32(array, 0);
			}
			num2 ^= 1103515245U;
			if (!string.IsNullOrEmpty(dataPermanent))
			{
				num = (uint)(Convert.ToInt32(dataPermanent, GlobalVariables.Culture) & 1);
			}
			uint num4 = (uint)(Convert.ToInt32(sequence, GlobalVariables.Culture) & 255);
			uint num5 = 1U | num3 << 6 | num << 10 | num4 << 11;
			num5 ^= 2747007092U;
			return string.Format(GlobalVariables.Culture, "{0:X8}{1:X8}", new object[]
			{
				num2,
				num5
			});
		}

		// Token: 0x06000172 RID: 370 RVA: 0x000084CC File Offset: 0x000066CC
		protected override void CompileAttributes(string appCapSID, string svcCapSID)
		{
			base.CompileAttributes(appCapSID, svcCapSID);
			this.CalculatePathAndType();
			base.ElementId = HashCalculator.CalculateSha256Hash("SDRegValue" + this.Path, false);
			if (base.RuleType == "ServiceAccess")
			{
				uint num = 2U;
				if ((AccessRightHelper.MergeAccessRight(base.ResolvedRights, this.inputAttribute, base.RuleType) & num) != 0U)
				{
					throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "It is not allowed to grant SERVICE_CHANGE_CONFIG on service '{0}'.", new object[]
					{
						this.inputAttribute
					}));
				}
			}
		}

		// Token: 0x06000173 RID: 371 RVA: 0x00008558 File Offset: 0x00006758
		private void CalculatePathAndType()
		{
			string ruleType = base.RuleType;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(ruleType);
			if (num <= 1114269721U)
			{
				if (num <= 601173383U)
				{
					if (num != 439927768U)
					{
						if (num == 601173383U)
						{
							if (ruleType == "WinRTLaunchPermission")
							{
								this.Path = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\WindowsRuntime\\Server\\" + this.inputAttribute + "\\Permissions";
								this.SDRegValueType = "WinRT";
							}
						}
					}
					else if (ruleType == "ETWProvider")
					{
						this.Path = "HKEY_LOCAL_MACHINE\\SYSTEM\\ControlSet001\\Control\\WMI\\Security\\" + this.inputAttribute;
						this.SDRegValueType = "ETWProvider";
					}
				}
				else if (num != 735153999U)
				{
					if (num == 1114269721U)
					{
						if (ruleType == "SDRegValue")
						{
							this.Path = base.ResolveMacro(this.inputAttribute, "Path");
							this.SDRegValueType = "SDRegValue";
						}
					}
				}
				else if (ruleType == "COMAccessPermission")
				{
					this.Path = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Classes\\AppID\\" + this.inputAttribute + "\\AccessPermission";
					this.SDRegValueType = "COM";
				}
			}
			else if (num <= 1411464970U)
			{
				if (num != 1127826844U)
				{
					if (num == 1411464970U)
					{
						if (ruleType == "WNF")
						{
							this.Path = "HKEY_LOCAL_MACHINE\\SYSTEM\\ControlSet001\\Control\\Notifications\\" + this.inputAttribute;
							this.SDRegValueType = "WNF";
						}
					}
				}
				else if (ruleType == "DeviceSetupClass")
				{
					this.Path = "HKEY_LOCAL_MACHINE\\SYSTEM\\ControlSet001\\Control\\Class\\" + this.inputAttribute + "\\Properties\\Security";
					this.SDRegValueType = "DeviceSetupClass";
				}
			}
			else if (num != 3562219862U)
			{
				if (num != 3715409434U)
				{
					if (num == 4128555198U)
					{
						if (ruleType == "ServiceAccess")
						{
							this.Path = "HKEY_LOCAL_MACHINE\\SYSTEM\\ControlSet001\\services\\" + this.inputAttribute + "\\Security\\Security";
							this.SDRegValueType = "ServiceAccess";
						}
					}
				}
				else if (ruleType == "WinRTAccessPermission")
				{
					this.Path = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\WindowsRuntime\\Server\\" + this.inputAttribute + "\\Permissions";
					this.SDRegValueType = "WinRT";
				}
			}
			else if (ruleType == "COMLaunchPermission")
			{
				this.Path = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Classes\\AppID\\" + this.inputAttribute + "\\LaunchPermission";
				this.SDRegValueType = "COM";
			}
			this.Path = NormalizedString.Get(this.Path);
		}

		// Token: 0x06000174 RID: 372 RVA: 0x00008829 File Offset: 0x00006A29
		public override string GetAttributesString()
		{
			if (this.SaveAsString != "Not Calculated")
			{
				return base.GetAttributesString() + this.Path + this.SaveAsString;
			}
			return base.GetAttributesString() + this.Path;
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00008866 File Offset: 0x00006A66
		public override void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel4, base.RuleType + "Rule");
			base.Print();
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel5, "Path", this.Path);
		}

		// Token: 0x04000104 RID: 260
		private string inputAttributeName = string.Empty;

		// Token: 0x04000105 RID: 261
		private string inputAttribute;

		// Token: 0x04000106 RID: 262
		private string path = "Not Calculated";

		// Token: 0x04000107 RID: 263
		private string sdRegValueType = "Not Calculated";

		// Token: 0x04000108 RID: 264
		private string saveAsString = "Not Calculated";
	}
}
