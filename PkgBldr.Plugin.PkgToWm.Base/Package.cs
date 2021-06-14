using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using BuildFilterExpressionEvaluator;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;
using Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi;
using Microsoft.CompPlat.PkgBldr.Tools;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000010 RID: 16
	[Export(typeof(IPkgPlugin))]
	internal class Package : PkgPlugin
	{
		// Token: 0x0600002E RID: 46 RVA: 0x00003578 File Offset: 0x00001778
		private void FullyExpandPolicyMacros(XElement customizationPolicy, Config environ)
		{
			foreach (XElement xelement in customizationPolicy.Descendants())
			{
				string attributeValue = PkgBldrHelpers.GetAttributeValue(xelement, "Path");
				if (attributeValue != null)
				{
					string text = null;
					try
					{
						text = environ.Macros.Resolve(attributeValue);
						text = environ.PolicyMacros.Resolve(text);
					}
					catch
					{
						environ.Logger.LogWarning("Could not fully expand policy Macro in path {0}, please add this macro to Macros_Policy.xml", new object[]
						{
							attributeValue
						});
					}
					if (text != null)
					{
						xelement.Attribute("Path").Value = text;
					}
				}
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00003630 File Offset: 0x00001830
		public override void ConvertEntries(XElement toWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement fromPkg)
		{
			enviorn.ExitStatus = ExitStatus.SUCCESS;
			enviorn.Macros = new MacroResolver();
			enviorn.Macros.Load(XmlReader.Create(PkgGenResources.GetResourceStream("Macros_PkgToWm.xml")));
			enviorn.Macros.Register("resId", "$(resId)");
			enviorn.PolicyMacros = new MacroResolver();
			enviorn.PolicyMacros.Load(XmlReader.Create(PkgGenResources.GetResourceStream("Macros_Policy.xml")));
			if (enviorn.Bld.BuildMacros != null)
			{
				Dictionary<string, Macro> macroTable = enviorn.Bld.BuildMacros.GetMacroTable();
				enviorn.Macros.Register(macroTable, true);
			}
			enviorn.Pass = BuildPass.PLUGIN_PASS;
			enviorn.Bld.WM.Root = toWm;
			enviorn.Bld.PKG.Root = fromPkg;
			FileConverter arg = new FileConverter(enviorn);
			enviorn.arg = arg;
			XNamespace ns = "urn:Microsoft.CompPlat/ManifestSchema.v1.00";
			toWm.Name = ns + "identity";
			toWm.Add(new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"));
			toWm.Add(new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"));
			string attributeValue = PkgBldrHelpers.GetAttributeValue(fromPkg, "Owner");
			string attributeValue2 = PkgBldrHelpers.GetAttributeValue(fromPkg, "Component");
			string attributeValue3 = PkgBldrHelpers.GetAttributeValue(fromPkg, "SubComponent");
			enviorn.Bld.PKG.Component = attributeValue2;
			enviorn.Bld.PKG.SubComponent = attributeValue3;
			string text = attributeValue;
			string text2 = null;
			string text3;
			if (string.IsNullOrEmpty(attributeValue3))
			{
				text3 = attributeValue2;
			}
			else
			{
				text2 = attributeValue2;
				text3 = attributeValue3;
			}
			string pattern = "[^A-Za-z0-9\\-]";
			text = Regex.Replace(text, pattern, "-");
			text3 = Regex.Replace(text3, pattern, "-");
			toWm.Add(new XAttribute("owner", text));
			toWm.Add(new XAttribute("name", text3));
			if (!string.IsNullOrEmpty(text2))
			{
				text2 = Regex.Replace(text2, pattern, "-");
				toWm.Add(new XAttribute("namespace", text2));
			}
			string text4 = text + "-";
			if (text2 != null)
			{
				text4 = text4 + text2 + "-";
			}
			text4 += text3;
			if (enviorn.AutoGenerateOutput)
			{
				enviorn.Output = enviorn.Output.TrimEnd(new char[]
				{
					'\\'
				}) + "\\" + text4;
				enviorn.Output += ".wm.xml";
			}
			enviorn.Bld.PKG.Partition = "MainOS";
			string text5;
			foreach (XAttribute xattribute in fromPkg.Attributes())
			{
				text5 = xattribute.Name.LocalName;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text5);
				if (num <= 757530448U)
				{
					if (num <= 405146044U)
					{
						if (num != 382723710U)
						{
							if (num == 405146044U)
							{
								if (text5 == "OwnerType")
								{
									enviorn.Bld.PKG.OwnerType = xattribute.Value;
								}
							}
						}
						else if (text5 == "ReleaseType")
						{
							enviorn.Bld.PKG.ReleaseType = xattribute.Value;
						}
					}
					else if (num != 718440320U)
					{
						if (num != 753180090U)
						{
							if (num == 757530448U)
							{
								if (!(text5 == "SubComponent"))
								{
								}
							}
						}
						else if (text5 == "BuildWow")
						{
							toWm.Add(new XAttribute("buildWow", "true"));
						}
					}
					else if (!(text5 == "Component"))
					{
					}
				}
				else if (num <= 1749011418U)
				{
					if (num != 1096353051U)
					{
						if (num != 1725856265U)
						{
							if (num == 1749011418U)
							{
								if (text5 == "BinaryPartition")
								{
									enviorn.Logger.LogWarning("<Package> BinaryPartition not converted", new object[0]);
								}
							}
						}
						else if (!(text5 == "Description"))
						{
						}
					}
					else if (text5 == "GroupingKey")
					{
						enviorn.Logger.LogWarning("<Package> GroupingKey not converted", new object[0]);
					}
				}
				else if (num != 2079518418U)
				{
					if (num != 2602745460U)
					{
						if (num == 3077057705U)
						{
							if (text5 == "Partition")
							{
								enviorn.Bld.PKG.Partition = xattribute.Value;
							}
						}
					}
					else if (!(text5 == "Owner"))
					{
					}
				}
				else if (!(text5 == "Platform"))
				{
				}
			}
			foreach (BuildPass pass in (BuildPass[])Enum.GetValues(typeof(BuildPass)))
			{
				enviorn.Pass = pass;
				base.ConvertEntries(toWm, plugins, enviorn, fromPkg);
			}
			List<XElement> list = toWm.Descendants(fromPkg.Name.Namespace + "SettingsGroup").ToList<XElement>();
			if (list.Count != 0)
			{
				string text6 = enviorn.Output.Replace(".wm.xml", ".policy.xml");
				XElement xelement = new XElement(fromPkg.Name.Namespace + "CustomizationPolicy");
				foreach (XElement xelement2 in list)
				{
					xelement.Add(xelement2);
					xelement2.Remove();
				}
				this.FullyExpandPolicyMacros(xelement, enviorn);
				XDocument xdocument = new XDocument(new object[]
				{
					xelement
				});
				enviorn.Logger.LogInfo("Saving: {0}", new object[]
				{
					text6
				});
				xdocument.Save(text6);
				XElement xelement3 = new XElement(toWm.Name.Namespace + "files");
				XElement xelement4 = new XElement(toWm.Name.Namespace + "file");
				xelement4.Add(new XAttribute("source", string.Format("$(build.nttree)\\bin\\retail\\{0}", Path.GetFileName(text6))));
				xelement4.Add(new XAttribute("destinationDir", "$(runtime.windows)\\CustomizationPolicy"));
				xelement3.Add(xelement4);
				toWm.Add(xelement3);
			}
			this.RemoveRawServiceTriggers(toWm, enviorn);
			if (enviorn.Bld.JsonDepot == null)
			{
				enviorn.Logger.LogWarning("Partition and other onecoreCapable attributes are lost unless /json:depotName is specifed on the command line.", new object[0]);
				return;
			}
			string text7 = Environment.ExpandEnvironmentVariables("%sdxroot%\\" + enviorn.Bld.JsonDepot);
			if (!Directory.Exists(text7))
			{
				throw new PkgGenException(string.Format("Can't find directory {0}", text7));
			}
			string attributeValue4 = PkgBldrHelpers.GetAttributeValue(toWm, "owner");
			string attributeValue5 = PkgBldrHelpers.GetAttributeValue(toWm, "namespace");
			string attributeValue6 = PkgBldrHelpers.GetAttributeValue(toWm, "name");
			string attributeValue7 = PkgBldrHelpers.GetAttributeValue(toWm, "buildWow");
			text5 = enviorn.Bld.PKG.Partition.ToLowerInvariant();
			string targetPartition;
			if (!(text5 == "efiesp"))
			{
				if (!(text5 == "updateos"))
				{
					if (!(text5 == "mainos"))
					{
						if (!(text5 == "data"))
						{
							if (!(text5 == "plat"))
							{
								enviorn.Logger.LogWarning("Partition {0} not supported, setting it to mainos", new object[0]);
								targetPartition = "mainos";
							}
							else
							{
								targetPartition = "plat";
							}
						}
						else
						{
							targetPartition = "data";
						}
					}
					else
					{
						targetPartition = "mainos";
					}
				}
				else
				{
					targetPartition = "updateos";
				}
			}
			else
			{
				targetPartition = "efiesp";
			}
			text5 = enviorn.Bld.PKG.ReleaseType.ToLowerInvariant();
			string releaseType;
			if (!(text5 == "production"))
			{
				if (!(text5 == "test"))
				{
					enviorn.Logger.LogWarning("ReleaseType {0} not supported, setting it to production", new object[0]);
					releaseType = "production";
				}
				else
				{
					releaseType = "test";
				}
			}
			else
			{
				releaseType = "production";
			}
			string text8 = Identity.WmIdentityNameToCsiAssemblyName(attributeValue4, attributeValue5, attributeValue6, null);
			string jsonPackageName = text8;
			string jsonDepot = enviorn.Bld.JsonDepot;
			XElement xelement5 = toWm.Element(toWm.Name.Namespace + "language");
			bool isMultilingual = false;
			bool hasLangContent = false;
			if (xelement5 != null)
			{
				hasLangContent = true;
				string attributeValue8 = PkgBldrHelpers.GetAttributeValue(xelement5, "multilingual");
				if (attributeValue8 != null && attributeValue8.Equals("true", StringComparison.InvariantCultureIgnoreCase))
				{
					isMultilingual = true;
				}
			}
			string jsonStr = this.GenerateJsonAsString(jsonPackageName, targetPartition, releaseType, text8, jsonDepot, attributeValue7, hasLangContent, isMultilingual);
			string jsonFileIdentity = Package.JsonFileIdentity(text8);
			string projFileDir = this.CreatePbxProjFileForComponent(enviorn.Output, text8, jsonDepot, enviorn, true);
			this.CreateJsonInDepotPkggenDirectory(jsonStr, jsonFileIdentity, jsonDepot, projFileDir, enviorn, true);
			this.CreateJsonsForResolutionComponents(enviorn, toWm, false, projFileDir, jsonPackageName, text8, releaseType, jsonDepot, targetPartition);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00003FB0 File Offset: 0x000021B0
		private void CreateJsonsForResolutionComponents(Config enviorn, XElement toWm, bool mirrorDepot, string projFileDir, string jsonPackageName, string csiComponentName, string releaseType, string depot, string targetPartition)
		{
			List<XElement> list = PkgBldrHelpers.FindMatchingAttributes(toWm, "files", "resolution").ToList<XElement>();
			list.AddRange(PkgBldrHelpers.FindMatchingAttributes(toWm, "regKeys", "resolution").ToList<XElement>());
			BuildFilterExpressionEvaluator buildFilterExpressionEvaluator = new BuildFilterExpressionEvaluator();
			if (enviorn.Bld.AllResolutions != null)
			{
				foreach (SatelliteId satelliteId in enviorn.Bld.AllResolutions)
				{
					buildFilterExpressionEvaluator.SetVariable(satelliteId.Id, false);
				}
				List<string> list2 = new List<string>();
				foreach (SatelliteId satelliteId2 in enviorn.Bld.AllResolutions)
				{
					buildFilterExpressionEvaluator.SetVariable(satelliteId2.Id, true);
					foreach (XElement element in list)
					{
						string text = PkgBldrHelpers.GetAttributeValue(element, "resolution").ToLowerInvariant();
						bool flag = false;
						try
						{
							flag = (text.Equals("*", StringComparison.InvariantCultureIgnoreCase) || buildFilterExpressionEvaluator.Evaluate(text));
						}
						catch
						{
							enviorn.Logger.LogWarning("Not generating a JSON for resolution = {0}, please add it to /resolutions:", new object[0]);
						}
						if (flag && !list2.Contains(satelliteId2.Id))
						{
							list2.Add(satelliteId2.Id);
							string jsonPackageName2 = jsonPackageName + "_" + satelliteId2.FileSuffix;
							string csiComponentName2 = csiComponentName + "_" + satelliteId2.FileSuffix;
							string jsonStr = this.GenerateJsonAsString(jsonPackageName2, targetPartition, releaseType, csiComponentName2, depot, null, false, false);
							string jsonFileIdentity = Package.JsonFileIdentity(csiComponentName2);
							this.CreateJsonInDepotPkggenDirectory(jsonStr, jsonFileIdentity, depot, projFileDir, enviorn, mirrorDepot);
						}
					}
					buildFilterExpressionEvaluator.SetVariable(satelliteId2.Id, false);
				}
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x0000420C File Offset: 0x0000240C
		private string GenerateJsonAsString(string jsonPackageName, string targetPartition, string releaseType, string csiComponentName, string depot, string buildWow, bool hasLangContent, bool isMultilingual)
		{
			string newLine = Environment.NewLine;
			StringBuilder stringBuilder = new StringBuilder("{" + newLine);
			stringBuilder.Append(string.Format("\"package\": \"{0}\"," + newLine, jsonPackageName));
			stringBuilder.Append("\"onecoreCapable\": \"true\"," + newLine);
			if (depot.Equals("phone", StringComparison.InvariantCultureIgnoreCase))
			{
				stringBuilder.Append("\"codesets\": \"cs_phone\"," + newLine);
			}
			else
			{
				stringBuilder.Append("\"codesets\": \"cs_phone,cs_windows,cs_xbox\"," + newLine);
			}
			stringBuilder.Append("\"onecorePackageInfo\": {" + newLine);
			stringBuilder.Append(string.Format("\"targetPartition\": \"{0}\"," + newLine, targetPartition));
			stringBuilder.Append(string.Format("\"releaseType\": \"{0}\"" + newLine, releaseType));
			stringBuilder.Append("}," + newLine);
			stringBuilder.Append("\"components\": [" + newLine);
			stringBuilder.Append("{" + newLine);
			stringBuilder.Append(string.Format("\"component\": \"{0}\"," + newLine, csiComponentName));
			stringBuilder.Append(string.Format("\"depot\": \"{0}\"," + newLine, depot));
			if (!string.IsNullOrEmpty(buildWow))
			{
				stringBuilder.Append("\"wow\": \"wow64\"," + newLine);
			}
			if (hasLangContent)
			{
				if (isMultilingual)
				{
					stringBuilder.Append("\"multilingual\": \"true\"," + newLine);
				}
				else
				{
					stringBuilder.Append("\"languageResources\": \"true\"," + newLine);
				}
			}
			if (depot.Equals("phone", StringComparison.InvariantCultureIgnoreCase))
			{
				stringBuilder.Append("\"codesets\": \"cs_phone\"" + newLine);
			}
			else
			{
				stringBuilder.Append("\"codesets\": \"cs_phone,cs_windows,cs_xbox\"" + newLine);
			}
			stringBuilder.Append("}" + newLine);
			stringBuilder.Append("]" + newLine);
			stringBuilder.Append("}" + newLine);
			return stringBuilder.ToString();
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000043F4 File Offset: 0x000025F4
		private void RemoveRawServiceTriggers(XElement wmXml, Config enviorn)
		{
			IEnumerable<XElement> enumerable = PkgBldrHelpers.FindMatchingAttributes(wmXml, "service", "name");
			IEnumerable<XElement> enumerable2 = PkgBldrHelpers.FindMatchingAttributes(wmXml, "regKey", "keyName");
			List<XElement> list = new List<XElement>();
			foreach (XElement xelement in enumerable2)
			{
				if (Regex.Match(PkgBldrHelpers.GetAttributeValue(xelement, "keyName"), "\\\\[^\\\\]+\\\\TriggerInfo\\\\[0-9]+$", RegexOptions.IgnoreCase).Success)
				{
					list.Add(xelement);
				}
			}
			foreach (XElement xelement2 in enumerable)
			{
				foreach (XElement xelement3 in list)
				{
					XElement parent = xelement3.Parent;
					string[] array = PkgBldrHelpers.GetAttributeValue(xelement3, "keyName").Split(new char[]
					{
						'\\'
					});
					if (array[array.Length - 3].Equals(PkgBldrHelpers.GetAttributeValue(xelement2, "name"), StringComparison.InvariantCultureIgnoreCase))
					{
						XElement xelement4 = this.ConvertRegValuesToServiceTriggerInfo(xelement3, enviorn);
						if (xelement4 != null)
						{
							xelement2.Add(xelement4);
							xelement3.Remove();
							if (!parent.HasElements)
							{
								parent.Remove();
							}
						}
					}
				}
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x0000455C File Offset: 0x0000275C
		private XElement ConvertRegValuesToServiceTriggerInfo(XElement regKey, Config enviorn)
		{
			XElement xelement = new XElement(regKey.Name.Namespace + "serviceTrigger");
			string text = null;
			string text2 = null;
			string text3 = null;
			foreach (XElement element in regKey.Elements(regKey.Name.Namespace + "regValue"))
			{
				string attributeValue = PkgBldrHelpers.GetAttributeValue(element, "name");
				string text4 = PkgBldrHelpers.GetAttributeValue(element, "value");
				string attributeValue2 = PkgBldrHelpers.GetAttributeValue(element, "type");
				if (attributeValue.Equals("action", StringComparison.InvariantCultureIgnoreCase))
				{
					text4 = text4.TrimStart("xX0".ToCharArray());
					if (!(text4 == "1"))
					{
						if (!(text4 == "2"))
						{
							enviorn.Logger.LogWarning(string.Format("Service action {0} not supported", text4), new object[0]);
							return null;
						}
						text2 = "stop";
					}
					else
					{
						text2 = "start";
					}
				}
				if (attributeValue.Equals("type", StringComparison.InvariantCultureIgnoreCase))
				{
					if (!attributeValue2.Equals("reg_dword", StringComparison.InvariantCultureIgnoreCase))
					{
						enviorn.Logger.LogWarning(string.Format("Service trigger type {0} not supported", attributeValue2), new object[0]);
						continue;
					}
					text4 = text4.TrimStart("xX0".ToCharArray());
					uint num = <PrivateImplementationDetails>.ComputeStringHash(text4);
					if (num <= 856466825U)
					{
						if (num <= 822911587U)
						{
							if (num != 806133968U)
							{
								if (num == 822911587U)
								{
									if (text4 == "4")
									{
										text = "FirewallPortEvent";
										goto IL_2C0;
									}
								}
							}
							else if (text4 == "5")
							{
								text = "GroupPolicyChange";
								goto IL_2C0;
							}
						}
						else if (num != 839689206U)
						{
							if (num == 856466825U)
							{
								if (text4 == "6")
								{
									text = "NetworkEndpointEvent";
									goto IL_2C0;
								}
							}
						}
						else if (text4 == "7")
						{
							text = "WnfStateEvent";
							goto IL_2C0;
						}
					}
					else if (num <= 906799682U)
					{
						if (num != 873244444U)
						{
							if (num == 906799682U)
							{
								if (text4 == "3")
								{
									text = "DomainAvailability";
									goto IL_2C0;
								}
							}
						}
						else if (text4 == "1")
						{
							text = "DeviceInterfaceArrival";
							goto IL_2C0;
						}
					}
					else if (num != 923577301U)
					{
						if (num == 2381486463U)
						{
							if (text4 == "20")
							{
								text = "Custom";
								goto IL_2C0;
							}
						}
					}
					else if (text4 == "2")
					{
						text = "IPAddressAvailability";
						goto IL_2C0;
					}
					enviorn.Logger.LogWarning(string.Format("Cant convert service trigger type = {0}", text4), new object[0]);
					return null;
				}
				IL_2C0:
				if (attributeValue.Equals("guid", StringComparison.InvariantCultureIgnoreCase))
				{
					string a = text4.ToUpperInvariant();
					if (!(a == "67D190BC70943941A9BABE0BBBF5B74D"))
					{
						if (!(a == "16287A2D5E0CFC459CE7570E5ECDE9C9"))
						{
							enviorn.Logger.LogWarning(string.Format("Can't map service trigger sub type = {0}", text4), new object[0]);
							return null;
						}
						text3 = "WNF_STATE_CHANGE";
					}
					else
					{
						text3 = "RPC_INTERFACE_EVENT";
					}
				}
				if (Regex.Match(attributeValue, "data.$", RegexOptions.IgnoreCase).Success)
				{
					char c = attributeValue.ToCharArray()[attributeValue.Length - 1];
					if (!attributeValue2.Equals("reg_binary", StringComparison.InvariantCultureIgnoreCase))
					{
						enviorn.Logger.LogWarning(string.Format("Can't map service trigger data type = {0}", text4), new object[0]);
						return null;
					}
					XElement xelement2 = new XElement(regKey.Name.Namespace + "triggerData");
					xelement2.Add(new XAttribute("type", "binary"));
					xelement2.Add(new XAttribute("value", text4));
					xelement.Add(xelement2);
				}
				if (Regex.Match(attributeValue, "datatype.$", RegexOptions.IgnoreCase).Success)
				{
					char c2 = attributeValue.ToCharArray()[attributeValue.Length - 1];
				}
			}
			if (text2 == null)
			{
				enviorn.Logger.LogWarning("Can't convert service trigger, action is a required attribute", new object[0]);
				return null;
			}
			xelement.Add(new XAttribute("action", text2));
			if (text == null)
			{
				enviorn.Logger.LogWarning("Can't convert service trigger, type is a required attribute", new object[0]);
				return null;
			}
			xelement.Add(new XAttribute("type", text));
			if (text3 == null)
			{
				enviorn.Logger.LogWarning("Can't convert service trigger, subtype is a required attribute", new object[0]);
				return null;
			}
			xelement.Add(new XAttribute("subtype", text3));
			return xelement;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00004A3C File Offset: 0x00002C3C
		private static string JsonFileIdentity(string csiComponentName)
		{
			string result = csiComponentName;
			if (csiComponentName.StartsWith("microsoft-windows-", StringComparison.InvariantCultureIgnoreCase))
			{
				result = csiComponentName.Substring(18);
			}
			return result;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00004A64 File Offset: 0x00002C64
		private void CreateJsonInDepotPkggenDirectory(string jsonStr, string jsonFileIdentity, string depot, string projFileDir, Config environ, bool mirrorDepot)
		{
			string text = Environment.ExpandEnvironmentVariables(string.Format("%sdxroot%\\{0}\\PkgGen", depot));
			string path = jsonFileIdentity + ".json";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			text = Path.Combine(text, path);
			environ.Logger.LogInfo("Saving: {0}", new object[]
			{
				text
			});
			if (File.Exists(text))
			{
				SdCommand.Run("edit", text);
			}
			File.WriteAllText(text, jsonStr);
			SdCommand.Run("add", text);
			Package.ReformatJsonMan(text, environ);
			string text2 = Path.GetDirectoryName(text);
			text2 = text2 + "\\build\\mbs\\" + jsonFileIdentity;
			if (!Directory.Exists(text2))
			{
				Directory.CreateDirectory(text2);
			}
			string text3 = Path.GetDirectoryName(text2) + "\\dirs";
			if (File.Exists(text3))
			{
				SdCommand.Run("edit", text3);
			}
			this.UpdateDirsWithNewIdentity(text3, Path.GetFileName(text2), true);
			SdCommand.Run("add", text3);
			string arg = Package.ReplaceFirstOccurrence(text, Environment.ExpandEnvironmentVariables("%sdxroot%"), "$(Sdxroot)");
			XElement xelement = XElement.Parse(string.Format("\r\n            <Project\r\n                DefaultTargets=\"ProductBuild\"\r\n                ToolsVersion=\"4.0\"\r\n                >\r\n              <Import Project=\"$(CustomizationsRoot)\\Mbs\\common\\Microsoft.Build.ModularBuild.ManifestProjectConfiguration.props\" />\r\n              <ItemGroup>\r\n                 <ManifestDefinitionFiles Include=\"{0}\" />\r\n              </ItemGroup>\r\n              <Import Project=\"$(CustomizationsRoot)\\Mbs\\common\\Microsoft.Build.ModularBuild.ManifestProjectConfiguration.targets\"/>\r\n            </Project>", arg));
			Package.SetProjNameSpace(xelement);
			XDocument xdocument = new XDocument(new object[]
			{
				xelement
			});
			string text4 = Path.Combine(text2, "product.pbxproj");
			environ.Logger.LogInfo(string.Format("Saving: {0}", text4), new object[0]);
			if (File.Exists(text4))
			{
				SdCommand.Run("edit", text4);
			}
			xdocument.Save(text4);
			SdCommand.Run("add", text4);
			string text5 = Path.Combine(Path.GetDirectoryName(text4), "sources.dep");
			if (File.Exists(text5))
			{
				SdCommand.Run("edit", text5);
				File.Delete(text5);
			}
			projFileDir = Path.GetFullPath(projFileDir);
			projFileDir = Package.ReplaceFirstOccurrence(projFileDir, Environment.ExpandEnvironmentVariables("%sdxroot%\\"), "");
			if (mirrorDepot)
			{
				projFileDir = Package.PathReplace(projFileDir, 0, depot);
			}
			using (StreamWriter streamWriter = new StreamWriter(text5))
			{
				streamWriter.WriteLine("PUBLIC_PASS0_CONSUMES= \\");
				streamWriter.WriteLine("    redist\\mspartners\\netfx45\\core\\binary_release|PASS0");
				streamWriter.WriteLine();
				streamWriter.WriteLine("BUILD_PASS3_CONSUMES= \\");
				streamWriter.WriteLine(string.Format("    {0}|PASS3 \\", projFileDir.ToLowerInvariant()));
			}
			SdCommand.Run("add", text5);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00004CBC File Offset: 0x00002EBC
		private void UpdateDirsWithNewIdentity(string dirsPath, string identity, bool sortEntries)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			if (File.Exists(dirsPath))
			{
				foreach (string text in File.ReadLines(dirsPath).ToList<string>())
				{
					string text2 = text.Trim(" \t \\".ToCharArray());
					if (!string.IsNullOrEmpty(text2) && !text2.Equals("DIRS=", StringComparison.InvariantCultureIgnoreCase) && !hashSet.Contains(text2))
					{
						hashSet.Add(text2);
					}
				}
				File.Delete(dirsPath);
			}
			if (!hashSet.Contains(identity))
			{
				hashSet.Add(identity);
			}
			using (StreamWriter streamWriter = new StreamWriter(dirsPath))
			{
				streamWriter.WriteLine("DIRS= \\");
				List<string> list = hashSet.ToList<string>();
				if (sortEntries)
				{
					list.Sort();
				}
				foreach (string arg in list)
				{
					streamWriter.WriteLine(string.Format("   {0} \\", arg));
				}
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00004DF4 File Offset: 0x00002FF4
		private string CreatePbxProjFileForComponent(string wmXmlPath, string componentIdentityName, string depot, Config environ, bool mirrorDepot)
		{
			string directoryName = Path.GetDirectoryName(wmXmlPath);
			string text3;
			if (true)
			{
				string text = Path.GetFullPath(Path.Combine(directoryName, "..\\components\\")).TrimEnd(new char[]
				{
					'\\'
				});
				string text2 = Path.Combine(Path.GetDirectoryName(text), "dirs");
				if (File.Exists(text2))
				{
					SdCommand.Run("edit", text2);
				}
				this.UpdateDirsWithNewIdentity(text2, "components", false);
				SdCommand.Run("add", text2);
				text3 = Path.Combine(text, componentIdentityName);
				if (!Directory.Exists(text3))
				{
					Directory.CreateDirectory(text3);
				}
				text2 = Path.Combine(Path.GetDirectoryName(text3), "dirs");
				if (File.Exists(text2))
				{
					SdCommand.Run("edit", text2);
				}
				this.UpdateDirsWithNewIdentity(text2, componentIdentityName, true);
				SdCommand.Run("add", text2);
				text3 = Path.Combine(text3, "product.pbxproj");
			}
			else
			{
				text3 = Path.Combine(directoryName, "product.pbxproj");
			}
			if (File.Exists(text3))
			{
				SdCommand.Run("edit", text3);
				File.Delete(text3);
			}
			XElement xelement = XElement.Parse(string.Format("\r\n            <Project\r\n                DefaultTargets=\"ProductBuild\"\r\n                ToolsVersion=\"4.0\"\r\n                >\r\n                <Import Project=\"$(CustomizationsRoot)\\Mbs\\common\\Microsoft.Build.ModularBuild.ManifestProjectConfiguration.props\" />\r\n                <ItemGroup />\r\n                <Import Project=\"$(CustomizationsRoot)\\Mbs\\common\\Microsoft.Build.ModularBuild.ManifestProjectConfiguration.targets\"/>\r\n            </Project>", new object[0]));
			Package.SetProjNameSpace(xelement);
			string text4 = Package.ReplaceFirstOccurrence(wmXmlPath, Environment.ExpandEnvironmentVariables("%sdxroot%"), "$(Sdxroot)");
			if (mirrorDepot)
			{
				text4 = Package.PathReplace(text4, 1, depot);
			}
			string text5 = string.Format("\r\n            <WindowsManifest Include=\"{0}\">\r\n                <Type>Windows</Type>\r\n            </WindowsManifest>\r\n            ", text4);
			XContainer xcontainer = xelement.Element(xelement.Name.Namespace + "ItemGroup");
			XElement xelement2 = XElement.Parse(text5);
			Package.SetProjNameSpace(xelement2);
			xcontainer.Add(xelement2);
			XDocument xdocument = new XDocument(new object[]
			{
				xelement
			});
			environ.Logger.LogInfo(string.Format("Saving: {0}", text3), new object[0]);
			xdocument.Save(text3);
			SdCommand.Run("add", text3);
			return Path.GetDirectoryName(text3);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00004FBC File Offset: 0x000031BC
		private static string PathReplace(string path, int position, string depot)
		{
			string[] array = path.Split(new char[]
			{
				'\\'
			});
			array[position] = depot;
			string text = null;
			foreach (string str in array)
			{
				text = text + str + "\\";
			}
			return text.TrimEnd(new char[]
			{
				'\\'
			});
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00005014 File Offset: 0x00003214
		private static void SetProjNameSpace(XElement xProj)
		{
			XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";
			foreach (XElement xelement in xProj.DescendantsAndSelf())
			{
				xelement.Name = ns + xelement.Name.LocalName;
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x0000507C File Offset: 0x0000327C
		public static string ReplaceFirstOccurrence(string Source, string Find, string Replace)
		{
			int startIndex = Source.IndexOf(Find, StringComparison.InvariantCultureIgnoreCase);
			return Source.Remove(startIndex, Find.Length).Insert(startIndex, Replace);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000050A8 File Offset: 0x000032A8
		public static void ReformatJsonMan(string fileName, Config environ)
		{
			Process process = new Process();
			process.StartInfo.FileName = Environment.ExpandEnvironmentVariables("%sdxroot%\\tools\\reformatjsonman.cmd");
			process.StartInfo.Arguments = string.Format("-inplace {0} ", fileName);
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.Start();
			string text = process.StandardOutput.ReadToEnd();
			if (!string.IsNullOrEmpty(text))
			{
				environ.Logger.LogError(text, new object[0]);
				throw new PkgGenException(string.Format("Failed to reformat JSON file {0}", fileName));
			}
			process.WaitForExit();
		}
	}
}
