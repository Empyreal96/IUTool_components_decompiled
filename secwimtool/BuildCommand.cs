using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using SecureWim.Properties;

namespace SecureWim
{
	// Token: 0x02000003 RID: 3
	internal class BuildCommand : IToolCommand
	{
		// Token: 0x06000004 RID: 4 RVA: 0x00002074 File Offset: 0x00000274
		public BuildCommand(string[] args)
		{
			if (args.Length < 2)
			{
				throw new ArgParseException("Expected at least two arguments.", new UsagePrinter.UsageDelegate(BuildCommand.PrintUsage));
			}
			this.wimPath = args[1];
			try
			{
				if (!Helpers.HasWimHeader(this.wimPath))
				{
					throw new ArgParseException(string.Format("Specified file does not appear to be a valid WIM: {0}", this.wimPath), new UsagePrinter.UsageDelegate(BuildCommand.PrintUsage));
				}
			}
			catch (IOException ex)
			{
				throw new ArgParseException(ex.Message, new UsagePrinter.UsageDelegate(BuildCommand.PrintUsage));
			}
			if (args.Length < 3)
			{
				throw new ArgParseException("Expected an output file argument.", new UsagePrinter.UsageDelegate(BuildCommand.PrintUsage));
			}
			this.outputPath = null;
			Regex regex = new Regex("^[/-]platform$", RegexOptions.Compiled);
			Regex regex2 = new Regex("^[/-]serial$", RegexOptions.Compiled);
			for (int i = 2; i < args.Length; i++)
			{
				if (regex.IsMatch(args[i]))
				{
					i++;
					if (i >= args.Length)
					{
						throw new ArgParseException("No platform IDs specified.", new UsagePrinter.UsageDelegate(BuildCommand.PrintUsage));
					}
					this.CheckEmpty(this.platformIds);
					this.platformIds = args[i].Split(new char[]
					{
						';'
					});
				}
				else
				{
					if (regex2.IsMatch(args[i]))
					{
						i++;
						if (i < args.Length)
						{
							try
							{
								this.CheckEmpty(this.serialNumbers);
								this.serialNumbers = Helpers.GetGuids(args[i].Split(new char[]
								{
									';'
								}));
								goto IL_191;
							}
							catch (FormatException ex2)
							{
								throw new ArgParseException(ex2.Message, new UsagePrinter.UsageDelegate(BuildCommand.PrintUsage));
							}
						}
						throw new ArgParseException("No device serial number specified.", new UsagePrinter.UsageDelegate(BuildCommand.PrintUsage));
					}
					this.CheckEmpty(this.outputPath);
					this.outputPath = args[i];
				}
				IL_191:;
			}
			if (this.outputPath == null)
			{
				throw new ArgParseException("Expected an output file argument but none was specified.", new UsagePrinter.UsageDelegate(BuildCommand.PrintUsage));
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000225C File Offset: 0x0000045C
		public static void PrintUsage()
		{
			Console.WriteLine(Resources.BuildUsageString);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002268 File Offset: 0x00000468
		public int Run()
		{
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					this.WriteSdiAndWim(memoryStream);
					this.GenerateTargetingData(memoryStream);
					this.GenerateCatalog(memoryStream);
					this.WriteToOutputFile(memoryStream);
				}
			}
			catch (BuildCommandException ex)
			{
				Console.WriteLine("\"build\" command failed.");
				Console.WriteLine("Details: {0}", ex.Message);
				Console.WriteLine("Exit code: 0x{0:x}", ex.ErrorCode);
				return -1;
			}
			catch (IOException ex2)
			{
				Console.WriteLine("\"build\" command failed.  File IO operation encountered an error.");
				Console.WriteLine("Details: {0}", ex2.Message);
				return -1;
			}
			catch (UnauthorizedAccessException ex3)
			{
				Console.WriteLine("\"build\" command failed. Problem accessing file.");
				Console.WriteLine("Details: {0}", ex3.Message);
				return -1;
			}
			return 0;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002350 File Offset: 0x00000550
		private void CheckEmpty(object p)
		{
			if (p != null)
			{
				throw new ArgParseException("Invalid command line specified: extra arguments present.", new UsagePrinter.UsageDelegate(BuildCommand.PrintUsage));
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000236C File Offset: 0x0000056C
		private void WriteSdiAndWim(MemoryStream memStm)
		{
			byte[] sdiData = Resources.sdiData;
			byte[] array = File.ReadAllBytes(this.wimPath);
			foreach (byte[] array3 in new byte[][]
			{
				sdiData,
				array
			})
			{
				memStm.Write(array3, 0, array3.Length);
			}
			Helpers.AddPadding(memStm, 4U);
			this.wimSize = (uint)(memStm.Length - (long)sdiData.Length);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000023D4 File Offset: 0x000005D4
		private void GenerateTargetingData(MemoryStream memStm)
		{
			long length = memStm.Length;
			if (this.platformIds != null)
			{
				Helpers.WriteUintToStream(1952541808U, memStm);
				Helpers.WriteUintToStream(0U, memStm);
				foreach (string s in this.platformIds)
				{
					byte[] bytes = Encoding.ASCII.GetBytes(s);
					memStm.Write(bytes, 0, bytes.Length);
					memStm.WriteByte(0);
				}
				memStm.WriteByte(0);
				Helpers.AddPadding(memStm, 4U);
			}
			long num = memStm.Length - length;
			length = memStm.Length;
			if (this.serialNumbers != null)
			{
				Helpers.WriteUintToStream(1769366884U, memStm);
				Helpers.WriteUintToStream((uint)num, memStm);
				Helpers.WriteUintToStream((uint)this.serialNumbers.Length, memStm);
				foreach (Guid guid in this.serialNumbers)
				{
					byte[] array3 = guid.ToByteArray();
					memStm.Write(array3, 0, array3.Length);
				}
				num = memStm.Length - length;
				length = memStm.Length;
				if (memStm.Length % 4L != 0L)
				{
					throw new BuildCommandException("Device IDs not DWORD aligned", -1);
				}
			}
			uint num2 = 1702521203U;
			uint num3 = (uint)num;
			uint num4 = (uint)Resources.sdiData.Length;
			uint num5 = this.wimSize;
			uint[] array4 = new uint[]
			{
				num2,
				num3,
				num4,
				num5
			};
			for (int i = 0; i < array4.Length; i++)
			{
				Helpers.WriteUintToStream(array4[i], memStm);
			}
			if (memStm.Length % 4L != 0L)
			{
				throw new BuildCommandException("Size structure not DWORD aligned", -1);
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000255C File Offset: 0x0000075C
		private void GenerateCatalog(MemoryStream memStm)
		{
			string tempFileName = Path.GetTempFileName();
			string tempFileName2 = Path.GetTempFileName();
			string tempFileName3 = Path.GetTempFileName();
			string arg = string.Format("<hash>{0}", tempFileName3);
			File.WriteAllBytes(tempFileName3, memStm.ToArray());
			using (StreamWriter streamWriter = new StreamWriter(tempFileName2))
			{
				streamWriter.WriteLine("[CatalogHeader]");
				streamWriter.WriteLine("CatalogVersion=2");
				streamWriter.WriteLine("HashAlgorithms=SHA256");
				streamWriter.WriteLine("Name={0}", tempFileName);
				streamWriter.WriteLine("[CatalogFiles]");
				streamWriter.WriteLine("{0}={1}", arg, tempFileName3);
			}
			using (Process process = new Process())
			{
				process.StartInfo.FileName = "makecat.exe";
				process.StartInfo.Arguments = string.Format("\"{0}\"", tempFileName2);
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				process.WaitForExit();
				if (process.ExitCode != 0)
				{
					throw new BuildCommandException("Failed to run makecat.exe.", process.ExitCode);
				}
			}
			byte[] array = File.ReadAllBytes(tempFileName);
			memStm.Write(array, 0, array.Length);
			Helpers.WriteUintToStream((uint)array.Length, memStm);
			File.Delete(tempFileName);
			File.Delete(tempFileName3);
			File.Delete(tempFileName2);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000026C4 File Offset: 0x000008C4
		private void WriteToOutputFile(MemoryStream memStm)
		{
			using (FileStream fileStream = File.OpenWrite(this.outputPath))
			{
				fileStream.SetLength(memStm.Length);
				memStm.WriteTo(fileStream);
			}
		}

		// Token: 0x04000002 RID: 2
		private string wimPath;

		// Token: 0x04000003 RID: 3
		private string outputPath;

		// Token: 0x04000004 RID: 4
		private uint wimSize;

		// Token: 0x04000005 RID: 5
		private string[] platformIds;

		// Token: 0x04000006 RID: 6
		private Guid[] serialNumbers;
	}
}
