using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.DTO.NovaPoshta.Responses
{
    public class NpCreateCounterpartyResult
    {
        public string Ref { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public NpContactPersonData ContactPerson { get; set; } = new();
    }
}
