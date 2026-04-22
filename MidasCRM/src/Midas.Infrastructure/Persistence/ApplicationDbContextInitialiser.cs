using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext dbContext)
    {
        public async Task InitialiseAsync()
        {
            try
            {
                await dbContext.Database.MigrateAsync();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An error occurred while initialising the database.");
                throw;
            }
        }
    }
}
