using System;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x02000038 RID: 56
	public class BinaryPartitionBuilder : PkgObjectBuilder<BinaryPartitionPkgObject, BinaryPartitionBuilder>
	{
		// Token: 0x060000D5 RID: 213 RVA: 0x000052D2 File Offset: 0x000034D2
		public BinaryPartitionBuilder(string imageSource)
		{
			if (string.IsNullOrEmpty(imageSource))
			{
				throw new ArgumentException("imageSource must not be null or empty.");
			}
			this.pkgObject.ImageSource = imageSource;
		}
	}
}
