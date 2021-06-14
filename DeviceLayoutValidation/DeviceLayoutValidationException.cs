using System;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000003 RID: 3
	public class DeviceLayoutValidationException : IUException
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		public DeviceLayoutValidationError Error { get; private set; }

		// Token: 0x06000003 RID: 3 RVA: 0x00002061 File Offset: 0x00000261
		public DeviceLayoutValidationException(DeviceLayoutValidationError error, string msg) : base(msg)
		{
			this.Error = error;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002071 File Offset: 0x00000271
		public DeviceLayoutValidationException(DeviceLayoutValidationError error, string message, params object[] args) : base(message, args)
		{
			this.Error = error;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002082 File Offset: 0x00000282
		public DeviceLayoutValidationException(DeviceLayoutValidationError error, Exception inner, string msg) : base(inner, msg)
		{
			this.Error = error;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002093 File Offset: 0x00000293
		public DeviceLayoutValidationException(DeviceLayoutValidationError error, Exception innerException, string message, params object[] args) : base(innerException, message, args)
		{
			this.Error = error;
		}
	}
}
