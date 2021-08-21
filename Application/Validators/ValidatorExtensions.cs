using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators
{
    public static class ValidatorExtensions
    {
       
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .NotEmpty()
                .MinimumLength(8).WithMessage("password should have at least 8")
                .Matches("[A-za-z]").WithMessage("password should have one capital letter")
                .Matches("[1-9]").WithMessage("password should have numbers")
                .Matches("[^a-zA-Z0-9]").WithMessage("password should have alphanumeric only");
            //best practice to make const string and save regx into it
            return options;
        }
    }
}
