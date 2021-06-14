using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents;
using Microsoft.WindowsPhone.MCSF.Offline;

namespace Microsoft.WindowsPhone.DeviceManagement.MCSF
{
	// Token: 0x02000002 RID: 2
	public class PkgRegWriter
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		public bool GenerateReadablePolicyXML { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002061 File Offset: 0x00000261
		// (set) Token: 0x06000004 RID: 4 RVA: 0x00002069 File Offset: 0x00000269
		private IMacroResolver MacroResolver { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000005 RID: 5 RVA: 0x00002072 File Offset: 0x00000272
		// (set) Token: 0x06000006 RID: 6 RVA: 0x0000207A File Offset: 0x0000027A
		private SortedSet<string> LocalMacros { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002083 File Offset: 0x00000283
		// (set) Token: 0x06000008 RID: 8 RVA: 0x0000208B File Offset: 0x0000028B
		private string PackageName { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002094 File Offset: 0x00000294
		// (set) Token: 0x0600000A RID: 10 RVA: 0x0000209C File Offset: 0x0000029C
		private string TempPath { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000B RID: 11 RVA: 0x000020A5 File Offset: 0x000002A5
		// (set) Token: 0x0600000C RID: 12 RVA: 0x000020AD File Offset: 0x000002AD
		private XDocument PolicyDoc { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600000D RID: 13 RVA: 0x000020B6 File Offset: 0x000002B6
		// (set) Token: 0x0600000E RID: 14 RVA: 0x000020BE File Offset: 0x000002BE
		private XElement PolicyRoot { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000020C7 File Offset: 0x000002C7
		// (set) Token: 0x06000010 RID: 16 RVA: 0x000020CF File Offset: 0x000002CF
		private OSComponentBuilder OSBuilder { get; set; }

		// Token: 0x06000011 RID: 17 RVA: 0x000020D8 File Offset: 0x000002D8
		private static int StringToInteger(string integerAsString, XElement element)
		{
			int result;
			try
			{
				string text = integerAsString.Trim();
				if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
				{
					result = int.Parse(text.Substring(2), NumberStyles.HexNumber, null);
				}
				else
				{
					result = int.Parse(text, null);
				}
			}
			catch (Exception ex)
			{
				if (ex is FormatException || ex is OverflowException || ex is InvalidCastException)
				{
					throw new PkgXmlException(element, "error: Unable to parse the input string \"{0}\" as an integer.", new object[]
					{
						integerAsString
					});
				}
				throw;
			}
			return result;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000215C File Offset: 0x0000035C
		private string OutputFile(string outputDir, XmlWriterSettings writerSettings)
		{
			Directory.CreateDirectory(outputDir);
			string text = Path.Combine(outputDir, this.PackageName + ".policy.xml");
			using (XmlWriter xmlWriter = XmlWriter.Create(text, writerSettings))
			{
				this.PolicyDoc.WriteTo(xmlWriter);
			}
			return text;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000021B8 File Offset: 0x000003B8
		public PkgRegWriter(string packageName, string tempPath, IMacroResolver macroResolver)
		{
			this.PackageName = packageName;
			this.TempPath = tempPath;
			this.MacroResolver = macroResolver;
			this.OSBuilder = new OSComponentBuilder();
			this.LocalMacros = new SortedSet<string>();
			this.OSBuilder.RegisterMacro("__IMSI", string.Format(null, "~{0}~", new object[]
			{
				"__IMSI"
			}));
			this.OSBuilder.RegisterMacro("__ICCID", string.Format(null, "~{0}~", new object[]
			{
				"__ICCID"
			}));
			this.MacroResolver.Register("__IMSI", string.Format(null, "~{0}~", new object[]
			{
				"__IMSI"
			}));
			this.MacroResolver.Register("__ICCID", string.Format(null, "~{0}~", new object[]
			{
				"__ICCID"
			}));
			this.PolicyDoc = new XDocument();
			this.PolicyRoot = new XElement("CustomizationPolicy");
			this.PolicyDoc.Add(this.PolicyRoot);
			this.GenerateReadablePolicyXML = false;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000022D4 File Offset: 0x000004D4
		public PkgObject ToPkgObject()
		{
			new PolicyStore().LoadPolicyXML(this.PolicyDoc);
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			if (this.GenerateReadablePolicyXML)
			{
				xmlWriterSettings.NewLineChars = "\r\n";
				xmlWriterSettings.Indent = true;
				xmlWriterSettings.IndentChars = "  ";
			}
			string text = Path.Combine(this.TempPath, "CustomizationPolicy");
			text = this.OutputFile(text, xmlWriterSettings);
			string environmentVariable = Environment.GetEnvironmentVariable("BINARY_ROOT");
			if (environmentVariable != null)
			{
				string outputDir = Path.Combine(environmentVariable, "files", "OEMCustomizations", "generatedPolicy");
				this.OutputFile(outputDir, xmlWriterSettings);
			}
			this.OSBuilder.AddFileGroup().AddFile(text, "$(runtime.windows)\\CustomizationPolicy");
			return this.OSBuilder.ToPkgObject();
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002388 File Offset: 0x00000588
		private static bool IsBuiltInMacro(string varName)
		{
			return varName == "__ICCID" || varName == "__IMSI" || varName == "__CANINDEX" || varName == "__MVID" || varName == "__SIMSLOT";
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000023DC File Offset: 0x000005DC
		private static bool IsRunTimeOnlyMacro(string varName)
		{
			return varName == "__ICCID" || varName == "__IMSI" || varName == "__CANINDEX" || varName == "__MVID" || varName == "__SIMSLOT";
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002430 File Offset: 0x00000630
		private bool RegisterVariableMacros(XElement element, string path, bool isImageTimeOnly = false)
		{
			bool result = false;
			string[] array = path.Split(new char[]
			{
				'/'
			});
			foreach (string text in array)
			{
				if (Regex.IsMatch(text, "^\\$\\(.*\\)$"))
				{
					result = true;
					if (text == array[0])
					{
						throw new PkgXmlException(element, "error: When specifying a Multi-Setting group path, there must be at least one non-macro segment at the beginning of the path. ({0})", new object[]
						{
							path
						});
					}
					string text2 = text.Substring(2, text.Length - 3);
					if (Regex.IsMatch(text2, "\\$\\(.*\\)"))
					{
						throw new PkgXmlException(element, "error: nesting macros is not allowed in the group path's Multi-Setting macro (variable name). ({0})", new object[]
						{
							path
						});
					}
					if (!PkgRegWriter.IsBuiltInMacro(text2))
					{
						if (!Regex.IsMatch(text2, "^[a-zA-Z][a-zA-Z_0-9.]*$"))
						{
							throw new PkgXmlException(element, "error: invalid characters in the group path's Multi-Setting macro (variable name). ({0})", new object[]
							{
								path
							});
						}
						if (!this.LocalMacros.Contains(text2))
						{
							this.OSBuilder.RegisterMacro(text2, string.Format(null, "~{0}~", new object[]
							{
								text2
							}));
							this.MacroResolver.Register(text2, string.Format(null, "~{0}~", new object[]
							{
								text2
							}));
							this.LocalMacros.Add(text2);
						}
					}
					else if (isImageTimeOnly && PkgRegWriter.IsRunTimeOnlyMacro(text2))
					{
						throw new PkgXmlException(element, "error: The {0} macro is available for RunTime Settings only.", new object[]
						{
							text2
						});
					}
				}
				else
				{
					if (Regex.IsMatch(text, "\\$\\(.*\\)"))
					{
						throw new PkgXmlException(element, "error: When specifying a Multi-Setting macro, it must be the entire URI segment (no other characters between slashes). ({0})", new object[]
						{
							text
						});
					}
					if (!Regex.IsMatch(text, "^[a-zA-Z0-9]*$"))
					{
						throw new PkgXmlException(element, "error: The SettingsGroup Path and Setting Name should contain only alphanumeric characters and forward slashes. ({0})", new object[]
						{
							text
						});
					}
				}
			}
			return result;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000025E0 File Offset: 0x000007E0
		private bool ValidateRegistryPath(XElement element, string path, bool isImageTimeOnly = false)
		{
			foreach (string text in path.Split(new char[]
			{
				'\\'
			}))
			{
				if (Regex.IsMatch(text, "^\\$\\(.*\\)$"))
				{
					string text2 = text.Substring(2, text.Length - 3);
					if (Regex.IsMatch(text2, "\\$\\(.*\\)"))
					{
						throw new PkgXmlException(element, "error: nesting macros is not allowed in the group path's Multi-Setting macro (variable name). ({0})", new object[]
						{
							path
						});
					}
					if (PkgRegWriter.IsBuiltInMacro(text2) && isImageTimeOnly && PkgRegWriter.IsRunTimeOnlyMacro(text2))
					{
						throw new PkgXmlException(element, "error: The {0} macro is available for RunTime Settings only.", new object[]
						{
							text2
						});
					}
				}
				else if (Regex.IsMatch(text, "\\$\\(.*\\)"))
				{
					throw new PkgXmlException(element, "error: When specifying a Multi-Setting macro, it must be the entire URI segment (no other characters between slashes). ({0})", new object[]
					{
						text
					});
				}
			}
			return true;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000026A4 File Offset: 0x000008A4
		public void WriteSettingsGroup(XElement group)
		{
			bool flag = false;
			this.LocalMacros.Clear();
			this.MacroResolver.BeginLocal();
			string groupPath = null;
			group.WithLocalAttribute("Path", delegate(XAttribute x)
			{
				groupPath = x.Value;
			});
			bool flag2 = this.RegisterVariableMacros(group, groupPath, false);
			bool imageTimeOnly = false;
			XElement xelement = group.LocalElement("Constraints");
			if (xelement != null)
			{
				xelement.WithLocalAttribute("ImageTimeOnly", delegate(XAttribute x)
				{
					imageTimeOnly = x.Value.Equals("Yes", StringComparison.Ordinal);
				});
			}
			List<string> list = new List<string>();
			foreach (XElement xelement2 in group.LocalElements("Asset"))
			{
				string value = xelement2.LocalAttribute("Name").Value;
				if (list.Contains(value))
				{
					throw new PkgXmlException(xelement2, "error: The asset or setting name \"{0}\" has already been used within this Settings Group.", new object[]
					{
						value
					});
				}
				list.Add(value);
				if (xelement2.LocalElement("ValueList") != null || xelement2.LocalElement("MultiStringList") != null)
				{
					flag = true;
				}
			}
			list = new List<string>();
			foreach (XElement xelement3 in group.LocalElements("Setting"))
			{
				string value2 = xelement3.LocalAttribute("Name").Value;
				if (list.Contains(value2))
				{
					throw new PkgXmlException(xelement3, "error: The asset or setting name \"{0}\" has already been used within this Settings Group.", new object[]
					{
						value2
					});
				}
				list.Add(value2);
				if (xelement3.LocalElement("RegistrySource") != null)
				{
					flag = true;
				}
			}
			if (flag && !imageTimeOnly)
			{
				RegistryKeyGroupBuilder groupBuilder = this.OSBuilder.AddRegistryGroup();
				foreach (XElement xelement4 in group.LocalElements("Asset"))
				{
					string assetName = null;
					xelement4.WithLocalAttribute("Name", delegate(XAttribute x)
					{
						assetName = x.Value;
					});
					this.RegisterVariableMacros(xelement4, assetName, false);
					string str = groupPath + "/" + assetName;
					XElement xelement5 = xelement4.LocalElement("MultiStringList");
					if (xelement5 != null)
					{
						string value3 = xelement5.LocalAttribute("Key").Value;
						string value4 = xelement5.LocalAttribute("Value").Value;
						new PkgRegWriter.Setting(str + ".OEMAssets", value3 + "\\" + value4, 7U).BuildRegKey(groupBuilder);
					}
					XElement xelement6 = xelement4.LocalElement("ValueList");
					if (xelement6 != null)
					{
						string oemKey = null;
						string operatorKey = null;
						xelement6.WithLocalAttribute("OEMKey", delegate(XAttribute x)
						{
							oemKey = x.Value;
						});
						xelement6.WithLocalAttribute("MOKey", delegate(XAttribute x)
						{
							operatorKey = x.Value;
						});
						if (oemKey != null)
						{
							new PkgRegWriter.Setting(str + ".OEMAssets/~AssetName~", oemKey + "\\~AssetName~", 1U).BuildRegKey(groupBuilder);
						}
						if (operatorKey != null)
						{
							new PkgRegWriter.Setting(str + ".MOAssets/~AssetName~", operatorKey + "\\~AssetName~", 1U).BuildRegKey(groupBuilder);
						}
					}
				}
				using (IEnumerator<XElement> enumerator = group.LocalElements("Setting").GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						XElement xelement7 = enumerator.Current;
						PkgRegWriter.<>c__DisplayClass48_3 CS$<>8__locals4 = new PkgRegWriter.<>c__DisplayClass48_3();
						CS$<>8__locals4.settingName = null;
						xelement7.WithLocalAttribute("Name", delegate(XAttribute x)
						{
							CS$<>8__locals4.settingName = x.Value;
						});
						bool flag3 = this.RegisterVariableMacros(xelement7, CS$<>8__locals4.settingName, false);
						XElement xelement8 = xelement7.LocalElement("RegistrySource");
						if (xelement8 == null)
						{
							if (xelement7.LocalElement("CspSource") == null)
							{
								throw new PkgXmlException(xelement7, "error: You must specify a valid *Source Element for each non-Asset Setting.", new object[0]);
							}
						}
						else
						{
							CS$<>8__locals4.setting = new PkgRegWriter.Setting();
							CS$<>8__locals4.setting.URI = groupPath + "/" + CS$<>8__locals4.settingName;
							XElement xelement9 = xelement7.LocalElement("AccessType");
							if (xelement9 != null)
							{
								xelement9.WithLocalAttribute("Create", delegate(XAttribute x)
								{
									CS$<>8__locals4.setting.Access &= ~(((PkgRegWriter.Option)Enum.Parse(typeof(PkgRegWriter.Option), x.Value) == PkgRegWriter.Option.No) ? PkgRegWriter.AccessType.Create : PkgRegWriter.AccessType.None);
								});
								xelement9.WithLocalAttribute("Delete", delegate(XAttribute x)
								{
									CS$<>8__locals4.setting.Access &= ~(((PkgRegWriter.Option)Enum.Parse(typeof(PkgRegWriter.Option), x.Value) == PkgRegWriter.Option.No) ? PkgRegWriter.AccessType.Delete : PkgRegWriter.AccessType.None);
								});
								xelement9.WithLocalAttribute("Get", delegate(XAttribute x)
								{
									CS$<>8__locals4.setting.Access &= ~(((PkgRegWriter.Option)Enum.Parse(typeof(PkgRegWriter.Option), x.Value) == PkgRegWriter.Option.No) ? PkgRegWriter.AccessType.Get : PkgRegWriter.AccessType.None);
								});
								xelement9.WithLocalAttribute("Replace", delegate(XAttribute x)
								{
									CS$<>8__locals4.setting.Access &= ~(((PkgRegWriter.Option)Enum.Parse(typeof(PkgRegWriter.Option), x.Value) == PkgRegWriter.Option.No) ? PkgRegWriter.AccessType.Replace : PkgRegWriter.AccessType.None);
								});
								xelement9.WithLocalAttribute("All", delegate(XAttribute x)
								{
									CS$<>8__locals4.setting.Access &= ~(((PkgRegWriter.Option)Enum.Parse(typeof(PkgRegWriter.Option), x.Value) == PkgRegWriter.Option.No) ? PkgRegWriter.AccessType.All : PkgRegWriter.AccessType.None);
								});
							}
							CS$<>8__locals4.setting.SourcePath = xelement8.LocalAttribute("Path").Value;
							string value5 = xelement8.LocalAttribute("Type").Value;
							XAttribute xattribute = xelement8.LocalAttribute("Default");
							XElement xelement10 = xelement7.LocalElement("RegistrySource");
							if (xelement10 != null)
							{
								this.ValidateRegistryPath(xelement8, CS$<>8__locals4.setting.SourcePath, imageTimeOnly);
							}
							string localName = xelement8.Name.LocalName;
							if (localName == "RegistrySource")
							{
								CS$<>8__locals4.setting.Source = PkgRegWriter.SourceType.MCSF_SETTINGSOURCE_REGISTRY;
								int num = CS$<>8__locals4.setting.SourcePath.LastIndexOf('\\');
								if (-1 == num || 2147483647 == num)
								{
									throw new PkgXmlException(xelement8, "error: invalid Source Path ({0}).", new object[]
									{
										CS$<>8__locals4.setting.SourcePath
									});
								}
								if (xattribute != null && (flag2 || flag3))
								{
									throw new PkgXmlException(xelement8, "error: Multi-settings may not supply a default value, as they do not point to an explicit registry location.", new object[0]);
								}
								CS$<>8__locals4.setting.SourceType = (uint)((PkgRegWriter.RegValueType)Enum.Parse(typeof(PkgRegWriter.RegValueType), value5));
								switch (CS$<>8__locals4.setting.SourceType)
								{
								case 1U:
								case 7U:
									CS$<>8__locals4.setting.IsString = true;
									goto IL_64D;
								case 3U:
								case 4U:
									CS$<>8__locals4.setting.IsString = false;
									goto IL_64D;
								}
								throw new PkgXmlException(xelement8, "error: The specified type is an unknown or unsupported type. ({0})", new object[]
								{
									value5
								});
							}
							IL_64D:
							XElement xelement11 = xelement7.LocalElement("Validate");
							if (xelement11 != null)
							{
								PkgRegWriter.SourceType source = CS$<>8__locals4.setting.Source;
								if (source == PkgRegWriter.SourceType.MCSF_SETTINGSOURCE_REGISTRY)
								{
									uint sourceType = CS$<>8__locals4.setting.SourceType;
									if (sourceType == 3U)
									{
										throw new PkgXmlException(xelement11, "error: Validation is not valid for the type {0}.", new object[]
										{
											Enum.GetName(typeof(PkgRegWriter.RegValueType), PkgRegWriter.RegValueType.REG_BINARY)
										});
									}
								}
								if (xelement11.LocalElements("Option").Count<XElement>() == 0)
								{
									CS$<>8__locals4.setting.Validation = PkgRegWriter.ValidationMethodType.MCSF_VALIDATIONMETHOD_RANGE;
									XAttribute xattribute2;
									XAttribute xattribute3;
									if (!CS$<>8__locals4.setting.IsString)
									{
										xattribute2 = xelement11.LocalAttribute("Min");
										xattribute3 = xelement11.LocalAttribute("Max");
										if (xattribute2 == null && xattribute3 == null)
										{
											throw new PkgXmlException(xelement11, "error: A Validate Element for an Integer Setting must contain Option Elements or specify a Min and/or Max value.", new object[0]);
										}
										if (xelement11.LocalAttribute("MinLength") != null || xelement11.LocalAttribute("MaxLength") != null)
										{
											throw new PkgXmlException(xelement11, "error: The MinLength and MaxLength attributes should only be used for validation of String Settings.", new object[0]);
										}
									}
									else
									{
										xattribute2 = xelement11.LocalAttribute("MinLength");
										xattribute3 = xelement11.LocalAttribute("MaxLength");
										if (xattribute2 == null && xattribute3 == null)
										{
											throw new PkgXmlException(xelement11, "error: A Validate Element for a String Setting must contain Option Elements or specify a MinLength and/or MaxLength value.", new object[0]);
										}
										if (xelement11.LocalAttribute("Min") != null || xelement11.LocalAttribute("Max") != null)
										{
											throw new PkgXmlException(xelement11, "error: The Min and Max attributes should only be used for validation of Integer Settings.", new object[0]);
										}
									}
									if (xattribute2 != null)
									{
										CS$<>8__locals4.setting.Min = PkgRegWriter.StringToInteger(xattribute2.Value, xelement11);
										if (CS$<>8__locals4.setting.IsString && CS$<>8__locals4.setting.Min < 0)
										{
											throw new PkgXmlException(xelement11, "error: invalid Min or Max value ({0}).", new object[0]);
										}
									}
									else if (CS$<>8__locals4.setting.IsString)
									{
										CS$<>8__locals4.setting.Min = 0;
									}
									if (xattribute3 != null)
									{
										CS$<>8__locals4.setting.Max = PkgRegWriter.StringToInteger(xattribute3.Value, xelement11);
									}
								}
								else
								{
									CS$<>8__locals4.setting.Validation = PkgRegWriter.ValidationMethodType.MCSF_VALIDATIONMETHOD_OPTION;
									using (IEnumerator<XElement> enumerator2 = xelement11.LocalElements("Option").GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											XElement option = enumerator2.Current;
											if (option != null)
											{
												if (CS$<>8__locals4.setting.IsString)
												{
													XElement option2 = option;
													string localName2 = "Value";
													XmlToLinqExtensions.WithEntityDelegate<XAttribute> del;
													if ((del = CS$<>8__locals4.<>9__11) == null)
													{
														PkgRegWriter.<>c__DisplayClass48_3 CS$<>8__locals6 = CS$<>8__locals4;
														del = (CS$<>8__locals6.<>9__11 = delegate(XAttribute x)
														{
															CS$<>8__locals6.setting.Options.Add(x.Value);
														});
													}
													option2.WithLocalAttribute(localName2, del);
												}
												else
												{
													uint dwordValue = 0U;
													option.WithLocalAttribute("Value", delegate(XAttribute x)
													{
														dwordValue = (uint)PkgRegWriter.StringToInteger(x.Value, option);
													});
													CS$<>8__locals4.setting.Options.Add(Convert.ToString((long)((ulong)dwordValue), 16));
												}
											}
										}
									}
								}
							}
							CS$<>8__locals4.setting.BuildRegKey(groupBuilder);
						}
					}
					goto IL_A92;
				}
			}
			foreach (XElement xelement12 in group.LocalElements("Asset"))
			{
				string assetName = null;
				xelement12.WithLocalAttribute("Name", delegate(XAttribute x)
				{
					assetName = x.Value;
				});
				this.RegisterVariableMacros(xelement12, assetName, false);
			}
			foreach (XElement xelement13 in group.LocalElements("Setting"))
			{
				string settingName = null;
				xelement13.WithLocalAttribute("Name", delegate(XAttribute x)
				{
					settingName = x.Value;
				});
				this.RegisterVariableMacros(xelement13, settingName, imageTimeOnly);
				if (imageTimeOnly)
				{
					XElement xelement14 = xelement13.LocalElement("RegistrySource");
					if (xelement14 != null)
					{
						this.ValidateRegistryPath(xelement14, xelement14.LocalAttribute("Path").Value, imageTimeOnly);
					}
				}
			}
			IL_A92:
			foreach (XElement source2 in group.LocalElements("Setting"))
			{
				XElement xelement15 = source2.LocalElement("RegistrySource");
				if (xelement15 != null)
				{
					XAttribute xattribute4 = xelement15.LocalAttribute("Default");
					if (xattribute4 != null)
					{
						int num2 = xelement15.LocalAttribute("Path").Value.LastIndexOf('\\');
						if (-1 == num2 || 2147483647 == num2)
						{
							throw new PkgXmlException(xelement15, "error: invalid Source Path ({0}).", new object[]
							{
								xelement15.LocalAttribute("Path").Value
							});
						}
						string keyName = xelement15.LocalAttribute("Path").Value.Substring(0, num2);
						string name = xelement15.LocalAttribute("Path").Value.Substring(num2 + 1);
						string value6 = xelement15.LocalAttribute("Type").Value;
						RegistryKeyBuilder registryKeyBuilder = this.OSBuilder.AddRegistryGroup().AddRegistryKey(keyName);
						if ((PkgRegWriter.RegValueType)Enum.Parse(typeof(PkgRegWriter.RegValueType), value6) == PkgRegWriter.RegValueType.REG_DWORD)
						{
							uint num3 = (uint)PkgRegWriter.StringToInteger(xattribute4.Value, xelement15);
							registryKeyBuilder.AddValue(name, value6, Convert.ToString((long)((ulong)num3), 16));
						}
						else
						{
							registryKeyBuilder.AddValue(name, value6, xattribute4.Value);
						}
					}
				}
			}
			XElement xelement16 = new XElement(group);
			foreach (XElement xelement17 in xelement16.DescendantsAndSelf())
			{
				foreach (XAttribute xattribute5 in xelement17.Attributes())
				{
					xattribute5.Value = this.MacroResolver.Resolve(xattribute5.Value);
				}
			}
			this.PolicyRoot.Add(xelement16);
			this.LocalMacros.Clear();
			this.MacroResolver.EndLocal();
		}

		// Token: 0x02000004 RID: 4
		private static class Strings
		{
			// Token: 0x04000009 RID: 9
			public const string AssetMacro = "~AssetName~";

			// Token: 0x0400000A RID: 10
			public const string OEMAssetSuffix = ".OEMAssets";

			// Token: 0x0400000B RID: 11
			public const string MOAssetSuffix = ".MOAssets";

			// Token: 0x0400000C RID: 12
			public const string DatastorePath = "$(hklm.software)\\Microsoft\\MCSF\\Settings";

			// Token: 0x02000014 RID: 20
			public static class Elements
			{
				// Token: 0x0400003F RID: 63
				public const string AccessType = "AccessType";

				// Token: 0x04000040 RID: 64
				public const string Asset = "Asset";

				// Token: 0x04000041 RID: 65
				public const string Constraints = "Constraints";

				// Token: 0x04000042 RID: 66
				public const string CspSource = "CspSource";

				// Token: 0x04000043 RID: 67
				public const string MultiStringList = "MultiStringList";

				// Token: 0x04000044 RID: 68
				public const string Option = "Option";

				// Token: 0x04000045 RID: 69
				public const string RegistrySource = "RegistrySource";

				// Token: 0x04000046 RID: 70
				public const string SettingsGroup = "SettingsGroup";

				// Token: 0x04000047 RID: 71
				public const string Setting = "Setting";

				// Token: 0x04000048 RID: 72
				public const string Validate = "Validate";

				// Token: 0x04000049 RID: 73
				public const string ValueList = "ValueList";
			}

			// Token: 0x02000015 RID: 21
			public static class Attributes
			{
				// Token: 0x0400004A RID: 74
				public const string All = "All";

				// Token: 0x0400004B RID: 75
				public const string Create = "Create";

				// Token: 0x0400004C RID: 76
				public const string Default = "Default";

				// Token: 0x0400004D RID: 77
				public const string Delete = "Delete";

				// Token: 0x0400004E RID: 78
				public const string Description = "Description";

				// Token: 0x0400004F RID: 79
				public const string Get = "Get";

				// Token: 0x04000050 RID: 80
				public const string ImageTimeOnly = "ImageTimeOnly";

				// Token: 0x04000051 RID: 81
				public const string Key = "Key";

				// Token: 0x04000052 RID: 82
				public const string Max = "Max";

				// Token: 0x04000053 RID: 83
				public const string MaxLength = "MaxLength";

				// Token: 0x04000054 RID: 84
				public const string Min = "Min";

				// Token: 0x04000055 RID: 85
				public const string MinLength = "MinLength";

				// Token: 0x04000056 RID: 86
				public const string MOKey = "MOKey";

				// Token: 0x04000057 RID: 87
				public const string Name = "Name";

				// Token: 0x04000058 RID: 88
				public const string OEMKey = "OEMKey";

				// Token: 0x04000059 RID: 89
				public const string Path = "Path";

				// Token: 0x0400005A RID: 90
				public const string Replace = "Replace";

				// Token: 0x0400005B RID: 91
				public const string Type = "Type";

				// Token: 0x0400005C RID: 92
				public const string Value = "Value";
			}

			// Token: 0x02000016 RID: 22
			public static class Values
			{
				// Token: 0x0400005D RID: 93
				public const string AllowedAccess = "AllowedAccess";

				// Token: 0x0400005E RID: 94
				public const string Options = "Options";

				// Token: 0x0400005F RID: 95
				public const string Source = "Source";

				// Token: 0x04000060 RID: 96
				public const string SourcePath = "SourcePath";

				// Token: 0x04000061 RID: 97
				public const string SourceType = "SourceType";

				// Token: 0x04000062 RID: 98
				public const string ValidateMax = "ValidateMax";

				// Token: 0x04000063 RID: 99
				public const string ValidateMin = "ValidateMin";

				// Token: 0x04000064 RID: 100
				public const string ValidationMethod = "ValidationMethod";
			}

			// Token: 0x02000017 RID: 23
			public static class Macros
			{
				// Token: 0x04000065 RID: 101
				public const string ICCID = "__ICCID";

				// Token: 0x04000066 RID: 102
				public const string IMSI = "__IMSI";

				// Token: 0x04000067 RID: 103
				public const string CANINDEX = "__CANINDEX";

				// Token: 0x04000068 RID: 104
				public const string MVID = "__MVID";

				// Token: 0x04000069 RID: 105
				public const string SIMSLOT = "__SIMSLOT";

				// Token: 0x0400006A RID: 106
				public const string URISegmentPattern = "^[a-zA-Z0-9]*$";

				// Token: 0x0400006B RID: 107
				public const string MacroPattern = "^\\$\\(.*\\)$";

				// Token: 0x0400006C RID: 108
				public const string NestedPattern = "\\$\\(.*\\)";

				// Token: 0x0400006D RID: 109
				public const string VariableNamePattern = "^[a-zA-Z][a-zA-Z_0-9.]*$";

				// Token: 0x0400006E RID: 110
				public const string MCSFFormat = "~{0}~";
			}

			// Token: 0x02000018 RID: 24
			public static class Policy
			{
				// Token: 0x0400006F RID: 111
				public const string RootNodeName = "CustomizationPolicy";

				// Token: 0x04000070 RID: 112
				public const string FileExtension = "policy.xml";

				// Token: 0x04000071 RID: 113
				public const string LocalPolicyPath = "CustomizationPolicy";

				// Token: 0x04000072 RID: 114
				public const string DevicePolicyPath = "$(runtime.windows)\\CustomizationPolicy";
			}

			// Token: 0x02000019 RID: 25
			public static class Messages
			{
				// Token: 0x04000073 RID: 115
				public const string DefaultOnMultiSetting = "error: Multi-settings may not supply a default value, as they do not point to an explicit registry location.";

				// Token: 0x04000074 RID: 116
				public const string DuplicateName = "error: The asset or setting name \"{0}\" has already been used within this Settings Group.";

				// Token: 0x04000075 RID: 117
				public const string FirstSegmentMacro = "error: When specifying a Multi-Setting group path, there must be at least one non-macro segment at the beginning of the path. ({0})";

				// Token: 0x04000076 RID: 118
				public const string IncompleteSegmentMacro = "error: When specifying a Multi-Setting macro, it must be the entire URI segment (no other characters between slashes). ({0})";

				// Token: 0x04000077 RID: 119
				public const string IntegerMinMaxLength = "error: The MinLength and MaxLength attributes should only be used for validation of String Settings.";

				// Token: 0x04000078 RID: 120
				public const string InvalidCharactersInURI = "error: The SettingsGroup Path and Setting Name should contain only alphanumeric characters and forward slashes. ({0})";

				// Token: 0x04000079 RID: 121
				public const string InvalidInteger = "error: Unable to parse the input string \"{0}\" as an integer.";

				// Token: 0x0400007A RID: 122
				public const string InvalidMinMaxValue = "error: invalid Min or Max value ({0}).";

				// Token: 0x0400007B RID: 123
				public const string InvalidMinMaxLengthValue = "error: invalid MinLength or MaxLength value ({0}).";

				// Token: 0x0400007C RID: 124
				public const string InvalidPath = "error: invalid Source Path ({0}).";

				// Token: 0x0400007D RID: 125
				public const string InvalidType = "error: The specified type is an unknown or unsupported type. ({0})";

				// Token: 0x0400007E RID: 126
				public const string InvalidVariableName = "error: invalid characters in the group path's Multi-Setting macro (variable name). ({0})";

				// Token: 0x0400007F RID: 127
				public const string NestedMacros = "error: nesting macros is not allowed in the group path's Multi-Setting macro (variable name). ({0})";

				// Token: 0x04000080 RID: 128
				public const string NoSource = "error: You must specify a valid *Source Element for each non-Asset Setting.";

				// Token: 0x04000081 RID: 129
				public const string NoValidatorMinMax = "error: A Validate Element for an Integer Setting must contain Option Elements or specify a Min and/or Max value.";

				// Token: 0x04000082 RID: 130
				public const string NoValidatorMinMaxLength = "error: A Validate Element for a String Setting must contain Option Elements or specify a MinLength and/or MaxLength value.";

				// Token: 0x04000083 RID: 131
				public const string PartnerVariableWithUnderscore = "error: Multi-Setting macros (variable names) must begin with alphabetic characters only.";

				// Token: 0x04000084 RID: 132
				public const string RunTimeMacroInImageTimeSetting = "error: The {0} macro is available for RunTime Settings only.";

				// Token: 0x04000085 RID: 133
				public const string StringMinMax = "error: The Min and Max attributes should only be used for validation of Integer Settings.";

				// Token: 0x04000086 RID: 134
				public const string ValidationUnsupportedType = "error: Validation is not valid for the type {0}.";
			}
		}

		// Token: 0x02000005 RID: 5
		private enum Option
		{
			// Token: 0x0400000E RID: 14
			No,
			// Token: 0x0400000F RID: 15
			Yes
		}

		// Token: 0x02000006 RID: 6
		private enum SourceType
		{
			// Token: 0x04000011 RID: 17
			MCSF_SETTINGSOURCE_REGISTRY = 1
		}

		// Token: 0x02000007 RID: 7
		private enum ValidationMethodType
		{
			// Token: 0x04000013 RID: 19
			MCSF_VALIDATIONMETHOD_NONE,
			// Token: 0x04000014 RID: 20
			MCSF_VALIDATIONMETHOD_RANGE,
			// Token: 0x04000015 RID: 21
			MCSF_VALIDATIONMETHOD_OPTION
		}

		// Token: 0x02000008 RID: 8
		private enum RegValueType
		{
			// Token: 0x04000017 RID: 23
			REG_SZ = 1,
			// Token: 0x04000018 RID: 24
			REG_BINARY = 3,
			// Token: 0x04000019 RID: 25
			REG_DWORD,
			// Token: 0x0400001A RID: 26
			REG_MULTI_SZ = 7
		}

		// Token: 0x02000009 RID: 9
		private enum CfgDataType
		{
			// Token: 0x0400001C RID: 28
			CFG_DATATYPE_INTEGER,
			// Token: 0x0400001D RID: 29
			CFG_DATATYPE_STRING,
			// Token: 0x0400001E RID: 30
			CFG_DATATYPE_BOOLEAN = 5,
			// Token: 0x0400001F RID: 31
			CFG_DATATYPE_BINARY,
			// Token: 0x04000020 RID: 32
			CFG_DATATYPE_MULTIPLE_STRING
		}

		// Token: 0x0200000A RID: 10
		[Flags]
		private enum AccessType
		{
			// Token: 0x04000022 RID: 34
			None = 0,
			// Token: 0x04000023 RID: 35
			Create = 1,
			// Token: 0x04000024 RID: 36
			Delete = 2,
			// Token: 0x04000025 RID: 37
			Get = 4,
			// Token: 0x04000026 RID: 38
			Replace = 8,
			// Token: 0x04000027 RID: 39
			All = 15
		}

		// Token: 0x0200000B RID: 11
		private class Setting
		{
			// Token: 0x1700000A RID: 10
			// (get) Token: 0x0600001E RID: 30 RVA: 0x00003504 File Offset: 0x00001704
			// (set) Token: 0x0600001F RID: 31 RVA: 0x0000350C File Offset: 0x0000170C
			public string URI { get; set; }

			// Token: 0x1700000B RID: 11
			// (get) Token: 0x06000020 RID: 32 RVA: 0x00003515 File Offset: 0x00001715
			// (set) Token: 0x06000021 RID: 33 RVA: 0x0000351D File Offset: 0x0000171D
			public PkgRegWriter.AccessType Access { get; set; }

			// Token: 0x1700000C RID: 12
			// (get) Token: 0x06000022 RID: 34 RVA: 0x00003526 File Offset: 0x00001726
			// (set) Token: 0x06000023 RID: 35 RVA: 0x0000352E File Offset: 0x0000172E
			public PkgRegWriter.SourceType Source { get; set; }

			// Token: 0x1700000D RID: 13
			// (get) Token: 0x06000024 RID: 36 RVA: 0x00003537 File Offset: 0x00001737
			// (set) Token: 0x06000025 RID: 37 RVA: 0x0000353F File Offset: 0x0000173F
			public string SourcePath { get; set; }

			// Token: 0x1700000E RID: 14
			// (get) Token: 0x06000026 RID: 38 RVA: 0x00003548 File Offset: 0x00001748
			// (set) Token: 0x06000027 RID: 39 RVA: 0x00003550 File Offset: 0x00001750
			public uint SourceType { get; set; }

			// Token: 0x1700000F RID: 15
			// (get) Token: 0x06000028 RID: 40 RVA: 0x00003559 File Offset: 0x00001759
			// (set) Token: 0x06000029 RID: 41 RVA: 0x00003561 File Offset: 0x00001761
			public PkgRegWriter.ValidationMethodType Validation { get; set; }

			// Token: 0x17000010 RID: 16
			// (get) Token: 0x0600002A RID: 42 RVA: 0x0000356A File Offset: 0x0000176A
			// (set) Token: 0x0600002B RID: 43 RVA: 0x00003572 File Offset: 0x00001772
			public int Min { get; set; }

			// Token: 0x17000011 RID: 17
			// (get) Token: 0x0600002C RID: 44 RVA: 0x0000357B File Offset: 0x0000177B
			// (set) Token: 0x0600002D RID: 45 RVA: 0x00003583 File Offset: 0x00001783
			public int Max { get; set; }

			// Token: 0x17000012 RID: 18
			// (get) Token: 0x0600002E RID: 46 RVA: 0x0000358C File Offset: 0x0000178C
			// (set) Token: 0x0600002F RID: 47 RVA: 0x00003594 File Offset: 0x00001794
			public List<string> Options { get; set; }

			// Token: 0x17000013 RID: 19
			// (get) Token: 0x06000030 RID: 48 RVA: 0x0000359D File Offset: 0x0000179D
			// (set) Token: 0x06000031 RID: 49 RVA: 0x000035A5 File Offset: 0x000017A5
			public bool IsString { get; set; }

			// Token: 0x06000032 RID: 50 RVA: 0x000035AE File Offset: 0x000017AE
			public Setting()
			{
				this.Access = PkgRegWriter.AccessType.All;
				this.Source = PkgRegWriter.SourceType.MCSF_SETTINGSOURCE_REGISTRY;
				this.Validation = PkgRegWriter.ValidationMethodType.MCSF_VALIDATIONMETHOD_NONE;
				this.Min = int.MinValue;
				this.Max = int.MaxValue;
				this.Options = new List<string>();
			}

			// Token: 0x06000033 RID: 51 RVA: 0x000035F0 File Offset: 0x000017F0
			public Setting(string uri, string sourcePath, uint sourceType)
			{
				this.URI = uri;
				this.Access = PkgRegWriter.AccessType.All;
				this.Source = PkgRegWriter.SourceType.MCSF_SETTINGSOURCE_REGISTRY;
				this.SourceType = sourceType;
				this.SourcePath = sourcePath;
				this.Validation = PkgRegWriter.ValidationMethodType.MCSF_VALIDATIONMETHOD_NONE;
				this.Min = int.MinValue;
				this.Max = int.MaxValue;
				this.Options = new List<string>();
			}

			// Token: 0x06000034 RID: 52 RVA: 0x00003650 File Offset: 0x00001850
			public void BuildRegKey(RegistryKeyGroupBuilder groupBuilder)
			{
				string text = "$(hklm.software)\\Microsoft\\MCSF\\Settings\\" + this.URI;
				RegistryKeyBuilder registryKeyBuilder = groupBuilder.AddRegistryKey(text);
				registryKeyBuilder.AddValue("AllowedAccess", PkgRegWriter.RegValueType.REG_DWORD.ToString(), Convert.ToString((long)((ulong)this.Access), 16));
				registryKeyBuilder.AddValue("Source", PkgRegWriter.RegValueType.REG_DWORD.ToString(), Convert.ToString((long)((ulong)this.Source), 16));
				registryKeyBuilder.AddValue("SourcePath", PkgRegWriter.RegValueType.REG_SZ.ToString(), this.SourcePath);
				registryKeyBuilder.AddValue("SourceType", PkgRegWriter.RegValueType.REG_DWORD.ToString(), Convert.ToString((long)((ulong)this.SourceType), 16));
				registryKeyBuilder.AddValue("ValidationMethod", PkgRegWriter.RegValueType.REG_DWORD.ToString(), Convert.ToString((long)((ulong)this.Validation), 16));
				PkgRegWriter.ValidationMethodType validation = this.Validation;
				if (validation != PkgRegWriter.ValidationMethodType.MCSF_VALIDATIONMETHOD_RANGE)
				{
					if (validation != PkgRegWriter.ValidationMethodType.MCSF_VALIDATIONMETHOD_OPTION)
					{
						return;
					}
					RegistryKeyBuilder registryKeyBuilder2 = groupBuilder.AddRegistryKey(text + "\\Options");
					uint num = 0U;
					using (List<string>.Enumerator enumerator = this.Options.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string value = enumerator.Current;
							if (this.IsString)
							{
								registryKeyBuilder2.AddValue(Convert.ToString((long)((ulong)num), 10), PkgRegWriter.RegValueType.REG_SZ.ToString(), value);
							}
							else
							{
								registryKeyBuilder2.AddValue(Convert.ToString((long)((ulong)num), 10), PkgRegWriter.RegValueType.REG_DWORD.ToString(), value);
							}
							num += 1U;
						}
						return;
					}
				}
				registryKeyBuilder.AddValue("ValidateMin", PkgRegWriter.RegValueType.REG_DWORD.ToString(), Convert.ToString((long)((ulong)this.Min), 16));
				registryKeyBuilder.AddValue("ValidateMax", PkgRegWriter.RegValueType.REG_DWORD.ToString(), Convert.ToString((long)((ulong)this.Max), 16));
			}
		}
	}
}
