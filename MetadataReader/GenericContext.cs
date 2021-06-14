using System;
using System.Reflection.Adds;
using System.Reflection.Mock;
using System.Text;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000010 RID: 16
	internal class GenericContext
	{
		// Token: 0x0600006D RID: 109 RVA: 0x00002CA6 File Offset: 0x00000EA6
		public GenericContext(Type[] typeArgs, Type[] methodArgs)
		{
			this.TypeArgs = ((typeArgs == null) ? Type.EmptyTypes : typeArgs);
			this.MethodArgs = ((methodArgs == null) ? Type.EmptyTypes : methodArgs);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00002CD4 File Offset: 0x00000ED4
		public GenericContext(MethodBase methodTypeContext) : this(methodTypeContext.DeclaringType.GetGenericArguments(), methodTypeContext.GetGenericArguments())
		{
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600006F RID: 111 RVA: 0x00002CEF File Offset: 0x00000EEF
		// (set) Token: 0x06000070 RID: 112 RVA: 0x00002CF7 File Offset: 0x00000EF7
		public Type[] TypeArgs { get; protected set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000071 RID: 113 RVA: 0x00002D00 File Offset: 0x00000F00
		// (set) Token: 0x06000072 RID: 114 RVA: 0x00002D08 File Offset: 0x00000F08
		public Type[] MethodArgs { get; protected set; }

		// Token: 0x06000073 RID: 115 RVA: 0x00002D14 File Offset: 0x00000F14
		public override bool Equals(object obj)
		{
			GenericContext genericContext = (GenericContext)obj;
			bool flag = genericContext == null;
			return !flag && GenericContext.IsArrayEqual<Type>(this.TypeArgs, genericContext.TypeArgs) && GenericContext.IsArrayEqual<Type>(this.MethodArgs, genericContext.MethodArgs);
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00002D60 File Offset: 0x00000F60
		public override int GetHashCode()
		{
			return GenericContext.GetArrayHashCode<Type>(this.TypeArgs) * 32768 + GenericContext.GetArrayHashCode<Type>(this.MethodArgs);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00002D90 File Offset: 0x00000F90
		public virtual GenericContext VerifyAndUpdateMethodArguments(int expectedNumberOfMethodArgs)
		{
			bool flag = this.MethodArgs.Length != expectedNumberOfMethodArgs;
			if (flag)
			{
				throw new ArgumentException(Resources.InvalidMetadataSignature);
			}
			return this;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00002DC4 File Offset: 0x00000FC4
		private static int GetArrayHashCode<T>(T[] a)
		{
			int num = 0;
			for (int i = 0; i < a.Length; i++)
			{
				num += a[i].GetHashCode() * i;
			}
			return num;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00002E08 File Offset: 0x00001008
		private static bool IsArrayEqual<T>(T[] a1, T[] a2) where T : Type
		{
			bool flag = a1.Length == a2.Length;
			bool result;
			if (flag)
			{
				for (int i = 0; i < a1.Length; i++)
				{
					bool flag2 = !a1[i].Equals(a2[i]);
					if (flag2)
					{
						return false;
					}
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00002E6C File Offset: 0x0000106C
		private static string ArrayToString<T>(T[] a)
		{
			bool flag = a == null;
			string result;
			if (flag)
			{
				result = "empty";
			}
			else
			{
				StringBuilder stringBuilder = StringBuilderPool.Get();
				for (int i = 0; i < a.Length; i++)
				{
					bool flag2 = i != 0;
					if (flag2)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(a[i]);
				}
				string text = stringBuilder.ToString();
				StringBuilderPool.Release(ref stringBuilder);
				result = text;
			}
			return result;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00002EEC File Offset: 0x000010EC
		public override string ToString()
		{
			return "Type: " + GenericContext.ArrayToString<Type>(this.TypeArgs) + ", Method: " + GenericContext.ArrayToString<Type>(this.MethodArgs);
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00002F24 File Offset: 0x00001124
		public GenericContext DeleteMethodArgs()
		{
			bool flag = this.MethodArgs.Length == 0;
			GenericContext result;
			if (flag)
			{
				result = this;
			}
			else
			{
				result = new GenericContext(this.TypeArgs, null);
			}
			return result;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00002F58 File Offset: 0x00001158
		public static bool IsNullOrEmptyMethodArgs(GenericContext context)
		{
			return context == null || context.MethodArgs.Length == 0;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00002F84 File Offset: 0x00001184
		public static bool IsNullOrEmptyTypeArgs(GenericContext context)
		{
			return context == null || context.TypeArgs.Length == 0;
		}
	}
}
