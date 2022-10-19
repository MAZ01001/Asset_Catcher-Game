using System.Collections.Generic;
using UnityEngine;

public class PrimitiveSpawner:MonoBehaviour{
    [System.Serializable]
    private struct Primitive{
        [SerializeField][Tooltip("The prefab of the primitive to spawn")]                                   public GameObject primitivePrefab;
        [SerializeField][Range(0f,1f)][Tooltip("The raritie (float from 0.00 to 1.00) for this primitive")] public float raritie;
        [SerializeField][Min(0)][Tooltip("The amount of points when collected")]                            public int points;
    }
    private class PrimitiveSpawn{
        /// <summary> The number of spawned primitives in this list </summary>
        public int Count;
        /// <summary> The game objects in-game of the spawned primitives </summary>
        public List<GameObject> gameObjects;
        /// <summary> The <paramref name="Primitive"/> of the <paramref name="gameObjects"/> in-game </summary>
        public List<Primitive> primitives;
        /// <summary> Creates a PrimitiveSpawn object </summary>
        public PrimitiveSpawn(){
            this.gameObjects=new List<GameObject>();
            this.primitives=new List<Primitive>();
            this.Count=0;
        }
        /// <summary> Get the <paramref name="Primitive"/> from the spawned game object </summary>
        /// <param name="gameObject"> The spawned game object in-game </param>
        /// <returns> The <paramref name="Primitive"/> coresponding to the <paramref name="gameObject"/> in-game and null otherwise </returns>
        public Primitive? GetPrimitiveByGameObject(GameObject gameObject){
            int index=this.gameObjects.IndexOf(gameObject);
            if(index==-1)return null;
            return this.primitives[index];
        }
        /// <summary> Add new gmaeobject and <paramref name="Primitive"/> to the end of this list </summary>
        /// <param name="gameObject"> The game objects in-game of the spawned primitives </param>
        /// <param name="primitive"> The <paramref name="Primitive"/> of the <paramref name="gameObjects"/> in-game </param>
        public void Add(GameObject gameObject,Primitive primitive){
            this.gameObjects.Add(gameObject);
            this.primitives.Add(primitive);
            this.Count++;
        }
        /// <summary>
        ///     Remove existing game object and <paramref name="Primitive"/> from this list
        ///     <br/><i>The remaining items in the list are renumbered to replace the removed item</i>
        /// </summary>
        /// <param name="gameObject"> The game objects in-game of the spawned primitive </param>
        /// <return> True when successfully deleted and false otherwise </return>
        public bool Remove(GameObject gameObject){
            int index=this.gameObjects.IndexOf(gameObject);
            if(index==-1)return false;
            this.gameObjects.RemoveAt(index);
            this.primitives.RemoveAt(index);
            this.Count--;
            return true;
        }
        /// <summary>
        ///     Remove existing game object and <paramref name="Primitive"/> from this list
        ///     <br/><i>The remaining items in the list are renumbered to replace the removed item</i>
        /// </summary>
        /// <param name="index"> The index in this list of the spawned primitive </param>
        /// <return> True when successfully deleted and false otherwise </return>
        public bool Remove(int index){
            if(
                index<0
                ||index>=this.Count
            )return false;
            this.gameObjects.RemoveAt(index);
            this.primitives.RemoveAt(index);
            this.Count--;
            return true;
        }
    }
    //~ inspector (private)
    [Header("Spawner")]
    [SerializeField][Tooltip("The primitives to spawn")]                                            private List<Primitive> primitives;
    [Header("Random spawn Area")]
    [SerializeField][Tooltip("The spawn origin as offset from this object for random spawning")]    private Vector3 spawnOffset=new Vector3(0f,7.5f,0f);
    [SerializeField][Tooltip("The size in either direction from spawn origin for random spawning")] private Vector3 spawnAreaSize=new Vector3(7f,0f,0f);
    [Header("Destroy Explosion")]
    [SerializeField][Tooltip("The explosion game object to spawn (must destroy itself !)")]         private GameObject explosionParticle;
    //~ private
    private PrimitiveSpawn spawnedPrimitives;
    //~ private methods
    private void Start(){this.spawnedPrimitives=new PrimitiveSpawn();}
    [ContextMenu("Spawn random primitive (in-game)")] //~ does work in editor, but those primitives are not added to the list and will not despawn !!
    private void SpawnPrimitive(){
        //~ get a random percentage as decimal between 0 to 1 (inclusive)
        float rarity=Random.value;
        //~ get a random primitive based on the percentage in spawnRateRarities (rarest possible)
        int primIndex=-1;
        float minRarity=float.PositiveInfinity;
        for(int i=0;i<this.primitives.Count;i++){
            if(
                this.primitives[i].raritie>=rarity
                &&this.primitives[i].raritie<minRarity
            ){
                primIndex=i;
                minRarity=this.primitives[i].raritie;
            }
        }
        if(primIndex==-1)return;
        //~ create a new instance of that primitive at a random rotation and location within the calculated spawn area
        this.spawnedPrimitives.Add(
            Object.Instantiate<GameObject>(
                this.primitives[primIndex].primitivePrefab,
                this.transform.position
                +this.spawnOffset
                +new Vector3(
                    (this.spawnAreaSize.x==0f?0f:Random.Range(-this.spawnAreaSize.x,this.spawnAreaSize.x)),
                    (this.spawnAreaSize.y==0f?0f:Random.Range(-this.spawnAreaSize.y,this.spawnAreaSize.y)),
                    (this.spawnAreaSize.z==0f?0f:Random.Range(-this.spawnAreaSize.z,this.spawnAreaSize.z))
                ),
                Random.rotationUniform,
                this.transform
            ),
            this.primitives[primIndex]
        );
    }
    private void OnTriggerExit(Collider collider){
        //~ despawn if it is a primitve that was spawned
        if(this.spawnedPrimitives.Remove(collider.gameObject))Object.Destroy(collider.gameObject);
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
    ///     Gets the amount of points set for that primitive or 0 if it is not a known primitive
    ///     <br/>Also spawns an explosion and destroys the primitive
    /// </summary>
    /// <returns> The amount of points set for that primitive or 0 if it is not a known primitive </returns>
    public int CollectAndGetPoints(GameObject primitive){
        int index=this.spawnedPrimitives.gameObjects.IndexOf(primitive);
        if(index==-1)return 0;
        //~ spawn explosion
        GameObject explosionObj=Object.Instantiate<GameObject>(
            this.explosionParticle,
            primitive.transform.position,
            Quaternion.identity,
            this.transform
        );
        //~ enable the rotation for the explosion particle when rarity is below 50%
        if(this.spawnedPrimitives.primitives[index].raritie<0.5f){
            ParticleSystem.RotationOverLifetimeModule explosionRotation=explosionObj.GetComponent<ParticleSystem>().rotationOverLifetime;
            explosionRotation.enabled=true;
        }
        //~ set mesh and material of primitive for particle explosion
        ParticleSystemRenderer explosionRenderer=explosionObj.GetComponent<ParticleSystemRenderer>();
        explosionRenderer.enabled=true;
        Renderer primitiveRenderer=primitive.GetComponent<Renderer>();
        MeshFilter primitiveMeshFilter=primitive.GetComponent<MeshFilter>();
        if(primitiveRenderer!=null)explosionRenderer.material=primitiveRenderer.sharedMaterial;
        if(primitiveMeshFilter!=null)explosionRenderer.mesh=primitiveMeshFilter.sharedMesh;
        //~ get points
        int points=this.spawnedPrimitives.primitives[index].points;
        //~ despawn primitive
        Object.Destroy(primitive);
        this.spawnedPrimitives.Remove(index);
        //~ return points
        return points;
    }
    /// <summary> Starts the spawner after given delay and spawn rate </summary>
    /// <param name="spawnRate"> The spawn rate per second </param>
    /// <param name="delay"> A one time delay in seconds to start the spawning </param>
    public void StartSpawner(float spawnRate,float delay=0f){InvokeRepeating("SpawnPrimitive",delay,spawnRate);}
    /// <summary> Stops the spawning of new objects </summary>
    [ContextMenu("Stop spawner (in-game)")] //~ does not work in the editor since there is no spawner instance
    public void StopSpawner(){CancelInvoke("SpawnPrimitive");}
    /// <summary>
    ///     Change the spawn rate
    ///     <br/>Automatically stops and restarts the spawning with the new <paramref name="spawnRate"/>
    /// </summary>
    /// <param name="spawnRate"> The new spawn rate in seconds </param>
    public void ChangeRate(float spawnRate){
        this.StopSpawner();
        this.StartSpawner(spawnRate);
    }
    /// <summary> Destroys all currently existing primitives </summary>
    [ContextMenu("Destroy all spawned (in-game)")] //~ does not work in the editor because there is no instance of this list to delete from
    public void DestroyAllSpawned(){
        List<GameObject> despawnList=new List<GameObject>(this.spawnedPrimitives.gameObjects);
        foreach(GameObject obj in despawnList){
            if(this.spawnedPrimitives.Remove(obj))Object.Destroy(obj);
        }
    }
}
