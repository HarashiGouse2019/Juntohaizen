namespace DSL.PromptOptionCase
{

    public class Option : IDialogueReference
    {
        public string Content { get; private set; }
        public int ID { get; private set; }
        public int gotoLine { get; private set; }

        public string DialogueReference { get; set; }

        public Option(int _ID, string _content)
        {
            ID = _ID;
            Content = _content;
        }

        public void SetDialogueReference(string _dialogue) => DialogueReference = _dialogue;

        public int FindDialoguePosition()
        {
            int position = 0;

            foreach (Dialogue dialogue in DialogueSystem.DialogueList)
            {
                if (DialogueReference.Contains(dialogue.Content))
                {
                    gotoLine = position;
                    return position;
                }
                position++;
            }

            return -1;
        }
    }
}
