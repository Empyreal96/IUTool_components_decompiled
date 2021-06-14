using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Mock;
using System.Text;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000040 RID: 64
	internal class SimpleParameterInfo : ParameterInfo
	{
		// Token: 0x0600046D RID: 1133 RVA: 0x0000ECA3 File Offset: 0x0000CEA3
		internal SimpleParameterInfo(MemberInfo member, Type paramType, int position)
		{
			this.Member = member;
			this.ParameterType = paramType;
			this.Position = position;
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x0000ECC4 File Offset: 0x0000CEC4
		public override string ToString()
		{
			StringBuilder stringBuilder = StringBuilderPool.Get();
			MetadataOnlyCommonType.TypeSigToString(this.ParameterType, stringBuilder);
			stringBuilder.Append(' ');
			string result = stringBuilder.ToString();
			StringBuilderPool.Release(ref stringBuilder);
			return result;
		}

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x0600046F RID: 1135 RVA: 0x0000ED02 File Offset: 0x0000CF02
		public override int Position { get; }

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06000470 RID: 1136 RVA: 0x0000ED0A File Offset: 0x0000CF0A
		public override Type ParameterType { get; }

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06000471 RID: 1137 RVA: 0x0000ED12 File Offset: 0x0000CF12
		public override MemberInfo Member { get; }

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x06000472 RID: 1138 RVA: 0x0000ED1C File Offset: 0x0000CF1C
		public override int MetadataToken
		{
			get
			{
				return 134217728;
			}
		}

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000473 RID: 1139 RVA: 0x0000ED34 File Offset: 0x0000CF34
		public override ParameterAttributes Attributes
		{
			get
			{
				return ParameterAttributes.None;
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000474 RID: 1140 RVA: 0x0000ED48 File Offset: 0x0000CF48
		public override string Name
		{
			get
			{
				return string.Empty;
			}
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000475 RID: 1141 RVA: 0x0000ED60 File Offset: 0x0000CF60
		public override object DefaultValue
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000476 RID: 1142 RVA: 0x0000ED74 File Offset: 0x0000CF74
		public override object RawDefaultValue
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x0000ED88 File Offset: 0x0000CF88
		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return new CustomAttributeData[0];
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x0000EDA0 File Offset: 0x0000CFA0
		public override Type[] GetOptionalCustomModifiers()
		{
			return Type.EmptyTypes;
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x0000EDB8 File Offset: 0x0000CFB8
		public override Type[] GetRequiredCustomModifiers()
		{
			return Type.EmptyTypes;
		}
	}
}
