using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovementSys : MonoBehaviour{

    [SerializeField] private float speed = 2.0F;          //the moving speed of the character
    [SerializeField] private float jumpSpeed = 10.0f;      //the jump force of the character
    [SerializeField] private float rotateSpeed = 100.0f;
                     private float timer = 0;
    [SerializeField] private float roamingTime = 3;

    [SerializeField] private float gravity = 20.0f;       //the force of gravity on the character

    private float groundOffset = .2f;    //the offset for the IsGrounded check. Useful for recognizing slopes and imperfect ground.

    private Vector3 moveDirection = Vector3.zero;   //the direction the character should move.
    private Vector3 jumpDirection = Vector3.zero;
    private Quaternion rotationStart, rotationEnd;

    [Tooltip("Transform of target")]
    [SerializeField] private Vector3 TargetPosition;//Starting position to pathfind to

    private Vector3 LastPositon;
    private bool isPathing;
    public  bool roaming = false;

    [Tooltip("Distance target must move betweeen calculations")]
    [SerializeField] private float recalculationDistance; 
    
    UnityEngine.AI.NavMeshAgent agent;

    private Rigidbody rb;

    private void Awake(){//When the program start
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();// get the NavMesh agent 
        TargetPosition = GameObject.FindGameObjectWithTag("Player").transform.position; //Debug purposes, will change later   
    }

    private void Update()//Every frame
    {
        Roam();
        LastPositon = transform.position;
        Vector3 temp = transform.position;
        //if the target has moved more than recalculation distance from the original destination
        Vector2 targetMoved = new Vector2(TargetPosition.x - LastPositon.x,TargetPosition.z-LastPositon.z);
        if(targetMoved.magnitude > recalculationDistance){
            isPathing = true;
            agent.destination = TargetPosition; 
        }
        isPathing = false;
    }

    public void SetNewTarget(Vector3 target){
        TargetPosition = target;
    }

    public bool isMoving(){
        return isPathing; //
    }

    ///The check to see if the character is currently on the ground.
    private bool isGrounded()
    {
        RaycastHit hit;
        Physics.Raycast(this.transform.position, -this.transform.up, out hit, 10);   //A short ray shot directly downward from the center of the character.

        if (System.Math.Abs(hit.distance) < System.Single.Epsilon)                                           //if the distance is zero, the ray probably did not hit anything.
        {
            return false;
        }
        if (hit.distance <= (this.transform.lossyScale.y / 2 + groundOffset))   //if the distance from the ray is less than half the height 
        {                                                                   //of the character (plus the offset), the character us grounded.
            return true;
        }
        return false;
    }

    /*
        Roam will pick a random position on the navmesh to move to, and will go somewhere only if roaming is toggled true 
        
    */
    public void Roam(){
        //For now the min and max will be set to the testing arena map, but we'll have a lookup and store the variables somewhere for roaming later
        int maxX=10,minX=-10,maxY=1,minY=1,maxZ=10,minZ=-10;
        if(roaming){
            timer += Time.deltaTime;
            if(timer >= roamingTime){
                SetNewTarget(GenerateRandomPos(maxX,minX,maxY,minY,maxZ,minZ));
                timer = 0;
            }
            
        }
        
    }
    /*
        GenerateRandomPos will generate a random position within the given boundaries
        Unless we can pull the dimensions of the nammesh, for now we can just store the boundaries that the AI can roam to.
    */
    private Vector3 GenerateRandomPos(int maxX,int minX,int maxY,int minY,int maxZ,int minZ){
        int randX = Random.Range(minX,maxX);
        int randY = Random.Range(minY,maxY);
        int randZ = Random.Range(minZ,maxZ);
        Vector3 newPos = new Vector3(randX,randY,randZ);
        return newPos;
    }

}