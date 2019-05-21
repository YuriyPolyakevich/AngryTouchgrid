namespace a
{
    public class GlobalConfiguration
    {
        private static readonly bool _isDevMode = true;
        
        public static bool IsDevMode()
        {
            return _isDevMode;
        }
    }
}