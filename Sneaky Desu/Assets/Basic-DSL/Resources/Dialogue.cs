namespace DSL
{
    /// <summary>
    /// A line of dialogue from a dialogue set.
    /// </summary>
    public class Dialogue
    {
        /// <summary>
        /// The content of a dialogue
        /// </summary>
        public string Content { get; private set; }

        //Constructor
        public Dialogue(string content)
        {
            AddContent(content);
        }

        /// <summary>
        /// Add content to a dialogue.
        /// </summary>
        /// <param name="_content"></param>
        public void AddContent(string _content) => Content = _content;
    }
}