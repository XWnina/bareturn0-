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
        public VideoPlayer backgroundVideoPlayer; // 第一个背景视频播放器
        public VideoPlayer transitionVideoPlayer; // 第二个背景视频播放器
        public VideoClip firstVideoClip;  // 第一个视频
        public VideoClip secondVideoClip; // 第二个视频
        private Vector2[] _originalPositions; // 记录字母的初始位置
        
        void Start()
        {
            // 确保第一个 Video Player 绑定了不同的视频
            if (backgroundVideoPlayer != null && firstVideoClip != null)
            {
                backgroundVideoPlayer.clip = firstVideoClip;
                backgroundVideoPlayer.Play();
                StartCoroutine(TransitionToSecondVideo(22f)); // 22秒后切换背景视频并开始 Logo 动画
            }
            else
            {
                Debug.LogError("❌ 第一个背景视频未绑定，请在 Inspector 里绑定 Video Player 组件和 Video Clip！");
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
        
        IEnumerator TransitionToSecondVideo(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            float fadeDuration = 2f;
            float timer = 0;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
                backgroundVideoPlayer.targetCameraAlpha = alpha;
                yield return null;
            }
            
            backgroundVideoPlayer.Stop();
            backgroundVideoPlayer.targetCameraAlpha = 0;
            
            if (transitionVideoPlayer != null && secondVideoClip != null)
            {
                transitionVideoPlayer.clip = secondVideoClip;
                transitionVideoPlayer.Play(); // 播放第二个背景视频
                StartCoroutine(FadeInLogo(1f)); // 1秒后渐显 Logo
            }
            else
            {
                Debug.LogError("❌ 第二个背景视频未绑定，请在 Inspector 里绑定 Video Player 组件和 Video Clip！");
            }
        }
        
        IEnumerator FadeInLogo(float delay)
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
