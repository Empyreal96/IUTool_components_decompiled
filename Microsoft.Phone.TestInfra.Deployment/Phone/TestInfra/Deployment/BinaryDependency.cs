using System;
using System.Xml.Serialization;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000034 RID: 52
	public class BinaryDependency : Dependency
	{
		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000242 RID: 578 RVA: 0x0000E8A9 File Offset: 0x0000CAA9
		// (set) Token: 0x06000243 RID: 579 RVA: 0x0000E8B1 File Offset: 0x0000CAB1
		[XmlAttribute(AttributeName = "Name")]
		public string FileName { get; set; }

		// Token: 0x06000244 RID: 580 RVA: 0x0000E8BC File Offset: 0x0000CABC
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
				bool flag2 = obj.GetType() != base.GetType();
				result = (!flag2 && this.Equals(obj as BinaryDependency));
			}
			return result;
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0000E904 File Offset: 0x0000CB04
		public bool Equals(BinaryDependency other)
		{
			return string.Compare(this.FileName, other.FileName, true) == 0;
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000E92C File Offset: 0x0000CB2C
		public override int GetHashCode()
		{
			return this.FileName.GetHashCode();
		}
	}
}
