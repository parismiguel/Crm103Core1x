// The Api module is designed to handle all interactions with the server

var Api = (function () {
    var requestPayload;
    var responsePayload;
    var messageEndpoint = '/api/message';

    // Publicly accessible methods defined
    return {
        sendRequest: sendRequest,

        // The request/response getters/setters are defined here to prevent internal methods
        // from calling the methods without any of the callbacks that are added elsewhere.
        getRequestPayload: function () {
            return requestPayload;
        },
        setRequestPayload: function (newPayloadStr) {
            requestPayload = JSON.parse(newPayloadStr);
        },
        getResponsePayload: function () {
            return responsePayload;
        },
        setResponsePayload: function (newPayloadStr) {
            responsePayload = JSON.parse(newPayloadStr);
            //responseHandler(newPayloadStr);
        }
    };

    // Send a message request to the server
    function sendRequest(text, context) {
        var url = "/Home/MessageChat/";
        var latestResponse = Api.getResponsePayload();

        if (latestResponse === undefined) {
            latestResponse = null;
        }

        $("#textInput").hide();
        $("#chat_loader").show();

        // Build request payload
        var payloadToWatson = {};

        if (context == null || context == undefined) {
            if (latestResponse != null) {
                payloadToWatson = latestResponse;
                context = latestResponse.context;
            }
        }
        else {
            payloadToWatson.context = context;
        }

        if (text) {
            if (text == "#algomas") {
                payloadToWatson.intents = {
                    confidence: 1,
                    intentdescription: text
                };
                text = null;
            }
            else {
                payloadToWatson.input = {
                    text: text
                };
            }

        }

        if (text === null) {
            text = 'welcome';
            context = null;
            payloadToWatson = {};
            payloadToWatson.input = {
                text: text
            };
        }

        var params = JSON.stringify(payloadToWatson);
        // Stored in variable (publicly visible through Api.getRequestPayload)
        // to be used throughout the application
        if (Object.getOwnPropertyNames(payloadToWatson).length !== 0) {
            Api.setRequestPayload(params);
        }

        //var url = "/Home/MessageChat/";
        $.post(url, { text: text, ctx: context }, function (result) {

            $("#textInput").hide();

            payloadToWatson.output = result.response.Output;

            if (result.response.Context) {
                payloadToWatson.context = result.response.Context;
            }

            if (result.response.Intents) {
                payloadToWatson.intents = result.response.Intents;
            }

            params = JSON.stringify(payloadToWatson);

            responseHandler(result, payloadToWatson);
        })
        .fail(function (result) {
            NotificationToast("error", result.statusText, "Error");
        });

    }

    function responseHandler(result, payloadToWatson) {
        var params = JSON.stringify(payloadToWatson);

        if (result.response.Intents) {

            var myIntent = result.response.Intents[0].IntentDescription;

            var servicio = result.response.Context.Servicio;
            var codigo = result.response.Context.Codigo;

            switch (servicio) {

                case "producto":

                    if (codigo !== undefined && codigo !== null) {

                        var url = "/Api/Productos/";

                        $.get(url, { codigo: codigo }, function (data) {

                            $("#textInput").hide();

                            var outputText;
                            
                            if (data == null) {
                                outputText = "No se encuentra el código de producto indicado";
                            }
                            else {
                                var precio = data.Precio;
                                var nombre = data.Nombre;

                                outputText = "El precio de " + nombre + " es de " +
                                    precio + " soles. Item consultado <strong>" + codigo + "</strong>";
                            }

                            payloadToWatson.output.Text[0] = outputText;
                            payloadToWatson.context.Servicio = null;
                            payloadToWatson.context.Codigo = null;

                            params = JSON.stringify(payloadToWatson);

                            $("#chat_loader").hide();
                            $("#textInput").show();

                            Api.setResponsePayload(params);

                            $("#textInput").focus();
                        })
                          .fail(function (error) {
                              NotificationToast("error", error, "Error");
                          });
                        break;
                    }

                   
                case "pedido":

                    if (codigo !== undefined && codigo !== null) {

                        var url = "/Api/Pedidos/";

                        $.get(url, { codigo: codigo }, function (data) {
                            $("#textInput").hide();

                            var outputText;

                            if (data == null) {
                                outputText = "No se encuentra el número de pedido indicado";
                            }
                            else {
                                var nroPedido = data.NroPedido;
                                outputText = "Su pedido con N° " + nroPedido +
                                    " para " + data.Nombre + " se encuentra en ruta";
                            }

                            payloadToWatson.output.Text[0] = outputText;
                            payloadToWatson.context.Servicio = null;
                            payloadToWatson.context.Codigo = null;

                            params = JSON.stringify(payloadToWatson);

                            $("#chat_loader").hide();
                            $("#textInput").show();

                            Api.setResponsePayload(params);

                            $("#textInput").focus();
                        })
                      .fail(function (error) {
                          NotificationToast("error", error, "Error");
                      });

                        break;
                    }

                default:

                    $("#chat_loader").hide();
                    $("#textInput").show();

                    Api.setResponsePayload(params);

                    $("#textInput").focus();
            }

        }
        else {
            $("#chat_loader").hide();
            $("#textInput").show();

            Api.setResponsePayload(params);

            $("#textInput").focus();
        }

    }


    function getActions(action) {
        let res = {};

        let cnt = 0;

        for (let key in action) {
            if (action.hasOwnProperty(key)) {
                res[cnt] = {
                    cmd: key,
                    arg: action[key]
                };
                cnt++;
            }
        }
        return res;
    }

    function getWeather(result, payloadToWatson) {
        var text = result.response.Input;
        var context = result.response.Context;
        var watson = result.response.Output.Text[0];
        var url = "/Home/WeatherAPI/";

        $.post(url, { msg: text, ctx: context }, function (result) {
            var city = result.response.Context.City;
            var temperature = result.response.Context.Temperature;
            var description = result.response.Context.Description;

            //watson = watson.format(city, temperature, description);

            payloadToWatson.output.Text[0] = result.response.Output.Text[0];

            //payloadToWatson.context.City = city;
            //payloadToWatson.context.Temperature = temperature;
            //payloadToWatson.context.Description = description;

            payloadToWatson.context.City = null;
            payloadToWatson.context.Temperature = null;
            payloadToWatson.context.Description = null;

            var params = JSON.stringify(payloadToWatson);

            Api.setResponsePayload(params);

            context.City = null;
            context.Temperature = null;
            context.Description = null;
            //context.Redirect = "true";

            //Api.sendRequest("#algomas", context);
        })
        .fail(function (result) {
            NotificationToast("error", result.statusText, "Error");
        });

    }


}());
