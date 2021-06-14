using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.WindowsPhone.Multivariant.Offline
{
	// Token: 0x0200000A RID: 10
	public class MVSetting
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000039 RID: 57 RVA: 0x000035F0 File Offset: 0x000017F0
		// (set) Token: 0x0600003A RID: 58 RVA: 0x000035F8 File Offset: 0x000017F8
		public string RegistryKey { get; private set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00003601 File Offset: 0x00001801
		// (set) Token: 0x0600003C RID: 60 RVA: 0x00003609 File Offset: 0x00001809
		public string RegistryValue { get; private set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00003612 File Offset: 0x00001812
		// (set) Token: 0x0600003E RID: 62 RVA: 0x0000361A File Offset: 0x0000181A
		public string RegType { get; private set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00003623 File Offset: 0x00001823
		// (set) Token: 0x06000040 RID: 64 RVA: 0x0000362B File Offset: 0x0000182B
		public IEnumerable<string> ProvisioningPath { get; private set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00003634 File Offset: 0x00001834
		// (set) Token: 0x06000042 RID: 66 RVA: 0x0000363C File Offset: 0x0000183C
		public MVSettingProvisioning ProvisioningTime { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00003645 File Offset: 0x00001845
		// (set) Token: 0x06000044 RID: 68 RVA: 0x0000364D File Offset: 0x0000184D
		public string Value { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000045 RID: 69 RVA: 0x00003656 File Offset: 0x00001856
		// (set) Token: 0x06000046 RID: 70 RVA: 0x0000365E File Offset: 0x0000185E
		public string Partition { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000047 RID: 71 RVA: 0x00003667 File Offset: 0x00001867
		// (set) Token: 0x06000048 RID: 72 RVA: 0x0000366F File Offset: 0x0000186F
		public string DataType { get; set; }

		// Token: 0x06000049 RID: 73 RVA: 0x00003678 File Offset: 0x00001878
		public MVSetting(IEnumerable<string> provisioningPath) : this(provisioningPath, null, null, null, null)
		{
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003685 File Offset: 0x00001885
		public MVSetting(IEnumerable<string> provisioningPath, string registryKey, string registryValue, string regType) : this(provisioningPath, registryKey, registryValue, regType, null)
		{
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00003694 File Offset: 0x00001894
		public MVSetting(IEnumerable<string> provisioningPath, string registryKey, string registryValue, string regType, string partition)
		{
			if (provisioningPath == null)
			{
				throw new ArgumentNullException("provisioningPath");
			}
			if (provisioningPath.Count<string>() == 0)
			{
				throw new ArgumentException("provisioningPath cannot be empty");
			}
			this.ProvisioningPath = provisioningPath;
			this.RegistryKey = registryKey;
			this.RegistryValue = registryValue;
			this.ProvisioningTime = MVSettingProvisioning.General;
			this.RegType = regType;
			this.Partition = partition;
			if (this.RegType == null)
			{
				throw new ArgumentException("RegType can not be null");
			}
			if (!MVSetting.DatatypeMapping.ContainsKey(this.RegType))
			{
				throw new ArgumentException("Unknown 'RegType': {0}", this.RegType);
			}
			this.DataType = MVSetting.DatatypeMapping[this.RegType];
		}

		// Token: 0x04000036 RID: 54
		private static readonly Dictionary<string, string> DatatypeMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"REG_DWORD",
				"integer"
			},
			{
				"REG_SZ",
				"string"
			},
			{
				"REG_EXPAND_SZ",
				"expandstring"
			},
			{
				"REG_MULTI_SZ",
				"multiplestring"
			},
			{
				"REG_BINARY",
				"binary"
			},
			{
				"CFG_DATATYPE_INTEGER",
				"integer"
			},
			{
				"CFG_DATATYPE_STRING",
				"string"
			},
			{
				"CFG_DATATYPE_MULTIPLE_STRING",
				"multiplestring"
			},
			{
				"CFG_DATATYPE_BOOLEAN",
				"boolean"
			},
			{
				"CFG_DATATYPE_BINARY",
				"binary"
			},
			{
				"CFG_DATATYPE_UNKNOWN",
				null
			}
		};
	}
}
