using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Microsoft.MetadataReader.Internal
{
	// Token: 0x02000002 RID: 2
	internal class Debug
	{
		// Token: 0x06000001 RID: 1
		[SuppressMessage("Microsoft.Usage", "CA2205:UseManagedEquivalentsOfWin32Api")]
		[SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
		[DllImport("user32.dll", BestFitMapping = false)]
		private static extern int MessageBoxA(int h, string m, string c, int type);

		// Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.MetadataReader.Internal.Debug.MessageBoxA(System.Int32,System.String,System.String,System.Int32)")]
		private static Debug.MessageBoxResult MessageBox(string message)
		{
			return (Debug.MessageBoxResult)Debug.MessageBoxA(0, message, "LMR Assert failed", 50);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002072 File Offset: 0x00000272
		[Conditional("DEBUG")]
		public static void Assert(bool f)
		{
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002078 File Offset: 0x00000278
		[Conditional("DEBUG")]
		public static void Assert(bool f, string message)
		{
			bool flag = !f;
			if (flag)
			{
				Debugger.Log(0, "assert", message);
				string stackTrace = Environment.StackTrace;
				Debug.MessageBoxResult messageBoxResult = Debug.MessageBox(message + "\r\n" + stackTrace + "\r\nAbort - terminate the process\r\nRetry - break into the debugger\r\nIgnore - ignore the assert and continue running");
				bool flag2 = messageBoxResult == Debug.MessageBoxResult.IDABORT;
				if (flag2)
				{
					Environment.Exit(1);
				}
				bool flag3 = messageBoxResult == Debug.MessageBoxResult.IDRETRY;
				if (flag3)
				{
					Debugger.Break();
				}
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002072 File Offset: 0x00000272
		[Conditional("DEBUG")]
		public static void Fail(string message)
		{
		}

		// Token: 0x02000023 RID: 35
		private enum MessageBoxResult
		{
			// Token: 0x04000082 RID: 130
			IDABORT = 3,
			// Token: 0x04000083 RID: 131
			IDRETRY,
			// Token: 0x04000084 RID: 132
			IDIGNORE
		}
	}
}
