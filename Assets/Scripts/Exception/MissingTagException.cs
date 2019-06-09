namespace Configuration.Exception
{
    public class MissingTagException : System.Exception
    {
        public MissingTagException(string message) : base(string.Format("Tag not found: {0}", message))
        {
            
        }
        
        
    }
}