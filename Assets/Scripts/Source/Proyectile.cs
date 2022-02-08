using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Proyectile : MonoBehaviour
{
    [SerializeField] VectorAndBooleanEvent onCollide;
    [SerializeField] float lifetime;
    [SerializeField] Damage damage;

    private Rigidbody rb;
    private float lifeTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        lifeTimer = 0f;
    }

    private void Update()
    {
        if(lifeTimer > lifetime)
        {
            lifeTimer = 0f;
            gameObject.SetActive(false);
        }
        lifeTimer += Time.deltaTime;
    }

    public void Stop()
    {
        rb.velocity = Vector3.zero;
    }

    public void Eject(Vector3 force, bool faceForce)
    {
        if (faceForce)
        {
            transform.forward = force.normalized;
        }
        rb.AddForce(force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable<Damage> dmg))
        {
            onCollide.Invoke(collision.GetContact(0).normal, true);
            dmg.TakeDamage(damage);
        }
        else
        {
            onCollide.Invoke(collision.GetContact(0).normal, false);
        }

        gameObject.SetActive(false);
    }
}
