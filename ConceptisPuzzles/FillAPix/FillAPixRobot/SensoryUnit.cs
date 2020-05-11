using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;
using System;

namespace FillAPixRobot
{
    public class SensoryUnit : ISensoryUnit, IComparable
    {
        public SensoryUnit(ISensoryUnit sensoryUnit)
        {
            Id = sensoryUnit.Id;
            Type = sensoryUnit.Type;
            Value = sensoryUnit.Value;
        }

        public SensoryUnit(SensoryTypes senseType, string value, bool saveable = true)
        {
            Id = -1;
            Type = senseType;
            Value = value;
        }

        public long Id { get; protected set; }
        public SensoryTypes Type { get; protected set; }
        public string Value { get; protected set; }

        public override bool Equals(object obj)
        {
            var sensoryUnit = obj as SensoryUnit;
            if (sensoryUnit == null)
            {
                return false;
            }

            if (Id >= 0 && Id.Equals(sensoryUnit.Id))
            {
                return true;
            }

            return sensoryUnit.Type == Type && sensoryUnit.Value == Value;
        }

        public override int GetHashCode()
        {
            if (Id >= 0)
            {
                return Id.GetHashCode();
            }
            // ToDo: Check integer overflow ... how should this be handled?
            return Type.GetHashCode() + Value.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            var sensoryUnit = obj as SensoryUnit;
            if (sensoryUnit == null)
            {
                return 1;
            }
            return Type.Equals(sensoryUnit.Type) ? Value.CompareTo(sensoryUnit.Value) : Type.CompareTo(sensoryUnit.Type);
        }

        public override string ToString()
        {
            return $"{{{Type}='{Value}'}}";
        }
    }
}
