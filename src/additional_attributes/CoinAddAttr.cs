using UnityEngine;

namespace SlabManBuff{
    public class CoinAddAttr:MonoBehaviour{
        public int hitAmmount = Property.CoinTotalHitAmmount;
        public float bounceForce = Property.CoinHitBounceForce;
        public bool noDeadCoin = false;
        public GameObject coinClone = null;

        public void CreateClone(Coin sourceCoin){
            coinClone = Object.Instantiate(sourceCoin.gameObject, sourceCoin.transform.position, 
                Quaternion.identity);
            coinClone.name = "NewCoin+" + (sourceCoin.power - 2f);
            coinClone.SetActive(false);

            //Set coin fields
            Coin coinCloneScript = coinClone.GetComponent<Coin>();
            if(coinCloneScript){
                coinCloneScript.shot = false;
                coinCloneScript.ricochets = 0;
                coinCloneScript.power += 1;
            }

            CoinAddAttr coinCloneAttr = coinClone.GetComponent<CoinAddAttr>();
            if(coinCloneAttr){
                coinCloneAttr.noDeadCoin = true;
                coinCloneAttr.hitAmmount -= 1;
            }
        }

        public void DelayedBounce(){
            Invoke("Bounce", 0.0f);
        }

        private void Bounce(){
            if(!coinClone) return;
            
            //Activate coin clone
            coinClone.SetActive(true);

            //Bounce coin
            Rigidbody coinCloneRigidbody = coinClone.GetComponent<Rigidbody>();
            if(coinCloneRigidbody){
                coinCloneRigidbody.isKinematic = false;
                coinCloneRigidbody.velocity = Vector3.zero;
                coinCloneRigidbody.AddForce(Vector3.up * bounceForce, ForceMode.VelocityChange);
            }

            //Set coinclone to false
            coinClone = null;
        }
    }
}