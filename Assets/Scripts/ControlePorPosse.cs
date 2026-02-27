using UnityEngine;

public class ControlePorPosse : MonoBehaviour
{
    public PosseBola posse;
    public Transform meuRoot;

    [Tooltip("Scripts que devem ligar/desligar com a posse (PlayerMovement, PasseBola, ChuteBola etc).")]
    public MonoBehaviour[] scriptsDeInput;

    bool estadoAnterior;

    void Update()
    {
        if (posse == null || meuRoot == null) return;

        bool ativo = posse.EhDono(meuRoot);

        if (ativo == estadoAnterior) return;
        estadoAnterior = ativo;

        foreach (var s in scriptsDeInput)
            if (s != null) s.enabled = ativo;
    }
}