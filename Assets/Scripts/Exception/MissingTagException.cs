namespace Configuration.Exception
{
    public class MissingTagException : System.Exception
    {
        public MissingTagException(string tag) : base(string.Format("Tag not found: {0}", tag))
        {
            
        }
        
        
    }
}