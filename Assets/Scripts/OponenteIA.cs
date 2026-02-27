using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class OponenteIA : MonoBehaviour
{
    [Header("Refs")]
    public Transform bola;
    public PosseBola posse;
    public Transform alvoGol;
    public ConducaoBola conducao;

    [Header("Time")]
    public int timeId = 1;

    [Header("Movimento")]
    public float velocidade = 5.5f;
    public float rotacao = 10f;

    [Header("Distâncias")]
    public float distParar = 0.8f;      // AUMENTEI (0.4 costuma gerar overshoot)
    public float distFreio = 2.0f;      // começa a reduzir a velocidade aqui
    public float distPerseguirBola = 25f;

    [Header("Debug")]
    public bool debug = true;
    public float debugInterval = 0.25f;

    CharacterController cc;
    float tDebug;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        tDebug = debugInterval;
    }

    void Update()
    {
        if (bola == null) return;

        bool temPosse = (posse != null && posse.EhDono(transform));
        bool conduzindo = (conducao != null && conducao.PegarBolaAtual() != null);

        Vector3 alvoPos;

        if (temPosse && alvoGol != null)
            alvoPos = alvoGol.position;
        else
            alvoPos = bola.position;

        Vector3 delta = alvoPos - transform.position;
        delta.y = 0f;

        float dist = delta.magnitude;

        // Debug visual: linha até o alvo
        if (debug)
        {
            Debug.DrawLine(transform.position + Vector3.up * 0.2f, alvoPos + Vector3.up * 0.2f, Color.yellow);
            Debug.DrawLine(transform.position + Vector3.up * 0.2f, transform.position + transform.forward * 2f + Vector3.up * 0.2f, Color.cyan);

            tDebug -= Time.deltaTime;
            if (tDebug <= 0f)
            {
                tDebug = debugInterval;
                string alvoNome = (temPosse && alvoGol != null) ? alvoGol.name : "BOLA";
                Debug.Log(
                    $"OPI | temPosse={temPosse} conduzindo={conduzindo} alvo={alvoNome} dist={dist:F2} " +
                    $"velCC={cc.velocity.magnitude:F2} donoAtual={(posse != null && posse.donoAtual != null ? posse.donoAtual.name : "null")}"
                );
            }
        }

        // Se já chegou, não anda (mata o círculo)
        if (dist <= distParar) return;

        Vector3 dir = delta / dist;

        // Rotaciona para a direção do movimento
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion alvoRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, alvoRot, rotacao * Time.deltaTime);
        }

        // desacelera quando está perto (evita overshoot/orbit)
        float fatorVel = 1f;
        if (dist < distFreio)
            fatorVel = Mathf.Clamp01((dist - distParar) / Mathf.Max(0.001f, (distFreio - distParar)));

        Vector3 move = dir * (velocidade * fatorVel) * Time.deltaTime;
        cc.Move(move);
    }
}