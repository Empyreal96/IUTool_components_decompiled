using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Phone.Test.TestMetadata.CommandLine
{
	// Token: 0x0200001B RID: 27
	[Serializable]
	public class CommandException : Exception
	{
		// Token: 0x06000093 RID: 147 RVA: 0x00003B7C File Offset: 0x00001D7C
		public CommandException()
		{
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00003B86 File Offset: 0x00001D86
		public CommandException(string message) : base(message)
		{
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00003B91 File Offset: 0x00001D91
		public CommandException(string message, Exception exception) : base(message, exception)
		{
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00003B9D File Offset: 0x00001D9D
		protected CommandException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00003BA9 File Offset: 0x00001DA9
		public CommandException(string message, Command command) : base(message)
		{
			this._command = command;
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000098 RID: 152 RVA: 0x00003BBC File Offset: 0x00001DBC
		// (set) Token: 0x06000099 RID: 153 RVA: 0x00003BD4 File Offset: 0x00001DD4
		public Command Command
		{
			get
			{
				return this._command;
			}
			set
			{
				this._command = value;
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00003BDE File Offset: 0x00001DDE
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		// Token: 0x0400009A RID: 154
		private Command _command;
	}
}
