using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgSignTool
{
	// Token: 0x02000003 RID: 3
	internal class Program
	{
		// Token: 0x06000009 RID: 9 RVA: 0x00002274 File Offset: 0x00000474
		private static Dictionary<string, string> CreateFileMap(string outputDir, PkgManifest manifest)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = 0;
			FileEntry[] files = manifest.Files;
			for (int i = 0; i < files.Length; i++)
			{
				Program.<>c__DisplayClass2_0 CS$<>8__locals1 = new Program.<>c__DisplayClass2_0();
				CS$<>8__locals1.fe = files[i];
				if (CS$<>8__locals1.fe.FileType == FileType.Regular && !CS$<>8__locals1.fe.SourcePath.EndsWith("update.mum", StringComparison.InvariantCultureIgnoreCase) && !CS$<>8__locals1.fe.SourcePath.EndsWith("man.dsm.xml", StringComparison.InvariantCultureIgnoreCase) && (!CS$<>8__locals1.fe.SourcePath.EndsWith("cat", StringComparison.InvariantCultureIgnoreCase) || !(Path.GetFileName(CS$<>8__locals1.fe.SourcePath) == CS$<>8__locals1.fe.CabPath)))
				{
					string text;
					for (;;)
					{
						text = Path.Combine(outputDir, num.ToString(CultureInfo.InvariantCulture) + Path.GetExtension(CS$<>8__locals1.fe.SourcePath));
						if (dictionary.Values.Contains(CS$<>8__locals1.fe.SourcePath))
						{
							break;
						}
						if (!File.Exists(text))
						{
							goto IL_14A;
						}
						num++;
					}
					IEnumerable<KeyValuePair<string, string>> source = dictionary;
					Func<KeyValuePair<string, string>, bool> predicate;
					if ((predicate = CS$<>8__locals1.<>9__0) == null)
					{
						Program.<>c__DisplayClass2_0 CS$<>8__locals2 = CS$<>8__locals1;
						predicate = (CS$<>8__locals2.<>9__0 = ((KeyValuePair<string, string> x) => x.Value == CS$<>8__locals2.fe.SourcePath));
					}
					text = source.First(predicate).Key;
					CS$<>8__locals1.fe.SourcePath = text;
					goto IL_183;
					IL_14A:
					LongPathFile.Move(CS$<>8__locals1.fe.SourcePath, text);
					dictionary.Add(text, CS$<>8__locals1.fe.SourcePath);
					CS$<>8__locals1.fe.SourcePath = text;
					num++;
				}
				IL_183:;
			}
			return dictionary;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002414 File Offset: 0x00000614
		private static Dictionary<string, string> ReadFileMap(string inputDir)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string path = Path.Combine(inputDir, "filemap.csv");
			if (!File.Exists(path))
			{
				return dictionary;
			}
			using (StreamReader streamReader = new StreamReader(path))
			{
				while (!streamReader.EndOfStream)
				{
					string text = streamReader.ReadLine();
					string[] array = text.Split(new char[]
					{
						','
					});
					if (2 != array.Length)
					{
						throw new Exception(string.Concat(new object[]
						{
							"Expected two entries in filemap.csv line.  Found ",
							array.Length,
							".\nLine: ",
							text
						}));
					}
					dictionary.Add(array[0], array[1]);
				}
			}
			return dictionary;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000024CC File Offset: 0x000006CC
		private static int Main(string[] args)
		{
			MultiCmdHandler multiCmdHandler = new MultiCmdHandler();
			multiCmdHandler.AddCmdHandler(new Program.UnpackCmdHandler());
			multiCmdHandler.AddCmdHandler(new Program.UpdateCmdHandler());
			multiCmdHandler.AddCmdHandler(new Program.RepackCmdHandler());
			ProcessPrivilege.Adjust(PrivilegeNames.BackupPrivilege, true);
			ProcessPrivilege.Adjust(PrivilegeNames.SecurityPrivilege, true);
			int result = -1;
			try
			{
				result = multiCmdHandler.Run(args);
			}
			catch (Exception ex)
			{
				Program.logger.LogException(ex);
				result = Marshal.GetHRForException(ex);
			}
			ProcessPrivilege.Adjust(PrivilegeNames.BackupPrivilege, false);
			ProcessPrivilege.Adjust(PrivilegeNames.SecurityPrivilege, false);
			return result;
		}

		// Token: 0x04000009 RID: 9
		private static string c_OpaqueContainerCookieFileName = "PkgSignToolOpaqueContainer.txt";

		// Token: 0x0400000A RID: 10
		private static IULogger logger = new IULogger();

		// Token: 0x02000004 RID: 4
		private class UnpackCmdHandler : QuietCmdHandler
		{
			// Token: 0x0600000E RID: 14 RVA: 0x00002578 File Offset: 0x00000778
			private static bool ShouldSign(FileEntry fe)
			{
				return fe.FileType == FileType.Regular && !fe.DevicePath.Contains("windows\\InfusedApps\\") && !fe.DevicePath.EndsWith("bootia32.efi", StringComparison.InvariantCultureIgnoreCase) && !fe.DevicePath.EndsWith("bootx64.efi", StringComparison.InvariantCultureIgnoreCase) && !fe.DevicePath.EndsWith("bootarm.efi", StringComparison.InvariantCultureIgnoreCase) && !fe.DevicePath.EndsWith("bootaa64.efi", StringComparison.InvariantCultureIgnoreCase) && !fe.DevicePath.EndsWith("sigcheck.efi", StringComparison.InvariantCultureIgnoreCase);
			}

			// Token: 0x0600000F RID: 15 RVA: 0x00002608 File Offset: 0x00000808
			private static void PkgUnpack(string package, string outputDir)
			{
				if (!LongPathFile.Exists(package))
				{
					throw new FileNotFoundException(string.Format("The specified package ('{0}') doesn't exist", package));
				}
				if (!LongPathDirectory.Exists(outputDir))
				{
					LongPathDirectory.CreateDirectory(outputDir);
				}
				IPkgInfo pkgInfo = Package.LoadFromCab(package);
				List<FileEntryBase> list = new List<FileEntryBase>();
				switch (pkgInfo.Type)
				{
				case PackageType.Canonical:
				case PackageType.Removal:
				{
					CabApiWrapper.Extract(package, outputDir);
					string path = PkgConstants.c_strCatalogFile;
					PkgManifest pkgManifest;
					if (pkgInfo.Style == PackageStyle.CBS)
					{
						Path.Combine(outputDir, PkgConstants.c_strMumFile);
						pkgManifest = PkgManifest.Load_CBS(outputDir);
						path = PkgConstants.c_strCBSCatalogFile;
					}
					else
					{
						pkgManifest = PkgManifest.Load(Path.Combine(outputDir, PkgConstants.c_strDsmFile));
					}
					pkgManifest.BuildSourcePaths(outputDir, BuildPathOption.UseCabPath);
					list.AddRange(pkgManifest.Files);
					if (pkgInfo.Style == PackageStyle.CBS)
					{
						Dictionary<string, string> fileMap = Program.CreateFileMap(outputDir, pkgManifest);
						LongPathFile.WriteAllLines(Path.Combine(outputDir, "filemap.csv"), from x in fileMap.Keys
						select fileMap[x] + "," + x);
					}
					LongPathFile.WriteAllLines(Path.Combine(outputDir, "embedded_sign.csv"), from x in pkgManifest.Files
					where Program.UnpackCmdHandler.ShouldSign(x)
					select x.SourcePath);
					IEnumerable<FileEntry> source = from x in pkgManifest.Files
					where x.FileType != FileType.Catalog
					select x;
					PackageTools.CreateCDF((from x in source
					select x.SourcePath).ToArray<string>(), (from x in source
					select x.DevicePath).ToArray<string>(), Path.Combine(outputDir, path), PackageTools.GetCatalogPackageName(pkgInfo), Path.Combine(outputDir, "content.cdf"));
					using (TextWriter textWriter = new StreamWriter(Path.Combine(outputDir, "files.txt")))
					{
						foreach (FileEntryBase fileEntryBase in list)
						{
							FileAttributes attributes = LongPathFile.GetAttributes(fileEntryBase.SourcePath);
							if (attributes.HasFlag(FileAttributes.ReadOnly))
							{
								LongPathFile.SetAttributes(fileEntryBase.SourcePath, attributes & ~FileAttributes.ReadOnly);
							}
							NativeMethods.SetLastWriteTimeLong(fileEntryBase.SourcePath, DateTime.Now);
							if (attributes.HasFlag(FileAttributes.ReadOnly))
							{
								LongPathFile.SetAttributes(fileEntryBase.SourcePath, attributes);
							}
							textWriter.WriteLine("{0},{1},{2}", fileEntryBase.SourcePath, fileEntryBase.DevicePath, NativeMethods.GetCreationTimeLong(fileEntryBase.SourcePath));
						}
					}
					Program.logger.LogInfo("PkgSignTool: Successfully unpacked {0} to {1}.", new object[]
					{
						package,
						outputDir
					});
					return;
				}
				case PackageType.Diff:
				{
					string text = Path.Combine(outputDir, Program.c_OpaqueContainerCookieFileName);
					string fullPathUNC = LongPath.GetFullPathUNC(package);
					Program.logger.LogInfo("PkgSignTool: Creating cookie indicating this is an opaque file at {0}.", new object[]
					{
						text
					});
					using (TextWriter textWriter2 = new StreamWriter(text))
					{
						textWriter2.WriteLine(fullPathUNC);
					}
					return;
				}
				default:
					throw new PackageException("Unexpected package type '{0}'. The package may contain a corrupted {1}.", new object[]
					{
						pkgInfo.Type,
						(pkgInfo.Style == PackageStyle.CBS) ? "update.mum" : "man.dsm.xml"
					});
				}
			}

			// Token: 0x06000010 RID: 16 RVA: 0x000029C0 File Offset: 0x00000BC0
			protected override int DoExecution()
			{
				string parameterAsString = this._cmdLineParser.GetParameterAsString("input");
				string switchAsString = this._cmdLineParser.GetSwitchAsString("out");
				base.SetLoggingVerbosity(Program.logger);
				Program.UnpackCmdHandler.PkgUnpack(parameterAsString, switchAsString);
				return 0;
			}

			// Token: 0x17000001 RID: 1
			// (get) Token: 0x06000011 RID: 17 RVA: 0x00002A00 File Offset: 0x00000C00
			public override string Command
			{
				get
				{
					return "Unpack";
				}
			}

			// Token: 0x17000002 RID: 2
			// (get) Token: 0x06000012 RID: 18 RVA: 0x00002A07 File Offset: 0x00000C07
			public override string Description
			{
				get
				{
					return "Unpacking a given package to the specified output directory";
				}
			}

			// Token: 0x06000013 RID: 19 RVA: 0x00002A10 File Offset: 0x00000C10
			public UnpackCmdHandler()
			{
				this._cmdLineParser.SetRequiredParameterString("input", "path to the package to be unpacked");
				this._cmdLineParser.SetOptionalSwitchString("out", "root output directory for the unpacked package", ".", new string[0]);
				base.SetQuietCommand();
			}

			// Token: 0x0400000B RID: 11
			private const string EFI_I386_BOOT_NAME = "bootia32.efi";

			// Token: 0x0400000C RID: 12
			private const string EFI_AMD64_BOOT_NAME = "bootx64.efi";

			// Token: 0x0400000D RID: 13
			private const string EFI_ARMNT_BOOT_NAME = "bootarm.efi";

			// Token: 0x0400000E RID: 14
			private const string EFI_ARM64_BOOT_NAME = "bootaa64.efi";

			// Token: 0x0400000F RID: 15
			private const string EFI_SIGCHECK_NAME = "sigcheck.efi";

			// Token: 0x04000010 RID: 16
			private const string APPX_INFUSEDAPPS_PATH = "windows\\InfusedApps\\";
		}

		// Token: 0x02000005 RID: 5
		private class UpdateCmdHandler : QuietCmdHandler
		{
			// Token: 0x06000014 RID: 20 RVA: 0x00002A60 File Offset: 0x00000C60
			private static void PkgUpdate(string inputDir, bool incVersion)
			{
				Dictionary<string, string> dictionary = Program.ReadFileMap(inputDir);
				List<string> list = new List<string>();
				PackageStyle packageStyle = PackageStyle.SPKG;
				string text = Path.Combine(inputDir, PkgConstants.c_strDsmFile);
				if (!LongPathFile.Exists(text))
				{
					text = Path.Combine(inputDir, PkgConstants.c_strMumFile);
					packageStyle = PackageStyle.CBS;
					if (!LongPathFile.Exists(text))
					{
						throw new FileNotFoundException(string.Format("The specified input directory '{0}' doesn't have man.dsm.xml or update.mum file. Unable to deduce type of package provided.", inputDir));
					}
				}
				if (packageStyle != PackageStyle.CBS)
				{
					return;
				}
				PkgManifest pkgManifest = PkgManifest.Load_CBS(text);
				List<FileEntryBase> list2 = new List<FileEntryBase>();
				list2.AddRange(pkgManifest.Files);
				SHA256 sha = SHA256.Create();
				foreach (FileEntryBase fileEntryBase in list2)
				{
					FileEntry fileEntry = (FileEntry)fileEntryBase;
					if (fileEntry.FileType == FileType.Manifest)
					{
						string cabPath = fileEntry.CabPath;
						if (cabPath.EndsWith("manifest", StringComparison.InvariantCultureIgnoreCase) || cabPath.EndsWith("mum", StringComparison.InvariantCultureIgnoreCase))
						{
							string text2 = Path.Combine(inputDir, cabPath);
							Program.logger.LogInfo("PkgSignTool: Updating hashes for manifest at path: {0}", new object[]
							{
								text2
							});
							string tempFileName = Path.GetTempFileName();
							Program.logger.LogInfo("PkgSignTool: Copying manifest to temp file: {0}", new object[]
							{
								tempFileName
							});
							LongPathFile.Copy(text2, tempFileName, true);
							list.Add(tempFileName);
							XDocument xdocument = XDocument.Load(tempFileName);
							XNamespace @namespace = xdocument.Root.Name.Namespace;
							string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(cabPath);
							if (cabPath.EndsWith("mum", StringComparison.InvariantCultureIgnoreCase))
							{
								if (!incVersion)
								{
									continue;
								}
								Program.logger.LogInfo("Increasing version number in mum file.", new object[0]);
								XElement xelement = xdocument.Root.Descendants(@namespace + "assemblyIdentity").First<XElement>();
								VersionInfo version = pkgManifest.Version;
								version.Build += 1;
								xelement.Attribute("version").Value = version.ToString();
							}
							else
							{
								foreach (XElement xelement2 in xdocument.Root.Descendants(@namespace + "file"))
								{
									string value = xelement2.Attribute("name").Value;
									string path = "";
									XNamespace ns = "urn:schemas-microsoft-com:asm.v2";
									XNamespace ns2 = "http://www.w3.org/2000/09/xmldsig#";
									XElement xelement3 = xelement2.Element(ns + "hash");
									if (xelement3 != null)
									{
										xelement3 = xelement3.Element(ns2 + "DigestValue");
										if (xelement3 != null && !xelement3.Value.Equals("00000000000000000000000000000000000000000000"))
										{
											XAttribute xattribute = xelement2.Attribute("name");
											XAttribute xattribute2 = xelement2.Attribute("sourcePath");
											if (xattribute2 != null)
											{
												path = xattribute2.Value;
											}
											string path2;
											if (xattribute != null)
											{
												path2 = xattribute.Value;
											}
											else
											{
												path2 = Path.GetFileName(value);
												string directoryName = Path.GetDirectoryName(value);
												if (!string.IsNullOrEmpty(directoryName))
												{
													path = directoryName;
												}
											}
											string text3 = Path.Combine(inputDir, fileNameWithoutExtension, path, path2);
											text3 = text3.Replace(".\\", "");
											string path3;
											if (dictionary.ContainsKey(text3))
											{
												path3 = dictionary[text3];
											}
											else
											{
												path3 = text3;
											}
											FileStream inputStream = new FileStream(path3, FileMode.Open, FileAccess.Read);
											string text4 = Convert.ToBase64String(sha.ComputeHash(inputStream)).Replace("-", string.Empty);
											int length = text4.Length;
											xelement3.SetValue(text4);
										}
									}
								}
							}
							xdocument.Save(tempFileName);
							LongPathFile.Copy(tempFileName, text2, true);
						}
					}
				}
				foreach (string text5 in list)
				{
					Program.logger.LogInfo("PkgSignTool: Deleting temp manifest at {0}", new object[]
					{
						text5
					});
					File.Delete(text5);
				}
			}

			// Token: 0x06000015 RID: 21 RVA: 0x00002E9C File Offset: 0x0000109C
			protected override int DoExecution()
			{
				string parameterAsString = this._cmdLineParser.GetParameterAsString("input");
				bool switchAsBoolean = this._cmdLineParser.GetSwitchAsBoolean("incVersion");
				base.SetLoggingVerbosity(Program.logger);
				Program.UpdateCmdHandler.PkgUpdate(parameterAsString, switchAsBoolean);
				return 0;
			}

			// Token: 0x17000003 RID: 3
			// (get) Token: 0x06000016 RID: 22 RVA: 0x00002EDC File Offset: 0x000010DC
			public override string Command
			{
				get
				{
					return "Update";
				}
			}

			// Token: 0x17000004 RID: 4
			// (get) Token: 0x06000017 RID: 23 RVA: 0x00002EE3 File Offset: 0x000010E3
			public override string Description
			{
				get
				{
					return "Update component manifests in an unpacked package after it has been signed";
				}
			}

			// Token: 0x06000018 RID: 24 RVA: 0x00002EEA File Offset: 0x000010EA
			public UpdateCmdHandler()
			{
				this._cmdLineParser.SetRequiredParameterString("input", "path to the directory containing the unpacked package");
				this._cmdLineParser.SetOptionalSwitchBoolean("incVersion", "increases the version number of the package (default: false)", false);
				base.SetQuietCommand();
			}

			// Token: 0x04000011 RID: 17
			private const string EMPTY_FILE_HASH = "00000000000000000000000000000000000000000000";
		}

		// Token: 0x02000006 RID: 6
		private class RepackCmdHandler : QuietCmdHandler
		{
			// Token: 0x06000019 RID: 25 RVA: 0x00002F24 File Offset: 0x00001124
			private static void PkgRepack(string inputDir, string outputCab, CompressionType compressionType)
			{
				Dictionary<string, string> dictionary = Program.ReadFileMap(inputDir);
				if (!LongPathDirectory.Exists(inputDir))
				{
					throw new DirectoryNotFoundException(string.Format("The specified input directory '{0}' doesn't exist", inputDir));
				}
				string text = Path.Combine(inputDir, Program.c_OpaqueContainerCookieFileName);
				if (LongPathFile.Exists(text))
				{
					string[] array = LongPathFile.ReadAllLines(text);
					if (array.Length != 1)
					{
						throw new PackageException("Expected one line in {0}.  Found {1} lines.", new object[]
						{
							text,
							array.Length
						});
					}
					string fullPathUNC = LongPath.GetFullPathUNC(array[0]);
					string fullPathUNC2 = LongPath.GetFullPathUNC(outputCab);
					if (fullPathUNC.Equals(fullPathUNC2, StringComparison.InvariantCultureIgnoreCase))
					{
						Program.logger.LogInfo("PkgSignTool: Source from unpack matches target for repack for an opaque container - skipping repack.", new object[0]);
						return;
					}
					Program.logger.LogInfo("PkgSignTool: Copying opaque container source indicated in {0} as {1} to the repack target: {2}", new object[]
					{
						text,
						fullPathUNC,
						fullPathUNC2
					});
					LongPathFile.Copy(fullPathUNC, fullPathUNC2, true);
					return;
				}
				else
				{
					PackageStyle packageStyle = PackageStyle.SPKG;
					string text2 = Path.Combine(inputDir, PkgConstants.c_strDsmFile);
					if (!LongPathFile.Exists(text2))
					{
						text2 = Path.Combine(inputDir, PkgConstants.c_strMumFile);
						packageStyle = PackageStyle.CBS;
						if (!LongPathFile.Exists(text2))
						{
							throw new PackageException("The specified input directory '{0}' doesn't have man.dsm.xml or update.mum file", new object[]
							{
								inputDir
							});
						}
					}
					string path = Path.Combine(inputDir, "files.txt");
					if (!LongPathFile.Exists(path))
					{
						throw new PackageException("The specified input directory '{0}' doesn't have files.txt", new object[]
						{
							inputDir
						});
					}
					string[] array2 = LongPathFile.ReadAllLines(path);
					for (int i = 0; i < array2.Length; i++)
					{
						string[] array3 = array2[i].Split(new char[]
						{
							','
						});
						if (array3.Length != 3)
						{
							throw new PackageException("Incorrect file information '{0}' in files.txt", array3);
						}
						DateTime time = DateTime.Parse(array3[2]);
						LongPathFile.SetAttributes(array3[0], FileAttributes.Normal);
						NativeMethods.SetCreationTimeLong(array3[0], time);
					}
					foreach (string text3 in dictionary.Keys)
					{
						LongPathFile.Copy(dictionary[text3], text3, true);
					}
					CabArchiver cab = new CabArchiver();
					List<FileEntryBase> list = new List<FileEntryBase>();
					string text4 = Path.Combine(inputDir, PkgConstants.c_strDiffDsmFile);
					if (!LongPathFile.Exists(text4))
					{
						PkgManifest pkgManifest;
						if (packageStyle == PackageStyle.CBS)
						{
							pkgManifest = PkgManifest.Load_CBS(text2);
						}
						else
						{
							pkgManifest = PkgManifest.Load(text2);
						}
						if (outputCab == null)
						{
							outputCab = pkgManifest.Name + (pkgManifest.IsRemoval ? PkgConstants.c_strRemovalPkgExtension : PkgConstants.c_strPackageExtension);
						}
						pkgManifest.BuildSourcePaths(inputDir, BuildPathOption.UseCabPath);
						list.AddRange((from c in pkgManifest.Files
						group c by c.CabPath into f
						select f.First<FileEntry>()).ToList<FileEntry>());
					}
					else
					{
						DiffPkgManifest diffPkgManifest = DiffPkgManifest.Load(text4);
						if (outputCab == null)
						{
							outputCab = diffPkgManifest.Name + PkgConstants.c_strDiffPackageExtension;
						}
						diffPkgManifest.BuildSourcePath(inputDir, BuildPathOption.UseCabPath);
						list.AddRange((from x in diffPkgManifest.Files
						where x.DiffType != DiffType.Remove
						select x into c
						group c by c.CabPath into f
						select f.First<DiffFileEntry>()).ToList<DiffFileEntry>());
						cab.AddFile(PkgConstants.c_strDiffDsmFile, text4);
					}
					list.ForEach(delegate(FileEntryBase x)
					{
						cab.AddFile(x.CabPath, x.SourcePath);
					});
					cab.Save(outputCab, compressionType);
					return;
				}
			}

			// Token: 0x17000005 RID: 5
			// (get) Token: 0x0600001A RID: 26 RVA: 0x000032DC File Offset: 0x000014DC
			public override string Command
			{
				get
				{
					return "Repack";
				}
			}

			// Token: 0x17000006 RID: 6
			// (get) Token: 0x0600001B RID: 27 RVA: 0x000032E3 File Offset: 0x000014E3
			public override string Description
			{
				get
				{
					return "Repacking an unpacked package and save to the specified file";
				}
			}

			// Token: 0x0600001C RID: 28 RVA: 0x000032EC File Offset: 0x000014EC
			protected override int DoExecution()
			{
				string parameterAsString = this._cmdLineParser.GetParameterAsString("input");
				string outputCab = this._cmdLineParser.IsAssignedSwitch("out") ? this._cmdLineParser.GetSwitchAsString("out") : null;
				CompressionType compressionType = (CompressionType)Enum.Parse(typeof(CompressionType), this._cmdLineParser.GetSwitchAsString("compress"), true);
				base.SetLoggingVerbosity(Program.logger);
				Program.RepackCmdHandler.PkgRepack(parameterAsString, outputCab, compressionType);
				return 0;
			}

			// Token: 0x0600001D RID: 29 RVA: 0x00003368 File Offset: 0x00001568
			public RepackCmdHandler()
			{
				this._cmdLineParser.SetRequiredParameterString("input", "directory of the unpacked package");
				this._cmdLineParser.SetOptionalSwitchString("out", "path for the result package", ".\\<name from manifest>.spkg", new string[0]);
				this._cmdLineParser.SetOptionalSwitchString("compress", "compression type", CompressionType.LZX.ToString(), false, Enum.GetNames(typeof(CompressionType)));
				base.SetQuietCommand();
			}
		}
	}
}
