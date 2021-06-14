using System;
using System.Windows.Forms;
using DeviceHealth;

namespace UEFIUSBFnDevTester
{
	// Token: 0x02000003 RID: 3
	internal static class Program
	{
		// Token: 0x0600001C RID: 28 RVA: 0x00003B89 File Offset: 0x00001D89
		[STAThread]
		private static void Main()
		{
			Log.Name = "UEFI";
			Log.Source = "USBSample";
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}

		// Token: 0x04000024 RID: 36
		private static Random random = new Random();

		// Token: 0x04000025 RID: 37
		private const int MTU = 16376;

		// Token: 0x04000026 RID: 38
		private const int USB_FS_MAX_PACKET_SIZE = 64;

		// Token: 0x04000027 RID: 39
		private const int USB_HS_MAX_PACKET_SIZE = 512;
	}
}
