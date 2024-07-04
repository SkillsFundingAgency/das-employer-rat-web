using SFA.DAS.EmployerRequestApprenticeTraining.Web.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class EnterMultipleLocationsEmployerRequestViewModel : SubmitEmployerRequestViewModel
    {
        private readonly List<RegionViewModel> AllRegions;
        
        public string[] SelectedSubRegions { get; set; }

        public EnterMultipleLocationsEmployerRequestViewModel()
        {
            AllRegions = new List<RegionViewModel>();
        }
        
        public EnterMultipleLocationsEmployerRequestViewModel(List<RegionViewModel> allRegions)
        {
            AllRegions = allRegions ?? new List<RegionViewModel>();
        }

        public IEnumerable<IGrouping<string, RegionViewModel>> SubregionsGroupedByRegions
        {
            get
            {
                foreach (var region in AllRegions)
                {
                    region.IsSelected = SelectedSubRegions.Contains(region.Id.ToString());
                }

                return AllRegions.GroupBy(x => x.RegionName).OrderBy(x => x.Key);
            }
        }

        public string BackRoute
        {
            get
            {
                if (BackToCheckAnswers)
                    return EmployerRequestController.CheckYourAnswersRouteGet;

                return EmployerRequestController.EnterSameLocationRouteGet;
            }
        }
    }
}
