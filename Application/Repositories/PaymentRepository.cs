using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Database;
using Application.Models.Entities;
using MongoDB.Driver;

namespace Application.Repositories
{
    public interface IPaymentRepository
    {
        Task<List<Transaction>> Get();
        Task<Transaction> GetById(string id);
        Task<Transaction> Create(Transaction payment);
        Task Update(string id, Transaction paymentIn);
        Task Remove(Transaction paymentIn);
        Task Remove(string id);
    }

    public class PaymentRepository : IPaymentRepository
    {
        private readonly IMongoCollection<Transaction> _payments;

        public PaymentRepository(IDatabaseContext dbContext)
        {
            if (dbContext.IsConnectionOpen())
                _payments = dbContext.Payments;
        }

        public async Task<List<Transaction>> Get()
        {
            return await (await _payments.FindAsync(payment => true)).ToListAsync();
        }

        public async Task<Transaction> GetById(string id)
        {
            return await (await _payments.FindAsync(payment => payment.Id == id)).FirstOrDefaultAsync();
        }

        public async Task<Transaction> Create(Transaction payment)
        {
            await _payments.InsertOneAsync(payment);
            return payment;
        }

        public async Task Update(string id, Transaction paymentIn)
        {
            await _payments.ReplaceOneAsync(payment => payment.Id == id, paymentIn);
        }

        public async Task Remove(Transaction paymentIn)
        {
            await _payments.DeleteOneAsync(payment => payment.Id == paymentIn.Id);
        }

        public async Task Remove(string id)
        {
            await _payments.DeleteOneAsync(payment => payment.Id == id);
        }
    }
}