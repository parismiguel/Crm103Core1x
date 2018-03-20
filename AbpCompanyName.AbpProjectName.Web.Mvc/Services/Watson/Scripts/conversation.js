// The ConversationPanel module is designed to handle
// all display and behaviors of the conversation column of the app.
/* eslint no-unused-vars: "off" */
/* global Api: true, Common: true*/

var ConversationPanel = (function () {
    var settings = {
        selectors: {
            chatBox: '#scrollingChat',
            fromUser: '.from-user',
            fromWatson: '.from-watson',
            latest: '.latest'
        },
        authorTypes: {
            user: 'user',
            watson: 'watson'
        }
    };

    // Publicly accessible methods defined
    return {
        init: init,
        inputKeyDown: inputKeyDown
    };

    // Initialize the module
    function init() {
        chatUpdateSetup();
        setupInputBox();

        //var cookiePayLoad = $.cookie().myPayload;
        var cookiePayLoad = localStorage.getItem("myPayload");
        var cookieResponse = localStorage.getItem("myResponse");

        if (cookiePayLoad != "" && cookiePayLoad != undefined) {
            $("#scrollingChat").html(cookiePayLoad);


            $("#ibm-contact-module").toggleClass("ibm-active");
            $(".ibm-contact-widget-btn").toggleClass("ibm-active");
            scrollToChatBottom();

            //Api.setResponsePayload(cookiePayLoad);

            //Api.responsePayload = cookieResponse;

            if (cookieResponse !== "undefined") {
                var model = JSON.parse(cookieResponse);
                model.output.Text = null;
                Api.setResponsePayload(JSON.stringify(model));
            }
        }
        else {
            Api.sendRequest(null, null);
        }

    }
    // Set up callbacks on payload setters in Api module
    // This causes the displayMessage function to be called when messages are sent / received
    function chatUpdateSetup() {
        var currentRequestPayloadSetter = Api.setRequestPayload;

        Api.setRequestPayload = function (newPayloadStr) {
            currentRequestPayloadSetter.call(Api, newPayloadStr);
            var payload = JSON.parse(newPayloadStr);

            if (payload.input) {
                if (payload.input.text != "welcome" && payload.input.text != null) {
                    displayMessage(payload, settings.authorTypes.user);
                }
            }

        };

        var currentResponsePayloadSetter = Api.setResponsePayload;

        Api.setResponsePayload = function (newPayloadStr) {
            currentResponsePayloadSetter.call(Api, newPayloadStr);

            var model = JSON.parse(newPayloadStr);

            if (model.output.Text != null) {
                displayMessage(JSON.parse(newPayloadStr), settings.authorTypes.watson);
            }
        };
    }

    // Set up the input box to underline text as it is typed
    // This is done by creating a hidden dummy version of the input box that
    // is used to determine what the width of the input text should be.
    // This value is then used to set the new width of the visible input box.
    function setupInputBox() {
        var input = document.getElementById('textInput');
        var dummy = document.getElementById('textInputDummy');
        var minFontSize = 14;
        var maxFontSize = 16;
        var minPadding = 4;
        var maxPadding = 6;

        // If no dummy input box exists, create one
        if (dummy === null) {
            var dummyJson = {
                'tagName': 'div',
                'attributes': [{
                    'name': 'id',
                    'value': 'textInputDummy'
                }]
            };

            dummy = Common.buildDomElement(dummyJson);
            document.body.appendChild(dummy);
            input.focus();

        }

        function adjustInput() {
            if (input.value === '') {
                // If the input box is empty, remove the underline
                input.classList.remove('underline');
                input.setAttribute('style', 'width:' + '100%');
                input.style.width = '100%';
            } else {
                // otherwise, adjust the dummy text to match, and then set the width of
                // the visible input box to match it (thus extending the underline)
                input.classList.add('underline');
                var txtNode = document.createTextNode(input.value);
                ['font-size', 'font-style', 'font-weight', 'font-family', 'line-height',
                  'text-transform', 'letter-spacing'].forEach(function (index) {
                      dummy.style[index] = window.getComputedStyle(input, null).getPropertyValue(index);
                  });
                dummy.textContent = txtNode.textContent;

                var padding = 0;
                var htmlElem = document.getElementsByTagName('html')[0];
                var currentFontSize = parseInt(window.getComputedStyle(htmlElem, null).getPropertyValue('font-size'), 10);
                if (currentFontSize) {
                    padding = Math.floor((currentFontSize - minFontSize) / (maxFontSize - minFontSize)
                      * (maxPadding - minPadding) + minPadding);
                } else {
                    padding = maxPadding;
                }

                var widthValue = (dummy.offsetWidth + padding) + 'px';
                input.setAttribute('style', 'width:' + widthValue);
                input.style.width = widthValue;
            }
        }

        // Any time the input changes, or the window resizes, adjust the size of the input box
        input.addEventListener('input', adjustInput);
        window.addEventListener('resize', adjustInput);

        // Trigger the input event once to set up the input box and dummy element
        Common.fireEvent(input, 'input');
    }

    // Display a user or Watson message that has just been sent/received
    function displayMessage(newPayload, typeValue) {
        var isUser = isUserMessage(typeValue);
        var textExists = (newPayload.input && newPayload.input.text)
          || (newPayload.output && newPayload.output.Text);

        if (isUser !== null && textExists) {

            // Create new message DOM element
            var messageDivs = buildMessageDomElements(newPayload, isUser);
            var chatBoxElement = document.querySelector(settings.selectors.chatBox);
            var previousLatest = chatBoxElement.querySelectorAll((isUser
                    ? settings.selectors.fromUser : settings.selectors.fromWatson)
                    + settings.selectors.latest);

            // Previous "latest" message is no longer the most recent
            if (previousLatest) {
                Common.listForEach(previousLatest, function (element) {
                    element.classList.remove('latest');
                });
            }

            messageDivs.forEach(function (currentDiv) {
                chatBoxElement.appendChild(currentDiv);
                // Class to start fade in animation
                currentDiv.classList.add('load');
            });

            // Move chat to the most recent messages when new messages are added
            scrollToChatBottom();

        }
    }

    // Checks if the given typeValue matches with the user "name", the Watson "name", or neither
    // Returns true if user, false if Watson, and null if neither
    // Used to keep track of whether a message was from the user or Watson
    function isUserMessage(typeValue) {
        if (typeValue === settings.authorTypes.user) {
            return true;
        } else if (typeValue === settings.authorTypes.watson) {
            return false;
        }
        return null;
    }

    // Constructs new DOM element from a message payload
    function buildMessageDomElements(newPayload, isUser) {
        var textArray = isUser ? newPayload.input.text : newPayload.output.Text;

        if (Object.prototype.toString.call(textArray) !== '[object Array]') {
            textArray = [textArray];
        }
        var messageArray = [];

        textArray.forEach(function (currentText) {
            if (currentText) {
                var messageJson = {
                    // <div class='segments'>
                    'tagName': 'div',
                    'classNames': ['segments'],
                    'children': [{
                        // <div class='from-user/from-watson latest'>
                        'tagName': 'div',
                        'classNames': [(isUser ? 'from-user' : 'from-watson'), 'latest', ((messageArray.length === 0) ? 'top' : 'sub')],
                        'children': [{
                            // <div class='message-inner'>
                            'tagName': 'div',
                            'classNames': ['message-inner'],
                            'children': [{
                                // <p>{messageText}</p>
                                'tagName': 'p',
                                'html': currentText
                            }]
                        }]
                    }]
                };

                if (!isUser) {
                    var textToS = getCleanedString(currentText);

                    //$("#Audio1").trigger("pause");
                    //$("#Audio1").attr("src", "about:blank");
                    //$("#Audio1").trigger("load");
                    //$("#Audio1").remove();

                    //var dummy = URL.createObjectURL(new Blob([], {type:"audio/mp3"}));
                    //$("#Audio1").attr("src", dummy)


                    //var url = "/Home/TextToSpeech/";
                    $.post("/Home/TextToSpeech/", { text: textToS }, function (sample) {
                        //$("#ibm-contact-module").append('<audio id="Audio1" preload="none"></audio>');
                        $("#Audio1").attr("src", sample).trigger("load").trigger("play");
                    })
                         .done(function (sample) {
                             //$("#Audio1").attr("src", sample).trigger("play");
                         })
                  .fail(function (result) {
                      NotificationToast("error", result, "Error");
                  });
                }

                messageArray.push(Common.buildDomElement(messageJson));
            }
        });

        return messageArray;
    }

    // Scroll to the bottom of the chat window (to the most recent messages)
    // Note: this method will bring the most recent user message into view,
    //   even if the most recent message is from Watson.
    //   This is done so that the "context" of the conversation is maintained in the view,
    //   even if the Watson message is long.
    function scrollToChatBottom() {
        var scrollingChat = document.querySelector('#scrollingChat');

        // Scroll to the latest message sent by the user
        var scrollEl = scrollingChat.querySelector(settings.selectors.fromUser
                + settings.selectors.latest);
        if (scrollEl) {
            scrollingChat.scrollTop = scrollEl.offsetTop;
        }
    }

    // Handles the submission of input
    function inputKeyDown(event, inputBox) {
        // Submit on enter key, dis-allowing blank messages
        if (event.keyCode === 13 && inputBox.value) {
            // Retrieve the context from the previous server response
            var context;
            var latestResponse = Api.getResponsePayload();

            if (latestResponse) {
                context = latestResponse.context;
            }


            // Send the user message
            Api.sendRequest(inputBox.value, context);

            // Clear input box for further messages
            inputBox.value = '';
            Common.fireEvent(inputBox, 'input');
        }
    }
}());
