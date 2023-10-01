using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text killCountText;
    [SerializeField] private TMP_Text ammoCountText;
    [SerializeField] private GameObject reloadIcon;

    [SerializeField] private int killCount;
    [SerializeField] private int ammoCount;

    private void OnEnable() {
        Enemy.OnEnemyDeath += UpdateKillCount;
        Weapon.OnPlayerShoot += UpdateAmmoCount;
        Weapon.OnPlayerReloadStart += HandleReloadUIUpdate;
    }

    private void OnDisable() {
        Enemy.OnEnemyDeath -= UpdateKillCount;
        Weapon.OnPlayerShoot -= UpdateAmmoCount;
        Weapon.OnPlayerReloadStart -= HandleReloadUIUpdate;
    }

    private void UpdateKillCount(EnemyType enemyType) {
        killCountText.text = $"Kill Count: {++killCount}";
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
