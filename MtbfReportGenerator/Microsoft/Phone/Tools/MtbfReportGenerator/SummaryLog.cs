using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Phone.Tools.MtbfReportGenerator.Properties;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x02000016 RID: 22
	public class SummaryLog
	{
		// Token: 0x06000077 RID: 119 RVA: 0x00003418 File Offset: 0x00001618
		public SummaryLog(int deviceNumber, string logFileName)
		{
			this.mtbfLogFilesFolderPath = Path.Combine(Path.GetDirectoryName(logFileName), Settings.Default.MtbfLogsFolderName);
			this.logEntryActions = new Dictionary<string, Action<SummaryLogEntry>>
			{
				{
					"MixInfoCommandGroup",
					null
				},
				{
					"MixInfoCommand",
					new Action<SummaryLogEntry>(this.OnMixInfoCommand)
				},
				{
					"StartCommandGroup",
					new Action<SummaryLogEntry>(this.OnStartCommandGroup)
				},
				{
					"EndCommandGroup",
					new Action<SummaryLogEntry>(this.OnEndCommandGroup)
				},
				{
					"StartLoop",
					new Action<SummaryLogEntry>(this.OnStartLoop)
				},
				{
					"EndLoop",
					new Action<SummaryLogEntry>(this.OnEndLoop)
				},
				{
					"StartSection",
					new Action<SummaryLogEntry>(this.OnStartSection)
				},
				{
					"EndSection",
					new Action<SummaryLogEntry>(this.OnEndSection)
				},
				{
					"StartCommand",
					new Action<SummaryLogEntry>(this.OnStartCommand)
				},
				{
					"EndCommand",
					new Action<SummaryLogEntry>(this.OnEndCommand)
				},
				{
					"LogFile",
					new Action<SummaryLogEntry>(this.OnLogFile)
				}
			};
			this.DeviceReport = new MtbfDeviceReport(deviceNumber);
			this.BuildDeviceReport(logFileName);
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000078 RID: 120 RVA: 0x00003556 File Offset: 0x00001756
		// (set) Token: 0x06000079 RID: 121 RVA: 0x0000355E File Offset: 0x0000175E
		public MtbfDeviceReport DeviceReport { get; private set; }

		// Token: 0x0600007A RID: 122 RVA: 0x00003568 File Offset: 0x00001768
		private void BuildDeviceReport(string logFileName)
		{
			using (StreamReader streamReader = new StreamReader(logFileName))
			{
				string text = null;
				int num = 0;
				SummaryLogEntry summaryLogEntry = null;
				while ((text = streamReader.ReadLine()) != null)
				{
					num++;
					if (!string.IsNullOrWhiteSpace(text))
					{
						try
						{
							summaryLogEntry = new SummaryLogEntry(text);
							if (this.logEntryActions.ContainsKey(summaryLogEntry.Type))
							{
								if (this.logEntryActions[summaryLogEntry.Type] != null)
								{
									this.logEntryActions[summaryLogEntry.Type](summaryLogEntry);
								}
							}
							else
							{
								Console.WriteLine("WARNING: Log entry line '{0}' at position {1} cannot be understood. Ignoring.", text, num);
							}
						}
						catch (MtbfLogParserException)
						{
							if (streamReader.ReadLine() != null)
							{
								Console.WriteLine("\nError processing summary log line: {0}, line number = {1}.", text, num);
								throw;
							}
							Console.WriteLine("WARNING: Incomplete summary log (invalid last line '{0}') - Ignoring.", text);
						}
					}
				}
				if (summaryLogEntry != null)
				{
					this.OnProcessingCompleted(summaryLogEntry);
				}
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x0000365C File Offset: 0x0000185C
		private void UpdateDuration(MtbfReportDuration duration, SummaryLogEntry entry)
		{
			duration.EndTime = entry.TimeStamp;
			duration.EndTickCount = entry.TickCount;
			duration.DurationMilliSeconds = duration.EndTickCount - duration.StartTickCount;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x0000368C File Offset: 0x0000188C
		private void OnMixInfoCommand(SummaryLogEntry entry)
		{
			this.DeviceReport.Mix.AddGroup(entry.GetAttribute("CommandGroupName")).AddCommand(entry.GetAttribute("SectionNumber"), int.Parse(entry.GetAttribute("ExpectedTestCaseCount"), CultureInfo.InvariantCulture));
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000036DC File Offset: 0x000018DC
		private void OnStartCommandGroup(SummaryLogEntry entry)
		{
			if (this.currGroup != null)
			{
				throw new MtbfLogParserException("StartCommandGroup event was found before the previous group's EndCommandGroup");
			}
			this.currGroup = this.DeviceReport.AddGroup(entry.GetAttribute("CommandGroupName"));
			this.currGroup.StartTime = entry.TimeStamp;
			this.currGroup.StartTickCount = entry.TickCount;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x0000373A File Offset: 0x0000193A
		private void OnEndCommandGroup(SummaryLogEntry entry)
		{
			if (this.currGroup == null)
			{
				throw new MtbfLogParserException("EndCommandGroup event for which no corresponding StartCommandGroup was found.");
			}
			this.UpdateDuration(this.currGroup, entry);
			this.currGroup = null;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00003764 File Offset: 0x00001964
		private void OnStartLoop(SummaryLogEntry entry)
		{
			if (this.currGroup == null)
			{
				throw new MtbfLogParserException("StartLoop event is found outside of a CommandGroup.");
			}
			if (this.currLoop != null)
			{
				throw new MtbfLogParserException("StartLoop event was found before the previous loop's EndLoop");
			}
			this.currLoop = this.currGroup.AddLoop(int.Parse(entry.GetAttribute("LoopNumber"), CultureInfo.InvariantCulture));
			this.currLoop.StartTime = entry.TimeStamp;
			this.currLoop.StartTickCount = entry.TickCount;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x000037DF File Offset: 0x000019DF
		private void OnEndLoop(SummaryLogEntry entry)
		{
			if (this.currLoop == null)
			{
				throw new MtbfLogParserException("EndLoop event for which no corresponding StartLoop was found.");
			}
			if (this.currSection != null)
			{
				this.OnEndSection(entry);
			}
			this.UpdateDuration(this.currLoop, entry);
			this.currLoop = null;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00003818 File Offset: 0x00001A18
		private void OnStartSection(SummaryLogEntry entry)
		{
			if (this.currLoop == null)
			{
				throw new MtbfLogParserException("StartSection event is found outside of a loop");
			}
			if (this.currSection != null)
			{
				throw new MtbfLogParserException("StartSection event was found before the previous sections's EndSection");
			}
			this.currSection = this.currLoop.AddSection(entry.GetAttribute("SectionNumber"));
			this.currSection.StartTime = entry.TimeStamp;
			this.currSection.StartTickCount = entry.TickCount;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00003889 File Offset: 0x00001A89
		private void OnEndSection(SummaryLogEntry entry)
		{
			if (this.currSection == null)
			{
				throw new MtbfLogParserException("EndSection event for which no corresponding StartSection was found.");
			}
			this.UpdateDuration(this.currSection, entry);
			this.currSection = null;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000038B4 File Offset: 0x00001AB4
		private void OnStartCommand(SummaryLogEntry entry)
		{
			if (this.currLoop == null)
			{
				throw new MtbfLogParserException("StartCommand event is found outside of a loop");
			}
			if (this.currTest != null)
			{
				throw new MtbfLogParserException("StartCommand event was found before the previous command's EndCommand");
			}
			if (this.currSection == null)
			{
				this.currSection = this.currLoop.AddSection(Settings.Default.UnknownSectionName);
				this.currSection.StartTime = entry.TimeStamp;
				this.currSection.StartTickCount = entry.TickCount;
			}
			this.currTest = new MtbfTestReport(entry.GetAttribute("TestFriendlyName"), int.Parse(entry.GetAttribute("CommandIndex")), entry.GetAttribute("Command"), entry.GetAttribute("Parameter"));
			this.currTest.StartTime = entry.TimeStamp;
			this.currTest.StartTickCount = entry.TickCount;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x0000398C File Offset: 0x00001B8C
		private void OnEndCommand(SummaryLogEntry entry)
		{
			if (this.currTest == null)
			{
				throw new MtbfLogParserException("EndCommand event for which no corresponding StartCommand was found.");
			}
			this.currTest.SetResult(entry.GetAttribute("TestResult"), int.Parse(entry.GetAttribute("ExpectedTestCaseCount"), CultureInfo.InvariantCulture), int.Parse(entry.GetAttribute("AttemptedTestCaseCount"), CultureInfo.InvariantCulture), int.Parse(entry.GetAttribute("PassedTestCaseCount"), CultureInfo.InvariantCulture), int.Parse(entry.GetAttribute("FailedTestCaseCount"), CultureInfo.InvariantCulture), int.Parse(entry.GetAttribute("SkippedTestCaseCount"), CultureInfo.InvariantCulture), int.Parse(entry.GetAttribute("AbortedTestCaseCount"), CultureInfo.InvariantCulture));
			this.UpdateDuration(this.currTest, entry);
			this.currSection.AddTest(this.currTest);
			this.currTest = null;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00003A68 File Offset: 0x00001C68
		private void OnLogFile(SummaryLogEntry entry)
		{
			if (this.currTest == null)
			{
				throw new MtbfLogParserException("LogFile event for which no corresponding StartCommand was found.");
			}
			string fileName = Path.GetFileName(entry.GetAttribute("File"));
			string text = Path.Combine(this.mtbfLogFilesFolderPath, fileName);
			if (File.Exists(text))
			{
				this.currTest.AddLogFile(text);
				return;
			}
			this.currTest.AddLogFile(fileName);
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00003AC8 File Offset: 0x00001CC8
		private void OnProcessingCompleted(SummaryLogEntry lastEntry)
		{
			if (this.currGroup != null)
			{
				this.UpdateDuration(this.currGroup, lastEntry);
			}
			if (this.currLoop != null)
			{
				this.UpdateDuration(this.currLoop, lastEntry);
			}
			if (this.currSection != null)
			{
				this.UpdateDuration(this.currSection, lastEntry);
			}
			if (this.currTest != null)
			{
				this.UpdateDuration(this.currTest, lastEntry);
			}
		}

		// Token: 0x04000030 RID: 48
		private Dictionary<string, Action<SummaryLogEntry>> logEntryActions;

		// Token: 0x04000031 RID: 49
		private MtbfGroupReport currGroup;

		// Token: 0x04000032 RID: 50
		private MtbfLoopReport currLoop;

		// Token: 0x04000033 RID: 51
		private MtbfSectionReport currSection;

		// Token: 0x04000034 RID: 52
		private MtbfTestReport currTest;

		// Token: 0x04000035 RID: 53
		private string mtbfLogFilesFolderPath;
	}
}
