using System;
using System.Text;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000041 RID: 65
	internal static class TypeNameQuoter
	{
		// Token: 0x0600047A RID: 1146 RVA: 0x0000EDD0 File Offset: 0x0000CFD0
		internal static string GetQuotedTypeName(string name)
		{
			bool flag = name.IndexOfAny(TypeNameQuoter.s_specialCharacters) == -1;
			string result;
			if (flag)
			{
				result = name;
			}
			else
			{
				StringBuilder stringBuilder = StringBuilderPool.Get();
				for (int i = 0; i < name.Length; i++)
				{
					bool flag2 = TypeNameQuoter.Contains(TypeNameQuoter.s_specialCharacters, name[i]);
					if (flag2)
					{
						stringBuilder.Append('\\');
					}
					stringBuilder.Append(name[i]);
				}
				string text = stringBuilder.ToString();
				StringBuilderPool.Release(ref stringBuilder);
				result = text;
			}
			return result;
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x0000EE60 File Offset: 0x0000D060
		private static bool Contains(char[] This, char ch)
		{
			foreach (char c in This)
			{
				bool flag = c == ch;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x040000E4 RID: 228
		private static readonly char[] s_specialCharacters = new char[]
		{
			'\\',
			'[',
			']',
			',',
			'+',
			'&',
			'*'
		};
	}
}
