using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.Imaging.WimInterop
{
	// Token: 0x02000005 RID: 5
	public class ProcessFileEventArgs : EventArgs
	{
		// Token: 0x06000014 RID: 20 RVA: 0x00002433 File Offset: 0x00000633
		public ProcessFileEventArgs(string file, IntPtr skipFileFlag)
		{
			this._filePath = file;
			this._skipFileFlag = skipFileFlag;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000244C File Offset: 0x0000064C
		public void SkipFile()
		{
			byte[] array = new byte[1];
			int length = array.Length;
			Marshal.Copy(array, 0, this._skipFileFlag, length);
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000016 RID: 22 RVA: 0x00002470 File Offset: 0x00000670
		public string FilePath
		{
			get
			{
				string result = "";
				if (this._filePath != null)
				{
					result = this._filePath;
				}
				return result;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000018 RID: 24 RVA: 0x0000249C File Offset: 0x0000069C
		// (set) Token: 0x06000017 RID: 23 RVA: 0x00002493 File Offset: 0x00000693
		public bool Abort
		{
			get
			{
				return this._abort;
			}
			set
			{
				this._abort = value;
			}
		}

		// Token: 0x04000008 RID: 8
		private string _filePath;

		// Token: 0x04000009 RID: 9
		private bool _abort;

		// Token: 0x0400000A RID: 10
		private IntPtr _skipFileFlag;
	}
}
