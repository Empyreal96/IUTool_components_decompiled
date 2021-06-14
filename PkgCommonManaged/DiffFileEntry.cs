using System;
using System.IO;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000003 RID: 3
	public sealed class DiffFileEntry : FileEntryBase, IDiffEntry
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002167 File Offset: 0x00000367
		// (set) Token: 0x06000007 RID: 7 RVA: 0x0000216F File Offset: 0x0000036F
		[XmlElement]
		public DiffType DiffType { get; set; }

		// Token: 0x06000008 RID: 8 RVA: 0x00002178 File Offset: 0x00000378
		public DiffFileEntry()
		{
			this.DiffType = DiffType.Invalid;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002187 File Offset: 0x00000387
		public DiffFileEntry(IntPtr objPtr) : base(objPtr)
		{
			this.DiffType = NativeMethods.DiffFileEntry_Get_DiffType(objPtr);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000219C File Offset: 0x0000039C
		public DiffFileEntry(FileType ft, DiffType diffType, string destination, string source)
		{
			if (diffType == DiffType.Invalid || ft == FileType.Invalid)
			{
				throw new PackageException("Invalid type should not be used for non-default constructor");
			}
			if (diffType != DiffType.Remove)
			{
				if (string.IsNullOrEmpty(source))
				{
					throw new PackageException("No source path specified for in non-default constructor with diff types other than Remove");
				}
				if (!File.Exists(source))
				{
					throw new PackageException("Source file '{0}' doesn't exist", new object[]
					{
						source
					});
				}
				base.SourcePath = source;
			}
			base.FileType = ft;
			this.DiffType = diffType;
			base.DevicePath = destination;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002214 File Offset: 0x00000414
		public new void Validate()
		{
			if (this.DiffType == DiffType.Invalid)
			{
				throw new PackageException("Invalid DiffType");
			}
			base.Validate();
		}
	}
}
