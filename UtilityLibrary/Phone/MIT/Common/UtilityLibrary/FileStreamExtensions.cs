using System;
using System.IO;

namespace Microsoft.Phone.MIT.Common.UtilityLibrary
{
	// Token: 0x02000017 RID: 23
	public static class FileStreamExtensions
	{
		// Token: 0x06000077 RID: 119 RVA: 0x000044D4 File Offset: 0x000026D4
		public static int ReadReally(this FileStream fs, byte[] destBuffer, int bytesToRead)
		{
			int num = 0;
			int num2;
			do
			{
				num2 = fs.Read(destBuffer, num, bytesToRead - num);
				num += num2;
			}
			while (num2 != 0 && num != bytesToRead);
			return num;
		}
	}
}
