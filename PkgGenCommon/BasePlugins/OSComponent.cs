using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BasePlugins
{
	// Token: 0x0200002D RID: 45
	[Export(typeof(IPkgPlugin))]
	public class OSComponent : PkgPlugin
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x0000438A File Offset: 0x0000258A
		public override bool UseSecurityCompilerPassthrough
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x0000438D File Offset: 0x0000258D
		public override string XmlSchemaPath
		{
			get
			{
				return PkgPlugin.BaseComponentSchemaPath;
			}
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00004469 File Offset: 0x00002669
		protected override void ValidateEntry(IPkgProject packageGenerator, XElement componentEntry)
		{
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00004824 File Offset: 0x00002A24
		protected override IEnumerable<PkgObject> ProcessEntry(IPkgProject packageGenerator, XElement componentEntry)
		{
			OSComponentBuilder oscomponentBuilder = new OSComponentBuilder();
			this.ProcessFiles<OSComponentPkgObject, OSComponentBuilder>(componentEntry, oscomponentBuilder);
			this.ProcessRegistry<OSComponentPkgObject, OSComponentBuilder>(componentEntry, oscomponentBuilder);
			return new List<PkgObject>
			{
				oscomponentBuilder.ToPkgObject()
			};
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00004858 File Offset: 0x00002A58
		protected void ProcessFiles<T, V>(XElement componentEntry, V builder) where T : OSComponentPkgObject, new() where V : OSComponentBuilder<T, V>
		{
			foreach (XElement xelement in componentEntry.LocalElements("Files"))
			{
				FileGroupBuilder groupBuilder = builder.AddFileGroup();
				xelement.WithLocalAttribute("Language", delegate(XAttribute x)
				{
					groupBuilder.SetLanguage(x.Value);
				});
				xelement.WithLocalAttribute("Resolution", delegate(XAttribute x)
				{
					groupBuilder.SetResolution(x.Value);
				});
				xelement.WithLocalAttribute("CpuFilter", delegate(XAttribute x)
				{
					groupBuilder.SetCpuId(x.Value);
				});
				foreach (XElement fileElement in xelement.Elements())
				{
					groupBuilder.AddFile(fileElement);
				}
			}
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00004948 File Offset: 0x00002B48
		protected void ProcessRegistry<T, V>(XElement componentEntry, V builder) where T : OSComponentPkgObject, new() where V : OSComponentBuilder<T, V>
		{
			foreach (XElement xelement in componentEntry.LocalDescendants("RegImport"))
			{
				builder.AddRegistryImport(xelement.Attribute("Source").Value);
			}
			foreach (XElement xelement2 in componentEntry.LocalElements("RegKeys"))
			{
				RegistryKeyGroupBuilder groupBuilder = builder.AddRegistryGroup();
				xelement2.WithLocalAttribute("Language", delegate(XAttribute x)
				{
					groupBuilder.SetLanguage(x.Value);
				});
				xelement2.WithLocalAttribute("Resolution", delegate(XAttribute x)
				{
					groupBuilder.SetResolution(x.Value);
				});
				xelement2.WithLocalAttribute("CpuFilter", delegate(XAttribute x)
				{
					groupBuilder.SetCpuId(x.Value);
				});
				foreach (XElement xelement3 in xelement2.Elements())
				{
					RegistryKeyBuilder registryKeyBuilder = groupBuilder.AddRegistryKey(xelement3.Attribute("KeyName").Value);
					foreach (XElement valueElement in xelement3.LocalElements("RegValue"))
					{
						registryKeyBuilder.AddValue(valueElement);
					}
				}
			}
		}
	}
}
