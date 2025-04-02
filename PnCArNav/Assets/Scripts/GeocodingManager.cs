using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json; // 반드시 추가!

public class GeocodingManager : MonoBehaviour
{
    // UI 연결: 목적지 이름 입력용 InputField와 결과 출력용 Text
    public InputField destinationInputField;
    public Text resultText;

    // 발급받은 API 키 (Google Cloud Console에서 생성한 자신의 API 키로 교체)
    private string apiKey = "AIzaSyBvSgBTks-PMwEgtoIplwlGplH8VlCivxI";
    // 파싱한 좌표를 저장할 변수 (소수점 10자리 정밀도)
    public PreciseCoordinates destinationCoords;

    // 버튼 클릭 시 호출할 함수 (OnClick 이벤트에 연결)
    public void OnSearchButtonClicked()
    {
        string destinationName = destinationInputField.text;
        if (!string.IsNullOrEmpty(destinationName))
        {
            StartCoroutine(GetCoordinatesFromAddress(destinationName));
        }
        else
        {
            Debug.Log("목적지 이름을 입력하세요.");
        }
    }

    IEnumerator GetCoordinatesFromAddress(string address)
    {
        // 주소를 URL 인코딩 처리 (공백, 한글 처리)
        string encodedAddress = UnityWebRequest.EscapeURL(address);
        // Geocoding API 요청 URL 구성
        string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={apiKey}";
        Debug.Log("Geocoding API 요청 URL: " + url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // API 응답 받은 JSON 문자열 출력 및 파싱
            string jsonResult = request.downloadHandler.text;
            Debug.Log("Geocoding API 응답: " + jsonResult);

            // Newtonsoft.Json을 사용해 double 타입으로 파싱
            PreciseGeocodingResponse response = JsonConvert.DeserializeObject<PreciseGeocodingResponse>(jsonResult);
            if (response.status == "OK" && response.results.Length > 0)
            {
                double lat = response.results[0].geometry.location.lat;
                double lng = response.results[0].geometry.location.lng;
                destinationCoords = new PreciseCoordinates(lat, lng);
                Debug.Log("목적지 좌표: " + destinationCoords.ToString());

                if (resultText != null)
                    resultText.text = $"좌표: {destinationCoords.ToString()}";
            }
            else
            {
                Debug.Log("주소를 찾을 수 없습니다.");
                if (resultText != null)
                    resultText.text = "주소를 찾을 수 없습니다.";
            }
        }
        else
        {
            Debug.Log("Geocoding API 요청 실패: " + request.error);
            if (resultText != null)
                resultText.text = "요청 실패: " + request.error;
        }
    }
}

// JSON 파싱을 위한 클래스들 (Google Geocoding API 응답 구조에 맞춤)
[System.Serializable]
public class PreciseGeocodingResponse
{
    public string status;
    public PreciseGeocodingResult[] results;
}

[System.Serializable]
public class PreciseGeocodingResult
{
    public PreciseGeometry geometry;
}

[System.Serializable]
public class PreciseGeometry
{
    public PreciseLocation location;
}

[System.Serializable]
public class PreciseLocation
{
    public double lat;
    public double lng;
}

// 더 높은 정밀도를 위한 구조체 (소수점 10자리까지 표현)
public struct PreciseCoordinates
{
    public double lat;
    public double lng;

    public PreciseCoordinates(double lat, double lng)
    {
        this.lat = lat;
        this.lng = lng;
    }

    public override string ToString()
    {
        return lat.ToString("F10") + ", " + lng.ToString("F10");
    }
}