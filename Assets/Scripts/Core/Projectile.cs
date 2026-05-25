using UnityEngine;

public class Projectile : MonoBehaviour
{
    private AttackData attackData;
    private GameObject owner;
    private Vector2 velocity;
    private float lifetime = 3f;

    public void Launch(AttackData data, GameObject ownerObject, Vector2 dir, float speed)
    {
        attackData = data;
        owner = ownerObject;
        velocity = dir * speed;
        Destroy(gameObject, lifetime);

        var hb = GetComponent<Hitbox>();
        if (hb != null) hb.Initialize(data, ownerObject);
    }

    private void Update()
    {
        transform.Translate(velocity * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == owner) return;
        
        FighterHealth health = other.GetComponent<FighterHealth>();
        if (health != null)
        {
            health.TakeHit(attackData, transform.position);
            Destroy(gameObject);
        }
    }
}
