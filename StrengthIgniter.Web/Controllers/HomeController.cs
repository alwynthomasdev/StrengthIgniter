using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StrengthIgniter.Core.Models;
using StrengthIgniter.Core.Services;
using StrengthIgniter.Web.Extentions;
using StrengthIgniter.Web.Models;

namespace StrengthIgniter.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IExerciseService _ExerciseService;

        public HomeController(
            IExerciseService exerciseService
        )
        {
            _ExerciseService = exerciseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SquatChart()
        {
            IEnumerable<RecordModel> records = _ExerciseService.GetBestRecordPerDay(Guid.Parse("63CCE723-DAD9-4AE7-9F0E-EFDA279FCEFE"), User.GetNameIdentifier());
            return Json(records.Select(r=>new { 
                r.Date, 
                r.Reps, 
                r.WeightKg,
                r.RPE,
                r.e1RM, 
                r.RPEMax 
            }));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
