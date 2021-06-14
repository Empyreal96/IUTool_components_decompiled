using System;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000015 RID: 21
	internal struct HCORENUM
	{
		// Token: 0x0600007D RID: 125 RVA: 0x00002FB0 File Offset: 0x000011B0
		public void Close(IMetadataImport import)
		{
			bool flag = this._hEnum != IntPtr.Zero;
			if (flag)
			{
				import.CloseEnum(this._hEnum);
				this._hEnum = IntPtr.Zero;
			}
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00002FEC File Offset: 0x000011EC
		public void Close(IMetadataImport2 import)
		{
			bool flag = this._hEnum != IntPtr.Zero;
			if (flag)
			{
				import.CloseEnum(this._hEnum);
				this._hEnum = IntPtr.Zero;
			}
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00003028 File Offset: 0x00001228
		public void Close(IMetadataAssemblyImport import)
		{
			bool flag = this._hEnum != IntPtr.Zero;
			if (flag)
			{
				import.CloseEnum(this._hEnum);
				this._hEnum = IntPtr.Zero;
			}
		}

		// Token: 0x04000043 RID: 67
		private IntPtr _hEnum;
	}
}
