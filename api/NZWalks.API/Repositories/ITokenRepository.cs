using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace NZWalks.API.Repositories
{
    public interface ITokenRepository
    {
        public string CreateJwtToken(IdentityUser user, List<string> roles);
    }
}