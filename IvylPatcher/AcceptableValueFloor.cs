using BepInEx.Configuration;
using System;

namespace Ivyl
{
    public class AcceptableValueFloor : AcceptableValueBase
    {
        public AcceptableValueFloor(IComparable minValue, Type valueType) : base(valueType)
        {
            if (minValue == null)
            {
                throw new ArgumentNullException("minValue");
            }
            this.minValue = minValue;
        }

        private readonly IComparable minValue;

        public override object Clamp(object value)
        {
            if (minValue.CompareTo(value) > 0)
            {
                return minValue;
            }
            return value;
        }

        public override bool IsValid(object value)
        {
            return minValue.CompareTo(value) <= 0;
        }

        public override string ToDescriptionString()
        {
            return $"# Minimum acceptable value: {minValue}";
        }
    }
}
