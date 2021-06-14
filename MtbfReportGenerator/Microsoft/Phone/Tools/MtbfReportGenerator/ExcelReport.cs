using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Office.Interop.Excel;
using Microsoft.Phone.Tools.MtbfReportGenerator.Properties;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x02000011 RID: 17
	public class ExcelReport : IReport
	{
		// Token: 0x06000068 RID: 104 RVA: 0x00002A5C File Offset: 0x00000C5C
		public void Generate(MtbfReport mtbfReport, string outputFile, string templateFile)
		{
			if (mtbfReport.DeviceReports.Count > Settings.Default.ExcelMaxDevices)
			{
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Number of device logs ({0}) exceeds the template capacity ({1}).", new object[]
				{
					mtbfReport.DeviceReports.Count,
					Settings.Default.ExcelMaxDevices
				}));
			}
			try
			{
				this.GenerateExcelReport(mtbfReport, outputFile, Path.GetFullPath(templateFile));
			}
			catch (COMException ex)
			{
				if (ex.ErrorCode == -2147221164)
				{
					Console.WriteLine("Error generating Excel report. Make sure that Microsoft Office Excel is installed.");
				}
				if (ex.ErrorCode == -2147352565)
				{
					Console.WriteLine("Error generating Excel report. Make sure that the template has two worksheets named '{0}' and '{1}'", Settings.Default.ExcelTimeSheetName, Settings.Default.ExcelCompletionSheetName);
				}
				throw ex;
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00002B24 File Offset: 0x00000D24
		private void GenerateExcelReport(MtbfReport mtbfReport, string outputFile, string templatePath)
		{
			Application application = null;
			Workbook workbook = null;
			Worksheet worksheet = null;
			Worksheet worksheet2 = null;
			try
			{
				application = (Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("00024500-0000-0000-C000-000000000046")));
				application.Visible = false;
				workbook = application.Workbooks.Open(templatePath, 0, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
				if (ExcelReport.<>o__5.<>p__0 == null)
				{
					ExcelReport.<>o__5.<>p__0 = CallSite<Func<CallSite, object, Worksheet>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(Worksheet), typeof(ExcelReport)));
				}
				worksheet = ExcelReport.<>o__5.<>p__0.Target(ExcelReport.<>o__5.<>p__0, workbook.Worksheets.get_Item(Settings.Default.ExcelTimeSheetName));
				if (ExcelReport.<>o__5.<>p__1 == null)
				{
					ExcelReport.<>o__5.<>p__1 = CallSite<Func<CallSite, object, Worksheet>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(Worksheet), typeof(ExcelReport)));
				}
				worksheet2 = ExcelReport.<>o__5.<>p__1.Target(ExcelReport.<>o__5.<>p__1, workbook.Worksheets.get_Item(Settings.Default.ExcelCompletionSheetName));
				this.FillExcelWorkSheets(mtbfReport, worksheet, worksheet2);
				workbook.SaveAs(outputFile, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
			}
			finally
			{
				if (worksheet2 != null)
				{
					Marshal.ReleaseComObject(worksheet2);
				}
				if (worksheet != null)
				{
					Marshal.ReleaseComObject(worksheet);
				}
				if (workbook != null)
				{
					Marshal.ReleaseComObject(workbook);
				}
				if (application != null)
				{
					if (application.ActiveWindow != null)
					{
						application.ActiveWindow.Close(false, Type.Missing, Type.Missing);
					}
					Marshal.ReleaseComObject(application);
				}
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00002D08 File Offset: 0x00000F08
		private void FillExcelWorkSheets(MtbfReport mtbfReport, Worksheet excelTimeSheet, Worksheet excelCompletionSheet)
		{
			int num = 0;
			foreach (MtbfDeviceReport mtbfDeviceReport in mtbfReport.DeviceReports)
			{
				MtbfGroupReport groupReport = mtbfDeviceReport.GetGroupReport("Main");
				MtbfMixGroup group = mtbfDeviceReport.Mix.GetGroup("Main");
				if (groupReport != null && group != null)
				{
					if (groupReport.LoopReports.Count > Settings.Default.ExcelMaxLoops)
					{
						throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Number of loops ({0}) exceeds the template capacity ({1}).", new object[]
						{
							groupReport.LoopReports.Count,
							Settings.Default.ExcelMaxLoops
						}));
					}
					for (int i = 0; i < Settings.Default.ExcelMaxSections; i++)
					{
						int num2 = ExcelReport.TimeSheetCoordinates[num, 0];
						int num3 = ExcelReport.TimeSheetCoordinates[num, 1];
						int num4 = ExcelReport.CompletionSheetCoordinates[num, 0];
						int num5 = ExcelReport.CompletionSheetCoordinates[num, 1];
						string cellText = this.GetCellText(excelTimeSheet, num2 + i, num3);
						string cellText2 = this.GetCellText(excelCompletionSheet, num4 + i, num5);
						if (!cellText.Trim().Equals(cellText2.Trim()))
						{
							throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Invalid template. Section numbers in time sheet ({0}) and completion sheet ({1}) does not match for device {2}", new object[]
							{
								cellText,
								cellText2,
								num + 1
							}));
						}
						string sectionNumberFromTemplateEntry = this.GetSectionNumberFromTemplateEntry(group, cellText);
						if (!string.IsNullOrEmpty(sectionNumberFromTemplateEntry))
						{
							int sectionCommandCount;
							int sectionExpectedCount = this.GetSectionExpectedCount(group, sectionNumberFromTemplateEntry, out sectionCommandCount);
							excelCompletionSheet.Cells[num4 + i, num5 + 1] = sectionExpectedCount;
							int num6 = 1;
							foreach (MtbfLoopReport loopReport in groupReport.LoopReports)
							{
								int num7;
								string value;
								if (this.ComputeSectionSummary(loopReport, sectionNumberFromTemplateEntry, sectionCommandCount, out num7, out value))
								{
									excelTimeSheet.Cells[num2 + i, num3 + num6] = value;
									excelCompletionSheet.Cells[num4 + i, num5 + num6 + 1] = num7;
								}
								num6++;
							}
						}
					}
					num++;
				}
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00002F9C File Offset: 0x0000119C
		private string GetCellText(Worksheet workSheet, int row, int column)
		{
			string result = string.Empty;
			Range range = null;
			try
			{
				if (ExcelReport.<>o__7.<>p__0 == null)
				{
					ExcelReport.<>o__7.<>p__0 = CallSite<Func<CallSite, object, Range>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(Range), typeof(ExcelReport)));
				}
				range = ExcelReport.<>o__7.<>p__0.Target(ExcelReport.<>o__7.<>p__0, workSheet.Cells[row, column]);
				if (ExcelReport.<>o__7.<>p__2 == null)
				{
					ExcelReport.<>o__7.<>p__2 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(string), typeof(ExcelReport)));
				}
				Func<CallSite, object, string> target = ExcelReport.<>o__7.<>p__2.Target;
				CallSite <>p__ = ExcelReport.<>o__7.<>p__2;
				if (ExcelReport.<>o__7.<>p__1 == null)
				{
					ExcelReport.<>o__7.<>p__1 = CallSite<Func<CallSite, Type, object, CultureInfo, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, typeof(ExcelReport), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
					}));
				}
				result = target(<>p__, ExcelReport.<>o__7.<>p__1.Target(ExcelReport.<>o__7.<>p__1, typeof(Convert), range.get_Value(Type.Missing), CultureInfo.InvariantCulture));
			}
			finally
			{
				if (range != null)
				{
					Marshal.ReleaseComObject(range);
				}
			}
			return result;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x000030F0 File Offset: 0x000012F0
		private string GetSectionNumberFromTemplateEntry(MtbfMixGroup mixGroup, string templateSectionNumber)
		{
			foreach (MtbfMixCommand mtbfMixCommand in mixGroup.Commands)
			{
				if (new Regex(string.Format(CultureInfo.InvariantCulture, "\\b{0}\\b", new object[]
				{
					mtbfMixCommand.SectionNumber
				})).IsMatch(templateSectionNumber))
				{
					return mtbfMixCommand.SectionNumber;
				}
			}
			return null;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00003174 File Offset: 0x00001374
		private int GetSectionExpectedCount(MtbfMixGroup mixGroup, string sectionNumber, out int totalCommands)
		{
			totalCommands = 0;
			int num = 0;
			foreach (MtbfMixCommand mtbfMixCommand in mixGroup.Commands)
			{
				if (sectionNumber.Equals(mtbfMixCommand.SectionNumber))
				{
					num += mtbfMixCommand.ExpectedCount;
					totalCommands++;
				}
			}
			return num;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x000031E4 File Offset: 0x000013E4
		private bool ComputeSectionSummary(MtbfLoopReport loopReport, string sectionNumber, int sectionCommandCount, out int passedCount, out string duration)
		{
			MtbfSectionReport mtbfSectionReport = loopReport.SectionReports.Find((MtbfSectionReport item) => item.Number.Equals(sectionNumber));
			passedCount = 0;
			duration = string.Empty;
			if (mtbfSectionReport == null)
			{
				return false;
			}
			if (mtbfSectionReport.TestReports.Count < sectionCommandCount)
			{
				return false;
			}
			duration = TimeSpan.FromMilliseconds((double)mtbfSectionReport.DurationMilliSeconds).ToString(Settings.Default.ExcelTimeFormat);
			foreach (MtbfTestReport mtbfTestReport in mtbfSectionReport.TestReports)
			{
				passedCount += mtbfTestReport.Result.Passed;
			}
			return true;
		}

		// Token: 0x0400002C RID: 44
		private const uint HResultClassNotRegistered = 2147746132U;

		// Token: 0x0400002D RID: 45
		private const uint HResultBadIndex = 2147614731U;

		// Token: 0x0400002E RID: 46
		private static readonly int[,] TimeSheetCoordinates = new int[,]
		{
			{
				26,
				1
			},
			{
				44,
				1
			},
			{
				62,
				1
			},
			{
				80,
				1
			},
			{
				98,
				1
			},
			{
				116,
				1
			},
			{
				134,
				1
			}
		};

		// Token: 0x0400002F RID: 47
		private static readonly int[,] CompletionSheetCoordinates = new int[,]
		{
			{
				32,
				2
			},
			{
				50,
				2
			},
			{
				68,
				2
			},
			{
				86,
				2
			},
			{
				104,
				2
			},
			{
				122,
				2
			},
			{
				140,
				2
			}
		};
	}
}
