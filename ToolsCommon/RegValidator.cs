using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000017 RID: 23
	public static class RegValidator
	{
		// Token: 0x060000DE RID: 222
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int ValidateRegistryHive(string RegHive);

		// Token: 0x060000DF RID: 223
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern int ValidateRegFiles(string[] rgRegFiles, int cRegFiles, string[] rgaFiles, int cRgaFiles);

		// Token: 0x060000E0 RID: 224 RVA: 0x00006C78 File Offset: 0x00004E78
		public static void Validate(IEnumerable<string> regFiles, IEnumerable<string> rgaFiles)
		{
			string[] array = (regFiles != null) ? regFiles.ToArray<string>() : new string[0];
			string[] array2 = (rgaFiles != null) ? rgaFiles.ToArray<string>() : new string[0];
			if (array.Length == 0 && array2.Length == 0)
			{
				return;
			}
			string[] array3 = array;
			int cRegFiles = array3.Length;
			string[] array4 = array2;
			int num = RegValidator.ValidateRegFiles(array3, cRegFiles, array4, array4.Length);
			if (num != 0)
			{
				throw new IUException("Registry validation failed, check output log for detailed failure information, err '0x{0:X8}'", new object[]
				{
					num
				});
			}
		}
	}
}
