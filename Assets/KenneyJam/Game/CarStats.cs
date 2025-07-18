using UnityEngine;

[CreateAssetMenu(fileName = "New Car Stats", menuName = "Scriptable Objects/CarStats")]
public class CarStats : ScriptableObject
{
    [Header("Movement")]
    public float acceleration = 10f;
    public float maxSpeed = 50f;
    public float brakeForce = 20f;
    public float reverseForce = 7.5f;
    public float turnTorque = 1700f;
    public float maxTurnSpeed = 1250f;

    [Header("Physics")]
    public float mass = 1000f;
    public float linearDamping = 5f;
    public float angularDamping = 1.0f;

    [Header("Visual/Audio")]
    public string carName = "Default Car";
}
