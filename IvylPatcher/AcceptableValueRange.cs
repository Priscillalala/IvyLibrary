using BepInEx.Configuration;
using System;

namespace Ivyl
{
    public class AcceptableValueRange : AcceptableValueBase
    {
        public AcceptableValueRange(IComparable minValue, IComparable maxValue, Type valueType) : base(valueType)
        {
            if (maxValue == null)
            {
                throw new ArgumentNullException("maxValue");
            }
            if (minValue == null)
            {
                throw new ArgumentNullException("minValue");
            }
            if (minValue.CompareTo(maxValue) >= 0)
            {
                throw new ArgumentException("minValue has to be lower than maxValue");
            }
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        private readonly IComparable minValue;
        private readonly IComparable maxValue;

        public override object Clamp(object value)
        {
            if (minValue.CompareTo(value) > 0)
            {
                return minValue;
            }
            if (maxValue.CompareTo(value) < 0)
            {
                return maxValue;
            }
            return value;
        }

        public override bool IsValid(object value)
        {
            return minValue.CompareTo(value) <= 0 && maxValue.CompareTo(value) >= 0;
        }

        public override string ToDescriptionString()
        {
            return $"# Acceptable value range: From {minValue} to {maxValue}";
        }
    }
}
