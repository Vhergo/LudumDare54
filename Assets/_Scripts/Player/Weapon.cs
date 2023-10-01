using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public static Weapon Instance { get; private set; }

    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletLifeTime;
    private float fireRateTimer;

    [SerializeField] private int maxAmmo;
    [SerializeField] private int currentAmmo;
    [SerializeField] private float reloadTime;
    private float reloadTimer;
    private bool isReloading;

    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioClip emptySound;

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
        HandleInput();
    }

    private void HandleInput() {
        if (Input.GetKey(KeyCode.Mouse0) && CanShoot()) {
            Shoot();
        } else {
            fireRateTimer += Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.R)) {
            StartReload();
        }

        Reload();
    }

    private void Shoot() {
        currentAmmo--;
        fireRateTimer = 0;

        OnAmmoChange?.Invoke(currentAmmo);
        FireBullet();

        // SoundManager.Instance.PlaySound(shootSound);
    }

    private void FireBullet() {
        GameObject bulletObject = Instantiate(bulletPrefab, gunBarrel.position, gunBarrel.rotation);

        Rigidbody2D bulletRb = bulletObject.GetComponent<Rigidbody2D>();
        bulletRb.velocity = gunBarrel.right * bulletSpeed;

        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.damage = damage;
        bullet.bulletLifeTime = bulletLifeTime;
    }

    private void StartReload() {
        if (!CanReload()) return;

        Debug.Log("Start Reload");
        OnReloadStart?.Invoke(reloadTime);
        reloadTimer = reloadTime;
        isReloading = true;

        // SoundManager.Instance.PlaySound(reloadSound);
    }

    private void Reload() {
        if (isReloading) {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0) {
                FinishReload();
            }
        }
    }

    private void FinishReload() {
        isReloading = false;
        currentAmmo = maxAmmo;
        Debug.Log("Finish Reload");
    }

    private bool CanShoot() {
        return !Player.Instance.isDead && currentAmmo > 0 && fireRateTimer >= fireRate && !isReloading;
    }

    private bool CanReload() {
        return !Player.Instance.isDead && currentAmmo < maxAmmo && !isReloading;
    }
}