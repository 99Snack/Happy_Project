using UnityEngine;

[System.Serializable]
public class StageData
{
    public int FlowerCount; // ²É ¸ó½ºÅÍ ¼ö
    public int BatCount;    // ¹ÚÁã ¸ó½ºÅÍ ¼ö
    public int BeeCount;    // ¹ú ¸ó½ºÅÍ ¼ö

    // ½ºÆù ¼ø¼­ ¹è¿­ »ı¼º (²É²É²É ¹ÚÁã¹ÚÁã ¹ú ÀÌ·± ½Ä)
    public int[] GetSpawnOrder()
    {
        int total = FlowerCount + BatCount + BeeCount;
        int[] order = new int[total];

        int index = 0;

        // ²É (0¹ø) 
        for (int i = 0; i < FlowerCount; i++)
        {
            order[index++] = 0;
            index++;
        }

        for (int i = 0; i < BatCount; i++)
        {
            order[index++] = 1;
            index++;
        }

        for (int i = 0; i < BeeCount; i++)
        {
            order[index++] = 2;
            index++;
        }
        return order; 
    }
}