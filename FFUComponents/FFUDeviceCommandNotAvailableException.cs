using System;
using System.Runtime.Serialization;

namespace FFUComponents
{
	// Token: 0x0200001B RID: 27
	[Serializable]
	public class FFUDeviceCommandNotAvailableException : FFUException
	{
		// Token: 0x060000A9 RID: 169 RVA: 0x00003C35 File Offset: 0x00001E35
		public FFUDeviceCommandNotAvailableException()
		{
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00003C3D File Offset: 0x00001E3D
		public FFUDeviceCommandNotAvailableException(string message) : base(message)
		{
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00003C46 File Offset: 0x00001E46
		public FFUDeviceCommandNotAvailableException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00003CB4 File Offset: 0x00001EB4
		public FFUDeviceCommandNotAvailableException(IFFUDevice device) : base(device)
		{
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00003CBD File Offset: 0x00001EBD
		protected FFUDeviceCommandNotAvailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
