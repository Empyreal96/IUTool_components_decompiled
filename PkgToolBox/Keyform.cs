using System;
using System.Text;
using Microsoft.Composition.ToolBox.Reflection;

namespace Microsoft.Composition.ToolBox
{
	// Token: 0x0200000C RID: 12
	public class Keyform : ReflectiveObject
	{
		// Token: 0x06000017 RID: 23 RVA: 0x000025E6 File Offset: 0x000007E6
		public Keyform()
		{
			this.Version = new Version();
			this.BuildType = BuildType.Invalid;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002600 File Offset: 0x00000800
		public Keyform(Keyform keyform)
		{
			this.Name = keyform.Name;
			this.HostArch = keyform.HostArch;
			this.GuestArch = keyform.GuestArch;
			this.BuildType = keyform.BuildType;
			this.ReleaseType = keyform.ReleaseType;
			this.Language = keyform.Language;
			this.VersionScope = keyform.VersionScope;
			this.Version = keyform.Version;
			this.PublicKeyToken = keyform.PublicKeyToken;
			this.InstallType = keyform.InstallType;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000268C File Offset: 0x0000088C
		public Keyform(string name, CpuArch hostArch, CpuArch guestArch, BuildType buildType, string releaseType, string language, string versionScope, Version version, string publicKeyToken)
		{
			this.Name = name;
			this.HostArch = hostArch;
			this.GuestArch = guestArch;
			this.BuildType = buildType;
			this.ReleaseType = releaseType;
			this.Language = language;
			this.VersionScope = versionScope;
			this.Version = version;
			this.PublicKeyToken = publicKeyToken;
			this.InstallType = string.Empty;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000026F0 File Offset: 0x000008F0
		public Keyform(string name, CpuArch hostArch, CpuArch guestArch, BuildType buildType, string releaseType, string language, string versionScope, Version version, string publicKeyToken, string installType)
		{
			this.Name = name;
			this.HostArch = hostArch;
			this.GuestArch = guestArch;
			this.BuildType = buildType;
			this.ReleaseType = releaseType;
			this.Language = language;
			this.VersionScope = versionScope;
			this.Version = version;
			this.PublicKeyToken = publicKeyToken;
			this.InstallType = installType;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600001B RID: 27 RVA: 0x00002750 File Offset: 0x00000950
		// (set) Token: 0x0600001C RID: 28 RVA: 0x00002758 File Offset: 0x00000958
		public string Name { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00002761 File Offset: 0x00000961
		// (set) Token: 0x0600001E RID: 30 RVA: 0x00002769 File Offset: 0x00000969
		public CpuArch HostArch { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002772 File Offset: 0x00000972
		// (set) Token: 0x06000020 RID: 32 RVA: 0x0000277A File Offset: 0x0000097A
		public CpuArch GuestArch { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002783 File Offset: 0x00000983
		// (set) Token: 0x06000022 RID: 34 RVA: 0x0000278B File Offset: 0x0000098B
		public BuildType BuildType { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00002794 File Offset: 0x00000994
		// (set) Token: 0x06000024 RID: 36 RVA: 0x0000279C File Offset: 0x0000099C
		public string ReleaseType { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000025 RID: 37 RVA: 0x000027A5 File Offset: 0x000009A5
		// (set) Token: 0x06000026 RID: 38 RVA: 0x000027AD File Offset: 0x000009AD
		public string Language { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000027 RID: 39 RVA: 0x000027B6 File Offset: 0x000009B6
		// (set) Token: 0x06000028 RID: 40 RVA: 0x000027BE File Offset: 0x000009BE
		public string VersionScope { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000029 RID: 41 RVA: 0x000027C7 File Offset: 0x000009C7
		// (set) Token: 0x0600002A RID: 42 RVA: 0x000027CF File Offset: 0x000009CF
		public Version Version { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600002B RID: 43 RVA: 0x000027D8 File Offset: 0x000009D8
		// (set) Token: 0x0600002C RID: 44 RVA: 0x000027E0 File Offset: 0x000009E0
		public string PublicKeyToken { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600002D RID: 45 RVA: 0x000027E9 File Offset: 0x000009E9
		// (set) Token: 0x0600002E RID: 46 RVA: 0x000027F1 File Offset: 0x000009F1
		public string InstallType { get; set; }

		// Token: 0x0600002F RID: 47 RVA: 0x000027FA File Offset: 0x000009FA
		public static string GenerateKeyform(string arch, string name, string version, string publicKey, string language)
		{
			return Keyform.GenerateKeyform(arch, name, version, publicKey, language, string.Empty, "SxS");
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002811 File Offset: 0x00000A11
		public static string GenerateKeyform(string arch, string name, string version, string publicKey, string language, string type)
		{
			return Keyform.GenerateKeyform(arch, name, version, publicKey, language, type, "SxS");
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002828 File Offset: 0x00000A28
		public static string GenerateKeyform(string arch, string name, string version, string publicKey, string language, string type, string sxs)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.EnsureCapacity(256);
			NativeMethods.GetKeyForm(arch, name, version, publicKey, sxs, language, type, stringBuilder, 256);
			return stringBuilder.ToString();
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002863 File Offset: 0x00000A63
		public static string GenerateKeyform(Keyform keyform)
		{
			return Keyform.GenerateKeyform(ManifestToolBox.GetCpuString(keyform), keyform.Name, keyform.Version.ToString(), keyform.PublicKeyToken, keyform.Language, keyform.InstallType, keyform.VersionScope);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x0000289C File Offset: 0x00000A9C
		public static string GenerateCBSKeyform(string name, string publicKey, string arch, string version, string language)
		{
			if (string.IsNullOrEmpty(language) || language.Equals("neutral", StringComparison.InvariantCulture))
			{
				return string.Format("{0}~{1}~{2}~~{3}", new object[]
				{
					name,
					publicKey,
					arch,
					version
				});
			}
			return string.Format("{0}~{1}~{2}~{3}~{4}", new object[]
			{
				name,
				publicKey,
				arch,
				language,
				version
			});
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002907 File Offset: 0x00000B07
		public static string GenerateCBSKeyform(Keyform keyform)
		{
			return Keyform.GenerateCBSKeyform(keyform.Name, keyform.PublicKeyToken, ManifestToolBox.GetCpuString(keyform), keyform.Version.ToString(), keyform.Language);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002934 File Offset: 0x00000B34
		public bool CompareKeyforms(Keyform keyform)
		{
			return keyform.PublicKeyToken == this.PublicKeyToken && keyform.HostArch == this.HostArch && keyform.GuestArch == this.GuestArch && string.Equals(keyform.Name, this.Name, StringComparison.CurrentCultureIgnoreCase) && string.Equals(keyform.Language, this.Language, StringComparison.CurrentCultureIgnoreCase) && string.Equals(keyform.VersionScope, this.VersionScope, StringComparison.CurrentCultureIgnoreCase) && keyform.Version.Equals(this.Version);
		}
	}
}
