using System;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public static Weapon Instance { get; private set; }

    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private float bulletSpread;
    [SerializeField] private int bulletsPerShot;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletLifeTime;
    [SerializeField] private float selfKnockbackForce;
    private float fireRateTimer;

    [SerializeField] private int maxAmmo;
    [SerializeField] private int currentAmmo;
    [SerializeField] private float reloadTime;
    private float reloadTimer;
    private bool isReloading;
    private bool playedEmpty;

    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip emptySound;
    [SerializeField] private AudioClip startReloadSound;
    [SerializeField] private AudioClip endReloadSound;
    [SerializeField] private AudioClip loadSound;
    [SerializeField] private float loadRate = 0.2f;
    [SerializeField] private int loadCount = 5;
    private float loadCounter;

    [SerializeField] private GameObject bulletPrefab;
    private Transform gunBarrel;

    public static Action<int> OnAmmoChange;
    public static Action<float> OnReloadStart;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        gunBarrel = GameObject.FindGameObjectWithTag("GunBarrel").transform;

        currentAmmo = maxAmmo;
        fireRateTimer = fireRate;

        OnAmmoChange?.Invoke(currentAmmo);
    }

    private void Update() {
        if (MySceneManager.Instance != null) {
            if (MySceneManager.Instance.isPaused) return;
        }
        HandleInput();
    }

    private void HandleInput() {
        if (Input.GetKey(KeyCode.Mouse0) && CanShoot()) {
            Shoot();
        } else if (Input.GetKeyDown(KeyCode.Mouse0)) {
            PlaySound(emptySound);
        } else {
            fireRateTimer += Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.R)) {
            StartReload();
        }
    }

    private void Shoot() {

        for (int i = 0; i < bulletsPerShot; i++) {
            FireBullet(CalculateSpread(i));
        }

        currentAmmo--;
        fireRateTimer = 0;

        OnAmmoChange?.Invoke(currentAmmo);
        Player.Instance.Knockback(transform.position - gunBarrel.position, selfKnockbackForce);

        PlaySound(shootSound);
    }

    private void FireBullet(Quaternion spreadAngle) {
        GameObject bulletObject = Instantiate(bulletPrefab, gunBarrel.position, spreadAngle);

        Rigidbody2D bulletRb = bulletObject.GetComponent<Rigidbody2D>();
        bulletRb.velocity = bulletObject.transform.right * bulletSpeed;

        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.damage = damage;
        bullet.bulletLifeTime = bulletLifeTime;
    }

    private Quaternion CalculateSpread(int i) {
        if (bulletsPerShot == 1) return gunBarrel.rotation;

        float fireAngle = (-bulletSpread / 2) + (bulletSpread / (bulletsPerShot - 1)) * i;

        Quaternion newRot = Quaternion.Euler(
            gunBarrel.eulerAngles.x,
            gunBarrel.eulerAngles.y,
            gunBarrel.eulerAngles.z + fireAngle);

        return newRot;
    }

    private void StartReload() {
        if (!CanReload()) return;

        Debug.Log("Start Reload");
        OnReloadStart?.Invoke(reloadTime);
        reloadTimer = reloadTime;
        isReloading = true;

        PlaySound(startReloadSound);
        StartCoroutine(Reload());
    }

    private IEnumerator Reload() {
        loadCounter = loadCount;
        yield return new WaitForSeconds(loadRate);
        while (loadCounter > 0) {
            PlaySound(loadSound);
            loadCounter--;
            yield return new WaitForSeconds(loadRate);
        }
        FinishReload();
    }

    private void FinishReload() {
        isReloading = false;
        currentAmmo = maxAmmo;
        OnAmmoChange?.Invoke(currentAmmo);
        PlaySound(endReloadSound);
        Debug.Log("Finish Reload");
    }

    private bool CanShoot() {
        return !Player.Instance.isDead && currentAmmo > 0 && fireRateTimer >= fireRate && !isReloading;
    }

    private bool CanReload() {
        return !Player.Instance.isDead && currentAmmo < maxAmmo && !isReloading;
    }

    private void PlaySound(AudioClip clip) {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound(clip);
    }
}