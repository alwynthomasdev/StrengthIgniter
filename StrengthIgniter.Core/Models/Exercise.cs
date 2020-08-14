using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Models
{
    public class ExerciseModel
    {
        public int ExerciseId { get; internal set; }
        public Guid Reference { get; internal set; }
        public string Name { get; set; }
    }
}
