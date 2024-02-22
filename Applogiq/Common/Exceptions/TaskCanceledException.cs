namespace Applogiq.Common.Exceptions
{
    public class TaskCanceledException : Exception
    {
        public TaskCanceledException(string task) : base($"${task} is canceled!")
        {

        }
    }
}
