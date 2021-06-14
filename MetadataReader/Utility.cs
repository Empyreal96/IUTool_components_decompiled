using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;
using Microsoft.Tools.IO;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000045 RID: 69
	internal static class Utility
	{
		// Token: 0x06000496 RID: 1174 RVA: 0x0000F3B4 File Offset: 0x0000D5B4
		public static bool Compare(string string1, string string2, bool ignoreCase)
		{
			return string.Equals(string1, string2, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		// Token: 0x06000497 RID: 1175 RVA: 0x0000F3D4 File Offset: 0x0000D5D4
		public static bool IsBindingFlagsMatching(MethodBase method, bool isInherited, BindingFlags bindingFlags)
		{
			return Utility.IsBindingFlagsMatching(method, method.IsStatic, method.IsPublic, isInherited, bindingFlags);
		}

		// Token: 0x06000498 RID: 1176 RVA: 0x0000F3FC File Offset: 0x0000D5FC
		public static bool IsBindingFlagsMatching(FieldInfo fieldInfo, bool isInherited, BindingFlags bindingFlags)
		{
			return Utility.IsBindingFlagsMatching(fieldInfo, fieldInfo.IsStatic, fieldInfo.IsPublic, isInherited, bindingFlags);
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x0000F424 File Offset: 0x0000D624
		public static bool IsBindingFlagsMatching(MemberInfo memberInfo, bool isStatic, bool isPublic, bool isInherited, BindingFlags bindingFlags)
		{
			bool flag = (bindingFlags & BindingFlags.DeclaredOnly) > BindingFlags.Default && isInherited;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				if (isPublic)
				{
					bool flag2 = (bindingFlags & BindingFlags.Public) == BindingFlags.Default;
					if (flag2)
					{
						return false;
					}
				}
				else
				{
					bool flag3 = (bindingFlags & BindingFlags.NonPublic) == BindingFlags.Default;
					if (flag3)
					{
						return false;
					}
				}
				bool flag4 = memberInfo.MemberType != MemberTypes.TypeInfo && memberInfo.MemberType != MemberTypes.NestedType;
				if (flag4)
				{
					if (isStatic)
					{
						bool flag5 = (bindingFlags & BindingFlags.FlattenHierarchy) == BindingFlags.Default && isInherited;
						if (flag5)
						{
							return false;
						}
						bool flag6 = (bindingFlags & BindingFlags.Static) == BindingFlags.Default;
						if (flag6)
						{
							return false;
						}
					}
					else
					{
						bool flag7 = (bindingFlags & BindingFlags.Instance) == BindingFlags.Default;
						if (flag7)
						{
							return false;
						}
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x0000F4E4 File Offset: 0x0000D6E4
		internal static string GetNamespaceHelper(string fullName)
		{
			bool flag = fullName.Contains(".");
			string result;
			if (flag)
			{
				int length = fullName.LastIndexOf('.');
				result = fullName.Substring(0, length);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x0000F51C File Offset: 0x0000D71C
		internal static string GetTypeNameFromFullNameHelper(string fullName, bool isNested)
		{
			string result;
			if (isNested)
			{
				int num = fullName.LastIndexOf('+');
				result = fullName.Substring(num + 1);
			}
			else
			{
				int num2 = fullName.LastIndexOf('.');
				result = fullName.Substring(num2 + 1);
			}
			return result;
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x0000F55C File Offset: 0x0000D75C
		internal static void VerifyNotByRef(MetadataOnlyCommonType type)
		{
			bool isByRef = type.IsByRef;
			if (isByRef)
			{
				string text = type.Name + "&";
				throw new TypeLoadException(string.Format(CultureInfo.InvariantCulture, Resources.CannotFindTypeInModule, new object[]
				{
					text,
					type.Resolver
				}));
			}
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x0000F5B0 File Offset: 0x0000D7B0
		internal static bool IsValidPath(string modulePath)
		{
			bool flag = string.IsNullOrEmpty(modulePath);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (char c in LongPathPath.GetInvalidPathChars())
				{
					foreach (char c2 in modulePath)
					{
						bool flag2 = c == c2;
						if (flag2)
						{
							return false;
						}
					}
				}
				try
				{
					bool flag3 = !LongPathPath.IsPathRooted(modulePath);
					if (flag3)
					{
						return false;
					}
				}
				catch (Exception ex)
				{
					throw;
				}
				result = true;
			}
			return result;
		}
	}
}
