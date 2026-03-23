using System;
using UnityEngine;

public class TransformSpectrogram : MonoBehaviour
{
    public enum LightAxis { X = 0, Y = 1, Z = 2 }

    [SerializeField] private BasicSpectrogramData spectrogramData;
    [SerializeField] private Transform[] transforms;
    [SerializeField] private LightAxis axis = LightAxis.Y;
    [SerializeField] private float minPosition;
    [SerializeField] private float maxPosition;
    [SerializeField] private bool scaleSamples = true;
    [SerializeField] private float scale = 1f;

    private Vector3 direction;
    private Vector3[] defaultPositions;

    void Awake()
    {
        if (spectrogramData == null)
            spectrogramData = FindObjectOfType<BasicSpectrogramData>();

        if (spectrogramData == null)
        {
            Debug.LogError("BasicSpectrogramData not found in scene!");
            return;
        }

        Vector3 a = Vector3.zero;
        switch (axis)
        {
            case LightAxis.X:
                direction = new Vector3(1f, 0f, 0f);
                a = new Vector3(0f, 1f, 1f);
                break;
            case LightAxis.Y:
                direction = new Vector3(0f, 1f, 0f);
                a = new Vector3(1f, 0f, 1f);
                break;
            case LightAxis.Z:
                direction = new Vector3(0f, 0f, 1f);
                a = new Vector3(1f, 1f, 0f);
                break;
        }

        defaultPositions = new Vector3[transforms.Length];
        for (int i = 0; i < transforms.Length; i++)
            defaultPositions[i] = Vector3.Scale(a, transforms[i].localPosition);
    }

    void Update()
    {
        if (spectrogramData == null) return;

        for (int i = 0; i < transforms.Length; i++)
        {
            int num = scaleSamples
                ? Mathf.RoundToInt((float)i / ((float)transforms.Length - 1f) * 63f * scale)
                : i;

            float t = spectrogramData.ProcessedSamples[num % 64];
            transforms[i].localPosition = defaultPositions[i] + direction * Mathf.Lerp(minPosition, maxPosition, t);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Populate Transforms From Children")]
    private void PopulateTransformsFromChildren()
    {
        transforms = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            transforms[i] = transform.GetChild(i);

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"Populated {transforms.Length} transforms.");
    }
#endif
}