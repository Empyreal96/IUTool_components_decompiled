using System;
using System.Globalization;
using Microsoft.CompPlat.PkgBldr.Tools;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x02000034 RID: 52
	public class PkgGenProjectException : PkgGenException
	{
		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x00007B06 File Offset: 0x00005D06
		// (set) Token: 0x060000DA RID: 218 RVA: 0x00007B0E File Offset: 0x00005D0E
		public string ProjectPath { get; private set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000DB RID: 219 RVA: 0x00007B17 File Offset: 0x00005D17
		// (set) Token: 0x060000DC RID: 220 RVA: 0x00007B1F File Offset: 0x00005D1F
		public int LineNumber { get; private set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000DD RID: 221 RVA: 0x00007B28 File Offset: 0x00005D28
		// (set) Token: 0x060000DE RID: 222 RVA: 0x00007B30 File Offset: 0x00005D30
		public int LinePosition { get; private set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000DF RID: 223 RVA: 0x00007B39 File Offset: 0x00005D39
		// (set) Token: 0x060000E0 RID: 224 RVA: 0x00007B41 File Offset: 0x00005D41
		public bool HasLineInfo { get; private set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000E1 RID: 225 RVA: 0x00007B4C File Offset: 0x00005D4C
		public override string Message
		{
			get
			{
				if (this.HasLineInfo)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0}({1},{2}): {3}", new object[]
					{
						LongPath.GetFileName(this.ProjectPath),
						this.LineNumber,
						this.LinePosition,
						base.Message
					});
				}
				return string.Format(CultureInfo.InvariantCulture, "{0}: {1}", new object[]
				{
					LongPath.GetFileName(this.ProjectPath),
					base.Message
				});
			}
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00007BD6 File Offset: 0x00005DD6
		public PkgGenProjectException(Exception innerException, string projectPath, string msg, params object[] args) : base(innerException, msg, args)
		{
			this.ProjectPath = projectPath;
			this.HasLineInfo = false;
			this.LineNumber = -1;
			this.LinePosition = -1;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00007BFE File Offset: 0x00005DFE
		public PkgGenProjectException(string projectPath, string msg, params object[] args) : this(null, projectPath, msg, args)
		{
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00007C0A File Offset: 0x00005E0A
		public PkgGenProjectException(Exception innerException, string projectPath, int lineNumber, int linePosition, string msg, params object[] args) : base(innerException, msg, args)
		{
			this.ProjectPath = projectPath;
			this.HasLineInfo = true;
			this.LineNumber = lineNumber;
			this.LinePosition = linePosition;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00007C34 File Offset: 0x00005E34
		public PkgGenProjectException(string projectPath, int lineNumber, int linePosition, string msg, params object[] args) : this(null, projectPath, lineNumber, linePosition, msg, args)
		{
		}
	}
}
