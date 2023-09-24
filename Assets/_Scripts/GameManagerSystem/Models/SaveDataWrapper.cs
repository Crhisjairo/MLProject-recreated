using _Scripts.Models;

namespace _Scripts.GameManagerSystem.Models
{
    public class SaveDataWrapper
    {
        public GameSettingsModel GameSettingsModel { get; set; }
        public PlayerModel PlayerModel { get; set; }

        public SaveDataWrapper(GameSettingsModel gameSettingsModel, PlayerModel playerModel)
        {
            GameSettingsModel = gameSettingsModel;
            PlayerModel = playerModel;
        }

        /// <summary>
        /// Creates an empty SaveDataWrapper.
        /// </summary>
        public SaveDataWrapper()
        {
            GameSettingsModel = new GameSettingsModel();
            PlayerModel = new PlayerModel();
        }
    }
}