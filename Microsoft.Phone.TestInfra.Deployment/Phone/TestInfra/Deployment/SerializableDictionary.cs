using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000014 RID: 20
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>
	{
		// Token: 0x060000FE RID: 254 RVA: 0x000068C4 File Offset: 0x00004AC4
		public void SerializeToFile(string file)
		{
			bool flag = string.IsNullOrEmpty(file);
			if (flag)
			{
				throw new ArgumentException("cannot be null or empty.", "file");
			}
			string directoryName = Path.GetDirectoryName(Path.GetFullPath(file));
			bool flag2 = !Directory.Exists(directoryName);
			if (flag2)
			{
				Directory.CreateDirectory(directoryName);
			}
			string contents = new JavaScriptSerializer
			{
				MaxJsonLength = SerializableDictionary<TKey, TValue>.MaxJsonLength
			}.Serialize(this);
			File.WriteAllText(file, contents);
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00006934 File Offset: 0x00004B34
		public static SerializableDictionary<TKey, TValue> DeserializeFile(string file)
		{
			bool flag = string.IsNullOrEmpty(file);
			if (flag)
			{
				throw new ArgumentException("cannot be null or empty.", "file");
			}
			bool flag2 = !File.Exists(file);
			if (flag2)
			{
				throw new FileNotFoundException(file);
			}
			object obj = new JavaScriptSerializer
			{
				MaxJsonLength = SerializableDictionary<TKey, TValue>.MaxJsonLength
			}.Deserialize(File.ReadAllText(file), typeof(SerializableDictionary<TKey, TValue>));
			bool flag3 = !(obj is SerializableDictionary<TKey, TValue>);
			if (flag3)
			{
				throw new InvalidDataException(string.Format("File {0} cannot be deserialized to an obj of type {1}", file, typeof(SerializableDictionary<TKey, TValue>).Name));
			}
			return obj as SerializableDictionary<TKey, TValue>;
		}

		// Token: 0x04000060 RID: 96
		private static int MaxJsonLength = 20971520;
	}
}
