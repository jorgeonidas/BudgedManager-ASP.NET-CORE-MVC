using System.Security.Claims;

namespace BudgetManagment.Services
{
    public class UsersService : IUsersService
    {
        private readonly HttpContext _httpContext;
        public UsersService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public int GetUserId()
        {
            if (_httpContext.User.Identity.IsAuthenticated)
            {
                var idClaim = _httpContext.User.Claims.
                                Where(x => x.Type == ClaimTypes.NameIdentifier)
                                .FirstOrDefault();
                var id = int.Parse(idClaim.Value);
                return id;
            }
            else
            {
                throw new ApplicationException("The user is not authenticated.");
            }
        }
    }
}
