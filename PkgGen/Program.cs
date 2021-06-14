using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Base.Security;
using Microsoft.CompPlat.PkgBldr.Base.Tools;
using Microsoft.CompPlat.PkgBldr.Tools;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.CompPlat.PkgBldr
{
	// Token: 0x02000002 RID: 2
	public class Program
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static void BuildPackage(Config config)
		{
			if (config.Convert == ConversionType.csi2pkg)
			{
				PkgBldrLoader pkgBldrLoader = new PkgBldrLoader(PluginType.Csi2Pkg, Program.m_cmdArgs, null);
				XDocument xdocument = PkgBldrHelpers.XDocumentLoadFromLongPath(config.Input);
				XElement xelement = new XElement("urn:Microsoft.WindowsPhone/PackageSchema.v8.00" + "Package");
				pkgBldrLoader.Plugins[xdocument.Root.Name.LocalName].ConvertEntries(xelement, pkgBldrLoader.Plugins, config, xdocument.Root);
				if (config.ExitStatus == ExitStatus.SKIPPED)
				{
					Program.logger.LogInfo("Skipping {0}", new object[]
					{
						config.Input
					});
					return;
				}
				XDocument xdocument2 = new XDocument(new object[]
				{
					xelement
				});
				pkgBldrLoader.ValidateOutput(xdocument2);
				PkgBldrHelpers.XDocumentSaveToLongPath(xdocument2, config.Output);
				return;
			}
			else if (config.Convert == ConversionType.csi2wm)
			{
				PkgBldrLoader pkgBldrLoader2 = new PkgBldrLoader(PluginType.CsiToWm, Program.m_cmdArgs, null);
				XDocument xdocument3 = PkgBldrHelpers.XDocumentLoadFromLongPath(config.Input);
				if (!Program.IsThisACsiManifest(xdocument3.Root))
				{
					Program.logger.LogWarning("Can't convert, assemblyIdentity not found ", new object[0]);
					return;
				}
				XElement xelement2 = new XElement("urn:Microsoft.CompPlat/ManifestSchema.v1.00" + "identity");
				pkgBldrLoader2.Plugins[xdocument3.Root.Name.LocalName].ConvertEntries(xelement2, pkgBldrLoader2.Plugins, config, xdocument3.Root);
				if (config.ExitStatus == ExitStatus.SKIPPED)
				{
					Program.logger.LogInfo("Skipping {0}", new object[]
					{
						config.Input
					});
					return;
				}
				XDocument xdocument4 = new XDocument(new object[]
				{
					xelement2
				});
				pkgBldrLoader2.ValidateOutput(xdocument4);
				PkgBldrHelpers.XDocumentSaveToLongPath(xdocument4, config.Output);
				return;
			}
			else if (config.Convert == ConversionType.pkg2wm)
			{
				if (Program.m_pkgToWmLoader == null)
				{
					Program.m_pkgToWmLoader = new PkgBldrLoader(PluginType.PkgToWm, Program.m_cmdArgs, null);
				}
				if (config.Bld.PKG.Root == null)
				{
					XDocument xdocument5 = PkgBldrHelpers.XDocumentLoadFromLongPath(config.Input);
					Program.m_pkgToWmLoader.ValidateInput(xdocument5);
					config.Bld.PKG.Root = xdocument5.Root;
				}
				XElement xelement3 = new XElement("identity");
				config.ExitStatus = ExitStatus.SUCCESS;
				Program.m_pkgToWmLoader.Plugins[config.Bld.PKG.Root.Name.LocalName].ConvertEntries(xelement3, Program.m_pkgToWmLoader.Plugins, config, config.Bld.PKG.Root);
				if (config.ExitStatus == ExitStatus.SKIPPED)
				{
					return;
				}
				XDocument xdocument6 = new XDocument(new object[]
				{
					xelement3
				});
				Program.m_pkgToWmLoader.ValidateOutput(xdocument6);
				if (config.Output != null)
				{
					Program.logger.LogInfo("Writing {0}", new object[]
					{
						config.Output
					});
					if (config.Bld.JsonDepot != null && File.Exists(config.Output))
					{
						SdCommand.Run("edit", config.Output);
					}
					PkgBldrHelpers.XDocumentSaveToLongPath(xdocument6, config.Output);
					Program.ReformatManifest(config.Output, config);
					if (config.Bld.JsonDepot != null)
					{
						SdCommand.Run("add", config.Output);
					}
				}
				return;
			}
			else
			{
				if (config.Convert != ConversionType.wm2csi)
				{
					return;
				}
				if (Program.m_wmToCsiLoader == null)
				{
					Program.m_wmToCsiLoader = new PkgBldrLoader(PluginType.WmToCsi, Program.m_cmdArgs, null);
				}
				if (Program.m_wmRoot == null)
				{
					XDocument xdocument7 = PkgBldrHelpers.XDocumentLoadFromLongPath(config.Input);
					Program.m_wmToCsiLoader.ValidateInput(xdocument7);
					Program.m_wmRoot = xdocument7.Root;
				}
				config.Bld.WM.Root = new XElement(Program.m_wmRoot);
				XElement xelement4 = new XElement("assembly");
				config.GlobalSecurity = new GlobalSecurity();
				config.ExitStatus = ExitStatus.SUCCESS;
				Program.m_wmToCsiLoader.Plugins[config.Bld.WM.Root.Name.LocalName].ConvertEntries(xelement4, Program.m_wmToCsiLoader.Plugins, config, config.Bld.WM.Root);
				if (config.ExitStatus == ExitStatus.SKIPPED)
				{
					return;
				}
				Program.m_wmToCsiLoader.ValidateOutput(new XDocument(new object[]
				{
					xelement4
				}));
				if (Program.m_csiToCsiLoader == null)
				{
					Program.m_csiToCsiLoader = new PkgBldrLoader(PluginType.CsiToCsi, Program.m_cmdArgs, null);
				}
				Program.m_csiToCsiLoader.Plugins[xelement4.Name.LocalName].ConvertEntries(xelement4, Program.m_csiToCsiLoader.Plugins, config, xelement4);
				XDocument xdocument8 = new XDocument(new object[]
				{
					xelement4
				});
				Program.m_csiToCsiLoader.ValidateOutput(xdocument8);
				if (config.GenerateCab)
				{
					config.Macros = new MacroResolver();
					PkgBldrLoader pkgBldrLoader3 = new PkgBldrLoader(PluginType.CsiToCab, Program.m_cmdArgs, null);
					pkgBldrLoader3.Plugins[xelement4.Name.LocalName].ConvertEntries(xelement4, pkgBldrLoader3.Plugins, config, xelement4);
					return;
				}
				PkgBldrHelpers.XDocumentSaveToLongPath(xdocument8, config.Output);
				return;
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002534 File Offset: 0x00000734
		private static void ReformatManifest(string fileName, Config environ)
		{
			Process process = new Process();
			process.StartInfo.FileName = Environment.ExpandEnvironmentVariables("%sdxroot%\\tools\\reformatmanifest.cmd");
			process.StartInfo.Arguments = string.Format("-inplace {0} ", fileName);
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.Start();
			process.StandardOutput.ReadToEnd();
			process.WaitForExit();
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000025B0 File Offset: 0x000007B0
		private static void WriteSchemas(string outputDir)
		{
			PkgBldrLoader pkgBldrLoader = new PkgBldrLoader(PluginType.WmToCsi, Program.m_cmdArgs, null);
			List<XmlSchema> list = pkgBldrLoader.WmSchemaSet();
			int num = 0;
			outputDir = outputDir.TrimEnd(new char[]
			{
				'\\'
			});
			foreach (XmlSchema schema in list)
			{
				string text = outputDir + "\\WM" + num.ToString(CultureInfo.InvariantCulture) + ".xsd";
				num++;
				Program.logger.LogInfo("Writing {0}", new object[]
				{
					text
				});
				Program.WriteXmlSchema(schema, text);
			}
			List<XmlSchema> list2 = pkgBldrLoader.CsiSchemaSet();
			num = 0;
			foreach (XmlSchema schema2 in list2)
			{
				string text2 = outputDir + "\\CSI" + num.ToString(CultureInfo.InvariantCulture) + ".xsd";
				num++;
				Program.logger.LogInfo("Writing {0}", new object[]
				{
					text2
				});
				Program.WriteXmlSchema(schema2, text2);
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000026EC File Offset: 0x000008EC
		private static void PkgToCab(Config config, List<string> spkgGenArgs, bool testSignOnly)
		{
			Build build = config.build;
			bool flag = false;
			if (Program.BuildWow(config.Input))
			{
				flag = true;
				build.AddGuest();
			}
			string wowDir = build.WowDir ?? Program.GetWowDirFromOutput(spkgGenArgs);
			int convertDsmThreadCount = config.pkgBldrArgs.ConvertDsmThreadCount;
			if (flag)
			{
				XElement xelement = null;
				using (List<Build.WowType>.Enumerator enumerator = build.GetWowTypes().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Build.WowType wowType = enumerator.Current;
						if (((config.Bld.Arch != CpuType.amd64 && config.Bld.Arch != CpuType.arm64) || wowType != Build.WowType.guest) && (!(config.build.WowBuilds == WowBuildType.HostOnly) || wowType != Build.WowType.guest) && (!(config.build.WowBuilds == WowBuildType.GuestOnly) || wowType == Build.WowType.guest))
						{
							XDocument unfiltered = PkgBldrHelpers.XDocumentLoadFromLongPath(config.Input);
							config.build.wow = wowType;
							Program.FilterPkgXml(unfiltered, out xelement, config);
							string text = Path.GetTempFileName();
							if (wowType != Build.WowType.host)
							{
								if (wowType == Build.WowType.guest)
								{
									text += ".guest.pkg.xml";
								}
							}
							else
							{
								text += ".host.pkg.xml";
							}
							XDocument document = new XDocument(new object[]
							{
								xelement
							});
							Program.logger.LogInfo("PkgFilter: {0}", new object[]
							{
								text
							});
							PkgBldrHelpers.XDocumentSaveToLongPath(document, text);
							Program.ChangeSpkgGenInput(spkgGenArgs, text);
							string tempDirectory = Microsoft.CompPlat.PkgBldr.Tools.FileUtils.GetTempDirectory();
							if (wowType == Build.WowType.guest)
							{
								Program.RedirectOutput(spkgGenArgs, tempDirectory);
							}
							bool inWindows = false;
							List<string> spkgList = new List<string>();
							Run.RunSPkgGen(spkgGenArgs, inWindows, Program.logger, Program.m_cmdArgs, spkgList);
							if (wowType == Build.WowType.guest)
							{
								Program.RunConvertDsmInParellel(spkgList, testSignOnly, convertDsmThreadCount, true, tempDirectory, wowDir);
							}
							else
							{
								Program.RunConvertDsmInParellel(spkgList, testSignOnly, convertDsmThreadCount, false, null, null);
							}
						}
					}
					return;
				}
			}
			bool inWindows2 = false;
			List<string> spkgList2 = new List<string>();
			Run.RunSPkgGen(spkgGenArgs, inWindows2, Program.logger, Program.m_cmdArgs, spkgList2);
			Program.RunConvertDsmInParellel(spkgList2, testSignOnly, convertDsmThreadCount, false, null, null);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002920 File Offset: 0x00000B20
		private static void RunConvertDsmInParellel(List<string> spkgList, bool testSignOnly, int convertDsmThreadCount, bool isGuest = false, string tempDir = null, string wowDir = null)
		{
			ParallelOptions parallelOptions = new ParallelOptions();
			parallelOptions.MaxDegreeOfParallelism = convertDsmThreadCount;
			if (parallelOptions.MaxDegreeOfParallelism != 0)
			{
				Program.logger.LogInfo("Using no more than {0} threads for initializing DsmConvertJobs", new object[]
				{
					parallelOptions.MaxDegreeOfParallelism
				});
			}
			Parallel.ForEach<string>(spkgList, parallelOptions, delegate(string spkg)
			{
				if (isGuest)
				{
					Run.RunDsmConverter(spkg, wowDir, true, false, testSignOnly);
					Program.CopyDsmXmlToWowDir(spkg, tempDir, wowDir);
					Microsoft.CompPlat.PkgBldr.Tools.LongPathFile.Delete(spkg);
					Program.logger.LogInfo("Done: {0}", new object[]
					{
						Path.Combine(wowDir, Path.GetFileName(spkg.Replace(".spkg", ".cab")))
					});
					return;
				}
				Run.RunDsmConverter(spkg, false, false, testSignOnly);
				Program.logger.LogInfo("Done: {0}", new object[]
				{
					spkg.Replace(".spkg", ".cab")
				});
			});
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000029A0 File Offset: 0x00000BA0
		private static void CopyDsmXmlToWowDir(string spkg, string tempDir, string wowDir)
		{
			string path = Path.GetFileNameWithoutExtension(spkg) + ".man.dsm.xml";
			string text = Path.Combine(tempDir, path);
			string destinationPath = Path.Combine(wowDir, path);
			if (Microsoft.CompPlat.PkgBldr.Tools.LongPathFile.Exists(text))
			{
				Microsoft.CompPlat.PkgBldr.Tools.LongPathFile.Copy(text, destinationPath, true);
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000029E0 File Offset: 0x00000BE0
		private static string GetWowDirFromOutput(List<string> spkgGenArgs)
		{
			string result = null;
			string outputOption = Program.GetOutputOption(spkgGenArgs);
			if (outputOption != null)
			{
				int num = outputOption.IndexOf(':');
				result = Regex.Replace(outputOption.Substring(num + 1), "\\\\prebuilt\\\\", "\\prebuilt\\wow\\", RegexOptions.IgnoreCase);
			}
			return result;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002A20 File Offset: 0x00000C20
		private static int Main(string[] args)
		{
			Program.logger = new Logger();
			int result;
			try
			{
				Program.logger.LogInfo(Microsoft.CompPlat.PkgBldr.Tools.CommonUtils.GetCopyrightString(), new object[0]);
				Program.m_cmdArgs = Microsoft.WindowsPhone.ImageUpdate.Tools.Common.CmdArgsParser.ParseArgs<PkgBldrCmd>(Program.FixArgs(args), new object[]
				{
					CmdModes.LegacySwitchFormat
				});
				if (args.Length == 0)
				{
					Microsoft.WindowsPhone.ImageUpdate.Tools.Common.CmdArgsParser.ParseUsage<PkgBldrCmd>(new List<CmdModes>
					{
						CmdModes.LegacySwitchFormat
					});
					result = -1;
				}
				else if (Program.m_cmdArgs == null)
				{
					result = -1;
				}
				else
				{
					if (Program.m_cmdArgs.quiet)
					{
						Program.logger.SetLoggingLevel(LoggingLevel.Warning);
					}
					else if (Program.m_cmdArgs.diagnostic)
					{
						Program.logger.SetLoggingLevel(LoggingLevel.Debug);
					}
					string version = Program.m_cmdArgs.version;
					Program.CheckVersion(ref version, Program.m_cmdArgs.usentverp);
					if (!string.IsNullOrEmpty(Program.m_cmdArgs.wmxsd))
					{
						Program.m_cmdArgs.wmxsd = Microsoft.CompPlat.PkgBldr.Tools.LongPath.GetFullPath(Program.m_cmdArgs.wmxsd.TrimEnd(new char[]
						{
							'\\'
						}));
						if (!Microsoft.CompPlat.PkgBldr.Tools.LongPathDirectory.Exists(Program.m_cmdArgs.wmxsd))
						{
							throw new PkgGenException("wmxsd directory {0} does not exist", new object[]
							{
								Program.m_cmdArgs.wmxsd
							});
						}
						Program.WriteSchemas(Program.m_cmdArgs.wmxsd);
						result = 0;
					}
					else
					{
						if (string.IsNullOrEmpty(Program.m_cmdArgs.variables))
						{
							Program.m_cmdArgs.variables = string.Format("BUILD_OS_VERSION={0}", version);
						}
						else
						{
							Program.m_cmdArgs.variables = Program.m_cmdArgs.variables.TrimEnd(new char[]
							{
								';'
							});
							PkgBldrCmd cmdArgs = Program.m_cmdArgs;
							cmdArgs.variables += string.Format(";BUILD_OS_VERSION={0}", version);
						}
						List<string> spkgGenArguments = Program.GetSpkgGenArguments(Program.m_cmdArgs, version);
						if (string.IsNullOrEmpty(Program.m_cmdArgs.project) && Program.m_cmdArgs.convert == ConversionType.pkg2cab)
						{
							bool inWindows = false;
							Run.RunSPkgGen(spkgGenArguments, inWindows, Program.logger, Program.m_cmdArgs, null);
							result = 0;
						}
						else
						{
							List<SatelliteId> list = new List<SatelliteId>();
							SatelliteId satelliteId = SatelliteId.Create(SatelliteType.Neutral, null);
							list.Add(satelliteId);
							if (!string.IsNullOrEmpty(Program.m_cmdArgs.languages))
							{
								List<SatelliteId> collection = (from x in Program.m_cmdArgs.languages.Split(new char[]
								{
									';'
								})
								select SatelliteId.Create(SatelliteType.Language, x)).ToList<SatelliteId>();
								list.AddRange(collection);
							}
							Config config = new Config();
							config.build = new Build();
							config.Input = Program.m_cmdArgs.project;
							config.Output = Program.m_cmdArgs.output;
							config.build.WowDir = Program.m_cmdArgs.wowdir;
							config.Convert = Program.m_cmdArgs.convert;
							config.build.WowBuilds = new WowBuildType?(Program.m_cmdArgs.wowbuild);
							config.ProcessInf = Program.m_cmdArgs.processInf;
							config.GenerateCab = Program.m_cmdArgs.makecab;
							config.Bld = new Bld();
							config.Bld.BuildMacros = new MacroResolver();
							config.Logger = Program.logger;
							config.pkgBldrArgs = Program.m_cmdArgs;
							if (!string.IsNullOrEmpty(Program.m_cmdArgs.variables))
							{
								config.Bld.BuildMacros.Register(Program.ImportCommandLineMacros(Program.m_cmdArgs.variables), false);
							}
							List<SatelliteId> list2 = null;
							if (!string.IsNullOrEmpty(Program.m_cmdArgs.resolutions))
							{
								list2 = (from x in Program.m_cmdArgs.resolutions.Split(new char[]
								{
									';'
								})
								select SatelliteId.Create(SatelliteType.Resolution, x.ToLowerInvariant())).ToList<SatelliteId>();
							}
							config.Bld.Version = version;
							config.Bld.Arch = Program.m_cmdArgs.cpu;
							config.Bld.Product = Program.m_cmdArgs.product;
							config.Bld.SetAllResolutions(list2);
							if (config.Convert == ConversionType.pkg2cab)
							{
								Program.PkgToCab(config, spkgGenArguments, Program.m_cmdArgs.testOnly);
								result = 0;
							}
							else
							{
								Program.VerifyInputExtension(config.Convert, config.Input);
								if (config.Convert == ConversionType.pkg2csi || config.Convert == ConversionType.pkg2wm)
								{
									config.build.satellite = satelliteId;
									config.Bld.JsonDepot = Program.m_cmdArgs.json;
									if (config.Convert != ConversionType.pkg2csi)
									{
										Program.BuildPackage(config);
										return 0;
									}
									config.Convert = ConversionType.pkg2wm;
									config.Output = null;
									Program.BuildPackage(config);
									config.Convert = ConversionType.wm2csi;
									PkgBldrHelpers.XDocumentSaveToLongPath(new XDocument(new object[]
									{
										config.Bld.WM.Root
									}), Microsoft.CompPlat.PkgBldr.Tools.LongPath.Combine(Program.m_cmdArgs.output, Path.GetRandomFileName() + ".wm.xml"));
								}
								config.build.AddGuest();
								foreach (SatelliteId satelliteId2 in list)
								{
									config.Output = Program.m_cmdArgs.output;
									config.build.satellite = satelliteId2;
									if (satelliteId2.Type != SatelliteType.Resolution)
									{
										foreach (Build.WowType wowType in config.build.GetWowTypes())
										{
											if ((config.Bld.Arch != CpuType.amd64 && config.Bld.Arch != CpuType.arm64) || wowType != Build.WowType.guest)
											{
												config.build.wow = wowType;
												config.Bld.Lang = null;
												config.Bld.Resolution = null;
												if (satelliteId2.Culture != null)
												{
													config.Bld.Lang = satelliteId2.Culture.ToLowerInvariant();
													config.Bld.BuildMacros.Register("langid", config.Bld.Lang, true);
												}
												else
												{
													config.Bld.Lang = "neutral";
												}
												if (wowType == Build.WowType.guest)
												{
													config.Bld.IsGuest = true;
												}
												else
												{
													config.Bld.IsGuest = false;
												}
												Program.BuildPackage(config);
											}
										}
									}
								}
								if (list2 != null)
								{
									foreach (SatelliteId satelliteId3 in list2)
									{
										config.Output = Program.m_cmdArgs.output;
										config.build.satellite = satelliteId3;
										config.build.wow = Build.WowType.host;
										config.Bld.Lang = "neutral";
										config.Bld.Resolution = satelliteId3.Id;
										Program.BuildPackage(config);
									}
								}
								result = 0;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Program.logger.LogInfo("--Stack Trace--", new object[0]);
				Program.logger.LogInfo(ex.StackTrace, new object[0]);
				Program.logger.LogInfo("--End Stack Trace--", new object[0]);
				string format = ex.Message.Replace(',', ' ');
				switch (Program.m_errorLevel)
				{
				case Program.ErrorLevel.error:
					Program.logger.LogError(format, new object[0]);
					result = -1;
					break;
				case Program.ErrorLevel.warn:
					Program.logger.LogWarning(format, new object[0]);
					result = 0;
					break;
				case Program.ErrorLevel.silent:
					Program.logger.LogInfo(format, new object[0]);
					result = 0;
					break;
				default:
					result = 0;
					break;
				}
			}
			return result;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00003248 File Offset: 0x00001448
		private static string[] FixArgs(string[] args)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			foreach (string text in args)
			{
				if (Program.IsNamedArg(text))
				{
					list2.Add(text);
				}
				else
				{
					list.Add(text);
				}
			}
			list.AddRange(list2);
			return list.ToArray();
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000329E File Offset: 0x0000149E
		private static bool IsNamedArg(string arg)
		{
			return arg.StartsWith("/", StringComparison.OrdinalIgnoreCase) || arg.StartsWith("+", StringComparison.OrdinalIgnoreCase) || arg.StartsWith("-", StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000032CC File Offset: 0x000014CC
		private static void CheckVersion(ref string version, bool usentverp)
		{
			if (!Regex.Match(version, "^[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+$", RegexOptions.CultureInvariant).Success)
			{
				throw new PkgGenException("Input version '{0}' is not correctly formatted.", new object[]
				{
					version
				});
			}
			if (usentverp)
			{
				version = Program.GetNtVerpVersion();
			}
			if (version.Split(new char[]
			{
				'.'
			}).Sum((string x) => int.Parse(x, CultureInfo.InvariantCulture)) == 0)
			{
				throw new PkgGenException("Version '{0}' can't be zero or package will fail to install", new object[]
				{
					version
				});
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00003360 File Offset: 0x00001560
		private static string GetNtVerpVersion()
		{
			string workingDirectory = Environment.ExpandEnvironmentVariables(Program.m_cmdArgs.razzleToolPath);
			string processName = Environment.ExpandEnvironmentVariables(Program.m_cmdArgs.toolPaths["perl"]);
			return Regex.Match(Run.RunProcess(workingDirectory, processName, "version.pl", Program.logger), "[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+", RegexOptions.CultureInvariant).Value;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000033BC File Offset: 0x000015BC
		private static void ChangeSpkgGenInput(List<string> sPkgGenArgs, string NewInput)
		{
			string item = null;
			foreach (string text in sPkgGenArgs)
			{
				if (text.TrimEnd(new char[]
				{
					'"'
				}).EndsWith(".pkg.xml", StringComparison.OrdinalIgnoreCase))
				{
					item = text;
					break;
				}
			}
			sPkgGenArgs.Remove(item);
			sPkgGenArgs.Add(NewInput);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00003438 File Offset: 0x00001638
		private static void RedirectOutput(List<string> spkgGenArgs, string newOutputDir)
		{
			string outputOption = Program.GetOutputOption(spkgGenArgs);
			if (!string.IsNullOrEmpty(outputOption))
			{
				Program.ReplaceOutputDirectory(spkgGenArgs, outputOption, newOutputDir);
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000345C File Offset: 0x0000165C
		private static void ReplaceOutputDirectory(List<string> spkgGenArgs, string oldOutputOption, string newOutputDirectory)
		{
			string text = Program.AddQuotes("/output:" + newOutputDirectory);
			spkgGenArgs.Remove(oldOutputOption);
			spkgGenArgs.Add(text);
			Program.logger.LogInfo("PkgFilter: SPkgGen output redirected to {0}", new object[]
			{
				text
			});
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000034A4 File Offset: 0x000016A4
		private static string GetOutputOption(List<string> spkgGenArgs)
		{
			string result = null;
			foreach (string text in spkgGenArgs)
			{
				if (text.StartsWith("/output:", StringComparison.OrdinalIgnoreCase))
				{
					result = text;
					break;
				}
			}
			return result;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00003500 File Offset: 0x00001700
		private static bool BuildWow(string input)
		{
			string attributeValue = PkgBldrHelpers.GetAttributeValue(PkgBldrHelpers.XDocumentLoadFromLongPath(input).Root, "BuildWow");
			if (attributeValue == null)
			{
				return false;
			}
			if (attributeValue.Equals("yes", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			throw new PkgGenException("BuildWow!=Yes");
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00003544 File Offset: 0x00001744
		private static void FilterPkgXml(XDocument unfiltered, out XElement filtered, Config config)
		{
			if (Program.m_pkgFilterLoader == null)
			{
				Program.m_pkgFilterLoader = new PkgBldrLoader(PluginType.PkgFilter, Program.m_cmdArgs, null);
			}
			filtered = new XElement("Package");
			Program.m_pkgFilterLoader.Plugins["PkgFilter"].ConvertEntries(filtered, Program.m_pkgFilterLoader.Plugins, config, unfiltered.Root);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000035A6 File Offset: 0x000017A6
		private static bool IsThisACsiManifest(XElement csiRoot)
		{
			return csiRoot.Element(csiRoot.Name.Namespace + "assemblyIdentity") != null;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000035C8 File Offset: 0x000017C8
		private static List<string> GetSpkgGenArguments(PkgBldrCmd pkgBldrArgs, string versionOverride)
		{
			List<string> list = new List<string>();
			if (!string.IsNullOrEmpty(pkgBldrArgs.project))
			{
				list.Add(Program.AddQuotes(pkgBldrArgs.project));
			}
			if (!string.IsNullOrEmpty(pkgBldrArgs.config))
			{
				list.Add(string.Format(CultureInfo.InvariantCulture, "/config:{0}", new object[]
				{
					Program.AddQuotes(pkgBldrArgs.config)
				}));
			}
			if (!string.IsNullOrEmpty(pkgBldrArgs.xsd))
			{
				list.Add(string.Format(CultureInfo.InvariantCulture, "/xsd:{0}", new object[]
				{
					Program.AddQuotes(pkgBldrArgs.xsd)
				}));
			}
			if (!string.IsNullOrEmpty(pkgBldrArgs.output))
			{
				list.Add(string.Format(CultureInfo.InvariantCulture, "/output:{0}", new object[]
				{
					Program.AddQuotes(pkgBldrArgs.output)
				}));
			}
			BuildType build = pkgBldrArgs.build;
			string text;
			if (build != BuildType.fre && build == BuildType.chk)
			{
				text = "chk";
			}
			else
			{
				text = "fre";
			}
			list.Add(string.Format(CultureInfo.InvariantCulture, "/build:{0}", new object[]
			{
				text
			}));
			string text2;
			switch (pkgBldrArgs.cpu)
			{
			case CpuType.x86:
				text2 = "X86";
				goto IL_140;
			case CpuType.amd64:
				text2 = "AMD64";
				goto IL_140;
			case CpuType.arm64:
				text2 = "ARM64";
				goto IL_140;
			}
			text2 = "ARM";
			IL_140:
			list.Add(string.Format(CultureInfo.InvariantCulture, "/cpu:{0}", new object[]
			{
				text2
			}));
			if (!string.IsNullOrEmpty(pkgBldrArgs.languages))
			{
				list.Add(string.Format(CultureInfo.InvariantCulture, "/languages:{0}", new object[]
				{
					Program.AddQuotes(pkgBldrArgs.languages)
				}));
			}
			if (!string.IsNullOrEmpty(pkgBldrArgs.resolutions))
			{
				list.Add(string.Format(CultureInfo.InvariantCulture, "/resolutions:{0}", new object[]
				{
					Program.AddQuotes(pkgBldrArgs.resolutions)
				}));
			}
			if (!string.IsNullOrEmpty(pkgBldrArgs.variables))
			{
				list.Add(string.Format(CultureInfo.InvariantCulture, "/variables:{0}", new object[]
				{
					Program.AddQuotes(pkgBldrArgs.variables)
				}));
			}
			if (!string.IsNullOrEmpty(pkgBldrArgs.spkgGenToolDirs))
			{
				list.Add(string.Format(CultureInfo.InvariantCulture, "/toolPaths:{0}", new object[]
				{
					Program.AddQuotes(pkgBldrArgs.spkgGenToolDirs)
				}));
			}
			if (pkgBldrArgs.toc)
			{
				list.Add("/toc");
			}
			if (pkgBldrArgs.compress)
			{
				list.Add("/compress");
			}
			if (pkgBldrArgs.diagnostic)
			{
				list.Add("/diagnostic");
			}
			if (pkgBldrArgs.nohives || pkgBldrArgs.onecore)
			{
				list.Add("/nohives");
			}
			if (pkgBldrArgs.isRazzleEnv)
			{
				list.Add("/isRazzleEnv");
			}
			list.Add(string.Format(CultureInfo.InvariantCulture, "/version:{0}", new object[]
			{
				versionOverride
			}));
			return list;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00003894 File Offset: 0x00001A94
		private static string AddQuotes(string arg)
		{
			string text = arg;
			if (arg.Contains(' '))
			{
				string value = Regex.Match(arg, "^/[^:]+:").Value;
				if (string.IsNullOrEmpty(value))
				{
					text = "\"" + arg;
				}
				else
				{
					text = Regex.Replace(arg, value, value + "\"");
				}
				text += "\"";
			}
			return text;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000038F4 File Offset: 0x00001AF4
		private static void WriteXmlSchema(XmlSchema schema, string filePath)
		{
			filePath = Microsoft.CompPlat.PkgBldr.Tools.LongPath.GetFullPath(filePath);
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				NewLineHandling = NewLineHandling.Replace
			};
			using (XmlWriter xmlWriter = XmlWriter.Create(filePath, settings))
			{
				schema.Write(xmlWriter);
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00003948 File Offset: 0x00001B48
		private static Dictionary<string, Macro> ImportCommandLineMacros(string variables)
		{
			Regex regex = new Regex("^(?<name>[[A-Za-z.0-9_{-][A-Za-z.0-9_+{}-]*)=\\s*(?<value>.*?)\\s*$");
			Dictionary<string, Macro> dictionary = new Dictionary<string, Macro>(StringComparer.OrdinalIgnoreCase);
			foreach (string text in variables.Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries))
			{
				Match match = regex.Match(text);
				if (match == null || !match.Success)
				{
					throw new PkgGenException("Incorrect syntax in variable definition '{0}'", new object[]
					{
						text
					});
				}
				string value = match.Groups["name"].Value;
				string text2 = match.Groups["value"].Value;
				if (text2.StartsWith("\"", StringComparison.OrdinalIgnoreCase))
				{
					if (!text2.EndsWith("\"", StringComparison.OrdinalIgnoreCase))
					{
						throw new PkgGenException("Incorrect syntax in variable definition '{0}'", new object[]
						{
							text
						});
					}
					text2 = text2.Substring(1, text2.Length - 2);
				}
				Macro macro = null;
				if (dictionary.TryGetValue(value, out macro))
				{
					Program.logger.LogWarning("Command line macro value overwriting, macro name:'{0}', old value:'{1}', new value:'{2}'", new object[]
					{
						value,
						macro.Value,
						text2
					});
				}
				Dictionary<string, Macro> dictionary2 = dictionary;
				string text3 = value;
				dictionary2[text3] = new Macro(text3, text2);
			}
			return dictionary;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00003A84 File Offset: 0x00001C84
		private static void VerifyInputExtension(ConversionType usage, string inputXml)
		{
			if (inputXml == null)
			{
				throw new PkgGenException("PkgGen project not set");
			}
			string text = null;
			switch (usage)
			{
			case ConversionType.wm2csi:
				text = ".wm.xml";
				break;
			case ConversionType.csi2wm:
			case ConversionType.csi2pkg:
				text = ".man";
				break;
			case ConversionType.pkg2wm:
			case ConversionType.pkg2csi:
			case ConversionType.pkg2cab:
				text = ".pkg.xml";
				break;
			}
			if (text != null && !inputXml.EndsWith(text, StringComparison.OrdinalIgnoreCase))
			{
				throw new PkgGenException("Input file {0} does not end with {1}", new object[]
				{
					inputXml,
					text
				});
			}
		}

		// Token: 0x04000001 RID: 1
		private const int ERROR_STATUS_SUCCESS = 0;

		// Token: 0x04000002 RID: 2
		private const int ERROR_STATUS_FAILED = -1;

		// Token: 0x04000003 RID: 3
		private const int ERROR_STATUS_SKIPPED = 1;

		// Token: 0x04000004 RID: 4
		private static Program.ErrorLevel m_errorLevel;

		// Token: 0x04000005 RID: 5
		private static PkgBldrLoader m_pkgFilterLoader;

		// Token: 0x04000006 RID: 6
		private static PkgBldrLoader m_pkgToWmLoader;

		// Token: 0x04000007 RID: 7
		private static PkgBldrLoader m_wmToCsiLoader;

		// Token: 0x04000008 RID: 8
		private static PkgBldrLoader m_csiToCsiLoader;

		// Token: 0x04000009 RID: 9
		private static PkgBldrCmd m_cmdArgs;

		// Token: 0x0400000A RID: 10
		private static XElement m_wmRoot;

		// Token: 0x0400000B RID: 11
		private static Logger logger;

		// Token: 0x02000006 RID: 6
		public enum ErrorLevel
		{
			// Token: 0x0400000E RID: 14
			error,
			// Token: 0x0400000F RID: 15
			warn,
			// Token: 0x04000010 RID: 16
			silent
		}
	}
}
