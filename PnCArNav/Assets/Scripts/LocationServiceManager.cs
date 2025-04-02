using UnityEngine;
using System.Collections;

public class LocationServiceManager : MonoBehaviour
{
    public static LocationServiceManager Instance;
    // 현재 위치 좌표 (위도, 경도)
    public Vector2 currentCoords = Vector2.zero;

    IEnumerator Start()
    {
#if UNITY_EDITOR
        // Unity 에디터에서 테스트할 때는 가상의 좌표를 사용합니다.
        // 예: 서울 시청의 좌표 (위도: 37.5665, 경도: 126.9780)
        currentCoords = new Vector2(37.5665f, 126.9780f);
        Debug.Log("Unity Editor 모드: 현재 위치 시뮬레이션 - " + currentCoords);
        yield break;
#else
        // 디바이스에서 실제 위치 서비스 사용
        // 싱글톤 패턴 적용 (여러 오브젝트 중 하나만 남기기 위한 처리)
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            yield break;
        }

        // 사용자가 위치 서비스를 활성화했는지 확인
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("사용자가 위치 서비스를 활성화하지 않았습니다.");
            yield break;
        }

        // 위치 서비스 시작
        Input.location.Start();

        // 위치 서비스 초기화 대기 (최대 20초)
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // 타임아웃 시 종료
        if (maxWait <= 0)
        {
            Debug.Log("위치 서비스 초기화 타임아웃");
            yield break;
        }

        // 위치 서비스 실패 시 종료
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("위치 서비스 실패");
            yield break;
        }

        // 정상적으로 위치 데이터를 받으면, currentCoords 업데이트
        currentCoords = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
        Debug.Log("현재 위치: " + currentCoords);
#endif
    }
}