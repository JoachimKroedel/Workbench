using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;
using System;

namespace FillAPixRobot
{
    public class SensoryUnit : ISensoryUnit, IComparable
    {

        static public ISensoryUnit Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            string parseText = text.Trim();
            if (!parseText.StartsWith("{") || !parseText.EndsWith("}"))
            {
                return null;
            }
            parseText = parseText.Substring(1, parseText.Length - 2);

            string[] splits = parseText.Split(new[] { '=' });
            if (splits.Length != 2)
            {
                return null;
            }

            SensoryTypes type = (SensoryTypes)Enum.Parse(typeof(SensoryTypes), splits[0].Trim(), true);
            string value = splits[1].Trim();
            if (value.Length >= 2)
            {
                value = value.Substring(1, value.Length - 2);
            }
            else
            {
                value = string.Empty;
            }

            return new SensoryUnit(type, value);
        }

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

        static public bool operator ==(SensoryUnit lhs, SensoryUnit rhs)
        {
            if (ReferenceEquals(lhs, null))
            {
                if (ReferenceEquals(rhs, null))
                {
                    return true;
                }
                return false;
            }
            return lhs.Equals(rhs);
        }

        static public bool operator !=(SensoryUnit lhs, SensoryUnit rhs)
        {
            return !(lhs == rhs);
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
