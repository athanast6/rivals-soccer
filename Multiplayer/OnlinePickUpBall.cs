using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class OnlinePickUpBall : NetworkBehaviour
{
    private NetworkObject BallEmpty;

    async void Awake(){
        await Task.Delay(100);
        BallEmpty = transform.GetComponentInChildren<NetworkObject>();
        await Task.CompletedTask;
    }
    // Change parent method
    [ServerRpc]
    public void ChangeParentServerRpc(bool pickingUpBall)
    {
        if(pickingUpBall){
            transform.SetParent(BallEmpty.transform);
        }else{
            transform.SetParent(null);
        }
        // Invoke the change on clients
        ChangeParentClientRpc(pickingUpBall);
    }

    [ClientRpc]
    public void ChangeParentClientRpc(bool pickingUpBall)
    {
        if(pickingUpBall){
            transform.SetParent(BallEmpty.transform);
        }else{
            transform.SetParent(null);
        }
    }
}
