using FluentValidation.TestHelper;
using NUnit.Framework;
using PasswordGenerator;
using PasswordValidationApi.Models;
using PasswordValidationApi.Validators;
using System.Diagnostics;
using System.Linq;


namespace PasswordValidationUnitTests
{
    public class PasswordValidatorTester
    {
        private PasswordValidator validator;

        [SetUp]
        public void Setup()
        {
            validator = new PasswordValidator();
        }

        [Test]
        public void Should_Have_Error_When_Password_Is_Null()
        {
            var model = new PasswordModel { Password = null };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(password => password.Password)
                .WithErrorMessage(Messages.cant_be_empty);
        }

        [Test]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            var model = new PasswordModel { Password = "" };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(password => password.Password)
                .WithErrorMessage(Messages.cant_be_empty);
        }

        [Test]
        public void Should_Have_Error_When_Not_Satisfy_Any_Condition()
        {
            var model = new PasswordModel { Password = "______" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(password => password.Password)
                .WithErrorMessage(Messages.cant_has_repetead)
                .WithErrorMessage(Messages.must_has_upper_case)
                .WithErrorMessage(Messages.must_has_special_character)
                .WithErrorMessage(Messages.must_has_min_size)
                .WithErrorMessage(Messages.must_has_number)
                .WithErrorMessage(Messages.must_has_lower_case)
                .WithErrorMessage(Messages.do_not_match_all_rules);
        }

        [Test]
        public void Should_Have_Error_When_Satisfy_Only_Lowercase_Letters_Condition()
        {
            var model = new PasswordModel { Password = "aa" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(password => password.Password)
                .WithErrorMessage(Messages.cant_has_repetead)
                .WithErrorMessage(Messages.must_has_upper_case)
                .WithErrorMessage(Messages.must_has_special_character)
                .WithErrorMessage(Messages.must_has_min_size)
                .WithErrorMessage(Messages.must_has_number)
                .WithoutErrorMessage(Messages.must_has_lower_case);
        }

        [Test]
        public void Should_Have_Error_When_Satisfy_Only_Uppercase_Letters_Condition()
        {
            var model = new PasswordModel { Password = "AAAA" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(password => password.Password)
                .WithErrorMessage(Messages.cant_has_repetead)
                .WithErrorMessage(Messages.must_has_lower_case)
                .WithErrorMessage(Messages.must_has_special_character)
                .WithErrorMessage(Messages.must_has_min_size)
                .WithErrorMessage(Messages.must_has_number)
                .WithoutErrorMessage(Messages.must_has_upper_case);
        }

        [Test]
        public void Should_Have_Error_When_Satisfy_Only_Special_Character_Condition()
        {
            var model = new PasswordModel { Password = "++" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(password => password.Password)
                .WithErrorMessage(Messages.cant_has_repetead)
                .WithErrorMessage(Messages.must_has_lower_case)
                .WithErrorMessage(Messages.must_has_upper_case)
                .WithErrorMessage(Messages.must_has_min_size)
                .WithErrorMessage(Messages.must_has_number)
                .WithoutErrorMessage(Messages.must_has_special_character);
        }

        [Test]
        public void Should_Have_Error_When_Satisfy_Only_Has_Number_Condition()
        {
            var model = new PasswordModel { Password = "12345688" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(password => password.Password)
                .WithErrorMessage(Messages.cant_has_repetead)
                .WithErrorMessage(Messages.must_has_lower_case)
                .WithErrorMessage(Messages.must_has_upper_case)
                .WithErrorMessage(Messages.must_has_min_size)
                .WithErrorMessage(Messages.must_has_special_character)
                .WithoutErrorMessage(Messages.must_has_number);
        }

        [Test]
        public void Should_Have_Error_When_Repetead_Letters()
        {
            var model = new PasswordModel { Password = "AAAbbbCcc" };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(password => password.Password)
                   .WithErrorMessage(Messages.cant_has_repetead);
        }

        [Test]
        public void Should_Have_Error_When_Have_White_Space()
        {
            var model = new PasswordModel { Password = "AbTp9!fok " };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(password => password.Password)
                   .WithErrorMessage(Messages.do_not_match_all_rules);
        }

        [Test]
        public void Should_Have_Error_When_Have_Invalid_Characters()
        {
            var model = new PasswordModel { Password = "AbTp9!fok_" };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(password => password.Password)
                   .WithErrorMessage(Messages.do_not_match_all_rules);
        }

        [Test]
        public void Should_Have_Error_When_Do_Not_Have_Number()
        {
            var model = new PasswordModel { Password = "AbTcp!fok" };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(password => password.Password)
                   .WithErrorMessage(Messages.must_has_number);
        }

        [Test]
        public void Should_Success_With_All_Especial_Characters()
        {
            var especialCharacters = @"!@#$%^&*()-+";
            var model = new PasswordModel { Password = $"aB1{especialCharacters}" };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(password => password.Password);
        }

        [Test]
        public void Should_Not_Hang_With_Redos_Attack_With_Regex_Input()
        {
            var inputRegex = string.Format(PasswordValidator.baseMultipleRegexExpression,
                $"{PasswordValidator.hasNumberPattern}{PasswordValidator.hasUpperCasePattern}{PasswordValidator.hasLowerCasePattern}{PasswordValidator.hasEspecialCharacterPattern}");

            var model = new PasswordModel { Password = inputRegex };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(password => password.Password)
                   .WithErrorMessage(Messages.do_not_match_all_rules);
        }


        [Test]
        public void Should_Elapsed_Time_Less_Than_30_Milisencds_Per_Input()
        {
            const int iterations = 10000;

            var pwd = new Password().IncludeLowercase().IncludeUppercase().IncludeSpecial().IncludeNumeric().LengthRequired(9);

            for (int index = 0; index < iterations; index++)
            {
                var model = new PasswordModel { Password = pwd.Next() };

                var sw = new Stopwatch();
                sw.Start();
                //just test validation for performa purposes here no matter whether it was successful or not 
                _ = validator.TestValidate(model);
                //elapsed time should be less than 30 miliseconds per input.
                Assert.Less(sw.Elapsed.Milliseconds, 30);
            }
        }


        [Test]
        public void Should_Success()
        {
            var model = new PasswordModel { Password = "AbTp9!fok" };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(password => password.Password);
        }


        [Test]
        public void Should_Success_With_All_Possible_Characters()
        {
            var numbers = "0123456789";
            var lowerCaseLetters = "abcdefghijklmnopqrstuvxzyw";
            var upperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVXZYW";
            var especialCharacters = "!@#$%^&*()-+";
            var model = new PasswordModel { Password = $"{numbers}{lowerCaseLetters}{upperCaseLetters}{especialCharacters}" };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(password => password.Password);
        }


        [Test]
        public void Should_Success_Random_Test()
        {
            var pwd = new Password().IncludeLowercase().IncludeUppercase().IncludeSpecial().IncludeNumeric().LengthRequired(9);
            var randomPassword = pwd.Next();
            while (randomPassword.Distinct().Count() != randomPassword.Length)
                randomPassword = pwd.Next();

            var model = new PasswordModel { Password = randomPassword };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(password => password.Password);
        }
    }
}