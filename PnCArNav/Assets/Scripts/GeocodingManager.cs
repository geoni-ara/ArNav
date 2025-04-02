using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json; // �ݵ�� �߰�!

public class GeocodingManager : MonoBehaviour
{
    // UI ����: ������ �̸� �Է¿� InputField�� ��� ��¿� Text
    public InputField destinationInputField;
    public Text resultText;

    // �߱޹��� API Ű (Google Cloud Console���� ������ �ڽ��� API Ű�� ��ü)
    private string apiKey = "AIzaSyBvSgBTks-PMwEgtoIplwlGplH8VlCivxI";
    // �Ľ��� ��ǥ�� ������ ���� (�Ҽ��� 10�ڸ� ���е�)
    public PreciseCoordinates destinationCoords;

    // ��ư Ŭ�� �� ȣ���� �Լ� (OnClick �̺�Ʈ�� ����)
    public void OnSearchButtonClicked()
    {
        string destinationName = destinationInputField.text;
        if (!string.IsNullOrEmpty(destinationName))
        {
            StartCoroutine(GetCoordinatesFromAddress(destinationName));
        }
        else
        {
            Debug.Log("������ �̸��� �Է��ϼ���.");
        }
    }

    IEnumerator GetCoordinatesFromAddress(string address)
    {
        // �ּҸ� URL ���ڵ� ó�� (����, �ѱ� ó��)
        string encodedAddress = UnityWebRequest.EscapeURL(address);
        // Geocoding API ��û URL ����
        string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={apiKey}";
        Debug.Log("Geocoding API ��û URL: " + url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // API ���� ���� JSON ���ڿ� ��� �� �Ľ�
            string jsonResult = request.downloadHandler.text;
            Debug.Log("Geocoding API ����: " + jsonResult);

            // Newtonsoft.Json�� ����� double Ÿ������ �Ľ�
            PreciseGeocodingResponse response = JsonConvert.DeserializeObject<PreciseGeocodingResponse>(jsonResult);
            if (response.status == "OK" && response.results.Length > 0)
            {
                double lat = response.results[0].geometry.location.lat;
                double lng = response.results[0].geometry.location.lng;
                destinationCoords = new PreciseCoordinates(lat, lng);
                Debug.Log("������ ��ǥ: " + destinationCoords.ToString());

                if (resultText != null)
                    resultText.text = $"��ǥ: {destinationCoords.ToString()}";
            }
            else
            {
                Debug.Log("�ּҸ� ã�� �� �����ϴ�.");
                if (resultText != null)
                    resultText.text = "�ּҸ� ã�� �� �����ϴ�.";
            }
        }
        else
        {
            Debug.Log("Geocoding API ��û ����: " + request.error);
            if (resultText != null)
                resultText.text = "��û ����: " + request.error;
        }
    }
}

// JSON �Ľ��� ���� Ŭ������ (Google Geocoding API ���� ������ ����)
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

// �� ���� ���е��� ���� ����ü (�Ҽ��� 10�ڸ����� ǥ��)
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