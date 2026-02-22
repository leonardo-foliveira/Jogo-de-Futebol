using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float velocidade = 6f;
    public float velocidadeRotacao = 12f;

    CharacterController cc;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // IMPORTANTE: clique na aba GAME para o W funcionar (foco da janela)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(h, 0f, v);
        if (move.sqrMagnitude > 1f) move.Normalize();

        cc.SimpleMove(move * velocidade);

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion alvo = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, alvo, velocidadeRotacao * Time.deltaTime);
        }
    }
}
