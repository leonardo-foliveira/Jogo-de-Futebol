using UnityEngine;

public class ChuteBola : MonoBehaviour
{
    public ConducaoBola conducao;          // arrastar o componente do AreaBola
    public Transform pontoChute;           // pode usar o PontoConducao
    public float raio = 0.8f;
    public float forcaChute = 12f;
    public float elevacao = 0.08f;         // bem pequeno
    public string tagBola = "Bola";
    public float suspenderConducao = 0.35f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // clique esquerdo, pode trocar para Space depois
            Chutar();
    }

    void Chutar()
    {
        if (pontoChute == null) return;

        Rigidbody rb = null;

        if (conducao != null)
            rb = conducao.PegarBolaAtual();

        if (rb == null)
        {
            Collider[] hits = Physics.OverlapSphere(pontoChute.position, raio);
            foreach (var h in hits)
            {
                if (h.CompareTag(tagBola))
                {
                    rb = h.attachedRigidbody;
                    break;
                }
            }
        }

        if (rb == null) return;

        if (conducao != null)
            conducao.SoltarBola(suspenderConducao);

        Vector3 dir = transform.forward;
        dir.y = 0f;
        dir.Normalize();

        Vector3 impulso = (dir + Vector3.up * elevacao).normalized * forcaChute;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(impulso, ForceMode.VelocityChange);
    }

    void OnDrawGizmosSelected()
    {
        if (pontoChute == null) return;
        Gizmos.DrawWireSphere(pontoChute.position, raio);
    }
}
