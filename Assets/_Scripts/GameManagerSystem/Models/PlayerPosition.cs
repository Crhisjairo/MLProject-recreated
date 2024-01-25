
namespace _Scripts.GameManagerSystem.Models
{
    [System.Serializable]
    public class PlayerPosition
    {
        public float x;
        public float y;
        public PlayerPosition(float positionX, float positionY)
        {
            x = positionX;
            y = positionY;
        }
    }
}
