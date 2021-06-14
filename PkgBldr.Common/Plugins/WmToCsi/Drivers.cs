using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000009 RID: 9
	[Export(typeof(IPkgPlugin))]
	internal class Drivers : PkgPlugin
	{
		// Token: 0x06000016 RID: 22 RVA: 0x000022F4 File Offset: 0x000004F4
		public override void ConvertEntries(XElement toCsi, Dictionary<string, IPkgPlugin> plugins, Config environ, XElement fromWm)
		{
			if (environ.build.wow == Build.WowType.guest)
			{
				return;
			}
			if (environ.build.satellite.Type != SatelliteType.Neutral)
			{
				return;
			}
			if (environ.Convert.Equals(ConversionType.pkg2csi))
			{
				environ.Logger.LogInfo("Cannot convert drivers directly from pkg.xml to csi.", new object[0]);
				return;
			}
			base.ConvertEntries(toCsi, plugins, environ, fromWm);
		}
	}
}
