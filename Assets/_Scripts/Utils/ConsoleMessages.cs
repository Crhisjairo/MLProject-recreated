namespace _Scripts.Utils
{
    public abstract class ConsoleMessages
    {
        public const string OptionalComponentNotFound = "Optional component {0} was not found on {1}. " +
                                                        "Initializing without {0} component.";

        public const string SingletonError = "Cannot have two {0} instances.";
    }
}