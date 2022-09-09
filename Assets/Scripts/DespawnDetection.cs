using UnityEngine;

public class DespawnDetection:MonoBehaviour{
    [SerializeField][Tooltip("A game object with a collider trigger that, when left, destroys this gameObject")]private GameObject keepAliveTrigger;

    private void OnTriggerExit(Collider collider){
        if(collider.gameObject==this.keepAliveTrigger)Destroy(this.gameObject);
    }
}
