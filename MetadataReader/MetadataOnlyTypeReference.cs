using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;
using System.Text;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000042 RID: 66
	[DebuggerDisplay("\\{Name = {Name} FullName = {FullName} {TypeRefToken}\\}")]
	internal class MetadataOnlyTypeReference : TypeProxy, ITypeReference, ITypeProxy
	{
		// Token: 0x0600047D RID: 1149 RVA: 0x0000EEB2 File Offset: 0x0000D0B2
		public MetadataOnlyTypeReference(MetadataOnlyModule resolver, Token typeRef) : base(resolver)
		{
			this.TypeRefToken = typeRef;
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x0000EEC4 File Offset: 0x0000D0C4
		protected override Type GetResolvedTypeWorker()
		{
			return this.m_resolver.ResolveTypeRef(this);
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x0600047F RID: 1151 RVA: 0x0000EEE4 File Offset: 0x0000D0E4
		public Module DeclaringScope
		{
			get
			{
				return this.m_resolver;
			}
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000480 RID: 1152 RVA: 0x0000EEFC File Offset: 0x0000D0FC
		public Token TypeRefToken { get; }

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000481 RID: 1153 RVA: 0x0000EF04 File Offset: 0x0000D104
		public Token ResolutionScope
		{
			get
			{
				int value;
				int num;
				this.m_resolver.RawImport.GetTypeRefProps(this.TypeRefToken.Value, out value, null, 0, out num);
				Token result = new Token(value);
				return result;
			}
		}

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000482 RID: 1154 RVA: 0x0000EF48 File Offset: 0x0000D148
		public virtual string RawName
		{
			get
			{
				int value = this.TypeRefToken.Value;
				int num;
				int capacity;
				this.m_resolver.RawImport.GetTypeRefProps(value, out num, null, 0, out capacity);
				StringBuilder stringBuilder = StringBuilderPool.Get(capacity);
				this.m_resolver.RawImport.GetTypeRefProps(value, out num, stringBuilder, stringBuilder.Capacity, out capacity);
				string result = stringBuilder.ToString();
				StringBuilderPool.Release(ref stringBuilder);
				return result;
			}
		}

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000483 RID: 1155 RVA: 0x0000EFBC File Offset: 0x0000D1BC
		public override string Namespace
		{
			get
			{
				bool flag = this.DeclaringType != null;
				string result;
				if (flag)
				{
					result = this.DeclaringType.Namespace;
				}
				else
				{
					result = Utility.GetNamespaceHelper(this.FullName);
				}
				return result;
			}
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000484 RID: 1156 RVA: 0x0000EFF8 File Offset: 0x0000D1F8
		public override string Name
		{
			get
			{
				return Utility.GetTypeNameFromFullNameHelper(this.FullName, base.IsNested);
			}
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x06000485 RID: 1157 RVA: 0x0000F01C File Offset: 0x0000D21C
		public override string FullName
		{
			get
			{
				int value = this.TypeRefToken.Value;
				string result = string.Empty;
				string text = string.Empty;
				StringBuilder stringBuilder;
				for (;;)
				{
					int value2;
					int capacity;
					this.m_resolver.RawImport.GetTypeRefProps(value, out value2, null, 0, out capacity);
					stringBuilder = StringBuilderPool.Get(capacity);
					Token token = new Token(value2);
					this.m_resolver.RawImport.GetTypeRefProps(value, out value2, stringBuilder, stringBuilder.Capacity, out capacity);
					bool flag = token.IsType(TokenType.TypeRef);
					if (!flag)
					{
						break;
					}
					text = "+" + stringBuilder + text;
					value = token.Value;
				}
				stringBuilder.Append(text);
				result = stringBuilder.ToString();
				StringBuilderPool.Release(ref stringBuilder);
				return result;
			}
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x06000486 RID: 1158 RVA: 0x0000F0EC File Offset: 0x0000D2EC
		private AssemblyName RequestedAssemblyName
		{
			get
			{
				Token resolutionScope = this.ResolutionScope;
				TokenType tokenType = resolutionScope.TokenType;
				if (tokenType <= TokenType.TypeRef)
				{
					if (tokenType != TokenType.Module)
					{
						if (tokenType != TokenType.TypeRef)
						{
							goto IL_78;
						}
						MetadataOnlyTypeReference metadataOnlyTypeReference = new MetadataOnlyTypeReference(this.m_resolver, resolutionScope);
						return metadataOnlyTypeReference.RequestedAssemblyName;
					}
				}
				else if (tokenType != TokenType.ModuleRef)
				{
					if (tokenType != TokenType.AssemblyRef)
					{
						goto IL_78;
					}
					return this.m_resolver.GetAssemblyNameFromAssemblyRef(resolutionScope);
				}
				return this.m_resolver.Assembly.GetName();
				IL_78:
				throw new InvalidOperationException(Resources.InvalidMetadata);
			}
		}

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x06000487 RID: 1159 RVA: 0x0000F180 File Offset: 0x0000D380
		public override Assembly Assembly
		{
			get
			{
				AssemblyName requestedAssemblyName = this.RequestedAssemblyName;
				return new AssemblyRef(requestedAssemblyName, base.TypeUniverse);
			}
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000488 RID: 1160 RVA: 0x0000F1A8 File Offset: 0x0000D3A8
		public override string AssemblyQualifiedName
		{
			get
			{
				string assemblyName = this.RequestedAssemblyName.ToString();
				string fullName = this.FullName;
				return System.Reflection.Assembly.CreateQualifiedName(assemblyName, fullName);
			}
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x0000F1D4 File Offset: 0x0000D3D4
		protected override bool IsByRefImpl()
		{
			return false;
		}

		// Token: 0x0600048A RID: 1162 RVA: 0x0000F1E8 File Offset: 0x0000D3E8
		protected override bool IsArrayImpl()
		{
			return false;
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x0600048B RID: 1163 RVA: 0x0000F1FC File Offset: 0x0000D3FC
		public override bool IsGenericParameter
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600048C RID: 1164 RVA: 0x0000F210 File Offset: 0x0000D410
		protected override bool IsPointerImpl()
		{
			return false;
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x0600048D RID: 1165 RVA: 0x0000F224 File Offset: 0x0000D424
		public override Type DeclaringType
		{
			get
			{
				int value = this.TypeRefToken.Value;
				int value2;
				int num;
				this.m_resolver.RawImport.GetTypeRefProps(value, out value2, null, 0, out num);
				Token tokenTypeRef = new Token(value2);
				bool flag = tokenTypeRef.IsType(TokenType.TypeRef);
				Type result;
				if (flag)
				{
					result = this.m_resolver.Factory.CreateTypeRef(this.m_resolver, tokenTypeRef);
				}
				else
				{
					result = null;
				}
				return result;
			}
		}
	}
}
