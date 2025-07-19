using UnityEngine;

[CreateAssetMenu(fileName = "New Car Stats", menuName = "Scriptable Objects/CarStats")]
public class CarStats : ScriptableObject
{
    [Header("Movement")]
    public float acceleration = 2f;
    public float maxSpeed = 10f;
    public float brakeForce = 3f;
    public float reverseForce = 1.5f;
    public float turnTorque = 500f;
    public float maxTurnSpeed = 300f;
    public float turnSmoothingEdge1 = -0.1f;
    public float turnSmoothingEdge2 = 0.1f;
    public AnimationCurve turningMotorCurve;

    [Header("Physics")]
    public float mass = 2000f;
    public float linearDamping = 5f;
    public float angularDamping = 3f;

    [Header("Visual/Audio")]
    public string carName = "Default Car";
    public float engineVolumeLinearVelocityFactor = 1;
    public float engineVolumeAngularVelocityFactor = 1;
    public float engineVolumeLerp = .05f;
    public float engineGlobalVolue = .3f;

    [Header("Gameplay")]
    public int maxHealth = 20;
}
