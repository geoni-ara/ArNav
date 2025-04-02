using UnityEngine;
using System.Collections;

public class LocationServiceManager : MonoBehaviour
{
    public static LocationServiceManager Instance;
    // ���� ��ġ ��ǥ (����, �浵)
    public Vector2 currentCoords = Vector2.zero;

    IEnumerator Start()
    {
#if UNITY_EDITOR
        // Unity �����Ϳ��� �׽�Ʈ�� ���� ������ ��ǥ�� ����մϴ�.
        // ��: ���� ��û�� ��ǥ (����: 37.5665, �浵: 126.9780)
        currentCoords = new Vector2(37.5665f, 126.9780f);
        Debug.Log("Unity Editor ���: ���� ��ġ �ùķ��̼� - " + currentCoords);
        yield break;
#else
        // ����̽����� ���� ��ġ ���� ���
        // �̱��� ���� ���� (���� ������Ʈ �� �ϳ��� ����� ���� ó��)
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            yield break;
        }

        // ����ڰ� ��ġ ���񽺸� Ȱ��ȭ�ߴ��� Ȯ��
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("����ڰ� ��ġ ���񽺸� Ȱ��ȭ���� �ʾҽ��ϴ�.");
            yield break;
        }

        // ��ġ ���� ����
        Input.location.Start();

        // ��ġ ���� �ʱ�ȭ ��� (�ִ� 20��)
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Ÿ�Ӿƿ� �� ����
        if (maxWait <= 0)
        {
            Debug.Log("��ġ ���� �ʱ�ȭ Ÿ�Ӿƿ�");
            yield break;
        }

        // ��ġ ���� ���� �� ����
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("��ġ ���� ����");
            yield break;
        }

        // ���������� ��ġ �����͸� ������, currentCoords ������Ʈ
        currentCoords = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
        Debug.Log("���� ��ġ: " + currentCoords);
#endif
    }
}