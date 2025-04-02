using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NavigationManager : MonoBehaviour
{
    public GeocodingManager geoManager;

    // Google Directions API를 호출하기 위한 API 키 (자신의 API 키로 교체)
    private string apiKey = "AIzaSyBvSgBTks-PMwEgtoIplwlGplH8VlCivxI";

    // 내비게이션 시작 버튼 등에서 호출할 함수
    public void StartNavigation()
    {
        // 현재 위치는 기존 Vector2 타입을 사용하고, 목적지는 PreciseCoordinates 타입으로 받아옴
        Vector2 origin = LocationServiceManager.Instance.currentCoords;
        PreciseCoordinates destination = geoManager.destinationCoords;

        // 현재 위치 체크
        if (origin == Vector2.zero)
        {
            Debug.Log("현재 위치를 받아오지 못했습니다.");
            return;
        }
        // PreciseCoordinates는 값이 없으면 lat과 lng가 모두 0
        if (destination.lat == 0 && destination.lng == 0)
        {
            Debug.Log("목적지 좌표가 없습니다.");
            return;
        }

        StartCoroutine(GetDirections(origin, destination));
    }

    IEnumerator GetDirections(Vector2 origin, PreciseCoordinates destination)
    {
        // 소수점 10자리까지 문자열로 변환
        string originParam = $"{origin.x.ToString("F10")},{origin.y.ToString("F10")}";
        string destinationParam = $"{destination.lat.ToString("F10")},{destination.lng.ToString("F10")}";
        string url = $"https://maps.googleapis.com/maps/api/directions/json?origin={originParam}&destination={destinationParam}&mode=driving&region=kr&key={apiKey}";
        Debug.Log("Directions API 요청 URL: " + url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            Debug.Log("Directions API 응답: " + jsonResult);
            // 여기서 JSON 파싱 후, 경로 데이터를 AR 오브젝트(화살표 등)로 시각화하는 로직 추가
        }
        else
        {
            Debug.Log("Directions API 요청 에러: " + request.error);
        }
    }
}