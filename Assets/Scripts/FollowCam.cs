using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform alvo;
    public Vector3 offset = new Vector3(0f, 3f, -7f);
    public float suavidade = 10f;

    public void SetAlvo(Transform novoAlvo)
    {
        alvo = novoAlvo;
    }

    void LateUpdate()
    {
        if (alvo == null) return;

        Vector3 desejado = alvo.position + offset;
        transform.position = Vector3.Lerp(transform.position, desejado, suavidade * Time.deltaTime);
        transform.LookAt(alvo.position + Vector3.up * 1.5f);
    }
}