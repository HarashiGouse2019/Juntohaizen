using System.IO;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueManagement
{
    public class Dialogue : MonoBehaviour
    {
        public static List<string> dialogue = new List<string>();

        public List<string> visibleDialogue = new List<string>();

        public int textSpeed;

        public static bool runningDialogue = false;

        public static uint lineIndex = 0;

        GameManager manager;

        static char[] delimiters = { '<', '>' };

        private void Update()
        {
            visibleDialogue = dialogue;

            if (dialogue.Count > 0)
            {
                Run(textSpeed);
                runningDialogue = true;
            }
        }

        void Run(float _speed)
        {
            if (dialogue.Count > 0)
            {
                manager = GameManager.Instance;
                StartCoroutine(manager.DisplayText(dialogue[(int)lineIndex], _speed));

                //Go to next line
                if (Input.GetKeyDown(KeyCode.Space))
                    Progress();
            }
        }

        public static void READ_DIALOGUE_SET(int _dialogueSet)
        {
            string dsPath = Application.streamingAssetsPath + @"/Chikara.dsf";
            string line = null;

            int position = 0;

            bool foundDialogueSet = false;

            if (File.Exists(dsPath))
            {
                using (StreamReader fileReader = new StreamReader(dsPath))
                {
                    while (true)
                    {
                        line = fileReader.ReadLine();

                        if (line == null && foundDialogueSet)
                        {

                            return;
                        }

                        line.Split(delimiters);

                        if (line.Contains("<DIALOGUE_SET_" + _dialogueSet.ToString("D3", CultureInfo.InvariantCulture) + ">"))
                        {
                            foundDialogueSet = true;
                            GetDialogue(position);
                        }

                        position++;
                    }
                }
            }
            Debug.LogError("File specified doesn't exist...");
        }

        static void GetDialogue(int _position)
        {
            string dsPath = Application.streamingAssetsPath + @"/Chikara.dsf";

            string line = null;

            bool atTargetLine = false;

            if (File.Exists(dsPath))
            {
                using (StreamReader fileReader = new StreamReader(dsPath))
                {
                    int position = 0;

                    while (true)
                    {
                        line = fileReader.ReadLine();

                        if (line == "<END>" && atTargetLine)
                        {
                            return;
                        }

                        if (position > _position)
                        {
                            atTargetLine = true;
                            dialogue.Add(line);
                        }

                        position++;
                    }
                }
            }
        }

        public static void Progress()
        {
            if (lineIndex != dialogue.Count)
            {
                lineIndex++;
                GameManager.Instance.dialogue.text = "";
            }
        }
    }
}
