using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Phone.Test.TestMetadata.CommandLine
{
	// Token: 0x02000022 RID: 34
	internal sealed class FormatUtility
	{
		// Token: 0x060000C8 RID: 200 RVA: 0x00004718 File Offset: 0x00002918
		private FormatUtility()
		{
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00004724 File Offset: 0x00002924
		public static string FormatStringForWidth(string toFormat, int offset, int hangingIndent, int width)
		{
			IList<string> list = FormatUtility.MakeWords(toFormat);
			StringBuilder stringBuilder = new StringBuilder();
			string value = new string(' ', offset + hangingIndent);
			int num = 0;
			int num2;
			for (int i = 0; i < list.Count; i = num2 + 1)
			{
				num2 = i;
				int num3 = offset;
				bool flag = num > 0;
				if (flag)
				{
					num3 += hangingIndent;
				}
				num3 += list[i].Length;
				bool flag2 = list[i].EndsWith(".", StringComparison.OrdinalIgnoreCase);
				if (flag2)
				{
					num3 += 2;
				}
				else
				{
					num3++;
				}
				while (num2 + 1 < list.Count && num3 + list[num2 + 1].Length < width)
				{
					num2++;
					num3 += list[num2].Length;
					bool flag3 = list[num2].EndsWith(".", StringComparison.OrdinalIgnoreCase);
					if (flag3)
					{
						num3 += 2;
					}
					else
					{
						num3++;
					}
				}
				bool flag4 = num > 0;
				if (flag4)
				{
					stringBuilder.Append(value);
				}
				else
				{
					stringBuilder.Append(new string(' ', offset));
				}
				for (int j = i; j <= num2; j++)
				{
					bool flag5 = j > i;
					if (flag5)
					{
						bool flag6 = list[j - 1].EndsWith(".", StringComparison.OrdinalIgnoreCase);
						if (flag6)
						{
							stringBuilder.Append("  ");
						}
						else
						{
							stringBuilder.Append(" ");
						}
					}
					stringBuilder.Append(list[j]);
				}
				stringBuilder.Append(Environment.NewLine);
				num++;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060000CA RID: 202 RVA: 0x000048F0 File Offset: 0x00002AF0
		public static IList<string> MakeWords(string toParse)
		{
			char[] array = toParse.ToCharArray();
			StringBuilder stringBuilder = new StringBuilder();
			List<string> list = new List<string>();
			for (int i = 0; i < array.Length; i++)
			{
				bool flag = char.IsWhiteSpace(array[i]);
				if (flag)
				{
					bool flag2 = stringBuilder.Length > 0;
					if (flag2)
					{
						list.Add(stringBuilder.ToString());
						stringBuilder.Length = 0;
					}
				}
				else
				{
					stringBuilder.Append(array[i]);
				}
			}
			bool flag3 = stringBuilder.Length > 0;
			if (flag3)
			{
				list.Add(stringBuilder.ToString());
			}
			return list;
		}

		// Token: 0x0200002E RID: 46
		private enum ParseState
		{
			// Token: 0x040000C2 RID: 194
			Start,
			// Token: 0x040000C3 RID: 195
			StartOfLine,
			// Token: 0x040000C4 RID: 196
			ReadNext,
			// Token: 0x040000C5 RID: 197
			EndOfLine,
			// Token: 0x040000C6 RID: 198
			EndOfString
		}
	}
}
