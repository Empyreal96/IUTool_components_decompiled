using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000028 RID: 40
	internal class MetadataOnlyMethodBody : MethodBody
	{
		// Token: 0x0600020E RID: 526 RVA: 0x00005E16 File Offset: 0x00004016
		protected MetadataOnlyMethodBody(MetadataOnlyMethodInfo method)
		{
			this.Method = method;
		}

		// Token: 0x0600020F RID: 527 RVA: 0x00005E28 File Offset: 0x00004028
		internal static MethodBody TryCreate(MetadataOnlyMethodInfo method)
		{
			MetadataOnlyModule resolver = method.Resolver;
			MethodBody methodBody = null;
			bool flag = resolver.Factory.TryCreateMethodBody(method, ref methodBody);
			MethodBody result;
			if (flag)
			{
				result = methodBody;
			}
			else
			{
				result = MetadataOnlyMethodBodyWorker.Create(method);
			}
			return result;
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000210 RID: 528 RVA: 0x00005E60 File Offset: 0x00004060
		protected MetadataOnlyMethodInfo Method { get; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000211 RID: 529 RVA: 0x0000232A File Offset: 0x0000052A
		public override IList<ExceptionHandlingClause> ExceptionHandlingClauses
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000212 RID: 530 RVA: 0x00002390 File Offset: 0x00000590
		public override bool InitLocals
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000213 RID: 531 RVA: 0x00002390 File Offset: 0x00000590
		public override int LocalSignatureMetadataToken
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000214 RID: 532 RVA: 0x00005E68 File Offset: 0x00004068
		public override IList<LocalVariableInfo> LocalVariables
		{
			get
			{
				Token token = new Token(this.LocalSignatureMetadataToken);
				EmbeddedBlobPointer pointer = default(EmbeddedBlobPointer);
				int num = 0;
				bool flag = !token.IsNil;
				if (flag)
				{
					this.Method.Resolver.RawImport.GetSigFromToken(token, out pointer, out num);
				}
				bool flag2 = num == 0;
				IList<LocalVariableInfo> result;
				if (flag2)
				{
					result = new MetadataOnlyLocalVariableInfo[0];
				}
				else
				{
					GenericContext context = new GenericContext(this.Method);
					byte[] sig = this.Method.Resolver.ReadEmbeddedBlob(pointer, num);
					int num2 = 0;
					CorCallingConvention corCallingConvention = SignatureUtil.ExtractCallingConvention(sig, ref num2);
					int num3 = SignatureUtil.ExtractInt(sig, ref num2);
					MetadataOnlyLocalVariableInfo[] array = new MetadataOnlyLocalVariableInfo[num3];
					for (int i = 0; i < num3; i++)
					{
						TypeSignatureDescriptor typeSignatureDescriptor = SignatureUtil.ExtractType(sig, ref num2, this.Method.Resolver, context, true);
						array[i] = new MetadataOnlyLocalVariableInfo(i, typeSignatureDescriptor.Type, typeSignatureDescriptor.IsPinned);
					}
					result = array;
				}
				return result;
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000215 RID: 533 RVA: 0x00002390 File Offset: 0x00000590
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public override int MaxStackSize
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		// Token: 0x06000216 RID: 534 RVA: 0x00002390 File Offset: 0x00000590
		public override byte[] GetILAsByteArray()
		{
			throw new InvalidOperationException();
		}
	}
}
