using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;
using System.Text;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200002A RID: 42
	internal class MetadataOnlyMethodInfo : MethodInfo
	{
		// Token: 0x06000220 RID: 544 RVA: 0x000062C8 File Offset: 0x000044C8
		internal static MethodBase Create(MetadataOnlyModule resolver, Token methodDef, GenericContext context)
		{
			Type[] typeArgs = Type.EmptyTypes;
			Type[] methodArgs = Type.EmptyTypes;
			bool flag = context != null;
			if (flag)
			{
				typeArgs = context.TypeArgs;
				methodArgs = context.MethodArgs;
			}
			return resolver.Factory.CreateMethodOrConstructor(resolver, methodDef, typeArgs, methodArgs);
		}

		// Token: 0x06000221 RID: 545 RVA: 0x00006314 File Offset: 0x00004514
		public MetadataOnlyMethodInfo(MetadataOnlyMethodInfo method)
		{
			this.Resolver = method.Resolver;
			this._methodDef = method._methodDef;
			this._tOwner = method._tOwner;
			this._descriptor = method._descriptor;
			this._name = method._name;
			this._nameLength = method._nameLength;
			this._attrs = method._attrs;
			this._returnParameter = method._returnParameter;
			this._methodBody = method._methodBody;
			this._declaringTypeDef = method._declaringTypeDef;
			this._sigBlob = method._sigBlob;
			this._typeArgs = method._typeArgs;
			this._methodArgs = method._methodArgs;
			this._context = method._context;
			this._fullyInitialized = method._fullyInitialized;
		}

		// Token: 0x06000222 RID: 546 RVA: 0x000063E0 File Offset: 0x000045E0
		public MetadataOnlyMethodInfo(MetadataOnlyModule resolver, Token methodDef, Type[] typeArgs, Type[] methodArgs)
		{
			this.Resolver = resolver;
			this._methodDef = methodDef;
			this._typeArgs = typeArgs;
			this._methodArgs = methodArgs;
			resolver.GetMethodAttrs(methodDef, out this._declaringTypeDef, out this._attrs, out this._nameLength);
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000642C File Offset: 0x0000462C
		private void InitializeName()
		{
			bool flag = string.IsNullOrEmpty(this._name);
			if (flag)
			{
				this.Resolver.GetMethodName(this._methodDef, this._nameLength, out this._name);
			}
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0000646C File Offset: 0x0000466C
		private void Initialize()
		{
			Type tOwner = null;
			Type[] typeArgs = null;
			bool flag = !this._declaringTypeDef.IsNil;
			if (flag)
			{
				this.GetOwnerTypeAndTypeArgs(out tOwner, out typeArgs);
			}
			else
			{
				typeArgs = this._typeArgs;
			}
			Type[] genericMethodArgs = this.GetGenericMethodArgs();
			GenericContext context = new GenericContext(typeArgs, genericMethodArgs);
			this.Resolver.GetMethodSig(this._methodDef, out this._sigBlob);
			MethodSignatureDescriptor descriptor = SignatureUtil.ExtractMethodSignature(this._sigBlob, this.Resolver, context);
			this._tOwner = tOwner;
			this._context = context;
			this._descriptor = descriptor;
			this._fullyInitialized = true;
		}

		// Token: 0x06000225 RID: 549 RVA: 0x00006504 File Offset: 0x00004704
		private void GetOwnerTypeAndTypeArgs(out Type ownerType, out Type[] typeArgs)
		{
			Type type = this.Resolver.ResolveTypeDefToken(this._declaringTypeDef);
			GenericContext genericContext = new GenericContext(this._typeArgs, this._methodArgs);
			bool flag = type.IsGenericType && GenericContext.IsNullOrEmptyTypeArgs(genericContext);
			if (flag)
			{
				genericContext = new GenericContext(type.GetGenericArguments(), this._methodArgs);
			}
			type = this.Resolver.GetGenericType(new Token(type.MetadataToken), genericContext);
			ownerType = type;
			typeArgs = genericContext.TypeArgs;
		}

		// Token: 0x06000226 RID: 550 RVA: 0x00006584 File Offset: 0x00004784
		private Type[] GetGenericMethodArgs()
		{
			Type[] array = null;
			int num = this.Resolver.CountGenericParams(this._methodDef);
			bool flag = this._methodArgs != null && this._methodArgs.Length != 0;
			bool flag2 = num > 0;
			if (flag2)
			{
				bool flag3 = !flag;
				if (flag3)
				{
					array = new Type[num];
					int num2 = 0;
					foreach (int value in this.Resolver.GetGenericParameterTokens(this._methodDef))
					{
						array[num2++] = this.Resolver.Factory.CreateTypeVariable(this.Resolver, new Token(value));
					}
				}
				else
				{
					bool flag4 = num != this._methodArgs.Length;
					if (flag4)
					{
						throw new ArgumentException(Resources.WrongNumberOfGenericArguments);
					}
					array = this._methodArgs;
				}
			}
			return (array == null) ? Type.EmptyTypes : array;
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000227 RID: 551 RVA: 0x00006694 File Offset: 0x00004894
		public override int MetadataToken
		{
			get
			{
				return this._methodDef.Value;
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000228 RID: 552 RVA: 0x000066B4 File Offset: 0x000048B4
		internal MetadataOnlyModule Resolver { get; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000229 RID: 553 RVA: 0x000066BC File Offset: 0x000048BC
		public override Module Module
		{
			get
			{
				return this.Resolver;
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600022A RID: 554 RVA: 0x000066D4 File Offset: 0x000048D4
		public override Type ReturnType
		{
			get
			{
				bool flag = !this._fullyInitialized;
				if (flag)
				{
					this.Initialize();
				}
				return this._descriptor.ReturnParameter.Type;
			}
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000670C File Offset: 0x0000490C
		public override bool Equals(object obj)
		{
			MetadataOnlyMethodInfo metadataOnlyMethodInfo = obj as MetadataOnlyMethodInfo;
			bool flag = metadataOnlyMethodInfo == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !this.DeclaringType.Equals(metadataOnlyMethodInfo.DeclaringType);
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !this.IsGenericMethod;
					if (flag3)
					{
						result = (metadataOnlyMethodInfo.GetHashCode() == this.GetHashCode());
					}
					else
					{
						bool flag4 = !metadataOnlyMethodInfo.IsGenericMethod;
						if (flag4)
						{
							result = false;
						}
						else
						{
							Type[] genericArguments = this.GetGenericArguments();
							Type[] genericArguments2 = metadataOnlyMethodInfo.GetGenericArguments();
							bool flag5 = genericArguments.Length != genericArguments2.Length;
							if (flag5)
							{
								result = false;
							}
							else
							{
								for (int i = 0; i < genericArguments.Length; i++)
								{
									bool flag6 = !genericArguments[i].Equals(genericArguments2[i]);
									if (flag6)
									{
										return false;
									}
								}
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600022C RID: 556 RVA: 0x000067EC File Offset: 0x000049EC
		public override int GetHashCode()
		{
			return this.Resolver.GetHashCode() * 32767 + this._methodDef.GetHashCode();
		}

		// Token: 0x0600022D RID: 557 RVA: 0x00006824 File Offset: 0x00004A24
		public override string ToString()
		{
			return MetadataOnlyMethodInfo.CommonToString(this);
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0000683C File Offset: 0x00004A3C
		internal static string CommonToString(MethodInfo m)
		{
			StringBuilder stringBuilder = StringBuilderPool.Get();
			MetadataOnlyCommonType.TypeSigToString(m.ReturnType, stringBuilder);
			stringBuilder.Append(" ");
			MetadataOnlyMethodInfo.ConstructMethodString(m, stringBuilder);
			string result = stringBuilder.ToString();
			StringBuilderPool.Release(ref stringBuilder);
			return result;
		}

		// Token: 0x0600022F RID: 559 RVA: 0x00006888 File Offset: 0x00004A88
		private static void ConstructMethodString(MethodInfo m, StringBuilder sb)
		{
			sb.Append(m.Name);
			string value = "";
			bool isGenericMethod = m.IsGenericMethod;
			if (isGenericMethod)
			{
				sb.Append("[");
				foreach (Type pThis in m.GetGenericArguments())
				{
					sb.Append(value);
					MetadataOnlyCommonType.TypeSigToString(pThis, sb);
					value = ",";
				}
				sb.Append("]");
			}
			sb.Append("(");
			MetadataOnlyMethodInfo.ConstructParameters(sb, m.GetParameters(), m.CallingConvention);
			sb.Append(")");
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0000692C File Offset: 0x00004B2C
		private static void ConstructParameters(StringBuilder sb, ParameterInfo[] parameters, CallingConventions callingConvention)
		{
			Type[] array = new Type[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				array[i] = parameters[i].ParameterType;
			}
			MetadataOnlyMethodInfo.ConstructParameters(sb, array, callingConvention);
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000696C File Offset: 0x00004B6C
		private static void ConstructParameters(StringBuilder sb, Type[] parameters, CallingConventions callingConvention)
		{
			string value = "";
			foreach (Type type in parameters)
			{
				sb.Append(value);
				MetadataOnlyCommonType.TypeSigToString(type, sb);
				bool isByRef = type.IsByRef;
				if (isByRef)
				{
					int length = sb.Length;
					sb.Length = length - 1;
					sb.Append(" ByRef");
				}
				value = ", ";
			}
			bool flag = (callingConvention & CallingConventions.VarArgs) == CallingConventions.VarArgs;
			if (flag)
			{
				sb.Append(value);
				sb.Append("...");
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000232 RID: 562 RVA: 0x00006A00 File Offset: 0x00004C00
		public override Type DeclaringType
		{
			get
			{
				bool flag = !this._fullyInitialized;
				if (flag)
				{
					this.Initialize();
				}
				return this._tOwner;
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000233 RID: 563 RVA: 0x00006A30 File Offset: 0x00004C30
		public override string Name
		{
			get
			{
				this.InitializeName();
				return this._name;
			}
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000240B File Offset: 0x0000060B
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000237 RID: 567 RVA: 0x0000240B File Offset: 0x0000060B
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public override Type ReflectedType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x06000238 RID: 568 RVA: 0x00006A50 File Offset: 0x00004C50
		public override ParameterInfo[] GetParameters()
		{
			bool flag = !this._fullyInitialized;
			if (flag)
			{
				this.Initialize();
			}
			int num = this._descriptor.Parameters.Length;
			ParameterInfo[] array = new ParameterInfo[num];
			Type[] array2 = new Type[num];
			for (int i = 0; i < num; i++)
			{
				array2[i] = this._descriptor.Parameters[i].Type;
			}
			int[] array3 = new int[num + 1];
			IMetadataImport rawImport = this.Resolver.RawImport;
			HCORENUM hcorenum = default(HCORENUM);
			uint num3;
			int num2 = rawImport.EnumParams(ref hcorenum, this._methodDef.Value, array3, array3.Length, out num3);
			bool flag2 = num2 == 1;
			ParameterInfo[] result;
			if (flag2)
			{
				for (int j = 0; j < num; j++)
				{
					array[j] = this.Resolver.Policy.GetFakeParameterInfo(this, array2[j], j);
				}
				result = array;
			}
			else
			{
				hcorenum.Close(rawImport);
				bool flag3 = num3 == 0U;
				if (flag3)
				{
					result = array;
				}
				else
				{
					ParameterInfo parameterInfo = null;
					int num4 = 0;
					while ((long)num4 < (long)((ulong)num3))
					{
						int num5 = array3[num4];
						int num6;
						uint num7;
						uint num8;
						uint num9;
						uint num10;
						UnusedIntPtr unusedIntPtr;
						uint num11;
						rawImport.GetParamProps(num5, out num6, out num7, null, 0U, out num8, out num9, out num10, out unusedIntPtr, out num11);
						bool flag4 = num7 == 0U;
						if (flag4)
						{
							parameterInfo = new MetadataOnlyParameterInfo(this.Resolver, new Token(num5), this.ReturnType, this._descriptor.ReturnParameter.CustomModifiers);
						}
						else
						{
							uint num12 = num7 - 1U;
							array[(int)num12] = new MetadataOnlyParameterInfo(this.Resolver, new Token(num5), array2[(int)num12], this._descriptor.Parameters[(int)num12].CustomModifiers);
						}
						num4++;
					}
					bool flag5 = parameterInfo == null;
					if (flag5)
					{
						parameterInfo = this.Resolver.Policy.GetFakeParameterInfo(this, this.ReturnType, -1);
					}
					this._returnParameter = parameterInfo;
					for (int k = 0; k < num; k++)
					{
						bool flag6 = array[k] == null;
						if (flag6)
						{
							array[k] = this.Resolver.Policy.GetFakeParameterInfo(this, array2[k], k);
						}
					}
					result = array;
				}
			}
			return result;
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000239 RID: 569 RVA: 0x00006C94 File Offset: 0x00004E94
		public override ParameterInfo ReturnParameter
		{
			get
			{
				bool flag = this._returnParameter == null;
				if (flag)
				{
					this.GetParameters();
				}
				bool flag2 = this._returnParameter == null;
				ParameterInfo result;
				if (flag2)
				{
					result = this.Resolver.Policy.GetFakeParameterInfo(this, this.ReturnType, -1);
				}
				else
				{
					result = this._returnParameter;
				}
				return result;
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600023A RID: 570 RVA: 0x00006CEC File Offset: 0x00004EEC
		public override MethodAttributes Attributes
		{
			get
			{
				return this._attrs;
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x0600023B RID: 571 RVA: 0x00006D04 File Offset: 0x00004F04
		public override CallingConventions CallingConvention
		{
			get
			{
				bool flag = !this._fullyInitialized;
				if (flag)
				{
					this.Initialize();
				}
				CorCallingConvention callingConvention = this._descriptor.CallingConvention;
				bool flag2 = (callingConvention & CorCallingConvention.Mask) == CorCallingConvention.VarArg;
				CallingConventions callingConventions;
				if (flag2)
				{
					callingConventions = CallingConventions.VarArgs;
				}
				else
				{
					callingConventions = CallingConventions.Standard;
				}
				bool flag3 = (callingConvention & CorCallingConvention.HasThis) > CorCallingConvention.Default;
				if (flag3)
				{
					callingConventions |= CallingConventions.HasThis;
				}
				bool flag4 = (callingConvention & CorCallingConvention.ExplicitThis) > CorCallingConvention.Default;
				if (flag4)
				{
					callingConventions |= CallingConventions.ExplicitThis;
				}
				return callingConventions;
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600023C RID: 572 RVA: 0x00006D70 File Offset: 0x00004F70
		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Method;
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600023D RID: 573 RVA: 0x00006D84 File Offset: 0x00004F84
		public override bool IsGenericMethodDefinition
		{
			get
			{
				bool flag = !this._fullyInitialized;
				if (flag)
				{
					this.Initialize();
				}
				bool flag2 = (this._descriptor.CallingConvention & CorCallingConvention.Generic) > CorCallingConvention.Default;
				bool flag3 = !flag2;
				bool result;
				if (flag3)
				{
					result = false;
				}
				else
				{
					bool flag4 = GenericContext.IsNullOrEmptyMethodArgs(this._context);
					if (flag4)
					{
						result = true;
					}
					else
					{
						MethodInfo methodInfo = this.Resolver.Factory.CreateMethodOrConstructor(this.Resolver, this._methodDef, null, null) as MethodInfo;
						foreach (Type type in this._context.MethodArgs)
						{
							bool flag5 = !type.IsGenericParameter;
							if (flag5)
							{
								return false;
							}
							bool flag6 = !methodInfo.Equals(type.DeclaringMethod);
							if (flag6)
							{
								return false;
							}
						}
						result = true;
					}
				}
				return result;
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600023E RID: 574 RVA: 0x00006E6C File Offset: 0x0000506C
		public override bool IsGenericMethod
		{
			get
			{
				bool flag = !this._fullyInitialized;
				if (flag)
				{
					this.Initialize();
				}
				return !GenericContext.IsNullOrEmptyMethodArgs(this._context);
			}
		}

		// Token: 0x0600023F RID: 575 RVA: 0x00006EA4 File Offset: 0x000050A4
		public override MethodInfo MakeGenericMethod(params Type[] types)
		{
			bool flag = !this.IsGenericMethodDefinition;
			if (flag)
			{
				throw new InvalidOperationException();
			}
			Type[] typeArgs = this._context.TypeArgs;
			GenericContext context = new GenericContext(typeArgs, types);
			return (MethodInfo)MetadataOnlyMethodInfo.Create(this.Resolver, this._methodDef, context);
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00006F00 File Offset: 0x00005100
		public override Type[] GetGenericArguments()
		{
			bool flag = !this._fullyInitialized;
			if (flag)
			{
				this.Initialize();
			}
			return (Type[])this._context.MethodArgs.Clone();
		}

		// Token: 0x06000241 RID: 577 RVA: 0x00006F40 File Offset: 0x00005140
		public override MethodInfo GetGenericMethodDefinition()
		{
			bool flag = !this.IsGenericMethod;
			if (flag)
			{
				throw new InvalidOperationException();
			}
			bool isGenericMethodDefinition = this.IsGenericMethodDefinition;
			MethodInfo result;
			if (isGenericMethodDefinition)
			{
				result = this;
			}
			else
			{
				result = (this.Resolver.Factory.CreateMethodOrConstructor(this.Resolver, this._methodDef, this._context.TypeArgs, null) as MethodInfo);
			}
			return result;
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000242 RID: 578 RVA: 0x00006FA4 File Offset: 0x000051A4
		public override bool ContainsGenericParameters
		{
			get
			{
				bool containsGenericParameters = this.DeclaringType.ContainsGenericParameters;
				bool result;
				if (containsGenericParameters)
				{
					result = true;
				}
				else
				{
					Type[] genericArguments = this.GetGenericArguments();
					for (int i = 0; i < genericArguments.Length; i++)
					{
						bool containsGenericParameters2 = genericArguments[i].ContainsGenericParameters;
						if (containsGenericParameters2)
						{
							return true;
						}
					}
					result = false;
				}
				return result;
			}
		}

		// Token: 0x06000243 RID: 579 RVA: 0x00006FFC File Offset: 0x000051FC
		public override MethodBody GetMethodBody()
		{
			bool flag = this._methodBody == null;
			if (flag)
			{
				this._methodBody = MetadataOnlyMethodBody.TryCreate(this);
			}
			return this._methodBody;
		}

		// Token: 0x06000244 RID: 580 RVA: 0x00007030 File Offset: 0x00005230
		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return this.Resolver.GetMethodImplFlags(this._methodDef);
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000245 RID: 581 RVA: 0x0000240B File Offset: 0x0000060B
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000240B File Offset: 0x0000060B
		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000247 RID: 583 RVA: 0x00007058 File Offset: 0x00005258
		public override MethodInfo GetBaseDefinition()
		{
			bool flag = !base.IsVirtual || base.IsStatic || this.DeclaringType == null || this.DeclaringType.IsInterface;
			MethodInfo result;
			if (flag)
			{
				result = this;
			}
			else
			{
				Type baseType = this.DeclaringType.BaseType;
				bool flag2 = baseType == null;
				if (flag2)
				{
					result = this;
				}
				else
				{
					List<Type> list = new List<Type>();
					foreach (ParameterInfo parameterInfo in this.GetParameters())
					{
						list.Add(parameterInfo.ParameterType);
					}
					MethodInfo method = baseType.GetMethod(this.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, this.CallingConvention, list.ToArray(), null);
					bool flag3 = method == null;
					if (flag3)
					{
						result = this;
					}
					else
					{
						result = method.GetBaseDefinition();
					}
				}
			}
			return result;
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000248 RID: 584 RVA: 0x0000240B File Offset: 0x0000060B
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public override ICustomAttributeProvider ReturnTypeCustomAttributes
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x06000249 RID: 585 RVA: 0x00007128 File Offset: 0x00005328
		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this.Resolver.GetCustomAttributeData(this.MetadataToken);
		}

		// Token: 0x0400007A RID: 122
		private readonly Token _methodDef;

		// Token: 0x0400007B RID: 123
		private string _name;

		// Token: 0x0400007C RID: 124
		private readonly uint _nameLength;

		// Token: 0x0400007D RID: 125
		private Type _tOwner;

		// Token: 0x0400007E RID: 126
		private MethodSignatureDescriptor _descriptor;

		// Token: 0x0400007F RID: 127
		private ParameterInfo _returnParameter;

		// Token: 0x04000080 RID: 128
		private MethodBody _methodBody;

		// Token: 0x04000081 RID: 129
		private readonly MethodAttributes _attrs;

		// Token: 0x04000082 RID: 130
		private readonly Type[] _typeArgs;

		// Token: 0x04000083 RID: 131
		private readonly Type[] _methodArgs;

		// Token: 0x04000084 RID: 132
		private GenericContext _context;

		// Token: 0x04000085 RID: 133
		private Token _declaringTypeDef;

		// Token: 0x04000086 RID: 134
		private SignatureBlob _sigBlob;

		// Token: 0x04000087 RID: 135
		private bool _fullyInitialized;
	}
}
