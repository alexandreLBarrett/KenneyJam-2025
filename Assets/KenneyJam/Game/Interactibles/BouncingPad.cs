using UnityEngine;

public class BouncingPad : MonoBehaviour
{
    [SerializeField]
    private float ImpulseForce = 1f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.body.GetComponentInParent<CarController>() != null)
        {
            Vector3 v = (collision.body.transform.position - transform.position).normalized;
            //v.y = Mathf.Abs(v.y);
            collision.rigidbody.AddForce(v * ImpulseForce, ForceMode.Impulse);
        }
    }
}
