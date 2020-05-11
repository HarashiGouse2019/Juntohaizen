using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using DSL.PromptOptionCase;

namespace DSL.Styling
{
    public enum ButtonStylerFormat
    {
        LIST,
        GRID
    }

    public class Styler : MonoBehaviour
    {
        
        private static Styler Instance;
        /*To stylize the UI of your game.*/
        //Reference to TextMeshPro
        [SerializeField]
        private TextMeshProUGUI text = null;

        //If there's a frame or dialgoue box, reference it.
        [SerializeField]
        private Image textBox = null;

        //For what buttons will look like
        [SerializeField]
        private Button button = null;

        //The font for the text
        [SerializeField]
        private TMP_FontAsset font = null;

        //Default Grid Size
        private const int DEFAULT_SIZE = 4;

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Get the TextMeshPro that's being used for the Dialgoue System
        /// </summary>
        /// <returns></returns>
        public static TextMeshProUGUI GetText() => Instance.text;

        /// <summary>
        /// Enable the Dialogue Box referred in the Dialgoue System
        /// </summary>
        public static void EnableDialogueBox() => Instance.textBox.gameObject.SetActive(true);

        /// <summary>
        /// Disable the Dialogue Box referred in the Dialogue System
        /// </summary>
        public static void DisableDialogueBox() => Instance.textBox.gameObject.SetActive(false);

        /// <summary>
        /// Generate buttons in a set format
        /// </summary>
        /// <param name="_format"></param>
        public static void GenerateOptions(ButtonStylerFormat _format, Option[] _content)
        {
            //The formatting of how the buttons will be displayed
            switch (_format)
            {
                case ButtonStylerFormat.LIST: LayoutButtonsByList(DEFAULT_SIZE, _content); break;

                case ButtonStylerFormat.GRID: LayoutButtonsByGrid(DEFAULT_SIZE, DEFAULT_SIZE, _content); break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Generate buttons in a set format
        /// </summary>
        /// <param name="_format"></param>
        public static void GenerateOptions(ButtonStylerFormat _format, int _listLength, Option[] _content)
        {
            //The formatting of how the buttons will be displayed
            switch (_format)
            {
                case ButtonStylerFormat.LIST: LayoutButtonsByList(_listLength, _content); break;

                case ButtonStylerFormat.GRID: LayoutButtonsByGrid(DEFAULT_SIZE, DEFAULT_SIZE, _content); break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Generate buttons in a set format
        /// </summary>
        /// <param name="_format"></param>
        public static void GenerateOptions(ButtonStylerFormat _format, int _gridWidth, int _gridHeight, Option[] _content)
        {
            //The formatting of how the buttons will be displayed
            switch (_format)
            {
                case ButtonStylerFormat.LIST: LayoutButtonsByList(DEFAULT_SIZE, _content); break;

                case ButtonStylerFormat.GRID: LayoutButtonsByGrid(_gridWidth, _gridHeight, _content); break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Layout buttons in a list
        /// </summary>
        /// <param name="_content"></param>
        static void LayoutButtonsByList(int _length, Option[] _content)
        {
            //I think the first thing that we should do is get the width and height of the buttons that will show as a list
            //2nd of all, we want to have a gameObject that has an object pool of these different buttons ready to go.
            //Then, we can freely change the size and the content, since they will have what we need.

            //The game object will be a normal button with no graphics, but as they are generated, we'll assign the right styling
            //that the creator wants for the ui to look like.
            
            //The font will also be assigned to all the TMPText component in all the objects


        }

        /// <summary>
        /// Layout buttons in a grid with a set width and height
        /// </summary>
        /// <param name="_gridWidth"></param>
        /// <param name="_gridHeight"></param>
        /// <param name="_content"></param>
        static void LayoutButtonsByGrid(int _gridWidth, int _gridHeight, Option[] _content)
        {

        }
    }
}