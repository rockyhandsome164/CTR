using CTR_FLS_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTR_FLS_2.ViewModels
{
    public class OnBaseViewModel
    {
        public List<OnBaseDocMeta> DocMetaData { get; set; }

        public string UserMessage { get; set; }
    }
}