using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Utils
{

    public interface IRpeUtility
    {
        /// <summary>
        /// Estimate one rep max using reps, weight and RPE
        /// </summary>
        /// <returns>e1RM</returns>
        decimal CalculateRpeMax(int reps, decimal weight, decimal rpe);

        /// <summary>
        /// Creates a basic RPE chart that gives the best percentage of 1RM to use given the reps and RPE
        /// </summary>
        /// <returns>RPE chart of percentages</returns>
        RpeChart GetBasicRpeChart();

        /// <summary>
        /// Generate a custom RPE chart based on a theoretical or actual lift using reps, weight and rpe. 
        /// Chart will containe weight to use for any rep/RPE set
        /// </summary>
        /// <returns>A custom RPE chart containing appropriate weights to use</returns>
        RpeChart GenerateCustomRpeChart(int reps, decimal weight, decimal rpe);
    }

    public class RpeUtility : IRpeUtility
    {
        #region RPE Chart
        private const string RpeChartCsv = @"x,1,2,3,4,5,6,7,8,9,10,11,12
10,100,95.5,92.2,89.2,86.3,83.7,81.1,78.6,76.2,73.9,70.7,68
9.5,97.8,93.9,90.7,87.8,85,82.4,79.9,77.4,75.1,72.3,69.4,66.7
9,95.5,92.2,89.2,86.3,83.7,81.1,78.6,76.2,73.9,70.7,68,65.3
8.5,93.9,90.7,87.8,85,82.4,79.9,77.4,75.1,72.3,69.4,66.7,64
8,92.2,89.2,86.3,83.7,81.1,78.6,76.2,73.9,70.7,68,65.3,62.6
7.5,90.7,87.8,85,82.4,79.9,77.4,75.1,72.3,69.4,66.7,64,61.3
7,89.2,86.3,83.7,81.1,78.6,76.2,73.9,70.7,68,65.3,62.6,59.9
6.5,87.8,85,82.4,79.9,77.4,75.1,72.3,69.4,66.7,64,61.3,58.6
6,86.3,83.7,81.1,78.6,76.2,73.9,70.7,68,65.3,62.6,59.9,57.2";
        #endregion

        #region CTOR
        private readonly Dictionary<int, IDictionary<decimal, decimal>> _Matrix;
        public RpeUtility()
        {
            _Matrix = new Dictionary<int, IDictionary<decimal, decimal>>();
            string[] rows = RpeChartCsv.Split('\n');

            //Create RPE Matrix using string

            string[] reps = rows[0].Split(',');
            for (int i = 1; i < reps.Length; i++)
                _Matrix[int.Parse(reps[i])] = new Dictionary<decimal, decimal>();

            for (int i = 1; i < rows.Length; i++)
            {
                string[] cells = rows[i].Split(',');
                decimal rpe = decimal.Parse(cells[0]);

                for (int j = 1; j < cells.Length; j++)
                    _Matrix[j][rpe] = decimal.Parse(cells[j]);
            }
        }
        #endregion

        public decimal CalculateRpeMax(int reps, decimal weight, decimal rpe)
        {
            RpeCalculationException.ThrowIfInvalid(reps, rpe);
            return (weight / _Matrix[reps][rpe]) * 100;
        }

        public RpeChart GetBasicRpeChart()
        {
            return new RpeChart(_Matrix);
        }

        
        public RpeChart GenerateCustomRpeChart(int reps, decimal weight, decimal rpe)
        {
            decimal max = CalculateRpeMax(reps, weight, rpe);

            IDictionary<int, IDictionary<decimal, decimal>> newMatrix = new Dictionary<int, IDictionary<decimal, decimal>>();

            foreach (var x in _Matrix.Keys)
            {
                newMatrix[x] = new Dictionary<decimal, decimal>();
                foreach (var y in _Matrix[x].Keys)
                {
                    newMatrix[x][y] = (_Matrix[x][y] / 100) * max;
                }
            }

            return new RpeChart(newMatrix);
        }
    }

    public static class RpeHelper
    {
        private static IRpeUtility RpeUtility() => new RpeUtility();

        public static decimal CalculateRpeMax(int reps, decimal weight, decimal rpe) =>
            RpeUtility().CalculateRpeMax(reps, weight, rpe);

        public static RpeChart GetBasicRpeChartPercentages() =>
            RpeUtility().GetBasicRpeChart();

        public static RpeChart GenerateCustomRpeChart(int reps, decimal weight, decimal rpe) =>
            RpeUtility().GenerateCustomRpeChart(reps, weight, rpe);
    } 

    /// <summary>
    /// Represents a RPE chart with custom weight or percentages 
    /// </summary>
    public class RpeChart
    {
        #region CTOR
        public RpeChart(IDictionary<int, IDictionary<decimal, decimal>> matrix)
        {
            Matrix = matrix;
        }
        #endregion

        public IDictionary<int, IDictionary<decimal, decimal>> Matrix { get; private set; }

        /// <summary>
        /// Get the weight or percentage for a lift of x reps at y RPE
        /// </summary>
        /// <returns>Weight / Percentage</returns>
        public decimal this[int reps, decimal rpe]
        {
            get
            {
                RpeCalculationException.ThrowIfInvalid(reps, rpe);
                return Matrix[reps][rpe];
            }
        }

        /// <summary>
        /// Get the maximum value of the chart (1 rep at RPE 10 / 100%)
        /// </summary>
        public decimal RpeMax
        {
            get
            {
                return this[1, 10m];
            }
        }
    }

    public class RpeCalculationException : Exception
    {
        public RpeCalculationException(string message) : base(message) { }

        public static void ThrowIfInvalid(int reps, decimal rpe)
        {
            if (reps < 1 || reps > 12)
                throw new RpeCalculationException("Reps must be between 1 and 12");

            if (rpe < 6 || rpe > 10 || rpe % 0.5m != 0)
                throw new RpeCalculationException("RPE must be between 6 and 10 at intervals of 0.5 ");
        }
    }
}
