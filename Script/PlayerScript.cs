using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerScript : MonoBehaviour
{
    [Header("Movement")]
    public float rotationSpeed = 10f;

    [Header("Combat")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;
    public int maxHealth = 100;
    public float knockbackForce = 0.5f;
    [SerializeField] private int currentHealth;

    [Header("Camera Effects")]
    public Camera playerCamera;
    public float shootShakeDuration = 0.1f;
    public float shootShakeMagnitude = 0.05f;
    public float damageShakeDuration = 0.3f;
    public float damageShakeMagnitude = 0.2f;

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip damageSound;
    public AudioClip lowHealthSound;

    private float nextFireTime = 0f;
    private AudioSource audioSource;
    private Vector3 originalCameraPosition;
    private bool isCameraShaking = false;
    private float shakeDurationRemaining = 0f;
    private float currentShakeMagnitude = 0f;

    public int health => currentHealth;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;

        if (playerCamera != null)
        {
            originalCameraPosition = playerCamera.transform.localPosition;
        }
        else
        {
            Debug.LogWarning("Player camera not assigned in inspector - attempting to find main camera");
            playerCamera = Camera.main;
            if (playerCamera != null)
            {
                originalCameraPosition = playerCamera.transform.localPosition;
            }
        }
    }

    void Update()
    {
        RotateToMouse();
        HandleShooting();
        UpdateCameraShake();
    }
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
    void RotateToMouse()
    {
        if (playerCamera == null) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = playerCamera.nearClipPlane;
        Vector3 worldPos = playerCamera.ScreenToWorldPoint(mousePos);

        Vector2 direction = (worldPos - transform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void HandleShooting()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            TriggerShake(shootShakeDuration, shootShakeMagnitude);

            if (shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }

            ApplyKnockback(knockbackForce);
        }
    }

    void ApplyKnockback(float force)
    {
        if (firePoint != null)
        {
            Vector2 knockbackDirection = -firePoint.up;
            transform.Translate(knockbackDirection * force * Time.deltaTime, Space.World);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);

        TriggerShake(damageShakeDuration, damageShakeMagnitude);

        if (damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        if (currentHealth <= maxHealth * 0.3f && lowHealthSound != null)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(lowHealthSound);
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player has died!");
        // Add death handling here
    }

    void TriggerShake(float duration, float magnitude)
    {
        if (playerCamera == null) return;

        if (isCameraShaking)
        {
            currentShakeMagnitude = Mathf.Max(currentShakeMagnitude, magnitude);
            shakeDurationRemaining = Mathf.Max(shakeDurationRemaining, duration);
        }
        else
        {
            currentShakeMagnitude = magnitude;
            shakeDurationRemaining = duration;
            isCameraShaking = true;
        }
    }

    void UpdateCameraShake()
    {
        if (playerCamera == null || !isCameraShaking) return;

        if (shakeDurationRemaining > 0)
        {
            float x = Mathf.PerlinNoise(Time.time * 30f, 0f) * 2f - 1f;
            float y = Mathf.PerlinNoise(0f, Time.time * 30f) * 2f - 1f;

            Vector3 shakeOffset = new Vector3(x, y, 0) * currentShakeMagnitude;
            playerCamera.transform.localPosition = originalCameraPosition + shakeOffset;

            shakeDurationRemaining -= Time.deltaTime;
            currentShakeMagnitude = Mathf.Lerp(0f, currentShakeMagnitude, shakeDurationRemaining / 0.5f);
        }
        else
        {
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                originalCameraPosition,
                Time.deltaTime * 10f);

            if (Vector3.Distance(playerCamera.transform.localPosition, originalCameraPosition) < 0.001f)
            {
                playerCamera.transform.localPosition = originalCameraPosition;
                isCameraShaking = false;
            }
        }
    }
}