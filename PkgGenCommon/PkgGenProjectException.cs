using System;
using System.IO;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon
{
	// Token: 0x0200000E RID: 14
	public class PkgGenProjectException : PkgGenException
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000034 RID: 52 RVA: 0x000025AB File Offset: 0x000007AB
		// (set) Token: 0x06000035 RID: 53 RVA: 0x000025B3 File Offset: 0x000007B3
		public string ProjectPath { get; private set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000036 RID: 54 RVA: 0x000025BC File Offset: 0x000007BC
		// (set) Token: 0x06000037 RID: 55 RVA: 0x000025C4 File Offset: 0x000007C4
		public int LineNumber { get; private set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000038 RID: 56 RVA: 0x000025CD File Offset: 0x000007CD
		// (set) Token: 0x06000039 RID: 57 RVA: 0x000025D5 File Offset: 0x000007D5
		public int LinePosition { get; private set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600003A RID: 58 RVA: 0x000025DE File Offset: 0x000007DE
		// (set) Token: 0x0600003B RID: 59 RVA: 0x000025E6 File Offset: 0x000007E6
		public bool HasLineInfo { get; private set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600003C RID: 60 RVA: 0x000025F0 File Offset: 0x000007F0
		public override string Message
		{
			get
			{
				if (this.HasLineInfo)
				{
					return string.Format("{0}({1},{2}): {3}", new object[]
					{
						Path.GetFileName(this.ProjectPath),
						this.LineNumber,
						this.LinePosition,
						base.Message
					});
				}
				return string.Format("{0}: {1}", Path.GetFileName(this.ProjectPath), base.Message);
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002664 File Offset: 0x00000864
		public PkgGenProjectException(Exception innerException, string projectPath, string msg, params object[] args) : base(innerException, msg, args)
		{
			this.ProjectPath = projectPath;
			this.HasLineInfo = false;
			this.LineNumber = -1;
			this.LinePosition = -1;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x0000268C File Offset: 0x0000088C
		public PkgGenProjectException(string projectPath, string msg, params object[] args) : this(null, projectPath, msg, args)
		{
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002698 File Offset: 0x00000898
		public PkgGenProjectException(Exception innerException, string projectPath, int lineNumber, int linePosition, string msg, params object[] args) : base(innerException, msg, args)
		{
			this.ProjectPath = projectPath;
			this.HasLineInfo = true;
			this.LineNumber = lineNumber;
			this.LinePosition = linePosition;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000026C2 File Offset: 0x000008C2
		public PkgGenProjectException(string projectPath, int lineNumber, int linePosition, string msg, params object[] args) : this(null, projectPath, lineNumber, linePosition, msg, args)
		{
		}
	}
}
