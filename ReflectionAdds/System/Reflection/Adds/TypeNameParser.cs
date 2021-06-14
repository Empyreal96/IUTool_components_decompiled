using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection.Mock;
using System.Text;

namespace System.Reflection.Adds
{
	// Token: 0x02000020 RID: 32
	internal static class TypeNameParser
	{
		// Token: 0x06000103 RID: 259 RVA: 0x00004A40 File Offset: 0x00002C40
		public static Type ParseTypeName(ITypeUniverse universe, Module module, string input)
		{
			bool throwOnError = true;
			return TypeNameParser.ParseTypeName(universe, module, input, throwOnError);
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00004A60 File Offset: 0x00002C60
		public static Type ParseTypeName(ITypeUniverse universe, Module module, string input, bool throwOnError)
		{
			int num = 0;
			Type result = TypeNameParser.ParseTypeName(universe, module, input, ref num, throwOnError, false, false);
			bool flag = throwOnError && num != input.Length;
			if (flag)
			{
				throw new ArgumentException(Resources.ExtraCharactersAfterTypeName);
			}
			return result;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00004AA8 File Offset: 0x00002CA8
		private static Type ParseTypeName(ITypeUniverse universe, Module defaultTokenResolver, string input, ref int idx, bool throwOnError, bool isGenericArgument, bool expectAssemblyName)
		{
			List<string> list = new List<string>();
			List<Type> list2 = new List<Type>();
			AssemblyName assemblyName = null;
			for (;;)
			{
				list.Add(TypeNameParser.ReadIdWithoutLeadingSpaces(input, ref idx));
				bool flag = TypeNameParser.PeekNextToken(input, idx) != TypeNameParser.TokenType.Plus;
				if (flag)
				{
					break;
				}
				TypeNameParser.ReadSpecialToken(input, TypeNameParser.TokenType.Plus, ref idx);
			}
			bool flag2 = TypeNameParser.IsGenericType(input, idx);
			if (flag2)
			{
				TypeNameParser.ReadSpecialToken(input, TypeNameParser.TokenType.LeftBracket, ref idx);
				for (;;)
				{
					bool flag3 = false;
					bool flag4 = TypeNameParser.PeekNextToken(input, idx) == TypeNameParser.TokenType.LeftBracket;
					if (flag4)
					{
						flag3 = true;
						TypeNameParser.ReadSpecialToken(input, TypeNameParser.TokenType.LeftBracket, ref idx);
					}
					Type type = TypeNameParser.ParseTypeName(universe, defaultTokenResolver, input, ref idx, throwOnError, true, flag3);
					bool flag5 = type == null;
					if (flag5)
					{
						break;
					}
					list2.Add(type);
					bool flag6 = flag3;
					if (flag6)
					{
						TypeNameParser.ReadSpecialToken(input, TypeNameParser.TokenType.RightBracket, ref idx);
					}
					bool flag7 = TypeNameParser.PeekNextToken(input, idx) == TypeNameParser.TokenType.Comma;
					if (!flag7)
					{
						goto IL_E4;
					}
					TypeNameParser.ReadSpecialToken(input, TypeNameParser.TokenType.Comma, ref idx);
				}
				return null;
				IL_E4:
				TypeNameParser.ReadSpecialToken(input, TypeNameParser.TokenType.RightBracket, ref idx);
			}
			int num = idx;
			TypeNameParser.ReadModifiers(null, input, ref idx);
			int num2 = idx;
			bool flag8 = !isGenericArgument || expectAssemblyName;
			if (flag8)
			{
				bool flag9 = TypeNameParser.PeekNextToken(input, idx) == TypeNameParser.TokenType.Comma;
				if (flag9)
				{
					TypeNameParser.ReadSpecialToken(input, TypeNameParser.TokenType.Comma, ref idx);
					assemblyName = TypeNameParser.ParseAssemblyInfo(input, ref idx);
				}
			}
			Assembly assembly = TypeNameParser.DetermineAssembly(assemblyName, defaultTokenResolver, universe);
			Type type2 = TypeNameParser.Resolve(list, list2, assembly);
			bool flag10 = type2 == null;
			Type result;
			if (flag10)
			{
				if (throwOnError)
				{
					throw new TypeLoadException(string.Format(CultureInfo.InvariantCulture, Resources.CannotFindTypeInModule, new object[]
					{
						input,
						assembly
					}));
				}
				result = null;
			}
			else
			{
				type2 = TypeNameParser.ReadModifiers(type2, input, ref num);
				result = type2;
			}
			return result;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00004C58 File Offset: 0x00002E58
		private static bool IsGenericType(string input, int idx)
		{
			return TypeNameParser.PeekNextToken(input, idx) == TypeNameParser.TokenType.LeftBracket && (TypeNameParser.PeekSecondToken(input, idx) != TypeNameParser.TokenType.RightBracket && TypeNameParser.PeekSecondToken(input, idx) != TypeNameParser.TokenType.Comma) && TypeNameParser.PeekSecondToken(input, idx) != TypeNameParser.TokenType.Pointer;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00004CA8 File Offset: 0x00002EA8
		private static Type ReadModifiers(Type type, string input, ref int idx)
		{
			for (;;)
			{
				bool flag = TypeNameParser.PeekNextToken(input, idx) == TypeNameParser.TokenType.LeftBracket;
				if (flag)
				{
					bool flag2 = TypeNameParser.PeekSecondToken(input, idx) == TypeNameParser.TokenType.RightBracket;
					if (flag2)
					{
						TypeNameParser.ReadSpecialToken(input, TypeNameParser.TokenType.LeftBracket, ref idx);
						TypeNameParser.ReadSpecialToken(input, TypeNameParser.TokenType.RightBracket, ref idx);
						bool flag3 = type != null;
						if (flag3)
						{
							type = type.MakeArrayType();
						}
						continue;
					}
					bool flag4 = TypeNameParser.PeekSecondToken(input, idx) == TypeNameParser.TokenType.Comma;
					if (flag4)
					{
						TypeNameParser.ReadSpecialToken(input, TypeNameParser.TokenType.LeftBracket, ref idx);
						int num = 1;
						while (TypeNameParser.PeekNextToken(input, idx) == TypeNameParser.TokenType.Comma)
						{
							TypeNameParser.ReadSpecialToken(input, TypeNameParser.TokenType.Comma, ref idx);
							num++;
						}
						bool flag5 = TypeNameParser.PeekNextToken(input, idx) == TypeNameParser.TokenType.RightBracket;
						if (flag5)
						{
							TypeNameParser.ReadSpecialToken(input, TypeNameParser.TokenType.RightBracket, ref idx);
							bool flag6 = type != null;
							if (flag6)
							{
								type = type.MakeArrayType(num);
							}
							continue;
						}
						break;
					}
				}
				bool flag7 = TypeNameParser.PeekNextToken(input, idx) == TypeNameParser.TokenType.Reference;
				if (flag7)
				{
					TypeNameParser.ReadSpecialToken(input, TypeNameParser.TokenType.Reference, ref idx);
					bool flag8 = type != null;
					if (flag8)
					{
						type = type.MakeByRefType();
					}
				}
				else
				{
					bool flag9 = TypeNameParser.PeekNextToken(input, idx) == TypeNameParser.TokenType.Pointer;
					if (!flag9)
					{
						return type;
					}
					TypeNameParser.ReadSpecialToken(input, TypeNameParser.TokenType.Pointer, ref idx);
					bool flag10 = type != null;
					if (flag10)
					{
						type = type.MakePointerType();
					}
				}
			}
			throw new ArgumentException(Resources.UnexpectedCharacterFound);
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00004DF8 File Offset: 0x00002FF8
		private static AssemblyName ParseAssemblyInfo(string input, ref int idx)
		{
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = TypeNameParser.ReadIdWithoutLeadingAndEndingSpaces(input, ref idx);
			string text;
			for (;;)
			{
				TypeNameParser.TokenType tokenType = TypeNameParser.PeekNextToken(input, idx);
				if (tokenType != TypeNameParser.TokenType.Comma)
				{
					break;
				}
				TypeNameParser.ReadSpecialToken(input, TypeNameParser.TokenType.Comma, ref idx);
				text = TypeNameParser.ReadIdWithoutLeadingAndEndingSpaces(input, ref idx);
				TypeNameParser.ReadSpecialToken(input, TypeNameParser.TokenType.Equals, ref idx);
				string text2 = TypeNameParser.ReadIdWithoutLeadingAndEndingSpaces(input, ref idx);
				string a = text;
				if (!(a == "Version"))
				{
					if (!(a == "Culture"))
					{
						if (!(a == "PublicKeyToken"))
						{
							goto Block_4;
						}
						bool flag = !text2.Equals("null");
						if (flag)
						{
							bool flag2 = (text2.Length & 1) != 0;
							if (flag2)
							{
								goto Block_8;
							}
							byte[] array = new byte[text2.Length / 2];
							for (int i = 0; i < array.Length; i++)
							{
								array[i] = byte.Parse(text2.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
							}
							assemblyName.SetPublicKeyToken(array);
						}
						else
						{
							assemblyName.SetPublicKeyToken(new byte[0]);
						}
					}
					else
					{
						bool flag3 = !text2.Equals("neutral");
						if (flag3)
						{
							assemblyName.CultureInfo = CultureInfo.GetCultureInfo(text2);
						}
						else
						{
							assemblyName.CultureInfo = CultureInfo.InvariantCulture;
						}
					}
				}
				else
				{
					bool flag4 = assemblyName.Version != null;
					if (flag4)
					{
						goto Block_5;
					}
					assemblyName.Version = new Version(text2);
				}
			}
			return assemblyName;
			Block_4:
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.UnrecognizedAssemblyAttribute, new object[]
			{
				text
			}));
			Block_5:
			throw new ArgumentException(Resources.VersionAlreadyDefined);
			Block_8:
			throw new ArgumentException(Resources.InvalidPublicKeyTokenLength);
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00004FB4 File Offset: 0x000031B4
		private static Assembly DetermineAssembly(AssemblyName assemblyName, Module defaultTokenResolver, ITypeUniverse universe)
		{
			bool flag = assemblyName != null;
			Assembly result;
			if (flag)
			{
				bool flag2 = universe == null;
				if (flag2)
				{
					throw new ArgumentException(Resources.HostSpecifierMissing);
				}
				Assembly assembly = universe.ResolveAssembly(assemblyName);
				bool flag3 = assembly == null;
				if (flag3)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.UniverseCannotResolveAssembly, new object[]
					{
						assemblyName
					}));
				}
				result = assembly;
			}
			else
			{
				bool flag4 = defaultTokenResolver == null;
				if (flag4)
				{
					throw new ArgumentException(Resources.DefaultTokenResolverRequired);
				}
				result = defaultTokenResolver.Assembly;
			}
			return result;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x0000503C File Offset: 0x0000323C
		private static Type Resolve(List<string> path, List<Type> genericTypeArgs, Assembly assembly)
		{
			Type type = assembly.GetType(path[0], false);
			bool flag = type == null;
			Type result;
			if (flag)
			{
				result = null;
			}
			else
			{
				for (int i = 1; i < path.Count; i++)
				{
					Type nestedType = type.GetNestedType(path[i], BindingFlags.Public | BindingFlags.NonPublic);
					bool flag2 = nestedType == null;
					if (flag2)
					{
						return null;
					}
					type = nestedType;
				}
				bool flag3 = genericTypeArgs.Count > 0;
				if (flag3)
				{
					type = type.MakeGenericType(genericTypeArgs.ToArray());
				}
				result = type;
			}
			return result;
		}

		// Token: 0x0600010B RID: 267 RVA: 0x000050C8 File Offset: 0x000032C8
		private static void ReadSpecialToken(string input, TypeNameParser.TokenType expected, ref int idx)
		{
			TypeNameParser.Token token = TypeNameParser.ReadToken(input, ref idx);
			bool flag = token == null || token.TokenType != expected;
			if (flag)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ExpectedTokenType, new object[]
				{
					expected
				}));
			}
		}

		// Token: 0x0600010C RID: 268 RVA: 0x0000511C File Offset: 0x0000331C
		private static string ReadIdToken(string input, ref int idx)
		{
			TypeNameParser.Token token = TypeNameParser.ReadToken(input, ref idx);
			bool flag = token == null || token.TokenType > TypeNameParser.TokenType.Id;
			if (flag)
			{
				throw new ArgumentException(Resources.IdTokenTypeExpected);
			}
			return token.Value;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x0000515C File Offset: 0x0000335C
		private static string ReadIdWithoutLeadingSpaces(string input, ref int idx)
		{
			return TypeNameParser.TrimLeadingSpaces(TypeNameParser.ReadIdToken(input, ref idx));
		}

		// Token: 0x0600010E RID: 270 RVA: 0x0000517C File Offset: 0x0000337C
		private static string ReadIdWithoutLeadingAndEndingSpaces(string input, ref int idx)
		{
			return TypeNameParser.ReadIdToken(input, ref idx).Trim();
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000519C File Offset: 0x0000339C
		private static TypeNameParser.TokenType PeekNextToken(string input, int idx)
		{
			TypeNameParser.Token token = TypeNameParser.ReadToken(input, ref idx);
			bool flag = token == null;
			TypeNameParser.TokenType result;
			if (flag)
			{
				result = TypeNameParser.TokenType.EndOfInput;
			}
			else
			{
				result = token.TokenType;
			}
			return result;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x000051CC File Offset: 0x000033CC
		private static TypeNameParser.TokenType PeekSecondToken(string input, int idx)
		{
			TypeNameParser.Token token = TypeNameParser.ReadToken(input, ref idx);
			bool flag = token == null;
			if (flag)
			{
				throw new ArgumentException(Resources.UnexpectedEndOfInput);
			}
			return TypeNameParser.PeekNextToken(input, idx);
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00005204 File Offset: 0x00003404
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		private static TypeNameParser.Token ReadToken(string input, ref int idx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			int i;
			for (i = idx; i < input.Length; i++)
			{
				char c = input[i];
				bool flag2 = flag;
				if (flag2)
				{
					stringBuilder.Append(c);
					flag = false;
				}
				else
				{
					char c2 = c;
					if (c2 <= '=')
					{
						switch (c2)
						{
						case '&':
						case '*':
						case '+':
						case ',':
							break;
						case '\'':
							flag = true;
							goto IL_15C;
						case '(':
						case ')':
							goto IL_152;
						default:
							if (c2 != '=')
							{
								goto IL_152;
							}
							break;
						}
					}
					else if (c2 != '[' && c2 != ']')
					{
						goto IL_152;
					}
					bool flag3 = stringBuilder.Length > 0;
					if (!flag3)
					{
						idx = i + 1;
						char c3 = c;
						if (c3 <= '=')
						{
							switch (c3)
							{
							case '&':
								return TypeNameParser.Token.Reference;
							case '\'':
							case '(':
							case ')':
								break;
							case '*':
								return TypeNameParser.Token.Pointer;
							case '+':
								return TypeNameParser.Token.Plus;
							case ',':
								return TypeNameParser.Token.Comma;
							default:
								if (c3 == '=')
								{
									return TypeNameParser.Token.Equals;
								}
								break;
							}
						}
						else
						{
							if (c3 == '[')
							{
								return TypeNameParser.Token.LeftBracket;
							}
							if (c3 == ']')
							{
								return TypeNameParser.Token.RightBracket;
							}
						}
						throw new InvalidOperationException(Resources.UnexpectedCharacterFound);
					}
					idx = i;
					return TypeNameParser.Token.MakeIdToken(stringBuilder.ToString());
					IL_15C:
					goto IL_15D;
					IL_152:
					stringBuilder.Append(c);
				}
				IL_15D:;
			}
			bool flag4 = flag;
			if (flag4)
			{
				throw new ArgumentException(Resources.EscapeSequenceMissingCharacter);
			}
			idx = i;
			bool flag5 = stringBuilder.Length > 0;
			if (flag5)
			{
				return TypeNameParser.Token.MakeIdToken(stringBuilder.ToString());
			}
			return null;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x000053C4 File Offset: 0x000035C4
		private static string TrimLeadingSpaces(string str)
		{
			int num = 0;
			while (num < str.Length && char.IsWhiteSpace(str, num))
			{
				num++;
			}
			return str.Substring(num);
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00005400 File Offset: 0x00003600
		public static bool IsCompoundType(string name)
		{
			return name.IndexOfAny(TypeNameParser.s_compoundTypeNameCharacters) > 0;
		}

		// Token: 0x0400007F RID: 127
		private static readonly char[] s_compoundTypeNameCharacters = new char[]
		{
			'+',
			',',
			'[',
			'*',
			'&'
		};

		// Token: 0x0200003A RID: 58
		private enum TokenType
		{
			// Token: 0x0400012D RID: 301
			Id,
			// Token: 0x0400012E RID: 302
			LeftBracket,
			// Token: 0x0400012F RID: 303
			RightBracket,
			// Token: 0x04000130 RID: 304
			Comma,
			// Token: 0x04000131 RID: 305
			Plus,
			// Token: 0x04000132 RID: 306
			Equals,
			// Token: 0x04000133 RID: 307
			Reference,
			// Token: 0x04000134 RID: 308
			Pointer,
			// Token: 0x04000135 RID: 309
			EndOfInput
		}

		// Token: 0x0200003B RID: 59
		private class Token
		{
			// Token: 0x06000138 RID: 312 RVA: 0x00005510 File Offset: 0x00003710
			private Token(TypeNameParser.TokenType tokenType, string value)
			{
				this.TokenType = tokenType;
				this.Value = value;
			}

			// Token: 0x1700006F RID: 111
			// (get) Token: 0x06000139 RID: 313 RVA: 0x00005528 File Offset: 0x00003728
			internal TypeNameParser.TokenType TokenType { get; }

			// Token: 0x17000070 RID: 112
			// (get) Token: 0x0600013A RID: 314 RVA: 0x00005530 File Offset: 0x00003730
			internal string Value { get; }

			// Token: 0x0600013B RID: 315 RVA: 0x00005538 File Offset: 0x00003738
			internal static TypeNameParser.Token MakeIdToken(string value)
			{
				return new TypeNameParser.Token(TypeNameParser.TokenType.Id, value);
			}

			// Token: 0x04000138 RID: 312
			internal static readonly TypeNameParser.Token Plus = new TypeNameParser.Token(TypeNameParser.TokenType.Plus, null);

			// Token: 0x04000139 RID: 313
			internal static readonly TypeNameParser.Token LeftBracket = new TypeNameParser.Token(TypeNameParser.TokenType.LeftBracket, null);

			// Token: 0x0400013A RID: 314
			internal static readonly TypeNameParser.Token RightBracket = new TypeNameParser.Token(TypeNameParser.TokenType.RightBracket, null);

			// Token: 0x0400013B RID: 315
			internal static readonly TypeNameParser.Token Comma = new TypeNameParser.Token(TypeNameParser.TokenType.Comma, null);

			// Token: 0x0400013C RID: 316
			internal new static readonly TypeNameParser.Token Equals = new TypeNameParser.Token(TypeNameParser.TokenType.Equals, null);

			// Token: 0x0400013D RID: 317
			internal static readonly TypeNameParser.Token Reference = new TypeNameParser.Token(TypeNameParser.TokenType.Reference, null);

			// Token: 0x0400013E RID: 318
			internal static readonly TypeNameParser.Token Pointer = new TypeNameParser.Token(TypeNameParser.TokenType.Pointer, null);
		}
	}
}
