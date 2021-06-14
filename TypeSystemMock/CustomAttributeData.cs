using System;
using System.Collections.Generic;

namespace System.Reflection.Mock
{
	// Token: 0x02000007 RID: 7
	internal class CustomAttributeData
	{
		// Token: 0x060000B5 RID: 181 RVA: 0x000032B4 File Offset: 0x000014B4
		protected CustomAttributeData()
		{
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x00002087 File Offset: 0x00000287
		public virtual ConstructorInfo Constructor
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x00002087 File Offset: 0x00000287
		public virtual IList<CustomAttributeTypedArgument> ConstructorArguments
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x00002087 File Offset: 0x00000287
		public virtual IList<CustomAttributeNamedArgument> NamedArguments
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
