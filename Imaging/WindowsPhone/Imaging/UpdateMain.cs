using System;
using System.Runtime.InteropServices;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200000A RID: 10
	internal sealed class UpdateMain : IDisposable
	{
		// Token: 0x06000076 RID: 118 RVA: 0x0000817E File Offset: 0x0000637E
		public static bool FAILED(int hr)
		{
			return hr < 0;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00008184 File Offset: 0x00006384
		public UpdateMain()
		{
			this.UpdateContext = UpdateMain.CreateUpdateContext();
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000081A4 File Offset: 0x000063A4
		~UpdateMain()
		{
			this.Dispose(false);
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000081D4 File Offset: 0x000063D4
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000081E4 File Offset: 0x000063E4
		private void Dispose(bool isDisposing)
		{
			if (this._alreadyDisposed)
			{
				return;
			}
			if (this.UpdateContext != IntPtr.Zero)
			{
				UpdateMain.Deinitialize(this.UpdateContext);
				if (UpdateMain.ReleaseUpdateContext(this.UpdateContext) == 0)
				{
					this.UpdateContext = IntPtr.Zero;
				}
			}
			this._alreadyDisposed = true;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00008236 File Offset: 0x00006436
		public int Initialize(int storeIdsCount, ImageStructures.STORE_ID[] storeIds, string UpdateInputFile, string AlternateStagingLocation, LogUtil.InteropLogString ErrorMsgHandler, LogUtil.InteropLogString WarningMsgHandler, LogUtil.InteropLogString InfoMsgHandler, LogUtil.InteropLogString DebugMsgHandler)
		{
			LogUtil.IULogTo(ErrorMsgHandler, WarningMsgHandler, InfoMsgHandler, DebugMsgHandler);
			UpdateMain.IU_LogTo(ErrorMsgHandler, WarningMsgHandler, InfoMsgHandler, DebugMsgHandler);
			return UpdateMain.Initialize(this.UpdateContext, storeIdsCount, storeIds, UpdateInputFile, AlternateStagingLocation);
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00008262 File Offset: 0x00006462
		public void RegisterProgressCallback(UpdateMain.IUPhase Phase)
		{
			UpdateMain.IU_InitializeDefaultProgressReporting(Phase);
		}

		// Token: 0x0600007D RID: 125 RVA: 0x0000826A File Offset: 0x0000646A
		public int PrepareUpdate()
		{
			this.RegisterProgressCallback(UpdateMain.IUPhase.IUPhase_Staging);
			return UpdateMain.PrepareUpdate(this.UpdateContext);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x0000827E File Offset: 0x0000647E
		public int ExecuteUpdate()
		{
			this.RegisterProgressCallback(UpdateMain.IUPhase.IUPhase_Commit);
			return UpdateMain.ExecuteUpdate(this.UpdateContext);
		}

		// Token: 0x0600007F RID: 127
		[DllImport("wcp.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		public static extern int WcpInitialize(out UIntPtr InitCookie);

		// Token: 0x06000080 RID: 128
		[DllImport("Ole32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		public static extern int CoGetMalloc(uint dwMemContext, out UIntPtr pMalloc);

		// Token: 0x06000081 RID: 129
		[DllImport("wcp.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		public static extern int SetIsolationIMalloc(UIntPtr IMalloc);

		// Token: 0x06000082 RID: 130
		[DllImport("wcp.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		public static extern int CreateNewWindows(uint dwFlags, [MarshalAs(UnmanagedType.LPWStr)] string szSystemDrive, ref UpdateMain.OFFLINE_STORE_CREATION_PARAMETERS pParameters, UIntPtr ppvKeys, out uint pdwDisposition);

		// Token: 0x06000083 RID: 131
		[DllImport("wcp.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		public static extern void WcpShutdown(UIntPtr InitCookie);

		// Token: 0x06000084 RID: 132
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern IntPtr CreateUpdateContext();

		// Token: 0x06000085 RID: 133
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern int ReleaseUpdateContext(IntPtr UpdateContext);

		// Token: 0x06000086 RID: 134
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern int Initialize(IntPtr UpdateContext, int storeIdsCount, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ImageStructures.STORE_ID[] storeIds, string UpdateInputFile, string AlternateStagingLocation);

		// Token: 0x06000087 RID: 135
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern void Deinitialize(IntPtr UpdateContext);

		// Token: 0x06000088 RID: 136
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern void IU_InitializeDefaultProgressReporting(UpdateMain.IUPhase Phase);

		// Token: 0x06000089 RID: 137
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern int PrepareUpdate(IntPtr UpdateContext);

		// Token: 0x0600008A RID: 138
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern int ExecuteUpdate(IntPtr UpdateContext);

		// Token: 0x0600008B RID: 139
		[DllImport("IUCore.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void IU_ClearCachedDataPath();

		// Token: 0x0600008C RID: 140
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern void IU_LogTo([MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString ErrorMsgHandler, [MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString WarningMsgHandler, [MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString InfoMsgHandler, [MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString DebugMsgHandler);

		// Token: 0x04000076 RID: 118
		public const int S_OK = 0;

		// Token: 0x04000077 RID: 119
		private IntPtr UpdateContext = IntPtr.Zero;

		// Token: 0x04000078 RID: 120
		private bool _alreadyDisposed;

		// Token: 0x02000011 RID: 17
		internal enum IUPhase
		{
			// Token: 0x040000A7 RID: 167
			IUPhase_Staging,
			// Token: 0x040000A8 RID: 168
			IUPhase_Commit
		}

		// Token: 0x02000012 RID: 18
		public struct OFFLINE_STORE_CREATION_PARAMETERS
		{
			// Token: 0x040000A9 RID: 169
			public UIntPtr cbSize;

			// Token: 0x040000AA RID: 170
			public uint dwFlags;

			// Token: 0x040000AB RID: 171
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszHostSystemDrivePath;

			// Token: 0x040000AC RID: 172
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszHostWindowsDirectoryPath;

			// Token: 0x040000AD RID: 173
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszTargetWindowsDirectoryPath;

			// Token: 0x040000AE RID: 174
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszHostRegistryMachineSoftwarePath;

			// Token: 0x040000AF RID: 175
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszHostRegistryMachineSystemPath;

			// Token: 0x040000B0 RID: 176
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszHostRegistryMachineSecurityPath;

			// Token: 0x040000B1 RID: 177
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszHostRegistryMachineSAMPath;

			// Token: 0x040000B2 RID: 178
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszHostRegistryMachineComponentsPath;

			// Token: 0x040000B3 RID: 179
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszHostRegistryUserDotDefaultPath;

			// Token: 0x040000B4 RID: 180
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszHostRegistryDefaultUserPath;

			// Token: 0x040000B5 RID: 181
			public uint ulProcessorArchitecture;

			// Token: 0x040000B6 RID: 182
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszHostRegistryMachineOfflineSchemaPath;
		}
	}
}
