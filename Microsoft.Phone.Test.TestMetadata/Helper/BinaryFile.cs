using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.MetadataReader;
using Microsoft.Tools.IO;

namespace Microsoft.Phone.Test.TestMetadata.Helper
{
	// Token: 0x02000010 RID: 16
	public static class BinaryFile
	{
		// Token: 0x0600003C RID: 60 RVA: 0x00002B78 File Offset: 0x00000D78
		public static List<PortableExecutableDependency> GetDependency(string fileName)
		{
			List<PortableExecutableDependency> list = new List<PortableExecutableDependency>();
			try
			{
				using (PortableExecutable portableExecutable = new PortableExecutable(fileName))
				{
					bool flag = !portableExecutable.IsPortableExecutableBinary;
					if (flag)
					{
						return list;
					}
					BinaryFile.GetNativeDependency(list, portableExecutable);
					bool flag2 = !portableExecutable.IsManaged;
					if (flag2)
					{
						return list;
					}
					BinaryFile.GetManangedDependency(portableExecutable, list);
				}
			}
			catch (Exception ex)
			{
				Log.Error("Unable to loading PE binary {0}", new object[]
				{
					fileName
				});
				Log.Error(ex.Message, new object[0]);
				bool flag3 = ex.InnerException != null;
				if (flag3)
				{
					Log.Error(ex.InnerException.Message, new object[0]);
				}
				Log.Error("{0}", new object[]
				{
					ex
				});
			}
			return list;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002C74 File Offset: 0x00000E74
		private static void GetNativeDependency(List<PortableExecutableDependency> dependencyList, PortableExecutable peFile)
		{
			dependencyList.AddRange(from import in peFile.Imports
			select new PortableExecutableDependency
			{
				Name = import.ToLowerInvariant(),
				Type = BinaryDependencyType.Native
			});
			dependencyList.AddRange(from delayLoadImport in peFile.DelayLoadImports
			select new PortableExecutableDependency
			{
				Name = delayLoadImport.ToLowerInvariant(),
				Type = BinaryDependencyType.DelayLoad
			});
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002CE4 File Offset: 0x00000EE4
		private static void GetManangedDependency(PortableExecutable peFile, List<PortableExecutableDependency> dependencyList)
		{
			using (DefaultUniverse defaultUniverse = new DefaultUniverse())
			{
				Assembly assembly = defaultUniverse.LoadAssemblyFromFile(peFile.FullFileName);
				AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
				dependencyList.AddRange(from assemblyName in referencedAssemblies
				select new PortableExecutableDependency
				{
					Name = assemblyName.Name.ToLowerInvariant() + ".dll",
					Type = BinaryDependencyType.Managed
				});
				BinaryFile.GetPlatformInvokeDependency(peFile, dependencyList);
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002D60 File Offset: 0x00000F60
		private static void GetPlatformInvokeDependency(PortableExecutable peFile, List<PortableExecutableDependency> dependencyList)
		{
			MetadataDispenser metadataDispenser = new MetadataDispenser();
			using (MetadataFile metadataFile = metadataDispenser.OpenFile(peFile.FullFileName))
			{
				IMetadataImport metadataImport = (IMetadataImport)Marshal.GetUniqueObjectForIUnknown(metadataFile.RawPtr);
				HCORENUM hcorenum = default(HCORENUM);
				int mur;
				uint num;
				metadataImport.EnumModuleRefs(ref hcorenum, out mur, 1, out num);
				while (num > 0U)
				{
					StringBuilder stringBuilder = new StringBuilder(1024);
					int num2;
					metadataImport.GetModuleRefProps(mur, stringBuilder, stringBuilder.Capacity, out num2);
					string text = stringBuilder.ToString();
					bool flag = string.IsNullOrEmpty(LongPathPath.GetExtension(text));
					if (flag)
					{
						text += ".dll";
					}
					PortableExecutableDependency item = new PortableExecutableDependency
					{
						Name = text.ToLowerInvariant(),
						Type = BinaryDependencyType.PlatfomInvoke
					};
					dependencyList.Add(item);
					metadataImport.EnumModuleRefs(ref hcorenum, out mur, 1, out num);
				}
			}
		}

		// Token: 0x04000077 RID: 119
		private const string DllExtension = ".dll";
	}
}
