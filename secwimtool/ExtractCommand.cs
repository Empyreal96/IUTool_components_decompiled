using System;
using System.IO;
using SecureWim.Properties;

namespace SecureWim
{
	// Token: 0x02000005 RID: 5
	internal class ExtractCommand : IToolCommand
	{
		// Token: 0x0600000F RID: 15 RVA: 0x00002730 File Offset: 0x00000930
		public ExtractCommand(string[] args)
		{
			if (args.Length < 2)
			{
				throw new ArgParseException("Please specify a .secwim to extract a catalog from.", new UsagePrinter.UsageDelegate(ExtractCommand.PrintUsage));
			}
			this.file = args[1];
			try
			{
				if (!Helpers.HasSdiHeader(this.file))
				{
					throw new ArgParseException(string.Format("The specified file does not appear to be a valid .secwim: {0}", this.file), new UsagePrinter.UsageDelegate(ExtractCommand.PrintUsage));
				}
			}
			catch (IOException ex)
			{
				throw new ArgParseException(ex.Message, new UsagePrinter.UsageDelegate(ExtractCommand.PrintUsage));
			}
			if (args.Length > 2)
			{
				this.outputfile = args[2];
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000027D0 File Offset: 0x000009D0
		public static void PrintUsage()
		{
			Console.WriteLine(Resources.ExtractUsageString);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000027DC File Offset: 0x000009DC
		public int Run()
		{
			try
			{
				byte[] array;
				using (FileStream fileStream = File.OpenRead(this.file))
				{
					int catalogSize = (int)Helpers.GetCatalogSize(fileStream);
					Helpers.SeekStreamToCatalogStart(fileStream);
					array = new byte[catalogSize];
					fileStream.Read(array, 0, array.Length);
				}
				Stream stream;
				if (this.outputfile != null)
				{
					stream = File.OpenWrite(this.outputfile);
				}
				else
				{
					stream = Console.OpenStandardOutput();
				}
				stream.Write(array, 0, array.Length);
			}
			catch (IOException ex)
			{
				Console.WriteLine("\"extractcat\" command failed.  File IO operation encountered an error.");
				Console.WriteLine("Details: {0}", ex.Message);
				return -1;
			}
			return 0;
		}

		// Token: 0x04000008 RID: 8
		private string file;

		// Token: 0x04000009 RID: 9
		private string outputfile;
	}
}
