using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetLocations;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.Location;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Controllers
{
    [Route("[controller]")]
    public class LocationsController : Controller
    {
        private readonly IMediator _mediator;

        public LocationsController (
            IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Locations([FromQuery]string searchTerm)
        {
            var result = await _mediator.Send(new GetLocationsQuery(searchTerm));
           
            var model = new LocationsViewModel
            {
                Locations = result.LocationItems?.Select(c => (LocationViewModel)c).ToList()
            };

            return new JsonResult(model);
        }
    }
}