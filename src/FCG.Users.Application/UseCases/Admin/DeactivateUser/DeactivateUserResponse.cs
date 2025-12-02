using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Users.Application.UseCases.Admin.DeactivateUser
{
    public record DeactivateUserResponse(Guid Id, bool IsActive);

}
