using System;
using System.Runtime.Serialization;

namespace FFUComponents
{
	// Token: 0x0200001A RID: 26
	[Serializable]
	public class FFUDeviceNotReadyException : FFUException
	{
		// Token: 0x060000A4 RID: 164 RVA: 0x00003C35 File Offset: 0x00001E35
		public FFUDeviceNotReadyException()
		{
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00003C3D File Offset: 0x00001E3D
		public FFUDeviceNotReadyException(string message) : base(message)
		{
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00003C46 File Offset: 0x00001E46
		public FFUDeviceNotReadyException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00003CB4 File Offset: 0x00001EB4
		public FFUDeviceNotReadyException(IFFUDevice device) : base(device)
		{
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00003CBD File Offset: 0x00001EBD
		protected FFUDeviceNotReadyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
