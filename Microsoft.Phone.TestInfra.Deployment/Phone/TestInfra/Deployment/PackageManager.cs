using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Mobile;
using Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary;
using Microsoft.Tools.IO;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x0200002C RID: 44
	public class PackageManager
	{
		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060001EF RID: 495 RVA: 0x0000BA30 File Offset: 0x00009C30
		public HashSet<string> DeployedPackages
		{
			get
			{
				return this.deployedPackages;
			}
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000BA48 File Offset: 0x00009C48
		public PackageManager(PackageManagerConfiguration config)
		{
			this.ValidateConfig(config);
			this.config = config;
			this.cacheManager = new CacheManager(this.config.CachePath, new TimeSpan?(this.config.ExpiresIn));
			this.packageLocator = new PackageLocator(this.config.RootPaths, this.config.AlternateRootPaths);
			this.packageExtractionRoot = this.config.PackagesExtractionCachePath;
			this.packageExtractor = new PackageExtractor();
			this.useSymlinks = PackageManager.IsUserAdministrator();
			PathCleaner.RegisterForCleanup(from path in config.RootPaths
			select Path.Combine(this.packageExtractionRoot, this.ComputeBinaryRootPathHash(path)), this.config.ExpiresIn, TimeSpan.FromMinutes(5.0));
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060001F1 RID: 497 RVA: 0x0000BB3B File Offset: 0x00009D3B
		// (set) Token: 0x060001F2 RID: 498 RVA: 0x0000BB43 File Offset: 0x00009D43
		public int ErrorCount { get; private set; }

		// Token: 0x060001F3 RID: 499 RVA: 0x0000BB4C File Offset: 0x00009D4C
		public void DeployPackages(IEnumerable<string> packages)
		{
			bool flag = packages == null || !packages.Any<string>() || packages.Any(new Func<string, bool>(string.IsNullOrEmpty));
			if (flag)
			{
				throw new ArgumentNullException("packages");
			}
			this.UpdatePackagesInstallationInfo(packages);
			foreach (string package in packages)
			{
				this.DeployPackage(package);
			}
			PackageManager.UpdateLocatorCacheFiles();
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000BBD8 File Offset: 0x00009DD8
		private static void UpdateLocatorCacheFiles()
		{
			PackageLocator.UpdateGeneralPackageLocatorCacheFile();
			BinaryLocator.UpdateGeneralBinaryLocatorCacheFile();
			BaseLocator.CleanCacheFiles();
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000BBF0 File Offset: 0x00009DF0
		private static bool IsUserAdministrator()
		{
			bool result;
			try
			{
				WindowsIdentity current = WindowsIdentity.GetCurrent();
				WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
				result = windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
			}
			catch (UnauthorizedAccessException ex)
			{
				result = false;
			}
			catch (Exception ex2)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000BC50 File Offset: 0x00009E50
		private void ValidateConfig(PackageManagerConfiguration config)
		{
			bool flag = config == null;
			if (flag)
			{
				throw new ArgumentNullException("config");
			}
			bool flag2 = string.IsNullOrEmpty(config.OutputPath);
			if (flag2)
			{
				throw new ArgumentNullException("config", "Output path is not set");
			}
			bool flag3 = config.RootPaths == null || !config.RootPaths.Any<string>() || config.RootPaths.Any(new Func<string, bool>(string.IsNullOrEmpty));
			if (flag3)
			{
				throw new ArgumentNullException("config", "Root paths are not set");
			}
			bool flag4 = string.IsNullOrEmpty(config.CachePath);
			if (flag4)
			{
				throw new ArgumentNullException("config", "Cache path is not set");
			}
			bool flag5 = string.IsNullOrEmpty(config.PackagesExtractionCachePath);
			if (flag5)
			{
				throw new ArgumentNullException("config", "Packages extraction cache path is not set");
			}
			bool flag6 = config.Macros == null;
			if (flag6)
			{
				config.Macros = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			}
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000BD3C File Offset: 0x00009F3C
		private void DeployPackage(string package)
		{
			Logger.Info("Deploying package {0}", new object[]
			{
				package
			});
			try
			{
				this.DeployPackage(package, true);
			}
			catch (Exception ex)
			{
				Logger.Error("Unhandled exception: {0}", new object[]
				{
					ex
				});
				int errorCount = this.ErrorCount;
				this.ErrorCount = errorCount + 1;
			}
			Logger.Info("Finished deploying package {0}", new object[]
			{
				package
			});
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000BDBC File Offset: 0x00009FBC
		private string BuildDepXmlWithNewManifest(PackageInfo pkgInfo, bool updateOrCreate)
		{
			bool flag = pkgInfo == null;
			if (flag)
			{
				throw new ArgumentNullException("pkgInfo");
			}
			Logger.Debug("Build DepXml With New Manifest files which include dependencies info.", new object[0]);
			string text = Path.Combine("DependencyXml", Guid.NewGuid().GetHashCode().ToString());
			text = Path.Combine(this.config.CachePath, text);
			NewDepXmlGenerator newDepXmlGenerator = new NewDepXmlGenerator(text, pkgInfo, false, this.packageLocator);
			string depXml = newDepXmlGenerator.GetDepXml();
			PathCleaner.RegisterForCleanup(text, this.config.ExpiresIn, TimeSpan.FromMinutes(5.0));
			return depXml;
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000BE68 File Offset: 0x0000A068
		private static string GetDependencyPathString(PackageInfo pkgInfo, IEnumerable<string> rootPaths)
		{
			bool flag = pkgInfo == null;
			if (flag)
			{
				throw new ArgumentNullException("pkgInfo");
			}
			HashSet<string> hashSet = new HashSet<string>();
			string containingFolderPath = PathHelper.GetContainingFolderPath(pkgInfo.AbsolutePath, "Prebuilt");
			bool flag2 = !string.IsNullOrEmpty(containingFolderPath);
			if (flag2)
			{
				hashSet.Add(containingFolderPath.Trim().TrimEnd(new char[]
				{
					'\\'
				}));
			}
			else
			{
				Logger.Info("Unable to find the Prebuilt folder in the path {0}. Ignore this path in dependency calculation.", new object[]
				{
					pkgInfo.AbsolutePath
				});
			}
			string text = string.Empty;
			foreach (string text2 in rootPaths)
			{
				bool flag3 = PathHelper.GetPathType(text2) == PathType.PhoneBuildPath;
				if (!flag3)
				{
					text = PathHelper.GetContainingFolderPath(text2, "Prebuilt");
					string item = string.IsNullOrEmpty(text) ? text2.Trim().TrimEnd(new char[]
					{
						'\\'
					}) : text.Trim().TrimEnd(new char[]
					{
						'\\'
					});
					hashSet.Add(item);
				}
			}
			return string.Join(";", hashSet);
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000BFA4 File Offset: 0x0000A1A4
		private string GenerateTestMetaDataToolArgString(PackageInfo pkgInfo, bool updateOrCreate, string outputFolder)
		{
			bool flag = pkgInfo == null;
			if (flag)
			{
				throw new ArgumentNullException("pkgInfo");
			}
			string text = Path.Combine(Constants.AssemblyDirectory, Constants.SupressionFileName);
			string text2 = Path.Combine(outputFolder, pkgInfo.PackageName + "DepGenerate.log");
			string dependencyPathString = PackageManager.GetDependencyPathString(pkgInfo, this.config.RootPaths);
			string text3 = string.Format("pkgdep -f \"{0}\" -i \"{1}\" -o \"{2}\" -rl -rr -s \"{3}\" -p windows -p test -l \"{4}\" -q -n {5}", new object[]
			{
				pkgInfo.PackageName,
				dependencyPathString,
				outputFolder,
				text,
				text2,
				Constants.NumOfLoaders
			});
			if (updateOrCreate)
			{
				string winBuildPath = PathHelper.GetWinBuildPath(pkgInfo.AbsolutePath);
				bool flag2 = !string.IsNullOrEmpty(winBuildPath);
				if (flag2)
				{
					text3 += string.Format(" -Up {0}", winBuildPath);
				}
			}
			return text3;
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000C07C File Offset: 0x0000A27C
		private static string GenerateTestMetadataOutputFolder()
		{
			string text = Path.Combine(Environment.GetEnvironmentVariable("UserProfile"), "AppData\\Local\\Temp\\TestMetaData");
			text = Path.Combine(text, Guid.NewGuid().GetHashCode().ToString());
			Directory.CreateDirectory(text);
			return text;
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000C0D0 File Offset: 0x0000A2D0
		private string CallTestMetaDatToolToGenerateDepXml(PackageInfo pkgInfo, bool updateOrCreate)
		{
			string text = Path.Combine(Constants.AssemblyDirectory, "TestMetaDataTool.exe");
			bool flag = !File.Exists(text);
			if (flag)
			{
				throw new InvalidOperationException(string.Format("Failed to find File {0}.", text));
			}
			string text2 = PackageManager.GenerateTestMetadataOutputFolder();
			string text3 = this.GenerateTestMetaDataToolArgString(pkgInfo, updateOrCreate, text2);
			Logger.Info("Running {0} {1} to generate dep.xml.", new object[]
			{
				text,
				text3
			});
			TimeSpan timeout = TimeSpan.FromMinutes(30.0);
			ProcessLauncher processLauncher = new ProcessLauncher(text, text3, delegate(string m)
			{
				Logger.Error(m, new object[0]);
			}, delegate(string m)
			{
				Logger.Info(m, new object[0]);
			}, delegate(string m)
			{
				Logger.Info(m, new object[0]);
			})
			{
				TimeoutHandler = delegate(Process p)
				{
					throw new TimeoutException(string.Format("Process {0} did not exit in {1} minutes", p.StartInfo.FileName, timeout.Minutes));
				}
			};
			processLauncher.RunToExit(Convert.ToInt32(timeout.TotalMilliseconds, CultureInfo.InvariantCulture));
			bool flag2 = !processLauncher.Process.HasExited;
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("Error: Process {0} has not exited.", text));
			}
			bool flag3 = processLauncher.Process.ExitCode != 0;
			if (flag3)
			{
				throw new InvalidOperationException(string.Format("{0} return an error exit code {1}", text, processLauncher.Process.ExitCode));
			}
			string text4 = Path.Combine(Path.Combine(text2, "packagedep"), pkgInfo.PackageName + ".dep.xml");
			bool flag4 = !File.Exists(text4);
			if (flag4)
			{
				Logger.Error("TestMetaDataTool.exe returns, but the dep.xml file is missing. \n", new object[0]);
				throw new FileNotFoundException(text4);
			}
			Logger.Info("TestMetaDataTool.exe generated the dep.xml file.\n", new object[0]);
			return text4;
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000C2B8 File Offset: 0x0000A4B8
		private string CreateDepXmlUsingTestMetaDataTool(PackageInfo pkgInfo)
		{
			bool flag = pkgInfo == null;
			if (flag)
			{
				throw new ArgumentNullException("pkgInfo");
			}
			return this.CallTestMetaDatToolToGenerateDepXml(pkgInfo, false);
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000C2E8 File Offset: 0x0000A4E8
		private string UpdateDepXml(PackageInfo pkgInfo)
		{
			bool flag = pkgInfo == null;
			if (flag)
			{
				throw new ArgumentNullException("pkgInfo");
			}
			return this.CallTestMetaDatToolToGenerateDepXml(pkgInfo, true);
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000C318 File Offset: 0x0000A518
		private static bool ManifestFileIncludeDependencies(string manifestFile)
		{
			bool flag = string.IsNullOrEmpty(manifestFile);
			if (flag)
			{
				throw new ArgumentException("manifestFile");
			}
			bool flag2 = !ReliableFile.Exists(manifestFile, 3, PackageManager.DefaultRetryDelay);
			if (flag2)
			{
				throw new FileNotFoundException(manifestFile);
			}
			bool result = false;
			try
			{
				RetryHelper.Retry(delegate()
				{
					using (XmlTextReader xmlTextReader = new XmlTextReader(manifestFile))
					{
						xmlTextReader.MoveToContent();
						result = xmlTextReader.ReadToDescendant("Dependencies");
					}
				}, 3, PackageManager.DefaultRetryDelay);
			}
			catch (InvalidOperationException)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000C3BC File Offset: 0x0000A5BC
		private bool ManifestFileInNewFormat(string manifestFile)
		{
			bool flag = string.IsNullOrEmpty(manifestFile);
			if (flag)
			{
				throw new ArgumentException("manifestFile");
			}
			return PackageManager.ManifestFileIncludeDependencies(manifestFile);
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000C3EC File Offset: 0x0000A5EC
		private string GetDepXml(PackageInfo pkgInfo)
		{
			bool flag = pkgInfo == null;
			if (flag)
			{
				throw new ArgumentNullException("pkgInfo");
			}
			bool flag2 = !LongPathFile.Exists(pkgInfo.AbsolutePath);
			if (flag2)
			{
				throw new ArgumentException(string.Format("File {0} does not exist.", pkgInfo.AbsolutePath));
			}
			string text = Path.ChangeExtension(pkgInfo.AbsolutePath, ".dep.xml");
			string text2 = Path.ChangeExtension(pkgInfo.AbsolutePath, Constants.ManifestFileExtension);
			bool flag3 = File.Exists(text);
			string result;
			if (flag3)
			{
				bool flag4 = PathHelper.GetPathType(pkgInfo.AbsolutePath) == PathType.PhoneBuildPath;
				if (flag4)
				{
					Logger.Info("It is a test package from Phone Build share, using the existing dep.xml.", new object[0]);
					result = text;
				}
				else
				{
					bool flag5 = !string.IsNullOrEmpty(PathHelper.GetWinBuildPath(pkgInfo.AbsolutePath));
					if (flag5)
					{
						result = this.UpdateDepXml(pkgInfo);
					}
					else
					{
						result = text;
					}
				}
			}
			else
			{
				bool flag6 = File.Exists(text2);
				if (flag6)
				{
					bool flag7 = this.ManifestFileInNewFormat(text2);
					if (flag7)
					{
						result = this.BuildDepXmlWithNewManifest(pkgInfo, true);
					}
					else
					{
						result = this.CreateDepXmlUsingTestMetaDataTool(pkgInfo);
					}
				}
				else
				{
					result = string.Empty;
				}
			}
			return result;
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000C4FC File Offset: 0x0000A6FC
		private void DeployPackage(string package, bool deployDependencies)
		{
			IEnumerable<string> tagsForPackage = this.GetTagsForPackage(package);
			string text = package;
			package = PathHelper.GetPackageNameWithoutExtension(package);
			package = this.NormalizePackageName(package);
			bool flag = string.IsNullOrEmpty(package);
			if (flag)
			{
				Logger.Warning("Skipping package {0} as the name is empty after normalization.", new object[]
				{
					text
				});
			}
			else
			{
				bool flag2 = this.deployedPackages.Contains(package);
				if (flag2)
				{
					Logger.Info("{0} is already deployed, skipping", new object[]
					{
						package
					});
				}
				else
				{
					this.deployedPackages.Add(package);
					PackageInfo packageInfo = this.packageLocator.FindPackage(package);
					bool flag3 = packageInfo == null;
					if (flag3)
					{
						Logger.Error("Unable to find package {0}", new object[]
						{
							package
						});
						int errorCount = this.ErrorCount;
						this.ErrorCount = errorCount + 1;
					}
					else
					{
						string cachedPackageFile = null;
						string cachedDependencyFile = null;
						try
						{
							Logger.Debug("Adding package to cache: {0}", new object[]
							{
								packageInfo.PackageName
							});
							this.cacheManager.AddFileToCache(packageInfo.AbsolutePath, delegate(string sourcePackage, string cachedPackage)
							{
								cachedPackageFile = cachedPackage;
							});
						}
						catch (Exception ex)
						{
							Logger.Error("Unable to cache package {0}: {1}", new object[]
							{
								packageInfo.AbsolutePath,
								ex
							});
							int errorCount = this.ErrorCount;
							this.ErrorCount = errorCount + 1;
							return;
						}
						if (deployDependencies)
						{
							try
							{
								string depXml = this.GetDepXml(packageInfo);
								Logger.Debug("Adding dependency XML to cache", new object[0]);
								bool flag4 = File.Exists(depXml);
								if (flag4)
								{
									this.cacheManager.AddFileToCache(depXml, delegate(string sourceDependency, string cachedDependency)
									{
										cachedDependencyFile = cachedDependency;
									});
								}
							}
							catch (Exception ex2)
							{
								Logger.Warning("Error in getting dependency file for package: {0}. Error: {1}. Ignored.", new object[]
								{
									package,
									ex2.ToString()
								});
							}
						}
						try
						{
							string text2 = Path.Combine(this.config.OutputPath, "files");
							string packageExtractionRootPath = Path.Combine(this.packageExtractionRoot, this.ComputeBinaryRootPathHash(packageInfo.RootPath));
							bool newPackage = false;
							string source = RetryHelper.Retry<string>(() => this.packageExtractor.ExtractPackage(cachedPackageFile, packageExtractionRootPath, out newPackage), 3, PackageManager.DefaultRetryDelay);
							this.CopyFilesToOutput(source, text2, newPackage);
							this.MoveCustomFiles(package, text2);
							bool flag5 = !string.IsNullOrEmpty(cachedDependencyFile);
							if (flag5)
							{
								string path = Path.Combine(this.config.OutputPath, "files\\data\\test\\metadata");
								this.CopyFileToOutput(cachedDependencyFile, Path.Combine(path, Path.GetFileName(cachedDependencyFile)), true);
							}
							Logger.Info("Deployed {0}", new object[]
							{
								package
							});
						}
						catch (Exception ex3)
						{
							Logger.Error("Unable to extract package {0}: {1}", new object[]
							{
								packageInfo.AbsolutePath,
								ex3
							});
							int errorCount = this.ErrorCount;
							this.ErrorCount = errorCount + 1;
							return;
						}
						bool flag6 = this.config.RecursiveDeployment || deployDependencies;
						if (flag6)
						{
							bool flag7 = !string.IsNullOrEmpty(cachedDependencyFile) && File.Exists(cachedDependencyFile);
							if (flag7)
							{
								XmlDocument xmlDocument = new XmlDocument();
								xmlDocument.Load(cachedDependencyFile);
								this.ParseDependencyXml(xmlDocument.FirstChild, tagsForPackage);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000C888 File Offset: 0x0000AA88
		private void ParseDependencyXml(XmlNode node, IEnumerable<string> tags)
		{
			do
			{
				bool flag = !node.HasChildNodes;
				if (!flag)
				{
					bool flag2 = !string.Equals(node.Name, "Required", StringComparison.OrdinalIgnoreCase);
					if (flag2)
					{
						this.ParseDependencyXml(node.FirstChild, tags);
					}
					else
					{
						foreach (object obj in node.ChildNodes)
						{
							XmlNode xmlNode = (XmlNode)obj;
							bool flag3 = string.Equals(xmlNode.Name, "EnvironmentPath", StringComparison.OrdinalIgnoreCase);
							if (!flag3)
							{
								bool flag4 = string.Equals(xmlNode.Name, "Package", StringComparison.OrdinalIgnoreCase);
								if (flag4)
								{
									bool flag5 = xmlNode.Attributes != null;
									if (flag5)
									{
										foreach (object obj2 in xmlNode.Attributes)
										{
											XmlAttribute xmlAttribute = (XmlAttribute)obj2;
											bool flag6 = string.Equals(xmlAttribute.Name, "Name", StringComparison.OrdinalIgnoreCase);
											if (flag6)
											{
												this.DeployPackage(xmlAttribute.Value, false);
												break;
											}
											Logger.Error("Unexpected XML Attribute {0} found in Package node", new object[]
											{
												xmlAttribute.Name
											});
											int errorCount = this.ErrorCount;
											this.ErrorCount = errorCount + 1;
										}
									}
									else
									{
										Logger.Error("No attributes found for Package node", new object[0]);
										int errorCount = this.ErrorCount;
										this.ErrorCount = errorCount + 1;
									}
								}
								else
								{
									bool flag7 = string.Equals(xmlNode.Name, "RemoteFile", StringComparison.OrdinalIgnoreCase);
									if (flag7)
									{
										bool flag8 = xmlNode.Attributes == null;
										if (flag8)
										{
											Logger.Error("RemoteFile attributes not found", new object[0]);
											int errorCount = this.ErrorCount;
											this.ErrorCount = errorCount + 1;
										}
										else
										{
											string text = null;
											string text2 = null;
											string text3 = null;
											string text4 = null;
											List<string> itemTags = new List<string>();
											foreach (object obj3 in xmlNode.Attributes)
											{
												XmlAttribute xmlAttribute2 = (XmlAttribute)obj3;
												bool flag9 = string.Equals(xmlAttribute2.Name, "SourcePath", StringComparison.OrdinalIgnoreCase);
												if (flag9)
												{
													text = this.MacroReplace(xmlAttribute2.Value, true);
												}
												else
												{
													bool flag10 = string.Equals(xmlAttribute2.Name, "Source", StringComparison.OrdinalIgnoreCase);
													if (flag10)
													{
														text2 = this.MacroReplace(xmlAttribute2.Value, false);
													}
													else
													{
														bool flag11 = string.Equals(xmlAttribute2.Name, "DestinationPath", StringComparison.OrdinalIgnoreCase);
														if (flag11)
														{
															text3 = this.MacroReplace(xmlAttribute2.Value, true);
														}
														else
														{
															bool flag12 = string.Equals(xmlAttribute2.Name, "Destination", StringComparison.OrdinalIgnoreCase);
															if (flag12)
															{
																text4 = this.MacroReplace(xmlAttribute2.Value, false);
															}
															else
															{
																bool flag13 = string.Equals(xmlAttribute2.Name, "Tags", StringComparison.OrdinalIgnoreCase);
																if (flag13)
																{
																	itemTags = new List<string>(xmlAttribute2.Value.Split(new char[]
																	{
																		','
																	}, StringSplitOptions.RemoveEmptyEntries));
																}
																else
																{
																	Logger.Error("Unexpected XML Attribute {0} found in RemoteFile node", new object[]
																	{
																		xmlAttribute2.Name
																	});
																	int errorCount = this.ErrorCount;
																	this.ErrorCount = errorCount + 1;
																}
															}
														}
													}
												}
											}
											bool flag14 = string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2) || string.IsNullOrEmpty(text3) || string.IsNullOrEmpty(text4);
											if (flag14)
											{
												Logger.Error("Missing attribute for RemoteFile", new object[0]);
												int errorCount = this.ErrorCount;
												this.ErrorCount = errorCount + 1;
												bool flag15 = string.IsNullOrEmpty(text);
												if (flag15)
												{
													Logger.Error("Missing SourcePath", new object[0]);
												}
												bool flag16 = string.IsNullOrEmpty(text2);
												if (flag16)
												{
													Logger.Error("Missing Source", new object[0]);
												}
												bool flag17 = string.IsNullOrEmpty(text3);
												if (flag17)
												{
													Logger.Error("Missing DestinationPath", new object[0]);
												}
												bool flag18 = string.IsNullOrEmpty(text4);
												if (flag18)
												{
													Logger.Error("Missing Destination", new object[0]);
												}
											}
											else
											{
												bool flag19 = itemTags.Any<string>() && !tags.Any((string tag) => itemTags.Contains(tag, StringComparer.OrdinalIgnoreCase));
												if (!flag19)
												{
													this.CopyRemoteFiles(text, text2, text3, text4);
												}
											}
										}
									}
									else
									{
										Logger.Error("Unexpected XML tag {0}", new object[]
										{
											xmlNode.Name
										});
										int errorCount = this.ErrorCount;
										this.ErrorCount = errorCount + 1;
									}
								}
							}
						}
					}
				}
			}
			while ((node = node.NextSibling) != null);
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000CDB8 File Offset: 0x0000AFB8
		private void CustomFileHelper(string searchPath, string fromPath, string toPath, string ext)
		{
			string path = Path.Combine(fromPath, searchPath);
			bool flag = !ReliableDirectory.Exists(path, 3, PackageManager.DefaultRetryDelay);
			if (!flag)
			{
				foreach (string text in Directory.EnumerateFiles(path, "*" + ext, SearchOption.TopDirectoryOnly))
				{
					string text2 = PathHelper.Combine(fromPath, toPath);
					Logger.Debug("{0} Directory = {1}", new object[]
					{
						ext,
						text2
					});
					Directory.CreateDirectory(text2);
					string text3 = PathHelper.Combine(text2, Path.GetFileName(text));
					try
					{
						File.Delete(text3);
						File.Move(text, text3);
						Logger.Debug("Moved {0} to {1}", new object[]
						{
							text,
							text3
						});
					}
					catch (IOException ex)
					{
						Logger.Warning("Unable to move {0} file {1}: {2}", new object[]
						{
							ext,
							text,
							ex.ToString()
						});
					}
				}
			}
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000CED8 File Offset: 0x0000B0D8
		private void MoveCustomFiles(string package, string outputPath)
		{
			this.CustomFileHelper("data\\Windows\\System32\\catroot\\{F750E6C3-38EE-11D1-85E5-00C04FC295EE}", outputPath, "Windows\\System32\\catroot\\{F750E6C3-38EE-11D1-85E5-00C04FC295EE}", ".cat");
			this.CustomFileHelper("windows\\packages\\DsmFiles", outputPath, "data\\test\\packages\\DsmFiles", ".dsm.xml");
			this.CustomFileHelper("data\\windows\\packages\\DsmFiles", outputPath, "data\\test\\packages\\DsmFiles", ".dsm.xml");
			this.CustomFileHelper("windows\\packages\\RegistryFiles", outputPath, "data\\test\\packages\\RegistryFiles", ".reg");
			this.CustomFileHelper("windows\\packages\\RegistryFiles", outputPath, "data\\test\\packages\\RegistryFiles", ".rga");
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000CF5C File Offset: 0x0000B15C
		private void CopyRemoteFiles(string sourcePath, string source, string destinationPath, string destination)
		{
			string remoteFileSource = PathHelper.Combine(sourcePath, source);
			string remoteFileDestination = PathHelper.Combine(PathHelper.Combine(Path.Combine(this.config.OutputPath, "files"), destinationPath), destination);
			try
			{
				bool flag = ReliableDirectory.Exists(remoteFileSource, 3, PackageManager.DefaultRetryDelay);
				if (flag)
				{
					this.cacheManager.AddFilesToCache(remoteFileSource, "*", true, delegate(string sourceFile, string cachedFile)
					{
						this.CopyFileToOutput(cachedFile, PathHelper.ChangeParent(sourceFile, remoteFileSource, remoteFileDestination), true);
					});
				}
				else
				{
					this.cacheManager.AddFileToCache(remoteFileSource, delegate(string sourceFile, string cachedFile)
					{
						this.CopyFileToOutput(cachedFile, remoteFileDestination, true);
					});
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Unable to copy remote files from {0}: {1}", new object[]
				{
					remoteFileSource,
					ex
				});
				int errorCount = this.ErrorCount;
				this.ErrorCount = errorCount + 1;
			}
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000D054 File Offset: 0x0000B254
		private void CopyFileToOutput(string sourceFile, string targetFile, bool overwrite)
		{
			bool flag = this.useSymlinks;
			if (flag)
			{
				try
				{
					RetryHelper.Retry(delegate()
					{
						SymlinkHelper.CreateSymlink(sourceFile, targetFile, overwrite);
					}, 3, PackageManager.DefaultRetryDelay);
				}
				catch (Exception ex)
				{
					Logger.Error("Unable to create symbolic link, falling back to File.Copy: {0}", new object[]
					{
						ex
					});
					this.useSymlinks = false;
				}
			}
			bool flag2 = !this.useSymlinks;
			if (flag2)
			{
				FileCopyHelper.CopyFile(sourceFile, targetFile, 5, TimeSpan.FromMilliseconds(200.0));
			}
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000D10C File Offset: 0x0000B30C
		private void CopyFilesToOutput(string source, string destination, bool overwrite)
		{
			bool flag = this.useSymlinks;
			if (flag)
			{
				try
				{
					RetryHelper.Retry(delegate()
					{
						SymlinkHelper.CreateSymlinks(source, destination, overwrite);
					}, 3, PackageManager.DefaultRetryDelay);
				}
				catch (Exception ex)
				{
					Logger.Error("Unable to create symbolic links, falling back to File.Copy: {0}", new object[]
					{
						ex
					});
					this.useSymlinks = false;
				}
			}
			bool flag2 = !this.useSymlinks;
			if (flag2)
			{
				FileCopyHelper.CopyFiles(source, destination, "*", true, 3, PackageManager.DefaultRetryDelay);
			}
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000D1C4 File Offset: 0x0000B3C4
		private void UpdatePackagesInstallationInfo(IEnumerable<string> packages)
		{
			IEnumerable<string> contents = packages.Select(new Func<string, string>(this.NormalizePackageName));
			string text = Path.Combine(this.config.OutputPath, "files\\data\\test\\metadata");
			Directory.CreateDirectory(text);
			File.WriteAllLines(Path.Combine(text, "currentpkg.txt"), contents);
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000D214 File Offset: 0x0000B414
		private string NormalizePackageName(string package)
		{
			bool flag = string.IsNullOrEmpty(package);
			if (flag)
			{
				throw new ArgumentNullException("package");
			}
			string fileNameWithoutExtension = PathHelper.GetFileNameWithoutExtension(package.Trim().ToLowerInvariant(), ".spkg");
			int num = fileNameWithoutExtension.IndexOf("[", StringComparison.OrdinalIgnoreCase);
			return (num == -1) ? fileNameWithoutExtension : fileNameWithoutExtension.Substring(0, num);
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000D270 File Offset: 0x0000B470
		private IEnumerable<string> GetTagsForPackage(string package)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			Match match = Regex.Match(package, ".+?\\[tags=(.+?)\\]", RegexOptions.IgnoreCase);
			bool success = match.Success;
			if (success)
			{
				foreach (string item in match.Groups[1].Value.Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries))
				{
					hashSet.Add(item);
				}
				Logger.Debug("Tags:", new object[0]);
				foreach (string text in hashSet)
				{
					Logger.Debug("  {0}", new object[]
					{
						text
					});
				}
			}
			return hashSet;
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0000D35C File Offset: 0x0000B55C
		private string MacroReplace(string str, bool fullStringMacro)
		{
			string text = str;
			bool flag = fullStringMacro && !text.StartsWith("$(", StringComparison.OrdinalIgnoreCase);
			if (flag)
			{
				text = "$(" + text + ")";
			}
			foreach (KeyValuePair<string, string> keyValuePair in this.config.Macros)
			{
				string pattern = string.Format("\\$\\({0}\\)", keyValuePair.Key);
				text = Regex.Replace(text, pattern, keyValuePair.Value, RegexOptions.IgnoreCase);
			}
			bool flag2 = text.IndexOf("$(", StringComparison.OrdinalIgnoreCase) != -1;
			if (flag2)
			{
				Logger.Error("Unknown Macro referenced in string {0}", new object[]
				{
					str
				});
				int errorCount = this.ErrorCount;
				this.ErrorCount = errorCount + 1;
			}
			bool flag3 = !string.Equals(str, text, StringComparison.OrdinalIgnoreCase);
			if (flag3)
			{
				Logger.Debug("MacroReplace: {0} => {1}", new object[]
				{
					str,
					text
				});
			}
			return text;
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000D474 File Offset: 0x0000B674
		private string ComputeBinaryRootPathHash(string path)
		{
			return PathHelper.EndWithDirectorySeparator(path).ToUpperInvariant().GetHashCode().ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x040000D2 RID: 210
		private const string FilesDir = "files";

		// Token: 0x040000D3 RID: 211
		private const string OrigCatPath = "Windows\\System32\\catroot\\{F750E6C3-38EE-11D1-85E5-00C04FC295EE}";

		// Token: 0x040000D4 RID: 212
		private const string OrigDsmPath = "windows\\packages\\DsmFiles";

		// Token: 0x040000D5 RID: 213
		private const string DataDsmPath = "data\\windows\\packages\\DsmFiles";

		// Token: 0x040000D6 RID: 214
		private const string OrigRegistryPath = "windows\\packages\\RegistryFiles";

		// Token: 0x040000D7 RID: 215
		private const string DataCatPath = "data\\Windows\\System32\\catroot\\{F750E6C3-38EE-11D1-85E5-00C04FC295EE}";

		// Token: 0x040000D8 RID: 216
		private const string DsmPath = "data\\test\\packages\\DsmFiles";

		// Token: 0x040000D9 RID: 217
		private const string RegistryPath = "data\\test\\packages\\RegistryFiles";

		// Token: 0x040000DA RID: 218
		private const string MetadataPath = "files\\data\\test\\metadata";

		// Token: 0x040000DB RID: 219
		private const int DefaultRetryCount = 3;

		// Token: 0x040000DC RID: 220
		private static readonly TimeSpan DefaultRetryDelay = TimeSpan.FromMilliseconds(300.0);

		// Token: 0x040000DD RID: 221
		private readonly PackageManagerConfiguration config;

		// Token: 0x040000DE RID: 222
		private readonly PackageLocator packageLocator;

		// Token: 0x040000DF RID: 223
		private readonly CacheManager cacheManager;

		// Token: 0x040000E0 RID: 224
		private readonly PackageExtractor packageExtractor;

		// Token: 0x040000E1 RID: 225
		private readonly string packageExtractionRoot;

		// Token: 0x040000E2 RID: 226
		private readonly HashSet<string> deployedPackages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x040000E3 RID: 227
		private volatile bool useSymlinks = true;

		// Token: 0x040000E4 RID: 228
		private HashSet<string> newManifestFormatRootPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
	}
}
