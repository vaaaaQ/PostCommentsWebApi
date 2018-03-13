namespace PostComments.Core.Entities.Post
{
    public class Title
    {
        public string Text { get; set; }

        public Title(string text)
        {
            Text = text;
        }
    }
}