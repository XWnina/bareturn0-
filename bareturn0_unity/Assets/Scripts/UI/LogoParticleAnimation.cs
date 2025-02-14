using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UI
{
    public class LogoParticleAnimation : MonoBehaviour
    {
        public CanvasGroup logoCanvasGroup;
        public RectTransform logoTransform;
        public RectTransform[] letterImages;
        public ParticleSystem logoParticleSystem;
        public VideoPlayer backgroundVideoPlayer;
        public VideoClip firstVideoClip;
        public Button skipButton;
        public TMPro.TextMeshProUGUI skipHintText;
        private Vector2[] _originalPositions;
        private bool _isSkipped;

        void Start()
        {
            if (backgroundVideoPlayer != null && firstVideoClip != null)
            {
                backgroundVideoPlayer.clip = firstVideoClip;
                backgroundVideoPlayer.Play();
                StartCoroutine(ShowLogoAfterDelay(5f));
            }

            logoCanvasGroup.alpha = 0;
            logoTransform.localScale = Vector3.zero;

            _originalPositions = new Vector2[letterImages.Length];
            for (int i = 0; i < letterImages.Length; i++)
            {
                _originalPositions[i] = letterImages[i].anchoredPosition;
                letterImages[i].anchoredPosition = GetRandomStartPosition();
            }

            if (skipButton != null)
            {
                skipButton.onClick.AddListener(SkipAnimation);
            }

            if (skipHintText != null)
            {
                skipHintText.text = "Press Enter to Skip";
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SkipAnimation();
            }
        }

        void SkipAnimation()
        {
            if (!_isSkipped)
            {
                _isSkipped = true;
                StopAllCoroutines();
                StartCoroutine(FadeOutEntireScene());
            }
        }

        IEnumerator ShowLogoAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (!_isSkipped)
            {
                StartCoroutine(PlayIntroAnimation());
            }
        }

        IEnumerator PlayIntroAnimation()
        {
            float duration = 1.5f;
            logoParticleSystem.Play();

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float progress = t / duration;
                logoCanvasGroup.alpha = Mathf.Lerp(0, 1, progress);
                logoTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
                yield return null;
            }
            logoCanvasGroup.alpha = 1;
            logoTransform.localScale = Vector3.one;

            for (int i = 0; i < letterImages.Length; i++)
            {
                yield return MoveUIElement(letterImages[i], _originalPositions[i], 1f, 360);
                yield return new WaitForSeconds(0.2f);
            }

            yield return new WaitForSeconds(1f);
            StartCoroutine(FadeOutEntireScene());
        }

        IEnumerator MoveUIElement(RectTransform element, Vector2 targetPosition, float duration, float rotationAmount)
        {
            Vector2 startPos = element.anchoredPosition;
            Quaternion startRot = element.rotation;
            Quaternion endRot = Quaternion.Euler(0, 0, rotationAmount);

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float progress = t / duration;
                element.anchoredPosition = Vector2.Lerp(startPos, targetPosition, progress);
                element.rotation = Quaternion.Lerp(startRot, endRot, progress);
                yield return null;
            }

            element.anchoredPosition = targetPosition;
            element.rotation = Quaternion.identity;
        }

        private Vector2 GetRandomStartPosition()
        {
            float x, y;
            int edge = Random.Range(0, 4);
            switch (edge)
            {
                case 0:
                    x = Random.Range(-Screen.width * 0.5f, Screen.width * 0.5f);
                    y = Screen.height * 0.6f;
                    break;
                case 1:
                    x = Random.Range(-Screen.width * 0.5f, Screen.width * 0.5f);
                    y = -Screen.height * 0.6f;
                    break;
                case 2:
                    x = -Screen.width * 0.6f;
                    y = Random.Range(-Screen.height * 0.5f, Screen.height * 0.5f);
                    break;
                case 3:
                    x = Screen.width * 0.6f;
                    y = Random.Range(-Screen.height * 0.5f, Screen.height * 0.5f);
                    break;
                default:
                    x = 0;
                    y = 0;
                    break;
            }
            return new Vector2(x, y);
        }

        IEnumerator FadeOutEntireScene()
        {
            float fadeDuration = 1.5f;
            CanvasGroup sceneCanvasGroup = gameObject.AddComponent<CanvasGroup>();
            sceneCanvasGroup.alpha = 1;

            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                float progress = t / fadeDuration;
                sceneCanvasGroup.alpha = Mathf.Lerp(1, 0, progress);
                yield return null;
            }

            SceneManager.LoadScene("LoginScene");
        }
    }
}
