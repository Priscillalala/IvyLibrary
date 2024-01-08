using UnityEngine;
using R2API;
using RoR2;

namespace IvyLibrary
{
    public record class DifficultyWrapper(DifficultyDef DifficultyDef, DifficultyIndex DifficultyIndex) 
        : DifficultyWrapper<DifficultyWrapper, DifficultyDef>(DifficultyDef, DifficultyIndex) 
    { }

    public record class DifficultyWrapper<TDifficultyDef>(TDifficultyDef DifficultyDef, DifficultyIndex DifficultyIndex) 
        : DifficultyWrapper<DifficultyWrapper<TDifficultyDef>, TDifficultyDef>(DifficultyDef, DifficultyIndex) where TDifficultyDef : DifficultyDef
    { }

    public abstract record class DifficultyWrapper<TDifficultyWrapper, TDifficultyDef>(TDifficultyDef DifficultyDef, DifficultyIndex DifficultyIndex)
        where TDifficultyWrapper : DifficultyWrapper<TDifficultyWrapper, TDifficultyDef>
        where TDifficultyDef : DifficultyDef
    {
        public TDifficultyWrapper SetScalingValue(float scalingValue)
        {
            DifficultyDef.scalingValue = scalingValue;
            return this as TDifficultyWrapper;
        }

        public TDifficultyWrapper SetIconSprite(Sprite iconSprite)
        {
            DifficultyDef.iconSprite = iconSprite;
            DifficultyDef.foundIconSprite = true;
            return this as TDifficultyWrapper;
        }

        public TDifficultyWrapper SetColor(Color color)
        {
            DifficultyDef.color = color;
            return this as TDifficultyWrapper;
        }

        public TDifficultyWrapper SetServerTag(string serverTag)
        {
            DifficultyDef.serverTag = serverTag;
            return this as TDifficultyWrapper;
        }

        public TDifficultyWrapper SetFlags(DifficultyFlags flags)
        {
            DifficultyDef.countsAsHardMode = (flags & DifficultyFlags.HardMode) > 0;
            if ((flags & DifficultyFlags.Hidden) > 0)
            {
                DifficultyAPI.hiddenDifficulties.Add(DifficultyDef);
            } 
            else
            {
                DifficultyAPI.hiddenDifficulties.Remove(DifficultyDef);
            }
            return this as TDifficultyWrapper;
        }

        public string NameToken => DifficultyDef.nameToken;

        public string DescriptionToken => DifficultyDef.descriptionToken;
    }
}