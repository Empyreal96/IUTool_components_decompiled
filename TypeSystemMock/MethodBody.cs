using System;
using System.Collections.Generic;

namespace System.Reflection.Mock
{
	// Token: 0x02000011 RID: 17
	internal abstract class MethodBody
	{
		// Token: 0x1700007C RID: 124
		// (get) Token: 0x0600011F RID: 287 RVA: 0x00002087 File Offset: 0x00000287
		public virtual IList<ExceptionHandlingClause> ExceptionHandlingClauses
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000120 RID: 288 RVA: 0x00002087 File Offset: 0x00000287
		public virtual bool InitLocals
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000121 RID: 289 RVA: 0x00002087 File Offset: 0x00000287
		public virtual int LocalSignatureMetadataToken
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000122 RID: 290 RVA: 0x00002087 File Offset: 0x00000287
		public virtual IList<LocalVariableInfo> LocalVariables
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000123 RID: 291 RVA: 0x00002087 File Offset: 0x00000287
		public virtual int MaxStackSize
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00002087 File Offset: 0x00000287
		public virtual byte[] GetILAsByteArray()
		{
			throw new NotImplementedException();
		}
	}
}
