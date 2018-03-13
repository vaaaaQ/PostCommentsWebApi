namespace PostComments.Core.Entities.Post
{
    public class Content
    {
        public string Text { get; }

        public string Image { get; }

        public Content(string text, string image = default)
        {
            Text = text;
            Image = image;
        }
    }
}