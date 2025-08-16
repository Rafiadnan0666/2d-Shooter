using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Movement and targeting variables
    public float speed = 2f;
    private Transform target;
    private PlayerScript playerScript;
    public GameObject SkinPrefab;

    // Audio and effect variables
    public AudioClip explosionSound;
    private AudioSource audioSource;
    public AudioClip WalkSound;
    private bool isDying = false;

    void Start()
    {
 
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            playerScript = player.GetComponent<PlayerScript>();
        }

     
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {

        if (target != null && !isDying)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check for player collision
        if (collision.gameObject.CompareTag("Player") && playerScript != null)
        {
            playerScript.TakeDamage(10);
            Destroy(gameObject);
        }

        // Check for bullet collision
        if (collision.gameObject.CompareTag("Bullet") && !isDying)
        {
            // Start the dissolve process and prevent further updates
            isDying = true;

        
            GameMaster gameMaster = FindObjectOfType<GameMaster>();
            if (gameMaster != null)
            {
                gameMaster.Score += 10;
            }

            if (explosionSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(explosionSound);
            }

       
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

          
            StartCoroutine(FadeAndDestroy(10.0f)); 
        }
    }

    IEnumerator FadeAndDestroy(float fadeDuration)
    {
        SpriteRenderer spriteRenderer = SkinPrefab.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SkinPrefab does not have a SpriteRenderer component.");
            Destroy(gameObject);
            yield break;
        }

        Color startColor = spriteRenderer.color;
        float startTime = Time.time;

        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;

            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1, 0, t));
            yield return null; 
        }

      
        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0);

       
        Destroy(gameObject,100);
    }
}