using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace IDMActClean.Core
{
    public static class ExportHelper
    {
        public static string ExportToFile(IdmRegistrationInfo info)
        {
            string json = BuildJson(info);
            byte[] plaintext = Encoding.UTF8.GetBytes(json);

            byte[] key = DeriveKey();
            byte[] iv  = GenerateIV();
            byte[] cipher = Encrypt(plaintext, key, iv);

            // format: [IV 16 bytes] + [ciphertext]
            byte[] blob = new byte[iv.Length + cipher.Length];
            Buffer.BlockCopy(iv, 0, blob, 0, iv.Length);
            Buffer.BlockCopy(cipher, 0, blob, iv.Length, cipher.Length);

            using (var dlg = new SaveFileDialog())
            {
                dlg.Title      = "Xuất thông tin IDM";
                dlg.Filter     = "IDM Backup (*.idmbak)|*.idmbak";
                dlg.FileName   = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.idmbak";
                dlg.DefaultExt = "idmbak";

                if (dlg.ShowDialog() != DialogResult.OK) return null;

                File.WriteAllBytes(dlg.FileName, blob);
                return dlg.FileName;
            }
        }

        private static string BuildJson(IdmRegistrationInfo info)
        {
            // Simple JSON without extra dependencies
            return "{\r\n" +
                $"  \"FName\": \"{Escape(info.FName)}\",\r\n" +
                $"  \"LName\": \"{Escape(info.LName)}\",\r\n" +
                $"  \"Email\": \"{Escape(info.Email)}\",\r\n" +
                $"  \"Serial\": \"{Escape(info.Serial)}\",\r\n" +
                $"  \"ExportedAt\": \"{DateTime.Now:O}\",\r\n" +
                $"  \"Machine\": \"{Escape(Environment.MachineName)}\"\r\n" +
                "}";
        }

        private static string Escape(string s) =>
            s?.Replace("\\", "\\\\").Replace("\"", "\\\"") ?? "";

        private static byte[] DeriveKey()
        {
            string seed = Environment.MachineName + Environment.UserName;
            using (var sha = SHA256.Create())
                return sha.ComputeHash(Encoding.UTF8.GetBytes(seed));
        }

        private static byte[] GenerateIV()
        {
            byte[] iv = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(iv);
            return iv;
        }

        private static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.Key     = key;
                aes.IV      = iv;
                aes.Mode    = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }
    }
}
