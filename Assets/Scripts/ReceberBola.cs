using UnityEngine;

public class ReceberBola : MonoBehaviour
{
    public string tagBola = "Bola";
    public PosseBola posse;        // arrasta do GameManager
    public Transform playerRoot;   // arrasta o root do jogador (PlayerRoot/Companheiro)
    public int timeId = 0;         // 0=casa, 1=fora

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagBola)) return;
        if (posse == null || playerRoot == null) return;

        // Se a bola estiver livre, este jogador assume
        if (posse.EstaLivre)
            posse.ForcarPosse(playerRoot, timeId);
    }

    void OnTriggerStay(Collider other)
    {
        // ajuda quando a bola passa rápido e não pega no Enter
        OnTriggerEnter(other);
    }
}