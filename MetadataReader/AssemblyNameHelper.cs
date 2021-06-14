using System;
using System.Configuration.Assemblies;
using System.Reflection;
using System.Reflection.Adds;
using System.Text;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000009 RID: 9
	internal static class AssemblyNameHelper
	{
		// Token: 0x06000038 RID: 56 RVA: 0x0000269C File Offset: 0x0000089C
		public static AssemblyName GetAssemblyName(MetadataOnlyModule module)
		{
			Token assemblyToken = MetadataOnlyAssembly.GetAssemblyToken(module);
			IMetadataAssemblyImport assemblyImport = (IMetadataAssemblyImport)module.RawImport;
			AssemblyNameHelper.AssemblyNameFromDefitionBuilder assemblyNameFromDefitionBuilder = new AssemblyNameHelper.AssemblyNameFromDefitionBuilder(assemblyToken, module.RawMetadata, assemblyImport);
			AssemblyName assemblyName = assemblyNameFromDefitionBuilder.CalculateName();
			assemblyName.CodeBase = MetadataOnlyAssembly.GetCodeBaseFromManifestModule(module);
			bool flag = !AssemblyNameHelper.HasV1Metadata((IMetadataImport2)module.RawImport);
			if (flag)
			{
				PortableExecutableKinds pek;
				ImageFileMachine ifm;
				module.GetPEKind(out pek, out ifm);
				ProcessorArchitecture processorArchitecture = AssemblyNameHelper.CalculateProcArchIndex(pek, ifm, assemblyNameFromDefitionBuilder.AssemblyNameFlags);
				assemblyName.ProcessorArchitecture = processorArchitecture;
			}
			else
			{
				assemblyName.ProcessorArchitecture = ProcessorArchitecture.None;
			}
			return assemblyName;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002734 File Offset: 0x00000934
		public static bool HasV1Metadata(IMetadataImport2 assemblyImport)
		{
			int num;
			assemblyImport.GetVersionString(null, 0, out num);
			bool flag = num < 2;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				StringBuilder stringBuilder = StringBuilderPool.Get(num);
				assemblyImport.GetVersionString(stringBuilder, stringBuilder.Capacity, out num);
				bool flag2 = stringBuilder[1] == '1';
				StringBuilderPool.Release(ref stringBuilder);
				result = flag2;
			}
			return result;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002790 File Offset: 0x00000990
		public static AssemblyName GetAssemblyNameFromRef(Token assemblyRefToken, MetadataOnlyModule module, IMetadataAssemblyImport assemblyImport)
		{
			AssemblyNameHelper.AssemblyNameFromRefBuilder assemblyNameFromRefBuilder = new AssemblyNameHelper.AssemblyNameFromRefBuilder(assemblyRefToken, module.RawMetadata, assemblyImport);
			return assemblyNameFromRefBuilder.CalculateName();
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000027B8 File Offset: 0x000009B8
		private static ProcessorArchitecture CalculateProcArchIndex(PortableExecutableKinds pek, ImageFileMachine ifm, AssemblyNameFlags flags)
		{
			bool flag = (flags & (AssemblyNameFlags)240) == (AssemblyNameFlags)112;
			ProcessorArchitecture result;
			if (flag)
			{
				result = ProcessorArchitecture.None;
			}
			else
			{
				bool flag2 = (pek & PortableExecutableKinds.PE32Plus) == PortableExecutableKinds.PE32Plus;
				if (flag2)
				{
					bool flag3 = ifm == ImageFileMachine.I386;
					if (flag3)
					{
						bool flag4 = (pek & PortableExecutableKinds.ILOnly) == PortableExecutableKinds.ILOnly;
						if (flag4)
						{
							return ProcessorArchitecture.MSIL;
						}
					}
					else
					{
						bool flag5 = ifm == ImageFileMachine.IA64;
						if (flag5)
						{
							return ProcessorArchitecture.IA64;
						}
						bool flag6 = ifm == ImageFileMachine.AMD64;
						if (flag6)
						{
							return ProcessorArchitecture.Amd64;
						}
					}
				}
				else
				{
					bool flag7 = ifm == ImageFileMachine.I386;
					if (flag7)
					{
						bool flag8 = (pek & PortableExecutableKinds.Required32Bit) != PortableExecutableKinds.Required32Bit && (pek & PortableExecutableKinds.ILOnly) == PortableExecutableKinds.ILOnly;
						if (flag8)
						{
							return ProcessorArchitecture.MSIL;
						}
						return ProcessorArchitecture.X86;
					}
				}
				result = ProcessorArchitecture.None;
			}
			return result;
		}

		// Token: 0x04000003 RID: 3
		private const int ProcessorArchitectureMask = 240;

		// Token: 0x04000004 RID: 4
		private const int ReferenceAssembly = 112;

		// Token: 0x02000049 RID: 73
		private abstract class AssemblyNameBuilder : IDisposable
		{
			// Token: 0x060004AA RID: 1194 RVA: 0x0000F70A File Offset: 0x0000D90A
			protected AssemblyNameBuilder(MetadataFile storage, IMetadataAssemblyImport assemblyImport)
			{
				this._storage = storage;
				this.m_assemblyImport = assemblyImport;
			}

			// Token: 0x060004AB RID: 1195
			protected abstract void Fetch();

			// Token: 0x060004AC RID: 1196 RVA: 0x0000F724 File Offset: 0x0000D924
			public AssemblyName CalculateName()
			{
				AssemblyName assemblyName = new AssemblyName();
				this.m_metadata = default(AssemblyMetaData);
				this.m_metadata.Init();
				this.m_szName = null;
				this.m_chName = 0;
				this.Fetch();
				this.m_szName = new StringBuilder();
				this.m_szName.Capacity = this.m_chName;
				int countBytes = (int)(this.m_metadata.cbLocale * 2U);
				this.m_metadata.szLocale = new UnmanagedStringMemoryHandle(countBytes);
				this.m_metadata.ulProcessor = 0U;
				this.m_metadata.ulOS = 0U;
				this.Fetch();
				assemblyName.CultureInfo = this.m_metadata.Locale;
				byte[] array = this._storage.ReadEmbeddedBlob(this.m_publicKey, this.m_cbPublicKey);
				assemblyName.HashAlgorithm = (AssemblyHashAlgorithm)this.m_hashAlgId;
				assemblyName.Name = this.m_szName.ToString();
				assemblyName.Version = this.m_metadata.Version;
				assemblyName.Flags = this.m_flags;
				bool flag = (this.m_flags & AssemblyNameFlags.PublicKey) > AssemblyNameFlags.None;
				if (flag)
				{
					assemblyName.SetPublicKey(array);
				}
				else
				{
					assemblyName.SetPublicKeyToken(array);
				}
				return assemblyName;
			}

			// Token: 0x17000121 RID: 289
			// (get) Token: 0x060004AD RID: 1197 RVA: 0x0000F854 File Offset: 0x0000DA54
			public AssemblyNameFlags AssemblyNameFlags
			{
				get
				{
					return this.m_flags;
				}
			}

			// Token: 0x060004AE RID: 1198 RVA: 0x0000F86C File Offset: 0x0000DA6C
			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			// Token: 0x060004AF RID: 1199 RVA: 0x0000F880 File Offset: 0x0000DA80
			protected virtual void Dispose(bool disposing)
			{
				if (disposing)
				{
					this.m_metadata.szLocale.Dispose();
				}
			}

			// Token: 0x040000ED RID: 237
			private readonly MetadataFile _storage;

			// Token: 0x040000EE RID: 238
			protected readonly IMetadataAssemblyImport m_assemblyImport;

			// Token: 0x040000EF RID: 239
			protected EmbeddedBlobPointer m_publicKey;

			// Token: 0x040000F0 RID: 240
			protected int m_cbPublicKey;

			// Token: 0x040000F1 RID: 241
			protected int m_hashAlgId;

			// Token: 0x040000F2 RID: 242
			protected StringBuilder m_szName;

			// Token: 0x040000F3 RID: 243
			protected int m_chName;

			// Token: 0x040000F4 RID: 244
			protected AssemblyNameFlags m_flags;

			// Token: 0x040000F5 RID: 245
			protected AssemblyMetaData m_metadata;
		}

		// Token: 0x0200004A RID: 74
		private class AssemblyNameFromDefitionBuilder : AssemblyNameHelper.AssemblyNameBuilder
		{
			// Token: 0x060004B0 RID: 1200 RVA: 0x0000F8A6 File Offset: 0x0000DAA6
			public AssemblyNameFromDefitionBuilder(Token assemblyToken, MetadataFile storage, IMetadataAssemblyImport assemblyImport) : base(storage, assemblyImport)
			{
				this._assemblyToken = assemblyToken;
			}

			// Token: 0x060004B1 RID: 1201 RVA: 0x0000F8BC File Offset: 0x0000DABC
			protected override void Fetch()
			{
				this.m_assemblyImport.GetAssemblyProps(this._assemblyToken, out this.m_publicKey, out this.m_cbPublicKey, out this.m_hashAlgId, this.m_szName, this.m_chName, out this.m_chName, ref this.m_metadata, out this.m_flags);
			}

			// Token: 0x040000F6 RID: 246
			private readonly Token _assemblyToken;
		}

		// Token: 0x0200004B RID: 75
		private class AssemblyNameFromRefBuilder : AssemblyNameHelper.AssemblyNameBuilder
		{
			// Token: 0x060004B2 RID: 1202 RVA: 0x0000F90C File Offset: 0x0000DB0C
			public AssemblyNameFromRefBuilder(Token assemblyRefToken, MetadataFile storage, IMetadataAssemblyImport assemblyImport) : base(storage, assemblyImport)
			{
				bool flag = assemblyRefToken.TokenType != TokenType.AssemblyRef;
				if (flag)
				{
					throw new ArgumentException(Resources.AssemblyRefTokenExpected);
				}
				this._assemblyRefToken = assemblyRefToken;
			}

			// Token: 0x060004B3 RID: 1203 RVA: 0x0000F94C File Offset: 0x0000DB4C
			protected override void Fetch()
			{
				UnusedIntPtr unusedIntPtr;
				uint num;
				this.m_assemblyImport.GetAssemblyRefProps(this._assemblyRefToken, out this.m_publicKey, out this.m_cbPublicKey, this.m_szName, this.m_chName, out this.m_chName, ref this.m_metadata, out unusedIntPtr, out num, out this.m_flags);
			}

			// Token: 0x040000F7 RID: 247
			private readonly Token _assemblyRefToken;
		}
	}
}
