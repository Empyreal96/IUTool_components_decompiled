using System;
using System.IO;
using System.Threading;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x0200002A RID: 42
	public class PackageInfo
	{
		// Token: 0x060001CF RID: 463 RVA: 0x0000ACFC File Offset: 0x00008EFC
		public PackageInfo(string rootPath, string relativePath)
		{
			bool flag = string.IsNullOrEmpty(rootPath);
			if (flag)
			{
				throw new ArgumentNullException("rootPath");
			}
			bool flag2 = string.IsNullOrEmpty(relativePath);
			if (flag2)
			{
				throw new ArgumentNullException("relativePath");
			}
			this.count = 1;
			this.RootPath = PathHelper.EndWithDirectorySeparator(rootPath);
			this.RelativePath = relativePath.TrimStart(new char[]
			{
				Path.DirectorySeparatorChar
			});
			this.AbsolutePath = PathHelper.Combine(this.RootPath, this.RelativePath);
			this.PackageName = PathHelper.GetPackageNameWithoutExtension(this.AbsolutePath);
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060001D0 RID: 464 RVA: 0x0000AD96 File Offset: 0x00008F96
		// (set) Token: 0x060001D1 RID: 465 RVA: 0x0000AD9E File Offset: 0x00008F9E
		public string RootPath { get; private set; }

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060001D2 RID: 466 RVA: 0x0000ADA7 File Offset: 0x00008FA7
		// (set) Token: 0x060001D3 RID: 467 RVA: 0x0000ADAF File Offset: 0x00008FAF
		public string RelativePath { get; private set; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060001D4 RID: 468 RVA: 0x0000ADB8 File Offset: 0x00008FB8
		// (set) Token: 0x060001D5 RID: 469 RVA: 0x0000ADC0 File Offset: 0x00008FC0
		public string AbsolutePath { get; private set; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060001D6 RID: 470 RVA: 0x0000ADC9 File Offset: 0x00008FC9
		// (set) Token: 0x060001D7 RID: 471 RVA: 0x0000ADD1 File Offset: 0x00008FD1
		public string PackageName { get; private set; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x0000ADDC File Offset: 0x00008FDC
		// (set) Token: 0x060001D9 RID: 473 RVA: 0x0000ADF4 File Offset: 0x00008FF4
		public int Count
		{
			get
			{
				return this.count;
			}
			set
			{
				bool flag = value < 0;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value", value, "Count is negative");
				}
				Interlocked.Exchange(ref this.count, value);
			}
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000AE30 File Offset: 0x00009030
		public override int GetHashCode()
		{
			return (this.AbsolutePath != null) ? this.AbsolutePath.GetHashCode() : 0;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000AE58 File Offset: 0x00009058
		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this == obj;
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool flag3 = obj.GetType() != base.GetType();
					result = (!flag3 && this.Equals((PackageInfo)obj));
				}
			}
			return result;
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000AEAC File Offset: 0x000090AC
		protected bool Equals(PackageInfo other)
		{
			return string.Equals(this.AbsolutePath, other.AbsolutePath, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x040000C6 RID: 198
		private int count;
	}
}
