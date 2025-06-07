using UnityEngine;

public class RainEffectController : MonoBehaviour
{
    public Material rainMaterial;
    public float rainSpeed = 1f;
    public float rainIntensity = 0.5f;
    public Color rainColor = Color.white;

    private void Start()
    {
        if (rainMaterial != null)
        {
            rainMaterial.SetFloat("_RainSpeed", rainSpeed);
            rainMaterial.SetFloat("_RainIntensity", rainIntensity);
            rainMaterial.SetColor("_RainColor", rainColor);
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (rainMaterial != null)
        {
            Graphics.Blit(source, destination, rainMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
} 