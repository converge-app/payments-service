using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Helpers;
using Application.Models.DataTransferObjects;
using Application.Models.Entities;
using Application.Repositories;
using Application.Services;
using Application.Utility;
using Application.Utility.Exception;
using Application.Utility.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

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

        [HttpPost]
        public async Task<IActionResult> OpenPayment([FromBody] PaymentCreationDto paymentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var createPayment = _mapper.Map<Payment>(paymentDto);
            try
            {
                var createdPayment = await _paymentservice.Open(createPayment);
                return Ok(createdPayment);
            }
            catch (UserNotFound)
            {
                return NotFound(new MessageObj("User not found"));
            }
            catch (EnvironmentNotSet)
            {
                throw;
            }
            catch (Exception e)
            {
                return BadRequest(new MessageObj(e.Message));
            }
        }

        [HttpPut("{paymentId}")]
        public async Task<IActionResult> AcceptPayment([FromHeader] string authorization, [FromRoute] string paymentId, [FromBody] PaymentUpdateDto paymentDto)
        {
            if (paymentId != paymentDto.Id)
                return BadRequest(new MessageObj("Invalid id(s)"));

            if (!ModelState.IsValid)
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var updatePayment = _mapper.Map<Payment>(paymentDto);
            try
            {
                if (await _paymentservice.Accept(updatePayment, authorization.Split(' ') [1]))
                    return Ok();
                throw new InvalidPayment();
            }
            catch (Exception e)
            {
                return BadRequest(new MessageObj(e.Message));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _paymentRepository.Get();
            var paymentDtos = _mapper.Map<IList<PaymentDto>>(payments);
            return Ok(paymentDtos);
        }

        [HttpGet("freelancer/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByFreelancerId(string id)
        {
            var payments = await _paymentRepository.GetByFreelancerId(id);
            var paymentsDto = _mapper.Map<PaymentDto>(payments);
            return Ok(paymentsDto);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string id)
        {
            var payment = await _paymentRepository.GetById(id);
            var paymentDto = _mapper.Map<PaymentDto>(payment);
            return Ok(paymentDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _paymentRepository.Remove(id);
            }
            catch (Exception e)
            {
                return BadRequest(new MessageObj(e.Message));
            }

            return Ok();
        }
    }
}