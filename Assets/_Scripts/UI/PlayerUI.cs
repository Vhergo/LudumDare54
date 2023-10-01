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


    private void OnEnable() {
        Player.OnPlayerTakeDamage += UpdatePlayerHealthUI;
        Enemy.OnEnemyDeath += UpdateKillCount;
        Weapon.OnAmmoChange += UpdateAmmoCount;
        Weapon.OnReloadStart += HandleReloadUIUpdate;
    }

    private void OnDisable() {
        Enemy.OnEnemyDeath -= UpdateKillCount;
        Weapon.OnAmmoChange -= UpdateAmmoCount;
        Weapon.OnReloadStart -= HandleReloadUIUpdate;
    }

    private void Start() {
        instantHealthBar.maxValue = Player.Instance.maxHealth;
        gradualHealthBar.maxValue = Player.Instance.maxHealth;
        currentGradualFillValue = gradualHealthBar.maxValue;
        killCount = 0;
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

    private void UpdateKillCount(EnemyType enemyType, bool enemyDiedInSafeZone) {
        killCountText.text = $"Kill Count: {killCount}";
        Debug.Log("Killed: " + enemyType.ToString());
    }

    private void UpdateAmmoCount(int ammoCount) {
        ammoCountText.text = $"Ammo: {ammoCount}";
    }

    private IEnumerator UpdateReloadUI(float reloadTime) {
        reloadIcon.SetActive(true);
        yield return new WaitForSeconds(reloadTime);
        reloadIcon.SetActive(false);
    }

    private void HandleReloadUIUpdate(float reloadTime) {
        StartCoroutine(UpdateReloadUI(reloadTime));
    }
}
