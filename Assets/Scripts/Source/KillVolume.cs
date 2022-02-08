using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class KillVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDamageable dmg))
        {
            dmg.Die();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.8f, 0.1f, 0.1f, 0.2f);
        Gizmos.DrawCube(transform.position, transform.lossyScale);
    }
#endif
}
