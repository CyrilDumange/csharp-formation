using auth.models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace auth.webapp.Auth
{
    public class AuthContext(DbContextOptions<AuthContext> options) :
        IdentityDbContext<AuthUser>(options)
    {
    }
}