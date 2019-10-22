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
    public interface IPaymentservice { }

    public class Paymentservice : IPaymentservice
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IClient _client;

        public Paymentservice(IPaymentRepository paymentRepository, IClient client)
        {
            _paymentRepository = paymentRepository;
            _client = client;
        }
    }
}