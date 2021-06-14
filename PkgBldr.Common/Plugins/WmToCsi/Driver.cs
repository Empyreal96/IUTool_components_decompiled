using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy;
using Microsoft.CompPlat.PkgBldr.Interfaces;
using Microsoft.CompPlat.PkgBldr.Tools;
using Microsoft.WindowsPhone.Security.SecurityPolicyCompiler;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x0200000A RID: 10
	[Export(typeof(IPkgPlugin))]
	internal class Driver : PkgPlugin
	{
		// Token: 0x06000018 RID: 24 RVA: 0x00002364 File Offset: 0x00000564
		private static void ApplySddlExceptions(XDocument deconstructedDriver)
		{
			foreach (XElement xelement in deconstructedDriver.Root.Descendants(deconstructedDriver.Root.Name.Namespace + "registryKey"))
			{
				string attributeValue = PkgBldrHelpers.GetAttributeValue(xelement, "keyName");
				if (Driver._regSddlMap.ContainsKey(attributeValue))
				{
					string value = Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy.HashCalculator.CalculateSha1Hash(attributeValue);
					foreach (XElement xelement2 in xelement.Elements(xelement.Name.Namespace + "securityDescriptor"))
					{
						xelement2.Remove();
					}
					XElement xelement3 = new XElement(xelement.Name.Namespace + "securityDescriptor");
					xelement3.Add(new XAttribute("name", value));
					xelement.Add(xelement3);
					XElement xelement4 = PkgBldrHelpers.AddIfNotFound(PkgBldrHelpers.AddIfNotFound(PkgBldrHelpers.AddIfNotFound(PkgBldrHelpers.AddIfNotFound(deconstructedDriver.Root, "trustInfo"), "security"), "accessControl"), "securityDescriptorDefinitions");
					XElement xelement5 = new XElement(xelement4.Name.Namespace + "securityDescriptorDefinition");
					xelement5.Add(new XAttribute("name", value));
					xelement5.Add(new XAttribute("sddl", Driver._regSddlMap[attributeValue]));
					xelement4.Add(xelement5);
				}
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002524 File Offset: 0x00000724
		public override void ConvertEntries(XElement toCsi, Dictionary<string, IPkgPlugin> plugins, Config environ, XElement driverWmElement)
		{
			Driver.CheckIfRunningElevated();
			this._environ = environ;
			this._logger = environ.Logger;
			this._targetProduct = environ.Bld.Product;
			if (string.IsNullOrEmpty(this._targetProduct))
			{
				this._targetProduct = "windows";
			}
			this._targetProduct = this._targetProduct.ToLowerInvariant();
			this._driverWmElement = driverWmElement;
			this._workingFolder = LongPath.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			LongPathDirectory.CreateDirectory(this._workingFolder);
			Driver.CapabilityListFile = Environment.ExpandEnvironmentVariables(environ.pkgBldrArgs.capabilityListCfg);
			string infPath = this.GetInfPath();
			string fileName = LongPath.GetFileName(infPath);
			string deconstructedInfName = this.GetDeconstructedInfName(fileName);
			XAttribute xattribute = this._driverWmElement.Attribute("phoneCompatible");
			if (xattribute != null)
			{
				this._legacyDriver = xattribute.Value.Equals("true", StringComparison.OrdinalIgnoreCase);
			}
			try
			{
				if (!CommonSettings.Default.ErrorOnDeconstructionFailure)
				{
					this._logger.LogWarning("Error detection is disabled", new object[0]);
				}
				this.BuildDriverManifests(infPath, fileName, deconstructedInfName);
				FileUtils.DeleteTree(this._workingFolder);
			}
			catch (Exception ex)
			{
				string format = string.Format(CultureInfo.InvariantCulture, "Driver deconstruction failed for {0}", new object[]
				{
					fileName
				});
				if (CommonSettings.Default.ErrorOnDeconstructionFailure)
				{
					this._logger.LogError(format, new object[0]);
					this._logger.LogError(ex.ToString(), new object[0]);
					throw;
				}
				this._logger.LogInfo(format, new object[0]);
				this._logger.LogInfo(ex.ToString(), new object[0]);
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000026E8 File Offset: 0x000008E8
		private static void CheckIfRunningElevated()
		{
			if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
			{
				throw new PkgGenException("!!!Drvstore.dll requires admin privileges. PkgGen.exe needs this dll when deconstructing drivers!!!");
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000270C File Offset: 0x0000090C
		private void BuildBinaryFileMaps(string infPath)
		{
			string input;
			Run.RunProcess(this._workingFolder, "infutil.exe", string.Format(CultureInfo.InvariantCulture, "/noincludes /files /arch {0} {1}", new object[]
			{
				this._environ.Bld.ArchAsString,
				LongPath.GetFileName(infPath)
			}), true, true, out input, null, null);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			foreach (string text in Regex.Split(input, "\r\n\r\n"))
			{
				if (!string.IsNullOrWhiteSpace(text))
				{
					string[] array2 = Regex.Split(text, "\r\n");
					string path = Regex.Split(array2[0], "          ")[1];
					if (!dictionary.ContainsKey(LongPath.GetFileName(path)))
					{
						dictionary.Add(LongPath.GetFileName(path), LongPath.GetDirectoryName(path));
					}
					string text2 = Regex.Split(array2[1], "     ")[1];
					string text3;
					if (text2.StartsWith(".\\", StringComparison.OrdinalIgnoreCase))
					{
						text3 = LongPath.Combine("$(runtime.drivers)", LongPath.GetFileName(text2));
					}
					else
					{
						text3 = Regex.Replace(text2, "%systemroot%\\\\system32\\\\drivers\\\\", "$(runtime.drivers)\\", RegexOptions.IgnoreCase);
						if (text3.Equals(text2, StringComparison.OrdinalIgnoreCase))
						{
							text3 = Regex.Replace(text2, "%systemroot%\\\\system32\\\\", "$(runtime.system32)\\", RegexOptions.IgnoreCase);
							if (text3.Equals(text2, StringComparison.OrdinalIgnoreCase))
							{
								text3 = Regex.Replace(text2, "%systemroot%\\\\", "$(runtime.windows)\\", RegexOptions.IgnoreCase);
								if (text3.Equals(text2, StringComparison.OrdinalIgnoreCase))
								{
									text3 = Regex.Replace(text2, "%systemdrive%\\\\", "$(runtime.bootDrive)\\", RegexOptions.IgnoreCase);
								}
							}
						}
					}
					string key = LongPath.GetFileName(text3).ToLowerInvariant();
					if (!dictionary2.ContainsKey(key))
					{
						dictionary2.Add(key, LongPath.GetDirectoryName(text3) + "\\");
					}
				}
			}
			this._fileNameSubDirMap = dictionary;
			this._fileNameDestinationMap = dictionary2;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000028CC File Offset: 0x00000ACC
		private string GetInfPath()
		{
			string text = "";
			XElement xelement = this._driverWmElement.Element(this._driverWmElement.GetDefaultNamespace() + "inf");
			if (xelement != null)
			{
				XAttribute xattribute = xelement.Attribute("source");
				if (xattribute != null)
				{
					text = this.ResolveNtTree(this.ResolvePath(xattribute.Value));
					if (!LongPathFile.Exists(text))
					{
						throw new PkgGenException("Can't find source INF {0}", new object[]
						{
							text
						});
					}
				}
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				throw new PkgGenException("Missing driver inf source attribute");
			}
			return LongPath.GetFullPath(text);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002960 File Offset: 0x00000B60
		private string GetDeconstructedInfName(string infName)
		{
			return "dual_" + infName;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002970 File Offset: 0x00000B70
		private void BuildDriverManifests(string infPath, string infName, string deconstructedInfName)
		{
			string text = this.ProductVariantProcessing(infPath, infName);
			this.UpdateInfSecurity(text);
			this.BuildBinaryFileMaps(text);
			string spkgPath = this.RunSpkgGen(text);
			XDocument xdocument = this.RunConvertDSM(this._workingFolder, spkgPath);
			XElement resourceManifestDependencyNode = this.ResourceManifestDependencyNode(infName);
			this.GeneratePigeonManifest(xdocument, infName, resourceManifestDependencyNode);
			this.GenerateDeconstructedDriver(xdocument, deconstructedInfName, resourceManifestDependencyNode);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000029C4 File Offset: 0x00000BC4
		private void GenerateDeconstructedDriver(XDocument dsmConvertedCsiManifest, string deconstructedInfName, XElement resourceManifestDependencyNode)
		{
			XDocument xdocument = new XDocument(dsmConvertedCsiManifest);
			this._logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "Deconstructing driver {0}", new object[]
			{
				deconstructedInfName
			}), new object[0]);
			this.PostprocessManifest(xdocument, deconstructedInfName, resourceManifestDependencyNode);
			this._logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "Driver deconstruction complete {0}", new object[]
			{
				deconstructedInfName
			}), new object[0]);
			Driver.ApplySddlExceptions(xdocument);
			this.SaveManifest(xdocument, deconstructedInfName);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002A44 File Offset: 0x00000C44
		private void SaveManifest(XDocument manifest, string deconstructedInfName)
		{
			string directoryName = LongPath.GetDirectoryName(this._environ.Output);
			string text = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}.man", new object[]
			{
				directoryName,
				deconstructedInfName
			});
			this._logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "Writing: {0}", new object[]
			{
				text
			}), new object[0]);
			PkgBldrHelpers.XDocumentSaveToLongPath(manifest, text);
			if (this._legacyDriver)
			{
				XElement root = manifest.Root;
				XElement xelement = root.Element(root.Name.Namespace + "assemblyIdentity");
				xelement.Attribute("type").Remove();
				string value = xelement.Attribute("name").Value;
				xelement.SetAttributeValue("name", value.Replace("dual_", "legacy_"));
				XElement xelement2 = root.Element(root.Name.Namespace + "deployment");
				if (xelement2 != null)
				{
					xelement2.Remove();
				}
				PkgBldrHelpers.XDocumentSaveToLongPath(new XDocument(new object[]
				{
					root
				}), text.Replace("dual_", "legacy_"));
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002B74 File Offset: 0x00000D74
		private string ProductVariantProcessing(string infPath, string deconstructedInfName)
		{
			this._logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "Process inf for target product [{0}]", new object[]
			{
				this._targetProduct
			}), new object[0]);
			string text = LongPath.Combine(this._workingFolder, deconstructedInfName);
			string text2 = text + ".tmp1";
			string text3 = text + ".tmp2";
			try
			{
				if (this._environ.ProcessInf)
				{
					string arguments = string.Format(CultureInfo.InvariantCulture, "/k \"{0} /DLANGUAGE_ID=0x0409 /DTARGET_PRODUCT_{1} -nologo /EP /C {2}\" > {3} & exit %ERRORLEVEL%", new object[]
					{
						this._environ.pkgBldrArgs.toolPaths["cl"],
						this._targetProduct.ToUpperInvariant(),
						infPath,
						text2
					});
					Run.RunProcess(this._workingFolder, "cmd.exe", arguments, this._logger);
					arguments = string.Format(CultureInfo.InvariantCulture, "-m -1252 {0} {1}", new object[]
					{
						text2,
						text3
					});
					Run.RunProcess(this._workingFolder, this._environ.pkgBldrArgs.toolPaths["unitext"], arguments, this._logger);
					arguments = string.Format(CultureInfo.InvariantCulture, "prodfilt -u {0} {1} +i", new object[]
					{
						text3,
						text
					});
					Run.RunProcess(this._workingFolder, this._environ.pkgBldrArgs.toolPaths["prodfilt"], arguments, this._logger);
					arguments = string.Format(CultureInfo.InvariantCulture, "-f {0}", new object[]
					{
						text
					});
					Run.RunProcess(this._workingFolder, this._environ.pkgBldrArgs.toolPaths["stampinf"], arguments, this._logger);
				}
				else
				{
					LongPathFile.WriteAllLines(text, LongPathFile.ReadAllLines(infPath));
				}
			}
			finally
			{
				Driver.DeleteFile(text2);
				Driver.DeleteFile(text3);
			}
			return text;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002D60 File Offset: 0x00000F60
		private void UpdateInfSecurity(string tempInfPath)
		{
			IEnumerable<XElement> enumerable = this._driverWmElement.Elements(this._driverWmElement.Name.Namespace + "security");
			if (enumerable != null)
			{
				List<string> list = LongPathFile.ReadAllLines(tempInfPath).ToList<string>();
				foreach (XElement xelement in enumerable)
				{
					string value = xelement.Attribute("infSectionName").Value;
					StringBuilder stringBuilder = new StringBuilder("HKR,,Security,,\"");
					stringBuilder.Append("D:P");
					bool flag = false;
					string text = this.GetSectionSddl(list, value);
					if (text != null)
					{
						string text2 = text.Remove(3);
						text = text.Remove(0, 3);
						if (text2 != "D:P")
						{
							throw new PkgGenException("Invalid INF security header : {0}", new object[]
							{
								text2
							});
						}
						stringBuilder.Append(text);
						if (Driver.regexDefaultDacl.Match(text).Success)
						{
							flag = true;
						}
					}
					foreach (XElement xelement2 in xelement.Elements())
					{
						if (!xelement2.Name.LocalName.Equals("accessedByCapability", StringComparison.OrdinalIgnoreCase))
						{
							throw new PkgGenException("Unsupported <accessedBy*> element {0}", new object[]
							{
								xelement2.Name.LocalName
							});
						}
						string text3 = this.GenerateSecurityDescriptor(xelement2);
						string text4 = text3.Remove(3);
						text3 = text3.Remove(0, 3);
						if (text4 != "D:P")
						{
							throw new PkgGenException("Invalid INF security header : {0}", new object[]
							{
								text4
							});
						}
						if (Driver.regexDefaultDacl.Match(text3).Success)
						{
							if (flag)
							{
								text3 = text3.Remove(0, 12);
							}
							else
							{
								flag = true;
							}
							stringBuilder.Append(text3);
						}
					}
					stringBuilder.Append("\"");
					this.SetSectionSddl(stringBuilder.ToString(), list, value);
				}
				LongPathFile.WriteAllLines(tempInfPath, list);
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002FA8 File Offset: 0x000011A8
		private string GenerateSecurityDescriptor(XElement accessedByElement)
		{
			string value = accessedByElement.Attribute("id").Value;
			string text = this._environ.Macros.Resolve(accessedByElement.Attribute("rights").Value);
			string result;
			if (this.ShouldUsePhoneSecurity())
			{
				result = Microsoft.WindowsPhone.Security.SecurityPolicyCompiler.GlobalVariables.GetPhoneSDDL(Driver.CapabilityListFile, value, text);
			}
			else
			{
				result = new DriverAccess(DriverAccessType.Capability, value, text).GetSecurityDescriptor();
			}
			return result;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00003017 File Offset: 0x00001217
		private bool ShouldUsePhoneSecurity()
		{
			return this._targetProduct.Equals(Driver.MobileCoreProductFilter, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x0000302C File Offset: 0x0000122C
		private string GetSectionSddl(List<string> infLines, string infSectionName)
		{
			int sectionStart = this.GetSectionStart(infLines, infSectionName);
			if (sectionStart == -1)
			{
				throw new PkgGenException("Section name {0} missing from inf", new object[]
				{
					infSectionName
				});
			}
			int sectionEnd = this.GetSectionEnd(infLines, sectionStart);
			string text = null;
			for (int i = sectionStart; i < sectionEnd; i++)
			{
				string input = infLines[i];
				Match match = Driver.regexInfSddl.Match(input);
				if (match.Success)
				{
					if (text != null)
					{
						throw new PkgGenException("More than one security SDDL string specified near line {0}", new object[]
						{
							i
						});
					}
					text = match.Groups["sddl"].Value;
				}
			}
			return text;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000030C8 File Offset: 0x000012C8
		private int GetSectionStart(List<string> infLines, string infSectionName)
		{
			int result = -1;
			string pattern = string.Format(CultureInfo.InvariantCulture, "^\\[[ \\t]*{0}[ \\t]*\\](.*)$", new object[]
			{
				infSectionName
			});
			for (int i = 0; i < infLines.Count; i++)
			{
				string text = infLines[i].Trim();
				if (!text.StartsWith(";", StringComparison.OrdinalIgnoreCase) && Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase))
				{
					result = i;
					break;
				}
			}
			return result;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x0000312C File Offset: 0x0000132C
		private int GetSectionEnd(List<string> infLines, int sectionStart)
		{
			int num = -1;
			for (int i = sectionStart + 1; i < infLines.Count; i++)
			{
				string text = infLines[i].Trim();
				if (!text.StartsWith(";", StringComparison.OrdinalIgnoreCase) && new Regex("^\\[.*\\](.*)$").IsMatch(text))
				{
					num = i;
					break;
				}
			}
			if (-1 == num)
			{
				num = infLines.Count;
			}
			return num;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x0000318C File Offset: 0x0000138C
		private void SetSectionSddl(string newSddl, List<string> infLines, string infSectionName)
		{
			int sectionStart = this.GetSectionStart(infLines, infSectionName);
			if (sectionStart == -1)
			{
				throw new PkgGenException("Section name {0} missing from inf", new object[]
				{
					infSectionName
				});
			}
			int sectionEnd = this.GetSectionEnd(infLines, sectionStart);
			List<string> list = new List<string>(from x in infLines.Skip(sectionStart).Take(sectionEnd - sectionStart)
			where !Driver.regexInfSddl.Match(x).Success
			select x);
			if (!string.IsNullOrEmpty(newSddl))
			{
				List<string> list2 = list;
				if (list2[list2.Count - 1].Equals(string.Empty))
				{
					List<string> list3 = list;
					list3.RemoveAt(list3.Count - 1);
				}
				list.Add(newSddl);
				list.Add(string.Empty);
			}
			infLines.RemoveRange(sectionStart, sectionEnd - sectionStart);
			infLines.InsertRange(sectionStart, list);
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00003254 File Offset: 0x00001454
		private string RunSpkgGen(string infPath)
		{
			Driver.WindowsManifestIdentity windowsManifestIdentity = this.GetWindowsManifestIdentity();
			string text = LongPath.Combine(this._workingFolder, string.Format(CultureInfo.InvariantCulture, "{0}.pkg.xml", new object[]
			{
				windowsManifestIdentity.Name
			}));
			this.CreatePkgXml(infPath, text, "s", "w", windowsManifestIdentity.OwnerType, windowsManifestIdentity.ReleaseType);
			List<string> list = this.ConstructSPkgGenArguments();
			list.Add("/output:" + this._workingFolder);
			list.Add("/nohives");
			list.Add(text);
			list.Add(string.Format(CultureInfo.InvariantCulture, "/toolPaths:{0}", new object[]
			{
				this._environ.pkgBldrArgs.spkgGenToolDirs
			}));
			if (this._environ.pkgBldrArgs.isRazzleEnv)
			{
				list.Add("/isRazzleEnv");
			}
			if (this._environ.pkgBldrArgs.diagnostic)
			{
				list.Add("/diagnostic");
			}
			bool inWindows = true;
			string result = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}.{2}.spkg", new object[]
			{
				this._workingFolder,
				"w",
				"s"
			});
			Run.RunSPkgGen(list, inWindows, this._logger, this._environ.pkgBldrArgs, null);
			return result;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00003394 File Offset: 0x00001594
		private XDocument RunConvertDSM(string workingDirectory, string spkgPath)
		{
			XDocument result;
			try
			{
				ConvertDSM.RunDsmConverter(spkgPath, workingDirectory, false, false, 32U);
				result = PkgBldrHelpers.XDocumentLoadFromLongPath(LongPathDirectory.GetFiles(workingDirectory, "*.manifest", SearchOption.AllDirectories)[0]);
			}
			catch (PkgGenException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new PkgGenException("Failed to run ConvertDSM", new object[]
				{
					ex
				});
			}
			return result;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000033F8 File Offset: 0x000015F8
		private void PostprocessManifest(XDocument csiManifest, string assemblyIdentity, XElement resourceManifestDependencyNode)
		{
			this._logger.LogInfo("Postprocessing manifest", new object[0]);
			XElement root = csiManifest.Root;
			XElement xelement = root.Element(root.Name.Namespace + "assemblyIdentity");
			xelement.SetAttributeValue("name", assemblyIdentity);
			xelement.SetAttributeValue("buildType", "$(build.buildType)");
			xelement.SetAttributeValue("processorArchitecture", "$(build.arch)");
			xelement.SetAttributeValue("publicKeyToken", "$(build.WindowsPublicKeyToken)");
			xelement.SetAttributeValue("version", "$(build.version)");
			xelement.Add(new XAttribute("type", "dualModeDriver"));
			if (!this._targetProduct.Equals("windows"))
			{
				xelement.Add(new XAttribute("product", "$(build.product)"));
			}
			if (resourceManifestDependencyNode != null)
			{
				xelement.AddAfterSelf(resourceManifestDependencyNode);
			}
			IEnumerable<XElement> enumerable = this._driverWmElement.Descendants(this._driverWmElement.Name.Namespace + "file");
			foreach (XElement xelement2 in root.Descendants(root.Name.Namespace + "file"))
			{
				string fileName = LongPath.GetFileName(xelement2.Attribute("name").Value);
				xelement2.SetAttributeValue("name", fileName);
				xelement2.Elements().Remove<XElement>();
				foreach (XElement xelement3 in enumerable)
				{
					string text = LongPath.GetFileName(xelement3.Attribute("source").Value);
					XAttribute xattribute = xelement3.Attribute("name");
					if (xattribute != null)
					{
						text = xattribute.Value;
					}
					if (fileName.Equals(text))
					{
						XAttribute xattribute2 = xelement2.Attribute("attributes");
						if (xattribute2 != null)
						{
							xattribute2.Remove();
						}
						XElement xelement4 = xelement3.Element(xelement3.Name.Namespace + "signatureInfo");
						if (xelement4 != null)
						{
							XElement xelement5 = new XElement(xelement2.Name.Namespace + "signatureInfo");
							foreach (XElement xelement6 in xelement4.Elements(xelement4.Name.Namespace + "signatureDescriptor"))
							{
								xelement5.Add(new XElement(xelement5.Name.Namespace + "signatureDescriptor", xelement6.Attributes()));
							}
							xelement2.Add(xelement5);
						}
						XAttribute xattribute3 = xelement3.Attribute("buildFilter");
						if (xattribute3 != null)
						{
							xelement2.Add(xattribute3);
						}
						string value = xelement3.Attribute("source").Value;
						string path = this.CleanNtTreePath(this.ResolvePath(value));
						xelement2.Add(new XAttribute("importPath", LongPath.GetDirectoryName(path) + "\\"));
						this._logger.LogInfo("Processing: [{0}]", new object[]
						{
							text
						});
						string extendedFileName = this.GetExtendedFileName(text);
						if (!extendedFileName.Equals(text, StringComparison.OrdinalIgnoreCase))
						{
							xelement2.SetAttributeValue("name", extendedFileName);
							xelement2.Add(new XAttribute("sourceName", text));
						}
						if (this._fileNameDestinationMap.ContainsKey(text.ToLowerInvariant()))
						{
							xelement2.SetAttributeValue("destinationPath", this._fileNameDestinationMap[text.ToLowerInvariant()]);
						}
					}
				}
			}
			IEnumerable<XElement> enumerable2 = root.Descendants(root.GetDefaultNamespace() + "registryKey");
			List<XElement> list = new List<XElement>();
			foreach (XElement xelement7 in enumerable2)
			{
				XAttribute xattribute4 = xelement7.Attribute("keyName");
				if (xattribute4 == null)
				{
					throw new PkgGenException("Invalid <registryKey> element, missing required keyName attribute");
				}
				string text2 = xattribute4.Value.ToLowerInvariant();
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text2);
				if (num <= 2779100942U)
				{
					if (num <= 1274657387U)
					{
						if (num != 632255635U)
						{
							if (num != 789429082U)
							{
								if (num != 1274657387U)
								{
									continue;
								}
								if (!(text2 == "hkey_local_machine\\system\\driverdatabase\\driverpackages"))
								{
									continue;
								}
								goto IL_62E;
							}
							else
							{
								if (!(text2 == "hkey_local_machine\\software\\microsoft\\windows\\currentversion\\setup"))
								{
									continue;
								}
								goto IL_62E;
							}
						}
						else if (!(text2 == "hkey_local_machine\\system\\currentcontrolset"))
						{
							continue;
						}
					}
					else if (num != 1521199005U)
					{
						if (num != 1641555814U)
						{
							if (num != 2779100942U)
							{
								continue;
							}
							if (!(text2 == "hkey_local_machine\\system\\currentcontrolset\\control\\class"))
							{
								continue;
							}
							goto IL_62E;
						}
						else if (!(text2 == "hkey_local_machine\\system\\currentcontrolset\\control"))
						{
							continue;
						}
					}
					else
					{
						if (!(text2 == "hkey_local_machine\\software\\microsoft\\windows\\currentversion\\setup\\pnpresources"))
						{
							continue;
						}
						goto IL_62E;
					}
				}
				else if (num <= 3297513341U)
				{
					if (num != 2984783304U)
					{
						if (num != 3191840456U)
						{
							if (num != 3297513341U)
							{
								continue;
							}
							if (!(text2 == "hkey_local_machine\\system\\currentcontrolset\\services"))
							{
								continue;
							}
						}
						else
						{
							if (!(text2 == "hkey_local_machine\\system\\driverdatabase"))
							{
								continue;
							}
							goto IL_62E;
						}
					}
					else
					{
						if (!(text2 == "hkey_local_machine\\system\\driverdatabase\\deviceids"))
						{
							continue;
						}
						goto IL_62E;
					}
				}
				else if (num != 3786205932U)
				{
					if (num != 4109116664U)
					{
						if (num != 4167256562U)
						{
							continue;
						}
						if (!(text2 == "hkey_local_machine\\software\\microsoft\\windows\\currentversion\\setup\\pnplockdownfiles"))
						{
							continue;
						}
						goto IL_62E;
					}
					else if (!(text2 == "hkey_local_machine\\system\\currentcontrolset\\enum"))
					{
						continue;
					}
				}
				else
				{
					if (!(text2 == "hkey_local_machine\\system\\driverdatabase\\driverinffiles"))
					{
						continue;
					}
					goto IL_62E;
				}
				if (xelement7.Descendants().All((XElement x) => x.Name.LocalName.Equals("securityDescriptor", StringComparison.OrdinalIgnoreCase)))
				{
					list.Add(xelement7);
					continue;
				}
				continue;
				IL_62E:
				list.Add(xelement7);
			}
			list.Remove<XElement>();
			foreach (XElement xelement8 in root.Descendants(root.Name.Namespace + "registryValue"))
			{
				XAttribute xattribute5 = xelement8.Attribute("mutable");
				if (xattribute5 != null)
				{
					xattribute5.Remove();
				}
				XAttribute xattribute6 = xelement8.Attribute("name");
				if (xattribute6 != null && string.IsNullOrEmpty(xattribute6.Value))
				{
					xattribute6.Remove();
				}
			}
			string value2 = this._driverWmElement.Element(this._driverWmElement.Name.Namespace + "inf").Attribute("source").Value;
			string path2 = this.CleanNtTreePath(value2);
			XElement xelement9 = new XElement(root.Name.Namespace + "file", new object[]
			{
				new XAttribute("importPath", LongPath.GetDirectoryName(path2) + "\\"),
				new XAttribute("name", LongPath.GetFileName(value2)),
				new XAttribute("sourceName", LongPath.GetFileName(value2)),
				new XElement(root.Name.Namespace + "infFile")
			});
			XElement xelement10 = new XElement(root.Name.Namespace + "deconstructionTool", new XAttribute("version", "10.0.0.0"));
			root.Add(new object[]
			{
				xelement9,
				xelement10
			});
			root.Add(new XElement(root.Name.Namespace + "deployment"));
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00003CA0 File Offset: 0x00001EA0
		private XElement ResourceManifestDependencyNode(string infName)
		{
			XAttribute xattribute = this._driverWmElement.Attribute("hasResources");
			if (xattribute != null && xattribute.Value.Equals("true", StringComparison.OrdinalIgnoreCase))
			{
				return XElement.Parse(string.Format(CultureInfo.InvariantCulture, "\r\n                    <dependency\r\n                       discoverable=\"false\"\r\n                       optional=\"false\"\r\n                       resourceType=\"Resources\"\r\n                       >\r\n                       <dependentAssembly dependencyType=\"prerequisite\">\r\n                          <assemblyIdentity\r\n                              name=\"{0}.Resources\"\r\n                              language=\"*\"\r\n                              processorArchitecture=\"{1}\"\r\n                              publicKeyToken=\"$(Build.WindowsPublicKeyToken)\"\r\n                              version=\"$(build.version)\"\r\n                              versionScope=\"nonSxS\"\r\n                           />\r\n                       </dependentAssembly>\r\n                   </dependency>", new object[]
				{
					infName,
					this._environ.Bld.ArchAsString
				}));
			}
			return null;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00003D0C File Offset: 0x00001F0C
		private void GeneratePigeonManifest(XDocument dsmConvertedManifest, string infName, XElement resourceManifestDependencyNode)
		{
			XElement root = dsmConvertedManifest.Root;
			XElement xelement = new XElement(root.Name.Namespace + "assembly", new XAttribute("manifestVersion", root.Attribute("manifestVersion").Value));
			XElement content = XElement.Parse(string.Format(CultureInfo.InvariantCulture, "\r\n               <assemblyIdentity\r\n                   buildType=\"$(build.buildType)\"\r\n                   language=\"neutral\"\r\n                   name=\"{0}\"\r\n                   processorArchitecture=\"{1}\"\r\n                   publicKeyToken=\"$(Build.WindowsPublicKeyToken)\"\r\n                   type=\"driverUpdate\"\r\n                   version=\"$(build.version)\"\r\n                   versionScope=\"nonSxS\"/>", new object[]
			{
				infName,
				this._environ.Bld.ArchAsString
			}));
			xelement.Add(content);
			if (resourceManifestDependencyNode != null)
			{
				xelement.Add(resourceManifestDependencyNode);
			}
			XNamespace ns = "urn:schemas-microsoft-com:asm.v3";
			foreach (XElement xelement2 in xelement.DescendantsAndSelf())
			{
				xelement2.Name = ns + xelement2.Name.LocalName;
			}
			if (!this._targetProduct.Equals("windows"))
			{
				foreach (XElement xelement3 in xelement.Descendants(xelement.Name.Namespace + "assemblyIdentity"))
				{
					xelement3.Add(new XAttribute("product", "$(build.product)"));
				}
			}
			string value = this._driverWmElement.Element(this._driverWmElement.Name.Namespace + "inf").Attribute("source").Value;
			string text = this.ResolvePath(value);
			text = this.CleanNtTreePath(text);
			XElement content2 = new XElement(xelement.Name.Namespace + "file", new object[]
			{
				new XAttribute("name", infName),
				new XAttribute("sourceName", LongPath.GetFileName(text)),
				new XAttribute("importPath", LongPath.GetDirectoryName(text) + "\\"),
				new XElement(xelement.Name.Namespace + "infFile")
			});
			xelement.Add(content2);
			XElement xelement4 = this._driverWmElement.Element(this._driverWmElement.Name.Namespace + "files");
			if (xelement4 != null)
			{
				using (IEnumerator<XElement> enumerator = xelement4.Elements().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						XElement xelement5 = enumerator.Current;
						string value2 = xelement5.Attribute("source").Value;
						string directoryName = LongPath.GetDirectoryName(value2);
						string text2 = this.CleanNtTreePath(this._environ.Macros.Resolve(directoryName));
						string fileName = LongPath.GetFileName(value2);
						this._logger.LogInfo("Processing: [{0}]", new object[]
						{
							fileName
						});
						string extendedFileName = this.GetExtendedFileName(fileName);
						XElement xelement6 = new XElement(xelement.Name.Namespace + "file", new object[]
						{
							new XAttribute("name", extendedFileName),
							new XAttribute("sourceName", fileName),
							new XAttribute("importPath", text2.TrimEnd(new char[]
							{
								'\\'
							}) + "\\")
						});
						IEnumerable<XElement> source = xelement5.Descendants(xelement5.Name.Namespace + "signatureDescriptor");
						if (source.Any<XElement>())
						{
							XElement pigeonSigInfo = new XElement(xelement6.Name.Namespace + "signatureInfo");
							source.ToList<XElement>().ForEach(delegate(XElement desc)
							{
								pigeonSigInfo.Add(new XElement(pigeonSigInfo.Name.Namespace + "signatureDescriptor", desc.Attributes()));
							});
							xelement6.Add(pigeonSigInfo);
						}
						xelement.Add(xelement6);
					}
					goto IL_408;
				}
			}
			this._logger.LogInfo("Manifest for INF [{0}] does not carry any files.", new object[]
			{
				infName
			});
			IL_408:
			xelement.Add(new XElement(xelement.Name.Namespace + "deployment"));
			PkgBldrHelpers.XDocumentSaveToLongPath(new XDocument(new object[]
			{
				xelement
			}), string.Format(CultureInfo.InvariantCulture, "{0}\\{1}.pigeon.man", new object[]
			{
				LongPath.GetDirectoryName(this._environ.Output),
				infName
			}));
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000041D0 File Offset: 0x000023D0
		private string GetExtendedFileName(string fileName)
		{
			string result = fileName;
			if (this._fileNameSubDirMap.ContainsKey(fileName) && !string.IsNullOrWhiteSpace(this._fileNameSubDirMap[fileName]))
			{
				this._logger.LogInfo("Key found!", new object[0]);
				result = this._fileNameSubDirMap[fileName] + "\\" + fileName;
			}
			return result;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000422F File Offset: 0x0000242F
		private string CleanNtTreePath(string rawPath)
		{
			return rawPath.ToLowerInvariant().Replace(Environment.ExpandEnvironmentVariables(this._environ.pkgBldrArgs.nttree).ToLowerInvariant(), "$(build.nttree)");
		}

		// Token: 0x06000030 RID: 48 RVA: 0x0000425C File Offset: 0x0000245C
		private string ResolvePath(string rawPath)
		{
			string text = this._environ.Macros.Resolve(rawPath);
			if (!text.Contains('$'))
			{
				if (Path.IsPathRooted(text))
				{
					text = LongPath.GetFullPath(text);
				}
				else
				{
					text = LongPath.GetFullPath(LongPath.Combine(LongPath.GetDirectoryName(this._environ.Input), text));
				}
			}
			return text;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000042B3 File Offset: 0x000024B3
		private string ResolveNtTree(string rawPath)
		{
			return rawPath.ToLowerInvariant().Replace("$(build.nttree)", Environment.ExpandEnvironmentVariables(this._environ.pkgBldrArgs.nttree).ToLowerInvariant());
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000042E0 File Offset: 0x000024E0
		private Driver.WindowsManifestIdentity GetWindowsManifestIdentity()
		{
			Driver.WindowsManifestIdentity result;
			try
			{
				result = new Driver.WindowsManifestIdentity
				{
					Name = this._environ.Bld.WM.Root.Attribute("name").Value,
					NameSpace = ((this._environ.Bld.WM.Root.Attribute("namespace") != null) ? this._environ.Bld.WM.Root.Attribute("namespace").Value : string.Empty),
					Owner = ((this._environ.Bld.WM.Root.Attribute("namespace") != null) ? this._environ.Bld.WM.Root.Attribute("owner").Value : "Microsoft"),
					OwnerType = "Microsoft",
					ReleaseType = "Production"
				};
			}
			catch (Exception)
			{
				throw new PkgGenException("Unable to parse the identity element from the Windows Manifest");
			}
			return result;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x0000441C File Offset: 0x0000261C
		private List<string> ConstructSPkgGenArguments()
		{
			return new List<string>
			{
				"/version:0.0.0.0",
				"/cpu:" + this._environ.Bld.ArchAsString
			};
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00004450 File Offset: 0x00002650
		private void CreatePkgXml(string infPath, string pkgXmlPath, string component, string owner, string ownerType, string releaseType)
		{
			XElement xelement = new XElement(Driver.PkgXml + "Package");
			xelement.Add(new XAttribute("Owner", owner));
			xelement.Add(new XAttribute("Component", component));
			xelement.Add(new XAttribute("OwnerType", ownerType));
			xelement.Add(new XAttribute("ReleaseType", releaseType));
			XElement xelement2 = new XElement(Driver.PkgXml + "Components");
			XElement xelement3 = new XElement(Driver.PkgXml + "Driver");
			xelement3.Add(new XAttribute("InfSource", infPath));
			IEnumerable<XElement> enumerable = this._driverWmElement.Descendants(this._driverWmElement.Name.Namespace + "file");
			if (enumerable.Count<XElement>() != 0)
			{
				XElement xelement4 = new XElement(Driver.PkgXml + "Files");
				foreach (XElement xelement5 in enumerable)
				{
					string text = LongPath.GetFileName(xelement5.Attribute("source").Value);
					XAttribute xattribute = xelement5.Attribute("name");
					if (xattribute != null)
					{
						text = xattribute.Value;
					}
					string text2 = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", new object[]
					{
						this._workingFolder,
						text
					});
					using (LongPathFile.Open(text2, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
					{
					}
					XElement xelement6 = new XElement(Driver.PkgXml + "Reference", new XAttribute("Source", text2));
					if (this._fileNameSubDirMap.ContainsKey(text) && !string.IsNullOrWhiteSpace(this._fileNameSubDirMap[text]))
					{
						xelement6.Add(new XAttribute("StagingSubDir", this._fileNameSubDirMap[text]));
					}
					xelement3.Add(xelement6);
					xelement4.Add(new XElement(Driver.PkgXml + "File", new XAttribute("Source", text2)));
				}
				xelement3.Add(xelement4);
			}
			xelement2.Add(xelement3);
			xelement.Add(xelement2);
			PkgBldrHelpers.XDocumentSaveToLongPath(new XDocument(new object[]
			{
				xelement
			}), pkgXmlPath);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000046FC File Offset: 0x000028FC
		private static void DeleteFile(string path)
		{
			if (LongPathFile.Exists(path))
			{
				LongPathFile.Delete(path);
			}
		}

		// Token: 0x04000003 RID: 3
		private static readonly string MobileCoreProductFilter = "mobilecore";

		// Token: 0x04000004 RID: 4
		private static string CapabilityListFile;

		// Token: 0x04000005 RID: 5
		private const string DeconstructedDriverPrefix = "dual_";

		// Token: 0x04000006 RID: 6
		private const string LegacyDriverPrefix = "legacy_";

		// Token: 0x04000007 RID: 7
		private static readonly XNamespace PkgXml = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00";

		// Token: 0x04000008 RID: 8
		private static readonly Regex regexInfSddl = new Regex("^HKR,,Security,,\"(?<sddl>.*)\"", RegexOptions.Compiled);

		// Token: 0x04000009 RID: 9
		private static readonly Regex regexDefaultDacl = new Regex("(\\(.*\\))*\\(A;;GA;;;SY\\)", RegexOptions.Compiled);

		// Token: 0x0400000A RID: 10
		private Config _environ;

		// Token: 0x0400000B RID: 11
		private string _targetProduct;

		// Token: 0x0400000C RID: 12
		private string _workingFolder;

		// Token: 0x0400000D RID: 13
		private XElement _driverWmElement;

		// Token: 0x0400000E RID: 14
		private bool _legacyDriver;

		// Token: 0x0400000F RID: 15
		private Dictionary<string, string> _fileNameSubDirMap;

		// Token: 0x04000010 RID: 16
		private Dictionary<string, string> _fileNameDestinationMap;

		// Token: 0x04000011 RID: 17
		private IDeploymentLogger _logger;

		// Token: 0x04000012 RID: 18
		private static Dictionary<string, string> _regSddlMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"hkey_local_machine\\software\\microsoft\\windows nt\\currentversion\\wudf",
				"D:(A;CIOI;KA;;;SY)(A;CIOI;KR;;;LS)(A;CIOI;KR;;;NS)(A;CIOI;KA;;;BA)(A;CIOI;KR;;;BU)"
			},
			{
				"hkey_local_machine\\software\\microsoft\\windows nt\\currentversion\\perflib",
				"D:P(A;CI;GR;;;IU)(A;CI;GA;;;BA)(A;CI;GA;;;SY)(A;CI;GA;;;CO)(A;CI;GR;;;LS)(A;CI;GR;;;NS)(A;CI;GR;;;LU)(A;CI;GR;;;MU)"
			},
			{
				"hkey_local_machine\\software\\microsoft\\windows nt\\currentversion",
				"O:S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464G:S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464D:P(A;CI;GA;;;S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464)(A;CI;GA;;;SY)(A;CI;GA;;;BA)(A;CI;GR;;;BU)(A;;DC;;;S-1-5-80-123231216-2592883651-3715271367-3753151631-4175906628)(A;CI;GR;;;S-1-15-2-1)(A;CI;GR;;;S-1-15-3-1024-1065365936-1281604716-3511738428-1654721687-432734479-3232135806-4053264122-3456934681)"
			},
			{
				"hkey_local_machine\\SYSTEM\\CurrentControlSet\\Services\\BTHPORT\\Parameters",
				"D:AR(A;CI;KA;;;LS)(A;CI;KA;;;SY)(A;CI;KA;;;BA)(A;CIIO;KA;;;CO)(A;CI;KR;;;AC)(A;CI;KR;;;BU)"
			},
			{
				"hkey_local_machine\\SYSTEM\\CurrentControlSet\\Services\\BTHPORT\\Parameters\\Devices",
				"D:AR(A;CI;KA;;;LS)(A;CI;KA;;;SY)(A;CI;KA;;;BA)(A;CIIO;KA;;;CO)(A;CI;KR;;;AC)(A;CI;KR;;;BU)"
			},
			{
				"hkey_local_machine\\SYSTEM\\CurrentControlSet\\Services\\BTHPORT\\Parameters\\Services",
				"D:AR(A;CI;KA;;;LS)(A;CI;KA;;;SY)(A;CI;KA;;;BA)(A;CIIO;KA;;;CO)(A;CI;KR;;;AC)(A;CI;KR;;;BU)"
			},
			{
				"hkey_local_machine\\SYSTEM\\CurrentControlSet\\Services\\BTHENUM\\Parameters",
				"D:AR(A;CI;KA;;;S-1-5-80-2586557155-168560303-1373426920-983201488-1499765686)(A;CI;KA;;;SY)(A;CI;KA;;;BA)(A;CIIO;KA;;;CO)(A;CI;KR;;;AC)(A;CI;KR;;;BU)"
			},
			{
				"hkey_local_machine\\SYSTEM\\CurrentControlSet\\Services\\BTHENUM\\Parameters\\Policy",
				"D:AR(A;CI;KA;;;S-1-5-80-2586557155-168560303-1373426920-983201488-1499765686)(A;CI;KA;;;SY)(A;CI;KA;;;BA)(A;CIIO;KA;;;CO)(A;CI;KR;;;AC)(A;CI;KR;;;BU)"
			},
			{
				"hkey_local_machine\\SYSTEM\\CurrentControlSet\\Services\\bthl2cap\\Parameters",
				"D:AR(A;CI;KA;;;S-1-5-80-2586557155-168560303-1373426920-983201488-1499765686)(A;CI;KA;;;SY)(A;CI;KA;;;BA)(A;CIIO;KA;;;CO)(A;CI;KR;;;AC)(A;CI;KR;;;BU)"
			},
			{
				"hkey_local_machine\\SYSTEM\\CurrentControlSet\\Services\\bthl2cap\\Parameters\\Policy",
				"D:AR(A;CI;KA;;;S-1-5-80-2586557155-168560303-1373426920-983201488-1499765686)(A;CI;KA;;;SY)(A;CI;KA;;;BA)(A;CIIO;KA;;;CO)(A;CI;KR;;;AC)(A;CI;KR;;;BU)"
			},
			{
				"hkey_local_machine\\SYSTEM\\CurrentControlSet\\Services\\BthLEEnum\\Parameters",
				"D:AR(A;CI;KA;;;S-1-5-80-2586557155-168560303-1373426920-983201488-1499765686)(A;CI;KA;;;SY)(A;CI;KA;;;BA)(A;CIIO;KA;;;CO)(A;CI;KR;;;AC)(A;CI;KR;;;BU)"
			},
			{
				"hkey_local_machine\\SYSTEM\\CurrentControlSet\\Services\\BthLEEnum\\Parameters\\Policy",
				"D:AR(A;CI;KA;;;S-1-5-80-2586557155-168560303-1373426920-983201488-1499765686)(A;CI;KA;;;SY)(A;CI;KA;;;BA)(A;CIIO;KA;;;CO)(A;CI;KR;;;AC)(A;CI;KR;;;BU)"
			},
			{
				"hkey_local_machine\\SYSTEM\\CurrentControlSet\\Services\\RFCOMM\\Parameters",
				"D:AR(A;CI;KA;;;S-1-5-80-2586557155-168560303-1373426920-983201488-1499765686)(A;CI;KA;;;SY)(A;CI;KA;;;BA)(A;CIIO;KA;;;CO)(A;CI;KR;;;BU)"
			},
			{
				"hkey_local_machine\\SYSTEM\\CurrentControlSet\\Services\\RFCOMM\\Parameters\\Policy",
				"D:AR(A;CI;KA;;;S-1-5-80-2586557155-168560303-1373426920-983201488-1499765686)(A;CI;KA;;;SY)(A;CI;KA;;;BA)(A;CIIO;KA;;;CO)(A;CI;KR;;;BU)"
			}
		};

		// Token: 0x0200005D RID: 93
		private class WindowsManifestIdentity
		{
			// Token: 0x17000098 RID: 152
			// (get) Token: 0x0600020F RID: 527 RVA: 0x0000B2DC File Offset: 0x000094DC
			// (set) Token: 0x06000210 RID: 528 RVA: 0x0000B2E4 File Offset: 0x000094E4
			public string Name { get; set; }

			// Token: 0x17000099 RID: 153
			// (get) Token: 0x06000211 RID: 529 RVA: 0x0000B2ED File Offset: 0x000094ED
			// (set) Token: 0x06000212 RID: 530 RVA: 0x0000B2F5 File Offset: 0x000094F5
			[SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Represents an attribute of a Windows manifest's assembly identity. Removal violates that representation.")]
			public string NameSpace { get; set; }

			// Token: 0x1700009A RID: 154
			// (get) Token: 0x06000213 RID: 531 RVA: 0x0000B2FE File Offset: 0x000094FE
			// (set) Token: 0x06000214 RID: 532 RVA: 0x0000B306 File Offset: 0x00009506
			[SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Represents an attribute of a Windows manifest's assembly identity. Removal violates that representation.")]
			public string Owner { get; set; }

			// Token: 0x1700009B RID: 155
			// (get) Token: 0x06000215 RID: 533 RVA: 0x0000B30F File Offset: 0x0000950F
			// (set) Token: 0x06000216 RID: 534 RVA: 0x0000B317 File Offset: 0x00009517
			public string OwnerType { get; set; }

			// Token: 0x1700009C RID: 156
			// (get) Token: 0x06000217 RID: 535 RVA: 0x0000B320 File Offset: 0x00009520
			// (set) Token: 0x06000218 RID: 536 RVA: 0x0000B328 File Offset: 0x00009528
			public string ReleaseType { get; set; }
		}
	}
}
