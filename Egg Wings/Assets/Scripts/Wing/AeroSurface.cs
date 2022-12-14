using PerfectlyNormalUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://github.com/brihernandez/SimpleWings
//https://www.youtube.com/watch?v=87N7S0islxU&list=ULbANTtzpmg6E&index=208

//https://www.youtube.com/watch?v=e91QA4WfL5Q
//https://github.com/gasgiant/Aircraft-Physics

/// <summary>
/// This is a wing that will be used to apply forces to the rigid body based on position/rotation
/// </summary>
/// <remarks>
/// This needs to be tied to a game object.  The span will be the X scale, chord is Z scale
/// </remarks>
public class AeroSurface : MonoBehaviour
{
    public bool ShowDebugLines_Structure = false;
    public bool ShowDebugLines_Physics = false;
    public bool ShowDebugLines_Velocity = false;
    private AeroDebugLines _debug = null;

    /// <summary>
    /// 0 has no lift, minimum drag (like a thin flat sheet of metal)
    /// 1 has max lift, high drag (flat on bottom, big curve on top)
    /// </summary>
    /// <remarks>
    /// TODO: Make a function that turns this into lift coefficient, drag coefficient, skin friction, etc
    /// 
    /// NOTE: You don't want really large wings, because drag based on angular velocity won't be very accurate (applied at the
    /// center of the wing * wing area)
    /// </remarks>
    public float LiftScale;

    private AnimationCurve _lift_by_angle = null;       // lift curve by angle of attack. X axis should be from 0 to 180, with the Y axis being lift coeffient
    private AnimationCurve _drag_by_angle = null;       // drag curve by angle of attack. X axis should be from 0 to 180, with the Y axis being drag coeffient

    private Rigidbody _body = null;

    //TODO: May want a class that returns wind at point
    private Vector3 _wind = Vector3.zero;

    //TODO: May want a class that returns air density at point
    //private float _air_density = 1.2f;
    private float _air_density = 1.2f;     // making this large, because the wings are really small compared to the body's mass

    private void Awake()
    {
        _body = GetComponentInParent<Rigidbody>();      // this is recursive
    }
    void Start()
    {
        // Convert liftscale into actual properties
        _lift_by_angle = AeroCurveBuilder.GetLiftByPercent(LiftScale);
        _drag_by_angle = AeroCurveBuilder.GetDragByPercent(LiftScale);

        if (ShowDebugLines_Structure || ShowDebugLines_Physics || ShowDebugLines_Velocity)
        {
            _debug = gameObject.AddComponent<AeroDebugLines>();
            _debug.Show_Structure = ShowDebugLines_Structure;       //NOTE: the debug component's start doesn't fire until this start has finished, so it's safe to set these properties here
            _debug.Show_Physics = ShowDebugLines_Physics;
            _debug.Show_Velocity = ShowDebugLines_Velocity;
        }
    }

    private void FixedUpdate()
    {
        Vector3 force = CalculateForces();

        _body.AddForceAtPosition(force, transform.position, ForceMode.Force);
    }

    public Vector3 CalculateForces()
    {
        const float ANGULAR_VELOCITY_MULT = 0.4f;      // if full angular velocity is used, then jitter occurs at high speed with low moment of inertia planes

        Vector3 force_at_pos = transform.position - _body.worldCenterOfMass;        // needed for calculating wind based on angular velocity

        Vector3 air_vel_world = GetWorldAirVelocity_Smoothed(force_at_pos, _body.angularVelocity * ANGULAR_VELOCITY_MULT);

        float area = transform.lossyScale.x * transform.lossyScale.z;

        // This is the force of wind reflecting off the surface
        //Vector3 force_deflection = GetForce_Deflection(air_vel_world, area);      // this isn't needed, since the lift curve takes this into account (and has slight adjustments to it)

        Vector3 air_vel_local = transform.InverseTransformDirection(air_vel_world);
        air_vel_local.x = 0f;

        float dynamic_pressure = 0.5f * _air_density * air_vel_local.sqrMagnitude;

        float angle_of_attack = Vector3.Angle(Vector3.forward, air_vel_local);      // this is in degrees

        Vector3 force_lift = GetForce_Lift(air_vel_world, air_vel_local, dynamic_pressure, area, angle_of_attack);
        Vector3 force_drag = GetForce_Drag(air_vel_world, dynamic_pressure, area, angle_of_attack);

        if (_debug != null)
        {
            if (_debug.Show_Physics)
            {
                //_debug.Force_Deflect = force_deflection;
                _debug.Force_Lift = force_lift;
                _debug.Force_Drag = force_drag;
            }

            if (_debug.Show_Velocity)
            {
                _debug.Air_Velocity = air_vel_world;
            }
        }

        return force_lift + force_drag;
    }

    private Vector3 GetForce_Deflection(Vector3 air_vel_world, float area)
    {
        // Scale the normal based on how much of the flow is along it
        Vector3 force = Vector3.Project(air_vel_world, transform.up);      // the length returned is from zero to flowAtPoint.Length.  avgNormal's length is ignored, only its direction

        // Scale the force by the area of the rectangle
        // Also scale by viscocity
        force *= area * _air_density;

        return force;
    }

    private Vector3 GetForce_Lift(Vector3 air_vel_world, Vector3 air_vel_local, float dynamic_pressure, float area, float angle_of_attack)
    {
        float coefficient = _lift_by_angle.Evaluate(angle_of_attack);

        float force = dynamic_pressure * coefficient * area;

        force *= Mathf.Sign(air_vel_local.y);      // Vector3.Angle always returns a positive value, so add the sign back in

        Vector3 lift_direction = Vector3.Cross(air_vel_world, transform.right).normalized;      // lift is always perpendicular to air flow

        return lift_direction * force;
    }
    private Vector3 GetForce_Drag(Vector3 air_vel_world, float dynamic_pressure, float area, float angle_of_attack)
    {
        float coefficient = _drag_by_angle.Evaluate(angle_of_attack);       // why is coefficient 2 at 90 degrees?

        float force = dynamic_pressure * coefficient * area;

        return air_vel_world.normalized * force;
    }

    //NOTE: this is the opposite of velocity
    private Vector3 GetWorldAirVelocity_Smoothed(Vector3 force_at_pos, Vector3 angularVelocity)
    {
        //Vector3 vel_final = _body.GetPointVelocity(transform.position);       // here is how to do it if there is no manual adjustment to angular velocity (or standard)

        Vector3 vel_rot = Vector3.Cross(angularVelocity, force_at_pos);
        Vector3 vel_final = _body.velocity + vel_rot;

        return _wind - vel_final;
    }
}
