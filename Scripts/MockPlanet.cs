using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class MockPlanet : MonoBehaviour
{
    [SerializeField] private Mesh planetMesh;
    [SerializeField] private Color planetBaseColor = Color.gray;
    [SerializeField] private Color northPoleColor = Color.red;
    [SerializeField] private Color southPoleColor = Color.blue;
    [SerializeField] public float planetSize = 100f;
    private void OnDrawGizmos()
    {
        Vector3 planetPosition = transform.position;
        if (planetMesh != null)
        {
            Gizmos.color = planetBaseColor;
            Gizmos.DrawMesh(planetMesh, Vector3.zero, Quaternion.identity, Vector3.one * planetSize * 2);
        }
        Gizmos.color = northPoleColor;
        Vector3 poleOffset = Vector3.up * (planetSize + 1 * (planetSize / 2));
        Vector3 poleOriginOffset = Vector3.up * planetSize;
        Gizmos.DrawLine(planetPosition + poleOriginOffset, planetPosition + poleOffset);
        Gizmos.color = southPoleColor;
        Gizmos.DrawLine(planetPosition - poleOriginOffset, planetPosition - poleOffset);
    }

    private void Update()
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one * planetSize; 
    }
}