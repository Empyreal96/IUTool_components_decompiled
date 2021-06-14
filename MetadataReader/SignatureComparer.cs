using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000039 RID: 57
	internal static class SignatureComparer
	{
		// Token: 0x06000437 RID: 1079 RVA: 0x0000D788 File Offset: 0x0000B988
		public static IEnumerable<MethodBase> FilterMethods(MethodFilter filter, MethodInfo[] allMethods)
		{
			List<MethodBase> list = new List<MethodBase>();
			CallingConventions reflectionCallingConvention = SignatureUtil.GetReflectionCallingConvention(filter.CallingConvention);
			foreach (MethodInfo methodInfo in allMethods)
			{
				bool flag = methodInfo.Name.Equals(filter.Name, StringComparison.Ordinal) && SignatureUtil.IsCallingConventionMatch(methodInfo, reflectionCallingConvention) && SignatureUtil.IsGenericParametersCountMatch(methodInfo, filter.GenericParameterCount) && methodInfo.GetParameters().Length == filter.ParameterCount;
				if (flag)
				{
					list.Add(methodInfo);
				}
			}
			return list;
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x0000D81C File Offset: 0x0000BA1C
		public static IEnumerable<MethodBase> FilterConstructors(MethodFilter filter, ConstructorInfo[] allConstructors)
		{
			List<MethodBase> list = new List<MethodBase>();
			CallingConventions reflectionCallingConvention = SignatureUtil.GetReflectionCallingConvention(filter.CallingConvention);
			foreach (ConstructorInfo constructorInfo in allConstructors)
			{
				bool flag = constructorInfo.Name.Equals(filter.Name, StringComparison.Ordinal) && SignatureUtil.IsCallingConventionMatch(constructorInfo, reflectionCallingConvention) && constructorInfo.GetParameters().Length == filter.ParameterCount;
				if (flag)
				{
					list.Add(constructorInfo);
				}
			}
			return list;
		}

		// Token: 0x06000439 RID: 1081 RVA: 0x0000D8A0 File Offset: 0x0000BAA0
		internal static bool IsParametersTypeMatch(MethodBase templateMethod, TypeSignatureDescriptor[] parameters)
		{
			ParameterInfo[] parameters2 = templateMethod.GetParameters();
			int num = parameters2.Length;
			for (int i = 0; i < num; i++)
			{
				bool flag = !parameters[i].Type.Equals(parameters2[i].ParameterType);
				if (flag)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x0000D8F8 File Offset: 0x0000BAF8
		public static MethodBase FindMatchingMethod(string methodName, Type typeToInspect, MethodSignatureDescriptor expectedSignature, GenericContext context)
		{
			bool flag = methodName.Equals(".ctor", StringComparison.Ordinal) || methodName.Equals(".cctor", StringComparison.Ordinal);
			int genericParameterCount = expectedSignature.GenericParameterCount;
			MethodFilter filter = new MethodFilter(methodName, genericParameterCount, expectedSignature.Parameters.Length, expectedSignature.CallingConvention);
			bool flag2 = flag;
			IEnumerable<MethodBase> enumerable;
			if (flag2)
			{
				enumerable = SignatureComparer.FilterConstructors(filter, typeToInspect.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
			}
			else
			{
				enumerable = SignatureComparer.FilterMethods(filter, typeToInspect.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
			}
			MethodBase result = null;
			bool flag3 = false;
			foreach (MethodBase methodBase in enumerable)
			{
				MethodBase methodBase2 = methodBase;
				bool flag4 = false;
				bool flag5 = genericParameterCount > 0 && context.MethodArgs.Length != 0;
				if (flag5)
				{
					methodBase2 = (methodBase as MethodInfo).MakeGenericMethod(context.MethodArgs);
					flag4 = true;
				}
				bool isGenericType = typeToInspect.IsGenericType;
				MethodBase methodBase3;
				if (isGenericType)
				{
					methodBase3 = SignatureComparer.GetTemplateMethod(typeToInspect, methodBase2.MetadataToken);
				}
				else
				{
					bool flag6 = flag4;
					if (flag6)
					{
						methodBase3 = methodBase;
					}
					else
					{
						methodBase3 = methodBase2;
					}
				}
				bool flag7 = !flag;
				if (flag7)
				{
					bool flag8 = !expectedSignature.ReturnParameter.Type.Equals((methodBase3 as MethodInfo).ReturnType);
					if (flag8)
					{
						continue;
					}
				}
				bool flag9 = !SignatureComparer.IsParametersTypeMatch(methodBase3, expectedSignature.Parameters);
				if (!flag9)
				{
					bool flag10 = !flag3;
					if (!flag10)
					{
						throw new AmbiguousMatchException();
					}
					result = methodBase2;
					flag3 = true;
				}
			}
			return result;
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x0000DA9C File Offset: 0x0000BC9C
		private static MethodBase GetTemplateMethod(Type typeToInspect, int methodToken)
		{
			return typeToInspect.GetGenericTypeDefinition().Module.ResolveMethod(methodToken);
		}

		// Token: 0x040000CC RID: 204
		private const BindingFlags MembersDeclaredOnTypeOnly = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
	}
}
