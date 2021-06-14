using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsPhone.ImageUpdate.Tools;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000002 RID: 2
	public class CabArchiver
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public void AddFile(string destination, string source)
		{
			this.AddFileAtIndex(this.filesInCab.Count, destination, source);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002065 File Offset: 0x00000265
		public void AddFileToFront(string destination, string source)
		{
			this.AddFileAtIndex(0, destination, source);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002070 File Offset: 0x00000270
		private void AddFileAtIndex(int index, string destination, string source)
		{
			if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(destination))
			{
				throw new PackageException("Both source and destination must be set");
			}
			this.filesInCab.Insert(index, new KeyValuePair<string, string>(destination, source));
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020A0 File Offset: 0x000002A0
		public void Save(string cabPath, CompressionType compressionType)
		{
			string tempDirectory = FileUtils.GetTempDirectory();
			try
			{
				CabApiWrapper.CreateCabSelected(cabPath, (from x in this.filesInCab
				select LongPath.GetFullPathUNC(x.Value)).ToArray<string>(), (from x in this.filesInCab
				select x.Key.TrimStart(new char[]
				{
					'\\'
				})).ToArray<string>(), tempDirectory, compressionType);
			}
			catch (Exception innerException)
			{
				throw new PackageException(innerException, "Failed to save cab to {0}", new object[]
				{
					cabPath
				});
			}
			finally
			{
				FileUtils.DeleteTree(tempDirectory);
			}
		}

		// Token: 0x04000001 RID: 1
		private List<KeyValuePair<string, string>> filesInCab = new List<KeyValuePair<string, string>>();
	}
}
