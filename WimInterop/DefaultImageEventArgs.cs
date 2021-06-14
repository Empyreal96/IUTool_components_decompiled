using System;

namespace Microsoft.WindowsPhone.Imaging.WimInterop
{
	// Token: 0x02000004 RID: 4
	public class DefaultImageEventArgs : EventArgs
	{
		// Token: 0x06000010 RID: 16 RVA: 0x000023FE File Offset: 0x000005FE
		public DefaultImageEventArgs(IntPtr wideParameter, IntPtr leftParameter, IntPtr userData)
		{
			this._wParam = wideParameter;
			this._lParam = leftParameter;
			this._userData = userData;
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000011 RID: 17 RVA: 0x0000241B File Offset: 0x0000061B
		public IntPtr WideParameter
		{
			get
			{
				return this._wParam;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000012 RID: 18 RVA: 0x00002423 File Offset: 0x00000623
		public IntPtr LeftParameter
		{
			get
			{
				return this._lParam;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000013 RID: 19 RVA: 0x0000242B File Offset: 0x0000062B
		public IntPtr UserData
		{
			get
			{
				return this._userData;
			}
		}

		// Token: 0x04000005 RID: 5
		private IntPtr _wParam;

		// Token: 0x04000006 RID: 6
		private IntPtr _lParam;

		// Token: 0x04000007 RID: 7
		private IntPtr _userData;
	}
}
