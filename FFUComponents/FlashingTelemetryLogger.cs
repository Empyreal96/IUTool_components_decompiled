using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Diagnostics.Telemetry;
using Microsoft.Diagnostics.Tracing;

namespace FFUComponents
{
	// Token: 0x0200004C RID: 76
	public sealed class FlashingTelemetryLogger
	{
		// Token: 0x0600024C RID: 588 RVA: 0x0000A0C8 File Offset: 0x000082C8
		private FlashingTelemetryLogger()
		{
			this.logger = new TelemetryEventSource("Microsoft-Windows-Deployment-Flashing");
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600024D RID: 589 RVA: 0x0000A10A File Offset: 0x0000830A
		public static FlashingTelemetryLogger Instance
		{
			get
			{
				if (FlashingTelemetryLogger.instance == null)
				{
					FlashingTelemetryLogger.instance = new FlashingTelemetryLogger();
				}
				return FlashingTelemetryLogger.instance;
			}
		}

		// Token: 0x0600024E RID: 590 RVA: 0x0000A124 File Offset: 0x00008324
		public void LogFlashingInitialized(Guid sessionId, IFFUDevice device, bool optimizeHint, string ffuFile)
		{
			try
			{
				this.LogString("FlashingInitialized", sessionId, new string[]
				{
					optimizeHint.ToString()
				});
				this.LogString("DeviceInfo", sessionId, new string[]
				{
					device.GetType().Name,
					device.DeviceFriendlyName
				});
				string location = base.GetType().Assembly.Location;
				FileInfo fileInfo = new FileInfo(location);
				this.LogString("DllInfo", sessionId, new string[]
				{
					location,
					FileVersionInfo.GetVersionInfo(location).ProductVersion,
					fileInfo.CreationTime.ToString("yyMMdd-HHmm", CultureInfo.InvariantCulture),
					fileInfo.LastWriteTime.ToString("yyMMdd-HHmm", CultureInfo.InvariantCulture)
				});
				string location2 = Assembly.GetEntryAssembly().Location;
				FileInfo fileInfo2 = new FileInfo(location2);
				string fileName = Path.GetFileName(location2);
				this.LogString("ExeInfo", sessionId, new string[]
				{
					fileName,
					location2,
					FileVersionInfo.GetVersionInfo(location2).ProductVersion,
					fileInfo2.CreationTime.ToString("yyMMdd-HHmm", CultureInfo.InvariantCulture),
					fileInfo2.LastWriteTime.ToString("yyMMdd-HHmm", CultureInfo.InvariantCulture)
				});
				this.LogFFUReadSpeed(sessionId, ffuFile);
			}
			catch
			{
			}
		}

		// Token: 0x0600024F RID: 591 RVA: 0x0000A290 File Offset: 0x00008490
		public void LogFlashingStarted(Guid sessionId)
		{
			this.LogString("FlashingStarted", sessionId, new string[0]);
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000A2A4 File Offset: 0x000084A4
		public void LogFlashingEnded(Guid sessionId, Stopwatch flashingStopwatch, string ffuFilePath, IFFUDevice device)
		{
			if ((float)flashingStopwatch.ElapsedMilliseconds == 0f)
			{
				this.LogFlashingFailed(sessionId, "Flashing took less than 1 second, which is impossible.");
				return;
			}
			float num = (float)new FileInfo(ffuFilePath).Length / 1024f / 1024f;
			float flashingSpeed = num / ((float)flashingStopwatch.ElapsedMilliseconds / 1000f);
			this.LogFlashingCompleted(sessionId, flashingStopwatch.Elapsed.TotalSeconds, flashingSpeed, device);
			this.LogString("FFUInfo", sessionId, new string[]
			{
				ffuFilePath,
				num.ToString()
			});
			this.LogFFULocation(sessionId, ffuFilePath);
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000A336 File Offset: 0x00008536
		public void LogFlashingException(Guid sessionId, Exception e)
		{
			this.LogFlashingFailed(sessionId, e.Message);
			if (e.InnerException != null)
			{
				this.LogString("FlashingException", sessionId, new string[]
				{
					e.InnerException.ToString()
				});
			}
		}

		// Token: 0x06000252 RID: 594 RVA: 0x0000A36D File Offset: 0x0000856D
		public void LogThorDeviceUSBConnectionType(Guid sessionId, ConnectionType connectionType)
		{
			this.LogString("ThorDeviceUSBConnectionType", sessionId, new string[]
			{
				connectionType.ToString()
			});
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000A394 File Offset: 0x00008594
		private void LogString(string _eventName, Guid sessionId, params string[] values)
		{
			switch (values.Length)
			{
			case 0:
				this.logger.Write("Flashing", this.telemetryOptionMeasures, new
				{
					EventName = _eventName,
					Value1 = sessionId
				});
				return;
			case 1:
				this.logger.Write("Flashing", this.telemetryOptionMeasures, new
				{
					EventName = _eventName,
					Value1 = sessionId,
					Value2 = values[0]
				});
				return;
			case 2:
				this.logger.Write("Flashing", this.telemetryOptionMeasures, new
				{
					EventName = _eventName,
					Value1 = sessionId,
					Value2 = values[0],
					Value3 = values[1]
				});
				return;
			case 3:
				this.logger.Write("Flashing", this.telemetryOptionMeasures, new
				{
					EventName = _eventName,
					Value1 = sessionId,
					Value2 = values[0],
					Value3 = values[1],
					Value4 = values[2]
				});
				return;
			case 4:
				this.logger.Write("Flashing", this.telemetryOptionMeasures, new
				{
					EventName = _eventName,
					Value1 = sessionId,
					Value2 = values[0],
					Value3 = values[1],
					Value4 = values[2],
					Value5 = values[3]
				});
				return;
			case 5:
				this.logger.Write("Flashing", this.telemetryOptionMeasures, new
				{
					EventName = _eventName,
					Value1 = sessionId,
					Value2 = values[0],
					Value3 = values[1],
					Value4 = values[2],
					Value5 = values[3],
					Value6 = values[4]
				});
				return;
			default:
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string value in values)
				{
					stringBuilder.Append(value);
					stringBuilder.Append(";");
				}
				this.logger.Write("Flashing", this.telemetryOptionMeasures, new
				{
					EventName = _eventName,
					Value1 = sessionId,
					Value2 = "Values count exceeded supported count.",
					Value3 = stringBuilder.ToString()
				});
				return;
			}
			}
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000A500 File Offset: 0x00008700
		private void LogFFULocation(Guid sessionId, string ffuFilePath)
		{
			try
			{
				Uri uri = new Uri(ffuFilePath);
				if (uri.IsUnc)
				{
					if (uri.IsLoopback)
					{
						DriveType localPathDriveType = this.GetLocalPathDriveType(uri.LocalPath);
						if (localPathDriveType == DriveType.Unknown)
						{
							this.LogFileLocation(sessionId, ffuFilePath, DriveType.Network, "The location was inferred based on best guess and can be inaccurate.");
						}
						else
						{
							this.LogFileLocation(sessionId, ffuFilePath, localPathDriveType, null);
						}
					}
					else
					{
						this.LogFileLocation(sessionId, ffuFilePath, DriveType.Network, "The location was inferred based on best guess and can be inaccurate.");
					}
				}
				else
				{
					this.LogFileLocation(sessionId, ffuFilePath, this.GetLocalPathDriveType(ffuFilePath), null);
				}
			}
			catch (Exception ex)
			{
				this.LogString("GetFFUFileLocationFailed", sessionId, new string[]
				{
					ffuFilePath,
					ex.Message
				});
			}
		}

		// Token: 0x06000255 RID: 597 RVA: 0x0000A5A4 File Offset: 0x000087A4
		private void LogFlashingCompleted(Guid sessionId, double elapsedTimeSeconds, float flashingSpeed, IFFUDevice device)
		{
			this.LogString("FlashingCompleted", sessionId, new string[]
			{
				elapsedTimeSeconds.ToString(),
				flashingSpeed.ToString(),
				device.GetType().Name
			});
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0000A5DB File Offset: 0x000087DB
		private void LogFlashingFailed(Guid sessionId, string message)
		{
			this.LogString("FlashingFailed", sessionId, new string[]
			{
				message
			});
		}

		// Token: 0x06000257 RID: 599 RVA: 0x0000A5F3 File Offset: 0x000087F3
		private void LogFileLocation(Guid sessionId, string ffuFilePath, DriveType ffuDriveType, string warningMessage)
		{
			this.LogString("FFUFileLocation", sessionId, new string[]
			{
				ffuFilePath,
				ffuDriveType.ToString(),
				warningMessage
			});
		}

		// Token: 0x06000258 RID: 600 RVA: 0x0000A620 File Offset: 0x00008820
		private DriveType GetLocalPathDriveType(string filePath)
		{
			foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
			{
				if (string.Equals(Path.GetPathRoot(filePath), driveInfo.Name, StringComparison.OrdinalIgnoreCase))
				{
					return driveInfo.DriveType;
				}
			}
			return DriveType.Unknown;
		}

		// Token: 0x06000259 RID: 601 RVA: 0x0000A664 File Offset: 0x00008864
		private void LogFFUReadSpeed(Guid sessionId, string ffuFilePath)
		{
			try
			{
				using (BinaryReader binaryReader = new BinaryReader(new FileStream(ffuFilePath, FileMode.Open, FileAccess.Read)))
				{
					int num = 52428800;
					byte[] buffer = new byte[num];
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
					binaryReader.Read(buffer, 0, num);
					stopwatch.Stop();
					this.LogString("FFUReadSpeed", sessionId, new string[]
					{
						(50000f / (float)stopwatch.ElapsedMilliseconds).ToString(),
						stopwatch.ElapsedMilliseconds.ToString(),
						Stopwatch.IsHighResolution.ToString()
					});
				}
			}
			catch (Exception ex)
			{
				this.LogString("GetFFUReadSpeedFailed", sessionId, new string[]
				{
					ffuFilePath,
					ex.Message
				});
			}
		}

		// Token: 0x04000173 RID: 371
		private static FlashingTelemetryLogger instance;

		// Token: 0x04000174 RID: 372
		public const string ErrorFlashingTimeTooShort = "Flashing took less than 1 second, which is impossible.";

		// Token: 0x04000175 RID: 373
		private const string generalEventName = "Flashing";

		// Token: 0x04000176 RID: 374
		private TelemetryEventSource logger;

		// Token: 0x04000177 RID: 375
		private readonly EventSourceOptions telemetryOptionMeasures = new EventSourceOptions
		{
			Keywords = (EventKeywords)140737488355328L
		};

		// Token: 0x04000178 RID: 376
		private const string fileLocationEventName = "FFUFileLocation";

		// Token: 0x04000179 RID: 377
		private const string fileLocationNotReliableWarning = "The location was inferred based on best guess and can be inaccurate.";

		// Token: 0x0400017A RID: 378
		private const int ffuReadSpeedTestLenghMB = 50;
	}
}
