using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary;

namespace Microsoft.TestInfra.UtilityLibrary
{
	// Token: 0x0200002C RID: 44
	public class WindowsJob : IDisposable
	{
		// Token: 0x060000CD RID: 205 RVA: 0x00005E58 File Offset: 0x00004058
		public WindowsJob()
		{
			this.jobObjectHandle = NativeMethods.CreateJobObject(null, null);
			NativeMethods.JOBOBJECT_BASIC_LIMIT_INFORMATION basicLimitInformation = default(NativeMethods.JOBOBJECT_BASIC_LIMIT_INFORMATION);
			basicLimitInformation.LimitFlags = NativeMethods.JOB_OBJECT_LIMIT.JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE;
			NativeMethods.JOBOBJECT_EXTENDED_LIMIT_INFORMATION jobobject_EXTENDED_LIMIT_INFORMATION = default(NativeMethods.JOBOBJECT_EXTENDED_LIMIT_INFORMATION);
			jobobject_EXTENDED_LIMIT_INFORMATION.BasicLimitInformation = basicLimitInformation;
			int num = Marshal.SizeOf(typeof(NativeMethods.JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr(jobobject_EXTENDED_LIMIT_INFORMATION, intPtr, false);
			if (!NativeMethods.SetInformationJobObject(this.jobObjectHandle, NativeMethods.JOBOBJECTINFOCLASS.ExtendedLimitInformation, intPtr, (uint)num))
			{
				throw new TypicalException<WindowsJob>(string.Format("Unable to set information.  Error: {0}", Marshal.GetLastWin32Error()));
			}
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00005EF4 File Offset: 0x000040F4
		~WindowsJob()
		{
			if (this.jobObjectHandle != IntPtr.Zero)
			{
				throw new InvalidOperationException("This IDisposable was not Disposed before the finalizer was called.  This is almost certainly a bug.  Please examine the code that instantiated the object and ensure it calls Dispose() or uses a using statement.");
			}
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00005F44 File Offset: 0x00004144
		public void Dispose()
		{
			if (this.jobObjectHandle != IntPtr.Zero)
			{
				NativeMethods.CloseHandle(this.jobObjectHandle);
				this.jobObjectHandle = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00005F8C File Offset: 0x0000418C
		public void AssignProcess(Process process)
		{
			if (!NativeMethods.AssignProcessToJobObject(this.jobObjectHandle, process.Handle))
			{
				throw new TypicalException<WindowsJob>(string.Format("AssignProcessToJobObject() failed for process '{0}'. Error: {1}", process.ProcessName, Marshal.GetLastWin32Error()));
			}
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00005FD0 File Offset: 0x000041D0
		public static bool IsProcessInJob(Process process, IntPtr job)
		{
			bool result = false;
			if (!NativeMethods.IsProcessInJob(process.Handle, job, out result))
			{
				throw new TypicalException<WindowsJob>(string.Format("IsProcessInJob() failed for process '{0}'. Error: {1}", process.ProcessName, Marshal.GetLastWin32Error()));
			}
			return result;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00006018 File Offset: 0x00004218
		public static bool IsProcessInAnyJob(Process process)
		{
			return WindowsJob.IsProcessInJob(process, IntPtr.Zero);
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00006038 File Offset: 0x00004238
		public bool IsProcessInJob(Process process)
		{
			return WindowsJob.IsProcessInJob(process, this.jobObjectHandle);
		}

		// Token: 0x040000A9 RID: 169
		private IntPtr jobObjectHandle;
	}
}
