using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StrengthIgniter.Core.Models;
using StrengthIgniter.Core.Services;
using StrengthIgniter.Web.Extentions;
using StrengthIgniter.Web.Models;

namespace StrengthIgniter.Web.Controllers
{
    public class RecordController : Controller
    {
        #region CTOR

        private readonly IRecordService _RecordService;
        public RecordController(
            IRecordService recordService
        )
        {
            _RecordService = recordService;
        }

        #endregion

        [Route("record/editor/{exerciseReference:guid}")]
        public IActionResult ExerciseRecordEditor(Guid exerciseReference)
        {
            return View("_RecordEditor", new RecordViewModel { ExerciseReference = exerciseReference });
        }

        [Route("record/editor/{reference}/{exerciseReference}")]
        public IActionResult Editor(Guid reference)
        {
            RecordModel record = _RecordService.GetByIdAndUser(reference, User.GetNameIdentifier());
            return View("_RecordEditor", new RecordViewModel
            {
                Reference = record.Reference,
                ExerciseReference = record.ExerciseReference.Value,
                Date = record.Date,
                Sets = record.Sets,
                Reps = record.Reps,
                Weight = record.WeightKg,
                Bodyweight = record.BodyweightKg,
                RPE = record.RPE,
                Notes = record.Notes
            });
        }

        [HttpPost]
        public IActionResult Save(RecordViewModel vm)
        {
            if(ModelState.IsValid)
            {
                _RecordService.SaveRecord(new RecordModel
                {
                    Reference = vm.Reference,
                    ExerciseReference = vm.ExerciseReference,
                    UserReference = User.GetNameIdentifier(),
                    Date = vm.Date.Value,
                    Reps = vm.Reps.Value,
                    Sets = vm.Sets,
                    WeightKg = vm.Weight.Value,
                    BodyweightKg = vm.Bodyweight,
                    RPE = vm.RPE,
                    Notes = vm.Notes
                });
                //TODO: return something more meaningful than true/false
                return Json(true);
            }
            return Json(false);
        }

        [HttpDelete]
        [Route("record/delete/{reference}")]
        public IActionResult Delete(Guid reference)
        {
            _RecordService.DeleteRecord(reference, User.GetNameIdentifier());
            return Json(true);
        }

    }
}
