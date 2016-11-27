using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WandController : SteamVR_TrackedController
{
    //  Variable used to retrieves the controller properties 
    public SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)controllerIndex); } }

    //  Get the Velocity 
    public Vector3 velocity { get { return controller.velocity; } }

    //  Get the Angular Velocity
    public Vector3 angularVelocity { get { return controller.angularVelocity; } }

    //  Parameters used for line renderer on the controller
    protected LineRenderer lineRenderer;
    protected Vector3[] lineRendererVertices;

    //  Paramters for rigid body GameObject we are currently colliding with
    HashSet<InteractableItem> objsCollidingWith = new HashSet<InteractableItem>();
    private InteractableItem closestItem;
    private InteractableItem interactingItem;
    private bool objCollision;
    private bool raySCObjCollision;

    // Use this for initialization
    //  Modified SteamVR_TrackedController s.t. it is protected override
    protected override void Start()
    {
        //  Let the parent-class implementaion of these methods run
        base.Start();

        // Initialize our LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.SetWidth(0.01f, 0.01f);
        lineRenderer.SetVertexCount(2);

        // Initialize our vertex array. This will just contain
        // two Vector3's which represent the start and end locations
        // of our LineRenderer
        lineRendererVertices = new Vector3[2];
    }

    // Update is called once per frame
    //  Modified SteamVR_TrackedController s.t. it is protected override
    protected override void Update()
    {
        //  Let the parent-class implementaion of these methods run
        base.Update();

        //  IF the controller is activated
        if (controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }

        //  If there is no current collision object, raycast 
        if( interactingItem == null)
        {
            // Update our LineRenderer with the start/end vertices and color
            if (lineRenderer && lineRenderer.enabled)
            { 
                RaycastHit hit;
                Vector3 startPos = transform.position;
                bool rayHitShowcaseObj = false;
                bool rayHitPlane = false;
                bool rayHitDeadZone = false;         

                // If our raycast hits, end the line at that position. Otherwise,
                // just make our line point straight out for 1000 meters.
                // If the raycast hits, the line will be green, otherwise it'll be red.
                if (Physics.Raycast(startPos, transform.forward, out hit, 1000.0f))
                {
                    lineRendererVertices[1] = hit.point;                    
                    //Debug.Log("[Update] :: Hit : " + hit.collider.gameObject.name);

                    if( hit.collider.gameObject.name == "Plane")
                    {
                        rayHitPlane = true;
                        raySCObjCollision = false;
                    }
                    else if (hit.collider.gameObject.tag == "Heart")
                    {                        
                        rayHitShowcaseObj = true;
                        raySCObjCollision = true;
                    }
                    else
                    {
                        rayHitDeadZone = true;
                        raySCObjCollision = false;
                    }
                }
                else
                {
                    lineRendererVertices[1] = startPos + transform.forward * 1000.0f;
                    rayHitDeadZone = true;
                    raySCObjCollision = false;
                }

                lineRendererVertices[0] = transform.position;
                lineRenderer.SetPositions(lineRendererVertices);
                
                if(objCollision )
                {
                    lineRenderer.SetColors(Color.clear, Color.clear);                 
                }
                else if ( !objCollision && rayHitShowcaseObj)
                {
                    lineRenderer.SetColors(Color.blue, Color.blue);
                }
                else if (!objCollision && rayHitPlane)
                {
                    lineRenderer.SetColors(Color.green, Color.green);                    
                }
                else if (rayHitDeadZone)
                {
                    lineRenderer.SetColors(Color.red, Color.red);
                }

            }
        }
    }

    /*  Custom wand controller controls 
     * -----------------------------------------------------------
     */

    //  Get the trigger axis from the controller
    public float GetTriggerAxis()
    {
        // If the controller isn't valid, return 0
        if (controller == null)
            return 0;

        // Use SteamVR_Controller.Device's GetAxis() method (mentioned earlier) to get the trigger's axis value
        return controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis1).x;
    }

    //  Get the touchpad info from the controller
    public Vector2 GetTouchpadAxis()
    {
        // If the controller isn't valid, return (basically) 0
        if (controller == null)
            return new Vector2();

        // Use SteamVR_Controller.Device's GetAxis() method (mentioned earlier) to get the touchpad's axis value
        return controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
    }

    // Override virtual controller methods for custom controls
    //------------------------------------------------------------

    public override void OnTriggerClicked(ClickedEventArgs e)
    {
        base.OnTriggerClicked(e);

        // We want to move the whole [CameraRig] around when we teleport,
        // which should be the parent of this controller. If we can't find the
        // [CameraRig], we can't teleport, so return.
        if (transform.parent == null)
            return;

        //  If there is a Ray Cast + Show Case Object Collision
        if(raySCObjCollision)
        {
            //  TODO :: do something interesting here
            Debug.Log("[OnTriggerClicked] :: show case object clicked");
        }
        //  else If the lineRendrer is enabled and there are no colliding objects allow teleport
        else if( lineRenderer.enabled && !objCollision)
        {
            RaycastHit hit; 
            Vector3 startPos = transform.position;

            Debug.Log("[OnTriggerClicked] :: teleportation clicked");

            // Perform a raycast starting from the controller's position and going 1000 meters
            // out in the forward direction of the controller to see if we hit something to teleport to.
            if (Physics.Raycast(startPos, transform.forward, out hit, 1000.0f))
            {
                transform.parent.position = hit.point;
            }
        }        
    }

    public override void OnTriggerUnclicked(ClickedEventArgs e)
    {
        base.OnTriggerUnclicked(e);
    }

    public override void OnMenuClicked(ClickedEventArgs e)
    {
        base.OnMenuClicked(e);

        Debug.Log("[OnMenuClicked] :: Teleporting to the start");

        // We want to move the whole [CameraRig] around when we teleport,
        // which should be the parent of this controller. If we can't find the
        // [CameraRig], we can't teleport, so return.
        if (transform.parent == null)
        {
            return;
        }
        //  Otherwise teleport me back to the start
        else
        {
            transform.parent.position = new Vector3(0, 0, 0);
        }

    }

    public override void OnMenuUnclicked(ClickedEventArgs e)
    {
        base.OnMenuUnclicked(e);
    }

    public override void OnSteamClicked(ClickedEventArgs e)
    {
        base.OnSteamClicked(e);
    }

    public override void OnPadClicked(ClickedEventArgs e)
    {
        base.OnPadClicked(e);
    }

    public override void OnPadUnclicked(ClickedEventArgs e)
    {
        base.OnPadUnclicked(e);
    }

    public override void OnPadTouched(ClickedEventArgs e)
    {
        base.OnPadTouched(e);
    }

    public override void OnPadUntouched(ClickedEventArgs e)
    {
        base.OnPadUntouched(e);
    }

    public override void OnGripped(ClickedEventArgs e)
    {
        base.OnGripped(e);

        //  Find the closest item via min dist
        float minDist = float.MaxValue;

        float dist;
        foreach ( InteractableItem item in objsCollidingWith)
        {
            //  This is not the actual distance but magnitude of distance
            dist = (item.transform.position - transform.position).sqrMagnitude;

            //  If closer to the hand
            if ( dist < minDist )
            {
                minDist = dist;
                closestItem = item;
            }
        }

        //  set current closest item as the interacting item
        interactingItem = closestItem;

        //  Wipe closest item cause it is no longer needed
        closestItem = null;

        //  Check non-null, there is interaction with an object 
        if (interactingItem)
        {
            //  Check that it is not already interacting with something
            if (interactingItem.isInteracting())
            {
                //  If so 86 taht prev interaction
                interactingItem.EndInteraction(this);
            }

            //  Begin new interaction
            interactingItem.BeginInteraction(this);

            Debug.Log("[OnGripped] :: Grabbing : " + interactingItem.name);
        }
    }

    public override void OnUngripped(ClickedEventArgs e)
    {
        base.OnUngripped(e);

        //  If the interaction item is not null 
        if (interactingItem != null)
        {
            //  86 that interaction
            interactingItem.EndInteraction( this);

            Debug.Log("[OnUngripped] :: Letting go of : " + interactingItem.name);

            interactingItem = null;
        }
    }


    /*  Collisions with rigid body
     * -----------------------------------------------------------
     */
    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("[OnTriggerEnter] :: Collider Entered ");

        //  Get the item that the wand controller collided with
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();

        //  Not all Game Objects will be InteractableItems so need to check not null
        if (collidedItem)
        {
            objsCollidingWith.Add(collidedItem);
            objCollision = true;
            Debug.Log("[OnTriggerEnter] :: objCollision ON");
        }

    }

    private void OnTriggerExit(Collider collider)
    {
        Debug.Log("[OnTriggerExit] :: Collider Exited ");

        //  Get the item that the wand controller collided with
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();

        //  Not all Game Objects will be InteractableItems so need to check not null
        if (collidedItem)
        {
            objsCollidingWith.Remove(collidedItem);
        }

        if (objsCollidingWith.Count == 0 )
        {
            objCollision = false;
            Debug.Log("[OnTriggerEnter] :: objCollision OFF");
        }
    }

}
