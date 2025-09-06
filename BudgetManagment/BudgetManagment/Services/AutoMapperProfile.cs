using AutoMapper;
using BudgetManagment.Models;

namespace BudgetManagment.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Account, AccountCreationViewModel>();
        }
    }
}
