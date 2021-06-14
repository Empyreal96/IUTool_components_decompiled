using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BasePlugins
{
	// Token: 0x02000033 RID: 51
	[Export(typeof(IPkgPlugin))]
	public class WinRTHost : OSComponent
	{
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x0000438D File Offset: 0x0000258D
		public override string XmlSchemaPath
		{
			get
			{
				return PkgPlugin.BaseComponentSchemaPath;
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x00004EB8 File Offset: 0x000030B8
		public override string XmlElementUniqueXPath
		{
			get
			{
				return "@Id";
			}
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00004EC0 File Offset: 0x000030C0
		protected override void ValidateEntry(IPkgProject packageGenerator, XElement componentEntry)
		{
			if (componentEntry.LocalElements("Dll").Count<XElement>() != 1)
			{
				throw new PkgXmlException(componentEntry, "Only 1 Dll may be specified per WinRTHost object", new object[0]);
			}
			if (componentEntry.LocalElements("WinRTClass").Count<XElement>() <= 0)
			{
				throw new PkgXmlException(componentEntry, "At least 1 WinRTClass must be specified in a WinRTHost object", new object[0]);
			}
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00004F18 File Offset: 0x00003118
		protected override IEnumerable<PkgObject> ProcessEntry(IPkgProject packageGenerator, XElement componentEntry)
		{
			OSComponentBuilder oscomponentBuilder = new OSComponentBuilder();
			XElement source = componentEntry.LocalElement("Dll");
			string value = source.LocalAttribute("Source").Value;
			string dllDestinationDir = "$(runtime.default)";
			string dllDestinationName = Path.GetFileName(value);
			source.WithLocalAttribute("DestinationDir", delegate(XAttribute x)
			{
				dllDestinationDir = x.Value;
			});
			source.WithLocalAttribute("Name", delegate(XAttribute x)
			{
				dllDestinationName = x.Value;
			});
			string value2 = Path.Combine(dllDestinationDir, dllDestinationName);
			FileBuilder dllFile = oscomponentBuilder.AddFileGroup().AddFile(value, dllDestinationDir).SetName(dllDestinationName);
			source.WithLocalAttribute("Attributes", delegate(XAttribute x)
			{
				dllFile.SetAttributes(x.Value);
			});
			foreach (XElement source2 in componentEntry.LocalElements("WinRTClass"))
			{
				string value3 = source2.LocalAttribute("Id").Value;
				string value4 = source2.LocalAttribute("ActivatableId").Value;
				ModernComActivation modernComActivation = (ModernComActivation)Enum.Parse(typeof(ModernComActivation), source2.LocalAttribute("ActivationType").Value);
				ModernComTrustLevel modernComTrustLevel = (ModernComTrustLevel)Enum.Parse(typeof(ModernComTrustLevel), source2.LocalAttribute("TrustLevel").Value);
				ModernComThreading modernComThreading = (ModernComThreading)Enum.Parse(typeof(ModernComThreading), source2.LocalAttribute("ThreadingModel").Value);
				RegistryKeyGroupBuilder registryKeyGroupBuilder = oscomponentBuilder.AddRegistryGroup();
				RegistryKeyBuilder registryKeyBuilder = registryKeyGroupBuilder.AddRegistryKey("$(hklm.software)\\Microsoft\\WindowsRuntime\\ActivatableClassId\\{0}", new object[]
				{
					value4
				}).AddValue("DllPath", "REG_SZ", value2).AddValue("CLSID", "REG_SZ", value3);
				string name = "ActivationType";
				string type = "REG_DWORD";
				int num = (int)modernComActivation;
				RegistryKeyBuilder registryKeyBuilder2 = registryKeyBuilder.AddValue(name, type, num.ToString("X"));
				string name2 = "Threading";
				string type2 = "REG_DWORD";
				num = (int)modernComThreading;
				RegistryKeyBuilder registryKeyBuilder3 = registryKeyBuilder2.AddValue(name2, type2, num.ToString("X"));
				string name3 = "TrustLevel";
				string type3 = "REG_DWORD";
				num = (int)modernComTrustLevel;
				registryKeyBuilder3.AddValue(name3, type3, num.ToString("X"));
				registryKeyGroupBuilder.AddRegistryKey("$(hklm.software)\\Microsoft\\WindowsRuntime\\CLSID\\{0}", new object[]
				{
					value3
				}).AddValue("ActivatableClassId", "REG_SZ", value4);
			}
			base.ProcessFiles<OSComponentPkgObject, OSComponentBuilder>(componentEntry, oscomponentBuilder);
			base.ProcessRegistry<OSComponentPkgObject, OSComponentBuilder>(componentEntry, oscomponentBuilder);
			return new List<PkgObject>
			{
				oscomponentBuilder.ToPkgObject()
			};
		}
	}
}
