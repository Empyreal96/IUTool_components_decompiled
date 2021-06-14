using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x0200001B RID: 27
	[Export(typeof(IPkgPlugin))]
	internal class SettingsGroup : PkgPlugin
	{
		// Token: 0x06000052 RID: 82 RVA: 0x00005CE7 File Offset: 0x00003EE7
		public override void ConvertEntries(XElement toWindowsManifest, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement settingsGroup)
		{
			toWindowsManifest.Add(settingsGroup);
		}
	}
}
