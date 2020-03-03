using System;
using System.Collections.Generic;

namespace HeatFuzzy.Logic
{
    abstract public class AbstractFuzzyLogic : BaseNotifyPropertyChanged, ILogic
    {
        public virtual object[] InputValues => throw new NotImplementedException();

        public virtual object OutputValue => throw new NotImplementedException();

        public virtual void CalculateOutput(double deltaTimeInSeconds)
        {
            throw new NotImplementedException();
        }

        public virtual bool SetInputValues(IList<object> inputValues)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Maps a linearly falling (or rising) curve between a minimum and a maximum.
        /// The values outside this area are 0.0 or 1.0;
        /// </summary>
        /// <param name="minValue">The left minimum value of the area.</param>
        /// <param name="maxValue">The right maximum value of the area.</param>
        /// <param name="value">The value to compare.</param>
        /// <param name="rising">If set to true the curve is rising, otherwise the curve is falling</param>
        /// <returns>Returns a value between 0.0 and 1.0.</returns>
        protected double GetTransitionAreaResult(double minValue, double maxValue, double value, bool rising)
        {
            if (!AreValuesDifferent(minValue, maxValue))
            {
                return 1.0;
            }
            double result;

            if (value < minValue)
            {
                result = 1.0;
            }
            else if (value < maxValue)
            {
                double diffValue = maxValue - minValue;
                result = (value - minValue) / diffValue;
            }
            else
            {
                result = 0.0;
            }

            if (rising)
            {
                result = 1.0 - result;
            }

            return result;
        }

        protected double GetIsoscelesTriangleResult(double minValue, double maxValue, double value)
        {
            if (!AreValuesDifferent(minValue, maxValue))
            {
                return 1.0;
            }

            double result = 0.0;
            double diffValue = (maxValue - minValue) / 2.0;
            double peakValue = minValue + diffValue;

            if (value >= minValue && value <= maxValue)
            {
                if (value <= peakValue)
                {
                    result = (value - minValue) / diffValue;
                }
                else
                {
                    result = (maxValue - value) / diffValue;
                }
            }

            return result;
        }
    }
}
