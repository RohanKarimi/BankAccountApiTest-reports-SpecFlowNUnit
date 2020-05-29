using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankAccount.SpecFlowNUnit.Models
{
    class BankApiTestInvalidResponseMessage
    {
        [JsonProperty("isValid")]
        public bool IsValid { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
