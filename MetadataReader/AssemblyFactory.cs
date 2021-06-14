using System;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000007 RID: 7
	internal static class AssemblyFactory
	{
		// Token: 0x06000032 RID: 50 RVA: 0x00002564 File Offset: 0x00000764
		public static Assembly CreateAssembly(MetadataOnlyModule manifestModule, string manifestFile)
		{
			return new MetadataOnlyAssembly(manifestModule, manifestFile);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002580 File Offset: 0x00000780
		public static Assembly CreateAssembly(ITypeUniverse typeUniverse, MetadataFile metadataImport, string manifestFile)
		{
			return AssemblyFactory.CreateAssembly(typeUniverse, metadataImport, new DefaultFactory(), manifestFile);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000025A0 File Offset: 0x000007A0
		public static Assembly CreateAssembly(ITypeUniverse typeUniverse, MetadataFile metadataImport, IReflectionFactory factory, string manifestFile)
		{
			return AssemblyFactory.CreateAssembly(typeUniverse, metadataImport, null, factory, manifestFile, null);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000025C0 File Offset: 0x000007C0
		public static Assembly CreateAssembly(ITypeUniverse typeUniverse, MetadataFile manifestModuleImport, MetadataFile[] netModuleImports, string manifestFile, string[] netModuleFiles)
		{
			return AssemblyFactory.CreateAssembly(typeUniverse, manifestModuleImport, netModuleImports, new DefaultFactory(), manifestFile, netModuleFiles);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000025E4 File Offset: 0x000007E4
		public static Assembly CreateAssembly(ITypeUniverse typeUniverse, MetadataFile manifestModuleImport, MetadataFile[] netModuleImports, IReflectionFactory factory, string manifestFile, string[] netModuleFiles)
		{
			int num = 1;
			bool flag = netModuleImports != null;
			if (flag)
			{
				num += netModuleImports.Length;
			}
			MetadataOnlyModule[] array = new MetadataOnlyModule[num];
			MetadataOnlyModule metadataOnlyModule = new MetadataOnlyModule(typeUniverse, manifestModuleImport, factory, manifestFile);
			array[0] = metadataOnlyModule;
			bool flag2 = num > 1;
			if (flag2)
			{
				for (int i = 0; i < netModuleImports.Length; i++)
				{
					array[i + 1] = new MetadataOnlyModule(typeUniverse, netModuleImports[i], factory, netModuleFiles[i]);
				}
			}
			return new MetadataOnlyAssembly(array, manifestFile);
		}
	}
}
