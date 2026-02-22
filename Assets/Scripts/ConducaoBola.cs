using UnityEngine;

public class ConducaoBola : MonoBehaviour
{
    public Transform pontoConducao;
    public string tagBola = "Bola";

    public float velocidadeMax = 8f;
    public float suavizacao = 12f;
    public float distanciaSolta = 2.0f;

    public PosseBola posse;
    public Transform dono;   // arraste o PlayerRoot aqui
    public int timeId = 0;   // 0 = seu time, 1 = adversário


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
        if (posse != null && dono != null && !posse.EhDono(dono)) return;


        Vector3 alvo = pontoConducao.position;
        Vector3 delta = alvo - rbBola.position;

        // Se a bola se afastar muito, solta
        if (delta.magnitude > distanciaSolta)
            {
                if (posse != null && dono != null) posse.SoltarSeDono(dono);
                rbBola = null;
                return;
            }


        // Mantém a bola no plano do chão (evita “levitar”)
        delta.y = 0f;

        Vector3 velDesejada = delta / Time.fixedDeltaTime;
        velDesejada = Vector3.ClampMagnitude(velDesejada, velocidadeMax);

        rbBola.linearVelocity = Vector3.Lerp(rbBola.linearVelocity, velDesejada, suavizacao * Time.fixedDeltaTime);
    }

    public void SuspenderConducao(float segundos)
    {
        tempoSuspenso = Mathf.Max(tempoSuspenso, segundos);
    }

    void OnTriggerEnter(Collider other)
    {
        TentarCapturar(other);
    }

    void OnTriggerStay(Collider other)
    {
        TentarCapturar(other);
    }

    void TentarCapturar(Collider other)
    {
        if (!other.CompareTag(tagBola)) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;

        if (posse != null && dono != null)
        {
            bool ok = posse.TentarAssumir(dono, timeId);
        if (!ok)
        {
            // roubo simples: se encostou na bola, rouba
            posse.TentarRoubar(dono, timeId);
}
        }

        rbBola = rb;
    }


    void OnTriggerExit(Collider other)
{
    if (rbBola == null) return;

    if (other.attachedRigidbody == rbBola)
    {
        rbBola = null;
        if (posse != null && dono != null) posse.SoltarSeDono(dono);
    }
}


    public Rigidbody PegarBolaAtual()
{
    return rbBola;
}

        public void SoltarBola(float suspenderSegundos = 0.35f)
    {
        if (posse != null && dono != null) posse.SoltarSeDono(dono);
        rbBola = null;
        SuspenderConducao(suspenderSegundos);
    }

    

}
