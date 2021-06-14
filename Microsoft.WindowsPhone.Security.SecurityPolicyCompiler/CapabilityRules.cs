using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000013 RID: 19
	public class CapabilityRules
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000073 RID: 115 RVA: 0x00003B8B File Offset: 0x00001D8B
		// (set) Token: 0x06000074 RID: 116 RVA: 0x00003B98 File Offset: 0x00001D98
		[XmlArrayItem(typeof(CapabilityRulesFile), ElementName = "File")]
		[XmlArrayItem(typeof(CapabilityRulesDirectory), ElementName = "Directory")]
		[XmlArrayItem(typeof(CapabilityRulesRegKey), ElementName = "RegKey")]
		[XmlArrayItem(typeof(CapabilityRulesSDRegValue), ElementName = "SDRegValue")]
		[XmlArrayItem(typeof(CapabilityRulesTransientObject), ElementName = "TransientObject")]
		[XmlArrayItem(typeof(CapabilityRulesPrivilege), ElementName = "Privilege")]
		[XmlArrayItem(typeof(CapabilityRulesWindows), ElementName = "WindowsCapability")]
		public RulePolicyElement[] Rules
		{
			get
			{
				return this.allRules.ToArray();
			}
			set
			{
				this.allRules.Clear();
				this.allRules.AddRange(value);
			}
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00003BB1 File Offset: 0x00001DB1
		public CapabilityRules()
		{
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00003BE1 File Offset: 0x00001DE1
		public CapabilityRules(CapabilityOwnerType value)
		{
			this.ownerType = value;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00003C18 File Offset: 0x00001E18
		public void Add(IXPathNavigable capabilityRulesXmlElement, string inAppCapSID, string inSvcCapSID)
		{
			this.appCapSID = inAppCapSID;
			this.svcCapSID = inSvcCapSID;
			this.AddElements((XmlElement)capabilityRulesXmlElement);
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00003C34 File Offset: 0x00001E34
		private void AddElements(XmlElement capabilityRulesXmlElement)
		{
			foreach (object obj in capabilityRulesXmlElement.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					XmlElement xmlElement = (XmlElement)xmlNode;
					string localName = xmlElement.LocalName;
					RulePolicyElement rulePolicyElement = null;
					uint num = <PrivateImplementationDetails>.ComputeStringHash(localName);
					if (num <= 1411464970U)
					{
						if (num <= 664194058U)
						{
							if (num <= 401640321U)
							{
								if (num != 356258747U)
								{
									if (num != 401640321U)
									{
										goto IL_401;
									}
									if (!(localName == "ChamberProfileDataShellContentFolder"))
									{
										goto IL_401;
									}
									rulePolicyElement = new CapabilityRulesChamberProfileShellContentFolder(this.ownerType);
								}
								else
								{
									if (!(localName == "ChamberProfileDataDefaultFolder"))
									{
										goto IL_401;
									}
									rulePolicyElement = new CapabilityRulesChamberProfileDefaultDataFolder(this.ownerType);
								}
							}
							else if (num != 439927768U)
							{
								if (num != 664194058U)
								{
									goto IL_401;
								}
								if (!(localName == "ChamberProfileDataMediaFolder"))
								{
									goto IL_401;
								}
								rulePolicyElement = new CapabilityRulesChamberProfileMediaFolder(this.ownerType);
							}
							else
							{
								if (!(localName == "ETWProvider"))
								{
									goto IL_401;
								}
								goto IL_3B8;
							}
						}
						else if (num <= 879225342U)
						{
							if (num != 723007075U)
							{
								if (num != 879225342U)
								{
									goto IL_401;
								}
								if (!(localName == "COM"))
								{
									goto IL_401;
								}
								this.AddSDRegValueCOMRule(xmlElement);
							}
							else
							{
								if (!(localName == "File"))
								{
									goto IL_401;
								}
								rulePolicyElement = new CapabilityRulesFile(this.ownerType);
							}
						}
						else if (num != 1114269721U)
						{
							if (num != 1127826844U)
							{
								if (num != 1411464970U)
								{
									goto IL_401;
								}
								if (!(localName == "WNF"))
								{
									goto IL_401;
								}
								goto IL_3B8;
							}
							else
							{
								if (!(localName == "DeviceSetupClass"))
								{
									goto IL_401;
								}
								goto IL_3B8;
							}
						}
						else
						{
							if (!(localName == "SDRegValue"))
							{
								goto IL_401;
							}
							goto IL_3B8;
						}
					}
					else if (num <= 3460508192U)
					{
						if (num <= 1868227890U)
						{
							if (num != 1709803557U)
							{
								if (num != 1868227890U)
								{
									goto IL_401;
								}
								if (!(localName == "TransientObject"))
								{
									goto IL_401;
								}
								rulePolicyElement = new CapabilityRulesTransientObject(this.ownerType);
							}
							else
							{
								if (!(localName == "InstallationFolder"))
								{
									goto IL_401;
								}
								rulePolicyElement = new CapabilityRulesInstallationFolder(this.ownerType);
							}
						}
						else if (num != 2485283368U)
						{
							if (num != 3125449749U)
							{
								if (num != 3460508192U)
								{
									goto IL_401;
								}
								if (!(localName == "WindowsCapability"))
								{
									goto IL_401;
								}
								rulePolicyElement = new CapabilityRulesWindows(this.ownerType);
							}
							else
							{
								if (!(localName == "ChamberProfileDataLiveTilesFolder"))
								{
									goto IL_401;
								}
								rulePolicyElement = new CapabilityRulesChamberProfileLiveTilesFolder(this.ownerType);
							}
						}
						else
						{
							if (!(localName == "Directory"))
							{
								goto IL_401;
							}
							rulePolicyElement = new CapabilityRulesDirectory(this.ownerType);
						}
					}
					else if (num <= 3520481279U)
					{
						if (num != 3503166012U)
						{
							if (num != 3520481279U)
							{
								goto IL_401;
							}
							if (!(localName == "WinRT"))
							{
								goto IL_401;
							}
							this.AddSDRegValueWinRTRule(xmlElement);
						}
						else
						{
							if (!(localName == "RegKey"))
							{
								goto IL_401;
							}
							rulePolicyElement = new CapabilityRulesRegKey(this.ownerType);
						}
					}
					else if (num != 4128555198U)
					{
						if (num != 4180493228U)
						{
							if (num != 4260098729U)
							{
								goto IL_401;
							}
							if (!(localName == "ChamberProfileDataPlatformDataFolder"))
							{
								goto IL_401;
							}
							rulePolicyElement = new CapabilityRulesChamberProfilePlatformDataFolder(this.ownerType);
						}
						else
						{
							if (!(localName == "Privilege"))
							{
								goto IL_401;
							}
							rulePolicyElement = new CapabilityRulesPrivilege(this.ownerType);
						}
					}
					else
					{
						if (!(localName == "ServiceAccess"))
						{
							goto IL_401;
						}
						goto IL_3B8;
					}
					IL_412:
					if (rulePolicyElement != null)
					{
						rulePolicyElement.Add(xmlElement, this.appCapSID, this.svcCapSID);
						this.allRules.Add(rulePolicyElement);
						continue;
					}
					continue;
					IL_3B8:
					this.AddSDRegValueRule(xmlElement, localName);
					goto IL_412;
					IL_401:
					throw new PolicyCompilerInternalException("Internal Error: Capability Rule Element has been an invalid type: " + localName);
				}
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000040B8 File Offset: 0x000022B8
		private void AddSDRegValueRule(XmlElement ruleXmlElement, string sdRegValueRuleType)
		{
			CapabilityRulesSDRegValue capabilityRulesSDRegValue = new CapabilityRulesSDRegValue(sdRegValueRuleType, this.ownerType);
			capabilityRulesSDRegValue.Add(ruleXmlElement, this.appCapSID, this.svcCapSID);
			this.allRules.Add(capabilityRulesSDRegValue);
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000040F4 File Offset: 0x000022F4
		private void AddSDRegValueCOMRule(XmlElement ruleXmlElement)
		{
			bool flag = true;
			if (!string.IsNullOrEmpty(ruleXmlElement.GetAttribute("LaunchPermission")))
			{
				this.AddSDRegValueRule(ruleXmlElement, "COMLaunchPermission");
				flag = false;
			}
			if (!string.IsNullOrEmpty(ruleXmlElement.GetAttribute("AccessPermission")))
			{
				this.AddSDRegValueRule(ruleXmlElement, "COMAccessPermission");
				flag = false;
			}
			if (!flag)
			{
				return;
			}
			if (this.ownerType == CapabilityOwnerType.Application || this.ownerType == CapabilityOwnerType.Service)
			{
				this.AddSDRegValueRule(ruleXmlElement, "COMLaunchPermission");
				this.AddSDRegValueRule(ruleXmlElement, "COMAccessPermission");
				return;
			}
			throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "Capability COM doesn't contain {0} or {1} attributes", new object[]
			{
				"LaunchPermission",
				"AccessPermission"
			}));
		}

		// Token: 0x0600007B RID: 123 RVA: 0x0000419C File Offset: 0x0000239C
		private void AddSDRegValueWinRTRule(XmlElement ruleXmlElement)
		{
			bool flag = true;
			string empty = string.Empty;
			if (!string.IsNullOrEmpty(ruleXmlElement.GetAttribute("LaunchPermission")))
			{
				this.AddSDRegValueRule(ruleXmlElement, "WinRTLaunchPermission");
				flag = false;
			}
			if (flag && !string.IsNullOrEmpty(ruleXmlElement.GetAttribute("AccessPermission")))
			{
				this.AddSDRegValueRule(ruleXmlElement, "WinRTAccessPermission");
				flag = false;
			}
			if (!flag)
			{
				return;
			}
			if (this.ownerType == CapabilityOwnerType.Application || this.ownerType == CapabilityOwnerType.Service)
			{
				this.AddSDRegValueRule(ruleXmlElement, "WinRTLaunchPermission");
				return;
			}
			throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "Capability WinRT doesn't contain {0} or {1} attributes", new object[]
			{
				"LaunchPermission",
				"AccessPermission"
			}));
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00004244 File Offset: 0x00002444
		public string GetAllAttributesString()
		{
			string text = string.Empty;
			if (this.allRules != null)
			{
				foreach (RulePolicyElement rulePolicyElement in this.allRules)
				{
					text += rulePolicyElement.GetAttributesString();
				}
			}
			return text;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000042AC File Offset: 0x000024AC
		public void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel3, "CapabilityRules");
			instance.DebugLine(string.Empty);
			if (this.allRules != null)
			{
				foreach (RulePolicyElement rulePolicyElement in this.allRules)
				{
					rulePolicyElement.Print();
				}
			}
		}

		// Token: 0x040000CA RID: 202
		private string appCapSID = string.Empty;

		// Token: 0x040000CB RID: 203
		private string svcCapSID = string.Empty;

		// Token: 0x040000CC RID: 204
		private CapabilityOwnerType ownerType = CapabilityOwnerType.Unknown;

		// Token: 0x040000CD RID: 205
		private List<RulePolicyElement> allRules = new List<RulePolicyElement>();
	}
}
