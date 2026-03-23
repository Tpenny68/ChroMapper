using System.Collections.Generic;
using UnityEngine;

public class BasicSpectrogramData : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool isDeterministic = false;
    [SerializeField] private float instantChangeThreshold = 0.1f;

    public const int kNumberOfSamples = 64;
    private bool hasData;
    private bool hasProcessedData;
    private float[] samples = new float[64];
    private List<float> processedSamples = new List<float>(64);

    public float[] Samples
    {
        get
        {
            if (!hasData && audioSource != null)
            {
                if (isDeterministic)
                    GetFakeSamples(samples);
                else
                    audioSource.GetSpectrumData(samples, 0, (FFTWindow) 5);
                hasData = true;
            }
            return samples;
        }
    }

    public List<float> ProcessedSamples
    {
        get
        {
            if (!hasProcessedData)
            {
                ProcessSamples(Samples, processedSamples);
                hasProcessedData = true;
            }
            return processedSamples;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        hasData = false;
        hasProcessedData = false;
        for (int i = 0; i<64; i++)
        {
            processedSamples.Add(0f);
        }
    }

    void LateUpdate()
    {
        hasData = false;
        hasProcessedData = false;
    }

    private void ProcessSamples(float[] sourceSamples, List<float> processedSamples)
    {
        float deltaTime = Time.deltaTime;
        for (int i = 0; i<sourceSamples.Length; i++)
        {
            float num = Mathf.Log(sourceSamples[i] + 1f) * (float)(i + 1);
            if (processedSamples[i] < num)
                if (num - processedSamples[i] > instantChangeThreshold)
                    processedSamples[i] = num;
                else
                    processedSamples[i] = Mathf.Lerp(processedSamples[i], num, deltaTime * 8f);
            else
                processedSamples[i] = Mathf.Lerp(processedSamples[i], num, deltaTime * 4f);
        }
    }
    private void GetFakeSamples(float[] samples)
    {
        float num = 3f;
        float num2 = 1f;
        for (int i = 0; i< samples.Length; i++)
        {
            samples[i] = 0.1f;
            samples[i] *= Mathf.PerlinNoise((float)i / (float)samples.Length * num, audioSource.time * num2);
            float num3 = Mathf.Sin(audioSource.time * 10000f);
            samples[i] *= (num3 + 1f) / 2f;
        }
    }    
}
