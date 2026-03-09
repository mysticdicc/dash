using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DashLib.DTO
{
    public class PingResponseDto
    {
        public string IP { get; set; }
        public bool IcmpResponse { get; set; }
        public string Exception { get; set; }

        public PingResponseDto()
        {
            IP = string.Empty;
            IcmpResponse = false;
            Exception = string.Empty;
        }

        [JsonConstructor] public PingResponseDto(string IP, bool IcmpResponse, string Exception)
        {
            this.IP = IP;
            this.IcmpResponse = IcmpResponse;
            this.Exception = Exception;
        }
    }
}
