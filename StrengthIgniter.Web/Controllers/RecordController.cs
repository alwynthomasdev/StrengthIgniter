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
        private readonly IRecordService _RecordService;
        public RecordController(
            IRecordService recordService
        )
        {
            _RecordService = recordService;
        }

        [Route("record/editor/{exerciseReference:guid}")]
        public IActionResult Editor(Guid exerciseReference)
        {
            return View("_RecordEditor", new RecordViewModel { ExerciseReference = exerciseReference });
        }

        [Route("record/editor/{id:int}")]
        public IActionResult Editor(int id)
        {
            RecordModel record = _RecordService.GetByIdAndUser(id, User.GetNameIdentifier());
            return View("_RecordEditor", new RecordViewModel
            {
                RecordId = record.RecordId,
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
            //TODO: return something more meaningful
            if(ModelState.IsValid)
            {
                _RecordService.SaveRecord(new RecordModel
                {
                    RecordId = vm.RecordId,
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
                return Json(true);
            }
            return Json(false);
        }

        [HttpDelete]
        [Route("record/delete")]
        public IActionResult Delete(int Id)
        {
            _RecordService.DeleteRecord(Id, User.GetNameIdentifier());
            return Json(true);
        }

    }
}
