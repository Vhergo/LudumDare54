using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject emptybulletPrefab;
    [SerializeField] private Transform bulletBar;
    [SerializeField] private Transform emptyBulletBar;
    private int maxAmmo;

    private List<Image> bulletList = new List<Image>();
    private List<Image> emptyBulletList = new List<Image>();

    private void OnEnable() {
        Weapon.OnAmmoChange += UpdateAmmoCount;
    }

    private void OnDisable() {
        Weapon.OnAmmoChange -= UpdateAmmoCount;
    }

    private void Start() {
        maxAmmo = Weapon.Instance.maxAmmo;
        SpawnBulletIcons(emptyBulletList, emptyBulletBar, emptybulletPrefab);
        SpawnBulletIcons(bulletList, bulletBar, bulletPrefab);
    }

    private void SpawnBulletIcons(List<Image> prefabList, Transform bar, GameObject prefab) {
        for (int i = 0; i < maxAmmo; i++) {
            GameObject ammo = Instantiate(prefab, bar);
            prefabList.Add(ammo.GetComponent<Image>());
        }
    }

    private void UpdateAmmoCount(int ammoCount) {
        Debug.Log("Ammo Count: " + ammoCount);
        for (int i = 0; i < bulletList.Count; i++) {
            if (i < ammoCount) {
                bulletList[i].enabled = true;
            } else {
                bulletList[i].enabled = false;
            }
        }
    }
}
