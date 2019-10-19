using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Models.DataTransferObjects;
using Application.Models.Entities;
using Application.Repositories;
using Application.Utility.ClientLibrary;
using Application.Utility.ClientLibrary.Project;
using Application.Utility.Exception;
using Application.Utility.Models;
using Newtonsoft.Json;

namespace Application.Services
{
    public interface IPaymentservice
    {
        Task<Payment> Open(Payment payment);
        Task<bool> Accept(Payment payment, string authorizationToken);
    }

    public class Paymentservice : IPaymentservice
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IClient _client;

        public Paymentservice(IPaymentRepository paymentRepository, IClient client)
        {
            _paymentRepository = paymentRepository;
            _client = client;
        }

        public async Task<Payment> Open(Payment payment)
        {
            var project = await _client.GetProjectAsync(payment.ProjectId);
            if (project == null) throw new InvalidPayment();

            var createdPayment = await _paymentRepository.Create(payment);

            return createdPayment ??
                throw new InvalidPayment();
        }

        public async Task<bool> Accept(Payment payment, string authorizationToken)
        {
            var project = await _client.GetProjectAsync(payment.ProjectId);
            if (project == null) throw new InvalidPayment("projectId invalid");

            project.FreelancerId = payment.FreelancerId;

            return await _client.UpdateProjectAsync(authorizationToken, project);
        }
    }
}