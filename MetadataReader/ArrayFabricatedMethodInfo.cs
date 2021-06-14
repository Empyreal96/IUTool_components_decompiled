using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000006 RID: 6
	internal abstract class ArrayFabricatedMethodInfo : MethodInfo
	{
		// Token: 0x06000013 RID: 19 RVA: 0x00002241 File Offset: 0x00000441
		protected ArrayFabricatedMethodInfo(Type arrayType)
		{
			this._arrayType = arrayType;
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000014 RID: 20 RVA: 0x00002254 File Offset: 0x00000454
		protected ITypeUniverse Universe
		{
			get
			{
				return Helpers.Universe(this._arrayType);
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000015 RID: 21 RVA: 0x00002274 File Offset: 0x00000474
		protected int Rank
		{
			get
			{
				return this._arrayType.GetArrayRank();
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002294 File Offset: 0x00000494
		protected Type GetElementType()
		{
			return this._arrayType.GetElementType();
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000022B4 File Offset: 0x000004B4
		protected ParameterInfo[] MakeParameterHelper(int extra)
		{
			int rank = this.Rank;
			ITypeUniverse universe = this.Universe;
			Type builtInType = universe.GetBuiltInType(CorElementType.Int);
			ParameterInfo[] array = new ParameterInfo[rank + extra];
			for (int i = 0; i < rank; i++)
			{
				array[i] = this.MakeParameterInfo(builtInType, i);
			}
			return array;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002310 File Offset: 0x00000510
		protected ParameterInfo MakeParameterInfo(Type t, int position)
		{
			return new SimpleParameterInfo(this, t, position);
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000019 RID: 25 RVA: 0x0000232A File Offset: 0x0000052A
		public override ICustomAttributeProvider ReturnTypeCustomAttributes
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002334 File Offset: 0x00000534
		public override MethodInfo GetBaseDefinition()
		{
			return this;
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600001B RID: 27 RVA: 0x00002348 File Offset: 0x00000548
		public override ParameterInfo ReturnParameter
		{
			get
			{
				return this.MakeParameterInfo(this.ReturnType, -1);
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600001C RID: 28 RVA: 0x00002368 File Offset: 0x00000568
		public override MethodAttributes Attributes
		{
			get
			{
				return MethodAttributes.Public;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600001D RID: 29 RVA: 0x0000237C File Offset: 0x0000057C
		public override CallingConventions CallingConvention
		{
			get
			{
				return CallingConventions.Standard | CallingConventions.HasThis;
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002390 File Offset: 0x00000590
		public override MethodInfo MakeGenericMethod(params Type[] types)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002398 File Offset: 0x00000598
		public override bool IsGenericMethodDefinition
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000023AC File Offset: 0x000005AC
		public override Type[] GetGenericArguments()
		{
			return Type.EmptyTypes;
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000021 RID: 33 RVA: 0x000023C4 File Offset: 0x000005C4
		public override bool ContainsGenericParameters
		{
			get
			{
				return this.GetElementType().IsGenericParameter;
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000023E4 File Offset: 0x000005E4
		public override MethodBody GetMethodBody()
		{
			return null;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000023F8 File Offset: 0x000005F8
		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return MethodImplAttributes.IL;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x0000240B File Offset: 0x0000060B
		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000025 RID: 37 RVA: 0x0000240B File Offset: 0x0000060B
		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000026 RID: 38 RVA: 0x00002414 File Offset: 0x00000614
		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Method;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000027 RID: 39 RVA: 0x00002428 File Offset: 0x00000628
		public override Type DeclaringType
		{
			get
			{
				return this._arrayType;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000028 RID: 40 RVA: 0x00002440 File Offset: 0x00000640
		public override int MetadataToken
		{
			get
			{
				return new Token(TokenType.MethodDef, 0);
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00002464 File Offset: 0x00000664
		public override Module Module
		{
			get
			{
				return this.DeclaringType.Module;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600002A RID: 42 RVA: 0x0000240B File Offset: 0x0000060B
		public override Type ReflectedType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002484 File Offset: 0x00000684
		public override object[] GetCustomAttributes(bool inherit)
		{
			return new object[0];
		}

		// Token: 0x0600002C RID: 44 RVA: 0x0000249C File Offset: 0x0000069C
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return new object[0];
		}

		// Token: 0x0600002D RID: 45 RVA: 0x0000232A File Offset: 0x0000052A
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000024B4 File Offset: 0x000006B4
		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return new CustomAttributeData[0];
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000024CC File Offset: 0x000006CC
		public override string ToString()
		{
			return MetadataOnlyMethodInfo.CommonToString(this);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000024E4 File Offset: 0x000006E4
		public override bool Equals(object obj)
		{
			ArrayFabricatedMethodInfo arrayFabricatedMethodInfo = obj as ArrayFabricatedMethodInfo;
			bool flag = arrayFabricatedMethodInfo == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !this.DeclaringType.Equals(arrayFabricatedMethodInfo.DeclaringType);
				result = (!flag2 && this.Name.Equals(arrayFabricatedMethodInfo.Name));
			}
			return result;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002538 File Offset: 0x00000738
		public override int GetHashCode()
		{
			return this.DeclaringType.GetHashCode() + this.Name.GetHashCode();
		}

		// Token: 0x04000002 RID: 2
		private readonly Type _arrayType;
	}
}
