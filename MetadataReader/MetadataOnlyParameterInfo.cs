using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;
using System.Text;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200002D RID: 45
	internal class MetadataOnlyParameterInfo : ParameterInfo
	{
		// Token: 0x060002ED RID: 749 RVA: 0x0000A264 File Offset: 0x00008464
		internal MetadataOnlyParameterInfo(MetadataOnlyModule resolver, Token parameterToken, Type paramType, CustomModifiers customModifiers)
		{
			this._resolver = resolver;
			this._parameterToken = parameterToken;
			this.ParameterType = paramType;
			this._customModifiers = customModifiers;
			IMetadataImport rawImport = this._resolver.RawImport;
			uint num;
			uint num2;
			uint num3;
			UnusedIntPtr unusedIntPtr;
			uint num4;
			rawImport.GetParamProps(this._parameterToken, out this._parentMemberToken, out num, null, 0U, out this._nameLength, out num2, out num3, out unusedIntPtr, out num4);
			this.Position = num - 1U;
			this.Attributes = num2;
		}

		// Token: 0x060002EE RID: 750 RVA: 0x0000A2DC File Offset: 0x000084DC
		private void InitializeName()
		{
			bool flag = string.IsNullOrEmpty(this._name);
			if (flag)
			{
				IMetadataImport rawImport = this._resolver.RawImport;
				StringBuilder stringBuilder = StringBuilderPool.Get((int)this._nameLength);
				int num;
				uint num2;
				uint num3;
				uint num4;
				uint num5;
				UnusedIntPtr unusedIntPtr;
				uint num6;
				rawImport.GetParamProps(this._parameterToken, out num, out num2, stringBuilder, (uint)stringBuilder.Capacity, out num3, out num4, out num5, out unusedIntPtr, out num6);
				this._name = stringBuilder.ToString();
				StringBuilderPool.Release(ref stringBuilder);
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060002EF RID: 751 RVA: 0x0000A34D File Offset: 0x0000854D
		public override ParameterAttributes Attributes { get; }

		// Token: 0x060002F0 RID: 752 RVA: 0x0000A358 File Offset: 0x00008558
		public override Type[] GetOptionalCustomModifiers()
		{
			bool flag = this._customModifiers == null;
			Type[] result;
			if (flag)
			{
				result = Type.EmptyTypes;
			}
			else
			{
				result = this._customModifiers.OptionalCustomModifiers;
			}
			return result;
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x0000A38C File Offset: 0x0000858C
		public override Type[] GetRequiredCustomModifiers()
		{
			bool flag = this._customModifiers == null;
			Type[] result;
			if (flag)
			{
				result = Type.EmptyTypes;
			}
			else
			{
				result = this._customModifiers.RequiredCustomModifiers;
			}
			return result;
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060002F2 RID: 754 RVA: 0x0000A3C0 File Offset: 0x000085C0
		public override string Name
		{
			get
			{
				this.InitializeName();
				return this._name;
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060002F3 RID: 755 RVA: 0x0000A3E0 File Offset: 0x000085E0
		public override MemberInfo Member
		{
			get
			{
				return this._resolver.ResolveMethod(this._parentMemberToken);
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060002F4 RID: 756 RVA: 0x0000A403 File Offset: 0x00008603
		public override int Position { get; }

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060002F5 RID: 757 RVA: 0x0000A40B File Offset: 0x0000860B
		public override Type ParameterType { get; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060002F6 RID: 758 RVA: 0x0000A414 File Offset: 0x00008614
		public override int MetadataToken
		{
			get
			{
				return this._parameterToken;
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060002F7 RID: 759 RVA: 0x00002390 File Offset: 0x00000590
		public override object DefaultValue
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060002F8 RID: 760 RVA: 0x0000232A File Offset: 0x0000052A
		public override object RawDefaultValue
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x0000A42C File Offset: 0x0000862C
		public override bool Equals(object obj)
		{
			MetadataOnlyParameterInfo metadataOnlyParameterInfo = obj as MetadataOnlyParameterInfo;
			bool flag = metadataOnlyParameterInfo != null;
			return flag && metadataOnlyParameterInfo._resolver.Equals(this._resolver) && metadataOnlyParameterInfo._parameterToken.Equals(this._parameterToken);
		}

		// Token: 0x060002FA RID: 762 RVA: 0x0000A47C File Offset: 0x0000867C
		public override int GetHashCode()
		{
			return this._resolver.GetHashCode() * 32767 + this._parameterToken.GetHashCode();
		}

		// Token: 0x060002FB RID: 763 RVA: 0x0000A4B0 File Offset: 0x000086B0
		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this._resolver.GetCustomAttributeData(this.MetadataToken);
		}

		// Token: 0x060002FC RID: 764 RVA: 0x0000A4D4 File Offset: 0x000086D4
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				MetadataOnlyCommonType.TypeSigToString(this.ParameterType),
				this.Name
			});
		}

		// Token: 0x0400009B RID: 155
		private readonly MetadataOnlyModule _resolver;

		// Token: 0x0400009C RID: 156
		private readonly int _parameterToken;

		// Token: 0x0400009D RID: 157
		private readonly CustomModifiers _customModifiers;

		// Token: 0x0400009E RID: 158
		private string _name;

		// Token: 0x0400009F RID: 159
		private readonly uint _nameLength;

		// Token: 0x040000A0 RID: 160
		private readonly int _parentMemberToken;
	}
}
