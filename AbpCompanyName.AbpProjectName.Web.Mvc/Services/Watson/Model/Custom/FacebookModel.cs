using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBM.VCA.Watson.Watson.Model.Custom
{
    public class FacebookModel
    {
        public Message[] messages { get; set; }
    }


    public class Message
    {
        public string text { get; set; }
        public Attachment attachment { get; set; }
    }

    public class FacebookModel2
    {
        public Message2 message { get; set; }
    }

    public class Message2
    {
        public Attachment attachment { get; set; }
    }


    public class Attachment
    {
        public string type { get; set; }
        public Payload payload { get; set; }
    }

    public class Payload
    {
        public string template_type { get; set; }
        public Element[] elements { get; set; }
    }

    public class Element
    {
        public string title { get; set; }
        public string image_url { get; set; }
        public string subtitle { get; set; }
        public Button[] buttons { get; set; }
    }

    public class Button
    {
        public string type { get; set; }
        public string url { get; set; }
        public string title { get; set; }
    }


}