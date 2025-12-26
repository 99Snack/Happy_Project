//처치 시 (폭발, 골드 획득 등)
public interface IOnKillAugment
{
    void OnKill(Tower owner, Enemy target, float value);
}