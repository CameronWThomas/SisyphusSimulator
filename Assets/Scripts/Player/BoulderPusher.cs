using UnityEngine;

public class BoulderPusher : MonoBehaviour
{
    public float forceMultiplier = 2f;

    private void OnCollisionStay(Collision collision)
    {
        CollisionStuff(collision);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CollisionStuff(collision);
    }

    private void CollisionStuff(Collision collision)
    {
        //TODO move boulder tag to constant?
        if (!collision.gameObject.CompareTag("Boulder"))
        {
            return;
        }
        if (collision.transform.GetComponent<Rigidbody>() is not Rigidbody boulderRigidbody)
        {
            throw new UnityException($"Boulder object is missing a {nameof(Rigidbody)}");
        }

        var direction = collision.transform.position - transform.position;
        direction.y = 0f;
        direction.Normalize();

        var rigidBody = GetComponent<Rigidbody>();
        var force = forceMultiplier * rigidBody.mass * direction;

        boulderRigidbody.AddForce(force, ForceMode.Impulse);
    }
}