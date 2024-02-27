using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace SlabManBuff{
    [HarmonyPatch(typeof(Revolver), "Start")]
    class Revolver_Start{
        public static void Postfix(Revolver __instance){
            if(__instance.gunVariation != 1 || !__instance.altVersion) return;

            Canvas canvas = __instance.gameObject.GetComponentInChildren<Canvas>();
            if(!canvas) return;

            GameObject panel = canvas.transform.GetChild(0).gameObject;
            if(!panel) return;

            //Destroy unnecessary coin panels
            GameObject[] coinPanelArray = new GameObject[2];
            for(int i = 0; i < panel.transform.childCount; i++){
                GameObject coinPanel = panel.transform.GetChild(i).gameObject;
                if(i%2 != 0) Object.Destroy(coinPanel);
                else coinPanelArray[i/2] = coinPanel;
            }
            
            //Initialize image array
            Image[] imageArray = new Image[coinPanelArray.Length];
            Sprite sprite = AssetManager.LoadAsset<Sprite>("assets/textures/coincharges.png");

            const float MARGIN = 2f;
            const float GAP = 1f;

            for(int i = 0; i < coinPanelArray.Length; i++){
                GameObject coinPanel = coinPanelArray[i];
                
                //Set transform
                RectTransform rectTransform = coinPanel.GetComponent<RectTransform>();
                rectTransform.pivot = new Vector2(0.5f, 0.5f);

                rectTransform.anchorMin = new Vector2(0.0f, 0.5f*(1-i));
                rectTransform.anchorMax = new Vector2(1.0f, 0.5f*(2-i));

                if(i == 0){
                    rectTransform.offsetMin = new Vector2(MARGIN, GAP);
                    rectTransform.offsetMax = new Vector2(-MARGIN, -MARGIN);
                }else if(i == 1){
                    rectTransform.offsetMin = new Vector2(MARGIN, MARGIN);
                    rectTransform.offsetMax = new Vector2(-MARGIN, -GAP);
                }

                //Set Images
                Image image = coinPanel.GetComponent<Image>();
                image.sprite = sprite;
                image.fillMethod = Image.FillMethod.Horizontal;
                image.fillOrigin = 1;
                imageArray[i] = image;
            }

            //Set coin panels
            __instance.coinPanels = imageArray;
        }
    }

    [HarmonyPatch(typeof(Revolver), "ThrowCoin")]
    class Revolver_ThrowCoinPatch{
        public static bool Prefix(Revolver __instance, ref Punch ___punch, 
            GameObject ___camObj, ref bool ___pierceReady){
            
            //Return if not slabman
            if(__instance.gunVariation != 1 || !__instance.altVersion) return true;

            //Get Punch Object
            if (___punch == null || !___punch.gameObject.activeInHierarchy) 
                ___punch = MonoSingleton<FistControl>.Instance.currentPunch;
            
            //Punch Flip Animation
            if (___punch) ___punch.CoinFlip();
            
            //Create Coin
            GameObject coinObject = Object.Instantiate<GameObject>(__instance.coin, ___camObj.transform.position + 
                ___camObj.transform.up * -0.5f, ___camObj.transform.rotation);

            //Add aditional Component
            coinObject.AddComponent<CoinAddAttr>();

            //Set the source weapon to this
            coinObject.GetComponent<Coin>().sourceWeapon = __instance.gc.currentWeapon;
            
            //Vibrate controller and something else ;)
            MonoSingleton<RumbleManager>.Instance.SetVibration(RumbleProperties.CoinToss);
            
            //Add Current Movement speed to coin
            NewMovement nm = MonoSingleton<NewMovement>.Instance;
            Vector3 currentMovement = nm.ridingRocket ? nm.ridingRocket.rb.velocity : nm.rb.velocity;
            Vector3 coinMovement = ___camObj.transform.forward * 20f + Vector3.up * 15f + currentMovement /*+ Vector3.zero*/;
            coinObject.GetComponent<Rigidbody>().AddForce(coinMovement, ForceMode.VelocityChange);
            
            //Set other properties
            __instance.pierceCharge = 0f;
            ___pierceReady = false;

            //Don't execute original method
            return false;
        }
    }

    [HarmonyPatch(typeof(Revolver), "Update")]
    class Revolver_UpdatePatch{
        public static bool Prefix(Revolver __instance, ref bool ___shootReady, ref bool ___pierceReady, bool ___gunReady, 
        InputManager ___inman, float ___coinCharge, CameraController ___cc, WeaponIdentifier ___wid, WeaponCharges ___wc,
        AudioSource ___ceaud, Light ___celight, MaterialPropertyBlock ___screenProps){

            if(__instance.gunVariation != 1 || !__instance.altVersion) return true;

            Traverse traverse = Traverse.Create(__instance);

            if (!___shootReady) {
                if (__instance.shootCharge + 200f * Time.deltaTime < 100f) {
                    __instance.shootCharge += 200f * Time.deltaTime;
                } else {
                    __instance.shootCharge = 100f;
                    ___shootReady = true;
                }
            }

            if (!___pierceReady) {
                if (__instance.pierceCharge + 480f * Time.deltaTime < 100f) {
                    __instance.pierceCharge += 480f * Time.deltaTime;
                }else {
                    __instance.pierceCharge = 100f;
                    ___pierceReady = true;
                }
            }

            float coinChargeCost = Property.MarksmanChargeCost;
            if (__instance.gc.activated) {
                if (___inman.InputSource.Fire2.WasPerformedThisFrame && ___pierceReady && ___coinCharge >= coinChargeCost) {
                    ___cc.StopShake();
                    if (!___wid || ___wid.delay == 0f) {
                        ___wc.rev1charge -= coinChargeCost;
                        traverse.Method("ThrowCoin").GetValue();
                    }else {
                        __instance.Invoke("ThrowCoin", ___wid.delay);
                        ___pierceReady = false;
                        __instance.pierceCharge = 0f;
                    }
                } else if (___gunReady && !___inman.PerformingCheatMenuCombo() && 
                ___inman.InputSource.Fire1.IsPressed && ___shootReady) {
                    if (!___wid || ___wid.delay == 0f) {
                        traverse.Method("Shoot", 1).GetValue();
                    } else {
                        ___shootReady = false;
                        __instance.shootCharge = 0f;
                        __instance.Invoke("DelayedShoot", ___wid.delay);
                    }
                    if (___ceaud && ___ceaud.volume != 0f) {
                        ___ceaud.volume = 0f;
                    }
                }
            }

            if (___celight) {
                if (__instance.pierceShotCharge == 0f && ___celight.enabled) {
                    ___celight.enabled = false;
                } else if (__instance.pierceShotCharge != 0f) {
                    ___celight.enabled = true;
                    ___celight.range = __instance.pierceShotCharge * 0.01f;
                }
            }

            if (__instance.gunVariation != 0) {
                traverse.Method("CheckCoinCharges").GetValue();
            } else if (__instance.pierceCharge == 100f && MonoSingleton<ColorBlindSettings>.Instance) {
                ___screenProps.SetColor("_Color", MonoSingleton<ColorBlindSettings>.Instance.variationColors[__instance.gunVariation]);
            }

            return false;
        }
    }    

    [HarmonyPatch(typeof(Revolver), "OnEnable")]
    class Revolver_OnEnable{
        public static void Postfix(Revolver __instance, WeaponCharges ___wc){
            if(__instance.gunVariation != 1 || !__instance.altVersion) return;
            
            WeaponChargesAddAttr attr = ___wc.gameObject.GetComponent<WeaponChargesAddAttr>();
            if(!attr){
                ___wc.gameObject.AddComponent<WeaponChargesAddAttr>();
                attr = ___wc.gameObject.GetComponent<WeaponChargesAddAttr>();
            }
            attr.rev1alt = __instance.altVersion;
        }
    }

    [HarmonyPatch(typeof(Revolver), "CheckCoinCharges")]
    class Revolver_CheckCoinCharges{
        public static bool Prefix(Revolver __instance, ref float ___coinCharge, ref AudioSource ___screenAud, 
        WeaponCharges ___wc, WeaponIdentifier ___wid){
            
            if(__instance.gunVariation != 1 || !__instance.altVersion) return true;

            if (__instance.coinPanelsCharged == null || __instance.coinPanelsCharged.Length == 0) {
                __instance.coinPanelsCharged = new bool[__instance.coinPanels.Length];
            }

            ___coinCharge = ___wc.rev1charge;
            for (int i = 0; i < __instance.coinPanels.Length; i++) {
                float coinCost = Property.MarksmanChargeCost;
                __instance.coinPanels[i].fillAmount = ___coinCharge / coinCost - (float)i;
                
                if (__instance.coinPanels[i].fillAmount < 1f) {
                    __instance.coinPanels[i].color = Color.red;
                    __instance.coinPanelsCharged[i] = false;
                } else {
                    ColorBlindSettings cbs = MonoSingleton<ColorBlindSettings>.Instance;
                    if (cbs && __instance.coinPanels[i].color != cbs.variationColors[__instance.gunVariation]) {
                        __instance.coinPanels[i].color = cbs.variationColors[__instance.gunVariation];
                    }
                    if (!__instance.coinPanelsCharged[i] && (!___wid || ___wid.delay == 0f)) {
                        if (!___screenAud) 
                            ___screenAud = __instance.GetComponentInChildren<Canvas>().GetComponent<AudioSource>();
                        ___screenAud.pitch = 1f + (float)i / 2f;
                        ___screenAud.Play();
                        __instance.coinPanelsCharged[i] = true;
                    }
                }
            }

            return false;
        }
    }
}