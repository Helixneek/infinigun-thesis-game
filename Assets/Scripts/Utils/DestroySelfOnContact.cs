using UnityEngine;

public class DestroySelfOnContact : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.attachedRigidbody && collision.attachedRigidbody.gameObject.layer == targetLayer)
        {
            Destroy(gameObject);
        }
    }
}
