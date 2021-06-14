using System;
using System.Text;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000018 RID: 24
	public static class FullFlashUpdateHeaders
	{
		// Token: 0x060000E5 RID: 229 RVA: 0x0000BCF4 File Offset: 0x00009EF4
		public static byte[] GetSecuritySignature()
		{
			return Encoding.ASCII.GetBytes("SignedImage ");
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x0000BD05 File Offset: 0x00009F05
		public static byte[] GetImageSignature()
		{
			return Encoding.ASCII.GetBytes("ImageFlash  ");
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000E7 RID: 231 RVA: 0x0000BD16 File Offset: 0x00009F16
		public static uint SecurityHeaderSize
		{
			get
			{
				return (uint)(FullFlashUpdateImage.SecureHeaderSize + FullFlashUpdateHeaders.GetSecuritySignature().Length);
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x0000BD25 File Offset: 0x00009F25
		public static uint ImageHeaderSize
		{
			get
			{
				return (uint)(FullFlashUpdateImage.ImageHeaderSize + FullFlashUpdateHeaders.GetImageSignature().Length);
			}
		}
	}
}
