using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindImageApp.Models
{
    public class SearchQuery
    {
        public enum eReplyMode
        {
            None,
            Url,
            FileContent,
            Redirect
        };

        public string Query { get; set; } = string.Empty;

        public eReplyMode ReplyMode { get; set; } = eReplyMode.None;
    }
}
