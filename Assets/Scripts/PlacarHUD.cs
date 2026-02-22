using UnityEngine;
using TMPro;

public class PlacarHUD : MonoBehaviour
{
    [Header("Textos (TMP)")]
    public TextMeshProUGUI txtTempo;
    public TextMeshProUGUI txtTimeCasa;
    public TextMeshProUGUI txtTimeFora;
    public TextMeshProUGUI txtPlacar;

    [Header("Tempo de jogo")]
    public float duracaoMinutos = 90f;

    // Para não ficar 90 minutos reais:
    // 1.0 = 1 segundo real vira 1 minuto de jogo (90s reais = 90min de jogo)
    public float minutosPorSegundo = 1f;

    private float tempoMinAtual = 0f;
    private bool rodando = false;
    private bool encerrado = false;

    public System.Action AoTerminar;

    public void ConfigurarTimes(string casa, string fora)
    {
        if (txtTimeCasa != null) txtTimeCasa.text = casa;
        if (txtTimeFora != null) txtTimeFora.text = fora;
    }

    public void AtualizarPlacar(int golsCasa, int golsFora)
    {
        if (txtPlacar != null)
            txtPlacar.text = golsCasa + " x " + golsFora;
    }

    public void ResetarTempoEIniciar()
    {
        tempoMinAtual = 0f;
        encerrado = false;
        rodando = true;
        AtualizarTempoUI();
    }

    public void Pausar()
    {
        rodando = false;
    }

    private void Update()
    {
        if (!rodando || encerrado) return;

        tempoMinAtual += Time.deltaTime * minutosPorSegundo;

        if (tempoMinAtual >= duracaoMinutos)
        {
            tempoMinAtual = duracaoMinutos;
            encerrado = true;
            rodando = false;
            AtualizarTempoUI();
            AoTerminar?.Invoke();
            return;
        }

        AtualizarTempoUI();
    }

    private void AtualizarTempoUI()
    {
        if (txtTempo == null) return;

        int totalSeg = Mathf.RoundToInt(tempoMinAtual * 60f);
        int min = totalSeg / 60;
        int seg = totalSeg % 60;

        txtTempo.text = min.ToString("00") + ":" + seg.ToString("00");
    }
}