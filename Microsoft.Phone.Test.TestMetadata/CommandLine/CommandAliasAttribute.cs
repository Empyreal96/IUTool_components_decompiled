using System;

namespace Microsoft.Phone.Test.TestMetadata.CommandLine
{
	// Token: 0x02000019 RID: 25
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class CommandAliasAttribute : Attribute
	{
		// Token: 0x06000087 RID: 135 RVA: 0x00003AEE File Offset: 0x00001CEE
		public CommandAliasAttribute(string alias)
		{
			this.Alias = alias;
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000088 RID: 136 RVA: 0x00003AFF File Offset: 0x00001CFF
		public string Alias { get; }
	}
}
