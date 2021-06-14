using System;
using System.Reflection.Mock;
using System.Text;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200001F RID: 31
	internal class MetadataOnlyArrayType : MetadataOnlyCommonArrayType
	{
		// Token: 0x0600015E RID: 350 RVA: 0x000037DB File Offset: 0x000019DB
		public MetadataOnlyArrayType(MetadataOnlyCommonType elementType, int rank) : base(elementType)
		{
			this._rank = rank;
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600015F RID: 351 RVA: 0x000037F0 File Offset: 0x000019F0
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
					result = fullName + "[" + MetadataOnlyArrayType.GetDimensionString(this._rank) + "]";
				}
				return result;
			}
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00003844 File Offset: 0x00001A44
		private static string GetDimensionString(int rank)
		{
			bool flag = rank == 1;
			string result;
			if (flag)
			{
				result = "*";
			}
			else
			{
				StringBuilder stringBuilder = StringBuilderPool.Get();
				for (int i = 1; i < rank; i++)
				{
					stringBuilder.Append(',');
				}
				string text = stringBuilder.ToString();
				StringBuilderPool.Release(ref stringBuilder);
				result = text;
			}
			return result;
		}

		// Token: 0x06000161 RID: 353 RVA: 0x000038A0 File Offset: 0x00001AA0
		public override int GetArrayRank()
		{
			return this._rank;
		}

		// Token: 0x06000162 RID: 354 RVA: 0x000038B8 File Offset: 0x00001AB8
		public override bool Equals(Type t)
		{
			bool flag = t == null;
			return !flag && (t is MetadataOnlyArrayType && t.GetArrayRank() == this.GetArrayRank()) && this.GetElementType().Equals(t.GetElementType());
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00003904 File Offset: 0x00001B04
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
				bool isArray = c.IsArray;
				if (isArray)
				{
					bool flag2 = c.GetArrayRank() != this._rank;
					if (flag2)
					{
						result = false;
					}
					else
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
				}
				else
				{
					result = MetadataOnlyTypeDef.IsAssignableFromHelper(this, c);
				}
			}
			return result;
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000164 RID: 356 RVA: 0x00003984 File Offset: 0x00001B84
		public override string Name
		{
			get
			{
				return this.GetElementType().Name + "[" + MetadataOnlyArrayType.GetDimensionString(this._rank) + "]";
			}
		}

		// Token: 0x06000165 RID: 357 RVA: 0x000039BC File Offset: 0x00001BBC
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.GetElementType(),
				"[",
				MetadataOnlyArrayType.GetDimensionString(this._rank),
				"]"
			});
		}

		// Token: 0x04000054 RID: 84
		private readonly int _rank;
	}
}
