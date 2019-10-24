using System.IO;
using Application.Repositories;
using Application.Utility.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Application.Controllers
{
    [Route("webhook")]
    [ApiController]
    public class WebhookController : Controller
    {
        const string secret = "whsec_wx5T4KMQt2SVbEytgs2GecG7zXT0Tp2z";
        private readonly ILogger<WebhookController> _logger;
        private readonly IAccountsRepository _accountsRepository;

        public WebhookController(
            ILogger<WebhookController> logger,
            IAccountsRepository accountsRepository)
        {
            _logger = logger;
            _accountsRepository = accountsRepository;
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> PostAsync()
        {
            try
            {
                var json = new StreamReader(HttpContext.Request.Body).ReadToEnd();
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], secret);

                PaymentIntent intent = null;

                switch (stripeEvent.Type)
                {
                    case "payment_intent.created":
                        intent = (PaymentIntent) stripeEvent.Data.Object;
                        _logger.LogInformation("Created: {ID}", intent.Id);
                        break;
                    case "payment_intent.succeeded":
                        intent = (PaymentIntent) stripeEvent.Data.Object;
                        _logger.LogInformation("Succeeded: {ID}", intent.Id);

                        var account = await _accountsRepository.GetByUserId(intent.Metadata["UserId"]);

                        var transferService = new TransferService();
                        var transferOptions = new TransferCreateOptions
                        {
                            Amount = intent.Amount,
                            Currency = "usd",
                            Destination = account.StripeUserId,
                            TransferGroup = intent.Metadata["TransferGroup"]
                        };
                        transferService.Create(transferOptions);
                        break;
                    case "payment_intent.failed":
                        intent = (PaymentIntent) stripeEvent.Data.Object;
                        _logger.LogInformation("Failure: {ID}", intent.Id);
                        break;
                    default:
                        break;
                }

                return new EmptyResult();
            }
            catch (StripeException e)
            {
                return BadRequest(new MessageObj(e.Message));
            }
        }
    }
}