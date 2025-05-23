﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace LorePdks.COMMON.Helpers
{
    public enum PasswordStrength
    {
        /// <summary>
        /// Blank Password (empty and/or space chars only)
        /// </summary>
        Blank = 0,
        /// <summary>
        /// Either too short (less than 5 chars), one-case letters only or digits only
        /// </summary>
        VeryWeak = 1,
        /// <summary>
        /// At least 5 characters, one strong condition met (>= 8 chars with 1 or more UC letters, LC letters, digits & special chars)
        /// </summary>
        Weak = 2,
        /// <summary>
        /// At least 5 characters, two strong conditions met (>= 8 chars with 1 or more UC letters, LC letters, digits & special chars)
        /// </summary>
        Medium = 3,
        /// <summary>
        /// At least 8 characters, three strong conditions met (>= 8 chars with 1 or more UC letters, LC letters, digits & special chars)
        /// </summary>
        Strong = 4,
        /// <summary>
        /// At least 8 characters, all strong conditions met (>= 8 chars with 1 or more UC letters, LC letters, digits & special chars)
        /// </summary>
        VeryStrong = 5
    }

    public static class PasswordCheck
    {
        /// <summary>
        /// Generic method to retrieve password strength: use this for general purpose scenarios, 
        /// i.e. when you don't have a strict policy to follow.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static PasswordStrength GetPasswordStrength(string password)
        {
            int score = 0;
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(password.Trim())) return PasswordStrength.Blank;
            if (HasMinimumLength(password, 5)) score++;
            if (HasMinimumLength(password, 8)) score++;
            if (HasUpperCaseLetter(password) && HasLowerCaseLetter(password)) score++;
            if (HasDigit(password)) score++;
            if (HasSpecialChar(password)) score++;
            return (PasswordStrength)score;
        }

        /// <summary>
        /// Sample password policy implementation:
        /// - minimum 8 characters
        /// - at lease one UC letter
        /// - at least one LC letter
        /// - at least one non-letter char (digit OR special char)
        /// </summary>
        /// <returns></returns>
        public static bool IsStrongPassword(string password)
        {
            return HasMinimumLength(password, 8)
                && HasUpperCaseLetter(password)
                && HasLowerCaseLetter(password)
                && (HasDigit(password) || HasSpecialChar(password));
        }

        /// <summary>
        /// Sample password policy implementation following the Microsoft.AspNetCore.Identity.PasswordOptions standard.
        /// </summary>
        //public static bool IsValidPassword(string password, PasswordOptions opts)
        //{
        //    return IsValidPassword(
        //        password,
        //        opts.RequiredLength,
        //        opts.RequiredUniqueChars,
        //        opts.RequireNonAlphanumeric,
        //        opts.RequireLowercase,
        //        opts.RequireUppercase,
        //        opts.RequireDigit);
        //}


        /// <summary>
        /// Sample password policy implementation following the Microsoft.AspNetCore.Identity.PasswordOptions standard.
        /// </summary>
        public static bool IsValidPassword(
            string password,
            int requiredLength,
            int requiredUniqueChars,
            bool requireNonAlphanumeric,
            bool requireLowercase,
            bool requireUppercase,
            bool requireDigit)
        {
            if (!HasMinimumLength(password, requiredLength)) return false;
            if (!HasMinimumUniqueChars(password, requiredUniqueChars)) return false;
            if (requireNonAlphanumeric && !HasSpecialChar(password)) return false;
            if (requireLowercase && !HasLowerCaseLetter(password)) return false;
            if (requireUppercase && !HasUpperCaseLetter(password)) return false;
            if (requireDigit && !HasDigit(password)) return false;
            return true;
        }

        public static List<string> IsValidPasswordV2(
           string password,
           int requiredLength,
           int requiredUniqueChars,
           bool requireNonAlphanumeric,
           bool requireLowercase,
           bool requireUppercase,
           bool requireDigit)
        {
            bool _HasMinimumLength = !HasMinimumLength(password, requiredLength) ? false : true;
            bool _HasMinimumUniqueChars = !HasMinimumUniqueChars(password, requiredUniqueChars) ? false : true;
            bool _HasSpecialChar = requireNonAlphanumeric && !HasSpecialChar(password) ? false : true;
            bool _HasLowerCaseLetter = requireLowercase && !HasLowerCaseLetter(password) ? false : true;
            bool _HasUpperCaseLetter = requireUppercase && !HasUpperCaseLetter(password) ? false : true;
            bool _HasDigit = requireDigit && !HasDigit(password) ? false : true;

            List<string> errorList = new List<string>();
            if (!_HasMinimumLength) { errorList.Add("Parola uzunluğu minimum " + requiredLength + " olmalıdır"); }
            if (!_HasMinimumUniqueChars) { errorList.Add("Parolanızda minimum " + requiredUniqueChars + " adet farklı karakter olmalıdır"); }
            if (!_HasSpecialChar) { errorList.Add("Parolanızda özel karakter yer almalıdır"); }
            if (!_HasLowerCaseLetter) { errorList.Add("Parolanızda küçük harf yer almalıdır"); }
            if (!_HasUpperCaseLetter) { errorList.Add("Parolanızda büyük harf yer almalıdır"); }
            if (!_HasDigit) { errorList.Add("Parolanızda rakam yer almalıdır"); }


            return errorList;
        }


        #region Helper Methods

        public static bool HasMinimumLength(string password, int minLength)
        {
            return password.Length >= minLength;
        }

        public static bool HasMinimumUniqueChars(string password, int minUniqueChars)
        {
            return password.Distinct().Count() >= minUniqueChars;
        }

        /// <summary>
        /// Returns TRUE if the password has at least one digit
        /// </summary>
        public static bool HasDigit(string password)
        {
            return password.Any(c => char.IsDigit(c));
        }

        /// <summary>
        /// Returns TRUE if the password has at least one special character
        /// </summary>
        public static bool HasSpecialChar(string password)
        {
            // return password.Any(c => char.IsPunctuation(c)) || password.Any(c => char.IsSeparator(c)) || password.Any(c => char.IsSymbol(c));
            return password.IndexOfAny("!@#$%^&*?_~-£().,".ToCharArray()) != -1;
        }

        /// <summary>
        /// Returns TRUE if the password has at least one uppercase letter
        /// </summary>
        public static bool HasUpperCaseLetter(string password)
        {
            return password.Any(c => char.IsUpper(c));
        }

        /// <summary>
        /// Returns TRUE if the password has at least one lowercase letter
        /// </summary>
        public static bool HasLowerCaseLetter(string password)
        {
            return password.Any(c => char.IsLower(c));
        }
        #endregion
    }
}