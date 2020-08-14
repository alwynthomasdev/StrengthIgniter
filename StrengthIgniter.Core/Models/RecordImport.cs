using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Models
{
    public class RecordImportModel
    {
        public int RecordImportId { get; internal set; }
        public Guid Reference { get; internal set; }
        public string Name { get; set; }
        public int UserId { get; internal set; }
        public int RecordImportSchemaId { get; internal set; }
        public DateTime ImportDateTimeUtc { get; internal set; }

        public IEnumerable<RecordImportRowModel> Rows { get; set; }

        //
        public Guid? RecordImportSchemaReference { get; set; } 
        public Guid UserReference { get; set; } 
    }

    public class RecordImportRowModel
    {
        public int RecordImportRowId { get; internal set; }
        public int RecordImportId { get; set; }

        public ImportRowStatusCode StatusCode { get; set; }

        public int ExerciseId { get; set; }
        public string ExerciseText { get; set; }
        public string DateText { get; set; }
        public string WeightKgText { get; set; }
        public string WeightLbText { get; set; }
        public string BodyweightKgText { get; set; }
        public string BodyweightLbText { get; set; }
        public string SetText { get; set; }
        public string RepText { get; set; }
        public string RpeText { get; set; }

        public string Notes { get; set; }

        public IEnumerable<RecordImportRowErrorModel> Errors { get; set; }
    }

    public class RecordImportRowErrorModel
    {
        public int RecordImportRowErrorId { get; internal set; }
        public int RecordImportRowId { get; set; }
        public ErrorCode ErrorCode { get; set; }
    }

    public enum ImportRowStatusCode
    {
        Error = -1,
        Ready = 0,
        Processed = 1
    }

    public enum ErrorCode
    {
        ExerciseRequired = 1,
        ExerciseCannotMap = 2,

        DateRequired = 3,
        DateInvalid = 4,

        WeightKgInvalid = 5,
        WeightLbInvalid = 6,
        WeightDuplicate = 7,

        BodyweightKgInvalid = 8,
        BodyweightLbInvalid = 9,
        BodyweightDuplicate = 7,

        SetInvalid = 11,

        RepRequired = 12,
        RepInvalid = 13,

        RpeInvalid = 14

    }


}
