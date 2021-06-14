using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x0200001B RID: 27
	public class NewDepXmlGenerator
	{
		// Token: 0x06000132 RID: 306 RVA: 0x0000866C File Offset: 0x0000686C
		public NewDepXmlGenerator(string outputPath, PackageInfo pkgInfo, bool updateOrCreate, PackageLocator packageLocator)
		{
			bool flag = string.IsNullOrEmpty(outputPath);
			if (flag)
			{
				throw new ArgumentNullException("outputPath");
			}
			bool flag2 = pkgInfo == null;
			if (flag2)
			{
				throw new ArgumentNullException("pkgInfo");
			}
			bool flag3 = packageLocator == null;
			if (flag3)
			{
				throw new ArgumentNullException("packageLocator");
			}
			this.OutputPath = outputPath;
			this.pkgInfo = pkgInfo;
			this.updateOrCreate = updateOrCreate;
			this.packageLocator = packageLocator;
			this.binaryLocator = new BinaryLocator(packageLocator);
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000133 RID: 307 RVA: 0x00008706 File Offset: 0x00006906
		// (set) Token: 0x06000134 RID: 308 RVA: 0x0000870E File Offset: 0x0000690E
		public string OutputPath { get; private set; }

		// Token: 0x06000135 RID: 309 RVA: 0x00008718 File Offset: 0x00006918
		public string GetDepXml()
		{
			this.ResolveDependency();
			return this.SaveDepXml();
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00008738 File Offset: 0x00006938
		private void ResolveDependency()
		{
			string pkgName = this.pkgInfo.PackageName.ToLowerInvariant();
			this.pkgDepSet.Add(new PkgDepResolve
			{
				PkgInfo = this.pkgInfo,
				IsProcessed = false
			});
			this.resolvedDependencies = new HashSet<Dependency>
			{
				new PackageDependency
				{
					PkgName = pkgName,
					RelativePath = this.pkgInfo.RelativePath
				}
			};
			this.ResolveDependency(this.pkgDepSet);
		}

		// Token: 0x06000137 RID: 311 RVA: 0x000087C4 File Offset: 0x000069C4
		private void ResolveDependency(HashSet<PkgDepResolve> pkgsWorkingSet)
		{
			IEnumerable<PkgDepResolve> enumerable = from x in pkgsWorkingSet
			where !x.IsProcessed
			select x;
			bool flag = enumerable.Count<PkgDepResolve>() == 0;
			if (!flag)
			{
				HashSet<PkgDepResolve> hashSet = new HashSet<PkgDepResolve>();
				foreach (PkgDepResolve pkgDepResolve in enumerable)
				{
					pkgDepResolve.IsProcessed = true;
					string text = Path.ChangeExtension(pkgDepResolve.PkgInfo.AbsolutePath, Constants.ManifestFileExtension);
					bool flag2 = !ReliableFile.Exists(text, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs));
					if (!flag2)
					{
						PackageDescription packageDescription = BinaryLocator.ReadPackageDescriptionFromManifestFile(text, pkgDepResolve.PkgInfo.RootPath);
						bool flag3 = packageDescription == null;
						if (!flag3)
						{
							this.includedBinaries.UnionWith(packageDescription.Binaries);
							foreach (Dependency dependency in packageDescription.Dependencies)
							{
								bool flag4 = dependency is PackageDependency;
								if (flag4)
								{
									PackageInfo packageInfo = this.packageLocator.FindPackage((dependency as PackageDependency).PkgName);
									bool flag5 = packageInfo == null;
									if (flag5)
									{
										Logger.Error("The excplicitly dependent package {0} was not found.", new object[]
										{
											(dependency as PackageDependency).PkgName
										});
									}
									else
									{
										hashSet.Add(new PkgDepResolve
										{
											PkgInfo = packageInfo,
											IsProcessed = false
										});
										(dependency as PackageDependency).AbsolutePath = packageInfo.AbsolutePath;
										(dependency as PackageDependency).RelativePath = packageInfo.RelativePath;
										this.resolvedDependencies.Add(dependency);
									}
								}
								else
								{
									bool flag6 = dependency is BinaryDependency;
									if (flag6)
									{
										string text2 = (dependency as BinaryDependency).FileName.ToLowerInvariant();
										bool flag7 = !this.includedBinaries.Contains(text2);
										if (flag7)
										{
											PackageInfo packageInfo2 = this.binaryLocator.FindContainingPackage(text2);
											bool flag8 = packageInfo2 != null;
											if (flag8)
											{
												hashSet.Add(new PkgDepResolve
												{
													PkgInfo = packageInfo2,
													IsProcessed = false
												});
												PackageDependency item = new PackageDependency
												{
													PkgName = packageInfo2.PackageName,
													AbsolutePath = packageInfo2.AbsolutePath,
													RelativePath = packageInfo2.RelativePath
												};
												this.resolvedDependencies.Add(item);
											}
										}
									}
									else
									{
										bool flag9 = dependency is RemoteFileDependency || dependency is EnvironmentPathDependency;
										if (flag9)
										{
											this.resolvedDependencies.Add(dependency);
										}
									}
								}
							}
						}
					}
				}
				pkgsWorkingSet.UnionWith(hashSet);
				this.ResolveDependency(pkgsWorkingSet);
			}
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00008B00 File Offset: 0x00006D00
		private string SaveDepXml()
		{
			bool flag = !Directory.Exists(this.OutputPath);
			if (flag)
			{
				Directory.CreateDirectory(this.OutputPath);
			}
			string text = Path.Combine(this.OutputPath, this.pkgInfo.PackageName + Constants.DepFileExtension);
			ResolvedDependency.Save(text, this.resolvedDependencies);
			return text;
		}

		// Token: 0x04000077 RID: 119
		private bool updateOrCreate;

		// Token: 0x04000078 RID: 120
		private PackageInfo pkgInfo;

		// Token: 0x04000079 RID: 121
		private HashSet<PkgDepResolve> pkgDepSet = new HashSet<PkgDepResolve>();

		// Token: 0x0400007A RID: 122
		private HashSet<Dependency> resolvedDependencies;

		// Token: 0x0400007B RID: 123
		private PackageLocator packageLocator;

		// Token: 0x0400007C RID: 124
		private BinaryLocator binaryLocator;

		// Token: 0x0400007D RID: 125
		private HashSet<string> includedBinaries = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
	}
}
