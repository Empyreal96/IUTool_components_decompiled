using System;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x0200001D RID: 29
	public class PkgDepResolve : IEquatable<PkgDepResolve>
	{
		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000140 RID: 320 RVA: 0x00008BC2 File Offset: 0x00006DC2
		// (set) Token: 0x06000141 RID: 321 RVA: 0x00008BCA File Offset: 0x00006DCA
		public PackageInfo PkgInfo { get; set; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000142 RID: 322 RVA: 0x00008BD3 File Offset: 0x00006DD3
		// (set) Token: 0x06000143 RID: 323 RVA: 0x00008BDB File Offset: 0x00006DDB
		public bool IsProcessed { get; set; }

		// Token: 0x06000144 RID: 324 RVA: 0x00008BE4 File Offset: 0x00006DE4
		public new bool Equals(object obj)
		{
			bool flag = obj == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = obj.GetType() != base.GetType();
				result = (!flag2 && this.Equals(obj as PkgDepResolve));
			}
			return result;
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00008C2C File Offset: 0x00006E2C
		public bool Equals(PkgDepResolve other)
		{
			return string.Compare(this.PkgInfo.PackageName, other.PkgInfo.PackageName, true) == 0;
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00008C60 File Offset: 0x00006E60
		public override int GetHashCode()
		{
			return this.PkgInfo.PackageName.GetHashCode();
		}
	}
}
