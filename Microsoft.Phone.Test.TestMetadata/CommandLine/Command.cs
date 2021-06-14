using System;
using System.IO;

namespace Microsoft.Phone.Test.TestMetadata.CommandLine
{
	// Token: 0x02000018 RID: 24
	public abstract class Command : IDisposable
	{
		// Token: 0x06000077 RID: 119 RVA: 0x00003A28 File Offset: 0x00001C28
		~Command()
		{
			this.Dispose(false);
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000078 RID: 120 RVA: 0x00003A5C File Offset: 0x00001C5C
		// (set) Token: 0x06000079 RID: 121 RVA: 0x00003A64 File Offset: 0x00001C64
		public int ReturnCode { get; set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00003A6D File Offset: 0x00001C6D
		// (set) Token: 0x0600007B RID: 123 RVA: 0x00003A75 File Offset: 0x00001C75
		public TextWriter Output { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00003A7E File Offset: 0x00001C7E
		// (set) Token: 0x0600007D RID: 125 RVA: 0x00003A86 File Offset: 0x00001C86
		public TextWriter Error { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600007E RID: 126 RVA: 0x00003A8F File Offset: 0x00001C8F
		// (set) Token: 0x0600007F RID: 127 RVA: 0x00003A97 File Offset: 0x00001C97
		public CommandSpecification Specification { get; set; }

		// Token: 0x06000080 RID: 128 RVA: 0x00003AA0 File Offset: 0x00001CA0
		public void Run()
		{
			bool flag = this.Output == null;
			if (flag)
			{
				throw new CommandException("No output writer has been given for the command.");
			}
			this.RunImplementation();
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00003ACE File Offset: 0x00001CCE
		public void Load(CommandOptionCollection options)
		{
			this.LoadImplementation(options);
		}

		// Token: 0x06000082 RID: 130
		protected abstract void RunImplementation();

		// Token: 0x06000083 RID: 131 RVA: 0x00003AD9 File Offset: 0x00001CD9
		protected virtual void LoadImplementation(CommandOptionCollection options)
		{
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00003AD9 File Offset: 0x00001CD9
		protected virtual void Dispose(bool suppressFinalize)
		{
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00003ADC File Offset: 0x00001CDC
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
