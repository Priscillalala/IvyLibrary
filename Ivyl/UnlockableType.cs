using System;

namespace IvyLibrary
{
    public struct UnlockableType : IEquatable<UnlockableType>, IEquatable<string>
	{
		public static readonly UnlockableType Items = "Items";
		public static readonly UnlockableType Logs = "Logs";
		public static readonly UnlockableType Characters = "Characters";
		public static readonly UnlockableType Artifacts = "Artifacts";
		public static readonly UnlockableType Skills = "Skills";
		public static readonly UnlockableType Skins = "Skins";
		public static readonly UnlockableType NewtStatue = "NewtStatue";

		private readonly string _string;

		public UnlockableType(string type)
		{
			_string = type;
		}

		public bool Equals(UnlockableType other) => _string == other._string;

		public bool Equals(string other) => _string == other;

		public override bool Equals(object other)
		{
			if (other is UnlockableType)
            {
				return Equals((UnlockableType)other); 
            }
			if (other is string)
            {
				return Equals((string)other);
            }
			return false;
		}

		public override int GetHashCode() => _string.GetHashCode();

		public override string ToString() => _string;

        public static implicit operator UnlockableType(string value)
		{
			return new UnlockableType(value);
		}

		public static explicit operator string(UnlockableType value)
		{
			return value._string;
		}

		public static bool operator ==(UnlockableType a, UnlockableType b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(UnlockableType a, UnlockableType b)
		{
			return !a.Equals(b);
		}
	}
}
