using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.DTO.NovaPoshta.Responses
{
    public class NpTrackingStatusResponse
    {
        public string Number { get; set; } = string.Empty; // ТТН
        public string StatusCode { get; set; } = string.Empty; // Код статусу (напр. "2")
        public string Status { get; set; } = string.Empty; // Опис статусу
    }
}
