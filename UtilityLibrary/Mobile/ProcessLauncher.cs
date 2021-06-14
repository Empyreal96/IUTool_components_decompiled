using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Microsoft.Mobile
{
	// Token: 0x02000021 RID: 33
	public class ProcessLauncher : IDisposable
	{
		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600008D RID: 141 RVA: 0x00004884 File Offset: 0x00002A84
		// (set) Token: 0x0600008E RID: 142 RVA: 0x0000489B File Offset: 0x00002A9B
		public Process Process { get; private set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600008F RID: 143 RVA: 0x000048A4 File Offset: 0x00002AA4
		// (set) Token: 0x06000090 RID: 144 RVA: 0x000048BB File Offset: 0x00002ABB
		public ProcessStartInfo ProcessStartInfo { get; private set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000091 RID: 145 RVA: 0x000048C4 File Offset: 0x00002AC4
		public bool IsRunning
		{
			get
			{
				return this.Process != null && !this.Process.HasExited;
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000048F0 File Offset: 0x00002AF0
		public ProcessLauncher(string fileName, string arguments) : this(fileName, arguments, null, null, null)
		{
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00004900 File Offset: 0x00002B00
		public ProcessLauncher(string fileName, string arguments, Action<string> onErrorLine, Action<string> onOutputLine, Action<string> onInfraLine)
		{
			this.SetDefaultTimeoutHandler(null);
			this.onErrorLine = onErrorLine;
			this.onOutputLine = onOutputLine;
			this.onInfraLine = onInfraLine;
			this.ProcessStartInfo = new ProcessStartInfo
			{
				FileName = fileName,
				Arguments = arguments,
				UseShellExecute = false,
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true
			};
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000049B4 File Offset: 0x00002BB4
		public void Start(bool runAttached = true)
		{
			this.CheckDisposed();
			if (this.Process != null)
			{
				throw new InvalidOperationException("The ProcessLauncher instance was already executed");
			}
			this.StartProcess(runAttached);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x000049EA File Offset: 0x00002BEA
		public void RunToExit()
		{
			this.RunToExit(-1);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x000049F8 File Offset: 0x00002BF8
		public bool RunToExit(int timeoutMs)
		{
			bool result;
			using (AutoResetEvent autoResetEvent = new AutoResetEvent(false))
			{
				result = this.RunToExit(timeoutMs, autoResetEvent);
			}
			return result;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00004A3C File Offset: 0x00002C3C
		public bool RunToExit(WaitHandle cancelEvent)
		{
			return this.RunToExit(-1, cancelEvent);
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00004A58 File Offset: 0x00002C58
		public bool RunToExit(int timeoutMs, WaitHandle cancelEvent)
		{
			this.CheckDisposed();
			if (timeoutMs < -1)
			{
				throw new ArgumentOutOfRangeException("timeout cannot be less than -1");
			}
			if (timeoutMs == -1)
			{
				timeoutMs = int.MaxValue;
			}
			bool flag = false;
			if (this.Process == null)
			{
				bool runAttached = timeoutMs != 0;
				this.StartProcess(runAttached);
			}
			bool result;
			if (!this.attached)
			{
				result = true;
			}
			else
			{
				try
				{
					this.runToExitCalled = true;
					int num = WaitHandle.WaitAny(new WaitHandle[]
					{
						this.processExitedEvent,
						cancelEvent
					}, timeoutMs, false);
					int num2 = num;
					switch (num2)
					{
					case 0:
						flag = true;
						break;
					case 1:
						this.SafeInvoke(this.onInfraLine, "Process execution was cancelled externally", new object[0]);
						break;
					default:
						if (num2 == 258)
						{
							this.timedOut = true;
							this.SafeInvoke(this.onInfraLine, "Process execution timed out", new object[0]);
						}
						break;
					}
					if (flag && this.captureOutput && this.Process.HasExited)
					{
						int millisecondsTimeout = 5000;
						if (!this.allStreamsClosed.Wait(millisecondsTimeout))
						{
							for (int i = 0; i < this.streamsClosed.Length; i++)
							{
								if (!this.streamsClosed[i])
								{
									this.SafeInvoke(this.onInfraLine, string.Format("Timed out waiting on stream close signal for {0}", (ProcessLauncher.StreamIdentifier)i), new object[0]);
								}
							}
						}
					}
					result = flag;
				}
				catch (Exception arg)
				{
					this.SafeInvoke(this.onInfraLine, "failed during process execution: " + arg, new object[0]);
					throw;
				}
				finally
				{
					this.CleanupProcessInstance(!flag);
				}
			}
			return result;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00004CEC File Offset: 0x00002EEC
		private void RunTimeoutHandler(Process dyingProcess)
		{
			string text = this.timeoutHandlerScriptPath ?? Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TimeoutHandler.bat");
			if (File.Exists(text))
			{
				try
				{
					this.SafeInvoke(this.onInfraLine, "Executing handler script for PID={0} NAME={1} at \"{2}\"", new object[]
					{
						dyingProcess.Id,
						dyingProcess.ProcessName,
						text
					});
					string lineHeader = string.Format("[{0}]: ", Path.GetFileName(text));
					using (ProcessLauncher processLauncher = new ProcessLauncher(ProcessLauncher.GetSystemToolFullPath("cmd"), string.Format("/c \"{0}\" {1} {2}", text, dyingProcess.Id, dyingProcess.ProcessName), delegate(string m)
					{
						this.SafeInvoke(this.onErrorLine, lineHeader + m, new object[0]);
					}, delegate(string m)
					{
						this.SafeInvoke(this.onOutputLine, lineHeader + m, new object[0]);
					}, delegate(string m)
					{
						this.SafeInvoke(this.onInfraLine, lineHeader + m, new object[0]);
					}))
					{
						processLauncher.RunToExit();
					}
				}
				catch (Exception ex)
				{
					this.SafeInvoke(this.onInfraLine, "Exception occurred trying to run handler script.", new object[0]);
					this.SafeInvoke(this.onInfraLine, ex.ToString(), new object[0]);
				}
			}
			else
			{
				this.SafeInvoke(this.onInfraLine, "No timeout handler script at \"{0}\", skipping.", new object[]
				{
					text
				});
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00004EA0 File Offset: 0x000030A0
		public void SetDefaultTimeoutHandler(string timeoutHandlerScriptPath = null)
		{
			this.TimeoutHandler = new Action<Process>(this.RunTimeoutHandler);
			this.timeoutHandlerScriptPath = timeoutHandlerScriptPath;
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600009B RID: 155 RVA: 0x00004EC0 File Offset: 0x000030C0
		// (set) Token: 0x0600009C RID: 156 RVA: 0x00004ED7 File Offset: 0x000030D7
		public Action<Process> TimeoutHandler { get; set; }

		// Token: 0x0600009D RID: 157 RVA: 0x00004EE0 File Offset: 0x000030E0
		private void StartProcess(bool runAttached)
		{
			this.attached = runAttached;
			this.captureOutput = (this.attached && (this.onOutputLine != null || this.onErrorLine != null));
			this.StartProcess(this.attached, this.captureOutput);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00004F34 File Offset: 0x00003134
		private void StartProcess(bool enableEvents, bool captureOutput)
		{
			try
			{
				this.Process = new Process
				{
					StartInfo = this.ProcessStartInfo,
					EnableRaisingEvents = enableEvents
				};
				if (captureOutput)
				{
					if (this.onOutputLine != null)
					{
						this.ProcessStartInfo.RedirectStandardOutput = true;
						this.Process.OutputDataReceived += this.OnOutputDataReceived;
					}
					else
					{
						this.SignalStreamCloseEvent(ProcessLauncher.StreamIdentifier.StdOut);
					}
					if (this.onErrorLine != null)
					{
						this.ProcessStartInfo.RedirectStandardError = true;
						this.Process.ErrorDataReceived += this.OnErrorDataReceived;
					}
					else
					{
						this.SignalStreamCloseEvent(ProcessLauncher.StreamIdentifier.StdErr);
					}
				}
				if (enableEvents)
				{
					this.Process.Exited += this.OnProcessExited;
				}
				this.Process.Start();
				if (captureOutput)
				{
					this.BeginReadingProcessOutput();
				}
			}
			catch (Exception arg)
			{
				this.SafeInvoke(this.onInfraLine, "failed before process execution: " + arg, new object[0]);
				throw;
			}
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00005068 File Offset: 0x00003268
		private void CleanupProcessInstance(bool forceTermination)
		{
			if (this.Process != null)
			{
				this.Process.Exited -= this.OnProcessExited;
				if (this.onOutputLine != null)
				{
					this.Process.OutputDataReceived -= this.OnOutputDataReceived;
				}
				if (this.onErrorLine != null)
				{
					this.Process.ErrorDataReceived -= this.OnErrorDataReceived;
				}
			}
			if (forceTermination)
			{
				this.ForceCleanup();
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x000050FC File Offset: 0x000032FC
		private void BeginReadingProcessOutput()
		{
			if (this.onOutputLine != null)
			{
				this.Process.BeginOutputReadLine();
			}
			if (this.onErrorLine != null)
			{
				this.Process.BeginErrorReadLine();
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00005140 File Offset: 0x00003340
		private void OnProcessExited(object sender, EventArgs e)
		{
			this.processExitedEvent.Set();
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00005150 File Offset: 0x00003350
		private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data != null)
			{
				this.SafeInvoke(this.onErrorLine, e.Data, new object[0]);
			}
			else
			{
				this.SignalStreamCloseEvent(ProcessLauncher.StreamIdentifier.StdErr);
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00005194 File Offset: 0x00003394
		private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data != null)
			{
				this.SafeInvoke(this.onOutputLine, e.Data, new object[0]);
			}
			else
			{
				this.SignalStreamCloseEvent(ProcessLauncher.StreamIdentifier.StdOut);
			}
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00005210 File Offset: 0x00003410
		public void ForceCleanup()
		{
			if (!this.IsDisposed)
			{
				try
				{
					if (this.Process == null || this.Process.HasExited)
					{
						return;
					}
					this.Process.CloseMainWindow();
					if (this.Process.HasExited)
					{
						return;
					}
				}
				catch (Exception arg)
				{
					this.SafeInvoke(this.onInfraLine, "failed during process cleanup: " + arg, new object[0]);
				}
				try
				{
					this.Process.KillProgeny(delegate(Process p)
					{
						if (this.timedOut && this.TimeoutHandler != null)
						{
							this.TimeoutHandler(p);
						}
					});
				}
				catch (Exception arg2)
				{
					this.SafeInvoke(this.onInfraLine, "failed to kill process: " + arg2, new object[0]);
				}
			}
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00005308 File Offset: 0x00003508
		private void CheckDisposed()
		{
			if (this.IsDisposed)
			{
				throw new ObjectDisposedException("Microsoft.Mobile.ProcessLauncher");
			}
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00005330 File Offset: 0x00003530
		private void SafeInvoke(Action<string> action, string arg, params object[] formatArgs)
		{
			if (action != null)
			{
				action((formatArgs.Length == 0) ? arg : string.Format(arg, formatArgs));
			}
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x0000535F File Offset: 0x0000355F
		private void SignalStreamCloseEvent(ProcessLauncher.StreamIdentifier streamId)
		{
			this.allStreamsClosed.Signal();
			this.streamsClosed[(int)streamId] = true;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00005378 File Offset: 0x00003578
		private static string GetSystemToolFullPath(string tool)
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), tool);
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00005398 File Offset: 0x00003598
		public void Dispose()
		{
			bool flag = true;
			if (!this.IsDisposed)
			{
				if (this.Process != null)
				{
					if (this.attached && !this.runToExitCalled)
					{
						flag = false;
					}
					this.Process.Dispose();
					this.Process = null;
				}
				this.IsDisposed = true;
			}
			if (!flag)
			{
				throw new InvalidOperationException("A ProcessLauncher instance is now being disposed, which was used to launch a waitable process without ever calling RunToExit().  Either make it asynchronous, or call RunToExit().");
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000AA RID: 170 RVA: 0x0000540C File Offset: 0x0000360C
		// (set) Token: 0x060000AB RID: 171 RVA: 0x00005423 File Offset: 0x00003623
		public bool IsDisposed { get; private set; }

		// Token: 0x04000089 RID: 137
		private Action<string> onErrorLine;

		// Token: 0x0400008A RID: 138
		private Action<string> onOutputLine;

		// Token: 0x0400008B RID: 139
		private Action<string> onInfraLine;

		// Token: 0x0400008C RID: 140
		private AutoResetEvent processExitedEvent = new AutoResetEvent(false);

		// Token: 0x0400008D RID: 141
		private string timeoutHandlerScriptPath = null;

		// Token: 0x0400008E RID: 142
		private CountdownEvent allStreamsClosed = new CountdownEvent(2);

		// Token: 0x0400008F RID: 143
		private bool[] streamsClosed = new bool[2];

		// Token: 0x04000090 RID: 144
		private bool timedOut = false;

		// Token: 0x04000091 RID: 145
		private bool attached = false;

		// Token: 0x04000092 RID: 146
		private bool captureOutput = false;

		// Token: 0x04000093 RID: 147
		private bool runToExitCalled = false;

		// Token: 0x02000022 RID: 34
		private enum StreamIdentifier
		{
			// Token: 0x04000099 RID: 153
			StdOut,
			// Token: 0x0400009A RID: 154
			StdErr,
			// Token: 0x0400009B RID: 155
			Last
		}
	}
}
