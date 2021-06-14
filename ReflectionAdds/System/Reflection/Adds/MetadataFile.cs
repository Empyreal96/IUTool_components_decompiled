using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Reflection.Adds
{
	// Token: 0x02000016 RID: 22
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class MetadataFile : IDisposable
	{
		// Token: 0x06000092 RID: 146 RVA: 0x00003938 File Offset: 0x00001B38
		public MetadataFile(object importer)
		{
			bool flag = importer == null;
			if (flag)
			{
				throw new ArgumentNullException("importer");
			}
			this._rawPointer = Marshal.GetIUnknownForObject(importer);
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00003970 File Offset: 0x00001B70
		internal MetadataFile(IntPtr rawImporter)
		{
			bool flag = rawImporter == IntPtr.Zero;
			if (flag)
			{
				throw new ArgumentNullException("rawImporter");
			}
			Marshal.AddRef(rawImporter);
			this._rawPointer = rawImporter;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000039B0 File Offset: 0x00001BB0
		~MetadataFile()
		{
			this.Dispose(false);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x000039E4 File Offset: 0x00001BE4
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x000039F8 File Offset: 0x00001BF8
		protected virtual void Dispose(bool disposing)
		{
			bool flag = this._rawPointer != IntPtr.Zero;
			if (flag)
			{
				Marshal.Release(this._rawPointer);
			}
			this._rawPointer = IntPtr.Zero;
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000097 RID: 151 RVA: 0x00003A34 File Offset: 0x00001C34
		public IntPtr RawPtr
		{
			get
			{
				this.EnsureNotDispose();
				return this._rawPointer;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000098 RID: 152 RVA: 0x00003A54 File Offset: 0x00001C54
		public virtual string FilePath
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00003A67 File Offset: 0x00001C67
		public virtual byte[] ReadRva(long rva, int countBytes)
		{
			throw new NotSupportedException(Resources.RVAUnsupported);
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00003A67 File Offset: 0x00001C67
		public virtual byte[] ReadResource(long offset)
		{
			throw new NotSupportedException(Resources.RVAUnsupported);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00003A74 File Offset: 0x00001C74
		public byte[] ReadEmbeddedBlob(EmbeddedBlobPointer pointer, int countBytes)
		{
			this.EnsureNotDispose();
			bool flag = countBytes == 0;
			byte[] result;
			if (flag)
			{
				result = new byte[0];
			}
			else
			{
				IntPtr getDangerousLivePointer = pointer.GetDangerousLivePointer;
				this.ValidateRange(getDangerousLivePointer, countBytes);
				byte[] array = new byte[countBytes];
				Marshal.Copy(getDangerousLivePointer, array, 0, countBytes);
				result = array;
			}
			return result;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00002072 File Offset: 0x00000272
		protected virtual void ValidateRange(IntPtr ptr, int countBytes)
		{
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00003A67 File Offset: 0x00001C67
		public virtual Token ReadEntryPointToken()
		{
			throw new NotSupportedException(Resources.RVAUnsupported);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00003AC4 File Offset: 0x00001CC4
		public virtual T ReadRvaStruct<T>(long rva) where T : new()
		{
			this.EnsureNotDispose();
			int countBytes = Marshal.SizeOf(typeof(T));
			byte[] value = this.ReadRva(rva, countBytes);
			GCHandle gchandle = GCHandle.Alloc(value, GCHandleType.Pinned);
			T result;
			try
			{
				IntPtr ptr = gchandle.AddrOfPinnedObject();
				result = (T)((object)Marshal.PtrToStructure(ptr, typeof(T)));
			}
			finally
			{
				gchandle.Free();
			}
			return result;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00003B40 File Offset: 0x00001D40
		protected void EnsureNotDispose()
		{
			bool flag = this._rawPointer == IntPtr.Zero;
			if (flag)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
		}

		// Token: 0x04000047 RID: 71
		[SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
		private IntPtr _rawPointer;
	}
}
