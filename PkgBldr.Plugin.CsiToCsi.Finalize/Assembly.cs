using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Base.Security;
using Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy;
using Microsoft.CompPlat.PkgBldr.Interfaces;
using Microsoft.CompPlat.PkgBldr.Tools;

namespace Microsoft.CompPlat.PkgBldr.Plugins
{
	// Token: 0x02000002 RID: 2
	[Export(typeof(IPkgPlugin))]
	internal class Assembly : PkgPlugin
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public override void ConvertEntries(XElement parent, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement component)
		{
			XNamespace @namespace = parent.Name.Namespace;
			GlobalSecurity globalSecurity = enviorn.GlobalSecurity;
			this.logger = enviorn.Logger;
			bool isNeutral = false;
			if (enviorn.build.satellite.Type == SatelliteType.Neutral)
			{
				isNeutral = true;
			}
			if (globalSecurity.SddlListsAreEmpty())
			{
				return;
			}
			enviorn.Bld.CSI.Root = parent;
			enviorn.Pass = BuildPass.PLUGIN_PASS;
			enviorn.Macros = new MacroResolver();
			enviorn.Macros.Load(XmlReader.Create(PkgGenResources.GetResourceStream("Macros_CsiToWm.xml")));
			XElement xelement = parent.Element(@namespace + "trustInfo");
			XElement xelement2 = null;
			if (xelement == null)
			{
				xelement = new XElement(@namespace + "trustInfo");
				XElement xelement3 = new XElement(@namespace + "security");
				XElement xelement4 = new XElement(@namespace + "accessControl");
				xelement2 = new XElement(@namespace + "securityDescriptorDefinitions");
				xelement.Add(xelement3);
				xelement3.Add(xelement4);
				xelement4.Add(xelement2);
				parent.Add(xelement);
			}
			xelement2 = xelement.Descendants(@namespace + "securityDescriptorDefinitions").First<XElement>();
			this.WriteFileSddls(globalSecurity, parent, xelement2);
			if (globalSecurity.DirSddlList.Count > 0)
			{
				XElement xelement5 = parent.Element(@namespace + "directories");
				if (xelement5 == null)
				{
					xelement5 = new XElement(@namespace + "directories");
					parent.Add(xelement5);
				}
				this.WriteDirectorySddls(globalSecurity, xelement5, xelement2);
			}
			if (globalSecurity.RegKeySddlList.Count > 0)
			{
				XElement xelement6 = parent.Element(@namespace + "registryKeys");
				if (xelement6 == null)
				{
					xelement6 = new XElement(@namespace + "registryKeys");
					parent.Add(xelement6);
				}
				this.WriteRegKeySddls(globalSecurity, xelement6, xelement2, isNeutral);
			}
			this.WriteServiceSddls(globalSecurity, parent, xelement2);
			if (globalSecurity.SdRegValuelList.Count > 0)
			{
				XElement registryKeys = PkgBldrHelpers.AddIfNotFound(parent, "registryKeys");
				this.WriteRegValueSddls(globalSecurity, registryKeys, xelement2);
			}
			if (globalSecurity.WnfValueList.Count > 0)
			{
				foreach (KeyValuePair<string, WnfValue> keyValuePair in globalSecurity.WnfValueList)
				{
					if (keyValuePair.Value.Name != null)
					{
						this.logger.LogInfo("WNF security descriptors need to be added to WNF manifests! Please add the following to the WNF notification {0}\nsddl={1}", new object[]
						{
							keyValuePair.Value.Name,
							keyValuePair.Value.SecurityDescriptor
						});
					}
					else
					{
						this.logger.LogInfo("WNF security descriptors need to be added to WNF manifests! Please add the following to the WNF manifest with Tag={0} for the notification with scope={1} and sequence={2}\nsddl={3}", new object[]
						{
							keyValuePair.Value.Tag,
							keyValuePair.Value.Scope,
							keyValuePair.Value.Sequence,
							keyValuePair.Value.SecurityDescriptor
						});
					}
				}
			}
			if (xelement2.Elements().Count<XElement>() == 0)
			{
				xelement.Remove();
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002344 File Offset: 0x00000544
		private void WriteRegValueSddls(GlobalSecurity gSecurity, XElement registryKeys, XElement securityDescriptorDefinitions)
		{
			foreach (KeyValuePair<string, SdRegValue> keyValuePair in gSecurity.SdRegValuelList)
			{
				XElement xelement = new XElement(registryKeys.Name.Namespace + "registryKey");
				xelement.Add(new XAttribute("keyName", keyValuePair.Value.GetRegPath()));
				XElement xelement2 = new XElement(xelement.Name.Namespace + "registryValue");
				xelement2.Add(new XAttribute("name", keyValuePair.Value.GetRegValueName()));
				xelement2.Add(new XAttribute("valueType", keyValuePair.Value.RegValueType));
				xelement2.Add(new XAttribute("value", keyValuePair.Value.GetRegValue()));
				xelement.Add(xelement2);
				if (keyValuePair.Value.HasAdditionalValue())
				{
					XElement xelement3 = new XElement(xelement.Name.Namespace + "registryValue");
					xelement3.Add(new XAttribute("name", keyValuePair.Value.GetRegValueName(true)));
					xelement3.Add(new XAttribute("valueType", keyValuePair.Value.RegValueType));
					xelement3.Add(new XAttribute("value", keyValuePair.Value.GetRegValue(true)));
					xelement.Add(xelement3);
				}
				registryKeys.Add(xelement);
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002514 File Offset: 0x00000714
		private void WriteServiceSddls(GlobalSecurity gSecurity, XElement Parent, XElement securityDescriptorDefinitions)
		{
			foreach (KeyValuePair<string, string> keyValuePair in gSecurity.ServiceSddlList)
			{
				XElement xelement = this.FindMatchingAttribute(Parent, "serviceData", "name", keyValuePair.Key);
				if (xelement == null)
				{
					this.logger.LogWarning("TBD: Error, sddl ref to non existing service {0}", new object[]
					{
						keyValuePair.Key
					});
				}
				else if (xelement != null && xelement.Element(Parent.Name.Namespace + "securityDescriptor") != null)
				{
					this.logger.LogInfo("TBD: Merge new Service SDDL with the existing security descriptor value", new object[0]);
				}
				else
				{
					string value = HashCalculator.CalculateSha256Hash(keyValuePair.Key);
					xelement.Add(new XElement(Parent.Name.Namespace + "securityDescriptor", new XAttribute("name", value)));
					XElement xelement2 = new XElement(Parent.Name.Namespace + "securityDescriptorDefinition");
					xelement2.Add(new XAttribute("name", value));
					xelement2.Add(new XAttribute("sddl", keyValuePair.Value));
					securityDescriptorDefinitions.Add(xelement2);
				}
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002684 File Offset: 0x00000884
		private void WriteFileSddls(GlobalSecurity gSecurity, XElement Parent, XElement securityDescriptorDefinitions)
		{
			foreach (KeyValuePair<string, string> keyValuePair in gSecurity.FileSddlList)
			{
				string directoryName = LongPath.GetDirectoryName(keyValuePair.Key);
				string fileName = LongPath.GetFileName(keyValuePair.Key);
				XElement xelement = this.FindMatchingChildAttributes(Parent, "file", "destinationPath", directoryName, "name", fileName);
				if (xelement == null)
				{
					this.logger.LogWarning("TBD: Error, sddl ref to non existing file {0}", new object[]
					{
						fileName
					});
				}
				else if (xelement != null && xelement.Element(xelement.Name.Namespace + "securityDescriptor") != null)
				{
					this.logger.LogInfo("TBD: Merge new File SDDL with the existing security descriptor value", new object[0]);
				}
				else
				{
					string value = HashCalculator.CalculateSha256Hash(keyValuePair.Key);
					xelement.Add(new XElement(Parent.Name.Namespace + "securityDescriptor", new XAttribute("name", value)));
					XElement xelement2 = new XElement(Parent.Name.Namespace + "securityDescriptorDefinition");
					xelement2.Add(new XAttribute("name", value));
					xelement2.Add(new XAttribute("sddl", keyValuePair.Value));
					securityDescriptorDefinitions.Add(xelement2);
				}
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002810 File Offset: 0x00000A10
		private void WriteDirectorySddls(GlobalSecurity gSecurity, XElement csiDirectories, XElement securityDescriptorDefinitions)
		{
			foreach (KeyValuePair<string, string> keyValuePair in gSecurity.DirSddlList)
			{
				XElement xelement = this.FindMatchingChildAttribute(csiDirectories, "directory", "destinationPath", keyValuePair.Key);
				if (xelement != null)
				{
					if (xelement.Element(csiDirectories.Name.Namespace + "securityDescriptor") != null)
					{
						this.logger.LogInfo("TBD: Merge new Directory SDDL with the existing security descriptor value", new object[0]);
						continue;
					}
				}
				else
				{
					xelement = new XElement(csiDirectories.Name.Namespace + "directory");
					xelement.Add(new XAttribute("destinationPath", keyValuePair.Key));
					csiDirectories.Add(xelement);
				}
				string value = HashCalculator.CalculateSha256Hash(keyValuePair.Key);
				xelement.Add(new XElement(csiDirectories.Name.Namespace + "securityDescriptor", new XAttribute("name", value)));
				XElement xelement2 = new XElement(csiDirectories.Name.Namespace + "securityDescriptorDefinition");
				xelement2.Add(new XAttribute("name", value));
				xelement2.Add(new XAttribute("sddl", keyValuePair.Value));
				securityDescriptorDefinitions.Add(xelement2);
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002998 File Offset: 0x00000B98
		private void WriteRegKeySddls(GlobalSecurity gSecurity, XElement csiRegKeys, XElement securityDescriptorDefinitions, bool isNeutral)
		{
			foreach (KeyValuePair<string, string> keyValuePair in gSecurity.RegKeySddlList)
			{
				XElement xelement = this.FindMatchingChildAttribute(csiRegKeys, "registryKey", "keyName", keyValuePair.Key);
				if (xelement != null)
				{
					if (xelement.Element(csiRegKeys.Name.Namespace + "securityDescriptor") != null)
					{
						this.logger.LogInfo("TBD: Merge new RegKey SDDL with the existing security descriptor value", new object[0]);
						continue;
					}
				}
				else if (isNeutral)
				{
					xelement = new XElement(csiRegKeys.Name.Namespace + "registryKey");
					xelement.Add(new XAttribute("keyName", keyValuePair.Key));
					csiRegKeys.Add(xelement);
				}
				if (xelement != null)
				{
					string value = HashCalculator.CalculateSha256Hash(keyValuePair.Key);
					xelement.Add(new XElement(csiRegKeys.Name.Namespace + "securityDescriptor", new XAttribute("name", value)));
					XElement xelement2 = new XElement(csiRegKeys.Name.Namespace + "securityDescriptorDefinition");
					xelement2.Add(new XAttribute("name", value));
					xelement2.Add(new XAttribute("sddl", keyValuePair.Value));
					securityDescriptorDefinitions.Add(xelement2);
				}
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002B28 File Offset: 0x00000D28
		public XElement FindMatchingChildAttribute(XElement Parent, string ElementName, string AttributeName, string AttributeValue)
		{
			XElement result = null;
			IEnumerable<XElement> enumerable = from el in Parent.Elements(Parent.Name.Namespace + ElementName)
			where el.Attribute(AttributeName).Value.Equals(AttributeValue, StringComparison.OrdinalIgnoreCase)
			select el;
			if (enumerable != null && enumerable.Count<XElement>() > 0)
			{
				result = enumerable.First<XElement>();
			}
			return result;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002B8C File Offset: 0x00000D8C
		public XElement FindMatchingAttribute(XElement Parent, string ElementName, string AttributeName, string AttributeValue)
		{
			XElement result = null;
			IEnumerable<XElement> enumerable = from el in Parent.Descendants(Parent.Name.Namespace + ElementName)
			where el.Attribute(AttributeName).Value.Equals(AttributeValue, StringComparison.OrdinalIgnoreCase)
			select el;
			if (enumerable != null && enumerable.Count<XElement>() > 0)
			{
				result = enumerable.First<XElement>();
			}
			return result;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002BF0 File Offset: 0x00000DF0
		public XElement FindMatchingChildAttributes(XElement Parent, string ElementName, string AttributeName1, string AttributeValue1, string AttributeName2, string AttributeValue2)
		{
			XElement result = null;
			IEnumerable<XElement> enumerable = from el in Parent.Elements(Parent.Name.Namespace + ElementName)
			where el.Attribute(AttributeName1).Value.Equals(AttributeValue1, StringComparison.OrdinalIgnoreCase) && el.Attribute(AttributeName2).Value.Equals(AttributeValue2, StringComparison.OrdinalIgnoreCase)
			select el;
			if (enumerable != null && enumerable.Count<XElement>() > 0)
			{
				result = enumerable.First<XElement>();
			}
			return result;
		}

		// Token: 0x04000001 RID: 1
		private IDeploymentLogger logger;
	}
}
