using Application.ViewModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class AddUserViewModelValidator : AbstractValidator<AddUserViewModel>
    {
        public AddUserViewModelValidator()
        {
            When(x => x.RoleName != "Admin", () =>
            {
                RuleFor(x => x.CareHomeId).Empty().WithMessage("Admins cannot be registered to a care home");
            });
            When(x => x.RoleName == "Manager", () =>
            {
                RuleFor(x => x.CareHomeId).NotEmpty().WithMessage("Please select the home of the manager");
            });
        }
    }

}
