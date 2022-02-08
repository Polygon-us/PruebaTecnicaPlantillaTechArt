using UnityEngine;
using System;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(ObjectSpawner))]
public class Enemy : MonoBehaviour
{
#if UNITY_EDITOR
    private static readonly Color detectionIndicatorColor = new Color(1f, 1f, 1f, 0.2f);
#endif

    [Flags]
    enum AIState
    {
        Patroling = 2,
        Attacking = 4
    }

    private static readonly Vector3 center = Vector3.zero;
    private static readonly Vector3 axis = Vector3.up;

    [SerializeField] Transform muzzle = null;
    [Header("Stats")]
    [SerializeField] float lifeTime = 5;
    [SerializeField] float movementSpeed = 5;
    [Header("Enemy Detection")]
    [SerializeField] float detectionCooldown = 0.5f;
    [SerializeField] float detectionRadius = 5;
    [SerializeField] LayerMask detectionMask;
    [Header("Attack")]
    [SerializeField] float fireCooldown = 0.2f;
    [SerializeField] float fireForce = 10;

    [Header("Callbacks")]
    [SerializeField][Tooltip("Called every frame in which the enemy is locked on the player")] DoubleVectorEvent onAim;
    [SerializeField][Tooltip("Called once the enemy stopped locking on the player")] UnityEvent onStopAiming;


    private ObjectSpawner objectSpawner;

    private AIState aiState = AIState.Patroling;
    private float detectionTimer = 0;
    private float fireTimer = 0;

    private Collider[] detectableColliders = new Collider[1];
    private Transform firingTarget;

    Vector3 lastPos;


    private void Awake()
    {
        Invoke(nameof(Deallocate), lifeTime);
        objectSpawner = GetComponent<ObjectSpawner>();
        lastPos = transform.position;
    }

    private void Update()
    {
        if(aiState == (aiState | AIState.Patroling))
        {
            Patrol();
            if (detectionTimer > detectionCooldown)
            {
                Detect();
                detectionTimer = 0f;
            }
            detectionTimer += Time.deltaTime;
        }
        if(aiState == (aiState | AIState.Attacking))
        {
            Aim();
            if (fireTimer > fireCooldown)
            {
                Fire();
                fireTimer = 0f;
            }
            fireTimer += Time.deltaTime;
        }
    }

    private void Patrol()
    {
        //Movement behaviour
        transform.RotateAround(center, axis, movementSpeed * Time.deltaTime);
        //Debug.DrawLine(transform.position, transform.position + (transform.position - lastPos).normalized * 10, Color.cyan, 0.1f);
        transform.forward = (transform.position - lastPos).normalized;
        lastPos = transform.position;
    }

    private void Detect()
    {
        int colliderCount = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, detectableColliders, detectionMask);
        if(colliderCount == 0) 
        {
            if(firingTarget != null)
            {
                aiState &= ~AIState.Attacking;
                aiState |= AIState.Patroling;
                onStopAiming?.Invoke();
            }
            firingTarget = null;
        }
        else if (colliderCount > 0)
        {
            firingTarget = detectableColliders[0].transform;
            aiState |= AIState.Attacking;
        }
        else if (colliderCount > 1)
        {
            Debug.LogWarning("More than one player is being detected, check layer configuration in scene hierarchy");
        }
    }

    private void Aim()
    {
        if (firingTarget == null) return;
        transform.LookAt(firingTarget);
        onAim?.Invoke(muzzle.position, firingTarget.position);
    }

    private void Fire()
    {
        if (firingTarget == null) return;
        Proyectile proyectile = objectSpawner.Spawn<Proyectile>("Proyectile", muzzle.position);
        proyectile.Stop();
        if(proyectile != null)
        {
            proyectile.Eject((firingTarget.position - muzzle.position).normalized * fireForce, true);
        }
    }

    private void Deallocate()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        aiState = AIState.Patroling;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        GUIStyle detectionIndicatorStyle = new GUIStyle { normal = new GUIStyleState { textColor = detectionIndicatorColor } };
        Gizmos.color = detectionIndicatorColor;
        Handles.color = detectionIndicatorColor;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Handles.Label(transform.position + transform.up * detectionRadius, "Detection Radius", detectionIndicatorStyle);
        Handles.Label(transform.position + transform.forward * detectionRadius, "Detection Radius", detectionIndicatorStyle);
        Handles.Label(transform.position + transform.right * detectionRadius, "Detection Radius", detectionIndicatorStyle);
        Handles.Label(transform.position - transform.up * detectionRadius, "Detection Radius", detectionIndicatorStyle);
        Handles.Label(transform.position - transform.forward * detectionRadius, "Detection Radius", detectionIndicatorStyle);
        Handles.Label(transform.position - transform.right * detectionRadius, "Detection Radius", detectionIndicatorStyle);
    }
#endif
}
