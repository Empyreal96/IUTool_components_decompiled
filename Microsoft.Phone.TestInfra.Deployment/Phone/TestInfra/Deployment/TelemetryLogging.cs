using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Diagnostics.Telemetry;
using Microsoft.Diagnostics.Tracing;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000015 RID: 21
	public static class TelemetryLogging
	{
		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000102 RID: 258 RVA: 0x000069F0 File Offset: 0x00004BF0
		public static EventSource Log
		{
			get
			{
				bool flag = TelemetryLogging.log == null;
				if (flag)
				{
					TelemetryLogging.Instance(null);
				}
				return TelemetryLogging.log;
			}
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00006A1C File Offset: 0x00004C1C
		public static EventSource Instance(string providersName = null)
		{
			bool flag = providersName != null;
			if (flag)
			{
				TelemetryLogging.telemetryProvider = providersName;
			}
			bool flag2 = TelemetryLogging.log == null;
			if (flag2)
			{
				TelemetryLogging.log = new TelemetryEventSource(TelemetryLogging.telemetryProvider);
			}
			return TelemetryLogging.log;
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00006A60 File Offset: 0x00004C60
		public static string EnumToString(this Enum eff)
		{
			return Enum.GetName(eff.GetType(), eff);
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00006A80 File Offset: 0x00004C80
		public static void LogEvent(TelemetryLogging.SpkgDeployTelemetryEventType eventType, IEnumerable<string> packages, IEnumerable<string> rootPath, IEnumerable<string> altRootPath, string message, DateTime time)
		{
			TelemetryLogging.Log.Write(eventType.EnumToString(), TelemetryLogging.TelemetryInfoOption, new
			{
				MachineName = Environment.MachineName,
				EventTime = time.ToString(),
				Message = (message ?? string.Empty),
				Packages = ((packages != null && packages.Any<string>()) ? string.Join(";", packages) : string.Empty),
				RootPath = ((rootPath != null && rootPath.Any<string>()) ? string.Join(";", rootPath) : string.Empty),
				AltRootPath = ((altRootPath != null && altRootPath.Any<string>()) ? string.Join(";", altRootPath) : string.Empty)
			});
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00006B22 File Offset: 0x00004D22
		public static void LogEvent(TelemetryLogging.SpkgDeployTelemetryEventType eventType, string message)
		{
			TelemetryLogging.log.Write(eventType.EnumToString(), TelemetryLogging.TelemetryInfoOption, new
			{
				MachineName = Environment.MachineName,
				Message = message
			});
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00006B4B File Offset: 0x00004D4B
		public static void LogError(TelemetryLogging.SpkgDeployTelemetryEventType eventType, string errorMessage)
		{
			TelemetryLogging.log.Write(eventType.EnumToString(), TelemetryLogging.TelemetryErrorOption, new
			{
				MachineName = Environment.MachineName,
				ExceptionMessage = errorMessage
			});
		}

		// Token: 0x04000061 RID: 97
		public static readonly EventSourceOptions MeasuresOption = new EventSourceOptions
		{
			Keywords = (EventKeywords)70368744177664L
		};

		// Token: 0x04000062 RID: 98
		public static readonly EventSourceOptions TelemetryInfoOption = new EventSourceOptions
		{
			Keywords = (EventKeywords)35184372088832L,
			Level = EventLevel.Informational,
			Opcode = EventOpcode.Info
		};

		// Token: 0x04000063 RID: 99
		public static readonly EventSourceOptions TelemetryErrorOption = new EventSourceOptions
		{
			Keywords = (EventKeywords)35184372088832L,
			Level = EventLevel.Error,
			Opcode = EventOpcode.Info
		};

		// Token: 0x04000064 RID: 100
		public static readonly EventSourceOptions TelemetryStartOption = new EventSourceOptions
		{
			Keywords = (EventKeywords)35184372088832L,
			Opcode = EventOpcode.Start
		};

		// Token: 0x04000065 RID: 101
		public static readonly EventSourceOptions TelemetryStopOption = new EventSourceOptions
		{
			Keywords = (EventKeywords)35184372088832L,
			Opcode = EventOpcode.Stop
		};

		// Token: 0x04000066 RID: 102
		private static EventSource log;

		// Token: 0x04000067 RID: 103
		private static string telemetryProvider = "Microsoft.OSG.QBI.SpkgDeploy";

		// Token: 0x02000047 RID: 71
		[Description("Telemetry Logging type")]
		public enum TelemetryLogType
		{
			// Token: 0x04000137 RID: 311
			Unknown,
			// Token: 0x04000138 RID: 312
			LogEvent,
			// Token: 0x04000139 RID: 313
			LogError
		}

		// Token: 0x02000048 RID: 72
		[Description("The spkg deployment events")]
		public enum SpkgDeployTelemetryEventType
		{
			// Token: 0x0400013B RID: 315
			Unknown,
			// Token: 0x0400013C RID: 316
			SpkgDeployStart,
			// Token: 0x0400013D RID: 317
			SpkgDeployFinished,
			// Token: 0x0400013E RID: 318
			SpkgErrorOccurred
		}
	}
}
