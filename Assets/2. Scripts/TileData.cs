
[System.Serializable]
public class TileData
{
    public enum TYPE
    {
        None, 
        Base, //단일 경로
        Wall, Road, AllyBase, EnemyBase
    }

    //실제 포지션 값
    public int X;
    public int Y;

    public TYPE Type;
    public bool IsBuildable;
    public bool IsWalkable;

    public TileData(int x, int y, TYPE type)
    {
        this.X = x;
        this.Y = y;
        this.Type = type;

        IsBuildable = (type == TYPE.Wall || type == TYPE.None);
        IsWalkable = (type == TYPE.Road || type == TYPE.AllyBase || type == TYPE.Base);
    }
}
