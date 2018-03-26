using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.AspNetCore.Mvc.Controllers;
using AbpCompanyName.AbpProjectName.Controllers;
using AbpCompanyName.AbpProjectName.Web.Models;
using HtmlAgilityPack;
using IBM.WatsonDeveloperCloud.NaturalLanguageUnderstanding.v1.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TemplateCoreParis.Controllers;
using TemplateCoreParis.Helpers;

namespace AbpCompanyName.AbpProjectName.Web.Mvc.Controllers
{
    //[Produces("application/json")]
    //[Route("api/Crm")]
    public class CrmController : AbpController
    {
        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public JsonResult SavePerson(SavePersonModel person)
        {
            //TODO: save new person to database and return new person's id

            // https://aspnetboilerplate.com/Pages/Documents/XSRF-CSRF-Protection

            //https://aspnetboilerplate.com/Pages/Documents/Authorization
            //https://aspnetboilerplate.com/Pages/Documents/Javascript-API/AJAX


            return Json(new { PersonId = 123456 });
        }

        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public JsonResult GetPhoneNumber(string text)
        {
            Response response = new Response();
            ResultGoogle resultGoogle = new ResultGoogle();

            if (string.IsNullOrEmpty(text))
            {
                response.Message = "Texto vacío o nulo";
                return Json(response);
            }

            try
            {
                //Get Entities using NLU

                string _district = "Lima";
                string _country = "Perú";
                string _entity = string.Empty;

                string _textModified = text.ToLower();

                if (!_textModified.Contains("lima"))
                {
                    _textModified = _textModified + " Lima";
                }

                if (!_textModified.Contains("perú") && !_textModified.Contains("peru"))
                {
                    _textModified = _textModified + " Perú";
                }

                if (!_textModified.Contains("teléfono") && !_textModified.Contains("telefono"))
                {
                    _textModified = "Teléfono " + _textModified;
                }

                if (_textModified?.Length > 14)
                {
                    AnalysisResults _nlu = NluController.Analyze(null, _textModified);

                    if (_nlu.Entities != null)
                    {
                        foreach (var item in _nlu.Entities)
                        {
                            switch (item.Type)
                            {
                                case "Organization":
                                    _entity = item.Text;
                                    break;
                                case "Location":
                                    if (item.Disambiguation.Subtype[0].StartsWith("City"))
                                    {
                                        _district = item.Text;
                                    }

                                    if (item.Disambiguation.Subtype[0].StartsWith("Country"))
                                    {
                                        _country = item.Text;
                                    }

                                    break;

                                default:
                                    break;
                            }
                        }

                    }

                    //_entity = _nlu.Entities.FirstOrDefault()?.Text;

                    if (string.IsNullOrEmpty(_entity))
                    {
                        //response.Message = "No se ha identificado la Entidad. Por favor, vuelva a intentarlo.";
                        //return Json(response);

                        resultGoogle = GetGoogleSearch(_textModified);

                    }
                    else
                    {
                        //string _queryParsed = string.Format("{0} {1} {2}", text, _district, _country);
                        string _queryParsed = $"Teléfono {_entity} {_district} {_country}";

                        resultGoogle = GetGoogleSearch(_queryParsed);

                    }

                }
                else
                {

                    resultGoogle = GetGoogleSearch(_textModified);

                }

                response.Status = true;

                if (Regex.IsMatch(resultGoogle.Text, @"^\d+$"))
                {
                    if (string.IsNullOrEmpty(_entity))
                    {
                        response.Message = $"El número de teléfono de {resultGoogle.Title} es: {resultGoogle.Text}";

                    }
                    else
                    {
                        response.Message = $"El número de teléfono de {_entity} es: {resultGoogle.Text}";
                    }
                }
                else
                {
                    response.Message = "No se ha encontrado un número. Por favor, vuelva a intentarlo.";
                }


            }
            catch (ArgumentException e)
            {
                response.Trace = e.StackTrace;
                response.Message = e.Message;
            }
            catch (Exception e)
            {
                response.Trace = e.StackTrace;
                response.Message = e.Message;
            }

            return Json(response);

        }

        public static ResultGoogle GetGoogleSearch(string query)
        {
            ResultGoogle result = new ResultGoogle();

            string urlGoogle = "http://google.com/search?q=" + query;

            HtmlWeb web = new HtmlWeb();

            HtmlDocument doc = web.Load(urlGoogle);

            //the parameter is use xpath see: https://www.w3schools.com/xml/xml_xpath.asp 

            HtmlNodeCollection graphGoogle = doc.DocumentNode.SelectNodes("//table[@class='O6u2Ve']");

            HtmlNodeCollection divsGoogle = doc.DocumentNode.SelectNodes("//div[@class='g']");

            string _title, _link, _trash, _text = string.Empty;
            int _length, _index = 0;
            int? phoneNumber;

            foreach (HtmlNode graph in graphGoogle)
            {
                _title = Helpers.ConvertStringToUTF8(graph.ChildNodes[1].ChildNodes[0].ChildNodes[0].InnerText.Split(",".ToCharArray()).FirstOrDefault());
                _text = graph.ChildNodes[1].ChildNodes[0].ChildNodes[1].ChildNodes[1].InnerText.Split(";".ToCharArray()).ElementAt(1);

                if (!string.IsNullOrEmpty(_text))
                {
                    phoneNumber = GetPhoneNumber(_text, 7, 9);

                    if (phoneNumber != null)
                    {
                        result.Title = _title.Trim();
                        result.Text = phoneNumber.ToString();
                        result.Status = true;

                        return result;
                    }
                }

            }


            foreach (HtmlNode item in divsGoogle)
            {
                if (item.ChildNodes.Count == 1)
                {
                    HtmlDocument innerDiv = new HtmlDocument();
                    innerDiv.LoadHtml(item.InnerHtml.ToString());

                    HtmlNodeCollection divs = innerDiv.DocumentNode.SelectNodes("//div");

                    _title = Helpers.ConvertStringToUTF8(divs[0].ChildNodes[0].ChildNodes[1].InnerText.Split(",".ToCharArray()).FirstOrDefault());
                    _text = divs[0].ChildNodes[0].ChildNodes[0].InnerText;

                    if (!string.IsNullOrEmpty(_text))
                    {
                        phoneNumber = GetPhoneNumber(_text, 7, 9);

                        if (phoneNumber != null)
                        {
                            result.Title = _title.Trim();
                            result.Text = phoneNumber.ToString();
                            result.Status = true;

                            return result;
                        }
                    }

                }

                if (item.ChildNodes.Count == 2)
                {
                    _title = Helpers.ConvertStringToUTF8(item.ChildNodes[0].InnerText);
                    _link = item.ChildNodes[0].ChildNodes[0].Attributes["href"].Value;
                    _link = _link.Replace("/url?q=", "");

                    _length = _link.Length;
                    _index = _link.IndexOf("&amp");

                    _trash = _link.Substring(_index, _length - _index);
                    _link = _link.Replace(_trash, "");

                    _text = Helpers.ConvertStringToUTF8(item.ChildNodes[1].ChildNodes[1].InnerText);

                    if (!string.IsNullOrEmpty(_text))
                    {
                        phoneNumber = GetPhoneNumber(_text, 7, 9);

                        if (phoneNumber != null)
                        {
                            result.Title = _title;
                            result.Text = phoneNumber.ToString();
                            result.Link = _link;
                            result.Status = true;

                            return result;
                        }
                    }

                    //_result = $"Título: {_title}, Link: {_link}, Texto: {_text}";

                }
            }

            result.Text = "No se ha encontrado el número";

            return result;
        }

        internal static int? GetPhoneNumber(string text, int min, int max)
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


    }

    public class SavePersonModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }


}
