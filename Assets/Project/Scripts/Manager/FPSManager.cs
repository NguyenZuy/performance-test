using UnityEngine;

namespace ZuyZuy.PT.Manager
{
    public class FPSManager : MonoBehaviour
    {
        private float deltaTime = 0.0f;
        private GUIStyle style;

        void Start()
        {
            Application.targetFrameRate = 60;

            // Initialize GUI style
            style = new GUIStyle();
            style.alignment = TextAnchor.UpperRight;
            style.fontSize = 20;
            style.normal.textColor = Color.white;
        }

        void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            float fps = 1.0f / deltaTime;
            string text = string.Format("FPS: {0:0.}", fps);
            float width = 100;
            float height = 20;
            float x = Screen.width - width - 50; // Position from right edge with 10px padding
            float y = 10; // 10px from top
            GUI.Label(new Rect(x, y, width, height), text, style);
        }
    }
}