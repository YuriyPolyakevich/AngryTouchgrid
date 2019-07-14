namespace Configuration.Exception
{
    public class CustomMissingComponentException : System.Exception
    {
        public CustomMissingComponentException(string component) : base(string.Format("Missing component {0}", component))
        {
            
        }
    }
}