using System;
using System.Runtime.Serialization;

namespace FFUComponents
{
	// Token: 0x0200001E RID: 30
	[Serializable]
	public class FFUDeviceDiskWriteException : FFUException
	{
		// Token: 0x060000BB RID: 187 RVA: 0x00003C35 File Offset: 0x00001E35
		public FFUDeviceDiskWriteException()
		{
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00003C3D File Offset: 0x00001E3D
		public FFUDeviceDiskWriteException(string message) : base(message)
		{
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00003C46 File Offset: 0x00001E46
		public FFUDeviceDiskWriteException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00003CD8 File Offset: 0x00001ED8
		public FFUDeviceDiskWriteException(IFFUDevice device, string message, Exception e) : base(device, message, e)
		{
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00003CBD File Offset: 0x00001EBD
		protected FFUDeviceDiskWriteException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
