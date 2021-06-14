using System;
using System.Globalization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200005C RID: 92
	public class BcdRegValue
	{
		// Token: 0x06000446 RID: 1094 RVA: 0x000133C0 File Offset: 0x000115C0
		public BcdRegValue(string name, string value, string type)
		{
			if (!(type == "REG_BINARY"))
			{
				if (type == "REG_DWORD")
				{
					value = string.Format("0x{0}", value.ToUpper(CultureInfo.InvariantCulture));
				}
			}
			else
			{
				value = this.TrimBinary(value).ToUpper(CultureInfo.InvariantCulture);
			}
			this._name = name;
			this._value = value;
			this._type = type;
		}

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x06000447 RID: 1095 RVA: 0x00013430 File Offset: 0x00011630
		public string Name
		{
			get
			{
				return this._name;
			}
		}

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06000448 RID: 1096 RVA: 0x00013438 File Offset: 0x00011638
		public string Value
		{
			get
			{
				return this._value;
			}
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06000449 RID: 1097 RVA: 0x00013440 File Offset: 0x00011640
		public string Type
		{
			get
			{
				return this._type;
			}
		}

		// Token: 0x0600044A RID: 1098 RVA: 0x00013448 File Offset: 0x00011648
		private string TrimBinary(string regBinaryStr)
		{
			return regBinaryStr.Replace("\r\n", "").Replace(",", "").Replace("\\", "").Trim();
		}

		// Token: 0x04000260 RID: 608
		private string _name;

		// Token: 0x04000261 RID: 609
		private string _value;

		// Token: 0x04000262 RID: 610
		private string _type;
	}
}
