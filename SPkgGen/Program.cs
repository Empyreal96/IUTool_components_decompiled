using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Security.SecurityPolicyCompiler;

namespace Microsoft.WindowsPhone.ImageUpdate.PackageGenerator
{
	// Token: 0x02000003 RID: 3
	internal class Program
	{
		// Token: 0x06000006 RID: 6 RVA: 0x00002430 File Offset: 0x00000630
		private static void LogErrorToBuffer(string logString)
		{
			Program.ErrorBuffer.Add(logString);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002440 File Offset: 0x00000640
		private static void WriteErrorBuffer()
		{
			if (Program.ErrorBuffer.Count > 0)
			{
				LogUtil.Error("Previous errors from native components:");
				foreach (string message in Program.ErrorBuffer)
				{
					LogUtil.Error(message);
				}
			}
			Program.ErrorBuffer.Clear();
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000024B0 File Offset: 0x000006B0
		private static Dictionary<string, IPkgPlugin> LoadPackagePlugins()
		{
			CompositionContainer compositionContainer = null;
			try
			{
				string directoryName = LongPath.GetDirectoryName(typeof(IPkgPlugin).Assembly.Location);
				AggregateCatalog aggregateCatalog = new AggregateCatalog();
				aggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(IPkgPlugin).Assembly));
				if (LongPathDirectory.Exists(directoryName))
				{
					aggregateCatalog.Catalogs.Add(new DirectoryCatalog(directoryName, "PkgGen.Plugin.*.dll"));
				}
				CompositionBatch batch = new CompositionBatch();
				compositionContainer = new CompositionContainer(aggregateCatalog, new ExportProvider[0]);
				compositionContainer.Compose(batch);
			}
			catch (CompositionException innerException)
			{
				throw new PkgGenException(innerException, "Failed to load package plugins.", new object[0]);
			}
			Dictionary<string, IPkgPlugin> dictionary = new Dictionary<string, IPkgPlugin>();
			foreach (IPkgPlugin pkgPlugin in compositionContainer.GetExportedValues<IPkgPlugin>())
			{
				if (string.IsNullOrEmpty(pkgPlugin.XmlElementName))
				{
					throw new PkgGenException("Failed to load package plugin '{0}'. Invalid XmlElementName.", new object[]
					{
						pkgPlugin.Name
					});
				}
				try
				{
					dictionary.Add(pkgPlugin.XmlElementName, pkgPlugin);
				}
				catch (ArgumentException innerException2)
				{
					string fileName = Path.GetFileName(pkgPlugin.GetType().Assembly.Location);
					IPkgPlugin pkgPlugin2 = dictionary[pkgPlugin.XmlElementName];
					string fileName2 = Path.GetFileName(pkgPlugin2.GetType().Assembly.Location);
					throw new PkgGenException(innerException2, "Failed to load package plugin '{0}' ({1}). Uses a duplicate XmlElementName with '{2}' ({3}).", new object[]
					{
						pkgPlugin.Name,
						fileName,
						pkgPlugin2.Name,
						fileName2
					});
				}
			}
			return dictionary;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002658 File Offset: 0x00000858
		private static void OnSchemaValidationEvent(object sender, ValidationEventArgs e)
		{
			if (e.Exception != null)
			{
				throw e.Exception;
			}
			throw new PkgGenException("Schema validation error: {0}", new object[]
			{
				e.Message
			});
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002684 File Offset: 0x00000884
		private static void WriteXmlSchema(XmlSchema schema, string filePath)
		{
			LogUtil.Message("Writing out XSD to '{0}'", new object[]
			{
				filePath
			});
			filePath = LongPath.GetFullPath(filePath);
			if (!LongPathDirectory.Exists(LongPath.GetDirectoryName(filePath)))
			{
				LongPathDirectory.CreateDirectory(LongPath.GetDirectoryName(filePath));
			}
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

		// Token: 0x0600000B RID: 11 RVA: 0x00002704 File Offset: 0x00000904
		private static void ImportCommandLineMacros(MacroResolver macroResolver, string variables)
		{
			Regex regex = new Regex("^(?<name>[A-Za-z.0-9_{-][A-Za-z.0-9_+{}-]*)=\\s*(?<value>.*?)\\s*$");
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
				if (text2.StartsWith("\"", StringComparison.InvariantCulture))
				{
					if (!text2.EndsWith("\"", StringComparison.InvariantCulture))
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
					LogUtil.Warning("Command line macro value overwriting, macro name:'{0}', old value:'{1}', new value:'{2}'", new object[]
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
			macroResolver.Register(dictionary);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002844 File Offset: 0x00000A44
		private static void ImportGlobalMacros(MacroResolver macroResolver, XmlValidator schemaValidator)
		{
			try
			{
				using (XmlReader xmlReader = schemaValidator.GetXmlReader(PkgGenResources.GetGlobalMacroStream()))
				{
					macroResolver.Load(xmlReader);
				}
			}
			catch (XmlSchemaValidationException ex)
			{
				throw new PkgGenException(ex, "Schema validation failed on embedded global macro file at line '{0}'", new object[]
				{
					ex.LineNumber
				});
			}
			catch (Exception innerException)
			{
				throw new PkgGenException(innerException, "Failed to load global macro definitions from embeded stream", new object[0]);
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000028CC File Offset: 0x00000ACC
		private static void ImportGlobalMacros(MacroResolver macroResolver, string file, XmlValidator schemaValidator)
		{
			if (!LongPathFile.Exists(file))
			{
				throw new PkgGenException("Global macro file '{0}' doesn't exist", new object[]
				{
					file
				});
			}
			try
			{
				using (XmlReader xmlReader = schemaValidator.GetXmlReader(file))
				{
					macroResolver.Load(xmlReader);
				}
			}
			catch (XmlSchemaValidationException ex)
			{
				throw new PkgGenException(ex, "Schema validation failed on global macro file '{0}' at line '{1}'.", new object[]
				{
					file,
					ex.LineNumber
				});
			}
			catch (Exception innerException)
			{
				throw new PkgGenException(innerException, "Failed to load global macro definitions from file '{0}'", new object[]
				{
					file
				});
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002978 File Offset: 0x00000B78
		private static int Main(string[] args)
		{
			int result;
			try
			{
				LogUtil.IULogTo(new LogUtil.InteropLogString(Program.LogErrorToBuffer), new LogUtil.InteropLogString(LogUtil.Warning), new LogUtil.InteropLogString(LogUtil.Message), new LogUtil.InteropLogString(LogUtil.Diagnostic));
				LogUtil.LogCopyright();
				CommandLineParser commandLineParser = new CommandLineParser();
				commandLineParser.SetOptionalParameterString("project", "Full path to the package project file");
				commandLineParser.SetOptionalSwitchString("config", "File with globally defined variables");
				commandLineParser.SetOptionalSwitchString("xsd", "Path to write the PkgGen auto-generated schema to");
				commandLineParser.SetOptionalSwitchString("output", "Output directory", ".", new string[0]);
				commandLineParser.SetOptionalSwitchString("version", "Version string in the form of <major>.<minor>.<qfe>.<build>", "0.0.0.0", new string[0]);
				commandLineParser.SetOptionalSwitchString("build", "Build type string", "fre", false, new string[]
				{
					"fre",
					"chk"
				});
				commandLineParser.SetOptionalSwitchString("cpu", "CPU type", "ARM", false, new string[]
				{
					"X86",
					"ARM",
					"ARM64",
					"AMD64"
				});
				commandLineParser.SetOptionalSwitchString("languages", "Supported language identifier list, separated by ';'", string.Empty, new string[0]);
				commandLineParser.SetOptionalSwitchString("resolutions", "Supported resolution identifier list, separated by ';'", string.Empty, new string[0]);
				commandLineParser.SetOptionalSwitchString("variables", "Additional variables used in the project file, syntax:<name>=<value>;<name>=<value>;...");
				commandLineParser.SetOptionalSwitchString("spkgsout", "Create an output file containing a list of generated SPKG's", string.Empty, new string[0]);
				commandLineParser.SetOptionalSwitchString("toolPaths", "Directories containing tools needed by spkggen.exe", string.Empty, new string[0]);
				commandLineParser.SetOptionalSwitchBoolean("toc", "Building TOC files instead of the actual package", false);
				commandLineParser.SetOptionalSwitchBoolean("compress", "Compressing the generated package", false);
				commandLineParser.SetOptionalSwitchBoolean("diagnostic", "Enable debug output", false);
				commandLineParser.SetOptionalSwitchBoolean("nohives", "Indicates whether or not this package has no hive dependency", false);
				commandLineParser.SetOptionalSwitchBoolean("isRazzleEnv", "Indicates whether or not spkggen is running in a razzle environment", false);
				if (!commandLineParser.ParseCommandLine())
				{
					LogUtil.Error("Invalid command line arguments:{0}", new object[]
					{
						commandLineParser.LastError
					});
					LogUtil.Message(commandLineParser.UsageString());
					result = -1;
				}
				else
				{
					LogUtil.SetVerbosity(commandLineParser.GetSwitchAsBoolean("diagnostic"));
					string text = commandLineParser.GetParameterAsString("project");
					string switchAsString = commandLineParser.GetSwitchAsString("xsd");
					string switchAsString2 = commandLineParser.GetSwitchAsString("config");
					if (string.IsNullOrEmpty(switchAsString) && string.IsNullOrEmpty(text))
					{
						throw new PkgGenException("Must provide a project path or use the /xsd switch.", new object[0]);
					}
					Dictionary<string, IPkgPlugin> dictionary = Program.LoadPackagePlugins();
					MergingSchemaValidator mergingSchemaValidator = new MergingSchemaValidator(new ValidationEventHandler(Program.OnSchemaValidationEvent));
					XmlSchema schema = mergingSchemaValidator.AddSchemaWithPlugins(PkgGenResources.GetProjSchemaStream(), dictionary.Values);
					if (!string.IsNullOrEmpty(switchAsString))
					{
						Program.WriteXmlSchema(schema, switchAsString);
					}
					if (string.IsNullOrEmpty(text))
					{
						result = 0;
					}
					else
					{
						text = LongPath.GetFullPath(text);
						if (!text.EndsWith(".pkg.xml", StringComparison.OrdinalIgnoreCase))
						{
							throw new PkgGenException("Invalid input project file '{0}', project file must have an extension of '{1}'", new object[]
							{
								text,
								".pkg.xml"
							});
						}
						string switchAsString3 = commandLineParser.GetSwitchAsString("variables");
						MacroResolver macroResolver = new MacroResolver();
						if (string.IsNullOrEmpty(switchAsString2))
						{
							LogUtil.Message("Using embedded macro file");
							Program.ImportGlobalMacros(macroResolver, mergingSchemaValidator);
						}
						else
						{
							LogUtil.Message("Using external macro file: '{0}'", new object[]
							{
								switchAsString2
							});
							Program.ImportGlobalMacros(macroResolver, switchAsString2, mergingSchemaValidator);
						}
						if (switchAsString3 != null)
						{
							Program.ImportCommandLineMacros(macroResolver, switchAsString3);
						}
						bool switchAsBoolean = commandLineParser.GetSwitchAsBoolean("toc");
						string switchAsString4 = commandLineParser.GetSwitchAsString("cpu");
						string switchAsString5 = commandLineParser.GetSwitchAsString("build");
						string switchAsString6 = commandLineParser.GetSwitchAsString("version");
						string switchAsString7 = commandLineParser.GetSwitchAsString("languages");
						string switchAsString8 = commandLineParser.GetSwitchAsString("resolutions");
						string switchAsString9 = commandLineParser.GetSwitchAsString("toolPaths");
						bool switchAsBoolean2 = commandLineParser.GetSwitchAsBoolean("compress");
						if (commandLineParser.GetSwitchAsBoolean("nohives"))
						{
							macroResolver.Register("__nohives", "true", true);
						}
						bool switchAsBoolean3 = commandLineParser.GetSwitchAsBoolean("isRazzleEnv");
						string text2 = commandLineParser.GetSwitchAsString("spkgsout");
						if (string.IsNullOrEmpty(text2))
						{
							text2 = null;
						}
						if (text2 != null)
						{
							string directoryName = LongPath.GetDirectoryName(text2);
							if (!LongPathDirectory.Exists(directoryName))
							{
								LongPathDirectory.CreateDirectory(directoryName);
							}
							if (LongPathFile.Exists(text2))
							{
								LongPathFile.Delete(text2);
							}
							using (new StreamWriter(text2))
							{
							}
						}
						PackageGenerator packageGenerator = new PackageGenerator(dictionary, switchAsBoolean ? BuildPass.BuildTOC : BuildPass.BuildPkg, CpuIdParser.Parse(switchAsString4), BuildTypeParser.Parse(switchAsString5), VersionInfo.Parse(switchAsString6), new PolicyCompiler(), macroResolver, mergingSchemaValidator, switchAsString7, switchAsString8, switchAsString9, switchAsBoolean3, switchAsBoolean2, text2);
						string switchAsString10 = commandLineParser.GetSwitchAsString("output");
						ProcessPrivilege.Adjust(PrivilegeNames.BackupPrivilege, true);
						ProcessPrivilege.Adjust(PrivilegeNames.SecurityPrivilege, true);
						LogUtil.Message("Building project file {0}", new object[]
						{
							text
						});
						packageGenerator.Build(text, switchAsString10, switchAsBoolean2);
						LogUtil.Message("Packages are generated to {0} successfully", new object[]
						{
							switchAsString10
						});
						ProcessPrivilege.Adjust(PrivilegeNames.BackupPrivilege, false);
						ProcessPrivilege.Adjust(PrivilegeNames.SecurityPrivilege, false);
						result = 0;
					}
				}
			}
			catch (PkgGenException ex)
			{
				Program.WriteErrorBuffer();
				LogUtil.Error(ex.MessageTrace);
				result = Marshal.GetHRForException(ex);
			}
			catch (PackageException ex2)
			{
				Program.WriteErrorBuffer();
				LogUtil.Error(ex2.MessageTrace);
				result = -1;
			}
			catch (PolicyCompilerInternalException ex3)
			{
				Program.WriteErrorBuffer();
				LogUtil.Error(ex3.ToString());
				result = -1;
			}
			catch (Exception ex4)
			{
				Program.WriteErrorBuffer();
				LogUtil.Error("Uncaught exception occured: {0}", new object[]
				{
					ex4.ToString()
				});
				result = -1;
			}
			return result;
		}

		// Token: 0x04000001 RID: 1
		private const string c_strProjExtension = ".pkg.xml";

		// Token: 0x04000002 RID: 2
		private static readonly List<string> ErrorBuffer = new List<string>();
	}
}
