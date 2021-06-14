using System;
using System.IO;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.WPImage
{
	// Token: 0x02000008 RID: 8
	public class SigningWrapper : IPayloadWrapper
	{
		// Token: 0x06000022 RID: 34 RVA: 0x000030F4 File Offset: 0x000012F4
		public SigningWrapper(FullFlashUpdateImage ffuImage, IPayloadWrapper innerWrapper)
		{
			this.ffuImage = ffuImage;
			this.innerWrapper = innerWrapper;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x0000310A File Offset: 0x0000130A
		public void InitializeWrapper(long payloadSize)
		{
			this.innerWrapper.InitializeWrapper(payloadSize);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00003118 File Offset: 0x00001318
		public void ResetPosition()
		{
			this.innerWrapper.ResetPosition();
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00003125 File Offset: 0x00001325
		public void Write(byte[] data)
		{
			this.innerWrapper.Write(data);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00003134 File Offset: 0x00001334
		public void FinalizeWrapper()
		{
			byte[] securityHeader = this.ffuImage.GetSecurityHeader(this.ffuImage.CatalogData, this.ffuImage.HashTableData);
			string tempFileName = Path.GetTempFileName();
			File.WriteAllBytes(tempFileName, this.ffuImage.CatalogData);
			SigningWrapper.SignFile(tempFileName);
			this.ffuImage.CatalogData = File.ReadAllBytes(tempFileName);
			byte[] securityHeader2 = this.ffuImage.GetSecurityHeader(this.ffuImage.CatalogData, this.ffuImage.HashTableData);
			if (securityHeader2.Length != securityHeader.Length)
			{
				throw new ImageStorageException("Signed catalog too large to fit in image.  Dismount without signing and use imagesigner.");
			}
			this.innerWrapper.ResetPosition();
			this.innerWrapper.Write(securityHeader2);
			this.innerWrapper.FinalizeWrapper();
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000031E8 File Offset: 0x000013E8
		private static void SignFile(string file)
		{
			int num = 0;
			try
			{
				num = CommonUtils.RunProcess("%COMSPEC%", string.Format("/C sign.cmd \"{0}\"", file));
			}
			catch (Exception innerException)
			{
				throw new ImageStorageException(string.Format("Failed to sign the file {0}", file), innerException);
			}
			if (num != 0)
			{
				throw new ImageStorageException(string.Format("Failed to sign file {0}, exit code {1}", file, num));
			}
		}

		// Token: 0x04000011 RID: 17
		private IPayloadWrapper innerWrapper;

		// Token: 0x04000012 RID: 18
		private FullFlashUpdateImage ffuImage;
	}
}
