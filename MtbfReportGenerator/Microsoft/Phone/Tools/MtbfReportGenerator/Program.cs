using System;
using System.IO;
using DH.Common;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x02000010 RID: 16
	public class Program
	{
		// Token: 0x06000064 RID: 100 RVA: 0x0000277C File Offset: 0x0000097C
		public static int Main(string[] args)
		{
			Program program = new Program();
			try
			{
				if (program.ProcessArguments(args))
				{
					program.GenerateReport();
					return 0;
				}
			}
			catch (Exception value)
			{
				Console.WriteLine("\nError generating the report.\n");
				Console.WriteLine(value);
			}
			return -1;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000027C8 File Offset: 0x000009C8
		private bool ProcessArguments(string[] args)
		{
			Argument[] array = new Argument[]
			{
				new Argument("Input", null, false, true, "Comma separated list of mtbf summary logs to be processed."),
				new Argument("Output", null, false, true, "Name of the output file."),
				new Argument("Type", null, false, true, "Type of the output report. Possible values are 'Xml', 'Html' and 'Excel'."),
				new Argument("Template", null, true, true, "Path to a custom XSLT/Excel template file to be used. If this argument is not specified, a default template will be used for HTML reports. In order to generate an Excel report, a template must be specified.")
			};
			CommandLineParser commandLineParser = new CommandLineParser(array);
			try
			{
				if (commandLineParser.Parse(args))
				{
					this.inputFileNames = commandLineParser.GetArgumentValueAsString(array[0].Name).Split(new char[]
					{
						','
					}, StringSplitOptions.RemoveEmptyEntries);
					if (this.inputFileNames.Length == 0)
					{
						throw new ArgumentException("No input file names were provided as argument.");
					}
					this.outputFileName = commandLineParser.GetArgumentValueAsString(array[1].Name);
					if (!Enum.TryParse<ReportType>(commandLineParser.GetArgumentValueAsString(array[2].Name), true, out this.reportType))
					{
						throw new ArgumentException("Invalid report type specified. Possible values are 'xml', 'html' and 'excel'.");
					}
					switch (this.reportType)
					{
					case ReportType.Xml:
						this.outputReport = new XmlReport();
						break;
					case ReportType.Html:
						this.outputReport = new HtmlReport();
						break;
					case ReportType.Excel:
						this.outputReport = new ExcelReport();
						break;
					}
					if (commandLineParser.IsArgumentProvided(array[3].Name))
					{
						this.templateFileName = commandLineParser.GetArgumentValueAsString(array[3].Name);
					}
					if (this.reportType == ReportType.Excel && string.IsNullOrEmpty(this.templateFileName))
					{
						throw new ArgumentException("Excel template not specified.");
					}
					return true;
				}
			}
			catch (ArgumentException ex)
			{
				Console.WriteLine("\nError Processing Arguments. " + ex.Message);
			}
			Console.WriteLine("\n\nThis tool processes one or more Mtbf summary log text files and generates \nan XML summary report. The generated report contains the the results from \nall given log files where each log file is assigned a device number (starting with 1).\n\n");
			Console.WriteLine("Arguments usage: \n");
			commandLineParser.ShowHelp();
			Console.WriteLine();
			Console.WriteLine("NOTE: In order to generate Excel reports, Microsoft Office Excel must be installed.");
			return false;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000029A4 File Offset: 0x00000BA4
		private void GenerateReport()
		{
			MtbfReport mtbfReport = new MtbfReport();
			Console.WriteLine("Generating Mtbf report from {0} summary log file(s)", this.inputFileNames.Length);
			int num = 1;
			foreach (string text in this.inputFileNames)
			{
				Console.WriteLine("Processing summary log file: {0}", text);
				SummaryLog summaryLog = new SummaryLog(num, text);
				mtbfReport.AddDeviceReport(summaryLog.DeviceReport);
				num++;
			}
			string fullPath = Path.GetFullPath(this.outputFileName);
			if (File.Exists(fullPath))
			{
				File.Delete(fullPath);
			}
			this.outputReport.Generate(mtbfReport, fullPath, this.templateFileName);
			Console.WriteLine("Generated the report: {0}", this.outputFileName);
		}

		// Token: 0x04000027 RID: 39
		private string[] inputFileNames;

		// Token: 0x04000028 RID: 40
		private string outputFileName;

		// Token: 0x04000029 RID: 41
		private ReportType reportType;

		// Token: 0x0400002A RID: 42
		private string templateFileName;

		// Token: 0x0400002B RID: 43
		private IReport outputReport;
	}
}
