using AbpCompanyName.AbpProjectName.Web.Mvc.Controllers;
using HtmlAgilityPack;
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
using System.Text.RegularExpressions;
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

        public static int? GetPhoneNumber(string text, int min, int max)
        {
            text = new String(text.ToCharArray().Where(c => Char.IsDigit(c)).ToArray()); //Get only digits

            bool isValidDigits = false;

            for (int i = min; i < max + 1; i++)
            {
                isValidDigits = Regex.Match(text, @"^([0-9]{" + i + "})$").Success;

                if (isValidDigits)
                {
                    break;
                }
            }

            if (isValidDigits)
            {
                return int.Parse(text);
            }

            return null;
        }

        public static int? GetPhoneNumber(string text)
        {
            bool isValidDigits = false;

            text = text.Replace(".", "").Replace(" ", "").Replace("-","").Replace("(01)", "").Replace("(","").Replace(")","").Replace("+511", "").Replace("+51", "").Replace("+5", "").Replace("+", "");

            //if (text.StartsWith("1"))
            //{
            //    text = text.Substring(1, text.Length - 1);
            //}

            isValidDigits = Regex.Match(text, @"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\s*([2-9]{2}|)\s*)\s*(?:[.-]\s*)?)?([2-9]{3})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$", RegexOptions.IgnoreCase).Success;

            if (isValidDigits)
            {
                return int.Parse(text);
            }

            string[] numbers = Regex.Split(text, @"\D+");

            foreach (string item in numbers)
            {
                if (item.Length == 7 || item.Length == 9)
                {
                    return int.Parse(item);
                }
            }

            return null;
        }

        public static ResultGoogle GetGoogleSearch(string query)
        {
            ResultGoogle result = new ResultGoogle();

            //https://moz.com/blog/the-ultimate-guide-to-the-google-search-parameters
            string urlGoogle = "https://www.google.com.pe/search?q=" + query + "&hl=es-PE&cr=countryPE&gl=pe";

            HtmlWeb web = new HtmlWeb();

            HtmlDocument doc = web.Load(urlGoogle);

            result.DocumentString = doc.DocumentNode.InnerHtml;

            //the parameter is use xpath see: https://www.w3schools.com/xml/xml_xpath.asp 

            HtmlNodeCollection graphGoogle = doc.DocumentNode.SelectNodes("//table[@class='O6u2Ve']");

            HtmlNodeCollection divsGoogle = doc.DocumentNode.SelectNodes("//div[@class='g']");

            string _title, _link, _trash, _text = string.Empty;
            int _length, _index = 0;
            int? phoneNumber;

            if (graphGoogle != null)
            {
                foreach (HtmlNode graph in graphGoogle)
                {
                    _title = ConvertStringToUTF8(graph.ChildNodes[1].ChildNodes[0].ChildNodes[0].InnerText.Split(",".ToCharArray()).FirstOrDefault());

                    if (string.IsNullOrEmpty(_title))
                    {
                        break;
                    }

                    if (graph.ChildNodes[1].ChildNodes[0].ChildNodes[1].ChildNodes.Count() > 1)
                    {
                        _text = graph.ChildNodes[1].ChildNodes[0].ChildNodes[1].ChildNodes[1].InnerText;
                    }
                    else
                    {
                        _text = graph.ChildNodes[1].ChildNodes[0].ChildNodes[1].InnerText;

                    }

                    string[] textArray = _text.Split(";".ToCharArray());

                    _text = textArray.FirstOrDefault();

                    if (textArray.Length > 1)
                    {
                        _text = textArray.ElementAt(1);
                    }

                    if (!string.IsNullOrEmpty(_text))
                    {
                        phoneNumber = GetPhoneNumber(_text);

                        if (phoneNumber != null)
                        {
                            result.Title = _title.Trim().Replace("amp;", "");
                            result.Text = phoneNumber.ToString();
                            result.Status = true;

                            return result;
                        }
                    }

                }

            }

            if (divsGoogle != null)
            {
                foreach (HtmlNode item in divsGoogle)
                {
                    if (item.ChildNodes.Count == 1)
                    {
                        HtmlDocument innerDiv = new HtmlDocument();
                        innerDiv.LoadHtml(item.InnerHtml.ToString());

                        HtmlNodeCollection divs = innerDiv.DocumentNode.SelectNodes("//div");

                        if (divs[0].ChildNodes[0].ChildNodes.Count() < 2)
                        {
                            break;
                        }

                        _title = ConvertStringToUTF8(divs[0].ChildNodes[0].ChildNodes[1].InnerText.Split(",".ToCharArray()).FirstOrDefault());
                        _text = divs[0].ChildNodes[0].ChildNodes[0].InnerText;

                        if (!string.IsNullOrEmpty(_text))
                        {
                            phoneNumber = GetPhoneNumber(_text);

                            if (phoneNumber != null)
                            {
                                result.Title = _title.Trim().Replace("amp;", "");
                                result.Text = phoneNumber.ToString();
                                result.Status = true;

                                return result;
                            }
                        }

                    }

                    if (item.ChildNodes.Count == 2)
                    {
                        _title = ConvertStringToUTF8(item.ChildNodes[0].InnerText);
                        _link = item.ChildNodes[0].ChildNodes[0].Attributes["href"].Value;
                        _link = _link.Replace("/url?q=", "");

                        _length = _link.Length;
                        _index = _link.IndexOf("&amp");

                        _trash = _link.Substring(_index, _length - _index);
                        _link = _link.Replace(_trash, "");

                        _text = ConvertStringToUTF8(item.ChildNodes[1].ChildNodes[1].InnerText);

                        if (!string.IsNullOrEmpty(_text))
                        {
                            phoneNumber = GetPhoneNumber(_text, 7, 9);
                            //phoneNumber = GetPhoneNumber(_text);

                            if (phoneNumber != null)
                            {
                                result.Title = _title.Trim().Replace("amp;", "");
                                result.Text = phoneNumber.ToString();
                                result.Link = _link;
                                result.Status = true;

                                return result;
                            }
                        }

                        //_result = $"Título: {_title}, Link: {_link}, Texto: {_text}";

                    }
                }

            }

            result.Text = "No se ha encontrado el número";

            return result;
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
