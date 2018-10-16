using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Security.Cryptography;
using System.IO;

namespace ControlClass
{
    public class CryptClass
    {
        #region DES加解密
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="pubKey">公匙</param>
        /// <param name="input">加密文</param>
        /// <returns></returns>
        public static string DesEncrypt(string pubKey, string input)
        {
            var des = new DESCryptoServiceProvider();
            var bytes = Encoding.UTF8.GetBytes(input);
            des.Key = Encoding.ASCII.GetBytes(pubKey);
            des.IV = Encoding.ASCII.GetBytes(pubKey);
            using (var ms = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.FlushFinalBlock();
                }
                var data = ms.ToArray();
                return BitConverter.ToString(data).Replace("-", "");
            }
        }
        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="pubKey">公匙</param>
        /// <param name="input">解密文</param>
        /// <returns></returns>
        public static string DesDecrypt(string pubKey, string input)
        {
            var des = new DESCryptoServiceProvider();
            var bytes = Hex2Bytes(input);
            des.Key = Encoding.ASCII.GetBytes(pubKey);
            des.IV = Encoding.ASCII.GetBytes(pubKey);
            using (var ms = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.FlushFinalBlock();
                }
                var data = ms.ToArray();
                return Encoding.UTF8.GetString(data);
            }
        }

        /// <summary>
        /// 将16进制数组转换成字节
        /// </summary>
        /// <param name="input">转换字节</param>
        /// <returns>16进制数组</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static byte[] Hex2Bytes(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;
            var offset = input.Length % 2;
            if (offset == 1) input = "0" + input;
            int i;
            var list = new List<byte>();
            for (i = 0; i < input.Length; i += 2)
            {
                var temp = input.Substring(i, 2);
                byte bv;
                var success = byte.TryParse(temp, NumberStyles.HexNumber, null, out bv);
                if (!success) throw new ArgumentOutOfRangeException();
                list.Add(bv);
            }
            return list.ToArray();
        }
        #endregion
        
        #region MD5加密
        /// <summary>
        /// 获取MD5小写字符串
        /// </summary>
        /// <param name="ConvertString"></param>
        /// <returns></returns>
        public static string GetMd5Str(string ConvertString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
            t2 = t2.Replace("-", "");

            t2 = t2.ToLower();

            return t2;
        }
        /// <summary>
        /// 获取MD5大写字符串
        /// </summary>
        /// <param name="ConvertString"></param>
        /// <returns></returns>
        public static string GetMd5STR(string ConvertString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }
        /// <summary>
        /// 获取MD5哈希码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StringToMD5Hash(string value)
        {
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            byte[] encryptedBytes = MD5.ComputeHash(Encoding.ASCII.GetBytes(value));
            StringBuilder strResult = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                strResult.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return strResult.ToString();
        }
        #endregion
    }
}
