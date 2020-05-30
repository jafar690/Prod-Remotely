using System;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Silgred.Shared.Win32
{
    public static class SECUR32
    {
        public enum KERB_LOGON_SUBMIT_TYPE
        {
            KerbInteractiveLogon = 2,
            KerbSmartCardLogon = 6,
            KerbWorkstationUnlockLogon = 7,
            KerbSmartCardUnlockLogon = 8,
            KerbProxyLogon = 9,
            KerbTicketLogon = 10,
            KerbTicketUnlockLogon = 11,
            KerbS4ULogon = 12,
            KerbCertificateLogon = 13,
            KerbCertificateS4ULogon = 14,
            KerbCertificateUnlockLogon = 15
        }

        // SECURITY_LOGON_TYPE
        public enum SecurityLogonType
        {
            Interactive = 2, // Interactively logged on (locally or remotely)
            Network, // Accessing system via network
            Batch, // Started via a batch queue
            Service, // Service started by service controller
            Proxy, // Proxy logon
            Unlock, // Unlock workstation
            NetworkCleartext, // Network logon with cleartext credentials
            NewCredentials, // Clone caller, new default credentials
            RemoteInteractive, // Remote, yet interactive. Terminal server
            CachedInteractive, // Try cached credentials without hitting the net.
            CachedRemoteInteractive, // Same as RemoteInteractive, this is used internally for auditing purpose
            CachedUnlock // Cached Unlock workstation
        }

        public enum TOKEN_INFORMATION_CLASS
        {
            /// <summary>
            ///     The buffer receives a TOKEN_USER structure that contains the user account of the token.
            /// </summary>
            TokenUser = 1,

            /// <summary>
            ///     The buffer receives a TOKEN_GROUPS structure that contains the group accounts associated with the token.
            /// </summary>
            TokenGroups,

            /// <summary>
            ///     The buffer receives a TOKEN_PRIVILEGES structure that contains the privileges of the token.
            /// </summary>
            TokenPrivileges,

            /// <summary>
            ///     The buffer receives a TOKEN_OWNER structure that contains the default owner security identifier (SID) for newly
            ///     created objects.
            /// </summary>
            TokenOwner,

            /// <summary>
            ///     The buffer receives a TOKEN_PRIMARY_GROUP structure that contains the default primary group SID for newly created
            ///     objects.
            /// </summary>
            TokenPrimaryGroup,

            /// <summary>
            ///     The buffer receives a TOKEN_DEFAULT_DACL structure that contains the default DACL for newly created objects.
            /// </summary>
            TokenDefaultDacl,

            /// <summary>
            ///     The buffer receives a TOKEN_SOURCE structure that contains the source of the token. TOKEN_QUERY_SOURCE access is
            ///     needed to retrieve this information.
            /// </summary>
            TokenSource,

            /// <summary>
            ///     The buffer receives a TOKEN_TYPE value that indicates whether the token is a primary or impersonation token.
            /// </summary>
            TokenType,

            /// <summary>
            ///     The buffer receives a SECURITY_IMPERSONATION_LEVEL value that indicates the impersonation level of the token. If
            ///     the access token is not an impersonation token, the function fails.
            /// </summary>
            TokenImpersonationLevel,

            /// <summary>
            ///     The buffer receives a TOKEN_STATISTICS structure that contains various token statistics.
            /// </summary>
            TokenStatistics,

            /// <summary>
            ///     The buffer receives a TOKEN_GROUPS structure that contains the list of restricting SIDs in a restricted token.
            /// </summary>
            TokenRestrictedSids,

            /// <summary>
            ///     The buffer receives a DWORD value that indicates the Terminal Services session identifier that is associated with
            ///     the token.
            /// </summary>
            TokenSessionId,

            /// <summary>
            ///     The buffer receives a TOKEN_GROUPS_AND_PRIVILEGES structure that contains the user SID, the group accounts, the
            ///     restricted SIDs, and the authentication ID associated with the token.
            /// </summary>
            TokenGroupsAndPrivileges,

            /// <summary>
            ///     Reserved.
            /// </summary>
            TokenSessionReference,

            /// <summary>
            ///     The buffer receives a DWORD value that is nonzero if the token includes the SANDBOX_INERT flag.
            /// </summary>
            TokenSandBoxInert,

            /// <summary>
            ///     Reserved.
            /// </summary>
            TokenAuditPolicy,

            /// <summary>
            ///     The buffer receives a TOKEN_ORIGIN value.
            /// </summary>
            TokenOrigin,

            /// <summary>
            ///     The buffer receives a TOKEN_ELEVATION_TYPE value that specifies the elevation level of the token.
            /// </summary>
            TokenElevationType,

            /// <summary>
            ///     The buffer receives a TOKEN_LINKED_TOKEN structure that contains a handle to another token that is linked to this
            ///     token.
            /// </summary>
            TokenLinkedToken,

            /// <summary>
            ///     The buffer receives a TOKEN_ELEVATION structure that specifies whether the token is elevated.
            /// </summary>
            TokenElevation,

            /// <summary>
            ///     The buffer receives a DWORD value that is nonzero if the token has ever been filtered.
            /// </summary>
            TokenHasRestrictions,

            /// <summary>
            ///     The buffer receives a TOKEN_ACCESS_INFORMATION structure that specifies security information contained in the
            ///     token.
            /// </summary>
            TokenAccessInformation,

            /// <summary>
            ///     The buffer receives a DWORD value that is nonzero if virtualization is allowed for the token.
            /// </summary>
            TokenVirtualizationAllowed,

            /// <summary>
            ///     The buffer receives a DWORD value that is nonzero if virtualization is enabled for the token.
            /// </summary>
            TokenVirtualizationEnabled,

            /// <summary>
            ///     The buffer receives a TOKEN_MANDATORY_LABEL structure that specifies the token's integrity level.
            /// </summary>
            TokenIntegrityLevel,

            /// <summary>
            ///     The buffer receives a DWORD value that is nonzero if the token has the UIAccess flag set.
            /// </summary>
            TokenUIAccess,

            /// <summary>
            ///     The buffer receives a TOKEN_MANDATORY_POLICY structure that specifies the token's mandatory integrity policy.
            /// </summary>
            TokenMandatoryPolicy,

            /// <summary>
            ///     The buffer receives the token's logon security identifier (SID).
            /// </summary>
            TokenLogonSid,

            /// <summary>
            ///     The maximum value for this enumeration
            /// </summary>
            MaxTokenInfoClass
        }

        public enum WinErrors : uint
        {
            NO_ERROR = 0
        }

        public enum WinLogonType
        {
            LOGON32_LOGON_INTERACTIVE = 2,
            LOGON32_LOGON_NETWORK = 3,
            LOGON32_LOGON_BATCH = 4,
            LOGON32_LOGON_SERVICE = 5,
            LOGON32_LOGON_UNLOCK = 7,
            LOGON32_LOGON_NETWORK_CLEARTEXT = 8,
            LOGON32_LOGON_NEW_CREDENTIALS = 9
        }

        public enum WinStatusCodes : uint
        {
            STATUS_SUCCESS = 0
        }


        [DllImport("secur32.dll", SetLastError = true)]
        public static extern WinStatusCodes LsaLogonUser(
            [In] IntPtr LsaHandle,
            [In] ref LSA_STRING OriginName,
            [In] SecurityLogonType LogonType,
            [In] uint AuthenticationPackage,
            [In] IntPtr AuthenticationInformation,
            [In] uint AuthenticationInformationLength,
            [In] /*PTOKEN_GROUPS*/ IntPtr LocalGroups,
            [In] ref TOKEN_SOURCE SourceContext,
            [Out] /*PVOID*/ out IntPtr ProfileBuffer,
            [Out] out uint ProfileBufferLength,
            [Out] out long LogonId,
            [Out] out IntPtr Token,
            [Out] out QUOTA_LIMITS Quotas,
            [Out] out WinStatusCodes SubStatus
        );

        [DllImport("secur32.dll", SetLastError = true)]
        public static extern WinStatusCodes LsaRegisterLogonProcess(
            IntPtr LogonProcessName,
            out IntPtr LsaHandle,
            out ulong SecurityMode
        );

        [DllImport("secur32.dll", SetLastError = false)]
        public static extern WinStatusCodes LsaLookupAuthenticationPackage([In] IntPtr LsaHandle,
            [In] ref LSA_STRING PackageName, [Out] out uint AuthenticationPackage);

        [DllImport("secur32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [ResourceExposure(ResourceScope.None)]
        internal static extern int LsaConnectUntrusted(
            [In] [Out] ref SafeLsaLogonProcessHandle LsaHandle);

        [DllImport("secur32.dll", SetLastError = false)]
        public static extern WinStatusCodes LsaConnectUntrusted([Out] out IntPtr LsaHandle);

        [DllImport("secur32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [ResourceExposure(ResourceScope.None)]
        internal static extern int LsaDeregisterLogonProcess(IntPtr handle);


        public static void CreateNewSession()
        {
            var kli = new KERB_INTERACTIVE_LOGON
            {
                MessageType = KERB_LOGON_SUBMIT_TYPE.KerbInteractiveLogon,
                UserName = "",
                Password = ""
            };
            IntPtr pluid;
            IntPtr lsaHan;
            uint authPackID;
            IntPtr kerbLogInfo;
            var logonProc = new LSA_STRING
            {
                Buffer = Marshal.StringToHGlobalAuto("InstaLogon"),
                Length = (ushort) Marshal.SizeOf(Marshal.StringToHGlobalAuto("InstaLogon")),
                MaximumLength = (ushort) Marshal.SizeOf(Marshal.StringToHGlobalAuto("InstaLogon"))
            };
            var originName = new LSA_STRING
            {
                Buffer = Marshal.StringToHGlobalAuto("InstaLogon"),
                Length = (ushort) Marshal.SizeOf(Marshal.StringToHGlobalAuto("InstaLogon")),
                MaximumLength = (ushort) Marshal.SizeOf(Marshal.StringToHGlobalAuto("InstaLogon"))
            };
            var authPackage = new LSA_STRING
            {
                Buffer = Marshal.StringToHGlobalAuto("MICROSOFT_KERBEROS_NAME_A"),
                Length = (ushort) Marshal.SizeOf(Marshal.StringToHGlobalAuto("MICROSOFT_KERBEROS_NAME_A")),
                MaximumLength = (ushort) Marshal.SizeOf(Marshal.StringToHGlobalAuto("MICROSOFT_KERBEROS_NAME_A"))
            };
            var hLogonProc = Marshal.AllocHGlobal(Marshal.SizeOf(logonProc));
            Marshal.StructureToPtr(logonProc, hLogonProc, false);
            ADVAPI32.AllocateLocallyUniqueId(out pluid);
            LsaConnectUntrusted(out lsaHan);
            //SECUR32.LsaRegisterLogonProcess(hLogonProc, out lsaHan, out secMode);
            LsaLookupAuthenticationPackage(lsaHan, ref authPackage, out authPackID);

            kerbLogInfo = Marshal.AllocHGlobal(Marshal.SizeOf(kli));
            Marshal.StructureToPtr(kli, kerbLogInfo, false);

            var ts = new TOKEN_SOURCE("Insta");
            IntPtr profBuf;
            uint profBufLen;
            long logonID;
            IntPtr logonToken;
            QUOTA_LIMITS quotas;
            WinStatusCodes subStatus;
            LsaLogonUser(lsaHan, ref originName, SecurityLogonType.Interactive, authPackID, kerbLogInfo,
                (uint) Marshal.SizeOf(kerbLogInfo), IntPtr.Zero, ref ts, out profBuf, out profBufLen, out logonID,
                out logonToken, out quotas, out subStatus);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LSA_UNICODE_STRING
        {
            public ushort Length;
            public ushort MaximumLength;
            public IntPtr Buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_SOURCE
        {
            public TOKEN_SOURCE(string name)
            {
                SourceName = new byte[8];
                Encoding.GetEncoding(1252).GetBytes(name, 0, name.Length, SourceName, 0);
                if (!ADVAPI32.AllocateLocallyUniqueId(out SourceIdentifier))
                    throw new Win32Exception();
            }

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] SourceName;

            public IntPtr SourceIdentifier;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KERB_INTERACTIVE_LOGON
        {
            public KERB_LOGON_SUBMIT_TYPE MessageType;
            public string LogonDomainName;
            public string UserName;
            public string Password;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct QUOTA_LIMITS
        {
            private readonly uint PagedPoolLimit;
            private readonly uint NonPagedPoolLimit;
            private readonly uint MinimumWorkingSetSize;
            private readonly uint MaximumWorkingSetSize;
            private readonly uint PagefileLimit;
            private readonly long TimeLimit;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LSA_STRING
        {
            public ushort Length;
            public ushort MaximumLength;
            public /*PCHAR*/ IntPtr Buffer;
        }

        [SecurityCritical] // auto-generated
        internal sealed class SafeLsaLogonProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeLsaLogonProcessHandle() : base(true)
            {
            }

            // 0 is an Invalid Handle
            internal SafeLsaLogonProcessHandle(IntPtr handle) : base(true)
            {
                SetHandle(handle);
            }

            internal static SafeLsaLogonProcessHandle InvalidHandle => new SafeLsaLogonProcessHandle(IntPtr.Zero);

            [SecurityCritical]
            protected override bool ReleaseHandle()
            {
                // LsaDeregisterLogonProcess returns an NTSTATUS
                return LsaDeregisterLogonProcess(handle) >= 0;
            }
        }
    }
}