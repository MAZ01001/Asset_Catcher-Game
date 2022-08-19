using UnityEngine;

public class DespawnDetection:MonoBehaviour{
    // TODO leave cube trigger = despawn
    [SerializeField][Tooltip("When exiting this objects trigger/boundary, will delete this () object")]private GameObject exitTriggerObject;
    private void OnCollisionExit(Collision collision){
        if(collision.gameObject==this.exitTriggerObject){
            Destroy(this.gameObject);
        }
    }
}
