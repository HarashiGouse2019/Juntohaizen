namespace DSL
{
    public class Dialogue
    {
        public string Content { get; private set; }

        public Dialogue(string content)
        {
            AddContent(content);
        }

        public void AddContent(string _content) => Content = _content;
    }
}