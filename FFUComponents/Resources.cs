using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace FFUComponents
{
	// Token: 0x0200004B RID: 75
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x06000225 RID: 549 RVA: 0x000034AC File Offset: 0x000016AC
		internal Resources()
		{
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000226 RID: 550 RVA: 0x00009D85 File Offset: 0x00007F85
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("FFUComponents.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000227 RID: 551 RVA: 0x00009DB1 File Offset: 0x00007FB1
		// (set) Token: 0x06000228 RID: 552 RVA: 0x00009DB8 File Offset: 0x00007FB8
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000229 RID: 553 RVA: 0x00009DC0 File Offset: 0x00007FC0
		internal static byte[] bootsdi
		{
			get
			{
				return (byte[])Resources.ResourceManager.GetObject("bootsdi", Resources.resourceCulture);
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600022A RID: 554 RVA: 0x00009DDB File Offset: 0x00007FDB
		internal static string ERROR_ACQUIRE_MUTEX
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_ACQUIRE_MUTEX", Resources.resourceCulture);
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600022B RID: 555 RVA: 0x00009DF1 File Offset: 0x00007FF1
		internal static string ERROR_ALREADY_RECEIVED_DATA
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_ALREADY_RECEIVED_DATA", Resources.resourceCulture);
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x0600022C RID: 556 RVA: 0x00009E07 File Offset: 0x00008007
		internal static string ERROR_BINDHANDLE
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_BINDHANDLE", Resources.resourceCulture);
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x0600022D RID: 557 RVA: 0x00009E1D File Offset: 0x0000801D
		internal static string ERROR_CALLBACK_TIMEOUT
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_CALLBACK_TIMEOUT", Resources.resourceCulture);
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x0600022E RID: 558 RVA: 0x00009E33 File Offset: 0x00008033
		internal static string ERROR_CM_GET_DEVICE_ID
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_CM_GET_DEVICE_ID", Resources.resourceCulture);
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x0600022F RID: 559 RVA: 0x00009E49 File Offset: 0x00008049
		internal static string ERROR_CM_GET_DEVICE_ID_SIZE
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_CM_GET_DEVICE_ID_SIZE", Resources.resourceCulture);
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000230 RID: 560 RVA: 0x00009E5F File Offset: 0x0000805F
		internal static string ERROR_CM_GET_PARENT
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_CM_GET_PARENT", Resources.resourceCulture);
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000231 RID: 561 RVA: 0x00009E75 File Offset: 0x00008075
		internal static string ERROR_DEVICE_IO_CONTROL
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_DEVICE_IO_CONTROL", Resources.resourceCulture);
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000232 RID: 562 RVA: 0x00009E8B File Offset: 0x0000808B
		internal static string ERROR_FFUMANAGER_NOT_STARTED
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_FFUMANAGER_NOT_STARTED", Resources.resourceCulture);
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000233 RID: 563 RVA: 0x00009EA1 File Offset: 0x000080A1
		internal static string ERROR_FILE_CLOSED
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_FILE_CLOSED", Resources.resourceCulture);
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000234 RID: 564 RVA: 0x00009EB7 File Offset: 0x000080B7
		internal static string ERROR_FLASH
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_FLASH", Resources.resourceCulture);
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000235 RID: 565 RVA: 0x00009ECD File Offset: 0x000080CD
		internal static string ERROR_INVALID_DATA
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_INVALID_DATA", Resources.resourceCulture);
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000236 RID: 566 RVA: 0x00009EE3 File Offset: 0x000080E3
		internal static string ERROR_INVALID_DEVICE_PARAMS
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_INVALID_DEVICE_PARAMS", Resources.resourceCulture);
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000237 RID: 567 RVA: 0x00009EF9 File Offset: 0x000080F9
		internal static string ERROR_INVALID_ENDPOINT_TYPE
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_INVALID_ENDPOINT_TYPE", Resources.resourceCulture);
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000238 RID: 568 RVA: 0x00009F0F File Offset: 0x0000810F
		internal static string ERROR_INVALID_HANDLE
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_INVALID_HANDLE", Resources.resourceCulture);
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000239 RID: 569 RVA: 0x00009F25 File Offset: 0x00008125
		internal static string ERROR_MULTIPE_DISCONNECT_NOTIFICATIONS
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_MULTIPE_DISCONNECT_NOTIFICATIONS", Resources.resourceCulture);
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x0600023A RID: 570 RVA: 0x00009F3B File Offset: 0x0000813B
		internal static string ERROR_NULL_OR_EMPTY_STRING
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_NULL_OR_EMPTY_STRING", Resources.resourceCulture);
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600023B RID: 571 RVA: 0x00009F51 File Offset: 0x00008151
		internal static string ERROR_RECONNECT_TIMEOUT
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_RECONNECT_TIMEOUT", Resources.resourceCulture);
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600023C RID: 572 RVA: 0x00009F67 File Offset: 0x00008167
		internal static string ERROR_RESULT_ALREADY_SET
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_RESULT_ALREADY_SET", Resources.resourceCulture);
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600023D RID: 573 RVA: 0x00009F7D File Offset: 0x0000817D
		internal static string ERROR_RESUME_UNEXPECTED_POSITION
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_RESUME_UNEXPECTED_POSITION", Resources.resourceCulture);
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600023E RID: 574 RVA: 0x00009F93 File Offset: 0x00008193
		internal static string ERROR_SETUP_DI_ENUM_DEVICE_INFO
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_SETUP_DI_ENUM_DEVICE_INFO", Resources.resourceCulture);
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600023F RID: 575 RVA: 0x00009FA9 File Offset: 0x000081A9
		internal static string ERROR_SETUP_DI_ENUM_DEVICE_INTERFACES
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_SETUP_DI_ENUM_DEVICE_INTERFACES", Resources.resourceCulture);
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000240 RID: 576 RVA: 0x00009FBF File Offset: 0x000081BF
		internal static string ERROR_SETUP_DI_GET_DEVICE_INTERFACE_DETAIL_W
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_SETUP_DI_GET_DEVICE_INTERFACE_DETAIL_W", Resources.resourceCulture);
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000241 RID: 577 RVA: 0x00009FD5 File Offset: 0x000081D5
		internal static string ERROR_SETUP_DI_GET_DEVICE_PROPERTY
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_SETUP_DI_GET_DEVICE_PROPERTY", Resources.resourceCulture);
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000242 RID: 578 RVA: 0x00009FEB File Offset: 0x000081EB
		internal static string ERROR_UNABLE_TO_COMPLETE_WRITE
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_UNABLE_TO_COMPLETE_WRITE", Resources.resourceCulture);
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000243 RID: 579 RVA: 0x0000A001 File Offset: 0x00008201
		internal static string ERROR_UNABLE_TO_READ_REGION
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_UNABLE_TO_READ_REGION", Resources.resourceCulture);
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000244 RID: 580 RVA: 0x0000A017 File Offset: 0x00008217
		internal static string ERROR_UNRECOGNIZED_COMMAND
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_UNRECOGNIZED_COMMAND", Resources.resourceCulture);
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000245 RID: 581 RVA: 0x0000A02D File Offset: 0x0000822D
		internal static string ERROR_USB_TRANSFER
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_USB_TRANSFER", Resources.resourceCulture);
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000246 RID: 582 RVA: 0x0000A043 File Offset: 0x00008243
		internal static string ERROR_WIMBOOT
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_WIMBOOT", Resources.resourceCulture);
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000247 RID: 583 RVA: 0x0000A059 File Offset: 0x00008259
		internal static string ERROR_WINUSB_INITIALIZATION
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_WINUSB_INITIALIZATION", Resources.resourceCulture);
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000248 RID: 584 RVA: 0x0000A06F File Offset: 0x0000826F
		internal static string ERROR_WINUSB_QUERY_INTERFACE_SETTING
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_WINUSB_QUERY_INTERFACE_SETTING", Resources.resourceCulture);
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000249 RID: 585 RVA: 0x0000A085 File Offset: 0x00008285
		internal static string ERROR_WINUSB_QUERY_PIPE_INFORMATION
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_WINUSB_QUERY_PIPE_INFORMATION", Resources.resourceCulture);
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600024A RID: 586 RVA: 0x0000A09B File Offset: 0x0000829B
		internal static string ERROR_WINUSB_SET_PIPE_POLICY
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_WINUSB_SET_PIPE_POLICY", Resources.resourceCulture);
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600024B RID: 587 RVA: 0x0000A0B1 File Offset: 0x000082B1
		internal static string MODULE_VERSION
		{
			get
			{
				return Resources.ResourceManager.GetString("MODULE_VERSION", Resources.resourceCulture);
			}
		}

		// Token: 0x04000171 RID: 369
		private static ResourceManager resourceMan;

		// Token: 0x04000172 RID: 370
		private static CultureInfo resourceCulture;
	}
}
