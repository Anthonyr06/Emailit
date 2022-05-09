using Emailit.Data;
using Emailit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public class ReceivedMessageStateRepositoryEF : IReceivedMessageStateRepository
    {

        private readonly ApplicationDbContext _dbContext;

        public ReceivedMessageStateRepositoryEF(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ReceivedMessageState> AddAsync(ReceivedMessageState state)
        {
            await _dbContext.AddAsync(state);
            await _dbContext.SaveChangesAsync();
            return state;
        }

        public Task<ReceivedMessageState> UpdateAsync(ReceivedMessageState state)
        {
            throw new NotImplementedException();
        }
    }
}
