using System;
using BuildFilterExpressionEvaluator;
using Microsoft.CompPlat.PkgBldr.Base.Security;
using Microsoft.CompPlat.PkgBldr.Base.Tools;
using Microsoft.CompPlat.PkgBldr.Tools;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x02000039 RID: 57
	public class Config
	{
		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x000081D4 File Offset: 0x000063D4
		// (set) Token: 0x060000F6 RID: 246 RVA: 0x000081DC File Offset: 0x000063DC
		public bool ProcessInf
		{
			get
			{
				return this._proccessInf;
			}
			set
			{
				this._proccessInf = value;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x000081E5 File Offset: 0x000063E5
		// (set) Token: 0x060000F8 RID: 248 RVA: 0x000081ED File Offset: 0x000063ED
		public bool GenerateCab
		{
			get
			{
				return this._generateCab;
			}
			set
			{
				this._generateCab = value;
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x000081F6 File Offset: 0x000063F6
		// (set) Token: 0x060000FA RID: 250 RVA: 0x000081FE File Offset: 0x000063FE
		public ConversionType Convert { get; set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000FB RID: 251 RVA: 0x00008207 File Offset: 0x00006407
		// (set) Token: 0x060000FC RID: 252 RVA: 0x0000820F File Offset: 0x0000640F
		public string Input
		{
			get
			{
				return this._input;
			}
			set
			{
				if (value != null)
				{
					this._input = LongPath.GetFullPath(value.TrimEnd(new char[]
					{
						'\\'
					}));
					if (!LongPathFile.Exists(this._input))
					{
						throw new PkgGenException("Input file does not exist");
					}
				}
				else
				{
					this._input = null;
				}
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000FD RID: 253 RVA: 0x0000824F File Offset: 0x0000644F
		// (set) Token: 0x060000FE RID: 254 RVA: 0x00008258 File Offset: 0x00006458
		public string Output
		{
			get
			{
				return this._output;
			}
			set
			{
				if (value == null)
				{
					this._output = null;
					this._autoGenerateOutput = false;
					return;
				}
				string text = LongPath.GetFullPath(value.TrimEnd(new char[]
				{
					'\\'
				}));
				if (text.ToLowerInvariant().EndsWith(".man", StringComparison.OrdinalIgnoreCase) || text.ToLowerInvariant().EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
				{
					this._autoGenerateOutput = false;
					this._output = text;
					text = LongPath.GetDirectoryName(text);
					return;
				}
				this._autoGenerateOutput = true;
				this._output = text;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000FF RID: 255 RVA: 0x000082D8 File Offset: 0x000064D8
		public bool AutoGenerateOutput
		{
			get
			{
				return this._autoGenerateOutput;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000100 RID: 256 RVA: 0x000082E0 File Offset: 0x000064E0
		// (set) Token: 0x06000101 RID: 257 RVA: 0x000082E8 File Offset: 0x000064E8
		public IDeploymentLogger Logger { get; set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000102 RID: 258 RVA: 0x000082F1 File Offset: 0x000064F1
		// (set) Token: 0x06000103 RID: 259 RVA: 0x000082F9 File Offset: 0x000064F9
		public bool Diagnostic { get; set; }

		// Token: 0x04000050 RID: 80
		public MacroResolver Macros;

		// Token: 0x04000051 RID: 81
		public MacroResolver PolicyMacros;

		// Token: 0x04000052 RID: 82
		public GlobalSecurity GlobalSecurity;

		// Token: 0x04000053 RID: 83
		public BuildPass Pass;

		// Token: 0x04000054 RID: 84
		public object arg;

		// Token: 0x04000055 RID: 85
		public ExitStatus ExitStatus;

		// Token: 0x04000056 RID: 86
		public Build build;

		// Token: 0x04000057 RID: 87
		public Bld Bld;

		// Token: 0x04000058 RID: 88
		public PkgBldrCmd pkgBldrArgs;

		// Token: 0x04000059 RID: 89
		public BuildFilterExpressionEvaluator ExpressionEvaluator;

		// Token: 0x0400005A RID: 90
		private string _input;

		// Token: 0x0400005B RID: 91
		private string _output;

		// Token: 0x0400005C RID: 92
		private bool _autoGenerateOutput;

		// Token: 0x0400005D RID: 93
		private bool _generateCab;

		// Token: 0x0400005E RID: 94
		private bool _proccessInf;
	}
}
