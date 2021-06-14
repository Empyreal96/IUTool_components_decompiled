using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.SecureBoot
{
	// Token: 0x02000007 RID: 7
	internal static class SigninfoDB
	{
		// Token: 0x0600001D RID: 29 RVA: 0x00002560 File Offset: 0x00000760
		private static string GetSigninfoPath()
		{
			string text = Environment.GetEnvironmentVariable("BINARY_ROOT");
			if (string.IsNullOrEmpty(text))
			{
				text = Environment.GetEnvironmentVariable("_NTTREE");
				if (!string.IsNullOrEmpty(text))
				{
					text += ".WM.MC";
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new Exception("SigninfoDB: Unable to find Signinfo directory.  Either BINARY_ROOT or _NTTREE must be set.");
			}
			return Path.Combine(text, SigninfoDB.SignInfoDir);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000025C0 File Offset: 0x000007C0
		private static void LoadSignInfos()
		{
			string signinfoPath = SigninfoDB.GetSigninfoPath();
			List<string> list = new List<string>();
			foreach (string text in Directory.EnumerateFiles(signinfoPath, "*.signinfo", SearchOption.AllDirectories))
			{
				SignInfo signInfo = SignInfo.LoadFromFile(text);
				if (SigninfoDB.FullFileHashToBinaryIDHash.ContainsKey(signInfo.FullFileHash))
				{
					list.Add(string.Format("SigninfoDB: Found an additional signinfo file with a duplicate FullFileHash: {0}  Filename={1}  ExistingFilename={2}", BitConverter.ToString(signInfo.FullFileHash).Replace("-", ""), text, SigninfoDB.FullFileHashToBinaryIDHash[signInfo.FullFileHash].SignInfoFilename));
				}
				else
				{
					SigninfoDB.FullFileHashToBinaryIDHash.Add(signInfo.FullFileHash, new SignInfoEntry(Convert.FromBase64String(signInfo.BinaryIdHash), text));
				}
			}
			if (list.Count > 0)
			{
				ConsoleColor foregroundColor = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Yellow;
				foreach (string value in list)
				{
					Console.WriteLine(value);
				}
				Console.ForegroundColor = foregroundColor;
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000026F4 File Offset: 0x000008F4
		public static SignInfoEntry GetBinaryIDHashFromFullFileHash(byte[] FullFileHash)
		{
			if (SigninfoDB.FullFileHashToBinaryIDHash == null)
			{
				SigninfoDB.FullFileHashToBinaryIDHash = new Dictionary<byte[], SignInfoEntry>(new ByteArrayComparer());
				SigninfoDB.LoadSignInfos();
			}
			return SigninfoDB.FullFileHashToBinaryIDHash[FullFileHash];
		}

		// Token: 0x04000010 RID: 16
		private static Dictionary<byte[], SignInfoEntry> FullFileHashToBinaryIDHash = null;

		// Token: 0x04000011 RID: 17
		public static string SignInfoDir = "SignInfo";
	}
}
