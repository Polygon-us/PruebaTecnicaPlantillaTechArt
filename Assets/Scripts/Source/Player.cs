using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour, IDamageable<Damage>
{
    [SerializeField] float movementSpeed;
    [SerializeField] UnityEvent onDamaged;

    Vector3 spawnPoint;
    Camera mainCamera;

    private void Awake()
    {
        spawnPoint = transform.position;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 forward = Vector3.ProjectOnPlane(mainCamera.transform.forward, transform.up);
        Vector3 right = Vector3.ProjectOnPlane(mainCamera.transform.right, transform.up);
        float scaledSpeed = movementSpeed * Time.deltaTime;
        transform.Translate(forward * Input.GetAxis("Vertical") * scaledSpeed + right * Input.GetAxis("Horizontal") * scaledSpeed);
    }

    public void Die()
    {
        transform.position = spawnPoint;
    }

    public void TakeDamage(Damage damage)
    {
        onDamaged?.Invoke();
    }
}
