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
            style.alignment = TextAnchor.UpperLeft;
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
            GUI.Label(new Rect(10, 10, 100, 20), text, style);
        }
    }
}