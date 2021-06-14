using System;
using System.Xml.Serialization;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000033 RID: 51
	[XmlInclude(typeof(BinaryDependency))]
	[XmlInclude(typeof(PackageDependency))]
	[XmlInclude(typeof(EnvironmentPathDependency))]
	[XmlInclude(typeof(RemoteFileDependency))]
	public abstract class Dependency
	{
	}
}
