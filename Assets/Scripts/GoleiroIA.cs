using UnityEngine;

public class GoleiroIA : MonoBehaviour
{
    public Transform bola;
    public Rigidbody rbBola;

    public Transform linha;          // LinhaGoleiro_Fora (centro do gol)
    public float limiteLateral = 3.2f;
    public float offsetZ = 0.4f;

    public float velocidadeMin = 2.0f;     // bola precisa estar “indo”
    public float tempoMaxPrev = 1.2f;      // prever até 1.2s
    public float cooldownDefesa = 0.8f;

    public Animator anim;

    float proximaAcao = 0f;

    void Reset()
    {
        anim = GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
        if (bola == null || rbBola == null || linha == null || anim == null) return;
        if (Time.time < proximaAcao) return;

        Vector3 v = rbBola.linearVelocity; // se não existir no seu Unity, troque por rbBola.velocity
        if (v.magnitude < velocidadeMin) return;

        // Vetor da bola até o centro do gol
        Vector3 toGoal = (linha.position - bola.position);

        // Se a bola não está indo na direção do gol, ignora
        if (Vector3.Dot(v, toGoal) <= 0f) return;

        // Plano do gol: use o forward da linha apontando PARA O CAMPO.
        Vector3 n = linha.forward;

        // Queremos quando a bola vai cruzar o plano da linha vindo do campo para o gol (direção -n)
        float denom = Vector3.Dot(v, -n);
        if (denom <= 0.05f) return;

        float dist = Vector3.Dot((linha.position - bola.position), -n);
        float t = dist / denom;

        if (t < 0f || t > tempoMaxPrev) return;

        Vector3 ponto = bola.position + v * t;

        // Lateral relativo ao gol (esquerda/direita)
        float lateral = Vector3.Dot(ponto - linha.position, linha.right);

        // Se vai bater fora da área coberta, não faz nada
        if (Mathf.Abs(lateral) > limiteLateral) return;

        // Decide ação
        // (ajuste os limites conforme seu gosto)
        anim.ResetTrigger("T_Catch");
        anim.ResetTrigger("T_Block");
        anim.ResetTrigger("T_Dive");

        if (Mathf.Abs(lateral) < 0.7f)
        {
            Debug.Log("GOLEIRO DEFESA: Catch (t=" + t + ")");
            anim.SetTrigger("T_Catch");
        }
        else if (Mathf.Abs(lateral) < 1.8f)
        {
            Debug.Log("GOLEIRO DEFESA: Block (t=" + t + ")");
            anim.SetTrigger("T_Block");
        }
        else
        {
            Debug.Log("GOLEIRO DEFESA: Dive (t=" + t + ")");
            anim.SetTrigger("T_Dive");
        }

        proximaAcao = Time.time + cooldownDefesa;

        // opcional: manter ele alinhado na linha do gol
        Vector3 alvo = linha.position + linha.forward * offsetZ;
        alvo += linha.right * Mathf.Clamp(lateral, -limiteLateral, limiteLateral);
        alvo.y = transform.position.y;
        transform.position = Vector3.Lerp(transform.position, alvo, 0.5f);
    }
}
