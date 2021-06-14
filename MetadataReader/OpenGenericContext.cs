using System;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000031 RID: 49
	internal class OpenGenericContext : GenericContext
	{
		// Token: 0x06000382 RID: 898 RVA: 0x0000BF56 File Offset: 0x0000A156
		public OpenGenericContext(Type[] typeArgs, Type[] methodArgs) : base(typeArgs, methodArgs)
		{
		}

		// Token: 0x06000383 RID: 899 RVA: 0x0000BF64 File Offset: 0x0000A164
		public OpenGenericContext(MetadataOnlyModule resolver, Type ownerType, Token ownerMethod) : base(null, null)
		{
			this._resolver = resolver;
			this._ownerMethod = ownerMethod;
			int num = ownerType.GetGenericArguments().Length;
			Type[] array = new Type[num];
			Token ownerToken = new Token(ownerType.MetadataToken);
			for (int i = 0; i < num; i++)
			{
				array[i] = new MetadataOnlyTypeVariableRef(resolver, ownerToken, i);
			}
			base.TypeArgs = array;
		}

		// Token: 0x06000384 RID: 900 RVA: 0x0000BFD0 File Offset: 0x0000A1D0
		public override GenericContext VerifyAndUpdateMethodArguments(int expectedNumberOfMethodArgs)
		{
			bool flag = expectedNumberOfMethodArgs != base.MethodArgs.Length;
			GenericContext result;
			if (flag)
			{
				Type[] array = new Type[expectedNumberOfMethodArgs];
				for (int i = 0; i < expectedNumberOfMethodArgs; i++)
				{
					array[i] = new MetadataOnlyTypeVariableRef(this._resolver, this._ownerMethod, i);
				}
				result = new OpenGenericContext(base.TypeArgs, array);
			}
			else
			{
				result = this;
			}
			return result;
		}

		// Token: 0x040000B8 RID: 184
		private readonly MetadataOnlyModule _resolver;

		// Token: 0x040000B9 RID: 185
		private readonly Token _ownerMethod;
	}
}
