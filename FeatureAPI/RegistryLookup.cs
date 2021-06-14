using System;
using Microsoft.Win32;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000022 RID: 34
	public class RegistryLookup
	{
		// Token: 0x060000C6 RID: 198 RVA: 0x00007CF4 File Offset: 0x00005EF4
		public static string GetValue(string path, string key)
		{
			RegistryHive hKey = RegistryHive.LocalMachine;
			string result = string.Empty;
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(hKey, RegistryView.Registry32))
			{
				using (RegistryKey registryKey2 = registryKey.OpenSubKey(path, false))
				{
					if (registryKey2 != null)
					{
						result = (registryKey2.GetValue(key) as string);
					}
				}
			}
			return result;
		}
	}
}
