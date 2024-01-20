using RoR2;
using System.Runtime.CompilerServices;

namespace IvyLibrary
{
    /// <summary>
    /// Static extensions for manipulating an <see cref="UnlockableDef"/> at runtime. 
    /// </summary>
    /// <remarks>
    /// Allows method chaining syntax.
    /// </remarks>
    public static class UnlockableExtensions
    {
        /// <summary>
        /// Set the boolean values of this unlockable with <see cref="UnlockableFlags"/>.
        /// </summary>
        /// <returns><paramref name="unlockableDef"/>, to continue a method chain.</returns>
        public static TUnlockableDef SetFlags<TUnlockableDef>(this TUnlockableDef unlockableDef, UnlockableFlags flags) where TUnlockableDef : UnlockableDef
        {
            unlockableDef.hidden = (flags & UnlockableFlags.Hidden) > UnlockableFlags.None;
            return unlockableDef;
        }

        /// <summary>
        /// Set the name token of this unlockable.
        /// </summary>
        /// <returns><paramref name="unlockableDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TUnlockableDef SetNameToken<TUnlockableDef>(this TUnlockableDef unlockableDef, string nameToken) where TUnlockableDef : UnlockableDef
        {
            unlockableDef.nameToken = nameToken;
            return unlockableDef;
        }
    }
}