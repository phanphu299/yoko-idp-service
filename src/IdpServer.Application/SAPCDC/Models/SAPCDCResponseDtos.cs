using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IdpServer.Application.SAPCDC.Model
{
    public class BaseSAPCDCResponseDto
    {
        public string CallId { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorDetails { get; set; }
        public string ErrorMessages { get; set; }
        public int ApiVersion { get; set; }
        public int StatusCode { get; set; }
        public string StatusReason { get; set; }
        public DateTime Time { get; set; }
        public IEnumerable<ValidationError> ValidationErrors { get; set; }

        public class ValidationError
        {
            public int ErrorCode { get; set; }
            public string Message { get; set; }
            public string FieldName { get; set; }
        }
    }

    public class RegTokenResponseDto : BaseSAPCDCResponseDto
    {
        public string RegToken { get; set; }
    }

    public class PasswordResetTokenResponseDto : BaseSAPCDCResponseDto
    {
        public string PasswordResetToken { get; set; }
    }

    public class SearchResponseDto : BaseSAPCDCResponseDto
    {
        public int ObjectsCount { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<SearchResultDto> Results { get; set; }
        public class SearchResultDto
        {
            public string UId { get; set; }
            public UserDataDto Data { get; set; }
            public UserProfileDto Profile { get; set; }
            public bool IsRegistered { get; set; }
        }
    }

    public class UserDetailsResponseDto : BaseSAPCDCResponseDto
    {
        public string UId { get; set; }
        public UserDataDto Data { get; set; }
        public UserProfileDto Profile { get; set; }
        public bool IsRegistered { get; set; }
        public bool IsActive { get; set; }
        public string SocialProviders { get; set; }
    }

    public class UserDataDto
    {
        [JsonProperty(PropertyName = "environment")]
        public string Environment { get; set; }
        [JsonProperty(PropertyName = "phoneNumber")]
        public string PhoneNumber { get; set; }
        [JsonProperty(PropertyName = "tenantId")]
        public string TenantId { get; set; }
        [JsonProperty(PropertyName = "subscriptionId")]
        public string SubscriptionId { get; set; }
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }
    }

    public class UserProfileDto
    {
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
    }
}