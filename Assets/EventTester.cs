using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTester : MonoBehaviour
{
    public void TestDualVector(Vector3 a, Vector3 b)
    {
        Debug.Log($"a: {a} || b: {b}");       
    }

    public void TestEmpty()
    {
        Debug.Log("Event Called!");
    }

    public void TestVectorBoolean(Vector3 a, bool b)
    {
        Debug.Log($"a: {a} || b: {b}");
    }
}
