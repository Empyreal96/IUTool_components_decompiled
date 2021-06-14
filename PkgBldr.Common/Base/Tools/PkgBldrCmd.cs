using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.CompPlat.PkgBldr.Base.Tools
{
	// Token: 0x0200005A RID: 90
	public class PkgBldrCmd
	{
		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060001B4 RID: 436 RVA: 0x0000AA14 File Offset: 0x00008C14
		// (set) Token: 0x060001B5 RID: 437 RVA: 0x0000AA1C File Offset: 0x00008C1C
		[Description("Full path to input file : .wm.xml, .pkg.xml, .man")]
		public string project { get; set; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060001B6 RID: 438 RVA: 0x0000AA25 File Offset: 0x00008C25
		// (set) Token: 0x060001B7 RID: 439 RVA: 0x0000AA2D File Offset: 0x00008C2D
		[Description("Output directory or file.")]
		public string output { get; set; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060001B8 RID: 440 RVA: 0x0000AA36 File Offset: 0x00008C36
		// (set) Token: 0x060001B9 RID: 441 RVA: 0x0000AA3E File Offset: 0x00008C3E
		[Description("Version string in the form of <major>.<minor>.<qfe>.<build>")]
		public string version { get; set; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060001BA RID: 442 RVA: 0x0000AA47 File Offset: 0x00008C47
		// (set) Token: 0x060001BB RID: 443 RVA: 0x0000AA4F File Offset: 0x00008C4F
		[Description("CPU type. Values: (x86|arm|arm64|amd64)")]
		public CpuType cpu { get; set; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060001BC RID: 444 RVA: 0x0000AA58 File Offset: 0x00008C58
		// (set) Token: 0x060001BD RID: 445 RVA: 0x0000AA60 File Offset: 0x00008C60
		[Description("Supported language identifier list, separated by ';'")]
		public string languages { get; set; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060001BE RID: 446 RVA: 0x0000AA69 File Offset: 0x00008C69
		// (set) Token: 0x060001BF RID: 447 RVA: 0x0000AA71 File Offset: 0x00008C71
		[Description("Supported resolution identifier list, separated by ';'")]
		public string resolutions { get; set; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060001C0 RID: 448 RVA: 0x0000AA7A File Offset: 0x00008C7A
		// (set) Token: 0x060001C1 RID: 449 RVA: 0x0000AA82 File Offset: 0x00008C82
		[Description("Additional variables used in the project file,syntax:<name>=<value>;<name>=<value>;....")]
		public string variables { get; set; }

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060001C2 RID: 450 RVA: 0x0000AA8B File Offset: 0x00008C8B
		// (set) Token: 0x060001C3 RID: 451 RVA: 0x0000AA93 File Offset: 0x00008C93
		[Description("Enable debug output.")]
		public bool diagnostic { get; set; }

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060001C4 RID: 452 RVA: 0x0000AA9C File Offset: 0x00008C9C
		// (set) Token: 0x060001C5 RID: 453 RVA: 0x0000AAA4 File Offset: 0x00008CA4
		[Description("Path to write the windows manifest schema.")]
		public string wmxsd { get; set; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060001C6 RID: 454 RVA: 0x0000AAAD File Offset: 0x00008CAD
		// (set) Token: 0x060001C7 RID: 455 RVA: 0x0000AAB5 File Offset: 0x00008CB5
		[Description("The type of conversion operation to perform. Values: (wm2csi|csi2wm|pkg2csi|pkg2wm|csi2pkg|pkg2cab)")]
		public ConversionType convert { get; set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060001C8 RID: 456 RVA: 0x0000AABE File Offset: 0x00008CBE
		// (set) Token: 0x060001C9 RID: 457 RVA: 0x0000AAC6 File Offset: 0x00008CC6
		[Description("Supported product identifier")]
		public string product { get; set; }

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x060001CA RID: 458 RVA: 0x0000AACF File Offset: 0x00008CCF
		// (set) Token: 0x060001CB RID: 459 RVA: 0x0000AAD7 File Offset: 0x00008CD7
		[Description("Output directory for guest packages")]
		public string wowdir { get; set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x060001CC RID: 460 RVA: 0x0000AAE0 File Offset: 0x00008CE0
		// (set) Token: 0x060001CD RID: 461 RVA: 0x0000AAE8 File Offset: 0x00008CE8
		[Description("HostOnly, GuestOnly, or Both")]
		public WowBuildType wowbuild { get; set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x060001CE RID: 462 RVA: 0x0000AAF1 File Offset: 0x00008CF1
		// (set) Token: 0x060001CF RID: 463 RVA: 0x0000AAF9 File Offset: 0x00008CF9
		[Description("Use NtverpUtils to get the Windows product version")]
		public bool usentverp { get; set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x060001D0 RID: 464 RVA: 0x0000AB02 File Offset: 0x00008D02
		// (set) Token: 0x060001D1 RID: 465 RVA: 0x0000AB0A File Offset: 0x00008D0A
		[Description("Process the driver's INF like the build does")]
		public bool processInf { get; set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060001D2 RID: 466 RVA: 0x0000AB13 File Offset: 0x00008D13
		// (set) Token: 0x060001D3 RID: 467 RVA: 0x0000AB1B File Offset: 0x00008D1B
		[Description("Generate CAB(s) when using wm.xml")]
		public bool makecab { get; set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060001D4 RID: 468 RVA: 0x0000AB24 File Offset: 0x00008D24
		// (set) Token: 0x060001D5 RID: 469 RVA: 0x0000AB2C File Offset: 0x00008D2C
		[Description("Only log warnings and errors")]
		public bool quiet { get; set; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x060001D6 RID: 470 RVA: 0x0000AB35 File Offset: 0x00008D35
		// (set) Token: 0x060001D7 RID: 471 RVA: 0x0000AB3D File Offset: 0x00008D3D
		[Description("Building TOC files instead of the actual package")]
		public bool toc { get; set; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x0000AB46 File Offset: 0x00008D46
		// (set) Token: 0x060001D9 RID: 473 RVA: 0x0000AB4E File Offset: 0x00008D4E
		[Description("Compressing the generated package.")]
		public bool compress { get; set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x060001DA RID: 474 RVA: 0x0000AB57 File Offset: 0x00008D57
		// (set) Token: 0x060001DB RID: 475 RVA: 0x0000AB5F File Offset: 0x00008D5F
		[Description("File with globally defined variables.")]
		public string config { get; set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x060001DC RID: 476 RVA: 0x0000AB68 File Offset: 0x00008D68
		// (set) Token: 0x060001DD RID: 477 RVA: 0x0000AB70 File Offset: 0x00008D70
		[Description("Build type string Values: (fre|chk)")]
		public BuildType build { get; set; }

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x060001DE RID: 478 RVA: 0x0000AB79 File Offset: 0x00008D79
		// (set) Token: 0x060001DF RID: 479 RVA: 0x0000AB81 File Offset: 0x00008D81
		[Description("Path to write the auto-generated windows phone manifest schema")]
		public string xsd { get; set; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x060001E0 RID: 480 RVA: 0x0000AB8A File Offset: 0x00008D8A
		// (set) Token: 0x060001E1 RID: 481 RVA: 0x0000AB92 File Offset: 0x00008D92
		[Description("The package is for onecore products, this sets nohives = true for BSP’s")]
		public bool onecore { get; set; }

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060001E2 RID: 482 RVA: 0x0000AB9B File Offset: 0x00008D9B
		// (set) Token: 0x060001E3 RID: 483 RVA: 0x0000ABA3 File Offset: 0x00008DA3
		[Description("Indicates whether or not this package has no hive dependency")]
		public bool nohives { get; set; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060001E4 RID: 484 RVA: 0x0000ABAC File Offset: 0x00008DAC
		// (set) Token: 0x060001E5 RID: 485 RVA: 0x0000ABB4 File Offset: 0x00008DB4
		[Description("Location of SdxRoot.")]
		public string sdxRoot { get; set; }

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060001E6 RID: 486 RVA: 0x0000ABBD File Offset: 0x00008DBD
		// (set) Token: 0x060001E7 RID: 487 RVA: 0x0000ABC5 File Offset: 0x00008DC5
		[Description("Location of RazzleToolPath.")]
		public string razzleToolPath { get; set; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060001E8 RID: 488 RVA: 0x0000ABCE File Offset: 0x00008DCE
		// (set) Token: 0x060001E9 RID: 489 RVA: 0x0000ABD6 File Offset: 0x00008DD6
		[Description("Location of RazzleDataPath")]
		public string razzleDataPath { get; set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060001EA RID: 490 RVA: 0x0000ABDF File Offset: 0x00008DDF
		// (set) Token: 0x060001EB RID: 491 RVA: 0x0000ABE7 File Offset: 0x00008DE7
		[Description("Location of _NTTREE")]
		public string nttree { get; set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060001EC RID: 492 RVA: 0x0000ABF0 File Offset: 0x00008DF0
		// (set) Token: 0x060001ED RID: 493 RVA: 0x0000ABF8 File Offset: 0x00008DF8
		[Description("Location of build.nttree")]
		public string buildNttree { get; set; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060001EE RID: 494 RVA: 0x0000AC01 File Offset: 0x00008E01
		// (set) Token: 0x060001EF RID: 495 RVA: 0x0000AC09 File Offset: 0x00008E09
		[Description("Location of capabilityList.cfg")]
		public string capabilityListCfg { get; set; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060001F0 RID: 496 RVA: 0x0000AC12 File Offset: 0x00008E12
		// (set) Token: 0x060001F1 RID: 497 RVA: 0x0000AC1A File Offset: 0x00008E1A
		[Description("_BuildBranch")]
		public string buildBranch { get; set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060001F2 RID: 498 RVA: 0x0000AC23 File Offset: 0x00008E23
		// (set) Token: 0x060001F3 RID: 499 RVA: 0x0000AC2B File Offset: 0x00008E2B
		[Description("Location of manifest_sddl.txt")]
		public string manifestSddlTxt { get; set; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060001F4 RID: 500 RVA: 0x0000AC34 File Offset: 0x00008E34
		// (set) Token: 0x060001F5 RID: 501 RVA: 0x0000AC3C File Offset: 0x00008E3C
		[Description("Location of NTDEV_LSSettings.lsconfig")]
		public string ntdevLssettingsLsconfig { get; set; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060001F6 RID: 502 RVA: 0x0000AC45 File Offset: 0x00008E45
		// (set) Token: 0x060001F7 RID: 503 RVA: 0x0000AC4D File Offset: 0x00008E4D
		[Description("Location of bldnump.h")]
		public string bldNumpH { get; set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060001F8 RID: 504 RVA: 0x0000AC56 File Offset: 0x00008E56
		// (set) Token: 0x060001F9 RID: 505 RVA: 0x0000AC5E File Offset: 0x00008E5E
		[Description("Location of cmiv2.dll")]
		public string cmiV2Dll { get; set; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060001FA RID: 506 RVA: 0x0000AC67 File Offset: 0x00008E67
		// (set) Token: 0x060001FB RID: 507 RVA: 0x0000AC6F File Offset: 0x00008E6F
		[Description("Path to PkgBldr.CSI.Xsd.")]
		public string csiXsdPath { get; set; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060001FC RID: 508 RVA: 0x0000AC78 File Offset: 0x00008E78
		// (set) Token: 0x060001FD RID: 509 RVA: 0x0000AC80 File Offset: 0x00008E80
		[Description("Path to PkgBldr.PKG.Xsd.")]
		public string pkgXsdPath { get; set; }

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060001FE RID: 510 RVA: 0x0000AC89 File Offset: 0x00008E89
		// (set) Token: 0x060001FF RID: 511 RVA: 0x0000AC91 File Offset: 0x00008E91
		[Description("Path to PkgBldr.Shared.Xsd.")]
		public string sharedXsdPath { get; set; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000200 RID: 512 RVA: 0x0000AC9A File Offset: 0x00008E9A
		// (set) Token: 0x06000201 RID: 513 RVA: 0x0000ACA2 File Offset: 0x00008EA2
		[Description("Path to PkgBldr.WM.Xsd.")]
		public string wmXsdPath { get; set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000202 RID: 514 RVA: 0x0000ACAB File Offset: 0x00008EAB
		// (set) Token: 0x06000203 RID: 515 RVA: 0x0000ACB3 File Offset: 0x00008EB3
		[Description("Directories containing tools needed by spkggen.exe")]
		public string spkgGenToolDirs { get; set; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000204 RID: 516 RVA: 0x0000ACBC File Offset: 0x00008EBC
		// (set) Token: 0x06000205 RID: 517 RVA: 0x0000ACC4 File Offset: 0x00008EC4
		[Description("Generate JSON's in the specifed depot when conerting from pkg.xml to wm.xml")]
		public string json { get; set; }

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000206 RID: 518 RVA: 0x0000ACCD File Offset: 0x00008ECD
		// (set) Token: 0x06000207 RID: 519 RVA: 0x0000ACD5 File Offset: 0x00008ED5
		[SuppressMessage("Microsoft.Usage", "CA2227", Justification = "Required by command line parser.")]
		[Description("Dictionary of tool paths needed by pkggen.exe")]
		public Dictionary<string, string> toolPaths { get; set; }

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000208 RID: 520 RVA: 0x0000ACDE File Offset: 0x00008EDE
		// (set) Token: 0x06000209 RID: 521 RVA: 0x0000ACE6 File Offset: 0x00008EE6
		[Description("Indicates that the package should be test signed only")]
		public bool testOnly { get; set; }

		// Token: 0x0600020A RID: 522 RVA: 0x0000ACF0 File Offset: 0x00008EF0
		public PkgBldrCmd()
		{
			this.output = ".";
			this.version = "1.0.0.0";
			this.cpu = CpuType.arm;
			this.convert = ConversionType.pkg2cab;
			this.product = "windows";
			this.wowbuild = WowBuildType.Both;
			this.usentverp = false;
			this.processInf = false;
			this.makecab = false;
			this.quiet = false;
			this.toc = false;
			this.compress = false;
			this.diagnostic = false;
			this.build = BuildType.fre;
			this.nohives = false;
			this.testOnly = false;
			this.sdxRoot = "%sdxroot%";
			this.razzleToolPath = "%RazzleToolPath%";
			this.razzleDataPath = "%RazzleDataPath%";
			this.nttree = "%_nttree%";
			this.buildNttree = "%build.nttree%";
			this.capabilityListCfg = "%RazzleToolPath%\\managed\\v4.0\\capabilitylist.cfg";
			this.buildBranch = "%_BuildBranch%";
			this.manifestSddlTxt = "%RazzleToolPath%\\manifest_sddl.txt";
			this.ntdevLssettingsLsconfig = "% RazzleToolPath %\\locstudio\\NTDEV_LSSettings.lsconfig";
			this.bldNumpH = "%PUBLIC_ROOT%\\sdk\\inc\\bldnump.h";
			this.cmiV2Dll = "%RazzleToolPath%\\x86\\cmiv2.dll";
			this.csiXsdPath = "%RazzleToolPath%\\managed\\v4.0\\PkgBldr.CSI.Xsd";
			this.pkgXsdPath = "%RazzleToolPath%\\managed\\v4.0\\PkgBldr.PKG.Xsd";
			this.sharedXsdPath = "%RazzleToolPath%\\managed\\v4.0\\PkgBldr.Shared.Xsd";
			this.wmXsdPath = "%RazzleToolPath%\\managed\\v4.0\\PkgBldr.WM.Xsd";
			this.spkgGenToolDirs = "%RazzleToolPath%\\x86";
			this.toolPaths = new Dictionary<string, string>
			{
				{
					"pkgresolvepartial",
					"%RazzleToolPath%\\x86\\pkgresolvepartial.exe"
				},
				{
					"genddf",
					"%RazzleToolPath%\\x86\\genddf.exe"
				},
				{
					"makecab",
					"%RazzleToolPath%\\x86\\MakeCab.exe"
				},
				{
					"updcat",
					"%RazzleToolPath%\\x86\\updcat.exe"
				},
				{
					"sign",
					"%RazzleToolPath%\\sign.cmd"
				},
				{
					"perl",
					"%RazzleToolPath%\\perl\\bin\\perl.exe"
				},
				{
					"spkggen",
					"%RazzleToolPath%\\managed\\v4.0\\spkggen.exe"
				},
				{
					"cmd",
					"cmd.exe"
				},
				{
					"unitext",
					"unitext.exe"
				},
				{
					"prodfilt",
					"prodfilt.exe"
				},
				{
					"stampinf",
					"stampinf.exe"
				},
				{
					"infutil",
					"infutil.exe"
				},
				{
					"binplace",
					"%RazzleToolPath%\\x86\\binplace.exe"
				},
				{
					"urtrun",
					"%RazzleToolPath%\\urtrun.cmd"
				},
				{
					"reformatmanifest",
					"%RazzleToolPath%\\reformatmanifest.cmd"
				},
				{
					"cmimanifest",
					"%RazzleToolPath\\cmi-manifest.pl"
				},
				{
					"filemanager",
					"%RazzleToolPath%\\x86\\filemanager.exe"
				},
				{
					"keyform",
					"%RazzleToolPath%\\x86\\keyform.exe"
				},
				{
					"cl",
					"%RazzleToolPath%\\DEV12\\x32\\x86\\cl.exe"
				}
			};
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x0600020B RID: 523 RVA: 0x0000AF74 File Offset: 0x00009174
		public bool isRazzleEnv
		{
			get
			{
				bool result = true;
				if (this.razzleDataPath.Equals("%RazzleDataPath%", StringComparison.OrdinalIgnoreCase))
				{
					result = !Environment.ExpandEnvironmentVariables(this.razzleDataPath).Equals(this.razzleDataPath);
				}
				return result;
			}
		}

		// Token: 0x0400012F RID: 303
		[Description("Number of threads to use when converting SPKG's to CAB's")]
		public int ConvertDsmThreadCount = 25;
	}
}
