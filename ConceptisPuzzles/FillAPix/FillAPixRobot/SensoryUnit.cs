using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;
using FillAPixRobot.Persistence;
using System;
using System.Collections.Generic;

namespace FillAPixRobot
{
    // Gibt die kleinste Einheit einer Wahrnehmung an: z.B. Nicht markiert, Wert 3, Feldposition 0, ...
    public class SensoryUnit : SQLiteSensoryUnit, IComparable
    {
        static private List<ISensoryUnit> _sensoryUnits = null;

        static public List<ISensoryUnit> SensoryUnits
        {
            get
            {
                if (_sensoryUnits == null)
                {
                    _sensoryUnits = new List<ISensoryUnit>();
                    foreach (ISensoryUnit sqLiteSensoryUnit in LoadAll())
                    {
                        _sensoryUnits.Add(new SensoryUnit(sqLiteSensoryUnit));
                    }
                    _sensoryUnits.Sort();
                }
                return _sensoryUnits;
            }
        }

        public SensoryUnit(ISensoryUnit sensoryUnit)
        {
            Id = sensoryUnit.Id;
            Type = sensoryUnit.Type;
            Value = sensoryUnit.Value;
        }

        public SensoryUnit(SensoryTypes senseType, string value, bool saveable = true)
            : base(senseType, value, saveable)
        {
            if (!SensoryUnits.Contains(this))
            {
                SensoryUnits.Add(this);
            }
        }

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
