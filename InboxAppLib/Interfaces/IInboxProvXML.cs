using System;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces
{
	// Token: 0x02000049 RID: 73
	public interface IInboxProvXML
	{
		// Token: 0x060000FA RID: 250
		void ReadProvXML();

		// Token: 0x060000FB RID: 251
		void Save(string outputBasePath);

		// Token: 0x060000FC RID: 252
		void Update(string installDestinationPath, string licenseFileDestinationPath);

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000FD RID: 253
		string ProvXMLDestinationPath { get; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000FE RID: 254
		string UpdateProvXMLDestinationPath { get; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000FF RID: 255
		string LicenseDestinationPath { get; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000100 RID: 256
		ProvXMLCategory Category { get; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000101 RID: 257
		// (set) Token: 0x06000102 RID: 258
		string DependencyHash { get; set; }
	}
}
