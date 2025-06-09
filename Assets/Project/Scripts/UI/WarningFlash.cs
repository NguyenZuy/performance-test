using TriInspector;
using UnityEngine;
using UnityEngine.UI;
using ZuyZuy.PT.Manager;

public class WarningFlash : MonoBehaviour
{
    public Image redOverlay; // Assign a full-screen UI Image in inspector
    public float flashSpeed = 2f;
    public float maxIntensity = 0.7f;
    public float flashDuration = 0.5f; // How long the flash effect lasts

    private float flashTimer = 0f;
    private bool isFlashing = false;

    void Start()
    {
        GameManager.Instance.OnPlayerHPChanged += OnTriggerFlash;
    }

    private void Update()
    {
        if (isFlashing)
        {
            flashTimer += Time.deltaTime;

            if (flashTimer >= flashDuration)
            {
                isFlashing = false;
                redOverlay.color = new Color(1, 0, 0, 0);
            }
            else
            {
                // Create a single flash effect that fades out
                float alpha = maxIntensity * (1 - (flashTimer / flashDuration));
                redOverlay.color = new Color(1, 0, 0, alpha);
            }
        }
    }

    public void OnTriggerFlash(int currentHP)
    {
        if (currentHP < GameManager.Instance.MaxPlayerHP)
        {
            TriggerFlash();
        }
    }

    public void TriggerFlash()
    {
        isFlashing = true;
        flashTimer = 0f;
    }

    [Button("Test Flash")]
    public void TestFlash()
    {
        TriggerFlash();
    }
}