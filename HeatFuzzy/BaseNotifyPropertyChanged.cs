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

        protected bool AreValuesDifferent(Enum a, Enum b)
        {
            return a.GetType() != b.GetType() || !a.Equals(b);
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
    }
}
