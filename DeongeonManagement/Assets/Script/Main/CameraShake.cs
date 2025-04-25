using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPosition;
    private float shakeTimeRemaining;
    private float shakePower;
    private float shakeFadeTime;
    private float shakeRotation;
    private bool isShaking = false;

    //// �ٸ� ��ũ��Ʈ���� ȣ���� �� �ִ� ���� �ν��Ͻ�
    //public static CameraShake instance;

    //private void Awake()
    //{
    //    instance = this;
    //}

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    StartShake(1.5f, 0.7f, 5.0f, 0.3f);
        //}


        if (shakeTimeRemaining > 0)
        {
            shakeTimeRemaining -= Time.deltaTime;

            float xAmount = Random.Range(-1f, 1f) * shakePower;
            float yAmount = Random.Range(-1f, 1f) * shakePower;

            transform.localPosition = originalPosition + new Vector3(xAmount, yAmount, 0f);

            // ȸ�� ��鸲 �߰� (�ɼ�)
            if (shakeRotation > 0)
            {
                float rotAmount = Random.Range(-1f, 1f) * shakeRotation;
                transform.localRotation = Quaternion.Euler(0f, 0f, rotAmount);
            }

            // �ð��� ���� ��鸲 ����
            if (shakeTimeRemaining <= shakeFadeTime)
            {
                shakePower = Mathf.Lerp(0f, shakePower, shakeTimeRemaining / shakeFadeTime);
                shakeRotation = Mathf.Lerp(0f, shakeRotation, shakeTimeRemaining / shakeFadeTime);
            }

            // ��鸲 ���� �� ���� ��ġ�� ����
            if (shakeTimeRemaining <= 0)
            {
                transform.localPosition = originalPosition;
                transform.localRotation = Quaternion.identity;
                isShaking = false;
            }
        }
    }

    /// <summary>
    /// ī�޶� ��鸲 ȿ�� ����
    /// </summary>
    /// <param name="duration">��鸲 ���� �ð�</param>
    /// <param name="power">��鸲 ����</param>
    /// <param name="rotationPower">ȸ�� ��鸲 ���� (�⺻�� 0)</param>
    /// <param name="fadeOutTime">���̵�ƿ� �ð� (�⺻���� ���� �ð��� ����)</param>
    public void StartShake(float duration, float power, float rotationPower = 0f, float fadeOutTime = -1f)
    {
        if (isShaking && power < shakePower) return; // �̹� �� ���� ��鸲�� ������ ����

        originalPosition = transform.localPosition;
        shakeTimeRemaining = duration;
        shakePower = power;
        shakeRotation = rotationPower;

        if (fadeOutTime <= 0)
        {
            shakeFadeTime = duration / 2f;
        }
        else
        {
            shakeFadeTime = fadeOutTime;
        }

        isShaking = true;
    }

    /// <summary>
    /// ��� ��鸲 ����
    /// </summary>
    public void StopShake()
    {
        shakeTimeRemaining = 0;
        transform.localPosition = originalPosition;
        transform.localRotation = Quaternion.identity;
        isShaking = false;
    }
}