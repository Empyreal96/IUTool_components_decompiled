using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Microsoft.Windows.ImageTools
{
	// Token: 0x02000005 RID: 5
	internal class EtwSession : IDisposable
	{
		// Token: 0x06000009 RID: 9 RVA: 0x0000258F File Offset: 0x0000078F
		public EtwSession() : this(true)
		{
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002598 File Offset: 0x00000798
		public EtwSession(bool log)
		{
			if (log)
			{
				this.sessionName = Process.GetCurrentProcess().ProcessName + Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture);
				this.EtlPath = Path.Combine(Path.GetTempPath(), this.sessionName + ".etl");
				this.traceHandle = ulong.MaxValue;
				this.StartTracing();
				return;
			}
			this.sessionName = string.Empty;
			this.EtlPath = string.Empty;
			this.traceHandle = ulong.MaxValue;
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000B RID: 11 RVA: 0x00002627 File Offset: 0x00000827
		// (set) Token: 0x0600000C RID: 12 RVA: 0x0000262F File Offset: 0x0000082F
		public string EtlPath { get; private set; }

		// Token: 0x0600000D RID: 13 RVA: 0x00002638 File Offset: 0x00000838
		public void Dispose()
		{
			if (this.traceHandle != 18446744073709551615UL)
			{
				EventTraceProperties eventTraceProperties;
				NativeMethods.StopTrace(this.traceHandle, this.sessionName, out eventTraceProperties);
				this.traceHandle = ulong.MaxValue;
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x0000266C File Offset: 0x0000086C
		private void StartTracing()
		{
			EventTraceProperties eventTraceProperties = EventTraceProperties.CreateProperties(this.sessionName, this.EtlPath, LoggingModeConstant.PrivateLoggerMode | LoggingModeConstant.PrivateInProc);
			int num = NativeMethods.StartTrace(out this.traceHandle, this.sessionName, ref eventTraceProperties);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
			Guid guid = new Guid("3bbd891e-180f-4386-94b5-d71ba7ac25a9");
			Guid guid2 = new Guid("fb961307-bc64-4de4-8828-81d583524da0");
			foreach (Guid providerId in new Guid[]
			{
				guid,
				guid2
			})
			{
				num = NativeMethods.EnableTraceEx2(this.traceHandle, providerId, 1U, TraceLevel.Verbose, ulong.MaxValue, 0UL, 0U);
				if (num != 0)
				{
					throw new Win32Exception(num);
				}
			}
		}

		// Token: 0x0400001B RID: 27
		private string sessionName;

		// Token: 0x0400001C RID: 28
		private ulong traceHandle;
	}
}
