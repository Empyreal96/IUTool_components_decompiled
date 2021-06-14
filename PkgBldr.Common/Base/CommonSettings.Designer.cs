using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x02000028 RID: 40
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
	internal sealed partial class CommonSettings : ApplicationSettingsBase
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000096 RID: 150 RVA: 0x00006CDB File Offset: 0x00004EDB
		public static CommonSettings Default
		{
			get
			{
				return CommonSettings.defaultInstance;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000097 RID: 151 RVA: 0x00006CE2 File Offset: 0x00004EE2
		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool ErrorOnDeconstructionFailure
		{
			get
			{
				return (bool)this["ErrorOnDeconstructionFailure"];
			}
		}

		// Token: 0x04000014 RID: 20
		private static CommonSettings defaultInstance = (CommonSettings)SettingsBase.Synchronized(new CommonSettings());
	}
}
