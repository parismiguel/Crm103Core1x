using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TemplateCoreParis.Helpers
{
    public class Helpers
    {
        public static IHostingEnvironment _wwwRoot;

        public Helpers(IHostingEnvironment environment)
        {
            _wwwRoot = environment;
        }

        public static object LoadJson(string filename)
        {
            string filePath = Path.Combine(_wwwRoot.WebRootPath, "data", filename);

            string text;
           
            //return File.ReadAllText(filePath);


            using (StreamReader reader = File.OpenText(filePath))
            {
                text = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject(text);
        }

        public static string PhoneFormatter(string phone)
        {
            var myPhone = phone.Replace("(", string.Empty);
            myPhone = myPhone.Replace(")", string.Empty);
            myPhone = myPhone.Replace("-", string.Empty);
            myPhone = myPhone.Replace(" ", string.Empty);

            return myPhone;
        }

        public static string EncryptString(string text, string keyString)
        {
            try
            {
                var key = Encoding.UTF8.GetBytes(keyString);

                using (var aesAlg = Aes.Create())
                {
                    using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                    {
                        using (var msEncrypt = new MemoryStream())
                        {
                            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(text);
                            }

                            var iv = aesAlg.IV;

                            var decryptedContent = msEncrypt.ToArray();

                            var result = new byte[iv.Length + decryptedContent.Length];

                            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                            return Convert.ToBase64String(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return string.Empty;
            }


        }

        public static string DecryptString(string cipherText, string keyString)
        {
            try
            {
                var fullCipher = Convert.FromBase64String(cipherText);

                var iv = new byte[16];
                var cipher = new byte[16];

                Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
                var key = Encoding.UTF8.GetBytes(keyString);

                using (var aesAlg = Aes.Create())
                {
                    using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                    {
                        string result;
                        using (var msDecrypt = new MemoryStream(cipher))
                        {
                            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (var srDecrypt = new StreamReader(csDecrypt))
                                {
                                    result = srDecrypt.ReadToEnd();
                                }
                            }
                        }

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return string.Empty;
            }


        }

        public static string Base64ForUrlEncode(string str)
        {

            var encbuff = Encoding.UTF8.GetBytes(str);
            //var result = WebUtility.UrlDecode(encbuff);

            return encbuff.ToString();
        }

        public static string Base64ForUrlDecode(string str)
        {
            return str != null ? WebUtility.UrlDecode(str) : null;
        }

        public static string GetRandomLine(string filename)
        {
            Random _rand = new Random();

            var lines = File.ReadAllLines(filename);
            var lineNumber = _rand.Next(0, lines.Length);

            return lines[lineNumber];
        }

        public static string CheckModelStateErrors(ModelStateDictionary model)
        {
            var errors = "";
            var c = 0;

            foreach (var item in model.Values)
            {
                if (item.Errors.Count > 0)
                {
                    var myValue = "";

                    if (item.AttemptedValue == null)
                    {
                        myValue = "(vacio)";
                    }
                    else
                    {
                        myValue = item.AttemptedValue.ToString();
                    }

                    errors += model.Keys.ElementAt(c) + ": " + myValue + Environment.NewLine;
                }
                c++;
            }

            return errors;
        }


        public static string ConvertStringToUTF8(string unicodeString)
        {
            // Create a UTF-8 encoding.
            UTF8Encoding utf8 = new UTF8Encoding();

            // Encode the string.
            Byte[] encodedBytes = utf8.GetBytes(unicodeString);

            // Decode bytes back to string.
            String decodedString = utf8.GetString(encodedBytes);

            return decodedString;
        }
    }


    public static class StreamExtensions
    {
        public static byte[] ToByteArray(this Stream stream)
        {
            try
            {
                stream.Position = 0;
                byte[] buffer = new byte[stream.Length];
                for (int totalBytesCopied = 0; totalBytesCopied < stream.Length;)
                    totalBytesCopied += stream.Read(buffer, totalBytesCopied, Convert.ToInt32(stream.Length) - totalBytesCopied);
                return buffer;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;

            }
        }
    }

    public static class StringHelper
    {
        public static string ToTitleCase(this string str)
        {
            var tokens = str.Split(new[] { " ", "-" }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                tokens[i] = token == token.ToUpper()
                    ? token
                    : token.Substring(0, 1).ToUpper() + token.Substring(1).ToLower();
            }

            return string.Join(" ", tokens);
        }

        public static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}
