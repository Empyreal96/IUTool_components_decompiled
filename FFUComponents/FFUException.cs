using System;
using System.Runtime.Serialization;

namespace FFUComponents
{
	// Token: 0x02000018 RID: 24
	[Serializable]
	public class FFUException : Exception
	{
		// Token: 0x06000090 RID: 144 RVA: 0x00003B04 File Offset: 0x00001D04
		public FFUException()
		{
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00003B0C File Offset: 0x00001D0C
		public FFUException(string message) : base(message)
		{
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00003B15 File Offset: 0x00001D15
		public FFUException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00003B20 File Offset: 0x00001D20
		protected FFUException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this.DeviceFriendlyName = (string)info.GetValue("DeviceFriendlyName", typeof(string));
			this.DeviceUniqueID = (Guid)info.GetValue("DeviceUniqueID", typeof(Guid));
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00003B83 File Offset: 0x00001D83
		protected new virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("DeviceFriendlyName", this.DeviceFriendlyName);
			info.AddValue("DeviceUniqueID", this.DeviceUniqueID);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00003BB4 File Offset: 0x00001DB4
		public FFUException(string deviceName, Guid deviceId, string message) : base(message)
		{
			this.DeviceFriendlyName = deviceName;
			this.DeviceUniqueID = deviceId;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00003BCB File Offset: 0x00001DCB
		public FFUException(IFFUDevice device)
		{
			if (device != null)
			{
				this.DeviceFriendlyName = device.DeviceFriendlyName;
				this.DeviceUniqueID = device.DeviceUniqueID;
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00003BEE File Offset: 0x00001DEE
		public FFUException(IFFUDevice device, string message, Exception e) : base(message, e)
		{
			if (device != null)
			{
				this.DeviceFriendlyName = device.DeviceFriendlyName;
				this.DeviceUniqueID = device.DeviceUniqueID;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000098 RID: 152 RVA: 0x00003C13 File Offset: 0x00001E13
		// (set) Token: 0x06000099 RID: 153 RVA: 0x00003C1B File Offset: 0x00001E1B
		public string DeviceFriendlyName { get; private set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600009A RID: 154 RVA: 0x00003C24 File Offset: 0x00001E24
		// (set) Token: 0x0600009B RID: 155 RVA: 0x00003C2C File Offset: 0x00001E2C
		public Guid DeviceUniqueID { get; private set; }
	}
}
