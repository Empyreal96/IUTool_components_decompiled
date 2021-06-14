using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Tools.IO;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000021 RID: 33
	internal class MetadataOnlyAssembly : Assembly, IAssembly2, IDisposable
	{
		// Token: 0x0600016E RID: 366 RVA: 0x00003B82 File Offset: 0x00001D82
		internal MetadataOnlyAssembly(MetadataOnlyModule manifestModule, string manifestFile) : this(new MetadataOnlyModule[]
		{
			manifestModule
		}, manifestFile)
		{
		}

		// Token: 0x0600016F RID: 367 RVA: 0x00003B98 File Offset: 0x00001D98
		internal MetadataOnlyAssembly(MetadataOnlyModule[] modules, string manifestFile)
		{
			MetadataOnlyAssembly.VerifyModules(modules);
			this._manifestModule = modules[0];
			this._name = AssemblyNameHelper.GetAssemblyName(this._manifestModule);
			this._manifestFile = manifestFile;
			foreach (MetadataOnlyModule metadataOnlyModule in modules)
			{
				metadataOnlyModule.SetContainingAssembly(this);
			}
			List<Module> list = new List<Module>(modules);
			bool getResources = false;
			List<string> fileNamesFromFilesTable = MetadataOnlyAssembly.GetFileNamesFromFilesTable(this._manifestModule, getResources);
			using (List<string>.Enumerator enumerator = fileNamesFromFilesTable.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string netModuleName = enumerator.Current;
					bool flag = list.Find((Module i) => i.Name.Equals(netModuleName, StringComparison.OrdinalIgnoreCase)) != null;
					if (!flag)
					{
						Module module = this._manifestModule.AssemblyResolver.ResolveModule(this, netModuleName);
						bool flag2 = module == null;
						if (flag2)
						{
							throw new InvalidOperationException(Resources.ResolverMustResolveToValidModule);
						}
						bool flag3 = module.Assembly != this;
						if (flag3)
						{
							throw new InvalidOperationException(Resources.ResolverMustSetAssemblyProperty);
						}
						list.Add(module);
					}
				}
			}
			this._modules = list.ToArray();
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00003CEC File Offset: 0x00001EEC
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00003D00 File Offset: 0x00001F00
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				bool flag = this._modules != null;
				if (flag)
				{
					foreach (Module module in this._modules)
					{
						IDisposable disposable = module as IDisposable;
						bool flag2 = disposable != null;
						if (flag2)
						{
							disposable.Dispose();
						}
					}
				}
			}
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00003D60 File Offset: 0x00001F60
		private static void VerifyModules(MetadataOnlyModule[] modules)
		{
			bool flag = modules == null || modules.Length < 1;
			if (flag)
			{
				throw new ArgumentException(Resources.ManifestModuleMustBeProvided);
			}
			bool flag2 = MetadataOnlyAssembly.GetAssemblyToken(modules[0]) == Token.Nil;
			if (flag2)
			{
				throw new ArgumentException(Resources.NoAssemblyManifest);
			}
			for (int i = 1; i < modules.Length; i++)
			{
				bool flag3 = MetadataOnlyAssembly.GetAssemblyToken(modules[i]) != Token.Nil;
				if (flag3)
				{
					throw new ArgumentException(Resources.ExtraAssemblyManifest);
				}
			}
		}

		// Token: 0x06000173 RID: 371 RVA: 0x00003DE4 File Offset: 0x00001FE4
		private static List<string> GetFileNamesFromFilesTable(MetadataOnlyModule manifestModule, bool getResources)
		{
			HCORENUM hcorenum = default(HCORENUM);
			List<string> list = new List<string>();
			StringBuilder stringBuilder = StringBuilderPool.Get();
			IMetadataAssemblyImport metadataAssemblyImport = (IMetadataAssemblyImport)manifestModule.RawImport;
			try
			{
				for (;;)
				{
					int token;
					int num;
					metadataAssemblyImport.EnumFiles(ref hcorenum, out token, 1, out num);
					bool flag = num == 0;
					if (flag)
					{
						break;
					}
					int capacity;
					UnusedIntPtr unusedIntPtr;
					uint num2;
					CorFileFlags corFileFlags;
					metadataAssemblyImport.GetFileProps(token, null, 0, out capacity, out unusedIntPtr, out num2, out corFileFlags);
					bool flag2 = !getResources;
					if (flag2)
					{
						bool flag3 = corFileFlags == CorFileFlags.ContainsNoMetaData;
						if (flag3)
						{
							continue;
						}
					}
					stringBuilder.Length = 0;
					stringBuilder.EnsureCapacity(capacity);
					metadataAssemblyImport.GetFileProps(token, stringBuilder, stringBuilder.Capacity, out capacity, out unusedIntPtr, out num2, out corFileFlags);
					list.Add(stringBuilder.ToString());
				}
			}
			finally
			{
				hcorenum.Close(metadataAssemblyImport);
			}
			StringBuilderPool.Release(ref stringBuilder);
			return list;
		}

		// Token: 0x06000174 RID: 372 RVA: 0x00003ED8 File Offset: 0x000020D8
		public override int GetHashCode()
		{
			return this._modules[0].GetHashCode();
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00003EF8 File Offset: 0x000020F8
		public override bool Equals(object obj)
		{
			Assembly assembly = obj as Assembly;
			bool flag = assembly == null;
			return !flag && this.ManifestModule.Equals(assembly.ManifestModule);
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00003F30 File Offset: 0x00002130
		public override Stream GetManifestResourceStream(Type type, string name)
		{
			StringBuilder stringBuilder = StringBuilderPool.Get();
			bool flag = type == null;
			if (flag)
			{
				bool flag2 = name == null;
				if (flag2)
				{
					throw new ArgumentNullException("type");
				}
			}
			else
			{
				string @namespace = type.Namespace;
				bool flag3 = @namespace != null;
				if (flag3)
				{
					stringBuilder.Append(@namespace);
					bool flag4 = name != null;
					if (flag4)
					{
						stringBuilder.Append(Type.Delimiter);
					}
				}
			}
			bool flag5 = name != null;
			if (flag5)
			{
				stringBuilder.Append(name);
			}
			string name2 = stringBuilder.ToString();
			StringBuilderPool.Release(ref stringBuilder);
			return this.GetManifestResourceStream(name2);
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00003FCC File Offset: 0x000021CC
		public override Stream GetManifestResourceStream(string name)
		{
			IMetadataAssemblyImport metadataAssemblyImport = (IMetadataAssemblyImport)this._manifestModule.RawImport;
			int num;
			metadataAssemblyImport.FindManifestResourceByName(name, out num);
			Token token = new Token(num);
			bool isNil = token.IsNil;
			Stream result;
			if (isNil)
			{
				result = null;
			}
			else
			{
				StringBuilder stringBuilder = StringBuilderPool.Get(name.Length + 1);
				int num2;
				int value;
				uint num3;
				CorManifestResourceFlags corManifestResourceFlags;
				metadataAssemblyImport.GetManifestResourceProps(num, stringBuilder, stringBuilder.Capacity, out num2, out value, out num3, out corManifestResourceFlags);
				StringBuilderPool.Release(ref stringBuilder);
				Token token2 = new Token(value);
				bool flag = token2.TokenType == TokenType.File;
				if (flag)
				{
					bool isNil2 = token2.IsNil;
					if (isNil2)
					{
						byte[] buffer = this._manifestModule.RawMetadata.ReadResource((long)((ulong)num3));
						result = new MemoryStream(buffer);
					}
					else
					{
						int capacity;
						UnusedIntPtr unusedIntPtr;
						uint num4;
						CorFileFlags corFileFlags;
						metadataAssemblyImport.GetFileProps(token2.Value, null, 0, out capacity, out unusedIntPtr, out num4, out corFileFlags);
						StringBuilder stringBuilder2 = StringBuilderPool.Get(capacity);
						metadataAssemblyImport.GetFileProps(token2.Value, stringBuilder2, stringBuilder2.Capacity, out capacity, out unusedIntPtr, out num4, out corFileFlags);
						string directoryName = LongPathPath.GetDirectoryName(this.Location);
						string path = LongPathPath.Combine(directoryName, stringBuilder2.ToString());
						StringBuilderPool.Release(ref stringBuilder2);
						result = new FileStream(path, FileMode.Open);
					}
				}
				else
				{
					bool flag2 = token2.TokenType == TokenType.AssemblyRef;
					if (flag2)
					{
						throw new NotImplementedException();
					}
					throw new ArgumentException(Resources.InvalidMetadata);
				}
			}
			return result;
		}

		// Token: 0x06000178 RID: 376 RVA: 0x0000412C File Offset: 0x0000232C
		public override string[] GetManifestResourceNames()
		{
			HCORENUM hcorenum = default(HCORENUM);
			List<string> list = new List<string>();
			StringBuilder stringBuilder = StringBuilderPool.Get();
			IMetadataAssemblyImport metadataAssemblyImport = (IMetadataAssemblyImport)this._manifestModule.RawImport;
			try
			{
				for (;;)
				{
					int mdmr;
					int num;
					metadataAssemblyImport.EnumManifestResources(ref hcorenum, out mdmr, 1, out num);
					bool flag = num == 0;
					if (flag)
					{
						break;
					}
					int capacity;
					int num2;
					uint num3;
					CorManifestResourceFlags corManifestResourceFlags;
					metadataAssemblyImport.GetManifestResourceProps(mdmr, null, 0, out capacity, out num2, out num3, out corManifestResourceFlags);
					stringBuilder.Length = 0;
					stringBuilder.EnsureCapacity(capacity);
					metadataAssemblyImport.GetManifestResourceProps(mdmr, stringBuilder, stringBuilder.Capacity, out capacity, out num2, out num3, out corManifestResourceFlags);
					list.Add(stringBuilder.ToString());
				}
			}
			finally
			{
				hcorenum.Close(metadataAssemblyImport);
			}
			StringBuilderPool.Release(ref stringBuilder);
			return list.ToArray();
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000420C File Offset: 0x0000240C
		public override AssemblyName GetName()
		{
			return this._name;
		}

		// Token: 0x0600017A RID: 378 RVA: 0x0000232A File Offset: 0x0000052A
		public override AssemblyName GetName(bool copiedName)
		{
			throw new NotImplementedException();
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x0600017B RID: 379 RVA: 0x00004224 File Offset: 0x00002424
		public override string FullName
		{
			get
			{
				return this._name.FullName;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x0600017C RID: 380 RVA: 0x00004244 File Offset: 0x00002444
		public override string Location
		{
			get
			{
				return this._manifestFile;
			}
		}

		// Token: 0x0600017D RID: 381 RVA: 0x0000425C File Offset: 0x0000245C
		public override Type[] GetExportedTypes()
		{
			Type[] types = this.GetTypes();
			List<Type> list = new List<Type>();
			foreach (Type type in types)
			{
				bool isVisible = type.IsVisible;
				if (isVisible)
				{
					list.Add(type);
				}
			}
			return list.ToArray();
		}

		// Token: 0x0600017E RID: 382 RVA: 0x000042B4 File Offset: 0x000024B4
		public override Type GetType(string name)
		{
			return this.GetType(name, false, false);
		}

		// Token: 0x0600017F RID: 383 RVA: 0x000042D0 File Offset: 0x000024D0
		public override Type GetType(string name, bool throwOnError)
		{
			return this.GetType(name, throwOnError, false);
		}

		// Token: 0x06000180 RID: 384 RVA: 0x000042EC File Offset: 0x000024EC
		public override Type GetType(string name, bool throwOnError, bool ignoreCase)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			for (int i = 0; i < this._modules.Length; i++)
			{
				Type type = this._modules[i].GetType(name, false, ignoreCase);
				bool flag2 = type != null;
				if (flag2)
				{
					return type;
				}
			}
			Type type2 = this._manifestModule.Policy.TryTypeForwardResolution(this, name, ignoreCase);
			bool flag3 = type2 != null;
			if (flag3)
			{
				return type2;
			}
			if (throwOnError)
			{
				throw new TypeLoadException(string.Format(CultureInfo.InvariantCulture, Resources.CannotFindTypeInModule, new object[]
				{
					name,
					this._modules[0].ScopeName
				}));
			}
			return null;
		}

		// Token: 0x06000181 RID: 385 RVA: 0x000043B0 File Offset: 0x000025B0
		public override Type[] GetTypes()
		{
			List<Type> list = new List<Type>();
			foreach (Module module in this._modules)
			{
				list.AddRange(module.GetTypes());
			}
			return list.ToArray();
		}

		// Token: 0x06000182 RID: 386 RVA: 0x000043FC File Offset: 0x000025FC
		public override Module GetModule(string name)
		{
			foreach (Module module in this._modules)
			{
				bool flag = module.ScopeName.Equals(name, StringComparison.OrdinalIgnoreCase);
				if (flag)
				{
					return module;
				}
			}
			return null;
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00004444 File Offset: 0x00002644
		public override Module[] GetModules(bool getResourceModules)
		{
			return this._modules;
		}

		// Token: 0x06000184 RID: 388 RVA: 0x0000445C File Offset: 0x0000265C
		public override Module[] GetLoadedModules(bool getResourceModules)
		{
			return this._modules;
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000185 RID: 389 RVA: 0x00004474 File Offset: 0x00002674
		public override Module ManifestModule
		{
			get
			{
				return this._modules[0];
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000186 RID: 390 RVA: 0x00004490 File Offset: 0x00002690
		internal MetadataOnlyModule ManifestModuleInternal
		{
			get
			{
				return this._manifestModule;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000187 RID: 391 RVA: 0x000044A8 File Offset: 0x000026A8
		public override string CodeBase
		{
			get
			{
				return MetadataOnlyAssembly.GetCodeBaseFromManifestModule(this._manifestModule);
			}
		}

		// Token: 0x06000188 RID: 392 RVA: 0x000044C8 File Offset: 0x000026C8
		internal static string GetCodeBaseFromManifestModule(MetadataOnlyModule manifestModule)
		{
			string text = manifestModule.FullyQualifiedName;
			bool flag = text.StartsWith("\\\\?\\");
			if (flag)
			{
				text = text.Substring("\\\\?\\".Length, text.Length - "\\\\?\\".Length);
			}
			bool flag2 = !Utility.IsValidPath(text);
			string result;
			if (flag2)
			{
				result = string.Empty;
			}
			else
			{
				try
				{
					result = new Uri(text).ToString();
				}
				catch (Exception ex)
				{
					throw;
				}
			}
			return result;
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000189 RID: 393 RVA: 0x0000454C File Offset: 0x0000274C
		public override MethodInfo EntryPoint
		{
			get
			{
				MetadataFile rawMetadata = this._manifestModule.RawMetadata;
				Token token = rawMetadata.ReadEntryPointToken();
				bool isNil = token.IsNil;
				MethodInfo result;
				if (isNil)
				{
					result = null;
				}
				else
				{
					TokenType tokenType = token.TokenType;
					if (tokenType == TokenType.FieldDef)
					{
						throw new NotImplementedException();
					}
					if (tokenType != TokenType.MethodDef)
					{
						throw new InvalidOperationException(Resources.InvalidMetadata);
					}
					MethodBase methodBase = this.ManifestModule.ResolveMethod(token.Value);
					result = (MethodInfo)methodBase;
				}
				return result;
			}
		}

		// Token: 0x0600018A RID: 394 RVA: 0x000045D0 File Offset: 0x000027D0
		internal static Token GetAssemblyToken(MetadataOnlyModule module)
		{
			int value;
			int assemblyFromScope = ((IMetadataAssemblyImport)module.RawImport).GetAssemblyFromScope(out value);
			bool flag = assemblyFromScope == 0;
			Token result;
			if (flag)
			{
				result = new Token(value);
			}
			else
			{
				result = Token.Nil;
			}
			return result;
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0000460C File Offset: 0x0000280C
		public override FileStream[] GetFiles(bool getResourceModules)
		{
			List<string> list = new List<string>();
			foreach (Module module in this._modules)
			{
				list.Add(module.FullyQualifiedName);
			}
			if (getResourceModules)
			{
				string directoryName = LongPathPath.GetDirectoryName(this._manifestFile);
				foreach (string path in MetadataOnlyAssembly.GetFileNamesFromFilesTable(this._manifestModule, true))
				{
					list.Add(LongPathPath.Combine(directoryName, path));
				}
			}
			return MetadataOnlyAssembly.ConvertFileNamesToStreams(list.ToArray());
		}

		// Token: 0x0600018C RID: 396 RVA: 0x000046CC File Offset: 0x000028CC
		public override FileStream GetFile(string name)
		{
			Module module = this.GetModule(name);
			bool flag = module == null;
			FileStream result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = new FileStream(module.FullyQualifiedName, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			return result;
		}

		// Token: 0x0600018D RID: 397 RVA: 0x00004700 File Offset: 0x00002900
		private static FileStream[] ConvertFileNamesToStreams(string[] filenames)
		{
			return Array.ConvertAll<string, FileStream>(filenames, (string n) => new FileStream(n, FileMode.Open, FileAccess.Read, FileShare.Read));
		}

		// Token: 0x0600018E RID: 398 RVA: 0x00004738 File Offset: 0x00002938
		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this._manifestModule.GetCustomAttributeData(MetadataOnlyAssembly.GetAssemblyToken(this._manifestModule));
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00004768 File Offset: 0x00002968
		public override AssemblyName[] GetReferencedAssemblies()
		{
			IMetadataAssemblyImport metadataAssemblyImport = (IMetadataAssemblyImport)this._manifestModule.RawImport;
			List<AssemblyName> list = new List<AssemblyName>();
			HCORENUM hcorenum = default(HCORENUM);
			try
			{
				for (;;)
				{
					Token assemblyRefToken;
					int num;
					int errorCode = metadataAssemblyImport.EnumAssemblyRefs(ref hcorenum, out assemblyRefToken, 1, out num);
					Marshal.ThrowExceptionForHR(errorCode);
					bool flag = num == 0;
					if (flag)
					{
						break;
					}
					AssemblyName assemblyNameFromRef = AssemblyNameHelper.GetAssemblyNameFromRef(assemblyRefToken, this._manifestModule, metadataAssemblyImport);
					list.Add(assemblyNameFromRef);
				}
			}
			finally
			{
				hcorenum.Close(metadataAssemblyImport);
			}
			return list.ToArray();
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000190 RID: 400 RVA: 0x00004808 File Offset: 0x00002A08
		public ITypeUniverse TypeUniverse
		{
			get
			{
				return this._manifestModule.AssemblyResolver;
			}
		}

		// Token: 0x04000055 RID: 85
		private readonly Module[] _modules;

		// Token: 0x04000056 RID: 86
		private readonly MetadataOnlyModule _manifestModule;

		// Token: 0x04000057 RID: 87
		private readonly string _manifestFile;

		// Token: 0x04000058 RID: 88
		private readonly AssemblyName _name;
	}
}
