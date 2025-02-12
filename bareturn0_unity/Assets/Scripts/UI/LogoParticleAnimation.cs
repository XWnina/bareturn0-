using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

namespace UI
{
    public class LogoParticleAnimation : MonoBehaviour
    {
        public CanvasGroup logoCanvasGroup; // Logo 透明度控制
        public RectTransform logoTransform; // Logo 缩放控制
        public RectTransform[] letterImages; // 字母图片 RectTransform
        public ParticleSystem logoParticleSystem; // 粒子系统
        public VideoPlayer backgroundVideoPlayer; // 背景视频播放器
        public VideoClip firstVideoClip;  // 视频
        private Vector2[] _originalPositions; 
        
        void Start()
        {
            // 确保 Video Player 绑定了视频
            if (backgroundVideoPlayer != null && firstVideoClip != null)
            {
                backgroundVideoPlayer.clip = firstVideoClip;
                backgroundVideoPlayer.Play();
                StartCoroutine(ShowLogoAfterDelay(5f)); // 视频播放 5 秒后显示 Logo
            }
            else
            {
                Debug.LogError("❌ 背景视频未绑定，请在 Inspector 里绑定 Video Player 组件和 Video Clip！");
            }
            
            // 初始化状态
            logoCanvasGroup.alpha = 0;
            logoTransform.localScale = Vector3.zero;
            
            // 记录每个字母的最终位置，并设置初始位置为屏幕外的四面八方
            _originalPositions = new Vector2[letterImages.Length];
            for (int i = 0; i < letterImages.Length; i++)
            {
                _originalPositions[i] = letterImages[i].anchoredPosition;
                letterImages[i].anchoredPosition = GetRandomStartPosition();
            }
        }
        
        IEnumerator ShowLogoAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            StartCoroutine(PlayIntroAnimation());
        }
        
        IEnumerator PlayIntroAnimation()
        {
            float duration = 1.5f;
            float timer = 0;
            logoParticleSystem.Play();
            
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float progress = timer / duration;
                logoCanvasGroup.alpha = Mathf.Lerp(0, 1, progress);
                logoTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
                yield return null;
            }
            
            for (int i = 0; i < letterImages.Length; i++)
            {
                yield return MoveUIElement(letterImages[i], _originalPositions[i], duration * 0.8f, 360);
                yield return new WaitForSeconds(0.2f);
            }
        }
        
        IEnumerator MoveUIElement(RectTransform element, Vector2 targetPosition, float duration, float rotationAmount)
        {
            float elapsedTime = 0;
            Vector2 startPos = element.anchoredPosition;
            Quaternion startRot = element.rotation;
            Quaternion endRot = Quaternion.Euler(0, 0, rotationAmount);

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;
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
    }
}
