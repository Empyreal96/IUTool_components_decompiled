using System;

namespace Microsoft.WindowsPhone.Imaging.WimInterop
{
	// Token: 0x02000002 RID: 2
	public interface IImage
	{
		// Token: 0x06000001 RID: 1
		void Apply(string pathToMountTo);

		// Token: 0x06000002 RID: 2
		void Mount(string pathToMountTo, bool isReadOnly);

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000003 RID: 3
		string MountedPath { get; }

		// Token: 0x06000004 RID: 4
		void DismountImage();

		// Token: 0x06000005 RID: 5
		void DismountImage(bool saveChanges);
	}
}
