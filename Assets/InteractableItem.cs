using UnityEngine;
using System.Collections;

public class InteractableItem : MonoBehaviour {
    protected Rigidbody myRigidBody;

    private bool currentlyInteracting;

    private WandController attachedWand;

    private Transform interactionPt;

    private float velFactor = 20000f;
    private Vector3 posDelta;
    private float rotFactor = 600f;
    private Quaternion rotDelta;
    private float angle;
    private Vector3 axis;
     

	// Use this for initialization
	protected void Start () {
        myRigidBody = GetComponent<Rigidbody>();
        interactionPt = new GameObject().transform;

        //  The obj's vel factor is effected by how heavy the obj is
        velFactor /= myRigidBody.mass;
        //  The obj's rot factor is effected by how heavy the obj is 
        rotFactor /= myRigidBody.mass;
	}
	
	// Update is called once per frame : called once every 11 ms
    //  TODO :: Use fixedupdate for rigidbody manipulation if fps starts to drop
	protected void Update () {
	    if( attachedWand && currentlyInteracting)
        {
            //  Get delta btn your hand (wand) and the object
            posDelta = attachedWand.transform.position - interactionPt.position;

            //  Set the rigid body's vel based: diff on pos * vel factor * time
            this.myRigidBody.velocity = posDelta * velFactor * Time.fixedDeltaTime;

            //  Set the rigid body's rotation
            rotDelta = attachedWand.transform.rotation * Quaternion.Inverse(interactionPt.rotation);
            rotDelta.ToAngleAxis(out angle, out axis);
            //  If rotation is greater than 180
            if ( angle > 180 )
            {
                //  Just rotate the obj the other way to get to angle of choice
                angle -= 360;
            }
            this.myRigidBody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * rotFactor;

        }
	}

    //  Begin Interation by passing in the wand controller
    public void BeginInteraction( WandController wand )
    {
        //  Record the passed in wand 
        attachedWand = wand;

        //  Set the interaction point as the wand's position
        interactionPt.position = wand.transform.position;

        //  Set the interaction point as the wand's rotation
        interactionPt.rotation = wand.transform.rotation;

        //  Set the interaction point parent to the transform of the Game Object
        interactionPt.SetParent( transform, true );

        //  Flag that we are interacting 
        currentlyInteracting = true;

    }

    //  End Interaction
    public void EndInteraction( WandController wand )
    {
        //  Check that the passed in wand is the attached wand bc 2 controllers
        if( wand = attachedWand )
        {
            //  Shut down attachment
            attachedWand = null;
            
            //  Unflag that we are currently interacting
            currentlyInteracting = false;
        }
    }

    //  Method to let you know if you are interacting or not
    public bool isInteracting()
    {
        return currentlyInteracting;
    }
}
