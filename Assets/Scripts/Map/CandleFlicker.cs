using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CandleFlicker : MonoBehaviour
{
    public Light2D candleLight;          // Reference to the 2D light component
    public float minIntensity = 0.5f;    // Minimum intensity of the light
    public float maxIntensity = 1.5f;    // Maximum intensity of the light
    public float minRange = 3.5f;        // Minimum range of the light
    public float maxRange = 4.0f;        // Maximum range of the light
    public float flickerSpeed = 1.3f;    // How quickly the light flickers
    public Color colorA = Color.yellow;  // First color for flickering
    public Color colorB = Color.red;     // Second color for flickering

    private float targetIntensity;       // Target intensity for smooth transitions
    private float targetRange;           // Target range for smooth transitions
    private Color targetColor;           // Target color for smooth transitions

    void Start()
    {
        if (candleLight == null)
        {
            candleLight = GetComponent<Light2D>();  // Automatically find the Light2D component if not assigned
        }

        // Initialize target values
        targetIntensity = candleLight.intensity;
        targetRange = candleLight.pointLightOuterRadius;
        targetColor = candleLight.color;
    }

    void Update()
    {
        // Randomly set new target intensity, range, and color within the defined limits
        if (Random.Range(0f, 1f) < 0.1f)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
            targetRange = Random.Range(minRange, maxRange);
            targetColor = Color.Lerp(colorA, colorB, Random.Range(0f, 1f));
        }

        // Smoothly transition the light's intensity, range, and color to the target values
        candleLight.intensity = Mathf.Lerp(candleLight.intensity, targetIntensity, Time.deltaTime * flickerSpeed);
        candleLight.pointLightOuterRadius = Mathf.Lerp(candleLight.pointLightOuterRadius, targetRange, Time.deltaTime * flickerSpeed);
        candleLight.color = Color.Lerp(candleLight.color, targetColor, Time.deltaTime * flickerSpeed);
    }
}
