using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Mobile
{
	// Token: 0x02000023 RID: 35
	public static class ProcessExtensions
	{
		// Token: 0x060000AD RID: 173 RVA: 0x0000545C File Offset: 0x0000365C
		public static void KillProgeny(this Process process, Action<Process> preKillVisitor = null)
		{
			try
			{
				List<Process> progeny = ProcessExtensions.ProcessHelper.GetProgeny(process);
				foreach (Process process2 in progeny)
				{
					try
					{
						if (preKillVisitor != null)
						{
							preKillVisitor(process2);
						}
						process2.Kill();
					}
					catch
					{
					}
				}
				int millisecondsTimeout = 5000;
				WaitHandle.WaitAll(progeny.ConvertAll<WaitHandle>((Process p) => new ManualResetEvent(false)
				{
					SafeWaitHandle = new SafeWaitHandle(p.Handle, false)
				}).ToArray<WaitHandle>(), millisecondsTimeout);
			}
			catch
			{
			}
		}

		// Token: 0x02000024 RID: 36
		private class ProcessHelper
		{
			// Token: 0x060000AF RID: 175 RVA: 0x00005578 File Offset: 0x00003778
			public static List<Process> GetProgeny(Process parent)
			{
				Process[] processes = Process.GetProcesses();
				KeyValuePair<Process, int>[] childParentPairs = new KeyValuePair<Process, int>[processes.Length];
				int i = 0;
				while (i < processes.Length)
				{
					Process process2 = processes[i];
					int value = 0;
					try
					{
						if (process2.Id != parent.Id && process2.StartTime >= parent.StartTime)
						{
							value = ProcessExtensions.ProcessHelper.GetParentPid(process2.Handle);
						}
					}
					catch
					{
						goto IL_85;
					}
					goto IL_6A;
					IL_85:
					i++;
					continue;
					IL_6A:
					childParentPairs[i] = new KeyValuePair<Process, int>(process2, value);
					goto IL_85;
				}
				List<Process> list = new List<Process>();
				list.Add(parent);
				int count;
				do
				{
					count = list.Count;
					int index;
					for (index = 0; index < childParentPairs.Length; index++)
					{
						bool flag;
						if (childParentPairs[index].Value != 0)
						{
							flag = !list.Any((Process process) => process.Id == childParentPairs[index].Value);
						}
						else
						{
							flag = true;
						}
						if (!flag)
						{
							list.Add(childParentPairs[index].Key);
							childParentPairs[index] = new KeyValuePair<Process, int>(childParentPairs[index].Key, 0);
						}
					}
				}
				while (count != list.Count);
				return list;
			}

			// Token: 0x060000B0 RID: 176
			[DllImport("NTDLL.DLL")]
			private static extern ProcessExtensions.ProcessHelper.NtStatus NtQueryInformationProcess(IntPtr hProcess, ProcessExtensions.ProcessHelper.PROCESSINFOCLASS pic, ref ProcessExtensions.ProcessHelper.PROCESS_BASIC_INFORMATION pbi, int sizePbi, out int pSize);

			// Token: 0x060000B1 RID: 177 RVA: 0x0000575C File Offset: 0x0000395C
			private static int GetParentPid(IntPtr hProcess)
			{
				ProcessExtensions.ProcessHelper.PROCESS_BASIC_INFORMATION process_BASIC_INFORMATION = default(ProcessExtensions.ProcessHelper.PROCESS_BASIC_INFORMATION);
				int num;
				ProcessExtensions.ProcessHelper.NtStatus ntStatus = ProcessExtensions.ProcessHelper.NtQueryInformationProcess(hProcess, ProcessExtensions.ProcessHelper.PROCESSINFOCLASS.ProcessBasicInformation, ref process_BASIC_INFORMATION, process_BASIC_INFORMATION.Size, out num);
				int result;
				if (ntStatus == ProcessExtensions.ProcessHelper.NtStatus.Success)
				{
					result = (int)process_BASIC_INFORMATION.InheritedFromUniqueProcessId;
				}
				else
				{
					result = 0;
				}
				return result;
			}

			// Token: 0x02000025 RID: 37
			private enum PROCESSINFOCLASS
			{
				// Token: 0x0400009E RID: 158
				ProcessBasicInformation
			}

			// Token: 0x02000026 RID: 38
			private enum NtStatus : uint
			{
				// Token: 0x040000A0 RID: 160
				Success
			}

			// Token: 0x02000027 RID: 39
			[StructLayout(LayoutKind.Sequential, Pack = 1)]
			private struct PROCESS_BASIC_INFORMATION
			{
				// Token: 0x17000026 RID: 38
				// (get) Token: 0x060000B3 RID: 179 RVA: 0x000057B0 File Offset: 0x000039B0
				public int Size
				{
					get
					{
						return Marshal.SizeOf(typeof(ProcessExtensions.ProcessHelper.PROCESS_BASIC_INFORMATION));
					}
				}

				// Token: 0x040000A1 RID: 161
				public IntPtr ExitStatus;

				// Token: 0x040000A2 RID: 162
				public IntPtr PebBaseAddress;

				// Token: 0x040000A3 RID: 163
				public IntPtr AffinityMask;

				// Token: 0x040000A4 RID: 164
				public IntPtr BasePriority;

				// Token: 0x040000A5 RID: 165
				public UIntPtr UniqueProcessId;

				// Token: 0x040000A6 RID: 166
				public IntPtr InheritedFromUniqueProcessId;
			}
		}
	}
}
