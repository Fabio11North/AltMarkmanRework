using HarmonyLib;
using UnityEngine;

namespace SlabManBuff{
    [HarmonyPatch(typeof(WeaponCharges), "Charge")]
    class WeaponCharges_Charge{
        public static void Postfix(WeaponCharges __instance, float amount){
            WeaponChargesAddAttr attr = __instance.gameObject.GetComponent<WeaponChargesAddAttr>();
            if(!attr) return;

            float secondPerCharges = Property.MarksmanChargesSeconds;
            float chargesPerSecond = Property.MarksmanChargeCost/secondPerCharges;

            if (__instance.rev1charge < 400f && attr.rev1alt) {
                //Add coin charge
                __instance.rev1charge = Mathf.MoveTowards(__instance.rev1charge, 400f, (chargesPerSecond-25f) * amount);
            }
        }
    }
}