using CodeFluff.Extensions.IEnumerable;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Models
{
    /// <summary>
    /// Configures how CsvHelper maps items in the csv file to the RecordImportStage
    /// </summary>
    public class RecordImportMap : ClassMap<RecordImportRowModel>
    {
        public RecordImportMap(RecordImportSchemaModel schema)
        {
            if (schema.ColumnMap.HasItems())
            {
                foreach (var cMap in schema.ColumnMap)
                {
                    switch (cMap.ColumnTypeCode)
                    {
                        case ColumnTypeCode.Exercise:
                            Map(m => m.ExerciseText)
                                .Name(cMap.HeaderName)
                                .Optional();
                            break;
                        case ColumnTypeCode.Date:
                            Map(m => m.DateText)
                                .Name(cMap.HeaderName)
                                .Optional();
                            break;
                        case ColumnTypeCode.WeightKg:
                            Map(m => m.WeightKgText)
                                .Name(cMap.HeaderName)
                                .Optional();
                            break;
                        case ColumnTypeCode.WeightLb:
                            Map(m => m.WeightLbText)
                                .Name(cMap.HeaderName)
                                .Optional();
                            break;
                        case ColumnTypeCode.Sets:
                            Map(m => m.SetText)
                                .Name(cMap.HeaderName)
                                .Optional();
                            break;
                        case ColumnTypeCode.Reps:
                            Map(m => m.RepText)
                                .Name(cMap.HeaderName)
                                .Optional();
                            break;
                        case ColumnTypeCode.BodyweightKg:
                            Map(m => m.BodyweightKgText)
                                .Name(cMap.HeaderName)
                                .Optional();
                            break;
                        case ColumnTypeCode.BodyweightLb:
                            Map(m => m.BodyweightLbText)
                                .Name(cMap.HeaderName)
                                .Optional();
                            break;
                        case ColumnTypeCode.Rpe:
                            Map(m => m.RpeText)
                                .Name(cMap.HeaderName)
                                .Optional();
                            break;
                        case ColumnTypeCode.Notes:
                            Map(m => m.Notes)
                                .Name(cMap.HeaderName)
                                .Optional();
                            break;
                    }
                }
            }
        }
    }
}
