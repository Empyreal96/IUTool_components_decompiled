using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200000C RID: 12
	public class AccessRightHelper
	{
		// Token: 0x06000025 RID: 37 RVA: 0x00002AA0 File Offset: 0x00000CA0
		public static AccessRightHelper.GenericMapping? GetGenericMapping(string resourceType)
		{
			if (AccessRightHelper.genericAccessTable.ContainsKey(resourceType))
			{
				return new AccessRightHelper.GenericMapping?(AccessRightHelper.genericAccessTable[resourceType]);
			}
			return null;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002AD4 File Offset: 0x00000CD4
		public static uint MergeAccessRight(string accessRightsString, string resourcePath, string resourceType)
		{
			uint num = 0U;
			int num2 = accessRightsString.IndexOf("0X", StringComparison.OrdinalIgnoreCase);
			if (num2 >= 0)
			{
				if (num2 > 0 || accessRightsString.Length > 10)
				{
					throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "The rule on resource '{0}' has the access rights '{1}' which has mixed Hex format and string format of access right.", new object[]
					{
						resourcePath,
						accessRightsString
					}));
				}
				try
				{
					num = uint.Parse(accessRightsString.Substring(2), NumberStyles.HexNumber, GlobalVariables.Culture);
					goto IL_ED;
				}
				catch (FormatException originalException)
				{
					throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "The rule on resource '{0}' has the access rights '{1}' which can't be converted to a integer.", new object[]
					{
						resourcePath,
						accessRightsString
					}), originalException);
				}
			}
			for (int i = 0; i < accessRightsString.Length; i += 2)
			{
				string text = accessRightsString.Substring(i, 2);
				if (!AccessRightHelper.accessRightsTable.ContainsKey(text))
				{
					throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "The rule on resource '{0}' has the access rights '{1}' which has a invalid access right '{2}'.", new object[]
					{
						resourcePath,
						accessRightsString,
						text
					}));
				}
				num |= AccessRightHelper.accessRightsTable[text];
			}
			IL_ED:
			AccessRightHelper.GenericMapping? genericMapping = AccessRightHelper.GetGenericMapping(resourceType);
			if (genericMapping != null)
			{
				if ((num & 2147483648U) != 0U)
				{
					num |= genericMapping.Value.GenericRead;
				}
				if ((num & 1073741824U) != 0U)
				{
					num |= genericMapping.Value.GenericWrite;
				}
				if ((num & 536870912U) != 0U)
				{
					num |= genericMapping.Value.GenericExecute;
				}
				if ((num & 268435456U) != 0U)
				{
					num |= genericMapping.Value.GenericAll;
				}
			}
			return num;
		}

		// Token: 0x040000B4 RID: 180
		private static readonly Dictionary<string, AccessRightHelper.GenericMapping> genericAccessTable = new Dictionary<string, AccessRightHelper.GenericMapping>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"Mutex",
				new AccessRightHelper.GenericMapping(131073U, 131072U, 1179648U, 2031617U)
			},
			{
				"Semaphore",
				new AccessRightHelper.GenericMapping(131073U, 131074U, 1179648U, 2031619U)
			},
			{
				"WaitableTimer",
				new AccessRightHelper.GenericMapping(131073U, 131074U, 1179648U, 2031619U)
			},
			{
				"AlpcPort",
				new AccessRightHelper.GenericMapping(131073U, 65537U, 0U, 2031617U)
			},
			{
				"Rpc",
				new AccessRightHelper.GenericMapping(2147483648U, 1073741824U, 536870912U, 268435456U)
			},
			{
				"Private",
				new AccessRightHelper.GenericMapping(2147483648U, 1073741824U, 536870912U, 268435456U)
			},
			{
				"Template",
				new AccessRightHelper.GenericMapping(2147483648U, 1073741824U, 536870912U, 268435456U)
			},
			{
				"WNF",
				new AccessRightHelper.GenericMapping(1179649U, 2U, 2031616U, 2031619U)
			},
			{
				"SDRegValue",
				new AccessRightHelper.GenericMapping(2147483648U, 1073741824U, 536870912U, 268435456U)
			},
			{
				"File",
				new AccessRightHelper.GenericMapping(1179785U, 1179926U, 1179808U, 2032127U)
			},
			{
				"Directory",
				new AccessRightHelper.GenericMapping(1179785U, 1179926U, 1179808U, 2032127U)
			},
			{
				"RegKey",
				new AccessRightHelper.GenericMapping(131097U, 131078U, 131097U, 983103U)
			},
			{
				"ServiceAccess",
				new AccessRightHelper.GenericMapping(131213U, 131074U, 131440U, 983551U)
			}
		};

		// Token: 0x040000B5 RID: 181
		private static readonly Dictionary<string, uint> accessRightsTable = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"CC",
				1U
			},
			{
				"DC",
				2U
			},
			{
				"LC",
				4U
			},
			{
				"SW",
				8U
			},
			{
				"RP",
				16U
			},
			{
				"WP",
				32U
			},
			{
				"DT",
				64U
			},
			{
				"LO",
				128U
			},
			{
				"CR",
				256U
			},
			{
				"SD",
				65536U
			},
			{
				"RC",
				131072U
			},
			{
				"WD",
				262144U
			},
			{
				"WO",
				524288U
			},
			{
				"GA",
				268435456U
			},
			{
				"GR",
				2147483648U
			},
			{
				"GW",
				1073741824U
			},
			{
				"GX",
				536870912U
			},
			{
				"FA",
				2032127U
			},
			{
				"FR",
				1179785U
			},
			{
				"FW",
				1179926U
			},
			{
				"FX",
				1179808U
			},
			{
				"KA",
				983103U
			},
			{
				"KR",
				131097U
			},
			{
				"KW",
				131078U
			},
			{
				"KX",
				131097U
			}
		};

		// Token: 0x02000048 RID: 72
		public enum AccessType : uint
		{
			// Token: 0x04000152 RID: 338
			DELETE = 65536U,
			// Token: 0x04000153 RID: 339
			READ_CONTROL = 131072U,
			// Token: 0x04000154 RID: 340
			WRITE_DAC = 262144U,
			// Token: 0x04000155 RID: 341
			WRITE_OWNER = 524288U,
			// Token: 0x04000156 RID: 342
			SYNCHRONIZE = 1048576U,
			// Token: 0x04000157 RID: 343
			STANDARD_RIGHTS_REQUIRED = 983040U,
			// Token: 0x04000158 RID: 344
			STANDARD_RIGHTS_READ = 131072U,
			// Token: 0x04000159 RID: 345
			STANDARD_RIGHTS_WRITE = 131072U,
			// Token: 0x0400015A RID: 346
			STANDARD_RIGHTS_EXECUTE = 131072U,
			// Token: 0x0400015B RID: 347
			STANDARD_RIGHTS_ALL = 2031616U,
			// Token: 0x0400015C RID: 348
			SPECIFIC_RIGHTS_ALL = 65535U,
			// Token: 0x0400015D RID: 349
			ACCESS_SYSTEM_SECURITY = 16777216U,
			// Token: 0x0400015E RID: 350
			GENERIC_READ = 2147483648U,
			// Token: 0x0400015F RID: 351
			GENERIC_WRITE = 1073741824U,
			// Token: 0x04000160 RID: 352
			GENERIC_EXECUTE = 536870912U,
			// Token: 0x04000161 RID: 353
			GENERIC_ALL = 268435456U
		}

		// Token: 0x02000049 RID: 73
		public enum SyncObjectsAccessType : uint
		{
			// Token: 0x04000163 RID: 355
			EVENT_MODIFY_STATE = 2U,
			// Token: 0x04000164 RID: 356
			EVENT_ALL_ACCESS = 2031619U,
			// Token: 0x04000165 RID: 357
			MUTANT_QUERY_STATE = 1U,
			// Token: 0x04000166 RID: 358
			MUTANT_ALL_ACCESS = 2031617U,
			// Token: 0x04000167 RID: 359
			SEMAPHORE_QUERY_STATE = 1U,
			// Token: 0x04000168 RID: 360
			SEMAPHORE_MODIFY_STATE,
			// Token: 0x04000169 RID: 361
			SEMAPHORE_ALL_ACCESS = 2031619U,
			// Token: 0x0400016A RID: 362
			TIMER_QUERY_STATE = 1U,
			// Token: 0x0400016B RID: 363
			TIMER_MODIFY_STATE,
			// Token: 0x0400016C RID: 364
			TIMER_ALL_ACCESS = 2031619U,
			// Token: 0x0400016D RID: 365
			WNF_STATE_SUBSCRIBE = 1U,
			// Token: 0x0400016E RID: 366
			WNF_STATE_PUBLISH,
			// Token: 0x0400016F RID: 367
			WNF_STATE_CROSS_SCOPE_ACCESS = 16U,
			// Token: 0x04000170 RID: 368
			PORT_CONNECT = 1U,
			// Token: 0x04000171 RID: 369
			PORT_ALL_ACCESS = 2031617U,
			// Token: 0x04000172 RID: 370
			JOB_OBJECT_ASSIGN_PROCESS = 1U,
			// Token: 0x04000173 RID: 371
			JOB_OBJECT_SET_ATTRIBUTES,
			// Token: 0x04000174 RID: 372
			JOB_OBJECT_QUERY = 4U,
			// Token: 0x04000175 RID: 373
			JOB_OBJECT_TERMINATE = 8U,
			// Token: 0x04000176 RID: 374
			JOB_OBJECT_SET_SECURITY_ATTRIBUTES = 16U,
			// Token: 0x04000177 RID: 375
			JOB_OBJECT_ALL_ACCESS = 2031647U,
			// Token: 0x04000178 RID: 376
			FILE_MAP_QUERY = 1U,
			// Token: 0x04000179 RID: 377
			FILE_MAP_WRITE,
			// Token: 0x0400017A RID: 378
			FILE_MAP_READ = 4U,
			// Token: 0x0400017B RID: 379
			FILE_MAP_EXECUTE = 32U,
			// Token: 0x0400017C RID: 380
			FILE_MAP_ALL_ACCESS = 983055U
		}

		// Token: 0x0200004A RID: 74
		public enum FileAccessType : uint
		{
			// Token: 0x0400017E RID: 382
			FILE_READ_DATA = 1U,
			// Token: 0x0400017F RID: 383
			FILE_WRITE_DATA,
			// Token: 0x04000180 RID: 384
			FILE_APPEND_DATA = 4U,
			// Token: 0x04000181 RID: 385
			FILE_READ_EA = 8U,
			// Token: 0x04000182 RID: 386
			FILE_WRITE_EA = 16U,
			// Token: 0x04000183 RID: 387
			FILE_EXECUTE = 32U,
			// Token: 0x04000184 RID: 388
			FILE_READ_ATTRIBUTES = 128U,
			// Token: 0x04000185 RID: 389
			FILE_WRITE_ATTRIBUTES = 256U,
			// Token: 0x04000186 RID: 390
			FILE_GENERIC_READ = 1179785U,
			// Token: 0x04000187 RID: 391
			FILE_GENERIC_WRITE = 1179926U,
			// Token: 0x04000188 RID: 392
			FILE_GENERIC_EXECUTE = 1179808U,
			// Token: 0x04000189 RID: 393
			FILE_ALL_ACCESS = 2032127U
		}

		// Token: 0x0200004B RID: 75
		public enum RegistryAccessType : uint
		{
			// Token: 0x0400018B RID: 395
			KEY_QUERY_VALUE = 1U,
			// Token: 0x0400018C RID: 396
			KEY_SET_VALUE,
			// Token: 0x0400018D RID: 397
			KEY_CREATE_SUB_KEY = 4U,
			// Token: 0x0400018E RID: 398
			KEY_ENUMERATE_SUB_KEYS = 8U,
			// Token: 0x0400018F RID: 399
			KEY_NOTIFY = 16U,
			// Token: 0x04000190 RID: 400
			KEY_CREATE_LINK = 32U,
			// Token: 0x04000191 RID: 401
			KEY_READ = 131097U,
			// Token: 0x04000192 RID: 402
			KEY_EXECUTE = 131097U,
			// Token: 0x04000193 RID: 403
			KEY_WRITE = 131078U,
			// Token: 0x04000194 RID: 404
			KEY_ALL_ACCESS = 983103U
		}

		// Token: 0x0200004C RID: 76
		public enum ServiceAccessType : uint
		{
			// Token: 0x04000196 RID: 406
			SERVICE_QUERY_CONFIG = 1U,
			// Token: 0x04000197 RID: 407
			SERVICE_CHANGE_CONFIG,
			// Token: 0x04000198 RID: 408
			SERVICE_QUERY_STATUS = 4U,
			// Token: 0x04000199 RID: 409
			SERVICE_ENUMERATE_DEPENDENTS = 8U,
			// Token: 0x0400019A RID: 410
			SERVICE_START = 16U,
			// Token: 0x0400019B RID: 411
			SERVICE_STOP = 32U,
			// Token: 0x0400019C RID: 412
			SERVICE_PAUSE_CONTINUE = 64U,
			// Token: 0x0400019D RID: 413
			SERVICE_INTERROGATE = 128U,
			// Token: 0x0400019E RID: 414
			SERVICE_USER_DEFINED_CONTROL = 256U,
			// Token: 0x0400019F RID: 415
			SERVICE_ALL_ACCESS = 983551U
		}

		// Token: 0x0200004D RID: 77
		public struct GenericMapping
		{
			// Token: 0x0600023F RID: 575 RVA: 0x0000A7D4 File Offset: 0x000089D4
			public GenericMapping(uint GenericRead, uint GenericWrite, uint GenericExecute, uint GenericAll)
			{
				this.GenericRead = GenericRead;
				this.GenericWrite = GenericWrite;
				this.GenericExecute = GenericExecute;
				this.GenericAll = GenericAll;
			}

			// Token: 0x040001A0 RID: 416
			public readonly uint GenericRead;

			// Token: 0x040001A1 RID: 417
			public readonly uint GenericWrite;

			// Token: 0x040001A2 RID: 418
			public readonly uint GenericExecute;

			// Token: 0x040001A3 RID: 419
			public readonly uint GenericAll;
		}
	}
}
