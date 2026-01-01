using Microsoft.AspNetCore.Identity;

namespace Services.Interfaces
{
    public interface IRoleService
    {
      public Task<List<IdentityRole>> GetAllRolesAsync(); 
    }
}