using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private SceneChanger manager;

    [Header("Tiempo")]
    public float duration = 120f; // 2 minutes

    [Header("UI (optional)")]
    public TextMeshProUGUI timerText;

    private float timeRemaining;
    private bool isRunning = false;

    void Start()
    {
        timeRemaining = duration;
        UpdateUI();
    }

    void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isRunning = false;

            OnTimerEnd();
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";

        if (timeRemaining <= 30f)
        {
            timerText.color = Color.red;
        }
    }

    void OnTimerEnd()
    {
        Debug.Log("Tiempo terminado");

        manager.ChangeScene(9);
    }

    public void ResetTimer()
    {
        timeRemaining = duration;
        isRunning = false;
        UpdateUI();
    }

    public void StartTimer()
    {
        isRunning = true;
    }
}