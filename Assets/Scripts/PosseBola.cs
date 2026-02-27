using UnityEngine;

public class PosseBola : MonoBehaviour
{
    [Header("Bola")]
    public Transform bola;
    public Rigidbody rbBola;

    [Header("Camera")]
    public FollowCam followCam;

    [Header("Time do jogador humano (camera não segue o adversário)")]
    public int timeHumano = 0; // normalmente 0 = seu time

    [Header("Opcional")]
    public Transform donoInicial; // por ex.: PlayerRoot
    public int timeInicial = 0;

    [Header("Debug")]
    public Transform donoAtual;
    public int timeAtual = -1;

    private Transform ultimoHumano; // último jogador do timeHumano que teve a posse

    public bool EstaLivre => donoAtual == null;
    public bool EhDono(Transform t) => donoAtual == t;

    void Start()
    {
        // define um alvo seguro pra câmera no começo
        if (donoInicial != null && timeInicial == timeHumano)
            ultimoHumano = donoInicial;

        FixarCameraNoHumano();
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

        SoltarBolaFisica();

        // NÃO mexe na câmera aqui
        FixarCameraNoHumano();
    }

    public void SoltarSeDono(Transform t)
    {
        if (donoAtual == t) Soltar();
    }

    // Use quando quiser forçar posse (ex.: recepção de passe)
    public void ForcarPosse(Transform novoDono, int novoTime)
    {
        if (novoDono == null) return;
        DefinirDono(novoDono, novoTime);
    }

    // Chame isso após gol/reset pra garantir câmera no humano
    public void FixarCameraNoHumano()
    {
        if (followCam == null) return;

        if (ultimoHumano != null) followCam.SetAlvo(ultimoHumano);
        else if (donoInicial != null) followCam.SetAlvo(donoInicial);
    }

    private void DefinirDono(Transform novoDono, int novoTime)
    {
        donoAtual = novoDono;
        timeAtual = novoTime;

        // atualiza "último humano" só se for do time humano
        if (novoTime == timeHumano)
            ultimoHumano = novoDono;

        // gruda bola no dono (se você estiver usando isso)
        GrudarBolaNoDono(novoDono);

        // câmera: só segue se for time humano, senão mantém no último humano
        if (followCam != null)
        {
            if (novoTime == timeHumano) followCam.SetAlvo(novoDono);
            else FixarCameraNoHumano();
        }
    }

    private void GrudarBolaNoDono(Transform dono)
    {
        if (bola == null || rbBola == null || dono == null) return;

        Transform ponto = dono.Find("PontoConducao");
        if (ponto == null) ponto = dono;

        rbBola.isKinematic = true;
        rbBola.useGravity = false;

        bola.SetParent(ponto);
        bola.localPosition = Vector3.zero;
        bola.localRotation = Quaternion.identity;
    }

    private void SoltarBolaFisica()
    {
        if (bola == null || rbBola == null) return;

        bola.SetParent(null);

        rbBola.isKinematic = false;
        rbBola.useGravity = true;
    }
}