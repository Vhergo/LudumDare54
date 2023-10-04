using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Slider instantHealthBar;
    [SerializeField] private Slider gradualHealthBar;
    [SerializeField] private float gradualDrainRate = 2f;
    [SerializeField] private float gradualDrainDelay = 0.5f;
    private float currentGradualFillValue;

    [SerializeField] private TMP_Text killCountText;
    private int killCount;
    [SerializeField] private TMP_Text ammoCountText;
    private int ammoCount;
    [SerializeField] private GameObject reloadIcon;
    [SerializeField] private TMP_Text waveCountText;
    [SerializeField] private TMP_Text waveTimerText;

    [Header("Game Over UI")]
    [SerializeField] private Image gameOverOverlay;
    [SerializeField] private float gameOverOverlayFadeInRate = 0.05f;
    [SerializeField] private GameObject gameOverDisplay;
    [SerializeField] private TMP_Text gameOverKillCountText;
    [SerializeField] private TMP_Text gameOverHighScoreText;
    [SerializeField] private TMP_Text killedByText;


    private void OnEnable() {
        Player.OnPlayerDeath += HandlePlayerDeath;
        Player.OnPlayerTakeDamage += UpdatePlayerHealthUI;
        Enemy.OnEnemyDeath += UpdateKillCount;
        Weapon.OnAmmoChange += UpdateAmmoCount;
        SpawnManager.OnWaveChange += UpdateWaveCount;
    }

    private void OnDisable() {
        Player.OnPlayerDeath -= HandlePlayerDeath;
        Player.OnPlayerTakeDamage -= UpdatePlayerHealthUI;
        Enemy.OnEnemyDeath -= UpdateKillCount;
        Weapon.OnAmmoChange -= UpdateAmmoCount;
        SpawnManager.OnWaveChange -= UpdateWaveCount;
    }

    private void Start() {
        instantHealthBar.maxValue = Player.Instance.maxHealth;
        gradualHealthBar.maxValue = Player.Instance.maxHealth;
        currentGradualFillValue = gradualHealthBar.maxValue;
        killCount = 0;

        instantHealthBar.value = Player.Instance.maxHealth;
        gradualHealthBar.value = Player.Instance.maxHealth;
    }

    private void Update() {
        waveTimerText.text = $"{(int)SpawnManager.Instance.waveTimer}";
    }

    private void HandlePlayerDeath(EnemyType enemyType) {
        gameOverKillCountText.text = $"Kill Count: {killCount}";
        // gameOverHighScoreText.text = $"High Score: {PlayerPrefs.GetInt("HighScore")}";
        killedByText.text = $"Killed By: {enemyType.ToString()}";
        StartCoroutine(GameOverUI(enemyType));
    }

    private IEnumerator GameOverUI(EnemyType enemyType) {
        while (gameOverOverlay.color.a < 1) {
            gameOverOverlay.color += new Color(0, 0, 0, gameOverOverlayFadeInRate);
            yield return null;
        }
        gameOverDisplay.SetActive(true);
        if (MySceneManager.Instance != null)
            MySceneManager.Instance.PauseGameToggle();
    }

    private void UpdatePlayerHealthUI(float currentHealth) {
        instantHealthBar.value = currentHealth;
        StartCoroutine(GradualHealthDecrease(currentHealth));
    }

    private IEnumerator GradualHealthDecrease(float currentHealth) {
        yield return new WaitForSeconds(gradualDrainDelay);

        while (currentGradualFillValue > currentHealth) {
            currentGradualFillValue -= gradualDrainRate * Time.deltaTime;
            gradualHealthBar.value = currentGradualFillValue;
            yield return null;
        }
        currentGradualFillValue = currentHealth;

    }

    private void UpdateWaveCount(int currentWave) {
        waveCountText.text = $"{currentWave} ";
    }

    private void UpdateKillCount(GameObject enemy, EnemyType enemyType, bool enemyDiedInSafeZone) {
        killCountText.text = $"Kill Count: {++killCount}";
        Debug.Log("Killed: " + enemyType.ToString());
    }

    private void UpdateAmmoCount(int ammoCount) {
        ammoCountText.text = $"Ammo: {ammoCount}";
    }
}
