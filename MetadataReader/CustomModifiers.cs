using System;
using System.Collections.Generic;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200000C RID: 12
	internal class CustomModifiers
	{
		// Token: 0x06000043 RID: 67 RVA: 0x00002927 File Offset: 0x00000B27
		public CustomModifiers(List<Type> optModifiers, List<Type> reqModifiers)
		{
			this._optional = optModifiers;
			this._required = reqModifiers;
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00002940 File Offset: 0x00000B40
		public Type[] OptionalCustomModifiers
		{
			get
			{
				bool flag = this._optional != null;
				Type[] result;
				if (flag)
				{
					result = this._optional.ToArray();
				}
				else
				{
					result = Type.EmptyTypes;
				}
				return result;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000045 RID: 69 RVA: 0x00002974 File Offset: 0x00000B74
		public Type[] RequiredCustomModifiers
		{
			get
			{
				bool flag = this._required != null;
				Type[] result;
				if (flag)
				{
					result = this._required.ToArray();
				}
				else
				{
					result = Type.EmptyTypes;
				}
				return result;
			}
		}

		// Token: 0x04000009 RID: 9
		private readonly List<Type> _optional;

		// Token: 0x0400000A RID: 10
		private readonly List<Type> _required;
	}
}
