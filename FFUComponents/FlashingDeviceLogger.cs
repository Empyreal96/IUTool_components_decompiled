using System;
using System.Diagnostics.Eventing;
using System.IO;

namespace FFUComponents
{
	// Token: 0x02000024 RID: 36
	public class FlashingDeviceLogger : IDisposable
	{
		// Token: 0x060000E7 RID: 231 RVA: 0x00004917 File Offset: 0x00002B17
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.m_provider.Dispose();
			}
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00004927 File Offset: 0x00002B27
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00004938 File Offset: 0x00002B38
		public bool LogDeviceEvent(byte[] logData, Guid deviceUniqueId, string deviceFriendlyName, out string errInfo)
		{
			BinaryReader binaryReader = new BinaryReader(new MemoryStream(logData));
			binaryReader.ReadByte();
			int num = (int)binaryReader.ReadInt16();
			byte b = binaryReader.ReadByte();
			byte b2 = binaryReader.ReadByte();
			byte b3 = binaryReader.ReadByte();
			byte b4 = binaryReader.ReadByte();
			int num2 = (int)binaryReader.ReadInt16();
			long keywords = binaryReader.ReadInt64();
			EventDescriptor eventDescriptor = new EventDescriptor(num, b, b2, b3, b4, num2, keywords);
			string text = binaryReader.ReadString();
			if (b3 <= 2)
			{
				errInfo = string.Format("{{ 0x{0:x}, 0x{1:x}, 0x{2:x}, 0x{3:x}, 0x{4:x}, 0x{5:x} }}", new object[]
				{
					num,
					b,
					b2,
					b3,
					b4,
					num2
				});
				if (text != "")
				{
					errInfo = errInfo + " : " + text;
				}
			}
			else
			{
				errInfo = "";
			}
			return this.m_provider.TemplateDeviceEvent(ref eventDescriptor, deviceUniqueId, deviceFriendlyName, text);
		}

		// Token: 0x0400005E RID: 94
		internal DeviceEventProvider m_provider = new DeviceEventProvider(new Guid("3bbd891e-180f-4386-94b5-d71ba7ac25a9"));
	}
}
