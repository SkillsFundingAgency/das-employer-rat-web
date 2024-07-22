﻿using Microsoft.AspNetCore.Http;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.UserAccounts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.Home
{
    [ExcludeFromCodeCoverage]
    public class SignedInStubViewModel
    {
        public const string HashedAccountIdPlaceholder = "{{hashedAccountId}}";
        private readonly ClaimsPrincipal _claimsPrinciple;

        public SignedInStubViewModel(IHttpContextAccessor httpContextAccessor, string returnUrl)
        {
            _claimsPrinciple = httpContextAccessor.HttpContext.User;
            ReturnUrl = returnUrl;
        }

        public string StubEmail => _claimsPrinciple.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value;
        public string StubId => _claimsPrinciple.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier))?.Value;

        public string ReturnUrl { get; }

        public bool HasHashedAccountIdPlaceholder()
        {
            return Uri.UnescapeDataString(ReturnUrl).Contains(HashedAccountIdPlaceholder);
        }

        public string ReplaceHashedAccountIdPlaceholderUrl(string hashedAccountId)
        {
            string replacedUrl = Regex.Replace(Uri.UnescapeDataString(ReturnUrl), 
                HashedAccountIdPlaceholder, 
                hashedAccountId, 
                RegexOptions.None, 
                TimeSpan.FromMilliseconds(25));

            return replacedUrl;
        }

        public List<EmployerUserAccount> GetAccounts()
        {
            var associatedAccountsClaim = _claimsPrinciple.Claims.FirstOrDefault(c => c.Type.Equals(EmployerClaims.UserAssociatedAccountsClaimsTypeIdentifier))?.Value;
            if (string.IsNullOrEmpty(associatedAccountsClaim))
                return new List<EmployerUserAccount>();

            try
            {
                var accountsDictionary = JsonSerializer.Deserialize<Dictionary<string, EmployerUserAccount>>(associatedAccountsClaim);
                return accountsDictionary?.Values.ToList() ?? new List<EmployerUserAccount>();
            }
            catch (JsonException)
            {
                return new List<EmployerUserAccount>();
            }
        }
    }
}
