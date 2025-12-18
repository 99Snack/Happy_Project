using UnityEngine;

    //▼ 방향을 나타내는 enum
    public enum DIRECTION
    {
        West = -2, South = 1, None = 0, North = -1, East = 2
    }
    
public class DirectionCalculator
{
    /// <summary>
    /// 반대 방향을 반환하는 메서드  
    /// </summary>
    /// <param name="dir">원하는 방향</param>
    /// <returns></returns>
    public DIRECTION OppositeDirection(DIRECTION dir)
    {
        switch (dir)
        {
            case DIRECTION.North:
                return DIRECTION.South;

            case DIRECTION.West:
                return DIRECTION.East;

            case DIRECTION.South:
                return DIRECTION.North;

            case DIRECTION.East:
                return DIRECTION.West;
            default:
                break;
        }
        Debug.LogError("Not valiable direction to find opposite direction");
        return DIRECTION.None;
    }



}
