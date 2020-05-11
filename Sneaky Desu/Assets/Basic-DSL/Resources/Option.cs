namespace DSL
{
    public class Option
    {
        public string Content { get; private set; }
        public int ID { get; private set; }
        public int JumpToValue { get; private set; }

        public Option(int _ID, string _content)
        {
            ID = _ID;
            Content = _content;
        }

        public void SetJumpValue(int _value) => JumpToValue = _value;
    }
}
