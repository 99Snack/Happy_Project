
using UnityEngine;

public class OneHeartAugment : IStatusCheckAugment
{
    private bool isApplied = false;
    private int nearTower;
    public void UpdateStatus(Tower owner, AugmentData augment)
    {
        //본인 좌표 가져오기
        Vector2Int currentPos = owner.Coord;

        //다른 타워들이 있는지
        bool isAnyTowerExist = false;

        //(0,0)은 자기 위치이므로 제외
        int[] xCoord = { -1, 1 };
        int[] yCoord = { -1, 1 };

        //8방향 탐색
        for (int x = 0; x < xCoord.Length; x++)
        {
            for (int y = 0; y < yCoord.Length; y++)
            {
                Vector2Int checkPos = new Vector2Int(currentPos.x + x, currentPos.y + y);

                //해당 좌표에 타워가 있는지
                if (TileManager.Instance.map.tiles[(x, y)].isAlreadyTower)
                {
                    isAnyTowerExist = true;
                    nearTower++;
                }
            }
        }

        bool lonely = !isAnyTowerExist;

        if (lonely && isApplied)
        {
            owner.atkPower.additiveStat -= owner.CalcStageStat(augment);
            isApplied = false;
            Debug.Log($"{owner.name}: 하나된 마음 해제");
        }
        else if (!lonely && !isApplied)
        {
            owner.atkPower.additiveStat += CalcAdditiveStat(augment);
            isApplied = true;
            Debug.Log($"{owner.name}: {CalcAdditiveStat(augment)} 하나된 마음.");
        }
    }

    int CalcAdditiveStat(AugmentData augment)
    {
        return Mathf.FloorToInt(augment.CalcGrowValue() * nearTower);
    }
}
