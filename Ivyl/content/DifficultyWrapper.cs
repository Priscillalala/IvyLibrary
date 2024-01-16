using UnityEngine;
using R2API;
using RoR2;
using System.Runtime.CompilerServices;

namespace IvyLibrary
{
    /// <summary>
    /// Associates a <see cref="DifficultyDef"/> with a <see cref="DifficultyIndex"/> and provides methods for manipulating them.
    /// </summary>
    /// <remarks>
    /// Allows method chaining syntax.
    /// </remarks>
    public record class DifficultyWrapper(DifficultyDef DifficultyDef, DifficultyIndex DifficultyIndex) 
        : DifficultyWrapper<DifficultyWrapper, DifficultyDef>(DifficultyDef, DifficultyIndex) 
    { }

    /// <inheritdoc cref="DifficultyWrapper"/>
    public record class DifficultyWrapper<TDifficultyDef>(TDifficultyDef DifficultyDef, DifficultyIndex DifficultyIndex) 
        : DifficultyWrapper<DifficultyWrapper<TDifficultyDef>, TDifficultyDef>(DifficultyDef, DifficultyIndex) where TDifficultyDef : DifficultyDef
    { }

    /// <inheritdoc cref="DifficultyWrapper"/>
    public abstract record class DifficultyWrapper<TDifficultyWrapper, TDifficultyDef>(TDifficultyDef DifficultyDef, DifficultyIndex DifficultyIndex)
        where TDifficultyWrapper : DifficultyWrapper<TDifficultyWrapper, TDifficultyDef>
        where TDifficultyDef : DifficultyDef
    {
        /// <summary>
        /// Set the scaling value of this difficulty.
        /// </summary>
        /// <remarks>
        /// Drizzle, Rainstorm, and Monsoon have scaling values of 1.0, 2.0, and 3.0 respectively.
        /// </remarks>
        /// <returns>this, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TDifficultyWrapper SetScalingValue(float scalingValue)
        {
            DifficultyDef.scalingValue = scalingValue;
            return this as TDifficultyWrapper;
        }

        /// <summary>
        /// Set the icon sprite of this difficulty.
        /// </summary>
        /// <remarks>
        /// Difficulty icons are usually 128px or 256px.
        /// </remarks>
        /// <returns>this, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TDifficultyWrapper SetIconSprite(Sprite iconSprite)
        {
            DifficultyDef.iconSprite = iconSprite;
            DifficultyDef.foundIconSprite = true;
            return this as TDifficultyWrapper;
        }

        /// <summary>
        /// Set the color of this difficulty.
        /// </summary>
        /// <returns>this, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TDifficultyWrapper SetColor(Color color)
        {
            DifficultyDef.color = color;
            return this as TDifficultyWrapper;
        }

        /// <summary>
        /// Set a tag for this difficulty in the server browser.
        /// </summary>
        /// <returns>this, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TDifficultyWrapper SetServerTag(string serverTag)
        {
            DifficultyDef.serverTag = serverTag;
            return this as TDifficultyWrapper;
        }

        /// <summary>
        /// Set the boolean values of this difficulty with <see cref="DifficultyFlags"/>.
        /// </summary>
        /// <returns>this, to continue a method chain.</returns>
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

        /// <summary>
        /// Access the <see cref="DifficultyDef.nameToken"/> of <see cref="DifficultyDef"/>.
        /// </summary>
        public string NameToken => DifficultyDef.nameToken;

        /// <summary>
        /// Access the <see cref="DifficultyDef.descriptionToken"/> of <see cref="DifficultyDef"/>.
        /// </summary>
        public string DescriptionToken => DifficultyDef.descriptionToken;
    }
}