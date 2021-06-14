using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Adds
{
	// Token: 0x02000017 RID: 23
	internal class MetadataFileAndRvaResolver : MetadataFile
	{
		// Token: 0x060000A0 RID: 160 RVA: 0x00003B74 File Offset: 0x00001D74
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			this._file.Dispose();
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x00003B8C File Offset: 0x00001D8C
		public override string FilePath
		{
			get
			{
				return this._file.Path;
			}
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00003BA9 File Offset: 0x00001DA9
		public MetadataFileAndRvaResolver(IntPtr importer, FileMapping file) : base(importer)
		{
			this._file = file;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00003BBB File Offset: 0x00001DBB
		public MetadataFileAndRvaResolver(object importer, FileMapping file) : base(importer)
		{
			this._file = file;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00003BD0 File Offset: 0x00001DD0
		private IntPtr ResolveRva(long rva)
		{
			ImageHelper imageHelper = new ImageHelper(this._file.BaseAddress, this._file.Length);
			IntPtr intPtr = imageHelper.ResolveRva(rva);
			bool flag = intPtr == IntPtr.Zero;
			if (flag)
			{
				throw new InvalidOperationException(Resources.CannotResolveRVA);
			}
			return intPtr;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00003C24 File Offset: 0x00001E24
		public override byte[] ReadRva(long rva, int countBytes)
		{
			IntPtr source = this.ResolveRva(rva);
			byte[] array = new byte[countBytes];
			Marshal.Copy(source, array, 0, array.Length);
			return array;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00003C54 File Offset: 0x00001E54
		protected override void ValidateRange(IntPtr ptr, int countBytes)
		{
			long num = this._file.BaseAddress.ToInt64();
			long num2 = num + this._file.Length;
			long num3 = ptr.ToInt64();
			bool flag = num3 < num || num3 + (long)countBytes >= num2;
			if (flag)
			{
				throw new InvalidOperationException();
			}
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00003CAC File Offset: 0x00001EAC
		public override byte[] ReadResource(long offset)
		{
			ImageHelper imageHelper = new ImageHelper(this._file.BaseAddress, this._file.Length);
			IntPtr intPtr = new IntPtr(imageHelper.GetResourcesSectionStart().ToInt64() + offset);
			uint num = (uint)Marshal.ReadInt32(intPtr);
			intPtr = new IntPtr(intPtr.ToInt64() + (long)Marshal.SizeOf(num));
			byte[] array = new byte[num];
			Marshal.Copy(intPtr, array, 0, array.Length);
			return array;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00003D30 File Offset: 0x00001F30
		public override Token ReadEntryPointToken()
		{
			ImageHelper imageHelper = new ImageHelper(this._file.BaseAddress, this._file.Length);
			return imageHelper.GetEntryPointToken();
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00003D64 File Offset: 0x00001F64
		public override T ReadRvaStruct<T>(long rva)
		{
			IntPtr ptr = this.ResolveRva(rva);
			return (T)((object)Marshal.PtrToStructure(ptr, typeof(T)));
		}

		// Token: 0x04000048 RID: 72
		private readonly FileMapping _file;
	}
}
