using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x0200001E RID: 30
	public class RgaBuilder
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000132 RID: 306 RVA: 0x00006094 File Offset: 0x00004294
		public bool HasContent
		{
			get
			{
				return this._rgaValues.Count > 0;
			}
		}

		// Token: 0x06000133 RID: 307 RVA: 0x000060A4 File Offset: 0x000042A4
		public void AddRgaValue(string keyName, string valueName, params string[] values)
		{
			KeyValuePair<string, string> key = new KeyValuePair<string, string>(keyName, valueName);
			List<string> list = null;
			if (!this._rgaValues.TryGetValue(key, out list))
			{
				list = new List<string>();
				this._rgaValues.Add(key, list);
			}
			list.AddRange(values);
		}

		// Token: 0x06000134 RID: 308 RVA: 0x000060E8 File Offset: 0x000042E8
		public void Save(string outputFile)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Windows Registry Editor Version 5.00");
			foreach (IGrouping<string, KeyValuePair<KeyValuePair<string, string>, List<string>>> grouping in from x in this._rgaValues
			group x by x.Key.Key)
			{
				stringBuilder.AppendFormat("[{0}]", grouping.Key);
				stringBuilder.AppendLine();
				foreach (KeyValuePair<KeyValuePair<string, string>, List<string>> keyValuePair in grouping)
				{
					RegUtil.RegOutput(stringBuilder, keyValuePair.Key.Value, keyValuePair.Value);
				}
				stringBuilder.AppendLine();
			}
			LongPathFile.WriteAllText(outputFile, stringBuilder.ToString(), Encoding.Unicode);
		}

		// Token: 0x04000058 RID: 88
		private Dictionary<KeyValuePair<string, string>, List<string>> _rgaValues = new Dictionary<KeyValuePair<string, string>, List<string>>(new RgaBuilder.KeyValuePairComparer());

		// Token: 0x02000041 RID: 65
		private class KeyValuePairComparer : IEqualityComparer<KeyValuePair<string, string>>
		{
			// Token: 0x060001F4 RID: 500 RVA: 0x000089C0 File Offset: 0x00006BC0
			public bool Equals(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
			{
				return x.Key.Equals(y.Key, StringComparison.OrdinalIgnoreCase) && x.Value.Equals(y.Value, StringComparison.OrdinalIgnoreCase);
			}

			// Token: 0x060001F5 RID: 501 RVA: 0x000089EE File Offset: 0x00006BEE
			public int GetHashCode(KeyValuePair<string, string> obj)
			{
				return obj.Key.GetHashCode() ^ obj.Value.GetHashCode();
			}
		}
	}
}
