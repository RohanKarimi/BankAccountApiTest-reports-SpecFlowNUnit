using System;
using Newtonsoft.Json;

namespace BankAccount.SpecFlowNUnit.Models
{
    public class BankApiTestSuccessResponse
    {
        [JsonProperty("isValid")]
        public bool IsValid { get; set; }
    }
}
