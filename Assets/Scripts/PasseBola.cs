using UnityEngine;

public class PasseBola : MonoBehaviour
{
    [Header("Refs")]
    public ConducaoBola conducao;
    public Transform pontoPasse;
    public TimeCasa timeCasa;

    [Header("Passe")]
    public KeyCode teclaPasse = KeyCode.X;
    public float forcaPasse = 10f;
    public float elevacao = 0.02f;
    public float suspenderConducao = 0.25f;

    [Header("Busca de alvo")]
    public float raioBusca = 30f;
    public float anguloMax = 70f;

    void Update()
    {
        if (Input.GetKeyDown(teclaPasse))
            TentarPasse();
    }

    void TentarPasse()
    {
        if (conducao == null || pontoPasse == null || timeCasa == null)
        {
            Debug.LogWarning("PasseBola: faltou conducao/pontoPasse/timeCasa.");
            return;
        }

        // só passa se você realmente está com a bola
        Rigidbody rbBola = conducao.PegarBolaAtual();
        if (rbBola == null)
        {
            Debug.Log("PasseBola: você não está com a bola agora.");
            return;
        }

        Transform alvo = BuscarMelhorAlvo();
        if (alvo == null)
        {
            Debug.Log("PasseBola: nenhum alvo válido encontrado (raio/ângulo).");
            return;
        }

        // IMPORTANTE: solta a bola e a posse antes do passe
        conducao.SoltarBola(suspenderConducao);

        Vector3 origem = pontoPasse.position;
        Vector3 destino = alvo.position + Vector3.up * 0.2f;
        Vector3 dir = (destino - origem).normalized;

        dir = (dir + Vector3.up * elevacao).normalized;

#if UNITY_6000_0_OR_NEWER
        rbBola.linearVelocity = Vector3.zero;
#else
        rbBola.velocity = Vector3.zero;
#endif
        rbBola.angularVelocity = Vector3.zero;

        rbBola.AddForce(dir * forcaPasse, ForceMode.Impulse);

        Debug.Log($"Passe para: {alvo.name}");
    }

    Transform BuscarMelhorAlvo()
    {
        Transform melhor = null;
        float melhorScore = float.MaxValue;

        Vector3 origem = transform.position;
        Vector3 frente = transform.forward;

        foreach (Transform j in timeCasa.jogadores)
        {
            if (j == null) continue;
            if (j == transform) continue;

            Vector3 delta = j.position - origem;
            float dist = delta.magnitude;
            if (dist > raioBusca) continue;

            float ang = Vector3.Angle(frente, delta);
            if (ang > anguloMax) continue;

            float score = ang * 2f + dist;
            if (score < melhorScore)
            {
                melhorScore = score;
                melhor = j;
            }
        }

        return melhor;
    }
}