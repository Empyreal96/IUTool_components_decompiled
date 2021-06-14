using System;
using System.Globalization;

namespace System.Reflection.Mock
{
	// Token: 0x0200000B RID: 11
	internal abstract class ExceptionHandlingClause
	{
		// Token: 0x060000D1 RID: 209 RVA: 0x000033C4 File Offset: 0x000015C4
		public override string ToString()
		{
			bool flag = this.Flags == ExceptionHandlingClauseOptions.Clause;
			string result;
			if (flag)
			{
				result = string.Format(CultureInfo.CurrentUICulture, "Flags={0}, TryOffset={1}, TryLength={2}, HandlerOffset={3}, HandlerLength={4}, CatchType={5}", new object[]
				{
					this.Flags,
					this.TryOffset,
					this.TryLength,
					this.HandlerOffset,
					this.HandlerLength,
					this.CatchType
				});
			}
			else
			{
				bool flag2 = this.Flags == ExceptionHandlingClauseOptions.Filter;
				if (flag2)
				{
					result = string.Format(CultureInfo.CurrentUICulture, "Flags={0}, TryOffset={1}, TryLength={2}, HandlerOffset={3}, HandlerLength={4}, FilterOffset={5}", new object[]
					{
						this.Flags,
						this.TryOffset,
						this.TryLength,
						this.HandlerOffset,
						this.HandlerLength,
						this.FilterOffset
					});
				}
				else
				{
					result = string.Format(CultureInfo.CurrentUICulture, "Flags={0}, TryOffset={1}, TryLength={2}, HandlerOffset={3}, HandlerLength={4}", new object[]
					{
						this.Flags,
						this.TryOffset,
						this.TryLength,
						this.HandlerOffset,
						this.HandlerLength
					});
				}
			}
			return result;
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x00002087 File Offset: 0x00000287
		public virtual Type CatchType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000D3 RID: 211 RVA: 0x00002087 File Offset: 0x00000287
		public virtual int FilterOffset
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x00002087 File Offset: 0x00000287
		public virtual ExceptionHandlingClauseOptions Flags
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x00002087 File Offset: 0x00000287
		public virtual int HandlerLength
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x00002087 File Offset: 0x00000287
		public virtual int HandlerOffset
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x00002087 File Offset: 0x00000287
		public virtual int TryLength
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x00002087 File Offset: 0x00000287
		public virtual int TryOffset
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
