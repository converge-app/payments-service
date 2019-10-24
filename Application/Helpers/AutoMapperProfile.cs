using Application.Models.DataTransferObjects;
using Application.Models.Entities;
using AutoMapper;

namespace Application.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Transaction, PaymentDto>();
            CreateMap<PaymentDto, Transaction>();
            CreateMap<PaymentUpdateDto, Transaction>();
            CreateMap<PaymentCreationDto, Transaction>();
            CreateMap<Account, CreatedAccountDto>();
            CreateMap<Account, AccountDto>();
        }
    }
}