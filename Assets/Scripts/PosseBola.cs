using UnityEngine;

public class PosseBola : MonoBehaviour
{
    [Header("Camera")]
    public FollowCam followCam;

    [Header("Posse")]
    public Transform donoAtual;   // PlayerRoot de quem está com a posse
    public int timeAtual = -1;    // -1 = livre

    [Header("Dono inicial (opcional)")]
    public Transform donoInicial;
    public int timeInicial = 0;

    public bool EstaLivre => donoAtual == null;
    public bool EhDono(Transform t) => donoAtual == t;

    void Start()
    {
        if (donoInicial != null)
            ForcarPosse(donoInicial, timeInicial);
        else if (donoAtual != null && followCam != null)
            followCam.SetAlvo(donoAtual);
    }

    public bool TentarAssumir(Transform novoDono, int novoTime)
    {
        if (novoDono == null) return false;

        if (donoAtual == null)
        {
            DefinirDono(novoDono, novoTime);
            return true;
        }

        return donoAtual == novoDono;
    }

    public bool TentarRoubar(Transform ladrao, int timeLadrao)
    {
        if (ladrao == null) return false;

        DefinirDono(ladrao, timeLadrao);
        return true;
    }

    public void Soltar()
    {
        donoAtual = null;
        timeAtual = -1;
        Debug.Log("POSSE: livre");
    }

    public void SoltarSeDono(Transform t)
    {
        if (donoAtual == t) Soltar();
    }

    public void ForcarPosse(Transform novoDono, int novoTime)
    {
        if (novoDono == null) return;
        DefinirDono(novoDono, novoTime);
    }

    private void DefinirDono(Transform novoDono, int novoTime)
    {
        donoAtual = novoDono;
        timeAtual = novoTime;

        if (followCam != null) followCam.SetAlvo(novoDono);

        Debug.Log($"POSSE: {novoDono.name} (time {novoTime})");
    }
}