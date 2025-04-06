
using UnityEngine;

public class Shop : MonoBehaviour
{

    [SerializeField] private ShopManager shopManager;


    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Home Player")||other.gameObject.CompareTag("Away Player")){
            shopManager.OpenShopUI();
        }
    }


}