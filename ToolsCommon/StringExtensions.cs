using System;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200002A RID: 42
	public static class StringExtensions
	{
		// Token: 0x06000167 RID: 359 RVA: 0x000082D8 File Offset: 0x000064D8
		public static string Replace(this string originalString, string oldValue, string newValue, StringComparison comparisonType)
		{
			int num = 0;
			for (;;)
			{
				num = originalString.IndexOf(oldValue, num, comparisonType);
				if (num == -1)
				{
					break;
				}
				originalString = originalString.Substring(0, num) + newValue + originalString.Substring(num + oldValue.Length);
				num += newValue.Length;
			}
			return originalString;
		}
	}
}
