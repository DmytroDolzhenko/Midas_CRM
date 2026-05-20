using MediatR;
using Microsoft.AspNetCore.Mvc;
using Midas.Application.DTO.NovaPoshta;
using Midas.Application.Entities.NovaPoshta;
using Midas.Application.Entities.NovaPoshta.Commands;

namespace Midas.Api.Controllers
{
    [ApiController]
    [Route("api/novaposhta/settings")]
    public class NovaPoshtaSettingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NovaPoshtaSettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 1. Отримати список контрагентів-відправників (ФОП/ТОВ/Фіз.Особа)
        [HttpGet("senders")]
        public async Task<ActionResult<List<NpSenderDto>>> GetSenders(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetNpSendersQuery(), ct);
            return Ok(result);
        }

        // 2. Отримати контактних осіб для обраного контрагента
        [HttpGet("contacts")]
        public async Task<ActionResult<List<NpContactDto>>> GetContacts([FromQuery] string senderRef, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetNpSenderContactsQuery(senderRef), ct);
            return Ok(result);
        }

        // 3. Зберегти налаштований логістичний профіль
        [HttpPost("logistic-profile")]
        public async Task<IActionResult> SaveProfile([FromBody] SaveLogisticProfileCommand command, CancellationToken ct)
        {
            await _mediator.Send(command, ct);
            return Ok(new { Message = "Профіль логістики успішно налаштовано!" });
        }
    }
}
