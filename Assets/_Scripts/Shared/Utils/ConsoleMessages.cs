namespace _Scripts.Shared.Utils
{
    public abstract class ConsoleMessages
    {
        public const string OptionalComponentNotFound = "Optional component {0} was not found on {1}. " +
                                                        "Proceeding without {0} component.";

        public const string SingletonError = "Cannot have two {0} instances.";
        public const string NoPointsAssignedToNpc = "No points was assigned to NPC {0} and \"Move Between Points\" " +
                                                    "is checked. Please, add points where to move NPC or disable " +
                                                    "\"Move Between Point\". ";

        public const string LifeLowerThanZeroUI = "Life amount cannot be lower than 0 on UI.";
        public const string MaxLifeMoreThanMaxHeartsOnUI = "Max life amount cannot be more than max UI elements " +
                                                           "created for lifes.\nThis behaviour will be change to" +
                                                           " auto create hearts GameObjets by max life amount.";

        public const string SoundNameNotFound = "Sound {0} was not found.\nPlease make sure that the " +
                                                "sound name is correct.";
        public const string MusicNameNotFound = "Music {0} was not found.\nPlease make sure that the " +
                                                "music name is correct.";
        public const string MoreOnceMusicPlayeOnAwake = "More than one musics was marked as Playe On Awake.\n" +
                                                        "Only the last one is player on awake.";
        public const string WrongComponentTypeAttached = "Component {0} is not type of {1}.";
    }
}