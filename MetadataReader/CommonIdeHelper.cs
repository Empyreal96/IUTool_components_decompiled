using System;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000008 RID: 8
	internal static class CommonIdeHelper
	{
		// Token: 0x06000037 RID: 55 RVA: 0x00002668 File Offset: 0x00000868
		public static AssemblyName GetNameFromPath(string path)
		{
			CommonIdeHelper.EmptyUniverse typeUniverse = new CommonIdeHelper.EmptyUniverse();
			MetadataFile metadataImport = new MetadataDispenser().OpenFile(path);
			Assembly assembly = AssemblyFactory.CreateAssembly(typeUniverse, metadataImport, path);
			return assembly.GetName();
		}

		// Token: 0x02000048 RID: 72
		private class EmptyUniverse : ITypeUniverse
		{
			// Token: 0x060004A3 RID: 1187 RVA: 0x0000232A File Offset: 0x0000052A
			public Type GetBuiltInType(CorElementType elementType)
			{
				throw new NotImplementedException();
			}

			// Token: 0x060004A4 RID: 1188 RVA: 0x0000232A File Offset: 0x0000052A
			public Type GetTypeXFromName(string fullName)
			{
				throw new NotImplementedException();
			}

			// Token: 0x060004A5 RID: 1189 RVA: 0x0000232A File Offset: 0x0000052A
			public Assembly GetSystemAssembly()
			{
				throw new NotImplementedException();
			}

			// Token: 0x060004A6 RID: 1190 RVA: 0x0000232A File Offset: 0x0000052A
			public Assembly ResolveAssembly(AssemblyName name)
			{
				throw new NotImplementedException();
			}

			// Token: 0x060004A7 RID: 1191 RVA: 0x0000232A File Offset: 0x0000052A
			public Assembly ResolveAssembly(Module scope, Token tokenAssemblyRef)
			{
				throw new NotImplementedException();
			}

			// Token: 0x060004A8 RID: 1192 RVA: 0x0000232A File Offset: 0x0000052A
			public Module ResolveModule(Assembly containingAssembly, string moduleName)
			{
				throw new NotImplementedException();
			}
		}
	}
}
