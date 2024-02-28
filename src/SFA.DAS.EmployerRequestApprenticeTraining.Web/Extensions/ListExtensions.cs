﻿using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Extensions
{
    public static class ListExtensions
    {
        public static bool IsComplete(this List<string> list)
        {
            return list != null && list.Any();
        }
    }
}