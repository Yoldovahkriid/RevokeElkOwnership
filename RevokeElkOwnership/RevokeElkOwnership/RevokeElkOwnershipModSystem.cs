using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace RevokeElkOwnership
{
    public class RevokeElkOwnershipModSystem : ModSystem
    {
        private Harmony Harmony;
        public override void StartServerSide(ICoreServerAPI api)
        {
            Harmony = new Harmony(Mod.Info.ModID);
            if (!Harmony.HasAnyPatches(Mod.Info.ModID))
            {
                Harmony.PatchAll();
            }
        }

        public void Dispose()
        {
            if (Harmony != null)
            {
                Harmony.UnpatchAll(Mod.Info.ModID);
            }
        }

    }

    [HarmonyPatch(typeof(EntityBehaviorOwnable), nameof(EntityBehaviorOwnable.IsOwner))]
    public class IsOwnerPatch
    {
        public static void Postfix(EntityBehaviorOwnable __instance, ref bool __result, EntityAgent byEntity)
        {
            if (byEntity is EntityPlayer byPlayer)
            {
                var tree = __instance.entity.WatchedAttributes.GetTreeAttribute("ownedby");
                if (tree == null) return;
                string uid = tree.GetString("uid");

                // only owner can remove medallion if present
                if (uid != null && byPlayer.PlayerUID == uid || byPlayer.Player.WorldData.CurrentGameMode == EnumGameMode.Creative)
                {
                    __result = true;
                    return;
                }
            }
        }
    }
}
