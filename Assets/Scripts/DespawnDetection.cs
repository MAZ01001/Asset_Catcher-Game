using UnityEngine;
using UnityEngine.Events;

public class DespawnDetection:MonoBehaviour{
    [HideInInspector]public GameObject despawnColliderObject=null;

    private void OnTriggerExit(Collider collider){
        if(collider.gameObject==this.despawnColliderObject)Destroy(this.gameObject);
    }
}