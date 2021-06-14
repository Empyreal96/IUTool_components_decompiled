using System;
using System.Collections.Generic;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000059 RID: 89
	public static class CertStoreBuilder
	{
		// Token: 0x0600017E RID: 382 RVA: 0x00006750 File Offset: 0x00004950
		public static bool Build(IEnumerable<string> certs, string output)
		{
			List<byte> list = new List<byte>();
			foreach (string text in certs)
			{
				if (!LongPathFile.Exists(text))
				{
					throw new PkgGenException("Certificate file '{0}' doens't exist", new object[]
					{
						text
					});
				}
				list.AddRange(LongPathFile.ReadAllBytes(text));
			}
			if (list.Count > 0)
			{
				LongPathFile.WriteAllBytes(output, list.ToArray());
				return true;
			}
			return false;
		}
	}
}
