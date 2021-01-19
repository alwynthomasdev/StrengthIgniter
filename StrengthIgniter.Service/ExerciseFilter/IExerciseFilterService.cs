using StrengthIgniter.Models.Exercise;
using StrengthIgniter.Service.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.ExerciseFilter
{
    public interface IExerciseFilterService
    {
        FilterResponse<ExerciseModel> Filter(FilterRequest request);
    }
}
