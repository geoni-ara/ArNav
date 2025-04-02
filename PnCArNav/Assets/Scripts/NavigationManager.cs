using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NavigationManager : MonoBehaviour
{
    public GeocodingManager geoManager;

    // Google Directions API�� ȣ���ϱ� ���� API Ű (�ڽ��� API Ű�� ��ü)
    private string apiKey = "AIzaSyBvSgBTks-PMwEgtoIplwlGplH8VlCivxI";

    // ������̼� ���� ��ư ��� ȣ���� �Լ�
    public void StartNavigation()
    {
        // ���� ��ġ�� ���� Vector2 Ÿ���� ����ϰ�, �������� PreciseCoordinates Ÿ������ �޾ƿ�
        Vector2 origin = LocationServiceManager.Instance.currentCoords;
        PreciseCoordinates destination = geoManager.destinationCoords;

        // ���� ��ġ üũ
        if (origin == Vector2.zero)
        {
            Debug.Log("���� ��ġ�� �޾ƿ��� ���߽��ϴ�.");
            return;
        }
        // PreciseCoordinates�� ���� ������ lat�� lng�� ��� 0
        if (destination.lat == 0 && destination.lng == 0)
        {
            Debug.Log("������ ��ǥ�� �����ϴ�.");
            return;
        }

        StartCoroutine(GetDirections(origin, destination));
    }

    IEnumerator GetDirections(Vector2 origin, PreciseCoordinates destination)
    {
        // �Ҽ��� 10�ڸ����� ���ڿ��� ��ȯ
        string originParam = $"{origin.x.ToString("F10")},{origin.y.ToString("F10")}";
        string destinationParam = $"{destination.lat.ToString("F10")},{destination.lng.ToString("F10")}";
        string url = $"https://maps.googleapis.com/maps/api/directions/json?origin={originParam}&destination={destinationParam}&mode=driving&region=kr&key={apiKey}";
        Debug.Log("Directions API ��û URL: " + url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            Debug.Log("Directions API ����: " + jsonResult);
            // ���⼭ JSON �Ľ� ��, ��� �����͸� AR ������Ʈ(ȭ��ǥ ��)�� �ð�ȭ�ϴ� ���� �߰�
        }
        else
        {
            Debug.Log("Directions API ��û ����: " + request.error);
        }
    }
}