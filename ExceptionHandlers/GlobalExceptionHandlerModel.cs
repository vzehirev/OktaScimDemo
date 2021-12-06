namespace OktaScimDemo.ExceptionHandlers
{
    public class GlobalExceptionHandlerModel
    {
        public GlobalExceptionHandlerModel(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public bool Success { get; }
        public string ErrorMessage { get; }
    }
}
