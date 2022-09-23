using System.Collections.Generic;
using UnityEngine;

public class PrimitiveSpawner:MonoBehaviour{
    //~ inspector (private)
    [Header("Spawner")]
    [SerializeField][Tooltip("The primitives to spawn (in order of rarity ascending)")]private GameObject[] primitives;
    [SerializeField][Tooltip("The raritie (float from 0.00 to 1.00) for each element in the `primitives` array")]private float[] spawnRateRarities;
    [SerializeField][Tooltip("The value (integer) when collected for each element in the `primitives` array")]private int[] spawnRaritiePoints;
    [Header("Random spawn Area")]
    [SerializeField][Tooltip("The spawn origin as offset from this object for random spawning")]private Vector3 spawnOffset=new Vector3(0f,7.5f,0f);
    [SerializeField][Tooltip("The size in either direction from spawn origin for random spawning")]private Vector3 spawnAreaSize=new Vector3(7f,0f,0f);
    [Header("Destroy Explosion")]
    [SerializeField][Tooltip("The explosion game object to spawn (must destroy itself !)")]private GameObject explosionParticle;
    //~ private
    private HashSet<GameObject> spawnedPrimitives;
    //~ private methods
    private void Start(){
        this.spawnedPrimitives=new HashSet<GameObject>();
        //~ exceptions if arrays under the spawner header are not of equal lengths
        if(this.spawnRateRarities.Length!=this.primitives.Length)throw new System.OverflowException($"[{this.gameObject.name} : PrimitiveSpawner] spawnRateRarities array is not the same length as primitives array");
        if(this.spawnRaritiePoints.Length!=this.primitives.Length)throw new System.OverflowException($"[{this.gameObject.name} : PrimitiveSpawner] spawnRaritiesPoints array is not the same length as primitives array");
    }
    [ContextMenu(itemName:"SpawnPrimitive")]
    private void SpawnPrimitive(){
        //~ get a random percentage as decimal between 0 to 1 (inclusive)
        float rarity=Random.value;
        //~ get a random primitive based on the percentage in spawnRateRarities (rarest possible)
        int minRarityIndex=-1;
        float minRarity=float.PositiveInfinity;
        for(int i=0;i<this.spawnRateRarities.Length;i++){
            if(
                this.spawnRateRarities[i]>=rarity
                &&this.spawnRateRarities[i]<minRarity
            ){
                minRarityIndex=i;
                minRarity=this.spawnRateRarities[i];
            }
        }
        if(minRarityIndex==-1)return;
        GameObject prim=this.primitives[minRarityIndex];
        //~ create a new instance of that primitive at a random rotation and location within the calculated spawn area
        this.spawnedPrimitives.Add(Object.Instantiate<GameObject>(
            prim,
            this.transform.position
            +this.spawnOffset
            +new Vector3(
                (this.spawnAreaSize.x==0f?0f:Random.Range(-this.spawnAreaSize.x,this.spawnAreaSize.x)),
                (this.spawnAreaSize.y==0f?0f:Random.Range(-this.spawnAreaSize.y,this.spawnAreaSize.y)),
                (this.spawnAreaSize.z==0f?0f:Random.Range(-this.spawnAreaSize.z,this.spawnAreaSize.z))
            ),
            Random.rotationUniform,
            this.transform
        ));
    }
    private void OnTriggerExit(Collider collider){
        //~ despawn if it is a primitve in the primitives array
        if(this.spawnedPrimitives.Remove(collider.gameObject)){
            // this.spawnedPrimitives.Remove(collider.gameObject);
            Object.Destroy(collider.gameObject);
        }
    }
    private void OnDrawGizmosSelected(){
        Gizmos.color=Color.blue;
        Gizmos.DrawLine(
            this.transform.position,
            this.transform.position
            +this.spawnOffset
        );
        Gizmos.color=Color.green;
        Gizmos.DrawWireCube(
            this.transform.position
            +this.spawnOffset,
            this.spawnAreaSize * 2f
        );
    }
    //~ public methods
    /// <summary>
    ///     gets the amount of points set for that primitive or 0 if it is not a known primitive
    ///     <br/>also spawns an explosion and destroys the primitive
    /// </summary>
    /// <returns> the amount of points set for that primitive or 0 if it is not a known primitive </returns>
    public int CollectAndGetPoints(GameObject primitive){
        if(!this.spawnedPrimitives.Contains(primitive))return 0;
        //~ get index of primitives
        int index=-1;
        for(int i=0;i<this.primitives.Length;i++){
            GameObject obj=this.primitives[i];
            if(primitive.name==$"{obj.name}(Clone)"){
                index=i;
                break;
            }
        }
        if(index==-1)return 0;
        //~ spawn explosion
        GameObject explosionObj=Object.Instantiate<GameObject>(
            this.explosionParticle,
            primitive.transform.position,
            Quaternion.identity,
            this.transform
        );
        //~ set mesh and material of primitive for particle explosion
        ParticleSystemRenderer explosionRenderer=explosionObj.GetComponent<ParticleSystemRenderer>();
        explosionRenderer.enabled=true;
        Renderer primitiveRenderer=primitive.GetComponent<Renderer>();
        MeshFilter primitiveMeshFilter=primitive.GetComponent<MeshFilter>();
        if(primitiveRenderer!=null)explosionRenderer.material=primitiveRenderer.sharedMaterial;
        if(primitiveMeshFilter!=null)explosionRenderer.mesh=primitiveMeshFilter.sharedMesh;
        // TODO rotation animation for rarer primitives ?
        //~ despawn primitive
        this.spawnedPrimitives.Remove(primitive);
        Object.Destroy(primitive);
        //~ return points
        return this.spawnRaritiePoints[index];
    }
    /// <summary> starts the spawner after given delay and spawn rate </summary>
    /// <param name="spawnRate"> the spawn rate per second </param>
    /// <param name="delay"> a one time delay in seconds to start the spawning </param>
    public void StartSpawner(float spawnRate,float delay=0f){InvokeRepeating("SpawnPrimitive",delay,spawnRate);}
    /// <summary> stops the spawning of new objects </summary>
    public void StopSpawner(){CancelInvoke("SpawnPrimitive");}
    /// <summary> change the spawn rate </summary>
    /// <param name="spawnRate"> the new spawn rate in seconds </param>
    public void ChangeRate(float spawnRate){
        this.StopSpawner();
        this.StartSpawner(spawnRate);
    }
    /// <summary> destroys all currently existing primitives </summary>
    [ContextMenu(itemName:"DestroyAllSpawned")]
    public void DestroyAllSpawned(){
        foreach(GameObject obj in this.spawnedPrimitives){
            this.spawnedPrimitives.Remove(obj);
            Object.Destroy(obj);
        }
    }
}
