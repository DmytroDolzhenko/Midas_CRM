using Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Entities.Companies.Commands;
using Midas.Core.Companies;

namespace Midas.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController(
        ISender sender,
        IGetQueries<Company, Guid> getQueries,
        ICurrentUserService currentUserService) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CompanyDto>>> GetCompanies(CancellationToken cancellationToken)
        {
            var companies = await getQueries.GetAllAsync(cancellationToken, q => q.Include(x => x.Members).ThenInclude(x => x.User));
            return Ok(companies.Select(CompanyDto.FromDomain));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CompanyDto>> GetCompanyById(Guid id, CancellationToken cancellationToken)
        {
            var company = await getQueries.GetByIdAsync(id, cancellationToken, q => q.Include(x => x.Members).ThenInclude(x => x.User));
            if (company is null)
            {
                return NotFound();
            }

            return Ok(CompanyDto.FromDomain(company));
        }

        [HttpGet("balance")]
        public async Task<ActionResult<CompanyBalanceDto>> GetCompanyBalance(CancellationToken cancellationToken)
        {
            var companyId = await currentUserService.GetCompanyIdAsync(cancellationToken);
            if (companyId is null)
            {
                return Unauthorized();
            }

            var company = await getQueries.GetByIdAsync(companyId.Value, cancellationToken);
            if (company is null)
            {
                return NotFound();
            }

            return Ok(new CompanyBalanceDto(company.Id, company.Balance));
        }

        [HttpPost]
        public async Task<ActionResult<CompanyDto>> CreateCompany([FromBody] CreateCompanyDto request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new CreateCompanyCommand { Name = request.Name, TaxNumber = request.TaxNumber }, cancellationToken);
            return Ok(CompanyDto.FromDomain(result));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CompanyDto>> UpdateCompany(Guid id, [FromBody] UpdateCompanyDto request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateCompanyCommand { Id = id, Name = request.Name, TaxNumber = request.TaxNumber }, cancellationToken);
            return Ok(CompanyDto.FromDomain(result));
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<CompanyDto>> DeleteCompany(Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteCompanyCommand { Id = id }, cancellationToken);
            return Ok(CompanyDto.FromDomain(result));
        }
        [HttpPost("add-member")]
        public async Task<ActionResult<CompanyDto>> AddMemberByEmail([FromBody] AddMemberByEmailDto request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new AddMemberByEmailCommand { CompanyId = request.CompanyId, Email = request.Email }, cancellationToken);
            return Ok(CompanyDto.FromDomain(result));
        }
    }
}
