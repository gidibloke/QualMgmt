namespace Web.Interfaces
{
    public interface ISetReturnMessages
    {
        void FailureMessage(string message);
        void SuccessMessage(string message);
        void InfoMessage(string message);
    }
}
