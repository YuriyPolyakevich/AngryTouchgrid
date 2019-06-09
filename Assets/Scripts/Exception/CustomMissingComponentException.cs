namespace Configuration.Exception
{
    public class CustomMissingComponentException : System.Exception
    {
        public CustomMissingComponentException(string message) : base(string.Format("Missing component {0}", message))
        {
            
        }
    }
}