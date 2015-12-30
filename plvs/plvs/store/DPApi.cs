using System;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Atlassian.plvs.store {
    public class DPApi {
        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool CryptProtectData(ref DATA_BLOB pPlainText,
                                                    string szDescription,
                                                    ref DATA_BLOB pEntropy,
                                                    IntPtr pReserved,
                                                    ref CRYPTPROTECT_PROMPTSTRUCT pPrompt,
                                                    int dwFlags,
                                                    ref DATA_BLOB pCipherText);

        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool CryptUnprotectData(ref DATA_BLOB pCipherText,
                                                      ref string pszDescription,
                                                      ref DATA_BLOB pEntropy,
                                                      IntPtr pReserved,
                                                      ref CRYPTPROTECT_PROMPTSTRUCT pPrompt,
                                                      int dwFlags,
                                                      ref DATA_BLOB pPlainText);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct DATA_BLOB {
            public int cbData;
            public IntPtr pbData;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct CRYPTPROTECT_PROMPTSTRUCT {
            public int cbSize;
            public int dwPromptFlags;
            public IntPtr hwndApp;
            public string szPrompt;
        }

        private const int CRYPTPROTECT_UI_FORBIDDEN = 0x1;

//        private static readonly byte[] EntropyBytes = Encoding.UTF8.GetBytes("some entropy bytes");

        private static void initPrompt(ref CRYPTPROTECT_PROMPTSTRUCT ps) {
            ps.cbSize = Marshal.SizeOf(typeof (CRYPTPROTECT_PROMPTSTRUCT));
            ps.dwPromptFlags = 0;
            ps.hwndApp = ((IntPtr) 0);
            ps.szPrompt = null;
        }

        private static void initBlob(byte[] data, ref DATA_BLOB blob) {
            if (data == null)
                data = new byte[0];

            blob.pbData = Marshal.AllocHGlobal(data.Length);
            if (blob.pbData == IntPtr.Zero)
                throw new Exception("Unable to allocate data buffer");
            blob.cbData = data.Length;
            Marshal.Copy(data, 0, blob.pbData, data.Length);
        }

        public static string encrypt(string plainText, string entropy) {

            byte[] txtBytes = Encoding.UTF8.GetBytes(plainText);

            DATA_BLOB txtBlob = new DATA_BLOB();
            DATA_BLOB cipherBlob = new DATA_BLOB();
            DATA_BLOB entropyBlob = new DATA_BLOB();

            CRYPTPROTECT_PROMPTSTRUCT prompt = new CRYPTPROTECT_PROMPTSTRUCT();
            initPrompt(ref prompt);

            try {
                initBlob(txtBytes, ref txtBlob);
                initBlob(Encoding.UTF8.GetBytes(entropy), ref entropyBlob);

                const int flags = CRYPTPROTECT_UI_FORBIDDEN;

                bool success = CryptProtectData(ref txtBlob, String.Empty, ref entropyBlob, IntPtr.Zero, ref prompt, flags, ref cipherBlob);
                if (!success) {
                    int errCode = Marshal.GetLastWin32Error();
                    throw new Exception("CryptProtectData failed.", new Win32Exception(errCode));
                }

                byte[] cipherTextBytes = new byte[cipherBlob.cbData];
                Marshal.Copy(cipherBlob.pbData, cipherTextBytes, 0, cipherBlob.cbData);

                return Convert.ToBase64String(cipherTextBytes);
            }
            catch (Exception ex) {
                throw new Exception("Error encrypting data", ex);
            }
            finally {
                if (txtBlob.pbData != IntPtr.Zero) Marshal.FreeHGlobal(txtBlob.pbData);
                if (cipherBlob.pbData != IntPtr.Zero) Marshal.FreeHGlobal(cipherBlob.pbData);
                if (entropyBlob.pbData != IntPtr.Zero) Marshal.FreeHGlobal(entropyBlob.pbData);
            }
        }

        public static string decrypt(string cipherText, string entropy) {

            DATA_BLOB txtBlob = new DATA_BLOB();
            DATA_BLOB cipherBlob = new DATA_BLOB();
            DATA_BLOB entropyBlob = new DATA_BLOB();

            CRYPTPROTECT_PROMPTSTRUCT prompt = new CRYPTPROTECT_PROMPTSTRUCT();
            initPrompt(ref prompt);

            string description = String.Empty;

            try {
                initBlob(Convert.FromBase64String(cipherText), ref cipherBlob);
                initBlob(Encoding.UTF8.GetBytes(entropy), ref entropyBlob);

                const int flags = CRYPTPROTECT_UI_FORBIDDEN;

                bool success = CryptUnprotectData(ref cipherBlob, ref description, ref entropyBlob, IntPtr.Zero, ref prompt, flags, ref txtBlob);
                if (!success) {
                    int errCode = Marshal.GetLastWin32Error();
                    throw new Exception("CryptUnprotectData failed.", new Win32Exception(errCode));
                }

                byte[] plainTextBytes = new byte[txtBlob.cbData];
                Marshal.Copy(txtBlob.pbData, plainTextBytes, 0, txtBlob.cbData);

                return Encoding.UTF8.GetString(plainTextBytes);
            }
            catch (Exception ex) {
                throw new Exception("Error decrypting data", ex);
            }
            finally {
                if (txtBlob.pbData != IntPtr.Zero) Marshal.FreeHGlobal(txtBlob.pbData);
                if (cipherBlob.pbData != IntPtr.Zero) Marshal.FreeHGlobal(cipherBlob.pbData);
                if (entropyBlob.pbData != IntPtr.Zero) Marshal.FreeHGlobal(entropyBlob.pbData);
            }
        }
    }
}