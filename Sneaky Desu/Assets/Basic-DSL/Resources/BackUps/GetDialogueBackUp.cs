//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class GetDialogueBackUp : MonoBehaviour
//{
//    /// <summary>
//    /// Get all dialogue in a specified Dialogue_Set inside a .dsl file
//    /// </summary>
//    /// <param name="_position">Position to start collecting data</param>
//    public static void GetDialogue(int _position)
//    {
//        //TODO: For this function only, collect information such as $ and @, $ stating that it's not dialogue-centric, and @ meaning
//        //that this is actual dialogue.

//        //That way, commands like CALL, and the start of other code can happen through those points

//        //Toggle if we are at a desired position in the file.
//        bool atTargetLine = false;

//        Prompt latestPrompt = null;

//        int position = 0;

//        foreach (string line in CompiledData)
//        {

//            string dataLine = line.Trim('\t', ' '); //Remove all tabs and spaces, so that we can always get to @

//            //If we reach the end of the dialogue set, we are done reading it
//            if (dataLine == END && atTargetLine)
//                return;

//            //However, if we are at the DialogueSet tag with a specified number, we collect all the dialogue that starts with "@"
//            if (position > _position)
//            {
//                atTargetLine = true; //Toggle on

//                //Collecting Dialogue
//                #region Dialogue Collection ("@")
//                //Make sure that we are specifically encountering "<...>"
//                if (dataLine != STRINGNULL && Validater.ValidateLineEndOperartor(dataLine))
//                {
//                    //Validate the use of a character
//                    dataLine = Validater.ValidateCharacterUsage(dataLine, position);

//                    Debug.Log(dataLine);

//                    #region CALL PROMPT
//                    if (Validater.ValidateCallFunction(dataLine, out int continuation))
//                    {
//                        //Get the entire call mehtod "CALL --- ---"
//                        string[] data = ExtractCallFrom(continuation, dataLine);

//                        //Get what you are calling, and the value
//                        string callingTarget = data[1];

//                        //Check for any of these keywords
//                        foreach (string keyword in Keywords)
//                        {

//                            //If you are calling to show the options from a prompt
//                            if (callingTarget == GetKeyWord("PROMPT"))
//                            {
//                                //Get what prompt number it is, and disply choices
//                                //We expect only 1 value, which is a number;
//                                int promptNumber = Convert.ToInt32(data[2]);

//                                //Get the prompt being called
//                                Prompt targetPrompt = GetDefinedPrompt(promptNumber);

//                                targetPrompt.SetCallingLine(position + 1);

//                                PromptStack.Push(targetPrompt);

//                                //TODO: Enhance the syling portion of DSL, especially for choices.
//                                //TODO: When prompt is called, have it search for OUT.
//                                //OUT will find either @, or if it can't find any, dialogue set ends



//                                break;
//                            }

//                        }
//                    }
//                    #endregion
//                    DialogueSystem.DialogueData.Add(dataLine);

//                    if (latestPrompt != null)
//                    {
//                        //This would be the next avaliable dialogue for the prompt in the stack
//                        latestPrompt.SetDialogueReference(dataLine);
//                        latestPrompt.FindDialoguePosition();

//                        latestPrompt = null;
//                    }
//                }
//                #endregion

//                #region 
//                else if (dataLine != STRINGNULL)
//                {

//                }
//                #endregion
//                //If an OUT is sceen
//                #region PROMPT OUT CALL
//                if (line == GetKeyWord("OUT"))
//                {
//                    //Get the prompt from the stack, and find the next dialogue avaliable
//                    latestPrompt = PromptStack.Pop();
//                }
//                #endregion
//            }
//            position++;
//        }
//    }
//}
