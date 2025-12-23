
[System.Serializable]
public class TileInfo
{
    public enum TYPE
    {
        None, 
        Wait, //대기석
        Wall, Road, AllyBase, EnemyBase,
    }

    public int X;
    public int Y;

    public TYPE Type;
    public bool IsBuildable;
    public bool IsWalkable;
    public bool IsTransition = false;

    public TileInfo(int x, int y, TYPE type)
    {
        this.X = x;
        this.Y = y;
        this.Type = type;

        IsBuildable = type == TYPE.Wall;
        IsWalkable = (type == TYPE.Road || type == TYPE.AllyBase);
        IsTransition = (type == TYPE.Wall || type == TYPE.Road);
    }
}
