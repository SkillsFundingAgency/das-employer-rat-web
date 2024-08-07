﻿using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetLocations
{
    public class GetLocationsQuery : IRequest<List<LocationSearchResult>>
    {
        public GetLocationsQuery(string searchTerm)
        {
            SearchTerm = searchTerm;
        }

        public string SearchTerm { get; }
    }
}