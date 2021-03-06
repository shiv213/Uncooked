using System.Collections;
using UnityEngine;

namespace Uncooked
{

    internal delegate void ToggleAction(bool toggled);

    internal delegate void FunctionAction();

    internal delegate void ParamFunctionAction(object param);

    internal class Menu
    {

        private readonly ArrayList menuButtons = new ArrayList();

        private bool inSection;
        private SectionButton currentSection;

        private int selectedIndex;

        private bool isOpen;

        public void Start(Bot bot)
        {
            inSection = false;
            isOpen = true;
            selectedIndex = 0;

            menuButtons.Add(new SectionButton("Logging", new Button[] {
                new FunctionButton("Log interactables", Debug.LogInteractables),
                new FunctionButton("Log closest plate", Debug.LogPlate),
                new FunctionButton("Log carrying", Debug.LogCarrying),
                new FunctionButton("Log chef positions", Debug.LogChefPositions),
                new FunctionButton("Log cooking station positions", Debug.LogCookingStations),
                new FunctionButton("Log orders", Debug.LogOrders),
                new FunctionButton("Log reflection util test", Debug.LogTestReflectionUtil)
            }));

            menuButtons.Add(new FunctionButton("Start/stop bot", bot.ToggleExecution));

            menuButtons.Add(new FunctionButton("Pathfind to player position", Debug.PathFindToPlayer));

            menuButtons.Add(new FunctionButton("Unload", Debug.Unload));
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                isOpen = !isOpen;
            }

            if (!isOpen)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                selectedIndex++;

                bool wrap = !inSection && selectedIndex >= menuButtons.Count ||
                            inSection && selectedIndex >= currentSection.GetChildren().Count;

                if (wrap)
                {
                    selectedIndex = 0;
                }
            }

            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                selectedIndex--;
                if (selectedIndex < 0)
                {
                    if (inSection)
                    {
                        selectedIndex = currentSection.GetChildren().Count - 1;
                    }
                    else
                    {
                        selectedIndex = menuButtons.Count - 1;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Button buttonToExecute;

                if (!inSection)
                {
                    buttonToExecute = (Button)menuButtons[selectedIndex];
                }
                else
                {
                    buttonToExecute = (Button)currentSection.GetChildren()[selectedIndex];
                }

                if (buttonToExecute is SectionButton sectionButton)
                {
                    currentSection = sectionButton;
                    selectedIndex = 0;
                    inSection = true;
                }
                else
                {
                    buttonToExecute.Execute();
                }
            }

            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                if (inSection)
                {
                    if (currentSection.HasParent())
                    {
                        selectedIndex = 0;
                        currentSection = currentSection.GetParent();
                    }
                    else
                    {
                        selectedIndex = 0;
                        inSection = false;
                        currentSection = null;
                    }
                }
            }
        }

        public void OnGUI()
        {
            if (!isOpen)
            {
                return;
            }

            int drawIndex = 0;

            ArrayList buttonList;

            DrawConsole("PLACEHOLDER");

            if (!inSection)
            {
                DrawHeader("Menu");
                buttonList = menuButtons;
            }
            else
            {
                DrawHeader(currentSection.GetHeaderText());

                buttonList = currentSection.GetChildren();
            }

            foreach (Button button in buttonList)
            {
                button.Draw(ref drawIndex, drawIndex == selectedIndex);
                drawIndex++;
            }
        }

        private static void DrawHeader(string headerText)
        {
            Rect rectangle = new Rect(Button.BOX_X_OFFSET,
                Button.BOX_Y_OFFSET - Button.BOX_HEIGHT - Button.BOX_MARGIN,
                Button.BOX_WIDTH, Button.BOX_HEIGHT);
            GUI.color = new Color(140f / 255f, 150f / 255f, 240f / 255f, 0.5f);
            GUI.DrawTexture(rectangle, Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(rectangle, headerText);
        }
        public static void DrawConsole(string consoleText)
        {
            Rect console_rect = new Rect(0, Screen.height - 500, 300, 500);
            GUI.DrawTexture(console_rect, Texture2D.blackTexture);
            GUI.Label(console_rect, consoleText);
        }
    }
}