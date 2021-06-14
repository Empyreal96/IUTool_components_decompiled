using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization.XML
{
	// Token: 0x0200001A RID: 26
	[XmlRoot(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class Condition
	{
		// Token: 0x1700009B RID: 155
		// (get) Token: 0x0600017A RID: 378 RVA: 0x00008081 File Offset: 0x00006281
		// (set) Token: 0x0600017B RID: 379 RVA: 0x00008089 File Offset: 0x00006289
		[XmlAttribute]
		public string Name { get; set; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x0600017C RID: 380 RVA: 0x00008092 File Offset: 0x00006292
		// (set) Token: 0x0600017D RID: 381 RVA: 0x0000809A File Offset: 0x0000629A
		[XmlAttribute]
		public string Value { get; set; }

		// Token: 0x0600017E RID: 382 RVA: 0x000080A3 File Offset: 0x000062A3
		public bool ShouldSerializeValue()
		{
			return !this.IsWildCard;
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x0600017F RID: 383 RVA: 0x000080AE File Offset: 0x000062AE
		// (set) Token: 0x06000180 RID: 384 RVA: 0x000080CA File Offset: 0x000062CA
		[XmlAttribute]
		public bool IsWildCard
		{
			get
			{
				return this.isWildCard != null && this.isWildCard.Value;
			}
			set
			{
				this.isWildCard = new bool?(value);
			}
		}

		// Token: 0x06000181 RID: 385 RVA: 0x000080D8 File Offset: 0x000062D8
		public bool ShouldSerializeIsWildCard()
		{
			return this.IsWildCard;
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00002230 File Offset: 0x00000430
		public Condition()
		{
		}

		// Token: 0x06000183 RID: 387 RVA: 0x000080E0 File Offset: 0x000062E0
		public Condition(string name, string value) : this(name, value, false)
		{
		}

		// Token: 0x06000184 RID: 388 RVA: 0x000080EB File Offset: 0x000062EB
		public Condition(string name, string value, bool IsWildCard)
		{
			this.Name = name;
			this.Value = value;
			this.IsWildCard = IsWildCard;
		}

		// Token: 0x0400007F RID: 127
		private bool? isWildCard;
	}
}
