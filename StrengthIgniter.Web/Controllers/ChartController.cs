using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StrengthIgniter.Core.Models;
using StrengthIgniter.Core.Services;
using StrengthIgniter.Web.Extentions;

namespace StrengthIgniter.Web.Controllers
{
    public class ChartController : Controller
    {
        #region CTOR

        private readonly IExerciseService _ExerciseService;
        public ChartController(
            IExerciseService exerciseService
        )
        {
            _ExerciseService = exerciseService;
        }

        #endregion

        //All actions for getting and returning data for chart generation here

        [Route("chart/max/{reference}")]
        public IActionResult Max(Guid reference)
        {
            IEnumerable<RecordModel> records = _ExerciseService.GetBestRecordPerDay(reference, User.GetNameIdentifier());
            return Json(records.Select(r => new {
                r.Date,
                r.Reps,
                r.WeightKg,
                r.RPE,
                r.e1RM,
                r.RPEMax
            }));
        }
    }
}
