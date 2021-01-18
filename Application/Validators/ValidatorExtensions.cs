using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators
{
    public static class ValidatorExtensions
    {
        public const string Phone = "(^[0-9-]*$)|(^[-٠-٩]*$)";
        public const string Numbers = "^[0-9-]*$";
        public const string CommercialRegistry = "^[0-9]*$";
        public const string Email = @"^(?("")("".+?(?<!\\)""@)|(([0 - 9a - z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
        public const string ArabicCharacters = @"^[\u0600-\u06ff ]*$";
        public const string ArabicCharactersWithNumbers = @"^[\u0600-\u06ff]^[0-9]*$";
        public const string EnglishCharacters = "^[a-zA-Z ]*$";

        public const string EnglishAndArabicCharactersWithWhiteSpace = @"^[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FFa-zA-Z]+[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FFa-zA-Z-_\ ]*$";
        public const string EnglishAndArabicCharactersOnly = @"^[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FFa-zA-Z]+[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FFa-zA-Z-_]*$";
        public const string EnglishCharactersWithNumbers = "^[a-zA-Z0-9 ]*$";

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
