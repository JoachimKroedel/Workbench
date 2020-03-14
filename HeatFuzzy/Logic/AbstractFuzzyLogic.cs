using System;
using System.Collections.Generic;
using System.Windows;

namespace HeatFuzzy.Logic
{
    abstract public class AbstractFuzzyLogic : BaseNotifyPropertyChanged, IFuzzyLogic
    {
        public virtual object[] InputValues => throw new NotImplementedException();

        public virtual object OutputValue => throw new NotImplementedException();

        public virtual void CalculateOutput()
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

        protected double GetTriangleResult(double minValue, double maxValue, double peakValue, double value)
        {
            if (!AreValuesDifferent(minValue, maxValue))
            {
                return 1.0;
            }

            double result = 0.0;

            if (value >= minValue && value <= maxValue)
            {
                if (value <= peakValue && minValue < peakValue)
                {
                    result = (value - minValue) / (peakValue - minValue);
                }
                else if (value > peakValue && maxValue > peakValue)
                {
                    result = (maxValue - value) / (maxValue - peakValue);
                }
                else
                {
                    result = 1.0;
                }
            }

            return result;
        }

        protected double GetFuzzyDegree(IList<Point> curvePoints, double value)
        {
            // ToDo: List should be sorted by x values
            Point leftPoint = new Point(double.MinValue, double.NaN);
            foreach (Point rightPoint in curvePoints)
            {
                if (value <= rightPoint.X)
                {
                    // if diffTemperature is on the left side of the curve point we have to return a value
                    if (double.IsNaN(leftPoint.Y))
                    {
                        // in that case there was no left point defined (diffTemperature is left outside the curve definition)
                        return rightPoint.Y;
                    }

                    double range = rightPoint.X - leftPoint.X;
                    if (range == 0.0)
                    {
                        // in that case the left and right points are on the same x-Axis value. To protect for Zero-Devision return the avarange of both points 
                        return (leftPoint.Y + rightPoint.Y) / 2.0;
                    }
                    // in that case calculate the linear percentage value between left and right point
                    double percentage = (value - leftPoint.X) / range;
                    return (rightPoint.Y - leftPoint.Y) * percentage + leftPoint.Y;
                }
                leftPoint = rightPoint;
            }
            // In the last case diffTemperature is right outside the curve definition.
            return leftPoint.Y;
        }

        protected double GetValueByDegree(IList<Point> curvePoints, double degree)
        {
            if (degree <= 0.0)
            {
                return double.NaN;
            }
            double result = 0.0;
            int hitCount = 0;
            Point leftPoint = new Point(double.MinValue, double.NaN);
            foreach (Point rightPoint in curvePoints)
            {
                if (!double.IsNaN(leftPoint.Y))
                {
                    double rangeValue = rightPoint.X - leftPoint.X;
                    if (leftPoint.Y < rightPoint.Y)
                    {
                        if (degree >= leftPoint.Y && degree <= rightPoint.Y)
                        {
                            double rangeDegree = rightPoint.Y - leftPoint.Y;
                            double percentage = (degree - leftPoint.Y) / rangeDegree;
                            result += rangeValue * percentage + leftPoint.X;
                            hitCount++;
                        }
                    }
                    else if (leftPoint.Y > rightPoint.Y)
                    {
                        if (degree <= leftPoint.Y && degree >= rightPoint.Y)
                        {
                            double rangeDegree = leftPoint.Y - rightPoint.Y;
                            double percentage = (degree - rightPoint.Y) / rangeDegree;
                            result += rightPoint.X - rangeValue * percentage;
                            hitCount++;
                        }
                    }
                }
                leftPoint = rightPoint;
            }

            return (hitCount > 0) ? (result / hitCount) : double.NaN;
        }

    }
}
