using CodeFluff.Extensions.IEnumerable;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using StrengthIgniter.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StrengthIgniter.Web.Models
{
    public class NewImportViewModel
    {
        public string Name { get; set; }

        [Required]
        [Display(Name = "Schema")]
        public Guid SchemaReference { get; set; }

        [Required]
        [Display(Name = "CSV File")]
        public IFormFile CsvFile { get; set; }


        IEnumerable<RecordImportSchemaModel> _Schemas;
        public IEnumerable<RecordImportSchemaModel> Schemas
        {
            set
            {
                _Schemas = value;
            }
        }
        public IEnumerable<SelectListItem> SchemaOptions
        {
            get
            {
                if (_Schemas.HasItems())
                {
                    return _Schemas
                        .Select(s => new SelectListItem { Value = s.Reference.ToString(), Text = s.Name })
                        .Prepend(new SelectListItem { Value = "", Text = "< Select Schema >" });
                }
                else return new SelectListItem[] { };//empty list
            }
        }
    }
}
