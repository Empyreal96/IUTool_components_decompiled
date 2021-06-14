using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Microsoft.Win32;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000006 RID: 6
	public class BcdElement
	{
		// Token: 0x06000028 RID: 40 RVA: 0x00003DBC File Offset: 0x00001FBC
		public override string ToString()
		{
			if (this.DataType != null)
			{
				return this.DataType.ToString();
			}
			return "Unnamed BcdElement";
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00003DD8 File Offset: 0x00001FD8
		public static BcdElement CreateElement(OfflineRegistryHandle elementKey)
		{
			BcdElementDataType bcdElementDataType = new BcdElementDataType();
			byte[] binaryData = null;
			string text = null;
			string[] multiStringData = null;
			uint rawValue = uint.Parse(elementKey.Name.Substring(elementKey.Name.LastIndexOf('\\') + 1), NumberStyles.HexNumber);
			bcdElementDataType.RawValue = rawValue;
			RegistryValueKind registryValueType = bcdElementDataType.RegistryValueType;
			if (registryValueType != RegistryValueKind.String)
			{
				if (registryValueType != RegistryValueKind.Binary)
				{
					if (registryValueType != RegistryValueKind.MultiString)
					{
						return null;
					}
					multiStringData = (string[])elementKey.GetValue("Element", null);
				}
				else
				{
					binaryData = (byte[])elementKey.GetValue("Element", null);
				}
			}
			else
			{
				text = (string)elementKey.GetValue("Element", string.Empty);
			}
			BcdElement result;
			switch (bcdElementDataType.Format)
			{
			case ElementFormat.Device:
				result = new BcdElementDevice(binaryData, bcdElementDataType);
				break;
			case ElementFormat.String:
				result = new BcdElementString(text, bcdElementDataType);
				break;
			case ElementFormat.Object:
				result = new BcdElementObject(text, bcdElementDataType);
				break;
			case ElementFormat.ObjectList:
				result = new BcdElementObjectList(multiStringData, bcdElementDataType);
				break;
			case ElementFormat.Integer:
				result = new BcdElementInteger(binaryData, bcdElementDataType);
				break;
			case ElementFormat.Boolean:
				result = new BcdElementBoolean(binaryData, bcdElementDataType);
				break;
			case ElementFormat.IntegerList:
				result = new BcdElementIntegerList(binaryData, bcdElementDataType);
				break;
			default:
				throw new ImageStorageException(string.Format("{0}: Unknown element format: {1}.", MethodBase.GetCurrentMethod().Name, bcdElementDataType.RawValue));
			}
			return result;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00003F1A File Offset: 0x0000211A
		protected BcdElement(BcdElementDataType dataType)
		{
			this.DataType = dataType;
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600002B RID: 43 RVA: 0x00003F29 File Offset: 0x00002129
		// (set) Token: 0x0600002C RID: 44 RVA: 0x00003F31 File Offset: 0x00002131
		public BcdElementDataType DataType { get; set; }

		// Token: 0x0600002D RID: 45 RVA: 0x00003F3A File Offset: 0x0000213A
		public byte[] GetBinaryData()
		{
			return this._binaryData;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00003F44 File Offset: 0x00002144
		public void SetBinaryData(byte[] binaryData)
		{
			if (this.DataType.RegistryValueType != RegistryValueKind.Binary)
			{
				throw new ImageStorageException(string.Format("{0}: Cannot set binary data for an element format: {1}", MethodBase.GetCurrentMethod().Name, this.DataType.Format));
			}
			this._binaryData = binaryData;
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00003F90 File Offset: 0x00002190
		// (set) Token: 0x06000030 RID: 48 RVA: 0x00003F98 File Offset: 0x00002198
		public string StringData
		{
			get
			{
				return this._stringData;
			}
			set
			{
				if (this.DataType.RegistryValueType != RegistryValueKind.String)
				{
					throw new ImageStorageException(string.Format("{0}: Cannot set string data for an element format: {1}", MethodBase.GetCurrentMethod().Name, this.DataType.Format));
				}
				this._stringData = value;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00003FE4 File Offset: 0x000021E4
		public List<string> MultiStringData
		{
			get
			{
				return this._multiStringData;
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003FEC File Offset: 0x000021EC
		[CLSCompliant(false)]
		public virtual void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			logger.LogInfo(str + "BCD Element:        {0:x}", new object[]
			{
				this.DataType.RawValue
			});
			this.DataType.LogInfo(logger, indentLevel);
		}

		// Token: 0x04000090 RID: 144
		private byte[] _binaryData;

		// Token: 0x04000091 RID: 145
		private string _stringData;

		// Token: 0x04000092 RID: 146
		protected List<string> _multiStringData;
	}
}
