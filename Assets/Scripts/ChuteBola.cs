using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ChuteBola : MonoBehaviour
{
    [Header("Refs")]
    public ConducaoBola conducao;           // AreaBola (ConducaoBola) do MESMO jogador
    public Transform pontoChute;            // geralmente o PontoConducao
    public Transform referenciaDirecao;     // opcional: se o forward do objeto do script for torto

    [Header("Altura real (velocidade vertical)")]
    public float alturaMin = 1.0f;   // velocidade vertical mínima
    public float alturaMax = 6.0f;   // velocidade vertical máxima

    [Header("Input")]
    public bool usarMouse = true;
    public KeyCode teclaChuteTeclado = KeyCode.Space;  // segurar/soltar
    public bool usarControlePS5 = true;                // Quadrado

    [Header("Detecção da bola (fallback)")]
    public float raio = 0.8f;
    public string tagBola = "Bola";

    [Header("Carga")]
    public float tempoMaxCarga = 1.0f;     // 1s para carga total
    public bool usarForcaVariavel = true;  // força varia com carga
    public bool usarAlturaVariavel = true; // altura varia com carga

    [Header("Força do chute (horizontal)")]
    public float forcaMin = 8f;
    public float forcaMax = 18f;

    [Header("Pós chute")]
    public float suspenderConducao = 0.35f; // para não “puxar” a bola de volta

    [Header("Debug")]
    public bool mostrarCargaNoConsole = false;

    bool carregando;
    float cargaT; // 0..tempoMaxCarga
    bool fonteMouse, fonteTeclado, fonteControle;

    void Update()
    {
        // início da carga
        if (!carregando && ApertouChute())
        {
            carregando = true;
            cargaT = 0f;

            if (mostrarCargaNoConsole) Debug.Log("CHUTE: começou carga");
        }

        // carregando...
        if (carregando)
        {
            cargaT += Time.deltaTime;
            if (cargaT > tempoMaxCarga) cargaT = tempoMaxCarga;

            // soltou = chuta
            if (SoltouChute())
            {
                carregando = false;
                float norm = cargaT / Mathf.Max(0.0001f, tempoMaxCarga);
                ExecutarChute(norm);

                if (mostrarCargaNoConsole) Debug.Log("CHUTE: soltou");
            }
        }
    }

    bool ApertouChute()
    {
        fonteMouse = false; fonteTeclado = false; fonteControle = false;

        bool ok = false;

        if (usarMouse && Input.GetMouseButtonDown(0))
        {
            ok = true; fonteMouse = true;
        }

        if (Input.GetKeyDown(teclaChuteTeclado))
        {
            ok = true; fonteTeclado = true;
        }

#if ENABLE_INPUT_SYSTEM
        if (usarControlePS5 && Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame) // Quadrado
        {
            ok = true; fonteControle = true;
        }
#endif
        return ok;
    }

    bool SoltouChute()
    {
        // soltura pela mesma fonte que iniciou
        if (fonteMouse && Input.GetMouseButtonUp(0)) return true;
        if (fonteTeclado && Input.GetKeyUp(teclaChuteTeclado)) return true;

#if ENABLE_INPUT_SYSTEM
        if (fonteControle && Gamepad.current != null && Gamepad.current.buttonWest.wasReleasedThisFrame) return true;
#endif

        // fallback (caso a fonte não tenha sido marcada)
        if (!fonteMouse && !fonteTeclado && !fonteControle)
        {
            if (usarMouse && Input.GetMouseButtonUp(0)) return true;
            if (Input.GetKeyUp(teclaChuteTeclado)) return true;
#if ENABLE_INPUT_SYSTEM
            if (usarControlePS5 && Gamepad.current != null && Gamepad.current.buttonWest.wasReleasedThisFrame) return true;
#endif
        }

        return false;
    }

    void ExecutarChute(float norm) // norm = 0..1
    {
        if (pontoChute == null) return;

        Rigidbody rb = PegarBola();
        if (rb == null) return;

        // solta condução para a bola sair
        if (conducao != null)
            conducao.SoltarBola(suspenderConducao);

        // curva de carga
        float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(norm));

        // direção do chute
        Transform baseDir = (referenciaDirecao != null) ? referenciaDirecao : transform;

        Vector3 dir = baseDir.forward;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector3.forward;
        dir.Normalize();

        // força e altura
        float forca = usarForcaVariavel ? Mathf.Lerp(forcaMin, forcaMax, t) : forcaMax;
        float velUp = usarAlturaVariavel ? Mathf.Lerp(alturaMin, alturaMax, t) : alturaMax;

        // impulso separado: horizontal + vertical
        Vector3 impulso = dir * forca + Vector3.up * velUp;

        rb.angularVelocity = Vector3.zero;

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = Vector3.zero;
#else
        rb.velocity = Vector3.zero;
#endif

        rb.AddForce(impulso, ForceMode.VelocityChange);

        if (mostrarCargaNoConsole)
            Debug.Log($"CHUTE: t={t:F2} forca={forca:F1} velUp={velUp:F2}");
    }

    Rigidbody PegarBola()
    {
        // 1) bola conduzida
        if (conducao != null)
        {
            Rigidbody rb = conducao.PegarBolaAtual();
            if (rb != null) return rb;
        }

        // 2) fallback: procura perto do ponto
        Collider[] hits = Physics.OverlapSphere(pontoChute.position, raio);
        foreach (var h in hits)
        {
            if (h != null && h.CompareTag(tagBola))
                return h.attachedRigidbody;
        }

        return null;
    }

    void OnDrawGizmosSelected()
    {
        if (pontoChute == null) return;
        Gizmos.DrawWireSphere(pontoChute.position, raio);
    }
}