using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Core.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Infrastructure.Persistence.Queries
{
    public class UserQueries : IUserQueries
    {
        private readonly ApplicationDbContext _context;
        public UserQueries(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
    }
}
