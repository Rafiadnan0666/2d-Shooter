using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    private Transform target;
    private PlayerScript playerScript;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            playerScript = player.GetComponent<PlayerScript>();
        }
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && playerScript != null)
        {
            playerScript.TakeDamage(10); // Use the public method instead of direct health access
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            GameMaster gameMaster = FindObjectOfType<GameMaster>();
            if (gameMaster != null)
            {
                gameMaster.Score += 10;
            }
            Destroy(gameObject);
        }
    }
}