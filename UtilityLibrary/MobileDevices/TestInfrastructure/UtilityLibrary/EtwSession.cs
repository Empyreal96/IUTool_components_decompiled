using System;
using System.ComponentModel;

namespace Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary
{
	// Token: 0x0200000E RID: 14
	public class EtwSession : IDisposable
	{
		// Token: 0x0600005B RID: 91 RVA: 0x00003D68 File Offset: 0x00001F68
		public EtwSession(string sessionName, string path)
		{
			this.traceHandle = ulong.MaxValue;
			this.sessionName = sessionName;
			this.EtlPath = path;
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00003D8C File Offset: 0x00001F8C
		// (set) Token: 0x0600005D RID: 93 RVA: 0x00003DA3 File Offset: 0x00001FA3
		public string EtlPath { get; private set; }

		// Token: 0x0600005E RID: 94 RVA: 0x00003DAC File Offset: 0x00001FAC
		public void Dispose()
		{
			this.StopTracing();
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00003DB8 File Offset: 0x00001FB8
		public void StartTracing()
		{
			EventTraceProperties eventTraceProperties = EventTraceProperties.CreateProperties(this.sessionName, this.EtlPath, LoggingModeConstant.PrivateLoggerMode | LoggingModeConstant.PrivateInProc);
			int num = NativeMethods.StartTrace(out this.traceHandle, this.sessionName, ref eventTraceProperties);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003E04 File Offset: 0x00002004
		public void AddProvider(Guid providerGuid)
		{
			if (this.traceHandle == 18446744073709551615UL)
			{
				throw new InvalidOperationException("AddProvider requires the etw session to be started");
			}
			int num = NativeMethods.EnableTraceEx2(this.traceHandle, providerGuid, 1U, TraceLevel.Verbose, ulong.MaxValue, 0UL, 0U);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00003E54 File Offset: 0x00002054
		public void StopTracing()
		{
			if (this.traceHandle != 18446744073709551615UL)
			{
				EventTraceProperties eventTraceProperties = EventTraceProperties.CreateProperties(this.sessionName, null, (LoggingModeConstant)0U);
				int num = NativeMethods.ControlTrace(this.traceHandle, null, ref eventTraceProperties, 1U);
				this.traceHandle = ulong.MaxValue;
				if (num != 0)
				{
					throw new Win32Exception(num);
				}
			}
		}

		// Token: 0x04000034 RID: 52
		public const int MaxPrivateLoggingSession = 3;

		// Token: 0x04000035 RID: 53
		private string sessionName;

		// Token: 0x04000036 RID: 54
		private ulong traceHandle;
	}
}
