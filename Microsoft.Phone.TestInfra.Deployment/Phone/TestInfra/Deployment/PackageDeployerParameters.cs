using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000024 RID: 36
	public class PackageDeployerParameters
	{
		// Token: 0x0600018E RID: 398 RVA: 0x0000A04C File Offset: 0x0000824C
		public PackageDeployerParameters(string outputPath, string rootPath)
		{
			bool flag = string.IsNullOrWhiteSpace(outputPath);
			if (flag)
			{
				throw new ArgumentNullException("outputPath");
			}
			bool flag2 = string.IsNullOrWhiteSpace(rootPath);
			if (flag2)
			{
				throw new ArgumentNullException("rootPath");
			}
			this.RootPaths = rootPath;
			this.OutputPath = outputPath;
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x0600018F RID: 399 RVA: 0x0000A09D File Offset: 0x0000829D
		// (set) Token: 0x06000190 RID: 400 RVA: 0x0000A0A5 File Offset: 0x000082A5
		public string RootPaths { get; private set; }

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000191 RID: 401 RVA: 0x0000A0AE File Offset: 0x000082AE
		// (set) Token: 0x06000192 RID: 402 RVA: 0x0000A0B6 File Offset: 0x000082B6
		public string AlternateRoots { get; set; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000193 RID: 403 RVA: 0x0000A0BF File Offset: 0x000082BF
		// (set) Token: 0x06000194 RID: 404 RVA: 0x0000A0C7 File Offset: 0x000082C7
		public string Macros { get; set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000195 RID: 405 RVA: 0x0000A0D0 File Offset: 0x000082D0
		// (set) Token: 0x06000196 RID: 406 RVA: 0x0000A0D8 File Offset: 0x000082D8
		public string OutputPath { get; private set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000197 RID: 407 RVA: 0x0000A0E1 File Offset: 0x000082E1
		// (set) Token: 0x06000198 RID: 408 RVA: 0x0000A0E9 File Offset: 0x000082E9
		public string Packages { get; set; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000199 RID: 409 RVA: 0x0000A0F2 File Offset: 0x000082F2
		// (set) Token: 0x0600019A RID: 410 RVA: 0x0000A0FA File Offset: 0x000082FA
		public string PackageFile { get; set; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x0600019B RID: 411 RVA: 0x0000A103 File Offset: 0x00008303
		// (set) Token: 0x0600019C RID: 412 RVA: 0x0000A10B File Offset: 0x0000830B
		[DefaultValue(TraceLevel.Info)]
		public TraceLevel ConsoleTraceLevel { get; set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x0600019D RID: 413 RVA: 0x0000A114 File Offset: 0x00008314
		// (set) Token: 0x0600019E RID: 414 RVA: 0x0000A11C File Offset: 0x0000831C
		[DefaultValue(TraceLevel.Info)]
		public TraceLevel FileTraceLevel { get; set; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x0600019F RID: 415 RVA: 0x0000A125 File Offset: 0x00008325
		// (set) Token: 0x060001A0 RID: 416 RVA: 0x0000A12D File Offset: 0x0000832D
		public string CacheRoot { get; set; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060001A1 RID: 417 RVA: 0x0000A136 File Offset: 0x00008336
		// (set) Token: 0x060001A2 RID: 418 RVA: 0x0000A13E File Offset: 0x0000833E
		[DefaultValue(false)]
		public bool SourceRootIsVolatile { get; set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060001A3 RID: 419 RVA: 0x0000A147 File Offset: 0x00008347
		// (set) Token: 0x060001A4 RID: 420 RVA: 0x0000A14F File Offset: 0x0000834F
		[DefaultValue(true)]
		public bool Recurse { get; set; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060001A5 RID: 421 RVA: 0x0000A158 File Offset: 0x00008358
		// (set) Token: 0x060001A6 RID: 422 RVA: 0x0000A194 File Offset: 0x00008394
		public TimeSpan ExpiresIn
		{
			get
			{
				return (this.expiresIn <= TimeSpan.Zero) ? TimeSpan.FromHours((double)Settings.Default.DefaultFileExpiresInHours) : this.expiresIn;
			}
			set
			{
				this.expiresIn = value;
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060001A7 RID: 423 RVA: 0x0000A1A0 File Offset: 0x000083A0
		// (set) Token: 0x060001A8 RID: 424 RVA: 0x0000A1E0 File Offset: 0x000083E0
		public string LogFile
		{
			get
			{
				return string.IsNullOrWhiteSpace(this.logFile) ? Path.Combine(Settings.Default.DefaultLogDirectory, Settings.Default.DefaultLogName) : this.logFile;
			}
			set
			{
				this.logFile = value;
			}
		}

		// Token: 0x040000A2 RID: 162
		private TimeSpan expiresIn;

		// Token: 0x040000A3 RID: 163
		private string logFile;
	}
}
