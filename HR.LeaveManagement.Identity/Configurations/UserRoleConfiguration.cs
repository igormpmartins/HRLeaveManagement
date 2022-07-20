using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.LeaveManagement.Identity.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "257D0A46-FD9B-479D-9B49-61282D009EC8",
                    UserId = "C5BBCF6B-1AD9-4C80-85A8-942E37FFC469"
                },                
                new IdentityUserRole<string>
                {
                    RoleId = "48110830-2686-4988-8F87-7556C34B3FFA",
                    UserId = "4C587C41-AF0A-42C0-A913-72DF01CB04B8"
                }
            );
        }
    }
}
