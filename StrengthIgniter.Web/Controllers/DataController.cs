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
    public class DataController : Controller
    {

        /*
         * 
         * New Import Page (Data/Import
         * Import page (Data/Import/{Reference}
         * - Delete Import
         * - Delete Row
         * - Edit Row
         * - Process Row
         * Import Row Page (Data/Import/Row/{reference}
         * - Delete / Edit / Process
         * 
         */

        private readonly IRecordImportService _RecordImportService;
        private readonly IRecordImportSchemaService _RecordImportSchemaService;

        public DataController(
            IRecordImportService recordImportService,
            IRecordImportSchemaService recordImportSchemaService
        )
        {
            _RecordImportService = recordImportService;
            _RecordImportSchemaService = recordImportSchemaService;
        }

        public IActionResult Index()
        {
            IEnumerable<RecordImportModel> imports = _RecordImportService.GetUserImports(User.GetNameIdentifier());
            return View(imports);
        }

        public IActionResult Import()
        {
            return View("NewImport", new NewImportViewModel
            {
                Schemas = _RecordImportSchemaService.GetAllSchemas()
            });
        }

        [HttpPost]
        public IActionResult Import(NewImportViewModel vm)
        {
            if(ModelState.IsValid)
            {
                if (vm.CsvFile == null || vm.CsvFile.Length == 0)
                {
                    return Content("file not selected");//TODO: ????
                }
                var stream = vm.CsvFile.OpenReadStream();
                string name = string.IsNullOrWhiteSpace(vm.Name) ? vm.CsvFile.FileName : vm.Name;

                Guid importReference = _RecordImportService.Import(new NewImportRequest
                {
                    Name = name,
                    CsvFile = stream,
                    SchemaReference = vm.SchemaReference,
                    UserReference = User.GetNameIdentifier()
                });
                return RedirectToAction("Import", new { reference = importReference });
            }
            vm.Schemas = _RecordImportSchemaService.GetAllSchemas();//re-get this
            return View("NewImport", vm);
        }

        [Route("data/import/{reference}", Name = "Import")]
        public IActionResult Import(Guid reference)
        {
            RecordImportModel import = _RecordImportService.GetByReference(reference, User.GetNameIdentifier());
            return View(import);
        }

        //public IActionResult Delete(Guid reference)
        //{
        //    //TODO: delete import
        //    return Json(true);
        //}
        //public IActionResult DeleteRow(Guid reference)
        //{
        //    //TODO: delete import row
        //    return Json(true);
        //}

    }
}
