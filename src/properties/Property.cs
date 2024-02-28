using Configgy;
using UnityEngine;

namespace SlabManBuff{
    class Property{
        [Configgable("", "Alt-Marksman Revolver Coin Charges", 0, 
        "Determines how many charges the alt-marksman has (It doesn't change the revolver display).")]
        private static int marksmanCharges = 2;
        
        [Configgable("", "Alt-Marksman Revolver Coin Reharge Time", 0, 
        "Determines how many seconds it takes for 1 charge to recharge.")]
        private static float marksmanChargesSeconds = 6.5f;
        
        
        [Configgable("", "Coin Bounce Times", 0, 
        "Determines how many times you can shoot the coin before breaking.")]
        private static int coinTotalHitAmmount = 2;
        
        [Configgable("", "Coin Bounce Force", 0, 
        "Determines the force with which the coin bounces after shooting it.")]
        private static float coinHitBounceForce = 25f;

        [Configgable("", "Coin size scale", 0, 
        "Determines the size of the coin, relative to the original size.")]
        private static float coinScale = 1.25f;

        public static int CoinTotalHitAmmount{
            get{return coinTotalHitAmmount;}
        }

        public static float CoinHitBounceForce{
            get{return coinHitBounceForce;}
        }

        public static int MarksmanCharges{
            get{return marksmanCharges;}
        }

        public static float MarksmanChargeCost{
            get{return 400/marksmanCharges;}
        }

        public static float MarksmanChargesSeconds{
            get{return marksmanChargesSeconds;}
        }

        public static Vector3 CoinScale{
            get{return coinScale*new Vector3(0.1f, 0.1f, 0.01f);}
        }
    }
}