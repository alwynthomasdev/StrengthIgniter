using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeFluff.Extensions.IEnumerable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StrengthIgniter.Core.Models;
using StrengthIgniter.Core.Services;
using StrengthIgniter.Web.Extentions;
using StrengthIgniter.Web.Models.Configuration;

namespace StrengthIgniter.Web.Controllers
{
    public class ExerciseController : Controller
    {
        #region CTOR

        private readonly IExerciseService _ExerciseService;
        private readonly ExerciseSearchConfig _ExerciseSearchConfig;

        public ExerciseController(
            IExerciseService exerciseService,
            IOptions<ExerciseSearchConfig> exerciseSearchConfigOptions
        )
        {
            _ExerciseService = exerciseService;
            _ExerciseSearchConfig = exerciseSearchConfigOptions.Value;
        }

        #endregion

        public IActionResult Index()
        {
            return View();
        }

        [Route("exercise/{reference}")]
        public IActionResult Index(Guid reference)
        {
            ExerciseModel exercise = _ExerciseService.GetByReference(reference);
            return View("Exercise", exercise);
        }

        #region Paged Action Methods

        public IActionResult PagedSearch(int pageNumber, int pageLength, string searchString = "")
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageLength < 1 || pageLength > _ExerciseSearchConfig.MaxPageLength)
            {
                pageLength = _ExerciseSearchConfig.DefaultPageLength;
            }

            ExerciseSearchResponse response =  _ExerciseService.Search(new ExerciseSearchRequest
            {
                ExerciseName = searchString,
                PageNo = pageNumber,
                PageLength = pageLength
            });
            return Json(new { 
                items = response.Exercises != null ? response.Exercises.Select(e => new { Reference = e.Reference, Name = e.Name }) : null, 
                total = response.TotalMatches 
            });
        }

        public IActionResult PagedExerciseRecords(int pageNumber, int pageLength, Guid reference)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageLength < 1 || pageLength > _ExerciseSearchConfig.MaxPageLength)
            {
                pageLength = _ExerciseSearchConfig.DefaultPageLength;
            }
            ExerciseRecordsResponse response = _ExerciseService.GetPagedExerciseRecords(new ExerciseRecordsRequest
            {
                ExerciseReference = reference,
                UserReference = User.GetNameIdentifier(),
                PageNo = pageNumber,
                PageLength = pageLength
            });
            return Json(new
            {
                items = response.Records,
                total = response.TotalMatches
            });
        }

        #endregion

    }
}
