using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StrengthIgniter.Web.Models
{
    public class RecordViewModel
    {
        public Guid Reference { get; set; }

        [Required]
        public Guid ExerciseReference { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Date { get; set; }
        [Display(Name = "Sets")]
        public int? Sets { get; set; }
        [Required]
        [Display(Name = "Reps")]
        [Range(1,500, ErrorMessage = "Atleast 1 rep")]
        public int? Reps { get; set; }
        [Display(Name = "Weight")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.0}")]
        public decimal? Weight { get; set; }
        [Display(Name = "Bodyweight")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.0}")]
        public decimal? Bodyweight { get; set; }
        [Display(Name = "RPE")]
        [Range(6,10, ErrorMessage = "RPE must be between 6 and 10")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.0}")]
        public decimal? RPE { get; set; }
        [Display(Name = "Notes")]
        public string Notes { get; set; }
    }
}
