using UnityEngine;

public class Monster : MonoBehaviour
{
    // 스폰 번호(순서) 길찾기에서 사용용
    public int SpawnNumber { get; private set; }

    // 스포너에서 호출해서 번호 설정
    public void SetSpawnNumber(int number)
    {
        SpawnNumber = number;
    }

    // 몬스터 죽을 때 호출 
    public void Die()
    {
        // 웨이브 매니저에 알리기
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        waveManager.OnMonsterDie();

        Destroy(gameObject);
    }

    // 테스트용: 마우스 클릭하면 죽음
    private void OnMouseDown()
    {
        Die();
    }
}