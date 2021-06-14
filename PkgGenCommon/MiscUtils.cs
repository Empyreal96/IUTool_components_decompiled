using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon
{
	// Token: 0x02000010 RID: 16
	public static class MiscUtils
	{
		// Token: 0x06000041 RID: 65 RVA: 0x000026D4 File Offset: 0x000008D4
		public static void RegisterObjectSpecificMacros(IMacroResolver macroResolver, ObjectType type, params KeyValuePair<string, string>[] attributes)
		{
			Dictionary<string, string> dictionary = attributes.ToDictionary((KeyValuePair<string, string> x) => x.Key, (KeyValuePair<string, string> x) => x.Value, StringComparer.OrdinalIgnoreCase);
			try
			{
				switch (type)
				{
				case ObjectType.OSComponent:
				case ObjectType.ComObj:
					macroResolver.Register("runtime.default", "$(runtime.system32)");
					macroResolver.Register("env.default", "$(env.system32)");
					break;
				case ObjectType.Application:
				case ObjectType.AppResource:
					macroResolver.Register("runtime.default", "$(runtime.apps)\\" + dictionary["Name"]);
					macroResolver.Register("env.default", "$(env.apps)\\" + dictionary["Name"]);
					break;
				case ObjectType.ComClass:
					macroResolver.Register("hkcr.clsid", "$(hkcr.root)\\CLSID\\" + dictionary["ID"]);
					break;
				case ObjectType.ComInterface:
					macroResolver.Register("hkcr.iid", "$(hkcr.root)\\Interface\\" + dictionary["ID"]);
					break;
				case ObjectType.Driver:
					macroResolver.Register("runtime.default", "$(runtime.drivers)");
					macroResolver.Register("env.default", "$(env.drivers)");
					break;
				case ObjectType.BinaryPartition:
				case ObjectType.BCDStore:
					break;
				case ObjectType.Service:
					macroResolver.Register("runtime.default", "$(runtime.system32)");
					macroResolver.Register("env.default", "$(env.system32)");
					macroResolver.Register("hklm.service", "$(hklm.system)\\controlset001\\services\\" + dictionary["Name"]);
					break;
				case ObjectType.SvcHostGroup:
					macroResolver.Register("hklm.svchostgroup", "$(hklm.svchost)\\" + dictionary["Name"]);
					break;
				case ObjectType.FullTrust:
					macroResolver.Register("runtime.default", "$(runtime.system32)");
					break;
				default:
					throw new PkgGenException("Unknown object type '{0}'", new object[]
					{
						type
					});
				}
			}
			catch (KeyNotFoundException innerException)
			{
				throw new PkgGenException(innerException, "A required attribute for '{0}' object is missing to register object specific macros", new object[]
				{
					type
				});
			}
		}
	}
}
