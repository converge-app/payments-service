using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Models.DataTransferObjects;
using Application.Repositories;
using Application.Services;
using Application.Utility.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using Account = Application.Models.Entities.Account;

namespace Application.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AccountsController> _logger;
        private readonly IAccountsRepository _accountsRepository;

        public AccountsController(
            IAccountsRepository paymentRepository,
            IMapper mapper,
            ILogger<AccountsController> logger)
        {
            _accountsRepository = paymentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccountAsync([FromBody] CreateAccountDto createAccountDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            StripeConfiguration.ApiKey = "sk_test_dEYerF4aiezK453envsRBmWZ";

            try
            {
                var options = new OAuthTokenCreateOptions
                {
                    GrantType = "authorization_code",
                    Code = createAccountDto.AuthorizationCode
                };

                var service = new OAuthTokenService();
                var response = await service.CreateAsync(options);

                if (response != null)
                {
                    var account = new Account
                    {
                    UserId = createAccountDto.UserId,
                    AccessToken = response.AccessToken,
                    LiveMode = response.Livemode,
                    RefreshToken = response.RefreshToken,
                    StripePublisableKey = response.StripePublishableKey,
                    StripeUserId = response.StripeUserId,
                    Scope = response.Scope
                    };

                    var createdAccount = await _accountsRepository.Create(account);
                    var createdAccountDto = _mapper.Map<CreatedAccountDto>(createdAccount);

                    return Ok(response.StripeUserId);
                }

                var deleteService = new AccountService();
                deleteService.Delete(response.StripeUserId)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new MessageObj(ex.Message));
            }
        }
    }
}