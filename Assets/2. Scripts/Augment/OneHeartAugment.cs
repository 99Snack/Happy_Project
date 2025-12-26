
using UnityEngine;

public class OneHeartAugment : IStatusCheckAugment
{
    private bool isApplied = false;
    private int nearTower;
    private AugmentData data;
    public OneHeartAugment(AugmentData data)
    {
        this.data = data;
    }
    public void UpdateStatus(Tower owner)
    {
        nearTower = 0;
        //본인 좌표 가져오기
        Vector2Int currentPos = owner.Coord;

        //다른 타워들이 있는지
        bool isAnyTowerExist = false;

        //(0,0)은 자기 위치이므로 제외
        int[] xCoord = { -1, 0, 1 };
        int[] yCoord = { -1, 0, 1 };

        //8방향 탐색
        for (int x = 0; x < xCoord.Length; x++)
        {
            for (int y = 0; y < yCoord.Length; y++)
            {
                if (x == 0 & y == 0) continue;

                Vector2Int checkPos = new Vector2Int(currentPos.x + xCoord[x], currentPos.y + yCoord[y]);
                Debug.Log(checkPos);

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
            owner.atkPower.additiveStat -= owner.CalcStageStat(data);
            isApplied = false;
            Debug.Log($"{owner.name}: 하나된 마음 해제");
        }
        else if (!lonely && !isApplied)
        {
            owner.atkPower.additiveStat += CalcAdditiveStat(data);
            isApplied = true;
            Debug.Log($"{owner.name}: {CalcAdditiveStat(data)} 하나된 마음.");
        }
    }

    int CalcAdditiveStat(AugmentData augment)
    {
        return Mathf.FloorToInt(augment.CalcGrowValue() * nearTower);
    }
}
