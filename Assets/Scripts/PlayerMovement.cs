using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float velocidade = 6f;
    public float velocidadeRotacao = 12f;

    private CharacterController cc;
    private Vector3 velY;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector2 input = LerMovimentoCombinado();

        Vector3 dir = new Vector3(input.x, 0f, input.y);
        if (dir.sqrMagnitude > 1f) dir.Normalize();

        cc.Move(dir * velocidade * Time.deltaTime);

        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, velocidadeRotacao * Time.deltaTime);
        }

        if (cc.isGrounded && velY.y < 0f) velY.y = -1f;
        velY.y += Physics.gravity.y * Time.deltaTime;
        cc.Move(velY * Time.deltaTime);
    }

    Vector2 LerMovimentoCombinado()
    {
        Vector2 v = Vector2.zero;

        // Teclado (Input antigo)
        v += new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

#if ENABLE_INPUT_SYSTEM
        // Controle (Input novo)
        if (Gamepad.current != null)
        {
            Vector2 ls = Gamepad.current.leftStick.ReadValue();
            if (ls.magnitude < 0.15f) ls = Vector2.zero;
            v += ls;
        }

        // Teclado também pelo Input novo (fallback, se quiser)
        if (Keyboard.current != null)
        {
            float x = 0f, y = 0f;
            if (Keyboard.current.aKey.isPressed) x -= 1f;
            if (Keyboard.current.dKey.isPressed) x += 1f;
            if (Keyboard.current.sKey.isPressed) y -= 1f;
            if (Keyboard.current.wKey.isPressed) y += 1f;
            v += new Vector2(x, y);
        }
#endif

        if (v.sqrMagnitude > 1f) v.Normalize();
        return v;
    }
}