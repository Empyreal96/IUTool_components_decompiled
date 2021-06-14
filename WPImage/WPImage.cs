using System;
using System.Globalization;
using System.Linq;

namespace Microsoft.WindowsPhone.WPImage
{
	// Token: 0x02000007 RID: 7
	internal class WPImage
	{
		// Token: 0x0600001C RID: 28 RVA: 0x00002FB8 File Offset: 0x000011B8
		private static void Main(string[] args)
		{
			new WPImage().Run(args);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002FC5 File Offset: 0x000011C5
		private void Run(string[] args)
		{
			this.ParseArgs(args);
			if (this._command != null)
			{
				this._command.Run();
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002FE4 File Offset: 0x000011E4
		private void ParseArgs(string[] args)
		{
			if (args.Length < 1)
			{
				this.PrintUsage();
				return;
			}
			foreach (IWPImageCommand iwpimageCommand in this._commands)
			{
				if (string.Compare(args[0], iwpimageCommand.Name, true, CultureInfo.InvariantCulture) == 0)
				{
					this._command = iwpimageCommand;
					break;
				}
			}
			if (this._command == null)
			{
				this.PrintUsage();
				return;
			}
			if (!this._command.ParseArgs(args.Skip(1).ToArray<string>()))
			{
				this._command.PrintUsage();
				this._command = null;
				return;
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00003074 File Offset: 0x00001274
		private void PrintUsage()
		{
			Console.WriteLine("WPImage.exe command [command_parameters]");
			Console.WriteLine("Commands:");
			foreach (IWPImageCommand iwpimageCommand in this._commands)
			{
				Console.WriteLine("  {0}", iwpimageCommand.Name);
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000030BE File Offset: 0x000012BE
		public static void NullLog(string unused, object[] notused)
		{
		}

		// Token: 0x0400000E RID: 14
		internal const string ModuleName = "WPImage.exe";

		// Token: 0x0400000F RID: 15
		private IWPImageCommand[] _commands = new IWPImageCommand[]
		{
			new MountCommand(),
			new DismountCommand(),
			new RemoveIdCommand(),
			new DisplayIdCommand()
		};

		// Token: 0x04000010 RID: 16
		private IWPImageCommand _command;
	}
}
