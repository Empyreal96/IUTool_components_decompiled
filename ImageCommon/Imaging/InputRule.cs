using System;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000023 RID: 35
	public abstract class InputRule
	{
		// Token: 0x17000062 RID: 98
		// (get) Token: 0x0600017C RID: 380 RVA: 0x0000DA5E File Offset: 0x0000BC5E
		// (set) Token: 0x0600017D RID: 381 RVA: 0x0000DA66 File Offset: 0x0000BC66
		public string Property { get; set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x0600017E RID: 382 RVA: 0x0000DA6F File Offset: 0x0000BC6F
		// (set) Token: 0x0600017F RID: 383 RVA: 0x0000DA77 File Offset: 0x0000BC77
		public string Mode { get; set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000180 RID: 384 RVA: 0x0000DA80 File Offset: 0x0000BC80
		public char ModeCharacter
		{
			get
			{
				if (string.CompareOrdinal(this.Mode, "AFFIRMATIVE") == 0)
				{
					return 'A';
				}
				if (string.CompareOrdinal(this.Mode, "NEGATIVE") == 0)
				{
					return 'N';
				}
				if (string.CompareOrdinal(this.Mode, "OPTIONAL") == 0)
				{
					return 'O';
				}
				throw new ArgumentException("Mode");
			}
		}
	}
}
