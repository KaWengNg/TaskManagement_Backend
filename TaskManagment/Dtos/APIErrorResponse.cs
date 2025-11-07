using System;
using System.Text.Json.Serialization;

namespace TaskManagment.Dtos
{
	public class APIErrorResponse
	{
        public Error error { get; set; } = new();
    }

    public class Error
    {
        public int code { get; set; }

        public string message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string detail { get; set; }
    }
}

