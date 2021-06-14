using System;
using System.Collections.Generic;
using Microsoft.WindowsPhone.MCSF.Offline;

namespace Microsoft.WindowsPhone.Multivariant.Offline
{
	// Token: 0x0200000B RID: 11
	public class MVSettingGroup
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600004D RID: 77 RVA: 0x00003808 File Offset: 0x00001A08
		// (set) Token: 0x0600004E RID: 78 RVA: 0x00003810 File Offset: 0x00001A10
		public string Path { get; private set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00003819 File Offset: 0x00001A19
		// (set) Token: 0x06000050 RID: 80 RVA: 0x00003821 File Offset: 0x00001A21
		public string PolicyPath
		{
			get
			{
				return this._policyPath;
			}
			private set
			{
				this._policyPath = PolicyMacroTable.MacroTildeToDollar(value);
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000051 RID: 81 RVA: 0x0000382F File Offset: 0x00001A2F
		// (set) Token: 0x06000052 RID: 82 RVA: 0x00003837 File Offset: 0x00001A37
		public List<MVSetting> Settings { get; private set; }

		// Token: 0x06000053 RID: 83 RVA: 0x00003840 File Offset: 0x00001A40
		public MVSettingGroup(string path, string policyPath)
		{
			this.Path = path;
			this.PolicyPath = policyPath;
			this.Settings = new List<MVSetting>();
		}

		// Token: 0x04000038 RID: 56
		private string _policyPath;
	}
}
