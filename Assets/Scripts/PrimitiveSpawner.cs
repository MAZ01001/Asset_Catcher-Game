using UnityEngine;

public class PrimitiveSpawner:MonoBehaviour{
    //~ inspector (private)
    [SerializeField][Tooltip("The primitives to spawn (in order of rarity ascending)")]private GameObject[] primitives;
    // [SerializeField][Tooltip("The spawn rate per seconds")]private float spawnRate;
    [SerializeField][Tooltip("The rarities (0.00 to 1.00 (%) in descanding order) for each element in the `primitives` array")]private float[] spawnRateRarities;
    [SerializeField][Tooltip("The spawn origin as offset from this object for random spawning")]private Vector3 spawnOffset=new Vector3(0f,7.5f,0f);
    [SerializeField][Tooltip("The size in either direction from spawn origin for random spawning")]private Vector3 spawnAreaSize=new Vector3(7f,0f,0f);
    //~ public methods
    public int GetIndexOf(GameObject primitive){
        // TODO
        return System.Array.IndexOf<GameObject>(this.primitives,primitive);
    }
    public void StartSpawner(float spawnRate,float delay=0f){InvokeRepeating("SpawnPrimitive",delay,spawnRate);}
    public void StopSpawner(){CancelInvoke("SpawnPrimitive");}
    public void ChangeRate(float spawnRate){
        this.StopSpawner();
        this.StartSpawner(spawnRate);
    }
    //~ private methods
    private void Start(){
        //~ sort array to escanding order and resize to primitives array length -1
        System.Array.Sort<float>(this.spawnRateRarities,new System.Comparison<float>((f1,f2)=>f2.CompareTo(f1)));
        System.Array.Resize<float>(ref this.spawnRateRarities,this.primitives.Length-1);
    }
    private void SpawnPrimitive(){
        //~ get a random percentage as decimal between 0 to 1 (inclusive)
        float rarity=Random.value;
        //~ get a random primitive based on the percentage in spawnRateRarities
        GameObject prim=null;
        for(int i=this.spawnRateRarities.Length-1;i>=0;i--){
            if(rarity<=this.spawnRateRarities[i]){
                prim=this.primitives[i+1];
                break;
            }
        }
        if(prim==null)prim=this.primitives[0];
        //~ create a new instance of that primitive at a random rotation and location within the calculated spawn area
        Instantiate<GameObject>(
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
        );
    }
}
