using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcProject.Models.UserModels;

namespace MvcProject.Data;

public class IdentityContext : IdentityDbContext<User>
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}