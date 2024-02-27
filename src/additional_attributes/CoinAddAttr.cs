using UnityEngine;

namespace SlabManBuff{
    public class CoinAddAttr:MonoBehaviour{
        public int hitAmmount = Property.CoinTotalHitAmmount;
        public float bounceForce = Property.CoinHitBounceForce;
        public bool noDeadCoin = false;
        public GameObject coinPrefab = null;
        public void CreateAndSetCoinPrefab(Coin sourceCoin, bool noDeadCoin, int hitAmmount){
            //Create Clone
            coinPrefab = Object.Instantiate<GameObject>(sourceCoin.gameObject, 
                    sourceCoin.transform.position, Quaternion.identity);

            //Set Initial parameters
            Coin coinPrefabScript = coinPrefab.GetComponent<Coin>();
            if(coinPrefabScript) {
                if(coinPrefabScript.ricochets > 0){
                    coinPrefabScript.ricochets = 0;
                    coinPrefabScript.power += 1;
                }
            }

            CoinAddAttr coinPrefabAttr = coinPrefab.GetComponent<CoinAddAttr>();
            if(coinPrefabAttr){
                coinPrefabAttr.noDeadCoin = noDeadCoin;
                coinPrefabAttr.hitAmmount = hitAmmount;
            }

            coinPrefab.SetActive(false);
        }

        public void DelayedCloneAndBounce(){
            Invoke("CloneAndBounce", 0f);
        }

        public void CloneAndBounce(){
            BounceCoin(coinPrefab.GetComponent<Coin>());
        }

        private void BounceCoin(Coin prefabCoinScript){
            //Create coin clone
            GameObject coinClone = Object.Instantiate(prefabCoinScript.gameObject, prefabCoinScript.transform.position, 
                Quaternion.identity);
            coinClone.SetActive(true);
            //Set coin clone attributes
            coinClone.name = "NewCoin+" + (prefabCoinScript.power - 2f);

            Coin coinCloneScript = coinClone.GetComponent<Coin>();
            if(coinCloneScript.shot) coinCloneScript.shot = false;

            Rigidbody coinCloneRigidbody = coinClone.GetComponent<Rigidbody>();
            if(coinCloneRigidbody){
                coinCloneRigidbody.isKinematic = false;
                coinCloneRigidbody.velocity = Vector3.zero;
                coinCloneRigidbody.AddForce(Vector3.up * bounceForce, ForceMode.VelocityChange);
            }

            //Remove coin
            prefabCoinScript.shot = true;
            prefabCoinScript.GetComponent<SphereCollider>().enabled = false;
            prefabCoinScript.gameObject.SetActive(false);
            new GameObject().AddComponent<CoinCollector>().coin = prefabCoinScript.gameObject;
            prefabCoinScript.CancelInvoke("GetDeleted");
        }
    }
}