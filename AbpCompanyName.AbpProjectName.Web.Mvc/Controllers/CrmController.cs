using System;
using System.Collections.Generic;
using System.Linq;
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
        [RemoteService]
        [AllowAnonymous]
        public JsonResult SavePerson(SavePersonModel person)
        {
            //TODO: save new person to database and return new person's id

            //https://aspnetboilerplate.com/Pages/Documents/Authorization
            //https://aspnetboilerplate.com/Pages/Documents/Javascript-API/AJAX


            return Json(new { PersonId = 123456 });
        }

        [HttpPost]
        [RemoteService]
        [AllowAnonymous]
        public JsonResult GetPhoneNumber(string text)
        {
            Response response = new Response();

            try
            {
                //Get Entities using NLU
                string resultText = string.Empty;

                string _district = "Lima";
                string _country = "Perú";
                string _entity = string.Empty;

                if (text.Length > 14)
                {
                    AnalysisResults _nlu = NluController.Analyze(null, text);


                    foreach (var item in _nlu.Entities)
                    {
                        switch (item.Type)
                        {
                            case "LOCAL_COUNTRY":
                                _country = item.Text;
                                break;

                            case "LOCAL_DISTRICT":
                                _district = item.Text;
                                break;
                            default:
                                break;
                        }
                    }

                    _entity = _nlu.Entities.FirstOrDefault()?.Text;

                    if (string.IsNullOrEmpty(_entity))
                    {
                        resultText = "No se ha identificado la Entidad. Por favor, vuelva a intentarlo.";

                        return Json(response);

                    }
                    else
                    {
                        //string _queryParsed = string.Format("{0} {1} {2}", text, _district, _country);
                        string _queryParsed = $"teléfono {_entity} {_district} {_country}";

                        resultText = GetGoogleSearch(_queryParsed);

                    }

                }
                else
                {
                    string _textModified = text;

                    if (!_textModified.Contains("Lima"))
                    {
                        _textModified = _textModified + " Lima";
                    }

                    if (!_textModified.Contains("Perú"))
                    {
                        _textModified = _textModified + " Perú";
                    }

                    resultText = GetGoogleSearch(_textModified);

                }

                response.Status = true;
                response.Message = $"El número de teléfono de {_entity} es: {resultText}";

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

        public static string GetGoogleSearch(string query)
        {
            string _result = string.Empty;

            var urlGoogle = "http://google.com/search?q=" + query;

            HtmlWeb web = new HtmlWeb();

            HtmlDocument doc = web.Load(urlGoogle);

            //the parameter is use xpath see: https://www.w3schools.com/xml/xml_xpath.asp 
            HtmlNodeCollection divsGoogle = doc.DocumentNode.SelectNodes("//div[@class='g']");

            foreach (var item in divsGoogle)
            {
                if (item.ChildNodes.Count == 2)
                {
                    var _title = Helpers.ConvertStringToUTF8(item.ChildNodes[0].InnerText);
                    var _link = item.ChildNodes[0].ChildNodes[0].Attributes["href"].Value;
                    _link = _link.Replace("/url?q=", "");

                    var _length = _link.Length;
                    var _index = _link.IndexOf("&amp");

                    var trash = _link.Substring(_index, _length - _index);
                    _link = _link.Replace(trash, "");

                    var _text = Helpers.ConvertStringToUTF8(item.ChildNodes[1].ChildNodes[1].InnerText);

                    _result = $"Título: {_title}, Link: {_link}, Texto: {_text}";

                    break;
                }
            }


            return _result;
        }


    }

    public class SavePersonModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
