namespace PasswordValidationApi.Validators
{
    public static class Messages
    {
        public static readonly string cant_be_empty = "The password can't be empty.";
        public static readonly string must_has_min_size = "The password must have 9 or more character.";
        public static readonly string must_has_number = "The password must have one number.";
        public static readonly string must_has_upper_case = "The password must have one upper case letter.";
        public static readonly string must_has_lower_case = "The password must have one lower case letter.";
        public static readonly string must_has_special_character = "The password must have one special character.";
        public static readonly string cant_has_repetead = "The password can't have repetead characters.";
        public static readonly string do_not_match_all_rules = "The password does not matches all secuty requirements.";
    }
}
