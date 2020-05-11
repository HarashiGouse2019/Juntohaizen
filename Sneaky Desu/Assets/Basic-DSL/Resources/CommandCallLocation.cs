namespace DSL.Core
{
    public class CommandCallLocation
    {
        public string Command { get; private set; } = "";
        public int CallPosition { get; private set; } = -1;

        public int CallLine { get; private set; } = -1;

        public int DialogueSetLocation { get; private set; } = -1;

        private CommandCallLocation(string command, int dialogueSetLocation, int callLine, int callPosition)
        {
            Command = command;

            DialogueSetLocation = dialogueSetLocation;

            CallLine = callLine;

            CallPosition = callPosition;
        }

        public static CommandCallLocation New(string command, int dialogueSetLocation, int callLine, int callPosition)
            => new CommandCallLocation(command, dialogueSetLocation, callLine, callPosition);

        public object[] GetCallLocation()
        {
            object[] data =
            {
            DialogueSetLocation,
            CallLine,
            CallPosition
        };


            return data;
        }
    }

}