using System;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000033 RID: 51
	public class DriverSecurity
	{
		// Token: 0x060001A7 RID: 423 RVA: 0x00008ED4 File Offset: 0x000070D4
		public string GetSddlString(string infSectionName, string oldSddl, IXPathNavigable driverPolicyDocument, IXPathNavigable driverRuleTemplateDocument)
		{
			XmlNode xmlNode = (XmlDocument)driverPolicyDocument;
			StringBuilder stringBuilder = new StringBuilder();
			if (string.IsNullOrEmpty(oldSddl))
			{
				stringBuilder.Append("D:P(A;;GA;;;SY)");
			}
			else
			{
				stringBuilder.Append(oldSddl);
			}
			string xpath = "//WP_Policy:Security[@InfSectionName='" + infSectionName + "']";
			XmlNode xmlNode2 = xmlNode.SelectSingleNode(xpath, GlobalVariables.NamespaceManager);
			if (xmlNode2 == null)
			{
				throw new PolicyCompilerInternalException("The driver security element can't be found: " + infSectionName);
			}
			if (xmlNode2.ChildNodes.Count > 0)
			{
				XmlNodeList childNodes = xmlNode2.ChildNodes;
				this.AddAccessControlEntries(stringBuilder, childNodes);
			}
			string attribute = ((XmlElement)xmlNode2).GetAttribute("RuleTemplate");
			if (!string.IsNullOrEmpty(attribute))
			{
				XmlDocument xmlDocument = (XmlDocument)driverRuleTemplateDocument;
				new XmlNamespaceManager(xmlDocument.NameTable).AddNamespace("WP_Policy", "urn:Microsoft.WindowsPhone/PackageSchema.v8.00");
				xpath = "//WP_Policy:DriverRule[@Name='" + attribute + "']";
				xmlNode2 = xmlDocument.SelectSingleNode(xpath, GlobalVariables.NamespaceManager);
				if (xmlNode2 == null)
				{
					throw new PolicyCompilerInternalException("The driver template rule element can't be found: " + attribute);
				}
				if (xmlNode2.ChildNodes.Count > 0)
				{
					XmlNodeList childNodes2 = xmlNode2.ChildNodes;
					this.AddAccessControlEntries(stringBuilder, childNodes2);
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x00008FEC File Offset: 0x000071EC
		private void AddAccessControlEntries(StringBuilder driverSddlString, XmlNodeList rulesXmlElementList)
		{
			foreach (object obj in rulesXmlElementList)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string appCapSID = string.Empty;
				string svcCapSID = string.Empty;
				string text = string.Empty;
				string empty = string.Empty;
				string empty2 = string.Empty;
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					XmlElement xmlElement = (XmlElement)xmlNode;
					string localName = xmlElement.LocalName;
					DriverRule driverRule;
					if (!(localName == "AccessedByCapability"))
					{
						if (!(localName == "AccessedByService"))
						{
							if (!(localName == "AccessedByApplication"))
							{
								throw new PolicyCompilerInternalException("Internal Error: Driver Security element has a invalid type: " + localName);
							}
							appCapSID = SidBuilder.BuildApplicationSidString(xmlElement.GetAttribute("Name"));
							driverRule = new DriverRule(localName);
						}
						else
						{
							svcCapSID = SidBuilder.BuildServiceSidString(xmlElement.GetAttribute("Name"));
							driverRule = new DriverRule(localName);
						}
					}
					else
					{
						text = xmlElement.GetAttribute("Id");
						if (ConstantStrings.PredefinedServiceCapabilities.Contains(text))
						{
							svcCapSID = text;
							appCapSID = null;
						}
						else
						{
							svcCapSID = GlobalVariables.SidMapping[text];
							appCapSID = SidBuilder.BuildApplicationCapabilitySidString(text);
						}
						driverRule = new DriverRule(localName);
					}
					if (driverRule != null)
					{
						driverRule.Add(xmlElement, appCapSID, svcCapSID);
						driverSddlString.Append(driverRule.DACL);
					}
				}
			}
		}

		// Token: 0x0400012A RID: 298
		public const string NodeDriverSecurity = "//WP_Policy:Security[@InfSectionName='";

		// Token: 0x0400012B RID: 299
		public const string NodeDriverRule = "//WP_Policy:DriverRule[@Name='";
	}
}
