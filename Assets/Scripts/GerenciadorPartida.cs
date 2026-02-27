using System.Collections;
using UnityEngine;

public class GerenciadorPartida : MonoBehaviour
{
    [Header("Referências")]
    public Transform bola;
    public Rigidbody rbBola;

    public PosseBola posse;
    public ConducaoBola[] conducoesParaLimpar; // PlayerRoot AreaBola + Companheiros + Oponente AreaBola

    public Transform playerRoot;
    public Transform oponenteRoot;
   
    public Transform spawnBola;
    public Transform spawnPlayer;
    public Transform spawnOponente;

    [Header("HUD")]
    public PlacarHUD hud;
    public string nomeTimeCasa = "CIVRIL";
    public string nomeTimeFora = "MANCHESTER";

    [Header("Config")]
    public float cooldownGol = 1.0f;
    private float ultimoGol = -999f;

    private int golsCasa = 0;
    private int golsFora = 0;

    [Header("Encerrar partida")]
    public MonoBehaviour[] scriptsParaDesativar;
    private bool partidaEncerrada = false;

    private void Start()
    {
        if (hud != null)
        {
            hud.ConfigurarTimes(nomeTimeCasa, nomeTimeFora);
            hud.AtualizarPlacar(golsCasa, golsFora);
            hud.AoTerminar += EncerrarPartida;
            hud.ResetarTempoEIniciar();
        }
    }

    public void RegistrarGol(bool foiGolCasa, string mensagem)
    {
        if (partidaEncerrada) return;

        if (Time.time - ultimoGol < cooldownGol) return;
        ultimoGol = Time.time;

        if (foiGolCasa) golsCasa++;
        else golsFora++;

        if (hud != null) hud.AtualizarPlacar(golsCasa, golsFora);

        Debug.Log(mensagem + " Placar: " + golsCasa + " x " + golsFora);
        StartCoroutine(ResetarPosicoesNoFixed());
    }

    public void EncerrarPartida()
    {
        if (partidaEncerrada) return;
        partidaEncerrada = true;

        Debug.Log("FIM DE JOGO! Placar final: " + golsCasa + " x " + golsFora);

        if (hud != null) hud.Pausar();

        if (scriptsParaDesativar != null)
        {
            foreach (var s in scriptsParaDesativar)
            {
                if (s != null) s.enabled = false;
            }
        }
    }

    private IEnumerator ResetarPosicoesNoFixed()
    {
        // limpa posse (bola fica oficialmente livre)
        if (posse != null) posse.Soltar();

        // limpa conduções (ninguém fica “achando” que ainda está com a bola)
        if (conducoesParaLimpar != null)
        {
            foreach (var c in conducoesParaLimpar)
                if (c != null) c.SoltarBola(0.15f);
        }
        yield return new WaitForFixedUpdate();

        if (rbBola != null)
        {
#if UNITY_6000_0_OR_NEWER
            rbBola.linearVelocity = Vector3.zero;
            rbBola.angularVelocity = Vector3.zero;
#else
            rbBola.velocity = Vector3.zero;
            rbBola.angularVelocity = Vector3.zero;
#endif
            rbBola.position = spawnBola.position;
            rbBola.rotation = spawnBola.rotation;
            rbBola.Sleep();
            rbBola.WakeUp();
        }
        else if (bola != null && spawnBola != null)
        {
            bola.position = spawnBola.position;
            bola.rotation = spawnBola.rotation;
        }

        TeleportarComCharacterController(playerRoot, spawnPlayer);
        TeleportarComCharacterController(oponenteRoot, spawnOponente);
        if (posse != null)
            posse.FixarCameraNoHumano();
    }

    private void TeleportarComCharacterController(Transform alvo, Transform spawn)
    {
        if (alvo == null || spawn == null) return;

        CharacterController cc = alvo.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        alvo.position = spawn.position;
        alvo.rotation = spawn.rotation;

        if (cc != null) cc.enabled = true;
    }
}