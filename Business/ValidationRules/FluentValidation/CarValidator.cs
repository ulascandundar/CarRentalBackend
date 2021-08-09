using Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.ValidationRules.FluentValidation
{
    public class CarValidator : AbstractValidator<Car>
    {
        public CarValidator()
        {
            RuleFor(c => c.BrandId).NotEmpty();
            //RuleFor(c => c.Description).Must(StartWith);

        }

        private bool StartWith(string arg)
        {
            return arg.StartsWith("A");
        }
    }
}
