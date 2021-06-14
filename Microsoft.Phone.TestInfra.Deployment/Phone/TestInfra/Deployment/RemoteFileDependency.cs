using System;
using System.Xml.Serialization;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000035 RID: 53
	public class RemoteFileDependency : Dependency
	{
		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000248 RID: 584 RVA: 0x0000E952 File Offset: 0x0000CB52
		// (set) Token: 0x06000249 RID: 585 RVA: 0x0000E95A File Offset: 0x0000CB5A
		[XmlAttribute]
		public string SourcePath { get; set; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x0600024A RID: 586 RVA: 0x0000E963 File Offset: 0x0000CB63
		// (set) Token: 0x0600024B RID: 587 RVA: 0x0000E96B File Offset: 0x0000CB6B
		[XmlAttribute]
		public string Source { get; set; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x0600024C RID: 588 RVA: 0x0000E974 File Offset: 0x0000CB74
		// (set) Token: 0x0600024D RID: 589 RVA: 0x0000E97C File Offset: 0x0000CB7C
		[XmlAttribute]
		public string DestinationPath { get; set; }

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x0600024E RID: 590 RVA: 0x0000E985 File Offset: 0x0000CB85
		// (set) Token: 0x0600024F RID: 591 RVA: 0x0000E98D File Offset: 0x0000CB8D
		[XmlAttribute]
		public string Destination { get; set; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000250 RID: 592 RVA: 0x0000E996 File Offset: 0x0000CB96
		// (set) Token: 0x06000251 RID: 593 RVA: 0x0000E99E File Offset: 0x0000CB9E
		[XmlAttribute]
		public string Tags { get; set; }

		// Token: 0x06000252 RID: 594 RVA: 0x0000E9A8 File Offset: 0x0000CBA8
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
				result = (!flag2 && this.Equals(obj as RemoteFileDependency));
			}
			return result;
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000E9F0 File Offset: 0x0000CBF0
		public bool Equals(RemoteFileDependency other)
		{
			bool flag = false;
			bool flag2 = !string.IsNullOrEmpty(this.Tags);
			if (flag2)
			{
				flag = this.Tags.Equals(other.Tags, StringComparison.OrdinalIgnoreCase);
			}
			else
			{
				bool flag3 = string.IsNullOrEmpty(other.Tags);
				if (flag3)
				{
					flag = true;
				}
			}
			return this.SourcePath.Equals(other.SourcePath, StringComparison.OrdinalIgnoreCase) && this.Source.Equals(other.Source, StringComparison.OrdinalIgnoreCase) && this.DestinationPath.Equals(other.DestinationPath, StringComparison.OrdinalIgnoreCase) && this.Destination.Equals(other.Destination, StringComparison.OrdinalIgnoreCase) && flag;
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000EA98 File Offset: 0x0000CC98
		public override int GetHashCode()
		{
			int hashCode = string.Empty.GetHashCode();
			bool flag = !string.IsNullOrEmpty(this.Tags);
			if (flag)
			{
				hashCode = this.Tags.GetHashCode();
			}
			return this.SourcePath.GetHashCode() ^ this.Source.GetHashCode() ^ this.DestinationPath.GetHashCode() ^ this.Destination.GetHashCode() ^ hashCode;
		}
	}
}
