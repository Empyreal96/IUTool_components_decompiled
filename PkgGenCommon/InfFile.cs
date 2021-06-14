using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon
{
	// Token: 0x02000021 RID: 33
	public class InfFile
	{
		// Token: 0x06000063 RID: 99 RVA: 0x00003EC6 File Offset: 0x000020C6
		public InfFile(string infFilePath)
		{
			this.m_infFilePath = infFilePath;
			this.m_infLines = new List<string>(LongPathFile.ReadAllLines(this.m_infFilePath));
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000065 RID: 101 RVA: 0x00003EF4 File Offset: 0x000020F4
		// (set) Token: 0x06000064 RID: 100 RVA: 0x00003EEB File Offset: 0x000020EB
		public ISecurityPolicyCompiler SecurityCompiler { get; set; }

		// Token: 0x06000066 RID: 102 RVA: 0x00003EFC File Offset: 0x000020FC
		public string GetSectionSddl(string infSectionName)
		{
			int sectionStart = this.GetSectionStart(infSectionName);
			if (sectionStart == -1)
			{
				throw new IUException("INF section {0} referenced in pkg xml not found in {1}, if encodings other than ANSI are used in the input inf file, please make sure right BOM is included.", new object[]
				{
					infSectionName,
					this.m_infFilePath
				});
			}
			int sectionEnd = this.GetSectionEnd(sectionStart);
			string text = null;
			for (int i = sectionStart; i <= sectionEnd; i++)
			{
				string input = this.m_infLines[i];
				Match match = InfFile.regexInfSddl.Match(input);
				if (match.Success)
				{
					if (text != null)
					{
						throw new IUException("More than one security SDDL strings are specified around line {0}, file {1}", new object[]
						{
							i,
							this.m_infFilePath
						});
					}
					text = match.Groups["sddl"].Value;
				}
			}
			return text;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003FAC File Offset: 0x000021AC
		public void SetSectionSddl(string infSectionName, string infSddl)
		{
			int sectionStart = this.GetSectionStart(infSectionName);
			if (sectionStart == -1)
			{
				throw new IUException("INF section {0} referenced in pkg xml not found in {1}, if encodings other than ANSI are used in the input inf file, please make sure right BOM is included.", new object[]
				{
					infSectionName,
					this.m_infFilePath
				});
			}
			int sectionEnd = this.GetSectionEnd(sectionStart);
			List<string> list = new List<string>(from x in this.m_infLines.Skip(sectionStart).Take(sectionEnd - sectionStart + 1)
			where !InfFile.regexInfSddl.Match(x).Success
			select x);
			if (infSddl != null)
			{
				string item = string.Format("HKR,,Security,,\"{0}\"", infSddl);
				list.Add(";------ SDDL auto-updated from pkg.xml policy");
				list.Add(item);
				list.Add(string.Empty);
			}
			this.m_infLines.RemoveRange(sectionStart, sectionEnd - sectionStart + 1);
			this.m_infLines.InsertRange(sectionStart, list);
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00004078 File Offset: 0x00002278
		public void UpdateSecurityPolicy(string infSectionName)
		{
			if (string.IsNullOrEmpty(infSectionName))
			{
				throw new ArgumentNullException("infSectionName");
			}
			if (this.SecurityCompiler == null)
			{
				throw new PkgGenException("SecurityCompiler property not initialized", new object[0]);
			}
			string text = this.GetSectionSddl(infSectionName);
			text = this.SecurityCompiler.GetDriverSddlString(infSectionName, text);
			this.SetSectionSddl(infSectionName, text);
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000040D0 File Offset: 0x000022D0
		public void SaveInf(string outputPath)
		{
			if (string.IsNullOrEmpty(outputPath))
			{
				throw new ArgumentNullException("outputPath");
			}
			try
			{
				LongPathFile.WriteAllLines(outputPath, this.m_infLines.ToArray());
			}
			catch (IOException innerException)
			{
				throw new PkgGenException(innerException, "Failed to write updated INF {0} to disk", new object[]
				{
					outputPath
				});
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x0000412C File Offset: 0x0000232C
		private int GetSectionStart(string sectionName)
		{
			bool flag = false;
			int i;
			for (i = 0; i < this.m_infLines.Count; i++)
			{
				string text = this.m_infLines[i].Trim();
				if (!text.TrimStart(new char[0]).StartsWith(";", StringComparison.InvariantCulture))
				{
					string pattern = string.Format("^\\[[ \\t]*{0}[ \\t]*\\](.*)$", sectionName);
					if (Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return -1;
			}
			return i;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x0000419C File Offset: 0x0000239C
		private int GetSectionEnd(int sectionStart)
		{
			bool flag = false;
			int i;
			for (i = sectionStart + 1; i < this.m_infLines.Count; i++)
			{
				string text = this.m_infLines[i].Trim();
				if (!text.TrimStart(new char[0]).StartsWith(";", StringComparison.InvariantCulture) && InfFile.regexSectionEnd.IsMatch(text))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return this.m_infLines.Count - 1;
			}
			return i - 1;
		}

		// Token: 0x040000C8 RID: 200
		private string m_infFilePath;

		// Token: 0x040000C9 RID: 201
		private List<string> m_infLines;

		// Token: 0x040000CA RID: 202
		private static readonly Regex regexSectionEnd = new Regex("^\\[.*\\](.*)$", RegexOptions.Compiled);

		// Token: 0x040000CB RID: 203
		private static readonly Regex regexInfSddl = new Regex("HKR,,Security,,\"(?<sddl>.*)\"", RegexOptions.Compiled);

		// Token: 0x040000CC RID: 204
		private const string STR_SDDL_FORMAT = "HKR,,Security,,\"{0}\"";

		// Token: 0x040000CD RID: 205
		private const string STR_INF_COMMENT_START = ";";

		// Token: 0x040000CE RID: 206
		private const string STR_SECTION_START_PATTERN = "^\\[[ \\t]*{0}[ \\t]*\\](.*)$";

		// Token: 0x040000CF RID: 207
		private const string STR_SDDL_COMMENT = ";------ SDDL auto-updated from pkg.xml policy";
	}
}
