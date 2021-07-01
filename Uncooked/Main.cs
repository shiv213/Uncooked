using UnityEngine;

namespace Uncooked
{
    class Main : MonoBehaviour
    {
        public void Start()
        {
        }
        public void Update()
        {
        }
        public void OnGUI()
        {
            // Here you can call IMGUI functions of Unity to build your UI for the hack :)
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 150f, 50f), "GAME INJECTED"); // Should work and when injected you will see this text in the middle of the screen
        }
    }
}
