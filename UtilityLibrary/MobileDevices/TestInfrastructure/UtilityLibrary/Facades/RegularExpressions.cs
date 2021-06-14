using System;
using System.Text.RegularExpressions;

namespace Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary.Facades
{
	// Token: 0x02000015 RID: 21
	public class RegularExpressions
	{
		// Token: 0x06000069 RID: 105 RVA: 0x000041B0 File Offset: 0x000023B0
		public static bool MatchRegEx(string stringToTest, string expression)
		{
			Regex expression2 = new Regex(expression);
			return RegularExpressions.MatchRegEx(stringToTest, expression2);
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000041D0 File Offset: 0x000023D0
		public static bool MatchRegEx(string stringToTest, Regex expression)
		{
			Match match = expression.Match(stringToTest);
			return match.Success;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000041F0 File Offset: 0x000023F0
		public static int FindFirstMatchGroupInString(string regularExpression, string stringToMatch, out string firstMatch)
		{
			Regex regex = new Regex(regularExpression, RegexOptions.IgnoreCase);
			return RegularExpressions.FindFirstMatchGroupInString(regex, stringToMatch, out firstMatch);
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00004214 File Offset: 0x00002414
		public static int FindFirstMatchGroupInMultiLineString(string regularExpression, string stringToMatch, out string firstMatch)
		{
			Regex regex = new Regex(regularExpression, RegexOptions.IgnoreCase | RegexOptions.Multiline);
			return RegularExpressions.FindFirstMatchGroupInString(regex, stringToMatch, out firstMatch);
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00004238 File Offset: 0x00002438
		public static int FindFirstMatchGroupInString(Regex regex, string stringToMatch, out string firstMatch)
		{
			int result = 0;
			Match match = regex.Match(stringToMatch);
			if (match.Success)
			{
				firstMatch = match.Groups[1].ToString();
				result = match.Groups.Count;
			}
			else
			{
				firstMatch = string.Empty;
			}
			return result;
		}
	}
}
