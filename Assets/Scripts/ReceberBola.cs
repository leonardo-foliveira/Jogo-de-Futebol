using UnityEngine;

public class ReceberBola : MonoBehaviour
{
    public string tagBola = "Bola";
    public PosseBola posse;
    public Transform playerRoot;
    public int timeId = 0;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagBola)) return;
        if (posse == null || playerRoot == null) return;

        // SEMPRE força posse quando a bola entra na área
        posse.ForcarPosse(playerRoot, timeId);
    }

    void OnTriggerStay(Collider other)
    {
        OnTriggerEnter(other);
    }
}