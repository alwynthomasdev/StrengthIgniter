using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Models
{
    public class RecordImportSchemaModel
    {
        public int RecordImportSchemaId { get; internal set; }
        public Guid Reference { get; internal set; }
        //public int UserId { get; set; }
        public string Name { get; set; }
        public string Delimiter { get; set; }

        public IEnumerable<RecordImportSchemaColumnMapModel> ColumnMap { get; set; }
        public IEnumerable<RecordImportSchemaExerciseMapModel> ExerciseMap { get; set; }
    }

    public class RecordImportSchemaColumnMapModel
    {
        public int RecordImportSchemaColumnMapId { get; internal set; }
        public int RecordImportSchemaId { get; set; }
        public string HeaderName { get; set; }
        public ColumnTypeCode ColumnTypeCode { get; set; }
    }

    public class RecordImportSchemaExerciseMapModel
    {
        public int RecordImportSchemaExerciseMapId { get; internal set; }
        public int RecordImportSchemaId { get; set; }
        public int ExerciseId { get; set; }
        public string Text { get; set; }
    }

    public enum ColumnTypeCode
    {
        Exercise = 1,
        Date = 2,
        WeightKg = 3,
        WeightLb = 4,
        BodyweightKg = 5,
        BodyweightLb = 6,
        Sets = 7,
        Reps = 8,
        Rpe = 9,
        Notes = 10
    }
}
