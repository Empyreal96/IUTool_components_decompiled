using System;
using System.Globalization;
using System.Security.AccessControl;
using System.Text;

namespace Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy
{
	// Token: 0x02000053 RID: 83
	public class SdRegValue
	{
		// Token: 0x0600018D RID: 397 RVA: 0x0000A49B File Offset: 0x0000869B
		public SdRegValue(SdRegType SdRegValueType, string Name)
		{
			this.Init(SdRegValueType, Name, null, false);
		}

		// Token: 0x0600018E RID: 398 RVA: 0x0000A4AD File Offset: 0x000086AD
		public SdRegValue(SdRegType SdRegValueType, string Name, string QualifyingType, bool IsString)
		{
			this.Init(SdRegValueType, Name, QualifyingType, IsString);
		}

		// Token: 0x0600018F RID: 399 RVA: 0x0000A4C0 File Offset: 0x000086C0
		private void Init(SdRegType SdRegValueType, string Name, string QualifyingType, bool IsString)
		{
			this.m_Type = SdRegValueType;
			this.m_Name = Name;
			this.m_QualifyingType = QualifyingType;
			this.RegValueType = (IsString ? RegistryValueType.REG_SZ : RegistryValueType.REG_BINARY);
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000190 RID: 400 RVA: 0x0000A4E5 File Offset: 0x000086E5
		// (set) Token: 0x06000191 RID: 401 RVA: 0x0000A4ED File Offset: 0x000086ED
		public string Value { get; set; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000192 RID: 402 RVA: 0x0000A4F6 File Offset: 0x000086F6
		// (set) Token: 0x06000193 RID: 403 RVA: 0x0000A4FE File Offset: 0x000086FE
		public string AdditionalValue { get; set; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000194 RID: 404 RVA: 0x0000A507 File Offset: 0x00008707
		// (set) Token: 0x06000195 RID: 405 RVA: 0x0000A50F File Offset: 0x0000870F
		public RegistryValueType RegValueType { get; set; }

		// Token: 0x06000196 RID: 406 RVA: 0x0000A518 File Offset: 0x00008718
		public string GetRegPath()
		{
			switch (this.m_Type)
			{
			case SdRegType.TransientObject:
				return "HKEY_LOCAL_MACHINE\\Software\\Microsoft\\SecurityManager\\TransientObjects\\%5C%5C.%5C" + this.m_QualifyingType + "%5C" + this.m_Name.Replace("\\", "%5C");
			case SdRegType.Com:
				return "HKEY_LOCAL_MACHINE\\Software\\Classes\\AppId\\" + this.m_Name;
			case SdRegType.WinRt:
				return "HKEY_LOCAL_MACHINE\\Software\\Microsoft\\WindowsRuntime\\Server\\" + this.m_Name;
			case SdRegType.EtwProvider:
				return "HKEY_LOCAL_MACHINE\\System\\CurrentControlSet\\Control\\WMI\\Security";
			case SdRegType.Generic:
				return this.m_Name.Substring(0, this.m_Name.LastIndexOf('\\'));
			}
			throw new PkgGenException("Invalid SDReg type. Failed to return registry path");
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000A5D4 File Offset: 0x000087D4
		public string GetUniqueIdentifier()
		{
			switch (this.m_Type)
			{
			case SdRegType.TransientObject:
				return this.m_QualifyingType + this.m_Name;
			case SdRegType.Com:
				return this.m_Name;
			case SdRegType.WinRt:
				return this.m_Name;
			case SdRegType.EtwProvider:
				return this.m_Name;
			case SdRegType.Generic:
				return this.m_Name;
			}
			throw new PkgGenException("Invalid SDReg type. Failed to return unique identifier");
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000A64E File Offset: 0x0000884E
		public bool HasAdditionalValue()
		{
			return this.m_Type == SdRegType.Com;
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000A659 File Offset: 0x00008859
		public string GetRegValueName()
		{
			return this.GetRegValueName(false);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0000A664 File Offset: 0x00008864
		public string GetRegValueName(bool GetAdditional)
		{
			switch (this.m_Type)
			{
			case SdRegType.TransientObject:
				return "SecurityDescriptor";
			case SdRegType.Com:
				return GetAdditional ? "LaunchPermission" : "AccessPermission";
			case SdRegType.WinRt:
				return "Permissions";
			case SdRegType.EtwProvider:
				return this.m_Name;
			case SdRegType.Generic:
				return this.m_Name.Substring(this.m_Name.LastIndexOf('\\') + 1);
			}
			throw new PkgGenException("Invalid SDReg type. Failed to return registry value name");
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000A6EE File Offset: 0x000088EE
		public string GetRegValue()
		{
			return this.GetRegValue(false);
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000A6F8 File Offset: 0x000088F8
		public string GetRegValue(bool GetAdditional)
		{
			string text = GetAdditional ? this.AdditionalValue : this.Value;
			if (text != null && this.RegValueType == RegistryValueType.REG_BINARY)
			{
				RegistrySecurity registrySecurity = new RegistrySecurity();
				registrySecurity.SetSecurityDescriptorSddlForm(text);
				byte[] securityDescriptorBinaryForm = registrySecurity.GetSecurityDescriptorBinaryForm();
				StringBuilder stringBuilder = new StringBuilder();
				foreach (byte b in securityDescriptorBinaryForm)
				{
					stringBuilder.Append(b.ToString("X2", CultureInfo.InvariantCulture));
				}
				return stringBuilder.ToString();
			}
			return text;
		}

		// Token: 0x04000108 RID: 264
		private SdRegType m_Type;

		// Token: 0x04000109 RID: 265
		private string m_Name;

		// Token: 0x0400010A RID: 266
		private string m_QualifyingType;
	}
}
