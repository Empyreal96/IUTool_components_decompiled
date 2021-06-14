using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;
using System.Runtime.InteropServices;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000029 RID: 41
	internal class MetadataOnlyMethodBodyWorker : MetadataOnlyMethodBody
	{
		// Token: 0x06000217 RID: 535 RVA: 0x00005F6C File Offset: 0x0000416C
		public MetadataOnlyMethodBodyWorker(MetadataOnlyMethodInfo method, MetadataOnlyMethodBodyWorker.IMethodHeader header) : base(method)
		{
			this._header = header;
		}

		// Token: 0x06000218 RID: 536 RVA: 0x00005F80 File Offset: 0x00004180
		internal static MethodBody Create(MetadataOnlyMethodInfo method)
		{
			MetadataOnlyModule resolver = method.Resolver;
			uint methodRva = resolver.GetMethodRva(method.MetadataToken);
			bool flag = methodRva == 0U;
			MethodBody result;
			if (flag)
			{
				result = null;
			}
			else
			{
				MetadataOnlyMethodBodyWorker.IMethodHeader methodHeader = MetadataOnlyMethodBodyWorker.GetMethodHeader(methodRva, resolver);
				MetadataOnlyMethodBody metadataOnlyMethodBody = new MetadataOnlyMethodBodyWorker(method, methodHeader);
				result = metadataOnlyMethodBody;
			}
			return result;
		}

		// Token: 0x06000219 RID: 537 RVA: 0x00005FC8 File Offset: 0x000041C8
		public static MetadataOnlyMethodBodyWorker.IMethodHeader GetMethodHeader(uint rva, MetadataOnlyModule scope)
		{
			byte[] array = scope.RawMetadata.ReadRva((long)((ulong)rva), 1);
			MetadataOnlyMethodBodyWorker.MethodHeaderFlags methodHeaderFlags = (MetadataOnlyMethodBodyWorker.MethodHeaderFlags)(array[0] & 3);
			bool flag = methodHeaderFlags == MetadataOnlyMethodBodyWorker.MethodHeaderFlags.FatFormat;
			MetadataOnlyMethodBodyWorker.IMethodHeader result;
			if (flag)
			{
				MetadataOnlyMethodBodyWorker.IMethodHeader methodHeader = scope.RawMetadata.ReadRvaStruct<MetadataOnlyMethodBodyWorker.FatHeader>((long)((ulong)rva));
				result = methodHeader;
			}
			else
			{
				bool flag2 = methodHeaderFlags == MetadataOnlyMethodBodyWorker.MethodHeaderFlags.TinyFormat;
				if (!flag2)
				{
					throw new InvalidOperationException(Resources.InvalidMetadata);
				}
				MetadataOnlyMethodBodyWorker.IMethodHeader methodHeader = new MetadataOnlyMethodBodyWorker.TinyHeader(array[0]);
				result = methodHeader;
			}
			return result;
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x0600021A RID: 538 RVA: 0x00006034 File Offset: 0x00004234
		public override IList<ExceptionHandlingClause> ExceptionHandlingClauses
		{
			get
			{
				bool flag = (this._header.Flags & MetadataOnlyMethodBodyWorker.MethodHeaderFlags.MoreSects) == (MetadataOnlyMethodBodyWorker.MethodHeaderFlags)0;
				IList<ExceptionHandlingClause> result;
				if (flag)
				{
					result = new ExceptionHandlingClause[0];
				}
				else
				{
					MetadataOnlyModule resolver = base.Method.Resolver;
					uint methodRva = resolver.GetMethodRva(base.Method.MetadataToken);
					long num = (long)((ulong)methodRva + (ulong)((long)this._header.HeaderSizeBytes) + (ulong)((long)this._header.CodeSize));
					num = (num - 1L & -4L) + 4L;
					byte b = resolver.RawMetadata.ReadRvaStruct<byte>(num);
					MetadataOnlyMethodBodyWorker.CorILMethod_Sect corILMethod_Sect = (MetadataOnlyMethodBodyWorker.CorILMethod_Sect)b;
					bool flag2 = (corILMethod_Sect & ~(MetadataOnlyMethodBodyWorker.CorILMethod_Sect.EHTable | MetadataOnlyMethodBodyWorker.CorILMethod_Sect.FatFormat)) > (MetadataOnlyMethodBodyWorker.CorILMethod_Sect)0;
					if (flag2)
					{
						throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, Resources.UnsupportedExceptionFlags, new object[]
						{
							corILMethod_Sect
						}));
					}
					bool flag3 = (corILMethod_Sect & MetadataOnlyMethodBodyWorker.CorILMethod_Sect.FatFormat) > (MetadataOnlyMethodBodyWorker.CorILMethod_Sect)0;
					bool flag4 = flag3;
					int num2;
					int num3;
					if (flag4)
					{
						byte[] array = resolver.RawMetadata.ReadRva(num + 1L, 3);
						num2 = (int)array[0] + (int)array[1] * 256 + (int)array[2] * 256 * 256;
						num3 = 24;
					}
					else
					{
						num2 = (int)resolver.RawMetadata.ReadRvaStruct<byte>(num + 1L);
						num3 = 12;
					}
					int num4 = (num2 - 4) / num3;
					ExceptionHandlingClause[] array2 = new MetadataOnlyMethodBodyWorker.ExceptionHandlingClauseWorker[num4];
					long num5 = num + 4L;
					for (int i = 0; i < num4; i++)
					{
						MetadataOnlyMethodBodyWorker.IEHClause iehclause2;
						if (!flag3)
						{
							MetadataOnlyMethodBodyWorker.IEHClause iehclause = resolver.RawMetadata.ReadRvaStruct<MetadataOnlyMethodBodyWorker.EHSmall>(num5);
							iehclause2 = iehclause;
						}
						else
						{
							MetadataOnlyMethodBodyWorker.IEHClause iehclause = resolver.RawMetadata.ReadRvaStruct<MetadataOnlyMethodBodyWorker.EHFat>(num5);
							iehclause2 = iehclause;
						}
						MetadataOnlyMethodBodyWorker.IEHClause data = iehclause2;
						num5 += (long)num3;
						array2[i] = new MetadataOnlyMethodBodyWorker.ExceptionHandlingClauseWorker(base.Method, data);
					}
					result = Array.AsReadOnly<ExceptionHandlingClause>(array2);
				}
				return result;
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600021B RID: 539 RVA: 0x000061D8 File Offset: 0x000043D8
		public override int MaxStackSize
		{
			get
			{
				return this._header.MaxStack;
			}
		}

		// Token: 0x0600021C RID: 540 RVA: 0x000061F8 File Offset: 0x000043F8
		public override byte[] GetILAsByteArray()
		{
			bool flag = this._header.CodeSize == 0;
			byte[] result;
			if (flag)
			{
				result = MetadataOnlyMethodBodyWorker.s_EmptyByteArray;
			}
			else
			{
				MetadataOnlyModule resolver = base.Method.Resolver;
				uint methodRva = resolver.GetMethodRva(base.Method.MetadataToken);
				byte[] array = resolver.RawMetadata.ReadRva((long)((ulong)methodRva + (ulong)((long)this._header.HeaderSizeBytes)), this._header.CodeSize);
				result = array;
			}
			return result;
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600021D RID: 541 RVA: 0x00006270 File Offset: 0x00004470
		public override bool InitLocals
		{
			get
			{
				return (this._header.Flags & MetadataOnlyMethodBodyWorker.MethodHeaderFlags.InitLocals) > (MetadataOnlyMethodBodyWorker.MethodHeaderFlags)0;
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600021E RID: 542 RVA: 0x00006294 File Offset: 0x00004494
		public override int LocalSignatureMetadataToken
		{
			get
			{
				return this._header.LocalVarSigTok.Value;
			}
		}

		// Token: 0x04000078 RID: 120
		private static readonly byte[] s_EmptyByteArray = new byte[0];

		// Token: 0x04000079 RID: 121
		private readonly MetadataOnlyMethodBodyWorker.IMethodHeader _header;

		// Token: 0x0200004E RID: 78
		private class ExceptionHandlingClauseWorker : ExceptionHandlingClause
		{
			// Token: 0x060004B9 RID: 1209 RVA: 0x0000F9C5 File Offset: 0x0000DBC5
			public ExceptionHandlingClauseWorker(MethodInfo method, MetadataOnlyMethodBodyWorker.IEHClause data)
			{
				this._method = method;
				this._data = data;
			}

			// Token: 0x17000122 RID: 290
			// (get) Token: 0x060004BA RID: 1210 RVA: 0x0000F9E0 File Offset: 0x0000DBE0
			public override Type CatchType
			{
				get
				{
					Token classToken = this._data.ClassToken;
					Module module = this._method.Module;
					return module.ResolveType(classToken, this._method.DeclaringType.GetGenericArguments(), this._method.GetGenericArguments());
				}
			}

			// Token: 0x17000123 RID: 291
			// (get) Token: 0x060004BB RID: 1211 RVA: 0x0000FA34 File Offset: 0x0000DC34
			public override int FilterOffset
			{
				get
				{
					return this._data.FilterOffset;
				}
			}

			// Token: 0x17000124 RID: 292
			// (get) Token: 0x060004BC RID: 1212 RVA: 0x0000FA54 File Offset: 0x0000DC54
			public override ExceptionHandlingClauseOptions Flags
			{
				get
				{
					return this._data.Flags;
				}
			}

			// Token: 0x17000125 RID: 293
			// (get) Token: 0x060004BD RID: 1213 RVA: 0x0000FA74 File Offset: 0x0000DC74
			public override int HandlerLength
			{
				get
				{
					return this._data.HandlerLength;
				}
			}

			// Token: 0x17000126 RID: 294
			// (get) Token: 0x060004BE RID: 1214 RVA: 0x0000FA94 File Offset: 0x0000DC94
			public override int HandlerOffset
			{
				get
				{
					return this._data.HandlerOffset;
				}
			}

			// Token: 0x17000127 RID: 295
			// (get) Token: 0x060004BF RID: 1215 RVA: 0x0000FAB4 File Offset: 0x0000DCB4
			public override int TryLength
			{
				get
				{
					return this._data.TryLength;
				}
			}

			// Token: 0x17000128 RID: 296
			// (get) Token: 0x060004C0 RID: 1216 RVA: 0x0000FAD4 File Offset: 0x0000DCD4
			public override int TryOffset
			{
				get
				{
					return this._data.TryOffset;
				}
			}

			// Token: 0x040000FB RID: 251
			private readonly MethodInfo _method;

			// Token: 0x040000FC RID: 252
			private readonly MetadataOnlyMethodBodyWorker.IEHClause _data;
		}

		// Token: 0x0200004F RID: 79
		internal interface IMethodHeader
		{
			// Token: 0x17000129 RID: 297
			// (get) Token: 0x060004C1 RID: 1217
			int MaxStack { get; }

			// Token: 0x1700012A RID: 298
			// (get) Token: 0x060004C2 RID: 1218
			int CodeSize { get; }

			// Token: 0x1700012B RID: 299
			// (get) Token: 0x060004C3 RID: 1219
			Token LocalVarSigTok { get; }

			// Token: 0x1700012C RID: 300
			// (get) Token: 0x060004C4 RID: 1220
			MetadataOnlyMethodBodyWorker.MethodHeaderFlags Flags { get; }

			// Token: 0x1700012D RID: 301
			// (get) Token: 0x060004C5 RID: 1221
			int HeaderSizeBytes { get; }
		}

		// Token: 0x02000050 RID: 80
		[Flags]
		internal enum MethodHeaderFlags
		{
			// Token: 0x040000FE RID: 254
			FatFormat = 3,
			// Token: 0x040000FF RID: 255
			TinyFormat = 2,
			// Token: 0x04000100 RID: 256
			MoreSects = 8,
			// Token: 0x04000101 RID: 257
			InitLocals = 16
		}

		// Token: 0x02000051 RID: 81
		[StructLayout(LayoutKind.Sequential)]
		internal class TinyHeader : MetadataOnlyMethodBodyWorker.IMethodHeader
		{
			// Token: 0x060004C6 RID: 1222 RVA: 0x0000FAF1 File Offset: 0x0000DCF1
			public TinyHeader()
			{
			}

			// Token: 0x060004C7 RID: 1223 RVA: 0x0000FAFB File Offset: 0x0000DCFB
			public TinyHeader(byte data)
			{
				this._flagsAndSize = data;
			}

			// Token: 0x1700012E RID: 302
			// (get) Token: 0x060004C8 RID: 1224 RVA: 0x0000FB0C File Offset: 0x0000DD0C
			public MetadataOnlyMethodBodyWorker.MethodHeaderFlags Flags
			{
				get
				{
					return (MetadataOnlyMethodBodyWorker.MethodHeaderFlags)(this._flagsAndSize & 3);
				}
			}

			// Token: 0x1700012F RID: 303
			// (get) Token: 0x060004C9 RID: 1225 RVA: 0x0000FB28 File Offset: 0x0000DD28
			public int CodeSize
			{
				get
				{
					return this._flagsAndSize >> 2 & 63;
				}
			}

			// Token: 0x17000130 RID: 304
			// (get) Token: 0x060004CA RID: 1226 RVA: 0x0000FB48 File Offset: 0x0000DD48
			public int MaxStack
			{
				get
				{
					return 8;
				}
			}

			// Token: 0x17000131 RID: 305
			// (get) Token: 0x060004CB RID: 1227 RVA: 0x0000FB5C File Offset: 0x0000DD5C
			public Token LocalVarSigTok
			{
				get
				{
					return Token.Nil;
				}
			}

			// Token: 0x17000132 RID: 306
			// (get) Token: 0x060004CC RID: 1228 RVA: 0x0000FB74 File Offset: 0x0000DD74
			public int HeaderSizeBytes
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x04000102 RID: 258
			private readonly byte _flagsAndSize;
		}

		// Token: 0x02000052 RID: 82
		[StructLayout(LayoutKind.Sequential)]
		internal class FatHeader : MetadataOnlyMethodBodyWorker.IMethodHeader
		{
			// Token: 0x17000133 RID: 307
			// (get) Token: 0x060004CD RID: 1229 RVA: 0x0000FB88 File Offset: 0x0000DD88
			public MetadataOnlyMethodBodyWorker.MethodHeaderFlags Flags
			{
				get
				{
					return (MetadataOnlyMethodBodyWorker.MethodHeaderFlags)(this._flagsAndSize & 4095);
				}
			}

			// Token: 0x17000134 RID: 308
			// (get) Token: 0x060004CE RID: 1230 RVA: 0x0000FBA8 File Offset: 0x0000DDA8
			public int MaxStack
			{
				get
				{
					return (int)this._maxStack;
				}
			}

			// Token: 0x17000135 RID: 309
			// (get) Token: 0x060004CF RID: 1231 RVA: 0x0000FBC0 File Offset: 0x0000DDC0
			public int CodeSize
			{
				get
				{
					return (int)this._codeSize;
				}
			}

			// Token: 0x17000136 RID: 310
			// (get) Token: 0x060004D0 RID: 1232 RVA: 0x0000FBD8 File Offset: 0x0000DDD8
			public Token LocalVarSigTok
			{
				get
				{
					return new Token(this._localVarSigTok);
				}
			}

			// Token: 0x17000137 RID: 311
			// (get) Token: 0x060004D1 RID: 1233 RVA: 0x0000FBF8 File Offset: 0x0000DDF8
			public int HeaderSizeBytes
			{
				get
				{
					int num = this._flagsAndSize >> 12 & 15;
					return num * 4;
				}
			}

			// Token: 0x04000103 RID: 259
			private readonly short _flagsAndSize;

			// Token: 0x04000104 RID: 260
			private readonly short _maxStack;

			// Token: 0x04000105 RID: 261
			private readonly uint _codeSize;

			// Token: 0x04000106 RID: 262
			private readonly uint _localVarSigTok;
		}

		// Token: 0x02000053 RID: 83
		[Flags]
		private enum CorILMethod_Sect
		{
			// Token: 0x04000108 RID: 264
			EHTable = 1,
			// Token: 0x04000109 RID: 265
			OptILTable = 2,
			// Token: 0x0400010A RID: 266
			FatFormat = 64,
			// Token: 0x0400010B RID: 267
			MoreSects = 128
		}

		// Token: 0x02000054 RID: 84
		private interface IEHClause
		{
			// Token: 0x17000138 RID: 312
			// (get) Token: 0x060004D3 RID: 1235
			ExceptionHandlingClauseOptions Flags { get; }

			// Token: 0x17000139 RID: 313
			// (get) Token: 0x060004D4 RID: 1236
			int TryOffset { get; }

			// Token: 0x1700013A RID: 314
			// (get) Token: 0x060004D5 RID: 1237
			int TryLength { get; }

			// Token: 0x1700013B RID: 315
			// (get) Token: 0x060004D6 RID: 1238
			int HandlerOffset { get; }

			// Token: 0x1700013C RID: 316
			// (get) Token: 0x060004D7 RID: 1239
			int HandlerLength { get; }

			// Token: 0x1700013D RID: 317
			// (get) Token: 0x060004D8 RID: 1240
			Token ClassToken { get; }

			// Token: 0x1700013E RID: 318
			// (get) Token: 0x060004D9 RID: 1241
			int FilterOffset { get; }
		}

		// Token: 0x02000055 RID: 85
		[StructLayout(LayoutKind.Sequential)]
		internal class EHSmall : MetadataOnlyMethodBodyWorker.IEHClause
		{
			// Token: 0x1700013F RID: 319
			// (get) Token: 0x060004DA RID: 1242 RVA: 0x0000FC1C File Offset: 0x0000DE1C
			ExceptionHandlingClauseOptions MetadataOnlyMethodBodyWorker.IEHClause.Flags
			{
				get
				{
					return (ExceptionHandlingClauseOptions)this._flags;
				}
			}

			// Token: 0x17000140 RID: 320
			// (get) Token: 0x060004DB RID: 1243 RVA: 0x0000FC34 File Offset: 0x0000DE34
			int MetadataOnlyMethodBodyWorker.IEHClause.TryOffset
			{
				get
				{
					return (int)this._tryOffset;
				}
			}

			// Token: 0x17000141 RID: 321
			// (get) Token: 0x060004DC RID: 1244 RVA: 0x0000FC4C File Offset: 0x0000DE4C
			int MetadataOnlyMethodBodyWorker.IEHClause.TryLength
			{
				get
				{
					return (int)this._tryLength;
				}
			}

			// Token: 0x17000142 RID: 322
			// (get) Token: 0x060004DD RID: 1245 RVA: 0x0000FC64 File Offset: 0x0000DE64
			int MetadataOnlyMethodBodyWorker.IEHClause.HandlerOffset
			{
				get
				{
					return (int)this._handlerOffset2 * 256 + (int)this._handlerOffset1;
				}
			}

			// Token: 0x17000143 RID: 323
			// (get) Token: 0x060004DE RID: 1246 RVA: 0x0000FC8C File Offset: 0x0000DE8C
			int MetadataOnlyMethodBodyWorker.IEHClause.HandlerLength
			{
				get
				{
					return (int)this._handlerLength;
				}
			}

			// Token: 0x17000144 RID: 324
			// (get) Token: 0x060004DF RID: 1247 RVA: 0x0000FCA4 File Offset: 0x0000DEA4
			Token MetadataOnlyMethodBodyWorker.IEHClause.ClassToken
			{
				get
				{
					return new Token(this._classToken);
				}
			}

			// Token: 0x17000145 RID: 325
			// (get) Token: 0x060004E0 RID: 1248 RVA: 0x0000FCC1 File Offset: 0x0000DEC1
			int MetadataOnlyMethodBodyWorker.IEHClause.FilterOffset { get; }

			// Token: 0x0400010C RID: 268
			private readonly ushort _flags;

			// Token: 0x0400010D RID: 269
			private readonly ushort _tryOffset;

			// Token: 0x0400010E RID: 270
			private readonly byte _tryLength;

			// Token: 0x0400010F RID: 271
			private readonly byte _handlerOffset1;

			// Token: 0x04000110 RID: 272
			private readonly byte _handlerOffset2;

			// Token: 0x04000111 RID: 273
			private readonly byte _handlerLength;

			// Token: 0x04000112 RID: 274
			private readonly uint _classToken;
		}

		// Token: 0x02000056 RID: 86
		[StructLayout(LayoutKind.Sequential)]
		internal class EHFat : MetadataOnlyMethodBodyWorker.IEHClause
		{
			// Token: 0x17000146 RID: 326
			// (get) Token: 0x060004E2 RID: 1250 RVA: 0x0000FCCC File Offset: 0x0000DECC
			ExceptionHandlingClauseOptions MetadataOnlyMethodBodyWorker.IEHClause.Flags
			{
				get
				{
					return (ExceptionHandlingClauseOptions)this._flags;
				}
			}

			// Token: 0x17000147 RID: 327
			// (get) Token: 0x060004E3 RID: 1251 RVA: 0x0000FCE4 File Offset: 0x0000DEE4
			int MetadataOnlyMethodBodyWorker.IEHClause.TryOffset { get; }

			// Token: 0x17000148 RID: 328
			// (get) Token: 0x060004E4 RID: 1252 RVA: 0x0000FCEC File Offset: 0x0000DEEC
			int MetadataOnlyMethodBodyWorker.IEHClause.TryLength { get; }

			// Token: 0x17000149 RID: 329
			// (get) Token: 0x060004E5 RID: 1253 RVA: 0x0000FCF4 File Offset: 0x0000DEF4
			int MetadataOnlyMethodBodyWorker.IEHClause.HandlerOffset { get; }

			// Token: 0x1700014A RID: 330
			// (get) Token: 0x060004E6 RID: 1254 RVA: 0x0000FCFC File Offset: 0x0000DEFC
			int MetadataOnlyMethodBodyWorker.IEHClause.HandlerLength { get; }

			// Token: 0x1700014B RID: 331
			// (get) Token: 0x060004E7 RID: 1255 RVA: 0x0000FD04 File Offset: 0x0000DF04
			Token MetadataOnlyMethodBodyWorker.IEHClause.ClassToken
			{
				get
				{
					return new Token(this._classToken);
				}
			}

			// Token: 0x1700014C RID: 332
			// (get) Token: 0x060004E8 RID: 1256 RVA: 0x0000FD21 File Offset: 0x0000DF21
			int MetadataOnlyMethodBodyWorker.IEHClause.FilterOffset { get; }

			// Token: 0x04000114 RID: 276
			private readonly uint _flags;

			// Token: 0x04000115 RID: 277
			private readonly uint _classToken;
		}
	}
}
