using UnityEngine;

public class ControleJogadorPorPosse : MonoBehaviour
{
    public PosseBola posse;
    public Transform playerRoot;

    [Tooltip("Scripts que recebem input (PlayerMovement, PasseBola, ChuteBola, EmpurrarBola etc). Não coloque ConducaoBola.")]
    public MonoBehaviour[] scriptsDeInput;

    bool ultimoEstado;

    void Start()
    {
        Aplicar();
    }

    void Update()
    {
        Aplicar();
    }

    void Aplicar()
    {
        bool temPosse = (posse != null && playerRoot != null && posse.EhDono(playerRoot));
        if (temPosse == ultimoEstado) return;
        ultimoEstado = temPosse;

        foreach (var s in scriptsDeInput)
            if (s != null) s.enabled = temPosse;
    }
}