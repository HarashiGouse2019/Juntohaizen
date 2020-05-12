public interface IDialogueReference
{
    string DialogueReference { get; set; }

    void SetDialogueReference(string _dialogue);

    int FindDialoguePosition();
}
