using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    private static readonly Vector3 startPosition = Vector3.one * -1000000;

    [SerializeField] SpawnerRegistry[] registries;
    [SerializeField] bool initializeOnAwake;

    private Dictionary<string, List<GameObject>> stock = new Dictionary<string, List<GameObject>>();

    private void Awake()
    {
        if (initializeOnAwake)
        {
            FillStock();
        }
    }

    private void Update()
    {
        
    }

    void FillStock()
    {
        foreach (SpawnerRegistry registry in registries)
        {
            List<GameObject> currentRegistry = new List<GameObject>();
            stock.Add(registry.nameId, currentRegistry);
            for(int i = 0; i < registry.startCount; i++)
            {
                currentRegistry.Add(Instantiate(registry.prefab, startPosition, Quaternion.identity));
            }
        }
    }

    GameObject Spawn(string nameId, Vector3? position, Quaternion? rotation, Vector3? scale,  Transform parent)
    {
        if (!stock.ContainsKey(nameId)) return null;
        List<GameObject> subStock = stock[nameId];
        GameObject ret = subStock.FirstOrDefault(obj => !obj.activeSelf);
        if (ret != null)
        {
            Transform retTransform = ret.transform;
            retTransform.SetPositionAndRotation(position ?? Vector3.zero, rotation ?? Quaternion.identity);
            retTransform.localScale = scale ?? Vector3.one;
            retTransform.parent = parent;
            ret.SetActive(true);
            return ret;
        }

        try
        {
            SpawnerRegistry registry = registries.First(reg => reg.nameId == nameId);
            ret = Instantiate(registry.prefab, startPosition, Quaternion.identity);
            subStock.Add(ret);
            Transform retTransform = ret.transform;
            retTransform.SetPositionAndRotation(position ?? Vector3.zero, rotation ?? Quaternion.identity);
            retTransform.localScale = scale ?? Vector3.one;
            retTransform.parent = parent;
            return ret;
        }
        catch
        {
            return null;
        }
    }

    public TComponent Spawn<TComponent>(string nameId, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, Transform parent = null) where TComponent : Component
    {
        GameObject allocObj = Spawn(nameId, position, rotation, scale, parent);
        if (allocObj == null) return null;
        return allocObj.GetComponent<TComponent>();
    }
}
