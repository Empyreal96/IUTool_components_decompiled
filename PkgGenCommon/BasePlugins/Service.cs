using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BasePlugins
{
	// Token: 0x0200002E RID: 46
	[Export(typeof(IPkgPlugin))]
	public class Service : OSComponent
	{
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000BB RID: 187 RVA: 0x0000438A File Offset: 0x0000258A
		public override bool UseSecurityCompilerPassthrough
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000BC RID: 188 RVA: 0x0000438D File Offset: 0x0000258D
		public override string XmlSchemaPath
		{
			get
			{
				return PkgPlugin.BaseComponentSchemaPath;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000BD RID: 189 RVA: 0x00004394 File Offset: 0x00002594
		public override string XmlElementUniqueXPath
		{
			get
			{
				return "@Name";
			}
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00004B28 File Offset: 0x00002D28
		protected override void ValidateEntry(IPkgProject packageGenerator, XElement componentEntry)
		{
			IEnumerable<XElement> source = componentEntry.LocalElements("Executable") ?? new XElement[0];
			IEnumerable<XElement> source2 = componentEntry.LocalElements("ServiceDll") ?? new XElement[0];
			string text = (string)componentEntry.LocalAttribute("Name");
			if (source.Count<XElement>() + source2.Count<XElement>() == 0)
			{
				throw new PkgXmlException(componentEntry, "Service '{0}': Executable or ServiceDll is missing.", new object[]
				{
					text
				});
			}
			if (source.Count<XElement>() + source2.Count<XElement>() > 1)
			{
				throw new PkgXmlException(componentEntry, "Service '{0}': Can only specify one entry file type. Excutable or ServiceDll.", new object[]
				{
					text
				});
			}
			if (source2.Count<XElement>() > 0 && componentEntry.LocalAttribute("SvcHostGroupName") == null)
			{
				throw new PkgXmlException(componentEntry, "Service '{0}': SvcHostGroupName is required when when ServiceDll is declared.", new object[]
				{
					text
				});
			}
			if (source.Count<XElement>() > 0 && componentEntry.LocalAttribute("SvcHostGroupName") != null)
			{
				throw new PkgXmlException(componentEntry, "Service '{0}': SvcHostGroupName can only be set when ServiceDll is declared.", new object[]
				{
					text
				});
			}
			if (componentEntry.LocalElements("FailureActions").Count<XElement>() != 1)
			{
				throw new PkgXmlException(componentEntry, "'FailureActions' element is required for 'Service' object.", new object[0]);
			}
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00004C3C File Offset: 0x00002E3C
		protected override IEnumerable<PkgObject> ProcessEntry(IPkgProject packageGenerator, XElement componentEntry)
		{
			ServiceBuilder builder = new ServiceBuilder(componentEntry.LocalAttribute("Name").Value);
			componentEntry.WithLocalAttribute("DisplayName", delegate(XAttribute x)
			{
				builder.SetDisplayName(x.Value);
			});
			componentEntry.WithLocalAttribute("Description", delegate(XAttribute x)
			{
				builder.SetDescription(x.Value);
			});
			componentEntry.WithLocalAttribute("Group", delegate(XAttribute x)
			{
				builder.SetGroup(x.Value);
			});
			componentEntry.WithLocalAttribute("DependOnGroup", delegate(XAttribute x)
			{
				builder.SetDependOnGroup(x.Value);
			});
			componentEntry.WithLocalAttribute("DependOnService", delegate(XAttribute x)
			{
				builder.SetDependOnService(x.Value);
			});
			componentEntry.WithLocalAttribute("Start", delegate(XAttribute x)
			{
				builder.SetStartMode(x.Value);
			});
			componentEntry.WithLocalAttribute("Type", delegate(XAttribute x)
			{
				builder.SetType(x.Value);
			});
			componentEntry.WithLocalAttribute("ErrorControl", delegate(XAttribute x)
			{
				builder.SetErrorControl(x.Value);
			});
			XElement xelement = componentEntry.LocalElement("FailureActions");
			builder.FailureActions.SetResetPeriod((string)xelement.LocalAttribute("ResetPeriod"));
			xelement.WithLocalAttribute("Command", delegate(XAttribute x)
			{
				builder.FailureActions.SetCommand((string)x);
			});
			xelement.WithLocalAttribute("RebootMessage", delegate(XAttribute x)
			{
				builder.FailureActions.SetRebootMessage((string)x);
			});
			foreach (XElement element in xelement.Elements())
			{
				builder.FailureActions.AddFailureAction(element);
			}
			if (componentEntry.LocalElement("Executable") != null)
			{
				builder.AddExecutable(componentEntry.LocalElement("Executable"));
			}
			else
			{
				builder.SetSvcHostGroupName(componentEntry.LocalAttribute("SvcHostGroupName").Value);
				builder.AddServiceDll(componentEntry.LocalElement("ServiceDll"));
			}
			base.ProcessFiles<ServicePkgObject, ServiceBuilder>(componentEntry, builder);
			base.ProcessRegistry<ServicePkgObject, ServiceBuilder>(componentEntry, builder);
			return new List<PkgObject>
			{
				builder.ToPkgObject()
			};
		}
	}
}
