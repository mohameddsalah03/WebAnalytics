namespace Analytics.Shared.Exceptions
{
    public class UnauthorizedException : ApplicationException
    {
        public UnauthorizedException(string? message =null)
            : base(message) { } 
        
            
        
    }
}
