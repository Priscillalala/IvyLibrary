using System;
using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using RoR2.ExpansionManagement;
using System.Collections.Generic;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace Ivyl
{
    public static class Overlays
    {
        private struct Overlay
        {
            public Material material;
            public Func<CharacterModel, bool> condition;
        }

        private static List<Overlay> overlays;
        private static bool _init;

        [SystemInitializer]
        private static void Init()
        {
            if (overlays != null && overlays.Count > 0)
            {
                IL.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;
            }
            _init = true;
        }

        private static void CharacterModel_UpdateOverlays(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            MethodReference methodReference = null;
            bool ilfound = c.TryGotoNext(MoveType.After,
                x => x.MatchLdsfld(typeof(DLC1Content.Buffs).GetField(nameof(DLC1Content.Buffs.VoidSurvivorCorruptMode))),
                x => x.MatchCallOrCallvirt<CharacterBody>(nameof(CharacterBody.HasBuff)),
                x => x.MatchCallOrCallvirt(out methodReference)
                );
            if (ilfound)
            {
                foreach (Overlay overlay in overlays)
                {
                    c.Emit(OpCodes.Ldarg, 0);
                    c.EmitDelegate<Func<Material>>(() => overlay.material);
                    c.Emit(OpCodes.Ldarg, 0);
                    c.EmitDelegate(overlay.condition);
                    c.Emit(OpCodes.Call, methodReference);
                }
            }
            else Debug.LogError($"{nameof(Overlays)}: {nameof(CharacterModel_UpdateOverlays)} IL hook failed!");
        }

        public static void RegisterConditionalOverlay(Material material, Func<CharacterModel, bool> condition)
        {
            if (_init)
            {
                throw new InvalidOperationException();
            }
            (overlays ??= new List<Overlay>()).Add(new Overlay 
            { 
                material = material, 
                condition = condition 
            });
        }
    }
}