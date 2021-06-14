using System;
using System.Reflection;

namespace Microsoft.Phone.Test.TestMetadata.CommandLine
{
	// Token: 0x02000026 RID: 38
	public class OptionSpecification
	{
		// Token: 0x060000E1 RID: 225 RVA: 0x00004BF3 File Offset: 0x00002DF3
		public OptionSpecification(OptionAttribute attribute, PropertyInfo relatedProperty)
		{
			this._attribute = attribute;
			this.RelatedProperty = relatedProperty;
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x00004C0C File Offset: 0x00002E0C
		public string OptionName
		{
			get
			{
				return this._attribute.Name;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x00004C2C File Offset: 0x00002E2C
		public OptionValueType ValueType
		{
			get
			{
				return this._attribute.ValueType;
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x00004C4C File Offset: 0x00002E4C
		public bool IsFinalOption
		{
			get
			{
				return this._attribute.IsFinalOption;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000E5 RID: 229 RVA: 0x00004C6C File Offset: 0x00002E6C
		public bool IsMultipleValue
		{
			get
			{
				return this._attribute.IsMultipleValue;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x00004C89 File Offset: 0x00002E89
		public PropertyInfo RelatedProperty { get; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000E7 RID: 231 RVA: 0x00004C94 File Offset: 0x00002E94
		public string DefaultValue
		{
			get
			{
				return this._attribute.DefaultValue;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x00004CB4 File Offset: 0x00002EB4
		public string Description
		{
			get
			{
				return this._attribute.Description;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x00004CD4 File Offset: 0x00002ED4
		public Type CollectionType
		{
			get
			{
				return this._attribute.CollectionType;
			}
		}

		// Token: 0x040000AE RID: 174
		private readonly OptionAttribute _attribute;
	}
}
