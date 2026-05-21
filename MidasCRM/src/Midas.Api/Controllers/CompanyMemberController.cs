using Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Entities.CompanyMembers.Commands;
using Midas.Core.CompanyMembers;

namespace Midas.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyMemberController(
        ISender sender,
        IGetQueries<CompanyMember, int> getQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CompanyMemberDto>>> GetCompanyMembers(CancellationToken cancellationToken)
        {
            var members = await getQueries.GetAllAsync(cancellationToken, q => q.Include(x => x.User));
            return Ok(members.Select(CompanyMemberDto.FromDomain));
        }

        [HttpPost]
        public async Task<ActionResult<CompanyMemberDto>> AddCompanyMember([FromBody] AddCompanyMemberDto request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new AddCompanyMemberCommand { UserId = request.UserId, Role = request.Role }, cancellationToken);
            return Ok(CompanyMemberDto.FromDomain(result));
        }

        [HttpPut("{userId:guid}/role")]
        public async Task<ActionResult<CompanyMemberDto>> UpdateCompanyMemberRole(Guid userId, [FromBody] UpdateCompanyMemberRoleDto request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateCompanyMemberRoleCommand { UserId = userId, Role = request.Role }, cancellationToken);
            return Ok(CompanyMemberDto.FromDomain(result));
        }

        [HttpDelete("{userId:guid}")]
        public async Task<ActionResult<CompanyMemberDto>> RemoveCompanyMember(Guid userId, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new RemoveCompanyMemberCommand { UserId = userId }, cancellationToken);
            return Ok(CompanyMemberDto.FromDomain(result));
        }
    }
}
