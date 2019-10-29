using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Exceptions;
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
        private const string ServiceKey = "sk_test_dEYerF4aiezK453envsRBmWZ";
        private readonly IMapper _mapper;
        private readonly IAccountsRepository _accountsRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentservice _paymentservice;

        public PaymentsController(IPaymentservice paymentservice,
            IPaymentRepository paymentsRepository,
            IMapper mapper,
            IAccountsRepository accountsRepository)
        {
            _paymentservice = paymentservice;
            _paymentRepository = paymentsRepository;
            _mapper = mapper;
            _accountsRepository = accountsRepository;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> StartDeposit([FromBody] DepositCreationDto depositCreationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            try
            {
                StripeConfiguration.ApiKey = ServiceKey;
                var transferGroup = Guid.NewGuid().ToString();

                var service = new PaymentIntentService();
                var options = new PaymentIntentCreateOptions
                {
                    Amount = depositCreationDto.Amount,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>
                    {
                    "card"
                    },
                    Metadata = new Dictionary<string, string>()
                    { { "UserId", User.FindFirstValue(ClaimTypes.Name) }
                    },
                    TransferData = new PaymentIntentTransferDataOptions
                    {
                    Destination = (await _accountsRepository.GetByUserId(User.FindFirstValue(ClaimTypes.Name))).StripeUserId
                    }
                };
                var intent = service.Create(options);

                ;

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

        [HttpPost("withdraw")]
        public async Task<IActionResult> StartWithdraw([FromBody] WithdrawCreationDto withdrawCreationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            try
            {
                StripeConfiguration.ApiKey = ServiceKey;
                var user = await _accountsRepository.GetByUserId(withdrawCreationDto.UserId);

                var service = new PayoutService();
                var options = new PayoutCreateOptions
                {
                    Amount = withdrawCreationDto.Amount,
                    Currency = "usd",
                    Metadata = new Dictionary<string, string>() { { "UserId", User.FindFirstValue(ClaimTypes.Name) } },
                };
                var payout = service.Create(options, new RequestOptions { StripeAccount = user.StripeUserId });

                return Ok(new PayoutCreatedDto
                {
                    PayoutId = payout.Id
                });
                throw new InvalidPayment("Couldn't process withdraw");

            }
            catch (Exception e)
            {
                return BadRequest(new MessageObj(e.Message));
            }
        }

        [HttpGet("balance/user/{userId}")]
        public async Task<IActionResult> GetBalance([FromRoute] string userId)
        {
            try
            {
                StripeConfiguration.ApiKey = ServiceKey;
                var user = await _accountsRepository.GetByUserId(userId);

                var service = new BalanceService();
                Balance balance = await service.GetAsync(new RequestOptions { StripeAccount = user.StripeUserId });
                return Ok(balance);
            }
            catch (Exception e)
            {
                return BadRequest(new MessageObj(e.Message));
            }
        }

        [HttpGet("transactions/user/{userId}")]
        public async Task<IActionResult> GetBalanceHistory([FromRoute] string userId)
        {
            try
            {
                StripeConfiguration.ApiKey = ServiceKey;
                var user = await _accountsRepository.GetByUserId(userId);

                var service = new BalanceTransactionService();
                List<BalanceTransaction> transactions = (await service.ListAsync(new BalanceTransactionListOptions
                    {

                    },
                    new RequestOptions { StripeAccount = user.StripeUserId })).ToList();
                return Ok(transactions);
            }
            catch (Exception e)
            {
                return BadRequest(new MessageObj(e.Message));
            }
        }

        public async Task<IActionResult> Transfer([FromBody] TransferCreationDto transferCreationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            try
            {
                StripeConfiguration.ApiKey = ServiceKey;

                // The sender is usually the employer and the freelancer the receiver. 
                //This is just to expose a generic interface
                var senderAccount = await _accountsRepository.GetByUserId(transferCreationDto.SenderId);
                var receiverAccount = await _accountsRepository.GetByUserId(transferCreationDto.ReceiverId);

                var service = new TransferService();
                var options = new TransferCreateOptions
                {
                    Amount = transferCreationDto.Amount,
                    Currency = "usd",
                    Destination = receiverAccount.StripeUserId
                };
                var transfer = service.Create(options, new RequestOptions { StripeAccount = senderAccount.StripeUserId });

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new MessageObj(e.Message));
            }
        }
    }
}