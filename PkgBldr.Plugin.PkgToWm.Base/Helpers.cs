using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Tools;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x0200000D RID: 13
	public static class Helpers
	{
		// Token: 0x06000022 RID: 34 RVA: 0x00003254 File Offset: 0x00001454
		public static string lowerCamel(string s)
		{
			if (s == "WNF")
			{
				return "wnf";
			}
			if (s == "COM")
			{
				return "com";
			}
			if (s == "ETWProvider")
			{
				return "etwProvider";
			}
			if (!(s == "SDRegValue"))
			{
				return char.ToLowerInvariant(s[0]).ToString() + s.Substring(1);
			}
			return "sdRegValue";
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000032CF File Offset: 0x000014CF
		public static string ConvertBuildFilter(string pkgBuildFilter)
		{
			return pkgBuildFilter.Replace("wow", "build.isWow").Replace("arch", "build.arch");
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000032F0 File Offset: 0x000014F0
		public static string ConvertResolutionFilter(string pkgResolutionFilter)
		{
			string text = pkgResolutionFilter;
			if (text != null && text != "*")
			{
				text = text.Replace("!", " not ");
				text = text.Replace(";", " or ");
				text = text.Trim(new char[]
				{
					' '
				});
			}
			return text;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00003344 File Offset: 0x00001544
		public static string ConvertMulitSz(string pkgValue)
		{
			string str = pkgValue.Replace(";", "&quot;,&quot;");
			return "&quot;" + str + "&quot;";
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00003378 File Offset: 0x00001578
		[SuppressMessage("Microsoft.Design", "CA1026")]
		public static string ComConvertThreading(string pkgThreading, IDeploymentLogger passedInLogger = null)
		{
			IDeploymentLogger deploymentLogger = passedInLogger ?? new Logger();
			string result = null;
			string a = pkgThreading.ToLowerInvariant();
			if (!(a == "both"))
			{
				if (!(a == "apartment"))
				{
					if (!(a == "free"))
					{
						if (!(a == "neutral"))
						{
							deploymentLogger.LogWarning("unknown COM threading {0}", new object[]
							{
								pkgThreading
							});
						}
						else
						{
							result = "Neutral";
						}
					}
					else
					{
						result = "MTA";
					}
				}
				else
				{
					result = "STA";
				}
			}
			else
			{
				result = "Both";
			}
			return result;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00003408 File Offset: 0x00001608
		internal static string GenerateWmResolutionFilter(XElement pkgElement)
		{
			string text = PkgBldrHelpers.GetAttributeValue(pkgElement, "Resolution");
			if (text != null)
			{
				text = Helpers.ConvertResolutionFilter(text);
			}
			return text;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x0000342C File Offset: 0x0000162C
		internal static string GenerateWmBuildFilter(XElement pkgElement, IDeploymentLogger logger)
		{
			string result = null;
			string attributeValue = PkgBldrHelpers.GetAttributeValue(pkgElement, "buildFilter");
			string attributeValue2 = PkgBldrHelpers.GetAttributeValue(pkgElement, "CpuFilter");
			if (attributeValue != null)
			{
				result = Helpers.ConvertBuildFilter(attributeValue);
			}
			if (attributeValue2 != null)
			{
				if (attributeValue == null)
				{
					result = "build.arch = " + attributeValue2.ToLowerInvariant();
				}
				else
				{
					logger.LogWarning("Pkg.xml contains both a CpuFilter and a buildFilter. Ignoring the CpuFilter.", new object[0]);
				}
			}
			return result;
		}
	}
}
