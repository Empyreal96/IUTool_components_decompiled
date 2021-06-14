using System;
using System.Diagnostics;
using System.Globalization;

namespace System.Reflection.Adds
{
	// Token: 0x0200001F RID: 31
	public struct Token
	{
		// Token: 0x060000EF RID: 239 RVA: 0x000047CA File Offset: 0x000029CA
		[DebuggerStepThrough]
		public Token(int value)
		{
			this.Value = value;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x000047D4 File Offset: 0x000029D4
		public Token(TokenType type, int rid)
		{
			this.Value = type + rid;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x000047CA File Offset: 0x000029CA
		[DebuggerStepThrough]
		public Token(uint value)
		{
			this.Value = value;
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x000047E0 File Offset: 0x000029E0
		public int Value { get; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x000047E8 File Offset: 0x000029E8
		public TokenType TokenType
		{
			get
			{
				return (TokenType)((long)this.Value & (long)((ulong)-16777216));
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x0000480C File Offset: 0x00002A0C
		public int Index
		{
			get
			{
				return this.Value & 16777215;
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x0000482C File Offset: 0x00002A2C
		public bool IsNil
		{
			get
			{
				return this.Index == 0;
			}
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00004848 File Offset: 0x00002A48
		public static implicit operator int(Token token)
		{
			return token.Value;
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00004864 File Offset: 0x00002A64
		public static bool operator ==(Token token1, Token token2)
		{
			return token1.Value == token2.Value;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00004888 File Offset: 0x00002A88
		public static bool operator !=(Token token1, Token token2)
		{
			return !(token1 == token2);
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x000048A4 File Offset: 0x00002AA4
		public static bool operator ==(Token token1, int token2)
		{
			return token1.Value == token2;
		}

		// Token: 0x060000FA RID: 250 RVA: 0x000048C0 File Offset: 0x00002AC0
		public static bool operator !=(Token token1, int token2)
		{
			return !(token1 == token2);
		}

		// Token: 0x060000FB RID: 251 RVA: 0x000048DC File Offset: 0x00002ADC
		public static bool operator ==(int token1, Token token2)
		{
			return token1 == token2.Value;
		}

		// Token: 0x060000FC RID: 252 RVA: 0x000048F8 File Offset: 0x00002AF8
		public static bool operator !=(int token1, Token token2)
		{
			return !(token1 == token2);
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00004914 File Offset: 0x00002B14
		public static bool IsType(int token, params TokenType[] types)
		{
			for (int i = 0; i < types.Length; i++)
			{
				bool flag = (TokenType)((long)token & (long)((ulong)-16777216)) == types[i];
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00004954 File Offset: 0x00002B54
		public bool IsType(TokenType type)
		{
			return this.TokenType == type;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00004970 File Offset: 0x00002B70
		public override bool Equals(object obj)
		{
			bool flag = obj is Token;
			bool result;
			if (flag)
			{
				Token token = (Token)obj;
				result = (this.Value == token.Value);
			}
			else
			{
				bool flag2 = obj is int;
				result = (flag2 && this.Value == (int)obj);
			}
			return result;
		}

		// Token: 0x06000100 RID: 256 RVA: 0x000049CC File Offset: 0x00002BCC
		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		// Token: 0x06000101 RID: 257 RVA: 0x000049EC File Offset: 0x00002BEC
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}(0x{1:x})", new object[]
			{
				this.TokenType,
				this.Index
			});
		}

		// Token: 0x0400007D RID: 125
		public static readonly Token Nil = new Token(0);
	}
}
