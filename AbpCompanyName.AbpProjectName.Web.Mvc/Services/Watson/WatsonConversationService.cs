using System;
using IBM.VCA.Watson.Watson.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IBM.VCA.Watson.Watson
{
    public class WatsonConversationService
    {
        public static ConversationService _conversation;
        public static Context _ctx;

        public class WatsonCredentials
        {
            public string workspaceID { get; set; }
            public string username { get; set; }
            public string password { get; set; }
        }

        public static MessageRequest Message(string text, Context ctx, WatsonCredentials creds)
        {

            MessageRequest messageRequest = new MessageRequest();

            if (text == null)
            {
                return messageRequest;

                //List<Intent> myIntents = new List<Intent>();

                //myIntents.Add(new Intent()
                //{
                //    Confidence = 1,
                //    IntentDescription = "algomas"
                //});

                //messageRequest.Intents = myIntents;
            }
            else
            {
                messageRequest.Input = new InputData()
                {
                    Text = text
                };
            }


            //if (ctx != null && ctx.System != null)
            //{
            //    messageRequest.Context = ctx;
            //}

            if (ctx != null)
            {
                messageRequest.Context = ctx;
            }

            _conversation = new ConversationService(creds.username, creds.password);


            MessageResponse result = _conversation.Message(creds.workspaceID, messageRequest);

            if (result != null && string.IsNullOrEmpty(result.Context.ErrorMessage))
            {
                if (result.Intents.Count > 0)
                {
                    messageRequest.Intents = result.Intents;
                }
                else
                {
                    Console.WriteLine("Intents vacíos");
                }

                if (result.Output != null)
                {
                    messageRequest.Context = result.Context.ToObject<Context>();

                    messageRequest.Output = result.Output;

                    return messageRequest;
                }
                else
                {
                    return messageRequest;
                }
            }
            else
            {

                if (result != null)
                {
                    messageRequest.Context = result.Context.ToObject<Context>();
                }

                return messageRequest;
            }
        }



    }
}
