using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Utils
{
    public interface IWeightConversionUtility
    {
        decimal ConvertKgToLb(decimal kg);
        decimal ConvertLbToKg(decimal lb);
        decimal RoundToZeroPointFive(decimal val);
    }

    public class WeightConversionUtility : IWeightConversionUtility
    {
        private const decimal Kg2Lb_Rate = 2.2046226218m;
        private const decimal Lb2Kg_Rate = 0.45359237m;

        public decimal ConvertKgToLb(decimal kg) => 
            kg * Kg2Lb_Rate;
        public decimal ConvertLbToKg(decimal lb) => 
            lb * Lb2Kg_Rate;

        public decimal RoundToZeroPointFive(decimal val) => 
            Math.Round(val * 2, MidpointRounding.AwayFromZero) / 2;
    }

    public static class WeightConversionHelper
    {
        private static IWeightConversionUtility WeightConversionUtility() =>
            new WeightConversionUtility();

        public static decimal ConvertKgToLb(decimal kg) =>
            WeightConversionUtility().ConvertKgToLb(kg);
        public static decimal ConvertLbToKg(decimal lb) =>
            WeightConversionUtility().ConvertLbToKg(lb);

        public static decimal RoundToZeroPointFive(decimal val) =>
            WeightConversionUtility().RoundToZeroPointFive(val);


    }

}
