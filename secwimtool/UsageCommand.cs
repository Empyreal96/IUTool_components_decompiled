using System;

namespace SecureWim
{
	// Token: 0x0200000A RID: 10
	internal class UsageCommand : IToolCommand
	{
		// Token: 0x06000023 RID: 35 RVA: 0x00002DE4 File Offset: 0x00000FE4
		public UsageCommand(string[] commands)
		{
			this.supportedCommands = commands;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002DF3 File Offset: 0x00000FF3
		public int Run()
		{
			this.PrintUsage();
			return 0;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002DFC File Offset: 0x00000FFC
		public void PrintUsage()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine("secwimtool <command> <arguments>\n");
			Console.WriteLine("Commands:");
			foreach (string str in this.supportedCommands)
			{
				Console.WriteLine("-".PadLeft(4) + str);
			}
			Console.WriteLine();
			Console.WriteLine("run secwimtool <command> -? for per-command help.");
		}

		// Token: 0x0400000C RID: 12
		private string[] supportedCommands;
	}
}
