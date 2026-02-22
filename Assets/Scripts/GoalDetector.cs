using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    public string tagBola = "Bola";
    public string mensagem = "GOL!";
    public bool foiGolCasa = true;

    public GerenciadorPartida gerenciador;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagBola)) return;
        if (gerenciador == null) return;

        gerenciador.RegistrarGol(foiGolCasa, mensagem);
    }
}