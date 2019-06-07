using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTT_Helper
{
    public class WebPTT
    {
        public string Popularity_ptt { get; set; }
        public string title_ptt { get; set; }
        public string author_ptt { get; set; }
        public string URL_ptt { get; set; }
        public string InnerText_ptt { get; set; }
        public List<WebPTT_Push> ptt_Push_info { get; set; }


    }
    public class WebPTT_InnerContext
    {
        public string Text { get; set; }
    }

    public class WebPTT_Push
    {
        public string push_tag { get; set; }
        public string user_id { get; set; }
        public string context { get; set; }
        public string datetime { get; set; }

    }
}
