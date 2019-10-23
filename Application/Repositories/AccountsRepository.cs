using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Database;
using Application.Models.Entities;
using MongoDB.Driver;

namespace Application.Repositories
{
    public interface IAccountsRepository
    {
        Task<List<Account>> Get();
        Task<Account> GetById(string id);
        Task<Account> Create(Account account);
        Task Update(string id, Account accountIn);
        Task Remove(Account accountIn);
        Task Remove(string id);
    }

    public class AccountsRepository : IAccountsRepository
    {
        private readonly IMongoCollection<Account> _accounts;

        public AccountsRepository(IDatabaseContext dbContext)
        {
            if (dbContext.IsConnectionOpen())
                _accounts = dbContext.Accounts;
        }

        public async Task<List<Account>> Get()
        {
            return await (await _accounts.FindAsync(account => true)).ToListAsync();
        }

        public async Task<Account> GetById(string id)
        {
            return await (await _accounts.FindAsync(account => account.Id == id)).FirstOrDefaultAsync();
        }

        public async Task<Account> Create(Account account)
        {
            await _accounts.InsertOneAsync(account);
            return account;
        }

        public async Task Update(string id, Account accountIn)
        {
            await _accounts.ReplaceOneAsync(account => account.Id == id, accountIn);
        }

        public async Task Remove(Account accountIn)
        {
            await _accounts.DeleteOneAsync(account => account.Id == accountIn.Id);
        }

        public async Task Remove(string id)
        {
            await _accounts.DeleteOneAsync(account => account.Id == id);
        }
    }
}