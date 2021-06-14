using System;
using System.Text;
using Microsoft.Diagnostics.Telemetry;
using Microsoft.Diagnostics.Tracing;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200000C RID: 12
	internal sealed class ImagingTelemetryLogger
	{
		// Token: 0x06000092 RID: 146 RVA: 0x000082E7 File Offset: 0x000064E7
		private ImagingTelemetryLogger()
		{
			this._logger = new TelemetryEventSource("Microsoft-Windows-Deployment-Imaging");
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000093 RID: 147 RVA: 0x0000830A File Offset: 0x0000650A
		public static ImagingTelemetryLogger Instance
		{
			get
			{
				if (ImagingTelemetryLogger._instance == null)
				{
					ImagingTelemetryLogger._instance = new ImagingTelemetryLogger();
				}
				return ImagingTelemetryLogger._instance;
			}
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00008324 File Offset: 0x00006524
		public void LogString(string _eventName, Guid sessionId, params string[] values)
		{
			switch (values.Length)
			{
			case 0:
				this._logger.Write("Imaging", this._telemetryOptionMeasure, new
				{
					EventName = _eventName,
					Value1 = sessionId
				});
				return;
			case 1:
				this._logger.Write("Imaging", this._telemetryOptionMeasure, new
				{
					EventName = _eventName,
					Value1 = sessionId,
					Value2 = values[0]
				});
				return;
			case 2:
				this._logger.Write("Imaging", this._telemetryOptionMeasure, new
				{
					EventName = _eventName,
					Value1 = sessionId,
					Value2 = values[0],
					Value3 = values[1]
				});
				return;
			case 3:
				this._logger.Write("Imaging", this._telemetryOptionMeasure, new
				{
					EventName = _eventName,
					Value1 = sessionId,
					Value2 = values[0],
					Value3 = values[1],
					Value4 = values[2]
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
				this._logger.Write("Imaging", this._telemetryOptionMeasure, new
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

		// Token: 0x04000079 RID: 121
		private static ImagingTelemetryLogger _instance;

		// Token: 0x0400007A RID: 122
		private const string _generalEventName = "Imaging";

		// Token: 0x0400007B RID: 123
		private TelemetryEventSource _logger;

		// Token: 0x0400007C RID: 124
		private readonly EventSourceOptions _telemetryOptionMeasure = TelemetryEventSource.MeasuresOptions();
	}
}
