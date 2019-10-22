using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Models.DataTransferObjects;
using Application.Repositories;
using Application.Services;
using Application.Utility.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Application.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentservice _paymentservice;

        public PaymentsController(IPaymentservice paymentservice, IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentservice = paymentservice;
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> StartDeposit([FromBody] DepositCreationDto depositCreationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            try
            {
                StripeConfiguration.ApiKey = "sk_test_dEYerF4aiezK453envsRBmWZ";

                var service = new PaymentIntentService();
                var options = new PaymentIntentCreateOptions
                {
                    Amount = depositCreationDto.Amount,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>
                    {
                    "card"
                    },
                    Metadata = new Dictionary<string, string>() { { "UserId", User.FindFirstValue(ClaimTypes.Name) } }
                };
                var intent = service.Create(options);

                return Ok(new PaymentIntentCreatedDto
                {
                    ClientSecret = intent.ClientSecret
                });
            }
            catch (Exception e)
            {
                return BadRequest(new MessageObj(e.Message));
            }
        }
    }
}