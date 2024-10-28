using Microsoft.AspNetCore.Identity;
using Playmove.DAO.Models;

namespace Playmove.DAO.IService
{
	public interface IServiceToken
    {
        DateTime GetExpiryTimestamp(string accessToken);
        string GenerateToken(IdentityUser user, List<string> currentRoles);
    }
}
