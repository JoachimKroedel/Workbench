using HeatFuzzy.Logic;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HeatFuzzy
{
    public abstract class BaseNotifyPropertyChanged : INotifyPropertyChanged
    {
        private const double DELTA_DOUBLE_DIFF = 0.001;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool AreValuesDifferent(DateTime a, DateTime b)
        {
            return !a.Equals(b);
        }

        protected bool AreValuesDifferent(bool a, bool b)
        {
            return !a.Equals(b);
        }

        protected bool AreValuesDifferent(int a, int b)
        {
            return !a.Equals(b);
        }

        protected bool AreValuesDifferent(double a, double b)
        {
            return System.Math.Abs(a - b) >= DELTA_DOUBLE_DIFF;
        }

        protected bool AreValuesDifferent<FT>(FuzzyObject<FT> a, FuzzyObject<FT> b) where FT : Enum
        {
            if (a == null && b == null)
            {
                return false;
            }
            if (a != null && b!= null)
            {
                return !a.Value.Equals(b.Value) || AreValuesDifferent(a.Degree, b.Degree);
            }
            return true;
        }
    }
}
