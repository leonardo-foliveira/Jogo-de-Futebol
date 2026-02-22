using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class OponenteIA : MonoBehaviour
{
    public Transform bola;
    public PosseBola posse;

    public Transform alvoComPosse;   // arraste o AlvoGol_Oponente aqui
    public int timeId = 1;           // opcional, só pra organização

    public float velocidade = 5.5f;
    public float rotacao = 10f;
    public ConducaoBola conducao; // arraste o ConducaoBola do AreaBola do oponente aqui


    private CharacterController cc;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (bola == null) return;

        bool comPosse = (posse != null && posse.EhDono(transform) && conducao != null && conducao.PegarBolaAtual() != null);

        Vector3 alvo;

        // Se tem posse, vai para o gol (ou segue em frente se não tiver alvo setado)
        if (comPosse)
        {
            if (alvoComPosse != null) alvo = alvoComPosse.position;
            else alvo = transform.position + transform.forward * 10f;
        }
        else
        {
            // Sem posse, corre até a bola
            alvo = bola.position;
        }

        Vector3 dir = alvo - transform.position;
        dir.y = 0f;

        if (dir.magnitude < 0.2f) return;

        dir.Normalize();

        cc.SimpleMove(dir * velocidade);

        Quaternion q = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, rotacao * Time.deltaTime);
    }
}
