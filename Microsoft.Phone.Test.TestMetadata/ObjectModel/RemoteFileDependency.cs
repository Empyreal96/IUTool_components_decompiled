using System;
using System.Xml.Serialization;

namespace Microsoft.Phone.Test.TestMetadata.ObjectModel
{
	// Token: 0x02000015 RID: 21
	[Serializable]
	public sealed class RemoteFileDependency : Dependency
	{
		// Token: 0x06000054 RID: 84 RVA: 0x000032A8 File Offset: 0x000014A8
		public RemoteFileDependency()
		{
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000033E9 File Offset: 0x000015E9
		public RemoteFileDependency(string fileShare, string source, string destinationPath, string destination)
		{
			this.SourcePath = fileShare;
			this.Source = source;
			this.DestinationPath = destinationPath;
			this.Destination = destination;
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000056 RID: 86 RVA: 0x00003414 File Offset: 0x00001614
		// (set) Token: 0x06000057 RID: 87 RVA: 0x0000341C File Offset: 0x0000161C
		[XmlAttribute]
		public string SourcePath { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000058 RID: 88 RVA: 0x00003425 File Offset: 0x00001625
		// (set) Token: 0x06000059 RID: 89 RVA: 0x0000342D File Offset: 0x0000162D
		[XmlAttribute]
		public string Source { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00003436 File Offset: 0x00001636
		// (set) Token: 0x0600005B RID: 91 RVA: 0x0000343E File Offset: 0x0000163E
		[XmlAttribute]
		public string DestinationPath { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00003447 File Offset: 0x00001647
		// (set) Token: 0x0600005D RID: 93 RVA: 0x0000344F File Offset: 0x0000164F
		[XmlAttribute]
		public string Destination { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00003458 File Offset: 0x00001658
		// (set) Token: 0x0600005F RID: 95 RVA: 0x00003460 File Offset: 0x00001660
		[XmlAttribute]
		public string Tags { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00003469 File Offset: 0x00001669
		// (set) Token: 0x06000061 RID: 97 RVA: 0x00003471 File Offset: 0x00001671
		[XmlAttribute]
		public bool IsWow { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000062 RID: 98 RVA: 0x0000347A File Offset: 0x0000167A
		// (set) Token: 0x06000063 RID: 99 RVA: 0x00003482 File Offset: 0x00001682
		[XmlAttribute]
		public string PackageArchitecture { get; set; }

		// Token: 0x06000064 RID: 100 RVA: 0x0000348C File Offset: 0x0000168C
		public override bool Equals(object obj)
		{
			RemoteFileDependency remoteFileDependency = obj as RemoteFileDependency;
			bool flag = remoteFileDependency == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = false;
				bool flag3 = !string.IsNullOrEmpty(this.Tags);
				if (flag3)
				{
					flag2 = this.Tags.Equals(remoteFileDependency.Tags, StringComparison.OrdinalIgnoreCase);
				}
				else
				{
					bool flag4 = string.IsNullOrEmpty(remoteFileDependency.Tags);
					if (flag4)
					{
						flag2 = true;
					}
				}
				bool flag5 = false;
				bool flag6 = !string.IsNullOrEmpty(this.PackageArchitecture);
				if (flag6)
				{
					flag5 = this.PackageArchitecture.Equals(remoteFileDependency.PackageArchitecture, StringComparison.OrdinalIgnoreCase);
				}
				else
				{
					bool flag7 = string.IsNullOrEmpty(remoteFileDependency.PackageArchitecture);
					if (flag7)
					{
						flag5 = true;
					}
				}
				result = (this.SourcePath.Equals(remoteFileDependency.SourcePath, StringComparison.OrdinalIgnoreCase) && this.Source.Equals(remoteFileDependency.Source, StringComparison.OrdinalIgnoreCase) && this.DestinationPath.Equals(remoteFileDependency.DestinationPath, StringComparison.OrdinalIgnoreCase) && this.Destination.Equals(remoteFileDependency.Destination, StringComparison.OrdinalIgnoreCase) && flag5 && this.IsWow == remoteFileDependency.IsWow && flag2);
			}
			return result;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000035A8 File Offset: 0x000017A8
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
