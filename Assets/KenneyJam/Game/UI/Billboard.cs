using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Vector3 oscillationSpeed = Vector3.zero;
    public Vector3 oscillationFactor = Vector3.one;

    private float oscillationTime;
    private Camera cam;
    private Vector3 localOffset;

    void Start()
    {
        cam = Camera.main;
        localOffset = transform.localPosition;
    }

    void Update()
    {
        transform.LookAt(cam.transform);
        transform.Rotate(0, 180, 0); // Flip to face camera correctly

        oscillationTime += Time.deltaTime;

        Vector3 oscillation = new Vector3(
            oscillationFactor.x * Mathf.Sin(oscillationSpeed.x * oscillationTime),
            oscillationFactor.y * Mathf.Sin(oscillationSpeed.y * oscillationTime),
            oscillationFactor.z * Mathf.Sin(oscillationSpeed.z * oscillationTime)
        );

        transform.localPosition = localOffset + oscillation;
}
}
