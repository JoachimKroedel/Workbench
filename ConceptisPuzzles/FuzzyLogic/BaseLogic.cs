using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace FuzzyLogic
{
    public abstract class BaseLogic : IFuzzyLogic
    {
        protected readonly Dictionary<Enum, IList<Point>> _fuzzyCurvePoints = new Dictionary<Enum, IList<Point>>();

        public FuzzyObject<Enum> If(Enum value)
        {
            double degree = GetDegree(value);
            return new FuzzyObject<Enum>(value, degree, this);
        }

        public void CalculateOutput()
        {
            Fuzzification();
            Implication();
            Defuzzification();
        }

        public virtual double GetDegree(Enum enumType)
        {
            return 0.0;
        }

        public double GetAndDegree(Enum enumTypeA, Enum enumTypeB)
        {
            return GetAndDegree(GetDegree(enumTypeA), GetDegree(enumTypeB));
        }

        public double GetAndDegree(double degreeA, double degreeB)
        {
            return Math.Min(degreeA, degreeB);
        }

        protected virtual void Fuzzification()
        {
            throw new NotImplementedException();
        }

        protected virtual void Implication()
        {
            throw new NotImplementedException();
        }

        protected virtual void Defuzzification()
        {
            throw new NotImplementedException();
        }

        protected virtual double GetValueByFuzzyDegree(IList<Point> curvePoints, double degree)
        {
            if (degree <= 0.0)
            {
                return double.NaN;
            }
            double result = 0.0;
            int hitCount = 0;
            Point leftPoint = new Point(double.MinValue, double.NaN);
            foreach (Point rightPoint in curvePoints.OrderBy(p => p.X))
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

        protected virtual FuzzyObject<FT> Create<FT>(FT fuzzyValue, double realValue) where FT : Enum
        {
            // ToDo: Make this part more generic by calling a generic GetFuzzyDegreeByValue
            double fuzzyDegree = 0.0;
            return new FuzzyObject<FT>(fuzzyValue, fuzzyDegree, this);
        }

        protected virtual double GetFuzzyDegreeByValue(IList<Point> curvePoints, double value)
        {
            Point leftPoint = new Point(double.MinValue, double.NaN);
            foreach (Point rightPoint in curvePoints.OrderBy(p => p.X))
            {
                if (value <= rightPoint.X)
                {
                    // if value is on the left side of the curve point we have to return a value
                    if (double.IsNaN(leftPoint.Y))
                    {
                        // in that case there was no left point defined (value is left outside the curve definition)
                        return rightPoint.Y;
                    }

                    double range = rightPoint.X - leftPoint.X;
                    if (range == 0.0)
                    {
                        // in that case the left and right points are on the same x-Axis value. To protect for Zero-Devision return the average of both points 
                        return (leftPoint.Y + rightPoint.Y) / 2.0;
                    }
                    // in that case calculate the linear percentage value between left and right point
                    double percentage = (value - leftPoint.X) / range;
                    return (rightPoint.Y - leftPoint.Y) * percentage + leftPoint.Y;
                }
                leftPoint = rightPoint;
            }
            // In the last case value is right outside the curve definition.
            return leftPoint.Y;
        }
    }
}
