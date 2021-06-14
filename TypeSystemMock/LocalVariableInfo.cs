using System;

namespace System.Reflection.Mock
{
	// Token: 0x0200000D RID: 13
	internal abstract class LocalVariableInfo
	{
		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060000EF RID: 239
		public abstract bool IsPinned { get; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060000F0 RID: 240
		public abstract int LocalIndex { get; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060000F1 RID: 241
		public abstract Type LocalType { get; }

		// Token: 0x060000F2 RID: 242 RVA: 0x000036B0 File Offset: 0x000018B0
		public override string ToString()
		{
			string text = string.Concat(new object[]
			{
				this.LocalType.ToString(),
				" (",
				this.LocalIndex,
				")"
			});
			bool isPinned = this.IsPinned;
			if (isPinned)
			{
				text += " (pinned)";
			}
			return text;
		}
	}
}
