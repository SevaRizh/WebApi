using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TZ_C_Jun.Models
{
    /// <summary>
    /// Представляет тело для Post запросов формата json 
    /// </summary>
    public class ReqJson
    {
        public string subject { get; set; }
        public string body { get; set; }
        public string[] recipients { get; set; }
    }
}
