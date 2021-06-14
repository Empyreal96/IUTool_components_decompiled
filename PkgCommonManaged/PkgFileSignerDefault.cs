using System;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000010 RID: 16
	internal class PkgFileSignerDefault : IPkgFileSigner
	{
		// Token: 0x06000081 RID: 129 RVA: 0x00004217 File Offset: 0x00002417
		public void SignFile(string fileToSign)
		{
			PackageTools.SignFile(fileToSign);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000421F File Offset: 0x0000241F
		public void SignFileWithOptions(string fileToSign, string options)
		{
			PackageTools.SignFileWithOptions(fileToSign, options);
		}
	}
}
