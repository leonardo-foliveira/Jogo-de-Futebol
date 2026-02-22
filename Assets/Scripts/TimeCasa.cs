using UnityEngine;

public class TimeCasa : MonoBehaviour
{
    [Tooltip("Arraste aqui os ROOTS dos jogadores do time (PlayerRoot, Companheiro1, Companheiro2...)")]
    public Transform[] jogadores;

    public Transform MelhorAlvoPasse(Transform quemPassa, Vector3 origemPasse, float raioBusca, float anguloMax)
    {
        if (jogadores == null || jogadores.Length == 0) return null;

        Transform melhor = null;
        float melhorDist = float.MaxValue;

        Vector3 fwd = quemPassa.forward; fwd.y = 0f; fwd.Normalize();

        foreach (var t in jogadores)
        {
            if (t == null) continue;

            // não passa para si mesmo (considera root)
            if (t == quemPassa || t.root == quemPassa.root) continue;

            Vector3 dir = (t.position - origemPasse);
            dir.y = 0f;

            float dist = dir.magnitude;
            if (dist < 0.01f) continue;
            if (dist > raioBusca) continue;

            float ang = Vector3.Angle(fwd, dir.normalized);
            if (ang > anguloMax) continue;

            if (dist < melhorDist)
            {
                melhorDist = dist;
                melhor = t;
            }
        }

        return melhor;
    }
}