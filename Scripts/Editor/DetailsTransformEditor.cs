using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Transform))]
public class DetailsTransformEditor : Editor
{
    private Transform _transform;
    private MockPlanet _guide;

    private bool IsDetail()
    {
        return _transform.gameObject.CompareTag("Detail");
    }

    private void OnEnable()
    {
        _transform = (Transform) target;
        _guide = _transform.root.gameObject.GetComponentInChildren<MockPlanet>();
    }

    private void OnSceneGUI()
    {
        if (IsDetail() && _guide != null)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 oldPosition = _transform.position;
            Vector3 newPosition = Handles.PositionHandle(oldPosition, _transform.rotation);
            Tools.hidden = Selection.Contains(_transform.gameObject) && Tools.current == Tool.Move;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_transform, "Move " + _transform.gameObject.name);
            }
            Vector3 guidePosition = _guide.transform.position;
            float yOffset = _transform.GetComponent<Collider>().bounds.size.y / 2;
            Vector3 newCorrectedPosition = (newPosition - guidePosition).normalized * (_guide.planetSize + yOffset);
            _transform.position = newCorrectedPosition;
            _transform.up = newCorrectedPosition.normalized;
        }
    }

    public override void OnInspectorGUI()
    {
        if (IsDetail())
        {
            DetailTransformInspector();
        }
        else
        {
            StandardTransformInspector();
        }
    }

    private void DetailTransformInspector()
    {
        if (_guide == null)
        {
            GUILayout.Label("The planet guide could not be found!");
        }
        else
        {
            GUILayout.Label("Detail mode is on, use the handles to edit position.");
        }
    }

    private void StandardTransformInspector()
    {
        bool didPositionChange = false;
        bool didRotationChange = false;
        bool didScaleChange = false;

        Vector3 initialLocalPosition = _transform.localPosition;
        Vector3 initialLocalEuler = _transform.localEulerAngles;
        Vector3 initialLocalScale = _transform.localScale;

        EditorGUI.BeginChangeCheck();
        Vector3 localPosition = EditorGUILayout.Vector3Field("Position", _transform.localPosition);
        if (EditorGUI.EndChangeCheck())
            didPositionChange = true;

        EditorGUI.BeginChangeCheck();
        Vector3 localEulerAngles = EditorGUILayout.Vector3Field("Euler Rotation", _transform.localEulerAngles);
        if (EditorGUI.EndChangeCheck())
            didRotationChange = true;

        EditorGUI.BeginChangeCheck();
        Vector3 localScale = EditorGUILayout.Vector3Field("Scale", _transform.localScale);
        if (EditorGUI.EndChangeCheck())
            didScaleChange = true;

        // Apply changes with record undo
        if (didPositionChange || didRotationChange || didScaleChange)
        {
            Undo.RecordObject(_transform, _transform.name);

            if (didPositionChange)
                _transform.localPosition = localPosition;

            if (didRotationChange)
                _transform.localEulerAngles = localEulerAngles;

            if (didScaleChange)
                _transform.localScale = localScale;
        }

        Transform[] selectedTransforms = Selection.transforms;
        if (selectedTransforms.Length > 1)
        {
            foreach (var item in selectedTransforms)
            {
                if (didPositionChange || didRotationChange || didScaleChange)
                    Undo.RecordObject(item, item.name);

                if (didPositionChange)
                {
                    item.localPosition = ApplyChangesOnly(
                        item.localPosition, initialLocalPosition, _transform.localPosition);
                }

                if (didRotationChange)
                {
                    item.localEulerAngles = ApplyChangesOnly(
                        item.localEulerAngles, initialLocalEuler, _transform.localEulerAngles);
                }

                if (didScaleChange)
                {
                    item.localScale = ApplyChangesOnly(
                        item.localScale, initialLocalScale, _transform.localScale);
                }
            }
        }
    }

    private Vector3 ApplyChangesOnly(Vector3 toApply, Vector3 initial, Vector3 changed)
    {
        if (!Mathf.Approximately(initial.x, changed.x))
            toApply.x = _transform.localPosition.x;

        if (!Mathf.Approximately(initial.y, changed.y))
            toApply.y = _transform.localPosition.y;

        if (!Mathf.Approximately(initial.z, changed.z))
            toApply.z = _transform.localPosition.z;

        return toApply;
    }
}