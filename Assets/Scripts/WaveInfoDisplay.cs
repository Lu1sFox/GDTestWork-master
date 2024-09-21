using UnityEngine;
using UnityEngine.UI;

public class WaveInfoDisplay : MonoBehaviour
{
    [SerializeField] private LevelConfig levelConfig;
    [SerializeField] private Text totatWaveText;  
    [SerializeField] private Text currentWaveText;  

    private void Start()
    {
        UpdateWaveInfo(); 
    }
    private void UpdateWaveInfo()
    {
        if (levelConfig != null)
        {
            int totalWaves = levelConfig.Waves.Length;
            totatWaveText.text = $"Total Waves: {totalWaves}"; 
            UpdateCurrentWaveText(); 
        }
        else
        {
            totatWaveText.text = "Total Waves: 0";
            currentWaveText.text = "Current Wave: N/A";
        }
    }
    private void UpdateCurrentWaveText()
    {
        if (SceneManager.Instance != null)
        {
            int currentWave = SceneManager.Instance.GetCurrentWaveIndex(); 
            currentWaveText.text = $"Current Wave: {currentWave}";
        }
        else
        {
            currentWaveText.text = "Current Wave: N/A";
        }
    }
    private void Update()
    {
        UpdateCurrentWaveText(); 
    }
}
