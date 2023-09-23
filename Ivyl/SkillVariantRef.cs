using System;
using RoR2;
using RoR2.Skills;

namespace Ivyl
{
	public struct SkillVariantRef
	{
		private delegate ref SkillFamily.Variant _Ref();
		private readonly _Ref _ref;

		public ViewablesCatalog.Node viewableNode
		{
			get => _ref().viewableNode;
			set => _ref().viewableNode = value;
		}

		public SkillDef skillDef
		{
			get => _ref().skillDef;
			set => _ref().skillDef = value;
		}

		[Obsolete("Use 'unlockableDef' instead.")]
		public string unlockableName
		{
			get => _ref().unlockableName;
			set => _ref().unlockableName = value;
		}

		public UnlockableDef unlockableDef
		{
			get => _ref().unlockableDef;
			set => _ref().unlockableDef = value;
		}

		public SkillVariantRef(SkillFamily skillFamily, int variantIndex)
		{
			_ref = () => ref skillFamily.variants[variantIndex];
		}

		public override int GetHashCode() => _ref().GetHashCode();

		public override string ToString() => _ref().ToString();


		public static implicit operator SkillFamily.Variant(SkillVariantRef value)
		{
			return value._ref();
		}
	}
}