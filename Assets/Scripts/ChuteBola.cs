using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ChuteBola : MonoBehaviour
{
    [Header("Refs")]
    public ConducaoBola conducao;          // arraste o componente do AreaBola
    public Transform pontoChute;           // pode usar o PontoConducao
    [Tooltip("Base para direção do chute. Se ficar torto, arraste aqui o objeto que realmente aponta pra frente (ex: o modelo do jogador).")]
    public Transform referenciaDirecao;    // opcional: PlayerRoot ou o mesh do jogador

    [Header("Input")]
    public bool usarMouse = true;
    public KeyCode teclaChuteTeclado = KeyCode.Space;  // chute no teclado (troque se quiser)

    [Header("Chute")]
    public float raio = 0.8f;
    public float forcaChute = 12f;
    public float elevacao = 0.08f;         // pequeno
    public string tagBola = "Bola";
    public float suspenderConducao = 0.35f;

    void Update()
    {
        bool apertouMouse = usarMouse && Input.GetMouseButtonDown(0);
        bool apertouTeclado = Input.GetKeyDown(teclaChuteTeclado);

#if ENABLE_INPUT_SYSTEM
        bool apertouQuadrado = (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame); // Quadrado
#else
        bool apertouQuadrado = false;
#endif

        if (apertouMouse || apertouTeclado || apertouQuadrado)
            Chutar();
    }

    void Chutar()
    {
        if (pontoChute == null) return;

        Rigidbody rb = null;

        // 1) Preferir a bola que você está conduzindo (mais confiável)
        if (conducao != null)
            rb = conducao.PegarBolaAtual();

        // 2) Fallback, procura por overlap
        if (rb == null)
        {
            Collider[] hits = Physics.OverlapSphere(pontoChute.position, raio);
            foreach (var h in hits)
            {
                if (h != null && h.CompareTag(tagBola))
                {
                    rb = h.attachedRigidbody;
                    break;
                }
            }
        }

        if (rb == null) return;

        // solta a condução pra bola sair
        if (conducao != null)
            conducao.SoltarBola(suspenderConducao);

        // direção do chute (evita "torto" se o script estiver num objeto com forward errado)
        Transform baseDir = (referenciaDirecao != null) ? referenciaDirecao : transform;

        Vector3 dir = baseDir.forward;
        dir.y = 0f;

        // fallback se der algum caso bizarro
        if (dir.sqrMagnitude < 0.0001f)
            dir = Vector3.forward;

        dir.Normalize();

        Vector3 impulso = (dir + Vector3.up * elevacao).normalized * forcaChute;

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
#else
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
#endif
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(impulso, ForceMode.VelocityChange);
    }

    void OnDrawGizmosSelected()
    {
        if (pontoChute == null) return;
        Gizmos.DrawWireSphere(pontoChute.position, raio);
    }
}