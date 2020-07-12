using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTelegramBotsBuilder.Models.ViewModels
{
    public class ErrorModel
    {
        public string Message { get; set; }
        public ErrorModel()
        {
            Message = "Undefined error";
        }
        public ErrorModel(string message)
        {
            Message = message;
        }
    }
}
