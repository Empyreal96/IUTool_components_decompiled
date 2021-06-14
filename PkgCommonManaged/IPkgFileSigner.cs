using System;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000023 RID: 35
	public interface IPkgFileSigner
	{
		// Token: 0x06000174 RID: 372
		void SignFile(string fileToSign);

		// Token: 0x06000175 RID: 373
		void SignFileWithOptions(string fileToSign, string options);
	}
}
