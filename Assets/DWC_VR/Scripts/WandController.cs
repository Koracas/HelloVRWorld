using UnityEngine;
using System.Collections;

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

        if( controller == null )
        {
            Debug.Log("Controller not initialized");
            return;
        }

        // Update our LineRenderer with the start/end vertices and color
        if (lineRenderer && lineRenderer.enabled)
        {
            RaycastHit hit;
            Vector3 startPos = transform.position;

            // If our raycast hits, end the line at that position. Otherwise,
            // just make our line point straight out for 1000 meters.
            // If the raycast hits, the line will be green, otherwise it'll be red.
            if (Physics.Raycast(startPos, transform.forward, out hit, 1000.0f))
            {
                lineRendererVertices[1] = hit.point;
                lineRenderer.SetColors(Color.green, Color.green);
            }
            else
            {
                lineRendererVertices[1] = startPos + transform.forward * 1000.0f;
                lineRenderer.SetColors(Color.red, Color.red);
            }

            lineRendererVertices[0] = transform.position;
            lineRenderer.SetPositions(lineRendererVertices);
        }
    }

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


    //
    // Override virtual controller methods for custom controls
    //

    public override void OnTriggerClicked(ClickedEventArgs e)
    {
        base.OnTriggerClicked(e);

        // We want to move the whole [CameraRig] around when we teleport,
        // which should be the parent of this controller. If we can't find the
        // [CameraRig], we can't teleport, so return.
        if (transform.parent == null)
            return;

        RaycastHit hit;
        Vector3 startPos = transform.position;

        Debug.Log("trigger clicked");

        // Perform a raycast starting from the controller's position and going 1000 meters
        // out in the forward direction of the controller to see if we hit something to teleport to.
        if (Physics.Raycast(startPos, transform.forward, out hit, 1000.0f))
        {
            transform.parent.position = hit.point;
        }
    }

    public override void OnTriggerUnclicked(ClickedEventArgs e)
    {
        base.OnTriggerUnclicked(e);
    }

    public override void OnMenuClicked(ClickedEventArgs e)
    {
        base.OnMenuClicked(e);
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
    }

    public override void OnUngripped(ClickedEventArgs e)
    {
        base.OnUngripped(e);
    }

}
