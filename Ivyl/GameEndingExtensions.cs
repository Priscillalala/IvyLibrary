using BepInEx;
using System;
using RoR2;
using System.Security.Permissions;
using System.Security;
using UnityEngine.ResourceManagement;
using UnityEngine;
using RoR2.ContentManagement;
using HG;
using UnityEngine.AddressableAssets;
using EntityStates;

namespace Ivyl
{
    public static class GameEndingExtensions
    {
        public static TGameEndingDef SetIconSprite<TGameEndingDef>(this TGameEndingDef gameEndingDef, Sprite iconSprite) where TGameEndingDef : GameEndingDef
        {
            gameEndingDef.icon = iconSprite;
            return gameEndingDef;
        }

        public static TGameEndingDef SetMaterial<TGameEndingDef>(this TGameEndingDef gameEndingDef, Material material) where TGameEndingDef : GameEndingDef
        {
            gameEndingDef.material = material;
            return gameEndingDef;
        }

        public static TGameEndingDef SetColors<TGameEndingDef>(this TGameEndingDef gameEndingDef, Color foregroundColor, Color backgroundColor) where TGameEndingDef : GameEndingDef
        {
            gameEndingDef.foregroundColor = foregroundColor;
            gameEndingDef.backgroundColor = backgroundColor;
            return gameEndingDef;
        }

        public static TGameEndingDef SetDefaultKillerBodyPrefab<TGameEndingDef>(this TGameEndingDef gameEndingDef, GameObject defaultKillerBodyPrefab) where TGameEndingDef : GameEndingDef
        {
            gameEndingDef.defaultKillerOverride = defaultKillerBodyPrefab;
            return gameEndingDef;
        }

        public static TGameEndingDef SetFlags<TGameEndingDef>(this TGameEndingDef gameEndingDef, GameEndingFlags flags) where TGameEndingDef : GameEndingDef
        {
            gameEndingDef.isWin = (flags & GameEndingFlags.Victory) > GameEndingFlags.None;
            gameEndingDef.showCredits = (flags & GameEndingFlags.ShowCredits) > GameEndingFlags.None;
            return gameEndingDef;
        }

        public static TGameEndingDef SetGameOverControllerState<TGameEndingDef>(this TGameEndingDef gameEndingDef, SerializableEntityStateType gameOverControllerState) where TGameEndingDef : GameEndingDef
        {
            gameEndingDef.gameOverControllerState = gameOverControllerState;
            return gameEndingDef;
        }

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

        public static TGameEndingDef SetLunarCoinReward<TGameEndingDef>(this TGameEndingDef gameEndingDef, uint lunarCoinReward) where TGameEndingDef : GameEndingDef
        {
            gameEndingDef.lunarCoinReward = lunarCoinReward;
            return gameEndingDef;
        }
    }
}