using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces
{
	// Token: 0x0200004B RID: 75
	public sealed class PackageDependency : IComparable, IComparer
	{
		// Token: 0x0600010F RID: 271 RVA: 0x00006D0F File Offset: 0x00004F0F
		public PackageDependency()
		{
			this.Name = string.Empty;
			this.MinVersion = string.Empty;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00006D2D File Offset: 0x00004F2D
		public PackageDependency(PackageDependency packageDependency)
		{
			this.Name = packageDependency.Name;
			this.MinVersion = packageDependency.MinVersion;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00006D4D File Offset: 0x00004F4D
		public PackageDependency(string name, string minVersion)
		{
			this.Name = name;
			this.MinVersion = minVersion;
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000112 RID: 274 RVA: 0x00006D63 File Offset: 0x00004F63
		// (set) Token: 0x06000113 RID: 275 RVA: 0x00006D6B File Offset: 0x00004F6B
		public string Name { get; set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000114 RID: 276 RVA: 0x00006D74 File Offset: 0x00004F74
		// (set) Token: 0x06000115 RID: 277 RVA: 0x00006D7C File Offset: 0x00004F7C
		public string MinVersion { get; set; }

		// Token: 0x06000116 RID: 278 RVA: 0x00006D88 File Offset: 0x00004F88
		public bool IsValid()
		{
			Regex regex = new Regex("(0|[1-9][0-9]{0,3}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])(\\.(0|[1-9][0-9]{0,3}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])){3}", RegexOptions.Singleline);
			return !string.IsNullOrWhiteSpace(this.Name) && !string.IsNullOrWhiteSpace(this.MinVersion) && regex.IsMatch(this.MinVersion);
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00006DCC File Offset: 0x00004FCC
		public bool MeetsVersionRequirements(string version)
		{
			if (string.IsNullOrWhiteSpace(version))
			{
				throw new ArgumentNullException("version", "The version parameter is null!");
			}
			if (!this.IsValid())
			{
				throw new InvalidDataException("INTERNAL ERROR: The PackageDependency object is not a valid object!");
			}
			if (!version.Contains("."))
			{
				throw new ArgumentException("The passed in version value does not look like a version string!", "version");
			}
			string[] array = this.MinVersion.Split(new char[]
			{
				'.'
			});
			string[] array2 = version.Split(new char[]
			{
				'.'
			});
			bool result = true;
			int num = array.Length;
			int num2 = array2.Length;
			int num3 = Math.Min(num, num2);
			int num4 = Math.Max(num, num2);
			if (num < num4)
			{
				StringBuilder stringBuilder = new StringBuilder(this.MinVersion);
				for (int i = 0; i < num4 - num; i++)
				{
					stringBuilder.Append(".0");
				}
				array = stringBuilder.ToString().Split(new char[]
				{
					'.'
				});
				LogUtil.Diagnostic("PackageDependency: Padded the shorter MinVersion from {0} to {1}", new object[]
				{
					this.MinVersion,
					string.Join(".", array)
				});
			}
			else if (num2 < num4)
			{
				StringBuilder stringBuilder2 = new StringBuilder(version);
				for (int j = 0; j < num4 - num2; j++)
				{
					stringBuilder2.Append(".0");
				}
				array2 = stringBuilder2.ToString().Split(new char[]
				{
					'.'
				});
				LogUtil.Diagnostic("PackageDependency: Padded the shorter version from {0} to {1}", new object[]
				{
					version,
					string.Join(".", array2)
				});
			}
			num3 = Math.Min(array.Length, array2.Length);
			for (int k = 0; k < num3; k++)
			{
				uint num5 = 0U;
				uint num6 = 0U;
				if (!uint.TryParse(array2[k], out num6) || !uint.TryParse(array[k], out num5))
				{
					throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "Internal error: {0} and {1} cannot be compared due to non-numeric fields", new object[]
					{
						version,
						this.MinVersion
					}));
				}
				if (num6 != num5)
				{
					result = (num6 > num5);
					break;
				}
			}
			return result;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00006FBB File Offset: 0x000051BB
		public int CompareTo(object obj)
		{
			return this.Compare(this, obj);
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00006FC8 File Offset: 0x000051C8
		public int Compare(object obj1, object obj2)
		{
			if ((obj1 is string || obj1 is PackageDependency) && (obj2 is string || obj2 is PackageDependency))
			{
				PackageDependency packageDependency = (obj1 is string) ? new PackageDependency(obj1 as string, "") : (obj1 as PackageDependency);
				PackageDependency packageDependency2 = (obj2 is string) ? new PackageDependency(obj2 as string, "") : (obj2 as PackageDependency);
				return string.Compare(packageDependency.Name, packageDependency2.Name, StringComparison.OrdinalIgnoreCase);
			}
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "cannot compare objects of type {0} against type {1}", new object[]
			{
				obj1.GetType(),
				obj2.GetType()
			}));
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00007074 File Offset: 0x00005274
		public override bool Equals(object obj)
		{
			return this.Compare(this, obj) == 0;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00007081 File Offset: 0x00005281
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00007089 File Offset: 0x00005289
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "(Name)=\"{0}\", (MinVersion)=\"{1}\"", new object[]
			{
				this.Name,
				this.MinVersion
			});
		}
	}
}
