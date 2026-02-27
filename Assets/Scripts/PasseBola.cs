using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PasseBola : MonoBehaviour
{
    [Header("Refs")]
    public ConducaoBola conducao;
    public Transform pontoPasse;
    public TimeCasa timeCasa;

    [Header("Passe")]
    public KeyCode teclaPasse = KeyCode.X;     // teclado
    public float forcaPasse = 10f;
    public float elevacao = 0.0f;             // 0 = rasteiro de verdade
    public float suspenderConducao = 0.25f;

    [Header("Busca de alvo")]
    public float raioBusca = 30f;
    public float anguloMax = 70f;

    private Rigidbody rbBola;

    void Start()
    {
        // Bola
        GameObject bolaObj = GameObject.FindGameObjectWithTag("Bola");
        if (bolaObj != null) rbBola = bolaObj.GetComponent<Rigidbody>();

        // Auto-fill básico (evita warning)
        if (conducao == null) conducao = GetComponentInChildren<ConducaoBola>();
        if (pontoPasse == null) pontoPasse = transform.Find("PontoConducao");
        if (timeCasa == null) timeCasa = FindObjectOfType<TimeCasa>();
    }

    void Update()
    {
        if (ApertouPasse())
            TentarPasse();
    }

    bool ApertouPasse()
    {
        bool ok = Input.GetKeyDown(teclaPasse);

#if ENABLE_INPUT_SYSTEM
        // X (Cross) no DualSense = buttonSouth
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
            ok = true;
#endif

        return ok;
    }

    void TentarPasse()
    {
        if (rbBola == null || pontoPasse == null || timeCasa == null)
        {
            Debug.LogWarning("PasseBola: faltou rbBola/pontoPasse/timeCasa.");
            return;
        }

        // Só quem tem posse pode passar (se estiver usando posse)
        if (conducao != null && conducao.posse != null && conducao.dono != null && !conducao.posse.EhDono(conducao.dono))
        {
            Debug.Log("PasseBola: você não está com a bola agora.");
            return;
        }

        Vector3 dirPreferida = PegarDirecaoPreferida();

        Transform alvo = BuscarMelhorAlvo(dirPreferida);
        Vector3 origem = pontoPasse.position;

        Vector3 dir;
        if (alvo != null)
        {
            Vector3 destino = alvo.position + Vector3.up * 0.2f;
            dir = (destino - origem).normalized;
        }
        else
        {
            // Se não achou alvo, passa reto na direção escolhida (ou pra frente)
            dir = (dirPreferida.sqrMagnitude > 0.0001f) ? dirPreferida.normalized : transform.forward;
        }

        if (elevacao > 0f) dir = (dir + Vector3.up * elevacao).normalized;

        // solta condução por um instante
        if (conducao != null) conducao.SoltarBola(suspenderConducao);

#if UNITY_6000_0_OR_NEWER
        rbBola.linearVelocity = Vector3.zero;
#else
        rbBola.velocity = Vector3.zero;
#endif
        rbBola.angularVelocity = Vector3.zero;

        rbBola.AddForce(dir * forcaPasse, ForceMode.Impulse);

        if (alvo != null) Debug.Log($"Passe para: {alvo.name}");
        else Debug.Log("Passe: sem alvo, passando reto.");
    }

    Vector3 PegarDirecaoPreferida()
    {
        // Se tiver analógico apontado, usa ele em vez do forward
#if ENABLE_INPUT_SYSTEM
        if (Gamepad.current != null)
        {
            Vector2 ls = Gamepad.current.leftStick.ReadValue();
            if (ls.magnitude >= 0.25f)
            {
                Vector3 local = new Vector3(ls.x, 0f, ls.y);
                return transform.TransformDirection(local); // vira direção mundo
            }
        }
#endif
        return transform.forward;
    }

    Transform BuscarMelhorAlvo(Vector3 dirPreferida)
    {
        Transform melhor = null;
        float melhorScore = float.MaxValue;

        Vector3 origem = transform.position;
        Vector3 frente = (dirPreferida.sqrMagnitude > 0.0001f) ? dirPreferida.normalized : transform.forward;

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