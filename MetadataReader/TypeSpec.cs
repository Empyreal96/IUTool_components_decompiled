using System;
using System.Diagnostics;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000044 RID: 68
	[DebuggerDisplay("{TypeSpecToken}")]
	internal class TypeSpec : TypeProxy, ITypeSpec, ITypeSignatureBlob, ITypeProxy
	{
		// Token: 0x06000491 RID: 1169 RVA: 0x0000F301 File Offset: 0x0000D501
		public TypeSpec(MetadataOnlyModule module, Token typeSpecToken, Type[] typeArgs, Type[] methodArgs) : base(module)
		{
			this.TypeSpecToken = typeSpecToken;
			this._context = new GenericContext(typeArgs, methodArgs);
		}

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000492 RID: 1170 RVA: 0x0000F321 File Offset: 0x0000D521
		public Token TypeSpecToken { get; }

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06000493 RID: 1171 RVA: 0x0000F32C File Offset: 0x0000D52C
		public byte[] Blob
		{
			get
			{
				EmbeddedBlobPointer pointer;
				int countBytes;
				int typeSpecFromToken = this.m_resolver.RawImport.GetTypeSpecFromToken(this.TypeSpecToken, out pointer, out countBytes);
				return this.m_resolver.ReadEmbeddedBlob(pointer, countBytes);
			}
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06000494 RID: 1172 RVA: 0x0000F368 File Offset: 0x0000D568
		public Module DeclaringScope
		{
			get
			{
				return this.Resolver;
			}
		}

		// Token: 0x06000495 RID: 1173 RVA: 0x0000F380 File Offset: 0x0000D580
		protected override Type GetResolvedTypeWorker()
		{
			byte[] blob = this.Blob;
			int num = 0;
			return SignatureUtil.ExtractType(blob, ref num, this.Resolver, this._context);
		}

		// Token: 0x040000E8 RID: 232
		private readonly GenericContext _context;
	}
}
