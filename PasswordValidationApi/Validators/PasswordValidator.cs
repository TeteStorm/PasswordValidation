using FluentValidation;
using PasswordValidationApi.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PasswordValidationApi.Validators
{
    public class PasswordValidator : AbstractValidator<PasswordModel>
    {
        public static readonly string hasNumberPattern = "0-9";
        public static readonly string hasUpperCasePattern = "A-Z";
        public static readonly string hasLowerCasePattern = "a-z";
        public static readonly string hasEspecialCharacterPattern = "!@#$%^&*()+-";

        public static readonly string baseSingleRegexExpression = "[{0}]+?";
        /// [ ] Character group.
        ///  +? Matches the previous element one or more times, but as few times as possible (lazy quantifiers).
        public static readonly string baseMultipleRegexExpression = $"^{baseSingleRegexExpression}$";
        ///  ^  Begin the match at the beginning of the string.
        ///  $  End the match at the end of the string.
        private readonly TimeSpan matchTimeout = new TimeSpan(100);

        public PasswordValidator()
        {
            //Continue mode to aggregate all erros 
            CascadeMode = CascadeMode.Continue;

            //Using regex with timeout follow Microsoft warning about process untrusted input and Denial-of-Service attack 
            var HasNumber = new Regex(string.Format(baseSingleRegexExpression, hasNumberPattern), RegexOptions.None, matchTimeout); ;
            var HasUpperCase = new Regex(string.Format(baseSingleRegexExpression, hasUpperCasePattern), RegexOptions.None, matchTimeout);
            var HasLowerCase = new Regex(string.Format(baseSingleRegexExpression, hasLowerCasePattern), RegexOptions.None, matchTimeout);
            var HasEspecialCharacter = new Regex(string.Format(baseSingleRegexExpression, hasEspecialCharacterPattern), RegexOptions.None, matchTimeout);

            RuleFor(x => x.Password)
                //basic null rules
                .NotNull().WithMessage(Messages.cant_be_empty)
                .NotEmpty().WithMessage(Messages.cant_be_empty)
                //max size to 74 (26 upper + 26 lower + 10 numbers + 12 special characters not repeated) 
                .Length(9, 74).WithMessage(Messages.must_has_min_size)
                //restricts requires rules, ensure each must fields
                .Matches(HasNumber).WithMessage(Messages.must_has_number)
                .Matches(HasUpperCase).WithMessage(Messages.must_has_upper_case)
                .Matches(HasLowerCase).WithMessage(Messages.must_has_lower_case)
                .Matches(HasEspecialCharacter).WithMessage(Messages.must_has_special_character)
                //repeated check 
                .Must(HaventRepeated).WithMessage(Messages.cant_has_repetead)
                //restrict without require, ensure just allowed fields
                .Must(HasValidPassword).WithMessage(Messages.do_not_match_all_rules);
        }

        /// <summary>
        /// Ensure that password do not have repeated characters 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private static bool HaventRepeated(string password)
        {
            return password == null || password.Distinct().Count() == password.Length;
        }

        /// <summary>
        /// Ensures that the sentence has only allowed characters using the constants rules, not restrict mode
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool HasValidPassword(string password)
        {
            var regexPattern = string.Format(baseMultipleRegexExpression, $"{hasNumberPattern}{hasUpperCasePattern}{hasLowerCasePattern}{hasEspecialCharacterPattern}");
            
            return (password != null && Regex.IsMatch(password, regexPattern, RegexOptions.None, matchTimeout));
        }
    }
}
