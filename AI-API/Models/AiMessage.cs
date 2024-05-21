namespace AI_API
{
    public class AiMessage
    {
        public string role;
        public string content;
        public AiMessage(string user,string message)
        {
            this.content = message;
            this.role = user;
        }
    }
}