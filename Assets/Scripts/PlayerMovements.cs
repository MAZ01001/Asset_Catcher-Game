using UnityEngine;

public class PlayerMovements : MonoBehaviour{
    //~ inspector (private)
    [SerializeField][Tooltip("The ground collision layer")]                           private LayerMask groundLayer;
    [SerializeField][Tooltip("The primitive trigger layer")]                          private LayerMask primitiveTriggerLayer;
    [SerializeField][Min(0f)][Tooltip("The jump height")]                             private float jumpHeight = 6f;
    [SerializeField][Min(0f)][Tooltip("The movement speed")]                          private float moveSpeed = 10f;
    [SerializeField][Min(0f)][Tooltip("The movement smoothing value")]                private float moveSmooth = 0.1f;
    [SerializeField][Min(0f)][Tooltip("The movement multiplier for in-air movement")] private float airMoveMultiplier = 0.7f;
    //~ private
    private GameScript gameScript;
    private InputProvider input;
    private SphereCollider sc;
    private Rigidbody rb;
    private Vector3 moveDir = Vector3.zero;
    private Vector3 smoothVelocity = Vector3.zero;
    private bool onGround = false;
    private bool jumpToggle = false;
    //~ unity methods (private)
    private void Start(){
        //~ get components
        this.input = this.GetComponent<InputProvider>();
        this.sc = this.GetComponent<SphereCollider>();
        this.rb = this.GetComponent<Rigidbody>();
        this.gameScript = Object.FindObjectOfType<GameScript>();
    }
    private void FixedUpdate(){
        //~ move
        if(this.input.move is > 0.05f or < -0.05f){
            this.moveDir = (Vector3.right * this.input.move).normalized * this.moveSpeed;
            if(!this.onGround) this.moveDir *= this.airMoveMultiplier;
            this.rb.velocity = Vector3.SmoothDamp(
                this.rb.velocity,
                new Vector3(
                    (this.rb.velocity.x + this.moveDir.x) * 0.5f,
                    this.rb.velocity.y, //~ preserve jump and gravity
                    0f
                ),
                ref this.smoothVelocity,
                this.moveSmooth
            );
        }
        //~ jump
        if(this.onGround){
            if(
                this.input.jumpPressed
                && !this.jumpToggle
            ){
                this.rb.AddForce(Physics.gravity * (this.jumpHeight * -10f), ForceMode.Force);
                this.jumpToggle = true;
            }else if(
                !this.input.jumpPressed
                && this.jumpToggle
            ) this.jumpToggle = false;
        }
    }
    private void OnTriggerEnter(Collider collider){
        //~ collect primitive
        if(this.CheckIsLayerInMask(this.primitiveTriggerLayer, collider.gameObject.layer)) this.gameScript.PrimitiveCollision(collider.transform.parent.gameObject);
    }
    private void OnCollisionEnter(Collision collision){
        //~ ground check → true
        if(
            !this.onGround
            && this.CheckIsLayerInMask(this.groundLayer, collision.gameObject.layer)
        )this.onGround = true;
    }
    private void OnCollisionExit(Collision collision){
        //~ ground check → false
        if(
            this.onGround
            && this.CheckIsLayerInMask(this.groundLayer, collision.gameObject.layer)
        )this.onGround = false;
    }
    //~ private methods
    /// <summary> Checks if the <paramref name="mask"/> contains the <paramref name="layer"/> </summary>
    /// <param name="mask"> A layer mask </param>
    /// <param name="layer"> the index of a layer </param>
    /// <returns> true if the <paramref name="mask"/> contains the <paramref name="layer"/> and false otherwise </returns>
    private bool CheckIsLayerInMask(LayerMask mask, int layer) => (mask & (1 << layer)) != 0;
}
