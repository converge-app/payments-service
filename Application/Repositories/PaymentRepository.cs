using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Database;
using Application.Models.Entities;
using MongoDB.Driver;

namespace Application.Repositories
{
    public interface IPaymentRepository
    {
        Task<List<Payment>> Get();
        Task<Payment> GetById(string id);
        Task<List<Payment>> GetByProject(string projectId);
        Task<List<Payment>> GetByFreelancerId(string freelancerId);
        Task<List<Payment>> GetByProjectAndFreelancer(string projectId, string freelancerId);
        Task<Payment> Create(Payment payment);
        Task Update(string id, Payment paymentIn);
        Task Remove(Payment paymentIn);
        Task Remove(string id);
    }

    public class PaymentRepository : IPaymentRepository
    {
        private readonly IMongoCollection<Payment> _payments;

        public PaymentRepository(IDatabaseContext dbContext)
        {
            if (dbContext.IsConnectionOpen())
                _payments = dbContext.Payments;
        }

        public async Task<List<Payment>> Get()
        {
            return await (await _payments.FindAsync(payment => true)).ToListAsync();
        }

        public async Task<Payment> GetById(string id)
        {
            return await (await _payments.FindAsync(payment => payment.Id == id)).FirstOrDefaultAsync();
        }

        public async Task<List<Payment>> GetByProject(string projectId)
        {
            return await (await _payments.FindAsync(payment => payment.ProjectId == projectId)).ToListAsync();
        }

        public async Task<List<Payment>> GetByFreelancerId(string freelancerId)
        {
            return await (await _payments.FindAsync(payment => payment.FreelancerId == freelancerId)).ToListAsync();
        }

        public async Task<List<Payment>> GetByProjectAndFreelancer(string projectId, string freelancerId)
        {
            return await (
                await _payments.FindAsync(
                    payment => payment.ProjectId == projectId && payment.FreelancerId == freelancerId)
            ).ToListAsync();
        }

        public async Task<Payment> Create(Payment payment)
        {
            await _payments.InsertOneAsync(payment);
            return payment;
        }

        public async Task Update(string id, Payment paymentIn)
        {
            await _payments.ReplaceOneAsync(payment => payment.Id == id, paymentIn);
        }

        public async Task Remove(Payment paymentIn)
        {
            await _payments.DeleteOneAsync(payment => payment.Id == paymentIn.Id);
        }

        public async Task Remove(string id)
        {
            await _payments.DeleteOneAsync(payment => payment.Id == id);
        }
    }
}