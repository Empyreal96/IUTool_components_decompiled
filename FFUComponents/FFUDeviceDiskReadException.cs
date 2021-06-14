using System;
using System.Runtime.Serialization;

namespace FFUComponents
{
	// Token: 0x0200001D RID: 29
	[Serializable]
	public class FFUDeviceDiskReadException : FFUException
	{
		// Token: 0x060000B6 RID: 182 RVA: 0x00003C35 File Offset: 0x00001E35
		public FFUDeviceDiskReadException()
		{
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00003C3D File Offset: 0x00001E3D
		public FFUDeviceDiskReadException(string message) : base(message)
		{
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00003C46 File Offset: 0x00001E46
		public FFUDeviceDiskReadException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00003CD8 File Offset: 0x00001ED8
		public FFUDeviceDiskReadException(IFFUDevice device, string message, Exception e) : base(device, message, e)
		{
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00003CBD File Offset: 0x00001EBD
		protected FFUDeviceDiskReadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
