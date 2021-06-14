using System;
using System.Runtime.Serialization;

namespace FFUComponents
{
	// Token: 0x0200001C RID: 28
	[Serializable]
	public class FFUDeviceRetailUnlockException : FFUException
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000AE RID: 174 RVA: 0x00003CC7 File Offset: 0x00001EC7
		// (set) Token: 0x060000AF RID: 175 RVA: 0x00003CCF File Offset: 0x00001ECF
		public int EfiStatus { get; set; }

		// Token: 0x060000B0 RID: 176 RVA: 0x00003C35 File Offset: 0x00001E35
		public FFUDeviceRetailUnlockException()
		{
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00003C3D File Offset: 0x00001E3D
		public FFUDeviceRetailUnlockException(string message) : base(message)
		{
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00003C46 File Offset: 0x00001E46
		public FFUDeviceRetailUnlockException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00003CD8 File Offset: 0x00001ED8
		public FFUDeviceRetailUnlockException(IFFUDevice device, string message, Exception e) : base(device, message, e)
		{
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00003CE3 File Offset: 0x00001EE3
		public FFUDeviceRetailUnlockException(IFFUDevice device, int efiStatus) : base(device)
		{
			this.EfiStatus = efiStatus;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00003CBD File Offset: 0x00001EBD
		protected FFUDeviceRetailUnlockException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
