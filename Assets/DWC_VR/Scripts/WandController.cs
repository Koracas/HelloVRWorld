using UnityEngine;
using System.Collections;

public class WandController : SteamVR_TrackedController
{
    //  Variable used to retrieves the controller properties 
    public SteamVR_Controller.Device controller {
        get {
            return SteamVR_Controller.Input((int)controllerIndex);
        }
    }

    //  Get the Velocity 
    public Vector3 velocity {
        get { return controller.velocity; }
    }
    
    //  Get the Angular Velocity
    public Vector3 angularVelocity {
        get { return controller.angularVelocity; }
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


    // Use this for initialization
    //  Modified SteamVR_TrackedController s.t. it is protected override
    protected override void Start () {
        //  Let the parent-class implementaion of these methods run
        base.Start();
	}

    // Update is called once per frame
    //  Modified SteamVR_TrackedController s.t. it is protected override
    protected override void Update () {
        //  Let the parent-class implementaion of these methods run
        base.Update();
    }

    // Override virtual controller methods for custom controls

    public override void OnTriggerClicked(ClickedEventArgs e)
    {
        base.OnTriggerClicked(e);
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
 