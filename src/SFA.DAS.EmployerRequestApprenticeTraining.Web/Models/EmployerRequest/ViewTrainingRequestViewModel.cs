using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class ViewTrainingRequestViewModel : ITrainingOptionsViewModel, ILocationsViewModel
    {
        public string HashedAccountId { get; set; }
        public Guid EmployerRequestId { get; set; }
        public string StandardTitle { get; set; }
        public int StandardLevel { get; set; }
        public int NumberOfApprentices { get; set; }
        public string SameLocation { get; set; }
        public string SingleLocation { get; set; }
        public bool AtApprenticesWorkplace { get; set; }
        public bool DayRelease { get; set; }
        public bool BlockRelease { get; set; }
        public DateTime RequestedAt { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public DateTime ExpiryAt { get; set; }
        public DateTime RemoveAt { get; set; }

        public List<Region> Regions { get; set; }
        public List<ProviderResponse> ProviderResponses { get; set; }

        public string BackFragment
        {
            get
            {
                if (Status == RequestStatus.Active)
                    return "active-requests";
                else if (Status == RequestStatus.Expired)
                    return "expired-requests";

                return string.Empty;
            }
        }

        public string CreateAbsoluteHrefLink(string url)
        {
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                url = "https://" + url;
            }

            return url;
        }

    }
}
