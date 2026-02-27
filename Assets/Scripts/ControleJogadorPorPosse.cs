using UnityEngine;

public class ControleJogadorPorPosse : MonoBehaviour
{
    public PosseBola posse;
    public Transform playerRoot;

    [Header("Sempre ativo (ex: PlayerMovement)")]
    public MonoBehaviour[] scriptsSempreAtivos;

    [Header("Só com posse (ex: ChuteBola, PasseBola, EmpurrarBola)")]
    public MonoBehaviour[] scriptsSomenteComPosse;

    bool ultimoTemPosse;

    void Start() => Aplicar(forcar: true);
    void Update() => Aplicar(forcar: false);

    void Aplicar(bool forcar)
    {
        if (posse == null || playerRoot == null) return;

        // Sempre ativo
        foreach (var s in scriptsSempreAtivos)
            if (s != null) s.enabled = true;

        bool temPosse = posse.EhDono(playerRoot);
        if (!forcar && temPosse == ultimoTemPosse) return;
        ultimoTemPosse = temPosse;

        foreach (var s in scriptsSomenteComPosse)
            if (s != null) s.enabled = temPosse;
    }
}