using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Utils
{
    public interface IEOneRepMaxUtility
    {
        decimal CalculateOneRepMax(int reps, decimal weight);
    }

    public class EpleyMaxCalculator : IEOneRepMaxUtility
    {
        public decimal CalculateOneRepMax(int reps, decimal weight)
        {
            return Epley((decimal)reps, weight);
        }

        private decimal Epley(decimal r, decimal w)
        {
            if (r == 1) return w;
            var o = w * (1 + (r / 30));
            return Math.Round(o, 1);
        }
    }


    public static class EOneRepMaxHelper
    {
        //epley formula is the default
        private static IEOneRepMaxUtility EOneRepMaxUtility() => new EpleyMaxCalculator();

        public static decimal CalculateOneRepMax(int reps, decimal weight) =>
            EOneRepMaxUtility().CalculateOneRepMax(reps, weight);
    }
}
