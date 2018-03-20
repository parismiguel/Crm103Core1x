using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBM.VCA.Watson.Watson.Model.Custom
{

    public class ChatModel
    {
        public Input input { get; set; }
        public bool alternate_intents { get; set; }
        public Context context { get; set; }
        public Intent[] intents { get; set; }
        public Output output { get; set; }
    }

    public class Input
    {
        public string text { get; set; }
    }

    public class Context
    {
        public string conversation_id { get; set; }
        public System system { get; set; }
        public string temperature { get; set; }
        public string city { get; set; }
        public string description { get; set; }
    }

    public class System
    {
        public Dialog_Stack[] dialog_stack { get; set; }
        public int dialog_turn_counter { get; set; }
        public int dialog_request_counter { get; set; }
    }

    public class Dialog_Stack
    {
        public string dialog_node { get; set; }
    }

    public class Output
    {
        public object[] log_messages { get; set; }
        public string[] text { get; set; }
        public string[] nodes_visited { get; set; }
    }

    public class Intent
    {
        public string intent { get; set; }
        public decimal confidence { get; set; }
    }

}