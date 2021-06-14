using System;
using System.Reflection.Mock;

namespace System.Reflection.Adds
{
	// Token: 0x0200001B RID: 27
	internal static class Helpers
	{
		// Token: 0x060000AE RID: 174 RVA: 0x00003D98 File Offset: 0x00001F98
		public static ITypeUniverse Universe(Type type)
		{
			ITypeProxy typeProxy = type as ITypeProxy;
			bool flag = typeProxy != null;
			ITypeUniverse result;
			if (flag)
			{
				result = typeProxy.TypeUniverse;
			}
			else
			{
				Assembly assembly = type.Assembly;
				IAssembly2 assembly2 = assembly as IAssembly2;
				bool flag2 = assembly2 == null;
				if (flag2)
				{
					result = null;
				}
				else
				{
					result = assembly2.TypeUniverse;
				}
			}
			return result;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00003DEC File Offset: 0x00001FEC
		public static Type EnsureResolve(Type type)
		{
			for (;;)
			{
				ITypeProxy typeProxy = type as ITypeProxy;
				bool flag = typeProxy == null;
				if (flag)
				{
					break;
				}
				type = typeProxy.GetResolvedType();
			}
			return type;
		}
	}
}
