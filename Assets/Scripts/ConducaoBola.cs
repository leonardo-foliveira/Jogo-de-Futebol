using UnityEngine;

public class ConducaoBola : MonoBehaviour
{
    public Transform pontoConducao;
    public string tagBola = "Bola";

    public float velocidadeMax = 8f;
    public float suavizacao = 12f;
    public float distanciaSolta = 2.0f;

    public PosseBola posse;
    public Transform dono;   // OponenteRoot ou PlayerRoot
    public int timeId = 0;   // 0 casa, 1 fora

    private Rigidbody rbBola;
    private float tempoSuspenso = 0f;

    void FixedUpdate()
    {
        if (tempoSuspenso > 0f)
        {
            tempoSuspenso -= Time.fixedDeltaTime;
            return;
        }

        if (rbBola == null || pontoConducao == null) return;

        // só conduz se for o dono atual
        if (posse != null && dono != null && !posse.EhDono(dono)) return;

        Vector3 alvo = pontoConducao.position;
        Vector3 delta = alvo - rbBola.position;

        // se afastou muito, aí sim solta
        if (delta.magnitude > distanciaSolta)
        {
            rbBola = null;
            if (posse != null && dono != null) posse.SoltarSeDono(dono);
            return;
        }

        delta.y = 0f;

        Vector3 velDesejada = delta / Time.fixedDeltaTime;
        velDesejada = Vector3.ClampMagnitude(velDesejada, velocidadeMax);

#if UNITY_6000_0_OR_NEWER
        rbBola.linearVelocity = Vector3.Lerp(rbBola.linearVelocity, velDesejada, suavizacao * Time.fixedDeltaTime);
#else
        rbBola.velocity = Vector3.Lerp(rbBola.velocity, velDesejada, suavizacao * Time.fixedDeltaTime);
#endif
    }

    public void SuspenderConducao(float segundos)
    {
        tempoSuspenso = Mathf.Max(tempoSuspenso, segundos);
    }

    void OnTriggerEnter(Collider other) => TentarCapturar(other);
    void OnTriggerStay(Collider other) => TentarCapturar(other);

    void TentarCapturar(Collider other)
    {
        if (tempoSuspenso > 0f) return;  // NÃO captura durante chute/passe
        if (!other.CompareTag(tagBola)) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;

        // assume/rouba posse
        if (posse != null && dono != null)
        {
            bool ok = posse.TentarAssumir(dono, timeId);
            if (!ok) posse.TentarRoubar(dono, timeId);
        }

        // mantém a referência (mesmo se a bola sair do trigger depois)
        rbBola = rb;
    }

    public Rigidbody PegarBolaAtual() => rbBola;

    public void SoltarBola(float suspenderSegundos = 0.35f)
    {
        if (posse != null && dono != null) posse.SoltarSeDono(dono);
        rbBola = null;
        SuspenderConducao(suspenderSegundos);
    }

    // ✅ IMPORTANTE: não zera rbBola no Exit
    // (se quiser, pode até apagar esse método)
    void OnTriggerExit(Collider other)
    {
        // não fazer nada
    }
}