using Emailit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public interface IReceivedMessageStateRepository
    {
        Task<ReceivedMessageState> AddAsync(ReceivedMessageState state);
        Task<ReceivedMessageState> UpdateAsync(ReceivedMessageState state);
    }
}
