using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200002C RID: 44
	[SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
	internal class MetadataOnlyModule : Module, IModule2, IDisposable
	{
		// Token: 0x06000277 RID: 631 RVA: 0x000075FF File Offset: 0x000057FF
		public MetadataOnlyModule(ITypeUniverse universe, MetadataFile import, string modulePath) : this(universe, import, new DefaultFactory(), modulePath)
		{
		}

		// Token: 0x06000278 RID: 632 RVA: 0x00007614 File Offset: 0x00005814
		public MetadataOnlyModule(ITypeUniverse universe, MetadataFile import, IReflectionFactory factory, string modulePath)
		{
			this.AssemblyResolver = universe;
			this.RawMetadata = import;
			this.Factory = factory;
			this.Policy = new MetadataExtensionsPolicy20(universe);
			this._modulePath = modulePath;
			this._cachedThreadAffinityImporter = new Dictionary<Thread, IMetadataImport>();
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000279 RID: 633 RVA: 0x00007668 File Offset: 0x00005868
		public override string FullyQualifiedName
		{
			get
			{
				return this._modulePath;
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x0600027A RID: 634 RVA: 0x00007680 File Offset: 0x00005880
		internal IMetadataExtensionsPolicy Policy { get; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x0600027B RID: 635 RVA: 0x00007688 File Offset: 0x00005888
		internal IReflectionFactory Factory { get; }

		// Token: 0x0600027C RID: 636 RVA: 0x00007690 File Offset: 0x00005890
		public override string ToString()
		{
			bool flag = this.RawMetadata == null;
			string result;
			if (flag)
			{
				result = "uninitialized";
			}
			else
			{
				result = this.ScopeName;
			}
			return result;
		}

		// Token: 0x0600027D RID: 637 RVA: 0x000076C0 File Offset: 0x000058C0
		public override bool Equals(object obj)
		{
			bool flag = obj == this;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				MetadataOnlyModule metadataOnlyModule = obj as MetadataOnlyModule;
				bool flag2 = metadataOnlyModule != null;
				if (flag2)
				{
					bool flag3 = !this.AssemblyResolver.Equals(metadataOnlyModule.AssemblyResolver);
					result = (!flag3 && this.ScopeName == metadataOnlyModule.ScopeName);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0600027E RID: 638 RVA: 0x00007724 File Offset: 0x00005924
		public override int GetHashCode()
		{
			return this.RawMetadata.RawPtr.GetHashCode() + this.AssemblyResolver.GetHashCode();
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x0600027F RID: 639 RVA: 0x00007755 File Offset: 0x00005955
		public ITypeUniverse AssemblyResolver { get; }

		// Token: 0x06000280 RID: 640 RVA: 0x00007760 File Offset: 0x00005960
		internal bool IsValidToken(int token)
		{
			return this.RawImport.IsValidToken((uint)token);
		}

		// Token: 0x06000281 RID: 641 RVA: 0x00007780 File Offset: 0x00005980
		internal bool IsValidToken(Token token)
		{
			return this.IsValidToken(token.Value);
		}

		// Token: 0x06000282 RID: 642 RVA: 0x000077A0 File Offset: 0x000059A0
		public byte[] ReadEmbeddedBlob(EmbeddedBlobPointer pointer, int countBytes)
		{
			return this.RawMetadata.ReadEmbeddedBlob(pointer, countBytes);
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000283 RID: 643 RVA: 0x000077BF File Offset: 0x000059BF
		internal MetadataFile RawMetadata { get; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000284 RID: 644 RVA: 0x000077C8 File Offset: 0x000059C8
		internal IMetadataImport RawImport
		{
			get
			{
				return this.GetThreadSafeImporter();
			}
		}

		// Token: 0x06000285 RID: 645 RVA: 0x000077E0 File Offset: 0x000059E0
		private IMetadataImport GetThreadSafeImporter()
		{
			object @lock = this._lock;
			IMetadataImport metadataImport;
			lock (@lock)
			{
				bool flag2 = !this._cachedThreadAffinityImporter.TryGetValue(Thread.CurrentThread, out metadataImport);
				if (flag2)
				{
					object uniqueObjectForIUnknown = Marshal.GetUniqueObjectForIUnknown(this.RawMetadata.RawPtr);
					metadataImport = (IMetadataImport)uniqueObjectForIUnknown;
					this._cachedThreadAffinityImporter.Add(Thread.CurrentThread, metadataImport);
				}
			}
			return metadataImport;
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000286 RID: 646 RVA: 0x00007870 File Offset: 0x00005A70
		public override string ScopeName
		{
			get
			{
				bool flag = this._scopeName == null;
				if (flag)
				{
					IMetadataImport threadSafeImporter = this.GetThreadSafeImporter();
					int num;
					Guid guid;
					threadSafeImporter.GetScopeProps(null, 0, out num, out guid);
					StringBuilder stringBuilder = StringBuilderPool.Get(num);
					threadSafeImporter.GetScopeProps(stringBuilder, stringBuilder.Capacity, out num, out guid);
					stringBuilder.Length = num - 1;
					this._scopeName = stringBuilder.ToString();
					StringBuilderPool.Release(ref stringBuilder);
				}
				return this._scopeName;
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000287 RID: 647 RVA: 0x000078EC File Offset: 0x00005AEC
		public override Guid ModuleVersionId
		{
			get
			{
				int num;
				Guid result;
				this.RawImport.GetScopeProps(null, 0, out num, out result);
				return result;
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000288 RID: 648 RVA: 0x00007914 File Offset: 0x00005B14
		public override string Name
		{
			get
			{
				return Path.GetFileName(this._modulePath);
			}
		}

		// Token: 0x06000289 RID: 649 RVA: 0x00007934 File Offset: 0x00005B34
		internal MetadataOnlyCommonType ResolveTypeDefToken(Token token)
		{
			return this.Factory.CreateSimpleType(this, token);
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00007958 File Offset: 0x00005B58
		private void EnsureValidToken(Token token)
		{
			bool flag = !this.IsValidToken(token);
			if (flag)
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidMetadataToken, new object[]
				{
					token
				}));
			}
		}

		// Token: 0x0600028B RID: 651 RVA: 0x0000799C File Offset: 0x00005B9C
		internal Type ResolveTypeTokenInternal(Token token, GenericContext context)
		{
			this.EnsureValidToken(token);
			bool flag = token.IsType(TokenType.TypeDef);
			Type result;
			if (flag)
			{
				result = this.ResolveTypeDefToken(token);
			}
			else
			{
				bool flag2 = token.IsType(TokenType.TypeRef);
				if (flag2)
				{
					result = this.Factory.CreateTypeRef(this, token);
				}
				else
				{
					bool flag3 = token.IsType(TokenType.TypeSpec);
					if (!flag3)
					{
						throw new ArgumentException(Resources.TypeTokenExpected);
					}
					Type[] typeArgs = null;
					Type[] methodArgs = null;
					bool flag4 = context != null;
					if (flag4)
					{
						typeArgs = context.TypeArgs;
						methodArgs = context.MethodArgs;
					}
					result = this.Factory.CreateTypeSpec(this, token, typeArgs, methodArgs);
				}
			}
			return result;
		}

		// Token: 0x0600028C RID: 652 RVA: 0x00007A44 File Offset: 0x00005C44
		internal Type GetGenericType(Token token, GenericContext context)
		{
			Type[] array = null;
			Type[] methodArgs = null;
			bool flag = context != null;
			if (flag)
			{
				array = context.TypeArgs;
				methodArgs = context.MethodArgs;
			}
			bool flag2 = token.IsType(TokenType.TypeDef);
			Type result;
			if (flag2)
			{
				bool flag3 = array != null && array.Length != 0;
				if (flag3)
				{
					result = this.Factory.CreateGenericType(this, token, array);
				}
				else
				{
					result = this.Factory.CreateSimpleType(this, token);
				}
			}
			else
			{
				bool flag4 = token.IsType(TokenType.TypeRef);
				if (flag4)
				{
					Type type = this.Factory.CreateTypeRef(this, token);
					bool flag5 = array != null && array.Length != 0;
					if (flag5)
					{
						type = type.MakeGenericType(array);
					}
					result = type;
				}
				else
				{
					bool flag6 = token.IsType(TokenType.TypeSpec);
					if (!flag6)
					{
						throw new ArgumentException(Resources.TypeTokenExpected);
					}
					result = this.Factory.CreateTypeSpec(this, token, array, methodArgs);
				}
			}
			return result;
		}

		// Token: 0x0600028D RID: 653 RVA: 0x00007B30 File Offset: 0x00005D30
		private MethodBase ResolveMethodTokenInternal(Token methodToken, GenericContext context)
		{
			this.EnsureValidToken(methodToken);
			bool flag = methodToken.IsType(TokenType.MethodDef);
			MethodBase result;
			if (flag)
			{
				result = this.ResolveMethodDef(methodToken);
			}
			else
			{
				bool flag2 = methodToken.IsType(TokenType.MemberRef);
				if (flag2)
				{
					result = this.ResolveMethodRef(methodToken, context, null);
				}
				else
				{
					bool flag3 = methodToken.IsType(TokenType.MethodSpec);
					if (!flag3)
					{
						throw new ArgumentException(Resources.MethodTokenExpected);
					}
					result = this.ResolveMethodSpec(methodToken, context);
				}
			}
			return result;
		}

		// Token: 0x0600028E RID: 654 RVA: 0x00007BA8 File Offset: 0x00005DA8
		private MethodInfo ResolveMethodSpec(Token methodToken, GenericContext context)
		{
			Token token;
			EmbeddedBlobPointer pointer;
			int countBytes;
			((IMetadataImport2)this.RawImport).GetMethodSpecProps(methodToken, out token, out pointer, out countBytes);
			byte[] sig = this.ReadEmbeddedBlob(pointer, countBytes);
			int num = 0;
			CorCallingConvention corCallingConvention = SignatureUtil.ExtractCallingConvention(sig, ref num);
			int num2 = SignatureUtil.ExtractInt(sig, ref num);
			Type[] array = new Type[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = SignatureUtil.ExtractType(sig, ref num, this, context);
			}
			Token token2 = new Token(token);
			TokenType tokenType = token2.TokenType;
			MethodInfo result;
			if (tokenType != TokenType.MethodDef)
			{
				if (tokenType != TokenType.MemberRef)
				{
					throw new InvalidOperationException();
				}
				result = (MethodInfo)this.ResolveMethodRef(token2, context, array);
			}
			else
			{
				result = this.GetGenericMethodInfo(token2, new GenericContext(null, array));
			}
			return result;
		}

		// Token: 0x0600028F RID: 655 RVA: 0x00007C84 File Offset: 0x00005E84
		private MethodBase ResolveMethodDef(Token methodToken)
		{
			List<Type> typeParameters = this.GetTypeParameters(methodToken.Value);
			GenericContext context = null;
			bool flag = typeParameters.Count > 0;
			if (flag)
			{
				context = new GenericContext(null, typeParameters.ToArray());
			}
			return MetadataOnlyMethodInfo.Create(this, methodToken, context);
		}

		// Token: 0x06000290 RID: 656 RVA: 0x00007CD0 File Offset: 0x00005ED0
		internal MethodInfo GetGenericMethodInfo(Token methodToken, GenericContext genericContext)
		{
			return (MethodInfo)this.GetGenericMethodBase(methodToken, genericContext);
		}

		// Token: 0x06000291 RID: 657 RVA: 0x00007CF0 File Offset: 0x00005EF0
		internal MethodBase GetGenericMethodBase(Token methodToken, GenericContext genericContext)
		{
			bool flag = genericContext != null;
			if (flag)
			{
				bool flag2 = (genericContext.TypeArgs == null || genericContext.TypeArgs.Length == 0) && (genericContext.MethodArgs == null || genericContext.MethodArgs.Length == 0);
				if (flag2)
				{
					genericContext = null;
				}
			}
			return MetadataOnlyMethodInfo.Create(this, methodToken, genericContext);
		}

		// Token: 0x06000292 RID: 658 RVA: 0x00007D48 File Offset: 0x00005F48
		internal MethodBase ResolveMethodRef(Token memberRef, GenericContext context, Type[] genericMethodParameters)
		{
			Token token;
			string methodName;
			SignatureBlob signatureBlob;
			this.GetMemberRefData(memberRef, out token, out methodName, out signatureBlob);
			byte[] signatureAsByteArray = signatureBlob.GetSignatureAsByteArray();
			int num = 0;
			CorCallingConvention corCallingConvention = SignatureUtil.ExtractCallingConvention(signatureAsByteArray, ref num);
			bool flag = corCallingConvention == CorCallingConvention.VarArg;
			if (flag)
			{
				throw new NotImplementedException(Resources.VarargSignaturesNotImplemented);
			}
			Type type = this.ResolveTypeTokenInternal(token, context);
			bool isArray = type.IsArray;
			MethodSignatureDescriptor expectedSignature;
			if (isArray)
			{
				expectedSignature = SignatureUtil.ExtractMethodSignature(signatureBlob, this, context);
			}
			else
			{
				GenericContext context2 = new OpenGenericContext(this, type, memberRef);
				expectedSignature = SignatureUtil.ExtractMethodSignature(signatureBlob, this, context2);
			}
			GenericContext context3 = new GenericContext(type.GetGenericArguments(), genericMethodParameters);
			MethodBase methodBase = SignatureComparer.FindMatchingMethod(methodName, type, expectedSignature, context3);
			bool flag2 = methodBase == null;
			if (flag2)
			{
				throw new MissingMethodException(type.Name, methodName);
			}
			return methodBase;
		}

		// Token: 0x06000293 RID: 659 RVA: 0x00007E0C File Offset: 0x0000600C
		internal FieldInfo ResolveFieldRef(Token memberRef, GenericContext context)
		{
			Token token;
			string name;
			SignatureBlob signatureBlob;
			this.GetMemberRefData(memberRef, out token, out name, out signatureBlob);
			Type type = this.ResolveTypeTokenInternal(token, context);
			return type.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		// Token: 0x06000294 RID: 660 RVA: 0x00007E44 File Offset: 0x00006044
		internal FieldInfo ResolveFieldTokenInternal(Token fieldToken, GenericContext context)
		{
			bool flag = fieldToken.IsType(TokenType.FieldDef);
			FieldInfo result;
			if (flag)
			{
				FieldInfo fieldInfo = this.Factory.CreateField(this, fieldToken, null, null);
				result = fieldInfo;
			}
			else
			{
				bool flag2 = fieldToken.IsType(TokenType.MemberRef);
				if (!flag2)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidMetadataToken, new object[]
					{
						fieldToken
					}));
				}
				FieldInfo fieldInfo2 = this.ResolveFieldRef(fieldToken, context);
				result = fieldInfo2;
			}
			return result;
		}

		// Token: 0x06000295 RID: 661 RVA: 0x00007EBC File Offset: 0x000060BC
		public override string ResolveString(int metadataToken)
		{
			Token token = new Token(metadataToken);
			IMetadataImport rawImport = this.RawImport;
			int num;
			rawImport.GetUserString(token, null, 0, out num);
			char[] array = new char[num];
			rawImport.GetUserString(token, array, array.Length, out num);
			return new string(array);
		}

		// Token: 0x06000296 RID: 662 RVA: 0x00007F14 File Offset: 0x00006114
		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this.GetCustomAttributeData(this.MetadataToken);
		}

		// Token: 0x06000297 RID: 663 RVA: 0x00007F34 File Offset: 0x00006134
		internal Type ResolveTypeRef(ITypeReference typeReference)
		{
			Token resolutionScope = typeReference.ResolutionScope;
			string rawName = typeReference.RawName;
			TokenType tokenType = resolutionScope.TokenType;
			if (tokenType <= TokenType.TypeRef)
			{
				if (tokenType == TokenType.Module)
				{
					return base.GetType(typeReference.FullName);
				}
				if (tokenType == TokenType.TypeRef)
				{
					Type type = this.Factory.CreateTypeRef(this, resolutionScope);
					return type.GetNestedType(rawName, BindingFlags.Public | BindingFlags.NonPublic);
				}
			}
			else
			{
				if (tokenType == TokenType.ModuleRef)
				{
					Module module = this.ResolveModuleRef(resolutionScope);
					return module.GetType(typeReference.FullName);
				}
				if (tokenType == TokenType.AssemblyRef)
				{
					Assembly assembly = this.AssemblyResolver.ResolveAssembly(this, resolutionScope);
					bool flag = assembly == null;
					if (flag)
					{
						throw new InvalidOperationException(Resources.ResolverMustResolveToValidAssembly);
					}
					IAssembly2 assembly2 = (IAssembly2)assembly;
					bool flag2 = assembly2.TypeUniverse != this.AssemblyResolver;
					if (flag2)
					{
						throw new InvalidOperationException(Resources.ResolvedAssemblyMustBeWithinSameUniverse);
					}
					return assembly.GetType(rawName, true);
				}
			}
			throw new InvalidOperationException(Resources.InvalidMetadata);
		}

		// Token: 0x06000298 RID: 664 RVA: 0x00008050 File Offset: 0x00006250
		internal Module ResolveModuleRef(Token moduleRefToken)
		{
			bool flag = this.Assembly == null;
			if (flag)
			{
				throw new InvalidOperationException(Resources.CannotResolveModuleRefOnNetModule);
			}
			StringBuilder stringBuilder = StringBuilderPool.Get();
			IMetadataImport rawImport = this.RawImport;
			int capacity;
			rawImport.GetModuleRefProps(moduleRefToken.Value, null, 0, out capacity);
			stringBuilder.EnsureCapacity(capacity);
			rawImport.GetModuleRefProps(moduleRefToken.Value, stringBuilder, stringBuilder.Capacity, out capacity);
			string name = stringBuilder.ToString();
			StringBuilderPool.Release(ref stringBuilder);
			return this.Assembly.GetModule(name);
		}

		// Token: 0x06000299 RID: 665 RVA: 0x000080D8 File Offset: 0x000062D8
		internal Token LookupTypeToken(string className)
		{
			return this.FindTypeDefByName(null, className, true);
		}

		// Token: 0x0600029A RID: 666 RVA: 0x000080F4 File Offset: 0x000062F4
		internal Token FindTypeDefByName(Type outerType, string className, bool fThrow)
		{
			Token outerTypeDefToken = new Token(0);
			bool flag = outerType != null;
			if (flag)
			{
				bool flag2 = outerType.Module != this;
				if (flag2)
				{
					throw new InvalidOperationException(Resources.DifferentTokenResolverForOuterType);
				}
				outerTypeDefToken = new Token(outerType.MetadataToken);
			}
			return this.FindTypeDefByName(outerTypeDefToken, className, fThrow);
		}

		// Token: 0x0600029B RID: 667 RVA: 0x0000814C File Offset: 0x0000634C
		internal Token FindTypeDefByName(Token outerTypeDefToken, string className, bool fThrow)
		{
			bool flag = !outerTypeDefToken.IsNil;
			if (flag)
			{
			}
			int value;
			int num = this.RawImport.FindTypeDefByName(className, outerTypeDefToken, out value);
			bool flag2 = !fThrow && num == -2146234064;
			Token result;
			if (flag2)
			{
				result = Token.Nil;
			}
			else
			{
				bool flag3 = num != 0;
				if (flag3)
				{
					throw Marshal.GetExceptionForHR(num);
				}
				Token token = new Token(value);
				result = token;
			}
			return result;
		}

		// Token: 0x0600029C RID: 668 RVA: 0x000081C0 File Offset: 0x000063C0
		internal void GetMemberRefData(Token token, out Token declaringTypeToken, out string nameMember, out SignatureBlob sig)
		{
			bool flag = !this.IsValidToken(token);
			if (flag)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidMetadataToken, new object[]
				{
					token
				}));
			}
			IMetadataImport rawImport = this.RawImport;
			uint capacity;
			EmbeddedBlobPointer pointer;
			uint countBytes;
			rawImport.GetMemberRefProps(token, out declaringTypeToken, null, 0, out capacity, out pointer, out countBytes);
			StringBuilder stringBuilder = StringBuilderPool.Get((int)capacity);
			rawImport.GetMemberRefProps(token, out declaringTypeToken, stringBuilder, stringBuilder.Capacity, out capacity, out pointer, out countBytes);
			nameMember = stringBuilder.ToString();
			StringBuilderPool.Release(ref stringBuilder);
			sig = SignatureBlob.ReadSignature(this.RawMetadata, pointer, (int)countBytes);
		}

		// Token: 0x0600029D RID: 669 RVA: 0x0000825C File Offset: 0x0000645C
		internal uint GetMethodRva(int methodDef)
		{
			IMetadataImport rawImport = this.RawImport;
			int num;
			uint num2;
			MethodAttributes methodAttributes;
			EmbeddedBlobPointer embeddedBlobPointer;
			uint num3;
			uint result;
			uint num4;
			rawImport.GetMethodProps((uint)methodDef, out num, null, 0, out num2, out methodAttributes, out embeddedBlobPointer, out num3, out result, out num4);
			return result;
		}

		// Token: 0x0600029E RID: 670 RVA: 0x00008294 File Offset: 0x00006494
		internal MethodImplAttributes GetMethodImplFlags(int methodToken)
		{
			uint num;
			uint result;
			this.RawImport.GetRVA(methodToken, out num, out result);
			return (MethodImplAttributes)result;
		}

		// Token: 0x0600029F RID: 671 RVA: 0x000082B8 File Offset: 0x000064B8
		internal void GetMethodAttrs(Token methodDef, out Token declaringTypeDef, out MethodAttributes attrs, out uint nameLength)
		{
			bool flag = !this.IsValidToken(methodDef);
			if (flag)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidMetadataToken, new object[]
				{
					methodDef
				}));
			}
			uint value = (uint)methodDef.Value;
			IMetadataImport rawImport = this.RawImport;
			int value2;
			EmbeddedBlobPointer embeddedBlobPointer;
			uint num;
			uint num2;
			uint num3;
			rawImport.GetMethodProps(value, out value2, null, 0, out nameLength, out attrs, out embeddedBlobPointer, out num, out num2, out num3);
			declaringTypeDef = new Token(value2);
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x00008334 File Offset: 0x00006534
		internal void GetMethodSig(Token methodDef, out SignatureBlob signature)
		{
			bool flag = !this.IsValidToken(methodDef);
			if (flag)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidMetadataToken, new object[]
				{
					methodDef
				}));
			}
			uint value = (uint)methodDef.Value;
			IMetadataImport rawImport = this.RawImport;
			int num;
			uint num2;
			MethodAttributes methodAttributes;
			EmbeddedBlobPointer pointer;
			uint countBytes;
			uint num3;
			uint num4;
			rawImport.GetMethodProps(value, out num, null, 0, out num2, out methodAttributes, out pointer, out countBytes, out num3, out num4);
			signature = SignatureBlob.ReadSignature(this.RawMetadata, pointer, (int)countBytes);
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x000083B4 File Offset: 0x000065B4
		internal void GetMethodName(Token methodDef, uint nameLength, out string name)
		{
			bool flag = !this.IsValidToken(methodDef);
			if (flag)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidMetadataToken, new object[]
				{
					methodDef
				}));
			}
			uint value = (uint)methodDef.Value;
			IMetadataImport rawImport = this.RawImport;
			StringBuilder stringBuilder = StringBuilderPool.Get((int)nameLength);
			int num;
			MethodAttributes methodAttributes;
			EmbeddedBlobPointer embeddedBlobPointer;
			uint num2;
			uint num3;
			uint num4;
			rawImport.GetMethodProps(value, out num, stringBuilder, stringBuilder.Capacity, out nameLength, out methodAttributes, out embeddedBlobPointer, out num2, out num3, out num4);
			name = stringBuilder.ToString();
			StringBuilderPool.Release(ref stringBuilder);
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x00008444 File Offset: 0x00006644
		internal CorElementType GetEnumUnderlyingType(Token tokenTypeDef)
		{
			IMetadataImport rawImport = this.RawImport;
			HCORENUM hcorenum = default(HCORENUM);
			try
			{
				int mb;
				uint num;
				rawImport.EnumFields(ref hcorenum, tokenTypeDef.Value, out mb, 1, out num);
				while (num > 0U)
				{
					int num2;
					int num3;
					FieldAttributes fieldAttributes;
					EmbeddedBlobPointer pointer;
					int countBytes;
					int num4;
					IntPtr intPtr;
					int num5;
					rawImport.GetFieldProps(mb, out num2, null, 0, out num3, out fieldAttributes, out pointer, out countBytes, out num4, out intPtr, out num5);
					bool flag = (fieldAttributes & FieldAttributes.Static) == FieldAttributes.PrivateScope;
					if (flag)
					{
						byte[] sig = this.ReadEmbeddedBlob(pointer, countBytes);
						int num6 = 0;
						CorCallingConvention corCallingConvention = SignatureUtil.ExtractCallingConvention(sig, ref num6);
						return SignatureUtil.ExtractElementType(sig, ref num6);
					}
					rawImport.EnumFields(ref hcorenum, tokenTypeDef.Value, out mb, 1, out num);
				}
				throw new ArgumentException(Resources.OperationValidOnEnumOnly);
			}
			finally
			{
				hcorenum.Close(rawImport);
			}
			CorElementType result;
			return result;
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x00008514 File Offset: 0x00006714
		internal void GetTypeAttributes(Token tokenTypeDef, out Token tokenExtends, out TypeAttributes attr, out int nameLength)
		{
			IMetadataImport rawImport = this.RawImport;
			int value;
			rawImport.GetTypeDefProps(tokenTypeDef.Value, null, 0, out nameLength, out attr, out value);
			tokenExtends = new Token(value);
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x0000854C File Offset: 0x0000674C
		internal void GetTypeName(Token tokenTypeDef, int nameLength, out string name)
		{
			IMetadataImport rawImport = this.RawImport;
			StringBuilder stringBuilder = StringBuilderPool.Get(nameLength);
			TypeAttributes typeAttributes;
			int num;
			rawImport.GetTypeDefProps(tokenTypeDef.Value, stringBuilder, stringBuilder.Capacity, out nameLength, out typeAttributes, out num);
			name = TypeNameQuoter.GetQuotedTypeName(stringBuilder.ToString());
			StringBuilderPool.Release(ref stringBuilder);
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x00008598 File Offset: 0x00006798
		internal static ConstructorInfo[] GetConstructorsOnType(MetadataOnlyCommonType type, BindingFlags flags)
		{
			MetadataOnlyModule.CheckBindingFlagsInMethod(flags, "GetConstructorsOnType");
			List<ConstructorInfo> list = new List<ConstructorInfo>();
			IEnumerable<MethodBase> declaredConstructors = type.GetDeclaredConstructors();
			foreach (MethodBase methodBase in declaredConstructors)
			{
				ConstructorInfo constructorInfo = (ConstructorInfo)methodBase;
				bool flag = Utility.IsBindingFlagsMatching(constructorInfo, false, flags);
				if (flag)
				{
					list.Add(constructorInfo);
				}
			}
			return list.ToArray();
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x00008620 File Offset: 0x00006820
		internal static ConstructorInfo GetConstructorOnType(MetadataOnlyCommonType type, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			MetadataOnlyModule.CheckBinderAndModifiersforLMR(binder, modifiers);
			ConstructorInfo[] constructorsOnType = MetadataOnlyModule.GetConstructorsOnType(type, bindingAttr);
			foreach (ConstructorInfo constructorInfo in constructorsOnType)
			{
				bool flag = !SignatureUtil.IsCallingConventionMatch(constructorInfo, callConvention);
				if (!flag)
				{
					bool flag2 = !SignatureUtil.IsParametersTypeMatch(constructorInfo, types);
					if (!flag2)
					{
						return constructorInfo;
					}
				}
			}
			return null;
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x00008688 File Offset: 0x00006888
		private static void CheckBinderAndModifiersforLMR(Binder binder, ParameterModifier[] modifiers)
		{
			bool flag = binder != null;
			if (flag)
			{
				throw new NotSupportedException();
			}
			bool flag2 = modifiers != null && modifiers.Length != 0;
			if (flag2)
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x000086BC File Offset: 0x000068BC
		internal static MethodInfo GetMethodImplHelper(Type type, string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConv, Type[] types, ParameterModifier[] modifiers)
		{
			bool flag = modifiers != null && modifiers.Length != 0;
			if (flag)
			{
				throw new NotSupportedException();
			}
			MethodInfo[] methods = type.GetMethods(bindingAttr);
			bool flag2 = binder == null;
			MethodInfo result;
			if (flag2)
			{
				result = MetadataOnlyModule.FilterMethod(methods, name, bindingAttr, callConv, types);
			}
			else
			{
				List<MethodBase> list = new List<MethodBase>();
				StringComparison stringComparison = SignatureUtil.GetStringComparison(bindingAttr);
				foreach (MethodInfo methodInfo in methods)
				{
					bool flag3 = !methodInfo.Name.Equals(name, stringComparison);
					if (!flag3)
					{
						bool flag4 = !SignatureUtil.IsCallingConventionMatch(methodInfo, callConv);
						if (!flag4)
						{
							list.Add(methodInfo);
						}
					}
				}
				result = (binder.SelectMethod(bindingAttr, list.ToArray(), types, modifiers) as MethodInfo);
			}
			return result;
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x00008788 File Offset: 0x00006988
		private static MethodInfo FilterMethod(MethodInfo[] methods, string name, BindingFlags bindingAttr, CallingConventions callConv, Type[] types)
		{
			bool flag = false;
			MethodInfo methodInfo = null;
			StringComparison stringComparison = SignatureUtil.GetStringComparison(bindingAttr);
			foreach (MethodInfo methodInfo2 in methods)
			{
				bool flag2 = flag && methodInfo.DeclaringType != null && !methodInfo.DeclaringType.Equals(methodInfo2.DeclaringType);
				if (flag2)
				{
					break;
				}
				bool flag3 = !methodInfo2.Name.Equals(name, stringComparison);
				if (!flag3)
				{
					bool flag4 = !SignatureUtil.IsCallingConventionMatch(methodInfo2, callConv);
					if (!flag4)
					{
						bool flag5 = !SignatureUtil.IsParametersTypeMatch(methodInfo2, types);
						if (!flag5)
						{
							bool flag6 = !flag;
							if (!flag6)
							{
								throw new AmbiguousMatchException();
							}
							methodInfo = methodInfo2;
							flag = true;
						}
					}
				}
			}
			return methodInfo;
		}

		// Token: 0x060002AA RID: 682 RVA: 0x00008854 File Offset: 0x00006A54
		internal static MethodInfo[] GetMethodsOnType(MetadataOnlyCommonType type, BindingFlags flags)
		{
			MetadataOnlyModule.CheckBindingFlagsInMethod(flags, "GetMethodsOnType");
			List<MethodInfo> list = new List<MethodInfo>();
			foreach (MethodBase methodBase in type.GetDeclaredMethods())
			{
				MethodInfo methodInfo = (MethodInfo)methodBase;
				bool flag = Utility.IsBindingFlagsMatching(methodInfo, false, flags);
				if (flag)
				{
					list.Add(methodInfo);
				}
			}
			bool flag2 = MetadataOnlyModule.WalkInheritanceChain(flags) && type.BaseType != null;
			if (flag2)
			{
				MethodInfo[] methods = type.BaseType.GetMethods(flags);
				List<MethodInfo> list2 = new List<MethodInfo>();
				foreach (MethodInfo methodInfo2 in methods)
				{
					bool flag3 = MetadataOnlyModule.IncludeInheritedMethod(methodInfo2, list, flags);
					if (flag3)
					{
						list2.Add(methodInfo2);
					}
				}
				list.AddRange(list2);
			}
			return list.ToArray();
		}

		// Token: 0x060002AB RID: 683 RVA: 0x00008950 File Offset: 0x00006B50
		private static bool WalkInheritanceChain(BindingFlags flags)
		{
			bool flag = (flags & BindingFlags.DeclaredOnly) > BindingFlags.Default;
			return !flag;
		}

		// Token: 0x060002AC RID: 684 RVA: 0x00008974 File Offset: 0x00006B74
		private static IList<PropertyInfo> FilterInheritedProperties(IList<PropertyInfo> inheritedProperties, IList<PropertyInfo> properties, BindingFlags flags)
		{
			bool flag = properties == null || properties.Count == 0;
			IList<PropertyInfo> result;
			if (flag)
			{
				result = inheritedProperties;
			}
			else
			{
				List<PropertyInfo> list = new List<PropertyInfo>();
				List<MethodInfo> list2 = new List<MethodInfo>();
				List<MethodInfo> list3 = new List<MethodInfo>();
				foreach (PropertyInfo propertyInfo in properties)
				{
					MethodInfo getMethod = propertyInfo.GetGetMethod();
					bool flag2 = getMethod != null;
					if (flag2)
					{
						list2.Add(getMethod);
					}
					MethodInfo setMethod = propertyInfo.GetSetMethod();
					bool flag3 = setMethod != null;
					if (flag3)
					{
						list3.Add(setMethod);
					}
				}
				foreach (PropertyInfo propertyInfo2 in inheritedProperties)
				{
					MethodInfo getMethod2 = propertyInfo2.GetGetMethod();
					bool flag4 = getMethod2 != null && !MetadataOnlyModule.IncludeInheritedAccessor(getMethod2, list2, flags);
					if (!flag4)
					{
						MethodInfo setMethod2 = propertyInfo2.GetSetMethod();
						bool flag5 = setMethod2 != null && !MetadataOnlyModule.IncludeInheritedAccessor(setMethod2, list3, flags);
						if (!flag5)
						{
							list.Add(propertyInfo2);
						}
					}
				}
				result = list;
			}
			return result;
		}

		// Token: 0x060002AD RID: 685 RVA: 0x00008AC0 File Offset: 0x00006CC0
		private static IList<EventInfo> FilterInheritedEvents(IList<EventInfo> inheritedEvents, IList<EventInfo> events)
		{
			bool flag = events == null || events.Count == 0;
			IList<EventInfo> result;
			if (flag)
			{
				result = inheritedEvents;
			}
			else
			{
				List<EventInfo> list = new List<EventInfo>();
				foreach (EventInfo eventInfo in inheritedEvents)
				{
					bool flag2 = false;
					foreach (EventInfo eventInfo2 in events)
					{
						bool flag3 = eventInfo.Name.Equals(eventInfo2.Name, StringComparison.Ordinal);
						if (flag3)
						{
							flag2 = true;
							break;
						}
					}
					bool flag4 = !flag2;
					if (flag4)
					{
						list.Add(eventInfo);
					}
				}
				result = list;
			}
			return result;
		}

		// Token: 0x060002AE RID: 686 RVA: 0x00008BA0 File Offset: 0x00006DA0
		private static bool IncludeInheritedMethod(MethodInfo inheritedMethod, IEnumerable<MethodInfo> methods, BindingFlags flags)
		{
			bool flag = !inheritedMethod.IsStatic;
			bool result;
			if (flag)
			{
				bool isVirtual = inheritedMethod.IsVirtual;
				result = (!isVirtual || !MetadataOnlyModule.IsOverride(methods, inheritedMethod));
			}
			else
			{
				bool flag2 = (flags & BindingFlags.FlattenHierarchy) > BindingFlags.Default;
				result = flag2;
			}
			return result;
		}

		// Token: 0x060002AF RID: 687 RVA: 0x00008BF0 File Offset: 0x00006DF0
		private static bool IncludeInheritedAccessor(MethodInfo inheritedMethod, IEnumerable<MethodInfo> methods, BindingFlags flags)
		{
			bool flag = !inheritedMethod.IsStatic;
			bool result;
			if (flag)
			{
				result = !MetadataOnlyModule.IsOverride(methods, inheritedMethod);
			}
			else
			{
				bool flag2 = (flags & BindingFlags.FlattenHierarchy) > BindingFlags.Default;
				result = (flag2 && !MetadataOnlyModule.IsOverride(methods, inheritedMethod));
			}
			return result;
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x00008C38 File Offset: 0x00006E38
		private static bool IncludeInheritedField(FieldInfo inheritedField, BindingFlags flags)
		{
			bool isPrivate = inheritedField.IsPrivate;
			bool result;
			if (isPrivate)
			{
				result = false;
			}
			else
			{
				bool flag = !inheritedField.IsStatic;
				if (flag)
				{
					result = true;
				}
				else
				{
					bool flag2 = (flags & BindingFlags.FlattenHierarchy) > BindingFlags.Default;
					result = flag2;
				}
			}
			return result;
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x00008C7C File Offset: 0x00006E7C
		internal IEnumerable<MethodBase> GetMethodBasesOnDeclaredTypeOnly(Token tokenTypeDef, GenericContext context, MetadataOnlyModule.EMethodKind kind)
		{
			IMetadataImport import = this.RawImport;
			HCORENUM hEnum = default(HCORENUM);
			try
			{
				for (;;)
				{
					int methodToken;
					int size;
					import.EnumMethods(ref hEnum, tokenTypeDef.Value, out methodToken, 1, out size);
					bool flag = size == 0;
					if (flag)
					{
						break;
					}
					List<Type> genericParams = this.GetTypeParameters(methodToken);
					GenericContext newContext = new GenericContext(context.TypeArgs, genericParams.ToArray());
					MethodBase methodBase = this.GetGenericMethodBase(new Token(methodToken), newContext);
					bool flag2 = methodBase is ConstructorInfo != (kind == MetadataOnlyModule.EMethodKind.Constructor);
					if (!flag2)
					{
						yield return methodBase;
						genericParams = null;
						newContext = null;
						methodBase = null;
					}
				}
			}
			finally
			{
				hEnum.Close(import);
			}
			yield break;
			yield break;
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x00008CA4 File Offset: 0x00006EA4
		private List<Type> GetTypeParameters(int token)
		{
			List<Type> list = new List<Type>();
			foreach (int value in this.GetGenericParameterTokens(token))
			{
				Token typeVariableToken = new Token(value);
				bool flag = typeVariableToken.IsType(TokenType.GenericPar);
				if (flag)
				{
					list.Add(this.Factory.CreateTypeVariable(this, typeVariableToken));
				}
			}
			return list;
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x00008D30 File Offset: 0x00006F30
		private static bool MatchSignatures(MethodBase m1, MethodBase methodCandidate)
		{
			bool flag = m1.Name != methodCandidate.Name;
			if (flag)
			{
				bool flag2 = m1.Name.Length > methodCandidate.Name.Length && m1.Name[m1.Name.Length - methodCandidate.Name.Length - 1] == '.' && m1.Name.EndsWith(methodCandidate.Name, StringComparison.Ordinal);
				bool flag3 = !flag2;
				if (flag3)
				{
					return false;
				}
			}
			bool flag4 = m1.IsStatic != methodCandidate.IsStatic;
			bool result;
			if (flag4)
			{
				result = false;
			}
			else
			{
				ParameterInfo[] parameters = m1.GetParameters();
				ParameterInfo[] parameters2 = methodCandidate.GetParameters();
				bool flag5 = parameters.Length != parameters2.Length;
				if (flag5)
				{
					result = false;
				}
				else
				{
					bool isGenericMethodDefinition = m1.IsGenericMethodDefinition;
					if (isGenericMethodDefinition)
					{
						Type[] genericArguments = methodCandidate.GetGenericArguments();
						m1 = (m1 as MethodInfo).MakeGenericMethod(genericArguments);
						parameters = m1.GetParameters();
					}
					for (int i = 0; i < parameters.Length; i++)
					{
						Type parameterType = parameters[i].ParameterType;
						Type parameterType2 = parameters2[i].ParameterType;
						bool flag6 = !parameterType.Equals(parameterType2);
						if (flag6)
						{
							return false;
						}
					}
					MethodInfo methodInfo = m1 as MethodInfo;
					MethodInfo methodInfo2 = methodCandidate as MethodInfo;
					bool flag7 = (methodInfo != null && methodInfo2 == null) || (methodInfo == null && methodInfo2 != null);
					if (flag7)
					{
						result = false;
					}
					else
					{
						bool flag8 = methodInfo != null;
						if (flag8)
						{
							Type returnType = methodInfo.ReturnType;
							bool flag9 = !returnType.Equals(methodInfo2.ReturnType);
							if (flag9)
							{
								return false;
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x00008EE0 File Offset: 0x000070E0
		private static bool IsOverride(IEnumerable<MethodInfo> methods, MethodInfo m)
		{
			foreach (MethodInfo m2 in methods)
			{
				bool flag = MetadataOnlyModule.IsOverride(m2, m);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x00008F3C File Offset: 0x0000713C
		private static bool IsOverride(MethodInfo m1, MethodInfo m2)
		{
			return MetadataOnlyModule.MatchSignatures(m1, m2);
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x00008F58 File Offset: 0x00007158
		internal static FieldInfo[] GetFieldsOnType(MetadataOnlyCommonType type, BindingFlags flags)
		{
			MetadataOnlyModule.CheckBindingFlagsInMethod(flags, "GetFieldsOnType");
			List<FieldInfo> list = new List<FieldInfo>();
			foreach (FieldInfo fieldInfo in type.Resolver.GetFieldsOnDeclaredTypeOnly(new Token(type.MetadataToken), type.GenericContext))
			{
				bool flag = Utility.IsBindingFlagsMatching(fieldInfo, false, flags);
				if (flag)
				{
					list.Add(fieldInfo);
				}
			}
			bool flag2 = MetadataOnlyModule.WalkInheritanceChain(flags) && type.BaseType != null;
			if (flag2)
			{
				FieldInfo[] fields = type.BaseType.GetFields(flags);
				List<FieldInfo> list2 = new List<FieldInfo>();
				foreach (FieldInfo fieldInfo2 in fields)
				{
					bool flag3 = MetadataOnlyModule.IncludeInheritedField(fieldInfo2, flags);
					if (flag3)
					{
						list2.Add(fieldInfo2);
					}
				}
				list.AddRange(list2);
			}
			return list.ToArray();
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x00009064 File Offset: 0x00007264
		private IEnumerable<FieldInfo> GetFieldsOnDeclaredTypeOnly(Token typeDefToken, GenericContext context)
		{
			HCORENUM hEnum = default(HCORENUM);
			IMetadataImport import = this.RawImport;
			Type[] typeArgs = Type.EmptyTypes;
			Type[] methodArgs = Type.EmptyTypes;
			bool flag = context != null;
			if (flag)
			{
				typeArgs = context.TypeArgs;
				methodArgs = context.MethodArgs;
			}
			try
			{
				for (;;)
				{
					int fieldToken;
					uint size;
					import.EnumFields(ref hEnum, typeDefToken, out fieldToken, 1, out size);
					bool flag2 = size == 0U;
					if (flag2)
					{
						break;
					}
					FieldInfo fieldInfo = this.Factory.CreateField(this, new Token(fieldToken), typeArgs, methodArgs);
					yield return fieldInfo;
					fieldInfo = null;
				}
			}
			finally
			{
				hEnum.Close(import);
			}
			yield break;
			yield break;
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x00009084 File Offset: 0x00007284
		internal static PropertyInfo[] GetPropertiesOnType(MetadataOnlyCommonType type, BindingFlags flags)
		{
			MetadataOnlyModule.CheckBindingFlagsInMethod(flags, "GetPropertiesOnType");
			List<PropertyInfo> list = new List<PropertyInfo>();
			bool isInherited = false;
			foreach (PropertyInfo propertyInfo in type.GetDeclaredProperties())
			{
				bool isStatic = false;
				bool isPublic = false;
				MetadataOnlyModule.CheckIsStaticAndIsPublicOnProperty(propertyInfo, ref isStatic, ref isPublic);
				bool flag = Utility.IsBindingFlagsMatching(propertyInfo, isStatic, isPublic, isInherited, flags);
				if (flag)
				{
					list.Add(propertyInfo);
				}
			}
			bool flag2 = MetadataOnlyModule.WalkInheritanceChain(flags) && type.BaseType != null;
			if (flag2)
			{
				PropertyInfo[] properties = type.BaseType.GetProperties(flags);
				IList<PropertyInfo> collection = MetadataOnlyModule.FilterInheritedProperties(properties, list, flags);
				list.AddRange(collection);
			}
			return list.ToArray();
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x0000915C File Offset: 0x0000735C
		internal IEnumerable<PropertyInfo> GetPropertiesOnDeclaredTypeOnly(Token tokenTypeDef, GenericContext context)
		{
			HCORENUM hEnum = default(HCORENUM);
			IMetadataImport import = this.RawImport;
			try
			{
				for (;;)
				{
					int propertyToken;
					uint size;
					import.EnumProperties(ref hEnum, tokenTypeDef.Value, out propertyToken, 1, out size);
					bool flag = size == 0U;
					if (flag)
					{
						break;
					}
					PropertyInfo property = this.Factory.CreatePropertyInfo(this, new Token(propertyToken), context.TypeArgs, context.MethodArgs);
					yield return property;
					property = null;
				}
			}
			finally
			{
				hEnum.Close(import);
			}
			yield break;
			yield break;
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0000917C File Offset: 0x0000737C
		internal static EventInfo[] GetEventsOnType(MetadataOnlyCommonType type, BindingFlags flags)
		{
			MetadataOnlyModule.CheckBindingFlagsInMethod(flags, "GetEventsOnType");
			List<EventInfo> list = new List<EventInfo>();
			foreach (EventInfo eventInfo in type.Resolver.GetEventsOnDeclaredTypeOnly(new Token(type.MetadataToken), type.GenericContext))
			{
				bool isStatic = false;
				bool isPublic = false;
				MetadataOnlyModule.CheckIsStaticAndIsPublicOnEvent(eventInfo, ref isStatic, ref isPublic);
				bool flag = Utility.IsBindingFlagsMatching(eventInfo, isStatic, isPublic, false, flags);
				if (flag)
				{
					list.Add(eventInfo);
				}
			}
			bool flag2 = MetadataOnlyModule.WalkInheritanceChain(flags) && type.BaseType != null;
			if (flag2)
			{
				EventInfo[] events = type.BaseType.GetEvents(flags);
				IList<EventInfo> collection = MetadataOnlyModule.FilterInheritedEvents(events, list);
				list.AddRange(collection);
			}
			return list.ToArray();
		}

		// Token: 0x060002BB RID: 699 RVA: 0x00009264 File Offset: 0x00007464
		private IEnumerable<EventInfo> GetEventsOnDeclaredTypeOnly(Token tokenTypeDef, GenericContext context)
		{
			HCORENUM hEnum = default(HCORENUM);
			IMetadataImport import = this.RawImport;
			try
			{
				for (;;)
				{
					int eventToken;
					uint size;
					import.EnumEvents(ref hEnum, tokenTypeDef.Value, out eventToken, 1, out size);
					bool flag = size == 0U;
					if (flag)
					{
						break;
					}
					EventInfo eventInfo = this.Factory.CreateEventInfo(this, new Token(eventToken), context.TypeArgs, context.MethodArgs);
					yield return eventInfo;
					eventInfo = null;
				}
			}
			finally
			{
				hEnum.Close(import);
			}
			yield break;
			yield break;
		}

		// Token: 0x060002BC RID: 700 RVA: 0x00009284 File Offset: 0x00007484
		internal IEnumerable<Type> GetNestedTypesOnType(MetadataOnlyCommonType type, BindingFlags flags)
		{
			return this.GetNestedTypesOnType(new Token(type.MetadataToken), flags);
		}

		// Token: 0x060002BD RID: 701 RVA: 0x000092A8 File Offset: 0x000074A8
		private void EnsureNestedTypeCacheExists()
		{
			bool flag = this._nestedTypeInfo == null;
			if (flag)
			{
				this._nestedTypeInfo = new MetadataOnlyModule.NestedTypeCache(this);
			}
		}

		// Token: 0x060002BE RID: 702 RVA: 0x000092D1 File Offset: 0x000074D1
		internal IEnumerable<Type> GetNestedTypesOnType(Token tokenTypeDef, BindingFlags flags)
		{
			MetadataOnlyModule.CheckBindingFlagsInMethod(flags, "GetNestedTypesOnType");
			this.EnsureNestedTypeCacheExists();
			IEnumerable<int> e = this._nestedTypeInfo.GetNestedTokens(tokenTypeDef);
			bool flag = e != null;
			if (flag)
			{
				foreach (int typeToken in e)
				{
					Type type = this.ResolveType(typeToken);
					bool isPublic = type.IsPublic || type.IsNestedPublic;
					bool flag2 = Utility.IsBindingFlagsMatching(type, false, isPublic, false, flags);
					if (flag2)
					{
						yield return type;
					}
					type = null;
				}
				IEnumerator<int> enumerator = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x060002BF RID: 703 RVA: 0x000092F0 File Offset: 0x000074F0
		public IList<CustomAttributeData> GetCustomAttributeData(int memberTokenValue)
		{
			List<CustomAttributeData> list = new List<CustomAttributeData>();
			HCORENUM hcorenum = default(HCORENUM);
			IMetadataImport rawImport = this.RawImport;
			try
			{
				for (;;)
				{
					Token token;
					uint num;
					rawImport.EnumCustomAttributes(ref hcorenum, memberTokenValue, 0, out token, 1U, out num);
					bool flag = num == 0U;
					if (flag)
					{
						break;
					}
					Token token2;
					Token customAttributeConstructorTokenValue;
					EmbeddedBlobPointer embeddedBlobPointer;
					int num2;
					rawImport.GetCustomAttributeProps(token, out token2, out customAttributeConstructorTokenValue, out embeddedBlobPointer, out num2);
					ConstructorInfo ctor = this.ResolveCustomAttributeConstructor(customAttributeConstructorTokenValue);
					CustomAttributeData item = new MetadataOnlyCustomAttributeData(this, token, ctor);
					list.Add(item);
				}
			}
			finally
			{
				hcorenum.Close(rawImport);
			}
			IEnumerable<CustomAttributeData> pseudoCustomAttributes = this.Policy.GetPseudoCustomAttributes(this, new Token(memberTokenValue));
			list.AddRange(pseudoCustomAttributes);
			return list;
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x000093B0 File Offset: 0x000075B0
		private ConstructorInfo ResolveCustomAttributeConstructor(Token customAttributeConstructorTokenValue)
		{
			Token token = customAttributeConstructorTokenValue;
			this.EnsureValidToken(token);
			bool flag = token.IsType(TokenType.MethodDef);
			ConstructorInfo result;
			if (flag)
			{
				MethodBase methodBase = this.ResolveMethodDef(token);
				result = (ConstructorInfo)methodBase;
			}
			else
			{
				bool flag2 = token.IsType(TokenType.MemberRef);
				if (!flag2)
				{
					throw new ArgumentException(Resources.MethodTokenExpected);
				}
				Token token2;
				string text;
				SignatureBlob signatureBlob;
				this.GetMemberRefData(token, out token2, out text, out signatureBlob);
				Type declaringType = this.ResolveTypeTokenInternal(token2, null);
				result = new ConstructorInfoRef(declaringType, this, token);
			}
			return result;
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x00009430 File Offset: 0x00007630
		internal void LazyAttributeParse(Token token, ConstructorInfo constructorInfo, out IList<CustomAttributeTypedArgument> constructorArguments, out IList<CustomAttributeNamedArgument> namedArguments)
		{
			IMetadataImport rawImport = this.RawImport;
			Token token2;
			Token token3;
			EmbeddedBlobPointer pointer;
			int countBytes;
			rawImport.GetCustomAttributeProps(token, out token2, out token3, out pointer, out countBytes);
			byte[] array = this.RawMetadata.ReadEmbeddedBlob(pointer, countBytes);
			int num = 0;
			bool flag = BitConverter.ToInt16(array, num) != 1;
			if (flag)
			{
				throw new ArgumentException(Resources.InvalidCustomAttributeFormat);
			}
			num += 2;
			constructorArguments = this.GetConstructorArguments(constructorInfo, array, ref num);
			namedArguments = this.GetNamedArguments(constructorInfo, array, ref num);
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x000094AC File Offset: 0x000076AC
		private IList<CustomAttributeTypedArgument> GetConstructorArguments(ConstructorInfo constructorInfo, byte[] customAttributeBlob, ref int index)
		{
			ParameterInfo[] parameters = constructorInfo.GetParameters();
			IList<CustomAttributeTypedArgument> list = new List<CustomAttributeTypedArgument>(parameters.Length);
			for (int i = 0; i < parameters.Length; i++)
			{
				Type parameterType = parameters[i].ParameterType;
				CorElementType typeId = SignatureUtil.GetTypeId(parameterType);
				Type type = null;
				bool flag = typeId != CorElementType.Object;
				object customAttributeArgumentValue;
				if (flag)
				{
					customAttributeArgumentValue = this.GetCustomAttributeArgumentValue(typeId, parameterType, customAttributeBlob, ref index);
					type = parameterType;
				}
				else
				{
					CorElementType typeId2;
					SignatureUtil.ExtractCustomAttributeArgumentType(this.AssemblyResolver, this, customAttributeBlob, ref index, out typeId2, out type);
					customAttributeArgumentValue = this.GetCustomAttributeArgumentValue(typeId2, type, customAttributeBlob, ref index);
				}
				CustomAttributeTypedArgument item = new CustomAttributeTypedArgument(type, customAttributeArgumentValue);
				list.Add(item);
			}
			return list;
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x0000955C File Offset: 0x0000775C
		private IList<CustomAttributeNamedArgument> GetNamedArguments(ConstructorInfo constructorInfo, byte[] customAttributeBlob, ref int index)
		{
			ushort num = BitConverter.ToUInt16(customAttributeBlob, index);
			index += 2;
			IList<CustomAttributeNamedArgument> list = new List<CustomAttributeNamedArgument>((int)num);
			bool flag = num == 0 && index != customAttributeBlob.Length;
			if (flag)
			{
				throw new ArgumentException(Resources.InvalidCustomAttributeFormat);
			}
			for (int i = 0; i < (int)num; i++)
			{
				NamedArgumentType namedArgumentType = SignatureUtil.ExtractNamedArgumentType(customAttributeBlob, ref index);
				CorElementType typeId;
				Type type;
				SignatureUtil.ExtractCustomAttributeArgumentType(this.AssemblyResolver, this, customAttributeBlob, ref index, out typeId, out type);
				string name = SignatureUtil.ExtractStringValue(customAttributeBlob, ref index);
				bool flag2 = type == null;
				if (flag2)
				{
					SignatureUtil.ExtractCustomAttributeArgumentType(this.AssemblyResolver, this, customAttributeBlob, ref index, out typeId, out type);
				}
				object customAttributeArgumentValue = this.GetCustomAttributeArgumentValue(typeId, type, customAttributeBlob, ref index);
				bool flag3 = namedArgumentType == NamedArgumentType.Field;
				MemberInfo memberInfo;
				if (flag3)
				{
					memberInfo = constructorInfo.DeclaringType.GetField(name, BindingFlags.Instance | BindingFlags.Public);
				}
				else
				{
					memberInfo = constructorInfo.DeclaringType.GetProperty(name);
				}
				CustomAttributeTypedArgument typedValue = new CustomAttributeTypedArgument(type, customAttributeArgumentValue);
				CustomAttributeNamedArgument item = new CustomAttributeNamedArgument(memberInfo, typedValue);
				list.Add(item);
			}
			bool flag4 = index != customAttributeBlob.Length;
			if (flag4)
			{
				throw new ArgumentException(Resources.InvalidCustomAttributeFormat);
			}
			return list;
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x00009680 File Offset: 0x00007880
		private object GetCustomAttributeArgumentValue(CorElementType typeId, Type type, byte[] customAttributeBlob, ref int index)
		{
			object result = null;
			if (typeId != CorElementType.SzArray)
			{
				if (typeId != CorElementType.Type)
				{
					if (typeId != CorElementType.Enum)
					{
						result = SignatureUtil.ExtractValue(typeId, customAttributeBlob, ref index);
					}
					else
					{
						Type underlyingType = MetadataOnlyModule.GetUnderlyingType(type);
						CorElementType typeId2 = SignatureUtil.GetTypeId(underlyingType);
						result = SignatureUtil.ExtractValue(typeId2, customAttributeBlob, ref index);
					}
				}
				else
				{
					result = SignatureUtil.ExtractTypeValue(this.AssemblyResolver, this, customAttributeBlob, ref index);
				}
			}
			else
			{
				uint num = SignatureUtil.ExtractUIntValue(customAttributeBlob, ref index);
				bool flag = num != uint.MaxValue;
				if (flag)
				{
					result = SignatureUtil.ExtractListOfValues(type.GetElementType(), this.AssemblyResolver, this, num, customAttributeBlob, ref index);
				}
			}
			return result;
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x0000971C File Offset: 0x0000791C
		internal static Type GetUnderlyingType(Type enumType)
		{
			FieldInfo[] fields = enumType.GetFields(BindingFlags.Instance | BindingFlags.Public);
			return fields[0].FieldType;
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x00009740 File Offset: 0x00007940
		internal Type GetEnclosingType(Token tokenTypeDef)
		{
			Token token = new Token(this.GetNestedClassProps(tokenTypeDef));
			bool isNil = token.IsNil;
			Type result;
			if (isNil)
			{
				result = null;
			}
			else
			{
				result = this.ResolveTypeTokenInternal(token, null);
			}
			return result;
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x00009780 File Offset: 0x00007980
		public AssemblyName GetAssemblyNameFromAssemblyRef(Token assemblyRefToken)
		{
			IMetadataAssemblyImport assemblyImport = (IMetadataAssemblyImport)this.RawImport;
			return AssemblyNameHelper.GetAssemblyNameFromRef(assemblyRefToken, this, assemblyImport);
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x000097A8 File Offset: 0x000079A8
		internal Token GetNestedClassProps(Token tokenTypeDef)
		{
			int value;
			int nestedClassProps = this.RawImport.GetNestedClassProps(tokenTypeDef, out value);
			bool flag = nestedClassProps == 0;
			Token result;
			if (flag)
			{
				result = new Token(value);
			}
			else
			{
				bool flag2 = nestedClassProps == -2146234064;
				if (!flag2)
				{
					throw Marshal.GetExceptionForHR(nestedClassProps);
				}
				result = new Token(0);
			}
			return result;
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x000097FC File Offset: 0x000079FC
		internal int CountGenericParams(Token token)
		{
			IMetadataImport2 metadataImport = this.RawImport as IMetadataImport2;
			bool flag = metadataImport == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				HCORENUM hEnum = default(HCORENUM);
				int num;
				uint num2;
				metadataImport.EnumGenericParams(ref hEnum, token.Value, out num, 1U, out num2);
				int num3;
				try
				{
					metadataImport.CountEnum(hEnum, out num3);
				}
				finally
				{
					hEnum.Close(metadataImport);
				}
				result = num3;
			}
			return result;
		}

		// Token: 0x060002CA RID: 714 RVA: 0x00009878 File Offset: 0x00007A78
		internal IEnumerable<int> GetGenericParameterTokens(int typeOrMethodToken)
		{
			Token token = new Token(typeOrMethodToken);
			IMetadataImport2 importer2 = this.RawImport as IMetadataImport2;
			bool flag = importer2 == null;
			if (flag)
			{
				yield break;
			}
			HCORENUM hEnum = default(HCORENUM);
			try
			{
				for (;;)
				{
					int mdGenericParam;
					uint count;
					importer2.EnumGenericParams(ref hEnum, typeOrMethodToken, out mdGenericParam, 1U, out count);
					bool flag2 = count != 1U;
					if (flag2)
					{
						break;
					}
					yield return mdGenericParam;
				}
			}
			finally
			{
				hEnum.Close(importer2);
			}
			yield break;
			yield break;
		}

		// Token: 0x060002CB RID: 715 RVA: 0x0000988F File Offset: 0x00007A8F
		internal IEnumerable<Type> GetConstraintTypes(int gpToken)
		{
			Token token = new Token(gpToken);
			IMetadataImport2 importer2 = this.RawImport as IMetadataImport2;
			bool flag = importer2 == null;
			if (flag)
			{
				yield break;
			}
			HCORENUM hEnum = default(HCORENUM);
			try
			{
				for (;;)
				{
					int mdGenericConstraint;
					uint count;
					importer2.EnumGenericParamConstraints(ref hEnum, gpToken, out mdGenericConstraint, 1U, out count);
					bool flag2 = count != 1U;
					if (flag2)
					{
						break;
					}
					int ownerParam;
					int constraintTypeToken;
					importer2.GetGenericParamConstraintProps(mdGenericConstraint, out ownerParam, out constraintTypeToken);
					yield return this.ResolveTypeTokenInternal(new Token(constraintTypeToken), null);
				}
			}
			finally
			{
				hEnum.Close(importer2);
			}
			yield break;
			yield break;
		}

		// Token: 0x060002CC RID: 716 RVA: 0x000098A8 File Offset: 0x00007AA8
		internal void GetGenericParameterProps(int mdGenericParam, out int ownerTypeToken, out int ownerMethodToken, out string name, out GenericParameterAttributes attributes, out uint genIndex)
		{
			IMetadataImport2 metadataImport = this.RawImport as IMetadataImport2;
			HCORENUM hcorenum = default(HCORENUM);
			try
			{
				int num;
				int num2;
				int num3;
				uint capacity;
				metadataImport.GetGenericParamProps(mdGenericParam, out genIndex, out num, out num2, out num3, null, 0U, out capacity);
				attributes = (GenericParameterAttributes)num;
				StringBuilder stringBuilder = StringBuilderPool.Get((int)capacity);
				metadataImport.GetGenericParamProps(mdGenericParam, out genIndex, out num, out num2, out num3, stringBuilder, (uint)stringBuilder.Capacity, out capacity);
				name = stringBuilder.ToString();
				StringBuilderPool.Release(ref stringBuilder);
				Token token = new Token(num2);
				bool flag = token.IsType(TokenType.MethodDef);
				if (flag)
				{
					ownerMethodToken = num2;
					ownerTypeToken = 0;
				}
				else
				{
					ownerTypeToken = num2;
					ownerMethodToken = 0;
				}
			}
			finally
			{
				hcorenum.Close(metadataImport);
			}
		}

		// Token: 0x060002CD RID: 717 RVA: 0x00009964 File Offset: 0x00007B64
		internal IEnumerable<Type> GetInterfacesOnType(Type type)
		{
			IMetadataImport import = this.RawImport;
			bool isGenericParameter = type.IsGenericParameter;
			if (isGenericParameter)
			{
				foreach (Type c in this.GetConstraintTypes(type.MetadataToken))
				{
					bool isInterface = c.IsInterface;
					if (isInterface)
					{
						yield return c;
					}
					c = null;
				}
				IEnumerator<Type> enumerator = null;
			}
			else
			{
				HCORENUM hEnum = default(HCORENUM);
				int cImpls = 1;
				for (;;)
				{
					int rImpls;
					import.EnumInterfaceImpls(ref hEnum, type.MetadataToken, out rImpls, 1, ref cImpls);
					bool flag = cImpls != 1;
					if (flag)
					{
						break;
					}
					Token tImpl = new Token(rImpls);
					int cls;
					int iface;
					import.GetInterfaceImplProps(tImpl.Value, out cls, out iface);
					Token tkClass = new Token(cls);
					Token tkInterface = new Token(iface);
					Type result = this.ResolveTypeTokenInternal(tkInterface, new GenericContext(type.GetGenericArguments(), null));
					yield return result;
					tImpl = default(Token);
					tkClass = default(Token);
					tkInterface = default(Token);
					result = null;
				}
				hEnum.Close(import);
			}
			yield break;
			yield break;
		}

		// Token: 0x060002CE RID: 718 RVA: 0x0000997C File Offset: 0x00007B7C
		public static Type GetInterfaceHelper(Type[] interfaces, string name, bool ignoreCase)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			Type type = null;
			foreach (Type type2 in interfaces)
			{
				bool flag2 = Utility.Compare(name, type2.Name, ignoreCase);
				bool flag3 = flag2;
				if (flag3)
				{
					bool flag4 = type != null;
					if (flag4)
					{
						throw new AmbiguousMatchException();
					}
					type = type2;
				}
			}
			return type;
		}

		// Token: 0x060002CF RID: 719 RVA: 0x000099F2 File Offset: 0x00007BF2
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public IEnumerable<Type> GetTypeList()
		{
			foreach (int typeToken in this.GetTypeTokenList())
			{
				Type result = this.ResolveTypeTokenInternal(new Token(typeToken), null);
				yield return result;
				result = null;
			}
			IEnumerator<int> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x00009A02 File Offset: 0x00007C02
		private IEnumerable<int> GetTypeTokenList()
		{
			IMetadataImport import = this.RawImport;
			HCORENUM hEnum = default(HCORENUM);
			try
			{
				uint count = 1U;
				for (;;)
				{
					int rTypeDefs;
					import.EnumTypeDefs(ref hEnum, out rTypeDefs, 1U, out count);
					bool flag = count != 1U;
					if (flag)
					{
						break;
					}
					yield return rTypeDefs;
				}
			}
			finally
			{
				hEnum.Close(import);
			}
			yield break;
			yield break;
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x00009A14 File Offset: 0x00007C14
		private static void CheckBindingFlagsInMethod(BindingFlags flags, string methodName)
		{
			bool flag = (flags | (BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.ExactBinding)) != (BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.ExactBinding);
			if (flag)
			{
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, Resources.MethodIsUsingUnsupportedBindingFlags, new object[]
				{
					methodName,
					flags
				}));
			}
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x00009A60 File Offset: 0x00007C60
		private static void CheckIsStaticAndIsPublicOnProperty(PropertyInfo propertyInfo, ref bool isStatic, ref bool isPublic)
		{
			bool nonPublic = true;
			MethodInfo getMethod = propertyInfo.GetGetMethod(nonPublic);
			MetadataOnlyModule.CheckIsStaticAndIsPublic(getMethod, ref isStatic, ref isPublic);
			MethodInfo setMethod = propertyInfo.GetSetMethod(nonPublic);
			MetadataOnlyModule.CheckIsStaticAndIsPublic(setMethod, ref isStatic, ref isPublic);
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x00009A94 File Offset: 0x00007C94
		private static void CheckIsStaticAndIsPublicOnEvent(EventInfo eventInfo, ref bool isStatic, ref bool isPublic)
		{
			bool nonPublic = true;
			MethodInfo addMethod = eventInfo.GetAddMethod(nonPublic);
			MetadataOnlyModule.CheckIsStaticAndIsPublic(addMethod, ref isStatic, ref isPublic);
			MethodInfo removeMethod = eventInfo.GetRemoveMethod(nonPublic);
			MetadataOnlyModule.CheckIsStaticAndIsPublic(removeMethod, ref isStatic, ref isPublic);
			MethodInfo raiseMethod = eventInfo.GetRaiseMethod(nonPublic);
			MetadataOnlyModule.CheckIsStaticAndIsPublic(raiseMethod, ref isStatic, ref isPublic);
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x00009AD8 File Offset: 0x00007CD8
		private static void CheckIsStaticAndIsPublic(MethodInfo methodInfo, ref bool isStatic, ref bool isPublic)
		{
			bool flag = methodInfo == null;
			if (!flag)
			{
				bool isStatic2 = methodInfo.IsStatic;
				if (isStatic2)
				{
					isStatic = true;
				}
				bool isPublic2 = methodInfo.IsPublic;
				if (isPublic2)
				{
					isPublic = true;
				}
			}
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x00009B0F File Offset: 0x00007D0F
		internal void SetContainingAssembly(Assembly assembly)
		{
			this._assembly = assembly;
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060002D6 RID: 726 RVA: 0x00009B1C File Offset: 0x00007D1C
		public override Assembly Assembly
		{
			get
			{
				return this._assembly;
			}
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x00009B34 File Offset: 0x00007D34
		public override Type GetType(string className, bool throwOnError, bool ignoreCase)
		{
			if (ignoreCase)
			{
				throw new NotImplementedException(Resources.CaseInsensitiveTypeLookupNotImplemented);
			}
			bool flag = className == null;
			if (flag)
			{
				throw new ArgumentNullException("className");
			}
			bool flag2 = TypeNameParser.IsCompoundType(className);
			Type result;
			if (flag2)
			{
				result = TypeNameParser.ParseTypeName(this.AssemblyResolver, this, className, throwOnError);
			}
			else
			{
				Token token = this.FindTypeDefByName(null, className, false);
				bool isNil = token.IsNil;
				if (isNil)
				{
					if (throwOnError)
					{
						throw new TypeLoadException(string.Format(CultureInfo.InvariantCulture, Resources.CannotFindTypeInModule, new object[]
						{
							className,
							this.ToString()
						}));
					}
					result = null;
				}
				else
				{
					Type type = base.ResolveType(token.Value);
					result = type;
				}
			}
			return result;
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x00009BE8 File Offset: 0x00007DE8
		public override Type[] GetTypes()
		{
			List<Type> list = new List<Type>(this.GetTypeList());
			return list.ToArray();
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x00009C0C File Offset: 0x00007E0C
		public override Type[] FindTypes(TypeFilter filter, object filterCriteria)
		{
			List<Type> list = new List<Type>();
			foreach (Type type in this.GetTypeList())
			{
				bool flag = filter(type, filterCriteria);
				if (flag)
				{
					list.Add(type);
				}
			}
			return list.ToArray();
		}

		// Token: 0x060002DA RID: 730 RVA: 0x00009C80 File Offset: 0x00007E80
		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			FieldInfo[] fields = this.GetFields(bindingAttr);
			foreach (FieldInfo fieldInfo in fields)
			{
				bool flag2 = fieldInfo.Name.Equals(name);
				if (flag2)
				{
					return fieldInfo;
				}
			}
			return null;
		}

		// Token: 0x060002DB RID: 731 RVA: 0x00009CE4 File Offset: 0x00007EE4
		public override FieldInfo[] GetFields(BindingFlags bindingFlags)
		{
			MetadataOnlyModule.CheckBindingFlagsInMethod(bindingFlags, "GetFields");
			IMetadataImport rawImport = this.RawImport;
			HCORENUM hcorenum = default(HCORENUM);
			List<FieldInfo> list = new List<FieldInfo>();
			try
			{
				uint num = 1U;
				for (;;)
				{
					int metadataToken;
					rawImport.EnumFields(ref hcorenum, this.MetadataToken, out metadataToken, 1, out num);
					bool flag = num != 1U;
					if (flag)
					{
						break;
					}
					FieldInfo fieldInfo = base.ResolveField(metadataToken);
					bool flag2 = Utility.IsBindingFlagsMatching(fieldInfo, false, bindingFlags);
					if (flag2)
					{
						list.Add(fieldInfo);
					}
				}
			}
			finally
			{
				hcorenum.Close(rawImport);
			}
			return list.ToArray();
		}

		// Token: 0x060002DC RID: 732 RVA: 0x00009D90 File Offset: 0x00007F90
		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			MetadataOnlyModule.CheckBinderAndModifiersforLMR(binder, modifiers);
			MethodInfo[] methods = this.GetMethods(bindingAttr);
			return MetadataOnlyModule.FilterMethod(methods, name, bindingAttr, callConvention, types);
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00009DC0 File Offset: 0x00007FC0
		public override MethodInfo[] GetMethods(BindingFlags bindingFlags)
		{
			MetadataOnlyModule.CheckBindingFlagsInMethod(bindingFlags, "GetMethods");
			IMetadataImport rawImport = this.RawImport;
			HCORENUM hcorenum = default(HCORENUM);
			List<MethodInfo> list = new List<MethodInfo>();
			try
			{
				int num = 1;
				for (;;)
				{
					int value;
					rawImport.EnumMethods(ref hcorenum, this.MetadataToken, out value, 1, out num);
					bool flag = num != 1;
					if (flag)
					{
						break;
					}
					MethodBase methodBase = this.ResolveMethodTokenInternal(new Token(value), null);
					bool flag2 = Utility.IsBindingFlagsMatching(methodBase, false, bindingFlags);
					if (flag2)
					{
						MethodInfo methodInfo = methodBase as MethodInfo;
						bool flag3 = methodInfo != null;
						if (flag3)
						{
							list.Add(methodInfo);
						}
					}
				}
			}
			finally
			{
				hcorenum.Close(rawImport);
			}
			return list.ToArray();
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060002DE RID: 734 RVA: 0x00009E88 File Offset: 0x00008088
		public override int MetadataToken
		{
			get
			{
				int result;
				this.RawImport.GetModuleFromScope(out result);
				return result;
			}
		}

		// Token: 0x060002DF RID: 735 RVA: 0x00009EAC File Offset: 0x000080AC
		public override bool IsResource()
		{
			return false;
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x00009EC0 File Offset: 0x000080C0
		public override Type ResolveType(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			Type type = this.ResolveTypeTokenInternal(new Token(metadataToken), new GenericContext(genericTypeArguments, genericMethodArguments));
			Helpers.EnsureResolve(type);
			return type;
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x00009EF0 File Offset: 0x000080F0
		public override FieldInfo ResolveField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			return this.ResolveFieldTokenInternal(new Token(metadataToken), new GenericContext(genericTypeArguments, genericMethodArguments));
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x00009F18 File Offset: 0x00008118
		public override MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			return this.ResolveMethodTokenInternal(new Token(metadataToken), new GenericContext(genericTypeArguments, genericMethodArguments));
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x0000232A File Offset: 0x0000052A
		public override MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x0000232A File Offset: 0x0000052A
		public override byte[] ResolveSignature(int metadataToken)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x00009F40 File Offset: 0x00008140
		internal bool IsSystemModule()
		{
			ITypeUniverse assemblyResolver = this.AssemblyResolver;
			return assemblyResolver.GetSystemAssembly().Equals(this.Assembly);
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x00009F6C File Offset: 0x0000816C
		internal TypeCode GetTypeCode(Type type)
		{
			bool isEnum = type.IsEnum;
			TypeCode result;
			if (isEnum)
			{
				type = MetadataOnlyModule.GetUnderlyingType(type);
				result = Type.GetTypeCode(type);
			}
			else
			{
				bool flag = !this.IsSystemModule();
				if (flag)
				{
					result = TypeCode.Object;
				}
				else
				{
					Token token = new Token(type.MetadataToken);
					bool flag2 = this._typeCodeMapping == null;
					if (flag2)
					{
						this._typeCodeMapping = this.CreateTypeCodeMapping();
					}
					for (int i = 0; i < this._typeCodeMapping.Length; i++)
					{
						bool flag3 = token == this._typeCodeMapping[i];
						if (flag3)
						{
							return (TypeCode)i;
						}
					}
					result = TypeCode.Object;
				}
			}
			return result;
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x0000A014 File Offset: 0x00008214
		private Token[] CreateTypeCodeMapping()
		{
			return new Token[]
			{
				default(Token),
				this.LookupTypeToken("System.Object"),
				this.LookupTypeToken("System.DBNull"),
				this.LookupTypeToken("System.Boolean"),
				this.LookupTypeToken("System.Char"),
				this.LookupTypeToken("System.SByte"),
				this.LookupTypeToken("System.Byte"),
				this.LookupTypeToken("System.Int16"),
				this.LookupTypeToken("System.UInt16"),
				this.LookupTypeToken("System.Int32"),
				this.LookupTypeToken("System.UInt32"),
				this.LookupTypeToken("System.Int64"),
				this.LookupTypeToken("System.UInt64"),
				this.LookupTypeToken("System.Single"),
				this.LookupTypeToken("System.Double"),
				this.LookupTypeToken("System.Decimal"),
				this.LookupTypeToken("System.DateTime"),
				default(Token),
				this.LookupTypeToken("System.String")
			};
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x0000A168 File Offset: 0x00008368
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x0000A17C File Offset: 0x0000837C
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (IMetadataImport o in this._cachedThreadAffinityImporter.Values)
				{
					int num = Marshal.ReleaseComObject(o);
				}
				this._cachedThreadAffinityImporter.Clear();
				bool flag = this.RawMetadata != null;
				if (flag)
				{
					this.RawMetadata.Dispose();
				}
			}
		}

		// Token: 0x060002EA RID: 746 RVA: 0x0000A20C File Offset: 0x0000840C
		public int RowCount(MetadataTable metadataTableIndex)
		{
			IMetadataTables metadataTables = (IMetadataTables)this.RawImport;
			int num;
			int result;
			UnusedIntPtr unusedIntPtr;
			metadataTables.GetTableInfo(metadataTableIndex, out num, out result, out num, out num, out unusedIntPtr);
			return result;
		}

		// Token: 0x060002EB RID: 747 RVA: 0x0000A240 File Offset: 0x00008440
		public override void GetPEKind(out PortableExecutableKinds peKind, out ImageFileMachine machine)
		{
			IMetadataImport2 metadataImport = (IMetadataImport2)this.RawImport;
			metadataImport.GetPEKind(out peKind, out machine);
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060002EC RID: 748 RVA: 0x0000232A File Offset: 0x0000052A
		public override int MDStreamVersion
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x0400008B RID: 139
		private readonly string _modulePath;

		// Token: 0x0400008C RID: 140
		private readonly Dictionary<Thread, IMetadataImport> _cachedThreadAffinityImporter;

		// Token: 0x0400008D RID: 141
		private readonly object _lock = new object();

		// Token: 0x0400008E RID: 142
		private string _scopeName;

		// Token: 0x0400008F RID: 143
		private Token[] _typeCodeMapping;

		// Token: 0x04000094 RID: 148
		private MetadataOnlyModule.NestedTypeCache _nestedTypeInfo;

		// Token: 0x04000095 RID: 149
		private Assembly _assembly;

		// Token: 0x04000096 RID: 150
		private const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

		// Token: 0x04000097 RID: 151
		private const BindingFlags MembersDeclaredOnTypeOnly = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		// Token: 0x02000057 RID: 87
		internal enum EMethodKind
		{
			// Token: 0x0400011C RID: 284
			Constructor,
			// Token: 0x0400011D RID: 285
			Methods
		}

		// Token: 0x02000058 RID: 88
		private class NestedTypeCache
		{
			// Token: 0x060004EA RID: 1258 RVA: 0x0000FD2C File Offset: 0x0000DF2C
			public NestedTypeCache(MetadataOnlyModule outer)
			{
				this._cache = new Dictionary<int, List<int>>();
				IEnumerable<int> typeTokenList = outer.GetTypeTokenList();
				foreach (int num in typeTokenList)
				{
					int num2 = outer.GetNestedClassProps(new Token(num));
					bool flag = num2 == 0;
					if (!flag)
					{
						bool flag2 = this._cache.ContainsKey(num2);
						if (flag2)
						{
							this._cache[num2].Add(num);
						}
						else
						{
							List<int> list = new List<int>();
							list.Add(num);
							this._cache.Add(num2, list);
						}
					}
				}
			}

			// Token: 0x060004EB RID: 1259 RVA: 0x0000FDF4 File Offset: 0x0000DFF4
			public IEnumerable<int> GetNestedTokens(Token tokenTypeDef)
			{
				List<int> list;
				bool flag = this._cache.TryGetValue(tokenTypeDef, out list);
				IEnumerable<int> result;
				if (flag)
				{
					result = list;
				}
				else
				{
					result = null;
				}
				return result;
			}

			// Token: 0x0400011E RID: 286
			private readonly Dictionary<int, List<int>> _cache;
		}
	}
}
