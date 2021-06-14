using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x0200001E RID: 30
	[XmlRoot(ElementName = "Required")]
	[Serializable]
	public class ResolvedDependency
	{
		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000148 RID: 328 RVA: 0x00008C82 File Offset: 0x00006E82
		// (set) Token: 0x06000149 RID: 329 RVA: 0x00008C8A File Offset: 0x00006E8A
		[XmlIgnore]
		public HashSet<Dependency> Dependencies { get; set; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x0600014A RID: 330 RVA: 0x00008C94 File Offset: 0x00006E94
		[XmlElement("Binary")]
		public List<BinaryDependency> BinaryDependency
		{
			get
			{
				return this.Dependencies.OfType<BinaryDependency>().ToList<BinaryDependency>();
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600014B RID: 331 RVA: 0x00008CB8 File Offset: 0x00006EB8
		[XmlElement("Package")]
		public List<PackageDependency> PackageDependency
		{
			get
			{
				return this.Dependencies.OfType<PackageDependency>().ToList<PackageDependency>();
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600014C RID: 332 RVA: 0x00008CDC File Offset: 0x00006EDC
		[XmlElement("RemoteFile")]
		public List<RemoteFileDependency> RemoteFileDependency
		{
			get
			{
				return this.Dependencies.OfType<RemoteFileDependency>().ToList<RemoteFileDependency>();
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600014D RID: 333 RVA: 0x00008D00 File Offset: 0x00006F00
		[XmlElement("EnvironmentPath")]
		public List<EnvironmentPathDependency> EnvironmentPaths
		{
			get
			{
				return this.Dependencies.OfType<EnvironmentPathDependency>().ToList<EnvironmentPathDependency>();
			}
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00008D24 File Offset: 0x00006F24
		internal static void Save(string fileName, HashSet<Dependency> dependencies)
		{
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true
			};
			ResolvedDependency o = new ResolvedDependency
			{
				Dependencies = dependencies
			};
			using (XmlWriter xmlWriter = XmlWriter.Create(fileName, settings))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(ResolvedDependency));
				xmlSerializer.Serialize(xmlWriter, o);
			}
		}
	}
}
