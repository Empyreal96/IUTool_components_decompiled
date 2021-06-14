using System;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000020 RID: 32
	internal class MetadataOnlyVectorType : MetadataOnlyCommonArrayType
	{
		// Token: 0x06000166 RID: 358 RVA: 0x00003A00 File Offset: 0x00001C00
		public MetadataOnlyVectorType(MetadataOnlyCommonType elementType) : base(elementType)
		{
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000167 RID: 359 RVA: 0x00003A0C File Offset: 0x00001C0C
		public override string FullName
		{
			get
			{
				string fullName = this.GetElementType().FullName;
				bool flag = fullName == null || this.GetElementType().IsGenericTypeDefinition;
				string result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = fullName + "[]";
				}
				return result;
			}
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00003A50 File Offset: 0x00001C50
		public override int GetArrayRank()
		{
			return 1;
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00003A64 File Offset: 0x00001C64
		protected override bool IsArrayImpl()
		{
			return true;
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00003A78 File Offset: 0x00001C78
		public override bool Equals(Type t)
		{
			bool flag = t == null;
			return !flag && (t is MetadataOnlyVectorType && t.GetArrayRank() == 1) && this.GetElementType().Equals(t.GetElementType());
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00003ABC File Offset: 0x00001CBC
		public override bool IsAssignableFrom(Type c)
		{
			bool flag = c == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = c.IsArray && c.GetArrayRank() == 1 && c is MetadataOnlyVectorType;
				if (flag2)
				{
					Type elementType = c.GetElementType();
					bool isValueType = elementType.IsValueType;
					if (isValueType)
					{
						result = this.GetElementType().Equals(elementType);
					}
					else
					{
						result = this.GetElementType().IsAssignableFrom(elementType);
					}
				}
				else
				{
					result = MetadataOnlyTypeDef.IsAssignableFromHelper(this, c);
				}
			}
			return result;
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600016C RID: 364 RVA: 0x00003B38 File Offset: 0x00001D38
		public override string Name
		{
			get
			{
				return this.GetElementType().Name + "[]";
			}
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00003B60 File Offset: 0x00001D60
		public override string ToString()
		{
			return this.GetElementType() + "[]";
		}
	}
}
