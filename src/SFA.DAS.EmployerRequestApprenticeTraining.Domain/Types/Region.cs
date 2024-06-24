﻿namespace SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types
{
    public class Region
    {
        public int Id { get; set; }
        public string SubregionName { get; set; }
        public string RegionName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
