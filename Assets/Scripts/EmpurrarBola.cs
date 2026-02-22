using UnityEngine;

public class EmpurrarBola : MonoBehaviour
{
    public float forcaEmpurrao = 4f;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.collider.CompareTag("Bola")) return;

        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;

        Vector3 dir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
        rb.AddForce(dir * forcaEmpurrao, ForceMode.VelocityChange);
    }
}
