using System;
using RoR2;
using UnityEngine;
using EntityStates;
using System.Runtime.CompilerServices;

namespace IvyLibrary
{
    /// <summary>
    /// Static extensions for manipulating a <see cref="GameEndingDef"/> at runtime. 
    /// </summary>
    /// <remarks>
    /// Allows method chaining syntax.
    /// </remarks>
    public static class GameEndingExtensions
    {
        /// <summary>
        /// Set the icon sprite of this game ending on the report screen.
        /// </summary>
        /// <remarks>
        /// Game ending icons are usually 128px.
        /// </remarks>
        /// <returns><paramref name="gameEndingDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TGameEndingDef SetIconSprite<TGameEndingDef>(this TGameEndingDef gameEndingDef, Sprite iconSprite) where TGameEndingDef : GameEndingDef
        {
            gameEndingDef.icon = iconSprite;
            return gameEndingDef;
        }

        /// <summary>
        /// Set a material used to animate the icon of this game ending on the report screen.
        /// </summary>
        /// <returns><paramref name="gameEndingDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TGameEndingDef SetMaterial<TGameEndingDef>(this TGameEndingDef gameEndingDef, Material material) where TGameEndingDef : GameEndingDef
        {
            gameEndingDef.material = material;
            return gameEndingDef;
        }

        /// <summary>
        /// Set the colors of this game ending on the report screen.
        /// </summary>
        /// <returns><paramref name="gameEndingDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TGameEndingDef SetColors<TGameEndingDef>(this TGameEndingDef gameEndingDef, Color foregroundColor, Color backgroundColor) where TGameEndingDef : GameEndingDef
        {
            gameEndingDef.foregroundColor = foregroundColor;
            gameEndingDef.backgroundColor = backgroundColor;
            return gameEndingDef;
        }

        /// <summary>
        /// Set a body prefab to be displayed as the killer if this game ending is triggered while players are still alive.
        /// </summary>
        /// <returns><paramref name="gameEndingDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TGameEndingDef SetDefaultKillerBodyPrefab<TGameEndingDef>(this TGameEndingDef gameEndingDef, GameObject defaultKillerBodyPrefab) where TGameEndingDef : GameEndingDef
        {
            gameEndingDef.defaultKillerOverride = defaultKillerBodyPrefab;
            return gameEndingDef;
        }

        /// <summary>
        /// Set the boolean values of this game ending with <see cref="GameEndingFlags"/>.
        /// </summary>
        /// <returns><paramref name="gameEndingDef"/>, to continue a method chain.</returns>
        public static TGameEndingDef SetFlags<TGameEndingDef>(this TGameEndingDef gameEndingDef, GameEndingFlags flags) where TGameEndingDef : GameEndingDef
        {
            gameEndingDef.isWin = (flags & GameEndingFlags.Victory) > GameEndingFlags.None;
            gameEndingDef.showCredits = (flags & GameEndingFlags.ShowCredits) > GameEndingFlags.None;
            return gameEndingDef;
        }

        /// <summary>
        /// Set an entity state to handle this game ending.
        /// </summary>
        /// <returns><paramref name="gameEndingDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TGameEndingDef SetGameOverControllerState<TGameEndingDef>(this TGameEndingDef gameEndingDef, SerializableEntityStateType gameOverControllerState) where TGameEndingDef : GameEndingDef
        {
            gameEndingDef.gameOverControllerState = gameOverControllerState;
            return gameEndingDef;
        }

        /// <inheritdoc cref="SetGameOverControllerState{TGameEndingDef}(TGameEndingDef, SerializableEntityStateType)"/>
        /// <remarks>
        /// <paramref name="gameOverControllerStateType"/> should inherit from <see cref="EntityState"/>.
        /// </remarks>
        public static TGameEndingDef SetGameOverControllerState<TGameEndingDef>(this TGameEndingDef gameEndingDef, Type gameOverControllerStateType) where TGameEndingDef : GameEndingDef
        {
            if (gameOverControllerStateType == null)
            {
                throw new ArgumentNullException(nameof(gameOverControllerStateType));
            }
            if (!gameOverControllerStateType.IsSubclassOf(typeof(EntityState)))
            {
                throw new ArgumentException(nameof(gameOverControllerStateType));
            }
            gameEndingDef.gameOverControllerState = new SerializableEntityStateType(gameOverControllerStateType);
            return gameEndingDef;
        }

        /// <summary>
        /// Set an amount of Lunar Coins to be awarded to players for reaching this game ending.
        /// </summary>
        /// <returns><paramref name="gameEndingDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TGameEndingDef SetLunarCoinReward<TGameEndingDef>(this TGameEndingDef gameEndingDef, uint lunarCoinReward) where TGameEndingDef : GameEndingDef
        {
            gameEndingDef.lunarCoinReward = lunarCoinReward;
            return gameEndingDef;
        }
    }
}