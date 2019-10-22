using Application.Repositories;
using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentservice _paymentservice;

        public TransactionsController(IPaymentservice paymentservice, IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentservice = paymentservice;
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }
    }
}