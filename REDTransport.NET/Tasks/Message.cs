namespace REDTransport.NET.Tasks
{
    public class Message
    {


        public static string GetMessageId(Message message)
        {
            return message.GetHashCode().ToString();
        }
    }
}