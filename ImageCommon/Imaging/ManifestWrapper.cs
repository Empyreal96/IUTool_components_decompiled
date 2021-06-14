using System;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000029 RID: 41
	public class ManifestWrapper : IPayloadWrapper
	{
		// Token: 0x060001BF RID: 447 RVA: 0x0000F7C0 File Offset: 0x0000D9C0
		public ManifestWrapper(FullFlashUpdateImage ffuImage, IPayloadWrapper innerWrapper)
		{
			this.ffuImage = ffuImage;
			this.innerWrapper = innerWrapper;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000F7D8 File Offset: 0x0000D9D8
		public void InitializeWrapper(long payloadSize)
		{
			byte[] manifestRegion = this.ffuImage.GetManifestRegion();
			long payloadSize2 = payloadSize + (long)manifestRegion.Length;
			this.innerWrapper.InitializeWrapper(payloadSize2);
			this.innerWrapper.Write(manifestRegion);
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000F810 File Offset: 0x0000DA10
		public void ResetPosition()
		{
			this.innerWrapper.ResetPosition();
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000F81D File Offset: 0x0000DA1D
		public void Write(byte[] data)
		{
			this.innerWrapper.Write(data);
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000F82B File Offset: 0x0000DA2B
		public void FinalizeWrapper()
		{
			this.innerWrapper.FinalizeWrapper();
		}

		// Token: 0x0400011F RID: 287
		private FullFlashUpdateImage ffuImage;

		// Token: 0x04000120 RID: 288
		private IPayloadWrapper innerWrapper;
	}
}
