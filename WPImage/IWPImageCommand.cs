using System;

namespace Microsoft.WindowsPhone.WPImage
{
	// Token: 0x02000002 RID: 2
	internal interface IWPImageCommand
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1
		string Name { get; }

		// Token: 0x06000002 RID: 2
		bool ParseArgs(string[] args);

		// Token: 0x06000003 RID: 3
		void PrintUsage();

		// Token: 0x06000004 RID: 4
		void Run();
	}
}
