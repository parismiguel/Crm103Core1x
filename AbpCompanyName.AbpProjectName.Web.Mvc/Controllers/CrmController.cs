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
                    _textModified = _textModified.ToTitleCase();

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



                    //if (string.IsNullOrEmpty(_entity))
                    //{
                    //    resultGoogle = GetGoogleSearch(_textModified);

                    //    //if (_nlu.Keywords != null)
                    //    //{
                    //    //    _entity = _nlu.Keywords.Where(k => k.Relevance > 0.8).Select(k => k.Text).FirstOrDefault().ToTitleCase();

                    //    //    _entity = _entity.Replace("Teléfono","").Replace("Telefono", "");
                    //    //}

                    //    //if (string.IsNullOrEmpty(_entity))
                    //    //{
                    //    //    resultGoogle = GetGoogleSearch(_textModified);
                    //    //}
                    //    //else
                    //    //{
                    //    //    resultGoogle = GetGoogleSearch($"Teléfono {_entity } {_district} {_country}");
                    //    //}
                    //}
                    //else
                    //{
                    //    string _queryParsed = $"Teléfono {_entity} {_district} {_country}";

                    //    resultGoogle = GetGoogleSearch(_queryParsed);

                    //}

                }
                //else
                //{

                //    resultGoogle = GetGoogleSearch(_textModified);

                //}

                resultGoogle = Helpers.GetGoogleSearch(_textModified);

                response.Status = resultGoogle.Status;
                response.Data = resultGoogle.DocumentString;

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


        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> GetPhoneNumberCustomAsync(string text)
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

                resultGoogle = await Helpers.GetGoogleSearchCustomAsync(_textModified);

                response.Status = resultGoogle.Status;
                response.Data = resultGoogle.DocumentString;

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


    }

    public class SavePersonModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }


}
