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

    //// 다른 스크립트에서 호출할 수 있는 정적 인스턴스
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

            // 회전 흔들림 추가 (옵션)
            if (shakeRotation > 0)
            {
                float rotAmount = Random.Range(-1f, 1f) * shakeRotation;
                transform.localRotation = Quaternion.Euler(0f, 0f, rotAmount);
            }

            // 시간에 따라 흔들림 감소
            if (shakeTimeRemaining <= shakeFadeTime)
            {
                shakePower = Mathf.Lerp(0f, shakePower, shakeTimeRemaining / shakeFadeTime);
                shakeRotation = Mathf.Lerp(0f, shakeRotation, shakeTimeRemaining / shakeFadeTime);
            }

            // 흔들림 종료 시 원래 위치로 복귀
            if (shakeTimeRemaining <= 0)
            {
                transform.localPosition = originalPosition;
                transform.localRotation = Quaternion.identity;
                isShaking = false;
            }
        }
    }

    /// <summary>
    /// 카메라 흔들림 효과 시작
    /// </summary>
    /// <param name="duration">흔들림 지속 시간</param>
    /// <param name="power">흔들림 강도</param>
    /// <param name="rotationPower">회전 흔들림 강도 (기본값 0)</param>
    /// <param name="fadeOutTime">페이드아웃 시간 (기본값은 지속 시간의 절반)</param>
    public void StartShake(float duration, float power, float rotationPower = 0f, float fadeOutTime = -1f)
    {
        if (isShaking && power < shakePower) return; // 이미 더 강한 흔들림이 있으면 무시

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
    /// 즉시 흔들림 중지
    /// </summary>
    public void StopShake()
    {
        shakeTimeRemaining = 0;
        transform.localPosition = originalPosition;
        transform.localRotation = Quaternion.identity;
        isShaking = false;
    }
}