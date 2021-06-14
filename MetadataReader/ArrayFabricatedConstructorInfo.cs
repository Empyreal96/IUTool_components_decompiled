using System;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000002 RID: 2
	internal class ArrayFabricatedConstructorInfo : MetadataOnlyConstructorInfo
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public ArrayFabricatedConstructorInfo(Type arrayType, int numParams) : base(ArrayFabricatedConstructorInfo.MakeMethodInfo(arrayType, numParams))
		{
			this._numParams = numParams;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002068 File Offset: 0x00000268
		private static MethodInfo MakeMethodInfo(Type arrayType, int numParams)
		{
			return new ArrayFabricatedConstructorInfo.Adapter(arrayType, numParams);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002084 File Offset: 0x00000284
		public override bool Equals(object obj)
		{
			ArrayFabricatedConstructorInfo arrayFabricatedConstructorInfo = obj as ArrayFabricatedConstructorInfo;
			bool flag = arrayFabricatedConstructorInfo == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !this.DeclaringType.Equals(arrayFabricatedConstructorInfo.DeclaringType);
				result = (!flag2 && this.ToString().Equals(arrayFabricatedConstructorInfo.ToString()));
			}
			return result;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020D8 File Offset: 0x000002D8
		public override int GetHashCode()
		{
			return this.DeclaringType.GetHashCode() + this._numParams;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020FC File Offset: 0x000002FC
		public override object[] GetCustomAttributes(bool inherit)
		{
			return new object[0];
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002114 File Offset: 0x00000314
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return new object[0];
		}

		// Token: 0x04000001 RID: 1
		private readonly int _numParams;

		// Token: 0x02000047 RID: 71
		private class Adapter : ArrayFabricatedMethodInfo
		{
			// Token: 0x0600049E RID: 1182 RVA: 0x0000F654 File Offset: 0x0000D854
			public Adapter(Type arrayType, int numParams) : base(arrayType)
			{
				this._numParams = numParams;
			}

			// Token: 0x1700011E RID: 286
			// (get) Token: 0x0600049F RID: 1183 RVA: 0x0000F668 File Offset: 0x0000D868
			public override string Name
			{
				get
				{
					return ".ctor";
				}
			}

			// Token: 0x060004A0 RID: 1184 RVA: 0x0000F680 File Offset: 0x0000D880
			public override ParameterInfo[] GetParameters()
			{
				ITypeUniverse universe = base.Universe;
				Type builtInType = universe.GetBuiltInType(CorElementType.Int);
				ParameterInfo[] array = new ParameterInfo[this._numParams];
				for (int i = 0; i < this._numParams; i++)
				{
					array[i] = base.MakeParameterInfo(builtInType, i);
				}
				return array;
			}

			// Token: 0x1700011F RID: 287
			// (get) Token: 0x060004A1 RID: 1185 RVA: 0x0000F6D4 File Offset: 0x0000D8D4
			public override MethodAttributes Attributes
			{
				get
				{
					return MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.RTSpecialName;
				}
			}

			// Token: 0x17000120 RID: 288
			// (get) Token: 0x060004A2 RID: 1186 RVA: 0x0000F6EC File Offset: 0x0000D8EC
			public override Type ReturnType
			{
				get
				{
					return base.Universe.GetBuiltInType(CorElementType.Void);
				}
			}

			// Token: 0x040000EC RID: 236
			private readonly int _numParams;
		}
	}
}
