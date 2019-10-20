using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Application.Controllers
{
    [Route("webhook")]
    [ApiController]
    public class WebhookController : Controller
    {
        const string secret = "sk_test_dEYerF4aiezK453envsRBmWZ";
        private readonly ILogger<WebhookController> logger;

        public WebhookController(ILogger<WebhookController> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        public IActionResult Post()
        {
            try
            {
                var json = new StreamReader(HttpContext.Request.Body).ReadToEnd();
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], secret);

                PaymentIntent intent = null;

                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        intent = (PaymentIntent) stripeEvent.Data.Object;
                        logger.LogInformation("Succeeded: {ID}", intent.Id);
                        break;
                    case "payment_intent.failed":
                        intent = (PaymentIntent) stripeEvent.Data.Object;
                        logger.LogInformation("Failure: {ID}", intent.Id);
                        break;
                    default:
                        break;
                }

                return new EmptyResult();
            }
            catch (StripeException e)
            {
                return BadRequest()
            }
        }
    }
}