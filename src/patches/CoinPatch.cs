using HarmonyLib;
using UnityEngine;

namespace SlabManBuff{
    [HarmonyPatch(typeof(Coin), "ReflectRevolver")]
    class Coin_ReflectRevolverPatch{
        public static void Postfix(Coin __instance, int ___hitTimes, GameObject ___altBeam){
            CoinAddAttr attr = __instance.gameObject.GetComponent<CoinAddAttr>();
            if(!attr) return;

            if(___hitTimes > 0 && ___altBeam == null) return;

            //Bounce coin
            if(attr.coinClone) attr.DelayedBounce();
        }
    }

    [HarmonyPatch(typeof(Coin), "DelayedReflectRevolver")]
    class Coin_DelayedReflectRevolverPatch{
        public static void Prefix(Coin __instance, bool ___checkingSpeed){
            CoinAddAttr attr = __instance.gameObject.GetComponent<CoinAddAttr>();
            if(!attr) return;

            if(!___checkingSpeed) return;

            //Create coin prefab with one less hitAmmount if the hit ammount > 1
            if(attr.hitAmmount > 1 && !__instance.shot && !attr.coinClone){
                attr.CreateClone(__instance);
                attr.hitAmmount = 0;
            }
        }
    }

    [HarmonyPatch(typeof(Coin), "Start")]
    class Coin_StartPatch{
        public static void Postfix(Coin __instance, Collider[] ___cols, ref bool ___checkingSpeed){
            CoinAddAttr attr = __instance.gameObject.GetComponent<CoinAddAttr>();
            if(!attr) return;

            if(attr.noDeadCoin){
                for (int i = 0; i < ___cols.Length; i++) {
                    ___cols[i].enabled = false;
                }
                ___checkingSpeed = true;
                
                //Reset deadcoin
                attr.noDeadCoin = false;
            }

            //Change coin appearance
            __instance.gameObject.transform.localScale = Property.CoinScale;
            
            Texture texture = AssetManager.LoadAsset<Texture>("assets/textures/coin01_2.png");
            if(texture){
                MeshRenderer mesh = __instance.GetComponent<MeshRenderer>();
                mesh.material.SetTexture("_MainTex", texture);
            }
        }
    }

    [HarmonyPatch(typeof(Coin), "TripleTime")]
    class Coin_TripleTimePatch{
        public static void Postfix(Coin __instance, GameObject ___currentCharge){
            CoinAddAttr attr = __instance.gameObject.GetComponent<CoinAddAttr>();
            if(!attr) return;

            if(__instance.shot) return;

            if(___currentCharge){
                SpriteRenderer renderer = ___currentCharge.GetComponentInChildren<SpriteRenderer>();
                renderer.sprite = AssetManager.LoadAsset<Sprite>("assets/textures/muzzleflash.png");
            }
        }
    }
}