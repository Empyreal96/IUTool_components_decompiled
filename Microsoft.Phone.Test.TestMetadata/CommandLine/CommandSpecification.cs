using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;

namespace Microsoft.Phone.Test.TestMetadata.CommandLine
{
	// Token: 0x0200001F RID: 31
	public class CommandSpecification
	{
		// Token: 0x060000B1 RID: 177 RVA: 0x00004170 File Offset: 0x00002370
		public CommandSpecification(Type type)
		{
			bool flag = type == null;
			if (flag)
			{
				throw new ArgumentNullException("type");
			}
			this.CommandType = type;
			object[] customAttributes = this.CommandType.GetCustomAttributes(typeof(CommandAttribute), false);
			this._commandAttribute = (customAttributes[0] as CommandAttribute);
			this.OptionSpecifications = new OptionSpecificationCollection();
			this.OptionSpecifications.LoadFromType(this.CommandType);
			this.CommandAliases = new StringCollection();
			object[] customAttributes2 = this.CommandType.GetCustomAttributes(typeof(CommandAliasAttribute), true);
			foreach (CommandAliasAttribute commandAliasAttribute in customAttributes2)
			{
				this.CommandAliases.Add(commandAliasAttribute.Alias);
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x0000423C File Offset: 0x0000243C
		public bool AllowNoNameOptions
		{
			get
			{
				return this._commandAttribute.AllowNoNameOptions;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x0000425C File Offset: 0x0000245C
		public string Name
		{
			get
			{
				return this._commandAttribute.Name;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x0000427C File Offset: 0x0000247C
		public string GeneralInformation
		{
			get
			{
				return this._commandAttribute.GeneralInformation;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x0000429C File Offset: 0x0000249C
		public string BriefDescription
		{
			get
			{
				return this._commandAttribute.BriefDescription;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x000042BC File Offset: 0x000024BC
		public string BriefUsage
		{
			get
			{
				return this._commandAttribute.BriefUsage;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x000042D9 File Offset: 0x000024D9
		public OptionSpecificationCollection OptionSpecifications { get; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x000042E1 File Offset: 0x000024E1
		public Type CommandType { get; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x000042E9 File Offset: 0x000024E9
		public StringCollection CommandAliases { get; }

		// Token: 0x060000BA RID: 186 RVA: 0x000042F4 File Offset: 0x000024F4
		public void PrintFullUsage(TextWriter writer)
		{
			bool flag = writer == null;
			if (flag)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteLine(" {0} - {1}", this.Name, this.BriefDescription);
			writer.WriteLine();
			writer.WriteLine(FormatUtility.FormatStringForWidth(this.GeneralInformation, 2, 0, 80));
			writer.WriteLine(" Usage: {0}", this.BriefUsage);
			writer.WriteLine();
			bool flag2 = this.OptionSpecifications.Count > 0;
			if (flag2)
			{
				writer.WriteLine(" Options:");
				writer.WriteLine();
				string[] array = new string[this.OptionSpecifications.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this.OptionSpecifications[i].OptionName;
				}
				Array.Sort<string>(array);
				foreach (string optionName in array)
				{
					OptionSpecification optionSpecification = this.OptionSpecifications[optionName];
					bool flag3 = optionSpecification.Description != null;
					if (flag3)
					{
						string toFormat = string.Format(CultureInfo.CurrentCulture, optionSpecification.Description, new object[]
						{
							optionSpecification.OptionName
						});
						writer.WriteLine(FormatUtility.FormatStringForWidth(toFormat, 2, 2, 80));
					}
				}
			}
		}

		// Token: 0x040000A0 RID: 160
		private readonly CommandAttribute _commandAttribute;
	}
}
