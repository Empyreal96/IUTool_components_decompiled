using System;
using System.Xml.Serialization;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000037 RID: 55
	public class EnvironmentPathDependency : Dependency
	{
		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000260 RID: 608 RVA: 0x0000EBCE File Offset: 0x0000CDCE
		// (set) Token: 0x06000261 RID: 609 RVA: 0x0000EBD6 File Offset: 0x0000CDD6
		[XmlAttribute(AttributeName = "Name")]
		public string EnvironmentPath { get; set; }

		// Token: 0x06000262 RID: 610 RVA: 0x0000EBE0 File Offset: 0x0000CDE0
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
				result = (!flag2 && this.Equals(obj as EnvironmentPathDependency));
			}
			return result;
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000EC28 File Offset: 0x0000CE28
		public bool Equals(EnvironmentPathDependency other)
		{
			return string.Compare(this.EnvironmentPath, other.EnvironmentPath, true) == 0;
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000EC50 File Offset: 0x0000CE50
		public override int GetHashCode()
		{
			return this.EnvironmentPath.ToLowerInvariant().GetHashCode();
		}
	}
}
