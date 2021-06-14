using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Reflection.Adds
{
	// Token: 0x02000014 RID: 20
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class MetadataDispenser
	{
		// Token: 0x0600008B RID: 139 RVA: 0x0000368C File Offset: 0x0000188C
		private static MetadataDispenser.IMetaDataDispenserEx GetDispenserShim()
		{
			return (MetadataDispenser.MetaDataDispenserEx)new MetadataDispenser.CorMetaDataDispenserExClass();
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000036A8 File Offset: 0x000018A8
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		[SuppressMessage("Microsoft.Reliability", "CA2004:RemoveCallsToGCKeepAlive")]
		public MetadataFile OpenFromByteArray(byte[] data)
		{
			data = (byte[])data.Clone();
			MetadataDispenser.IMetaDataDispenserEx dispenserShim = MetadataDispenser.GetDispenserShim();
			Guid guid = typeof(MetadataDispenser.IMetadataImportDummy).GUID;
			IntPtr zero = IntPtr.Zero;
			GCHandle gchandle = default(GCHandle);
			MetadataFile result;
			try
			{
				gchandle = GCHandle.Alloc(data, GCHandleType.Pinned);
				IntPtr pData = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
				uint cbData = (uint)data.Length;
				int errorCode = dispenserShim.OpenScopeOnMemory(pData, cbData, MetadataDispenser.CorOpenFlags.ReadOnly, ref guid, out zero);
				Marshal.ThrowExceptionForHR(errorCode);
				GC.KeepAlive(dispenserShim);
				Marshal.FinalReleaseComObject(dispenserShim);
				MetadataDispenser.MetadataFileOnByteArray metadataFileOnByteArray = new MetadataDispenser.MetadataFileOnByteArray(ref gchandle, zero);
				result = metadataFileOnByteArray;
			}
			finally
			{
				bool isAllocated = gchandle.IsAllocated;
				if (isAllocated)
				{
					gchandle.Free();
				}
				bool flag = zero != IntPtr.Zero;
				if (flag)
				{
					Marshal.Release(zero);
				}
			}
			return result;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00003780 File Offset: 0x00001980
		[SuppressMessage("Microsoft.Reliability", "CA2004:RemoveCallsToGCKeepAlive")]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public MetadataFile OpenFileAsFileMapping(string fileName)
		{
			FileMapping fileMapping = new FileMapping(fileName);
			MetadataDispenser.IMetaDataDispenserEx dispenserShim = MetadataDispenser.GetDispenserShim();
			Guid guid = typeof(MetadataDispenser.IMetadataImportDummy).GUID;
			IntPtr zero = IntPtr.Zero;
			MetadataFile result;
			try
			{
				IntPtr baseAddress = fileMapping.BaseAddress;
				uint cbData = (uint)fileMapping.Length;
				int errorCode = dispenserShim.OpenScopeOnMemory(baseAddress, cbData, MetadataDispenser.CorOpenFlags.ReadOnly, ref guid, out zero);
				Marshal.ThrowExceptionForHR(errorCode);
				GC.KeepAlive(dispenserShim);
				Marshal.FinalReleaseComObject(dispenserShim);
				result = new MetadataFileAndRvaResolver(zero, fileMapping);
			}
			finally
			{
				bool flag = zero != IntPtr.Zero;
				if (flag)
				{
					Marshal.Release(zero);
				}
			}
			return result;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00003828 File Offset: 0x00001A28
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public MetadataFile OpenFileAsFileMapping(object importer, string fileName)
		{
			bool flag = importer == null;
			if (flag)
			{
				throw new ArgumentNullException("importer");
			}
			MetadataDispenser.IMetaDataInfo metaDataInfo = importer as MetadataDispenser.IMetaDataInfo;
			bool flag2 = metaDataInfo != null;
			if (flag2)
			{
				IntPtr baseAddress;
				long fileLength;
				MetadataDispenser.CorFileMapping corFileMapping;
				bool flag3 = metaDataInfo.GetFileMapping(out baseAddress, out fileLength, out corFileMapping) == 0 && corFileMapping == MetadataDispenser.CorFileMapping.Flat;
				if (flag3)
				{
					FileMapping file = new FileMapping(baseAddress, fileLength, fileName);
					return new MetadataFileAndRvaResolver(importer, file);
				}
			}
			return this.OpenFileAsFileMapping(fileName);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000038A4 File Offset: 0x00001AA4
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		[SuppressMessage("Microsoft.Reliability", "CA2004:RemoveCallsToGCKeepAlive")]
		public MetadataFile OpenFile(string fileName)
		{
			MetadataDispenser.IMetaDataDispenserEx dispenserShim = MetadataDispenser.GetDispenserShim();
			Guid guid = typeof(MetadataDispenser.IMetadataImportDummy).GUID;
			IntPtr zero = IntPtr.Zero;
			MetadataFile result;
			try
			{
				int errorCode = dispenserShim.OpenScope(fileName, MetadataDispenser.CorOpenFlags.ReadOnly, ref guid, out zero);
				Marshal.ThrowExceptionForHR(errorCode);
				GC.KeepAlive(dispenserShim);
				Marshal.FinalReleaseComObject(dispenserShim);
				result = new MetadataFile(zero);
			}
			finally
			{
				bool flag = zero != IntPtr.Zero;
				if (flag)
				{
					Marshal.Release(zero);
				}
			}
			return result;
		}

		// Token: 0x02000032 RID: 50
		private class MetadataFileOnByteArray : MetadataFile
		{
			// Token: 0x0600012B RID: 299 RVA: 0x000054DB File Offset: 0x000036DB
			public MetadataFileOnByteArray(ref GCHandle h, IntPtr pUnk) : base(pUnk)
			{
				this._handle = h;
				h = default(GCHandle);
			}

			// Token: 0x0600012C RID: 300 RVA: 0x000054F9 File Offset: 0x000036F9
			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
				this._handle.Free();
			}

			// Token: 0x0400011E RID: 286
			private GCHandle _handle;
		}

		// Token: 0x02000033 RID: 51
		private enum CorOpenFlags : uint
		{
			// Token: 0x04000120 RID: 288
			Read,
			// Token: 0x04000121 RID: 289
			Write,
			// Token: 0x04000122 RID: 290
			ReadWriteMask = 1U,
			// Token: 0x04000123 RID: 291
			CopyMemory,
			// Token: 0x04000124 RID: 292
			ManifestMetadata = 8U,
			// Token: 0x04000125 RID: 293
			ReadOnly = 16U,
			// Token: 0x04000126 RID: 294
			TakeOwnership = 32U,
			// Token: 0x04000127 RID: 295
			CacheImage = 4U,
			// Token: 0x04000128 RID: 296
			NoTypeLib = 128U
		}

		// Token: 0x02000034 RID: 52
		internal enum CorFileMapping : uint
		{
			// Token: 0x0400012A RID: 298
			Flat,
			// Token: 0x0400012B RID: 299
			ExecutableImage
		}

		// Token: 0x02000035 RID: 53
		[Guid("7DAC8207-D3AE-4c75-9B67-92801A497D44")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IMetadataImportDummy
		{
		}

		// Token: 0x02000036 RID: 54
		[Guid("31BCFCE2-DAFB-11D2-9F81-00C04F79A0A3")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[ComImport]
		private interface IMetaDataDispenserEx
		{
			// Token: 0x0600012D RID: 301
			int DefineScope(ref Guid rclsid, uint dwCreateFlags, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppIUnk);

			// Token: 0x0600012E RID: 302
			[PreserveSig]
			int OpenScope([MarshalAs(UnmanagedType.LPWStr)] string szScope, MetadataDispenser.CorOpenFlags dwOpenFlags, ref Guid riid, out IntPtr ppIUnk);

			// Token: 0x0600012F RID: 303
			[PreserveSig]
			int OpenScopeOnMemory(IntPtr pData, uint cbData, MetadataDispenser.CorOpenFlags dwOpenFlags, ref Guid riid, out IntPtr ppIUnk);

			// Token: 0x06000130 RID: 304
			int SetOption(ref Guid optionid, [MarshalAs(UnmanagedType.Struct)] object value);

			// Token: 0x06000131 RID: 305
			[PreserveSig]
			int GetOption(ref Guid optionid, [MarshalAs(UnmanagedType.Struct)] out object pvalue);

			// Token: 0x06000132 RID: 306
			int _OpenScopeOnITypeInfo();

			// Token: 0x06000133 RID: 307
			int GetCORSystemDirectory([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] char[] szBuffer, uint cchBuffer, out uint pchBuffer);

			// Token: 0x06000134 RID: 308
			int FindAssembly([MarshalAs(UnmanagedType.LPWStr)] string szAppBase, [MarshalAs(UnmanagedType.LPWStr)] string szPrivateBin, [MarshalAs(UnmanagedType.LPWStr)] string szGlobalBin, [MarshalAs(UnmanagedType.LPWStr)] string szAssemblyName, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] char[] szName, uint cchName, out uint pcName);

			// Token: 0x06000135 RID: 309
			int FindAssemblyModule([MarshalAs(UnmanagedType.LPWStr)] string szAppBase, [MarshalAs(UnmanagedType.LPWStr)] string szPrivateBin, [MarshalAs(UnmanagedType.LPWStr)] string szGlobalBin, [MarshalAs(UnmanagedType.LPWStr)] string szAssemblyName, [MarshalAs(UnmanagedType.LPWStr)] string szModuleName, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] char[] szName, uint cchName, out uint pcName);
		}

		// Token: 0x02000037 RID: 55
		[Guid("E5CB7A31-7512-11D2-89CE-0080C792E5D8")]
		[ComImport]
		private class CorMetaDataDispenserExClass
		{
			// Token: 0x06000136 RID: 310
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern CorMetaDataDispenserExClass();
		}

		// Token: 0x02000038 RID: 56
		[Guid("31BCFCE2-DAFB-11D2-9F81-00C04F79A0A3")]
		[CoClass(typeof(MetadataDispenser.CorMetaDataDispenserExClass))]
		[ComImport]
		private interface MetaDataDispenserEx : MetadataDispenser.IMetaDataDispenserEx
		{
		}

		// Token: 0x02000039 RID: 57
		[Guid("7998EA64-7F95-48B8-86FC-17CAF48BF5CB")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[ComImport]
		internal interface IMetaDataInfo
		{
			// Token: 0x06000137 RID: 311
			[PreserveSig]
			int GetFileMapping(out IntPtr ppvData, out long pcbData, out MetadataDispenser.CorFileMapping pdwMappingType);
		}
	}
}
