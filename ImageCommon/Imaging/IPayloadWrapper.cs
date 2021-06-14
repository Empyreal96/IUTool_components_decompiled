using System;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000028 RID: 40
	public interface IPayloadWrapper
	{
		// Token: 0x060001BB RID: 443
		void InitializeWrapper(long payloadSize);

		// Token: 0x060001BC RID: 444
		void ResetPosition();

		// Token: 0x060001BD RID: 445
		void Write(byte[] data);

		// Token: 0x060001BE RID: 446
		void FinalizeWrapper();
	}
}
