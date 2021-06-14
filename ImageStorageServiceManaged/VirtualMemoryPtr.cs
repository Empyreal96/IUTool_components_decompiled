using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200004E RID: 78
	public sealed class VirtualMemoryPtr : SafeHandle
	{
		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x0600039D RID: 925 RVA: 0x000111B7 File Offset: 0x0000F3B7
		public IntPtr AllocatedPointer
		{
			get
			{
				return this._allocatedPointer;
			}
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x0600039E RID: 926 RVA: 0x000111BF File Offset: 0x0000F3BF
		[CLSCompliant(false)]
		public uint MemorySize
		{
			get
			{
				return (uint)this._memorySize;
			}
		}

		// Token: 0x0600039F RID: 927 RVA: 0x000111CC File Offset: 0x0000F3CC
		[CLSCompliant(false)]
		public VirtualMemoryPtr(uint memorySize) : base(IntPtr.Zero, true)
		{
			this._memorySize = (UIntPtr)memorySize;
			try
			{
				this._allocatedPointer = Win32Exports.VirtualAlloc(this._memorySize, Win32Exports.AllocationType.MEM_COMMIT | Win32Exports.AllocationType.MEM_RESERVE, Win32Exports.MemoryProtection.PAGE_READWRITE);
			}
			catch (Win32ExportException innerException)
			{
				throw new ImageStorageException("Unable to create the virtual memory pointer.", innerException);
			}
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x00011228 File Offset: 0x0000F428
		public static implicit operator IntPtr(VirtualMemoryPtr virtualMemoryPointer)
		{
			return virtualMemoryPointer.AllocatedPointer;
		}

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x060003A1 RID: 929 RVA: 0x00011230 File Offset: 0x0000F430
		public override bool IsInvalid
		{
			get
			{
				return this._disposed;
			}
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x00011238 File Offset: 0x0000F438
		protected override bool ReleaseHandle()
		{
			if (!this._disposed)
			{
				this._disposed = true;
				GC.SuppressFinalize(this);
				try
				{
					Win32Exports.VirtualFree(this.AllocatedPointer, Win32Exports.FreeType.MEM_RELEASE);
				}
				catch (Win32ExportException arg)
				{
					throw new ImageStorageException(string.Format("Unable to free the virtual memory pointer.", arg));
				}
			}
			return true;
		}

		// Token: 0x040001F0 RID: 496
		private readonly IntPtr _allocatedPointer;

		// Token: 0x040001F1 RID: 497
		private readonly UIntPtr _memorySize;

		// Token: 0x040001F2 RID: 498
		private bool _disposed;
	}
}
