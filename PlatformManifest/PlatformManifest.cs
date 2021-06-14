using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.SecureBoot
{
	// Token: 0x02000003 RID: 3
	public class PlatformManifest
	{
		// Token: 0x06000002 RID: 2 RVA: 0x00002059 File Offset: 0x00000259
		private static byte[] GetHash(string input)
		{
			return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002070 File Offset: 0x00000270
		private static byte[] GetUnicodeHash(string input)
		{
			return SHA256.Create().ComputeHash(Encoding.Unicode.GetBytes(input));
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002087 File Offset: 0x00000287
		public PlatformManifest(Guid id, string bs)
		{
			this.Id = id;
			this.BuildString = bs;
			this.ImageType = PlatformManifest.ImageReleaseType.Retail;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000005 RID: 5 RVA: 0x000020BA File Offset: 0x000002BA
		// (set) Token: 0x06000006 RID: 6 RVA: 0x000020C2 File Offset: 0x000002C2
		public PlatformManifest.ImageReleaseType ImageType { get; set; }

		// Token: 0x06000007 RID: 7 RVA: 0x000020CB File Offset: 0x000002CB
		public void AddRawEntry(byte[] NewEntry)
		{
			if (NewEntry.Length != 32)
			{
				throw new Exception("PlatformManifest!AddRawEntry: Invalid Entry Length for Platform Manifest");
			}
			if (this.ImageType == PlatformManifest.ImageReleaseType.Retail && NewEntry.SequenceEqual(PlatformManifest.DebugIDHash))
			{
				throw new InvalidDebugIDException("PlatformManitest!AddRawEntry: Debug ID not allowed when generating a Retail image.");
			}
			this.Entries.Add(NewEntry);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000210B File Offset: 0x0000030B
		public void AddStringEntry(string NewEntry)
		{
			this.AddRawEntry(PlatformManifest.GetHash(NewEntry));
			this.TextEntries.Add(NewEntry);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002128 File Offset: 0x00000328
		public void AddBinaryFromSignInfo(string SigninfoFile)
		{
			SignInfo signInfo = SignInfo.LoadFromFile(SigninfoFile);
			this.AddRawEntry(Convert.FromBase64String(signInfo.BinaryIdHash));
			this.TextEntries.Add(string.Format("SignInfo: {0}  BinaryIdHash={1}", Path.GetFileName(SigninfoFile), signInfo.BinaryIdHash));
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002170 File Offset: 0x00000370
		public void AddBinaryFromFullFileHash(byte[] FullFileHash)
		{
			SignInfoEntry binaryIDHashFromFullFileHash = SigninfoDB.GetBinaryIDHashFromFullFileHash(FullFileHash);
			this.AddRawEntry(binaryIDHashFromFullFileHash.BinaryIDHash);
			this.TextEntries.Add(string.Format("SignInfo: {2}  FullFileHash={0}  BinaryIdHash={1}", BitConverter.ToString(FullFileHash).Replace("-", ""), BitConverter.ToString(binaryIDHashFromFullFileHash.BinaryIDHash).Replace("-", ""), binaryIDHashFromFullFileHash.SignInfoFilename));
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000021DC File Offset: 0x000003DC
		public void WriteRawPlatformManifestToFile(string OutputPath, string TextFilePath)
		{
			if (this.ImageType == PlatformManifest.ImageReleaseType.Test)
			{
				this.AddRawEntry(PlatformManifest.DebugIDHash);
			}
			this.AddRawEntry(PlatformManifest.GetUnicodeHash("Bootmgr"));
			BinaryWriter binaryWriter = new BinaryWriter(File.Open(OutputPath, FileMode.Create), Encoding.Unicode);
			StreamWriter streamWriter = File.CreateText(TextFilePath);
			char[] array = new char[128];
			char[] array2 = this.BuildString.ToCharArray();
			int num = 0;
			foreach (char c in array2)
			{
				array[num++] = c;
				if (num > array.Length)
				{
					break;
				}
			}
			binaryWriter.Write(1718446928U);
			binaryWriter.Write(1);
			binaryWriter.Write(this.Id.ToByteArray());
			binaryWriter.Write(1U);
			binaryWriter.Write(array);
			binaryWriter.Write((uint)this.Entries.Count);
			foreach (byte[] array4 in this.Entries)
			{
				if (array4.Length != 32)
				{
					throw new Exception("PlatformManifest!WriteToFile: Invalid hash length");
				}
				binaryWriter.Write(array4);
			}
			foreach (string value in this.TextEntries)
			{
				streamWriter.WriteLine(value);
			}
			binaryWriter.Close();
			streamWriter.Close();
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002354 File Offset: 0x00000554
		public void SignFileP7(string Filename)
		{
			int num = 0;
			if (!File.Exists(Environment.ExpandEnvironmentVariables("%RazzleToolPath%\\urtrun.cmd")))
			{
				throw new Exception("Unable to find urtrun.cmd!");
			}
			try
			{
				num = CommonUtils.RunProcess("%COMSPEC%", string.Format("/C sign.cmd -platman \"{0}\"", Filename));
			}
			catch (Exception innerException)
			{
				throw new Exception(string.Format("Failed to sign the Platform Manifest with commandline sign.cmd -platman \"{0}\"", Filename), innerException);
			}
			if (num != 0)
			{
				throw new Exception(string.Format("Failed to sign the Platform Manifest with commandline sign.cmd -platman \"{0}\", exit code {1}", Filename, num));
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000023D8 File Offset: 0x000005D8
		public void WriteToFile(string OutputPath)
		{
			string tempFile = FileUtils.GetTempFile();
			this.WriteRawPlatformManifestToFile(tempFile, OutputPath + ".txt");
			this.SignFileP7(tempFile);
			if (File.Exists(OutputPath))
			{
				File.Delete(OutputPath);
			}
			File.Move(tempFile + ".p7", OutputPath);
			File.Delete(tempFile);
		}

		// Token: 0x04000001 RID: 1
		private Guid Id;

		// Token: 0x04000002 RID: 2
		private string BuildString;

		// Token: 0x04000003 RID: 3
		private IList<byte[]> Entries = new List<byte[]>();

		// Token: 0x04000004 RID: 4
		private IList<string> TextEntries = new List<string>();

		// Token: 0x04000005 RID: 5
		private const string DebugIDString = "DebugFileId";

		// Token: 0x04000006 RID: 6
		private const string BootmgrId = "Bootmgr";

		// Token: 0x04000007 RID: 7
		private static byte[] DebugIDHash = PlatformManifest.GetUnicodeHash("DebugFileId");

		// Token: 0x04000008 RID: 8
		public const int PACKAGE_MANIFEST_V1_HASH_LENGTH = 32;

		// Token: 0x02000008 RID: 8
		public enum ImageReleaseType
		{
			// Token: 0x04000013 RID: 19
			Retail,
			// Token: 0x04000014 RID: 20
			Test
		}
	}
}
