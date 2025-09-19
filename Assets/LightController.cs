using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightController : MonoBehaviour
{
    [Header("Fluctuation Settings")]
    public float minIntensity = 0.8f;   // Lowest light intensity
    public float maxIntensity = 1.2f;   // Highest light intensity
    public float period = 2f;           // Seconds for a full cycle

    private Light2D _light;

    void Awake()
    {
        _light = GetComponent<Light2D>();
    }

    void Update()
    {
        // Calculate a 0–1 oscillation using sine wave
        float t = (Mathf.Sin(Time.time * (2f * Mathf.PI / period)) + 1f) / 2f;

        // Lerp between min and max based on t
        _light.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
    }
}
