using System;
using System.Text;
using Microsoft.Win32;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000005 RID: 5
	public class BcdElementDataType
	{
		// Token: 0x06000018 RID: 24 RVA: 0x00003ABC File Offset: 0x00001CBC
		public BcdElementDataType()
		{
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00003AC4 File Offset: 0x00001CC4
		[CLSCompliant(false)]
		public BcdElementDataType(uint dataType)
		{
			this.RawValue = dataType;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00003AD3 File Offset: 0x00001CD3
		[CLSCompliant(false)]
		public BcdElementDataType(ElementClass elementClass, ElementFormat elementFormat, uint elementSubClass)
		{
			this.Class = elementClass;
			this.Format = elementFormat;
			this.SubClass = elementSubClass;
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600001B RID: 27 RVA: 0x00003AF0 File Offset: 0x00001CF0
		// (set) Token: 0x0600001C RID: 28 RVA: 0x00003AF8 File Offset: 0x00001CF8
		[CLSCompliant(false)]
		public uint RawValue { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00003B01 File Offset: 0x00001D01
		// (set) Token: 0x0600001E RID: 30 RVA: 0x00003B12 File Offset: 0x00001D12
		[CLSCompliant(false)]
		public ElementClass Class
		{
			get
			{
				return (ElementClass)((this.RawValue & 4026531840U) >> 28);
			}
			set
			{
				this.RawValue = ((this.RawValue & 268435455U) | (uint)((uint)value << 28));
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00003B2B File Offset: 0x00001D2B
		// (set) Token: 0x06000020 RID: 32 RVA: 0x00003B3C File Offset: 0x00001D3C
		[CLSCompliant(false)]
		public ElementFormat Format
		{
			get
			{
				return (ElementFormat)((this.RawValue & 251658240U) >> 24);
			}
			set
			{
				this.RawValue = ((this.RawValue & 4043309055U) | (uint)((uint)value << 24));
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00003B55 File Offset: 0x00001D55
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00003B63 File Offset: 0x00001D63
		[CLSCompliant(false)]
		public uint SubClass
		{
			get
			{
				return this.RawValue & 16777215U;
			}
			set
			{
				this.RawValue = ((this.RawValue & 4278190080U) | (value & 16777215U));
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00003B80 File Offset: 0x00001D80
		public RegistryValueKind RegistryValueType
		{
			get
			{
				switch (this.Format)
				{
				case ElementFormat.Device:
					return RegistryValueKind.Binary;
				case ElementFormat.String:
					return RegistryValueKind.String;
				case ElementFormat.Object:
					return RegistryValueKind.String;
				case ElementFormat.ObjectList:
					return RegistryValueKind.MultiString;
				case ElementFormat.Integer:
					return RegistryValueKind.Binary;
				case ElementFormat.Boolean:
					return RegistryValueKind.Binary;
				case ElementFormat.IntegerList:
					return RegistryValueKind.Binary;
				default:
					return RegistryValueKind.Binary;
				}
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00003BCC File Offset: 0x00001DCC
		public override bool Equals(object obj)
		{
			BcdElementDataType bcdElementDataType = obj as BcdElementDataType;
			return bcdElementDataType != null && (bcdElementDataType.Format == this.Format && bcdElementDataType.Class == this.Class && bcdElementDataType.SubClass == this.SubClass);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00003C12 File Offset: 0x00001E12
		public override int GetHashCode()
		{
			return (int)(this.Format ^ (ElementFormat)this.Class ^ (ElementFormat)this.SubClass);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00003C28 File Offset: 0x00001E28
		[CLSCompliant(false)]
		public void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			logger.LogInfo(str + "Class:              {0}", new object[]
			{
				this.Class
			});
			logger.LogInfo(str + "Format:             {0}", new object[]
			{
				this.Format
			});
			if (BcdElementDataTypes.ApplicationTypes.ContainsKey(this))
			{
				logger.LogInfo(str + "SubClass:           {0} (0x{1:x})", new object[]
				{
					BcdElementDataTypes.ApplicationTypes[this],
					this.SubClass
				});
			}
			else if (BcdElementDataTypes.LibraryTypes.ContainsKey(this))
			{
				logger.LogInfo(str + "SubClass:           {0} (0x{1:x})", new object[]
				{
					BcdElementDataTypes.LibraryTypes[this],
					this.SubClass
				});
			}
			else if (BcdElementDataTypes.DeviceTypes.ContainsKey(this))
			{
				logger.LogInfo(str + "SubClass:           {0} (0x{1:x})", new object[]
				{
					BcdElementDataTypes.DeviceTypes[this],
					this.SubClass
				});
			}
			else
			{
				logger.LogInfo(str + "SubClass:           0x{0:x}", new object[]
				{
					this.SubClass
				});
			}
			logger.LogInfo(str + "Registry Data Type: {0}", new object[]
			{
				this.RegistryValueType
			});
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00003DA5 File Offset: 0x00001FA5
		public override string ToString()
		{
			return string.Format("{0:x8}", this.RawValue);
		}
	}
}
