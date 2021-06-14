using System;
using System.IO;
using SecureWim.Properties;

namespace SecureWim
{
	// Token: 0x02000008 RID: 8
	internal class ReplaceCommand : IToolCommand
	{
		// Token: 0x0600001D RID: 29 RVA: 0x00002AB0 File Offset: 0x00000CB0
		public ReplaceCommand(string[] args)
		{
			if (args.Length < 3)
			{
				throw new ArgParseException("Expected both a catalog and a WIM to be specified.", new UsagePrinter.UsageDelegate(ReplaceCommand.PrintUsage));
			}
			this.catalogFile = args[1];
			this.wimFile = args[2];
			try
			{
				if (!Helpers.HasSdiHeader(this.wimFile))
				{
					throw new ArgParseException(string.Format("The specified .secwim does not appear to be valid: {0}.", this.wimFile), new UsagePrinter.UsageDelegate(ReplaceCommand.PrintUsage));
				}
			}
			catch (IOException ex)
			{
				throw new ArgParseException(ex.Message, new UsagePrinter.UsageDelegate(ReplaceCommand.PrintUsage));
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002B4C File Offset: 0x00000D4C
		public static void PrintUsage()
		{
			Console.WriteLine(Resources.ReplaceUsageString);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002B58 File Offset: 0x00000D58
		public int Run()
		{
			byte[] array = File.ReadAllBytes(this.catalogFile);
			try
			{
				using (FileStream fileStream = File.Open(this.wimFile, FileMode.Open, FileAccess.ReadWrite))
				{
					Helpers.SeekStreamToCatalogStart(fileStream);
					FileStream fileStream2 = fileStream;
					fileStream2.SetLength(fileStream2.Position + (long)array.Length + 4L);
					fileStream.Write(array, 0, array.Length);
					Helpers.WriteUintToStream((uint)array.Length, fileStream);
				}
			}
			catch (IOException ex)
			{
				Console.WriteLine("\"replacecat\" command failed.  File IO operation encountered an error.");
				Console.WriteLine("Details: {0}", ex.Message);
				return -1;
			}
			return 0;
		}

		// Token: 0x0400000A RID: 10
		private string catalogFile;

		// Token: 0x0400000B RID: 11
		private string wimFile;
	}
}
