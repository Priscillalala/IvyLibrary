using RoR2;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace IvyLibrary
{
    /// <summary>
    /// Static extensions for manipulating a <see cref="BuffDef"/> at runtime. 
    /// </summary>
    /// <remarks>
    /// Allows method chaining syntax.
    /// </remarks>
    public static class BuffExtensions
    {
        /// <summary>
        /// Set the icon sprite of this buff and a tint color for that sprite.
        /// </summary>
        /// <remarks>
        /// <para>Buff icons are usually 128px.</para>
        /// <para>Use <see cref="SetIconSprite{TBuffDef}(TBuffDef, Sprite)"/> to set an icon with no color tint.</para>
        /// </remarks>
        /// <returns><paramref name="buffDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TBuffDef SetIconSprite<TBuffDef>(this TBuffDef buffDef, Sprite iconSprite, Color spriteColor) where TBuffDef : BuffDef
        {
            buffDef.iconSprite = iconSprite;
            buffDef.buffColor = spriteColor;
            return buffDef;
        }

        /// <summary>
        /// Set the icon sprite of this buff.
        /// </summary>
        /// <remarks>
        /// <para>Buff icons are usually 128px.</para>
        /// <para>This overload is used for icon sprites that are already colored (e.g., icons requiring multiple colors). Use <see cref="SetIconSprite{TBuffDef}(TBuffDef, Sprite, Color)"/> to set an icon with a color tint.</para>
        /// </remarks>
        /// <returns><paramref name="buffDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TBuffDef SetIconSprite<TBuffDef>(this TBuffDef buffDef, Sprite iconSprite) where TBuffDef : BuffDef
        {
            buffDef.iconSprite = iconSprite;
            buffDef.buffColor = Color.white;
            return buffDef;
        }

        /// <summary>
        /// Set the boolean values of this buff with <see cref="BuffFlags"/>.
        /// </summary>
        /// <returns><paramref name="buffDef"/>, to continue a method chain.</returns>
        public static TBuffDef SetFlags<TBuffDef>(this TBuffDef buffDef, BuffFlags flags) where TBuffDef : BuffDef
        {
            buffDef.canStack = (flags & BuffFlags.Stackable) > BuffFlags.None;
            buffDef.isDebuff = (flags & BuffFlags.Debuff) > BuffFlags.None;
            buffDef.isCooldown = (flags & BuffFlags.Cooldown) > BuffFlags.None;
            buffDef.isHidden = (flags & BuffFlags.Hidden) > BuffFlags.None;
            return buffDef;
        }

        /// <summary>
        /// Set a <see cref="NetworkSoundEventDef"/> to be played when this timed buff is added to a character.
        /// </summary>
        /// <returns><paramref name="buffDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TBuffDef SetStartSfx<TBuffDef>(this TBuffDef buffDef, NetworkSoundEventDef startSfx) where TBuffDef : BuffDef
        {
            buffDef.startSfx = startSfx;
            return buffDef;
        }
    }
}