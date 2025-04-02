using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DeckInfoLoader : MonoBehaviour
{
    [Serializable]
    public class DeckDTO
    {
        public string _id;
        public string name;
    }

    [Serializable]
    public class DeckListResponse
    {
        public List<DeckDTO> decks;
    }

    [Serializable]
    public class SaveIdResponse
    {
        public string saveFileId;
    }

    public List<DeckDTO> deckList = new List<DeckDTO>();

    public void LoadAllDecks(Action<List<DeckDTO>> onDecksLoaded)
    {
        StartCoroutine(GetSaveFileIdAndLoadDecks(onDecksLoaded));
    }

    private IEnumerator GetSaveFileIdAndLoadDecks(Action<List<DeckDTO>> onDecksLoaded)
    {
        string saveName = PlayerPrefs.GetString("currentSaveName", "");
        string token = PlayerPrefs.GetString("token", "");
        string url = $"http://localhost:3000/savefiles/{saveName}/id";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + token);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("DeckInfoLoader: 获取 SaveFileId 失败 - " + request.error);
            onDecksLoaded?.Invoke(new List<DeckDTO>());
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("DeckInfoLoader: 收到 saveFileId 响应: " + json);

            SaveIdResponse response = JsonUtility.FromJson<SaveIdResponse>(json);

            if (response != null && !string.IsNullOrEmpty(response.saveFileId))
            {
                Debug.Log("DeckInfoLoader: 提取到 saveFileId: " + response.saveFileId);
                yield return StartCoroutine(GetDecksBySaveId(response.saveFileId, onDecksLoaded));
            }
            else
            {
                Debug.LogWarning("DeckInfoLoader: SaveFileId 解析失败");
                onDecksLoaded?.Invoke(new List<DeckDTO>());
            }
        }
    }

    private IEnumerator GetDecksBySaveId(string saveFileId, Action<List<DeckDTO>> onDecksLoaded)
    {
        string url = $"http://localhost:3000/carddecks/save/{saveFileId}";
        string token = PlayerPrefs.GetString("token", "");

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + token);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("DeckInfoLoader: 获取卡组失败 - " + request.error);
            onDecksLoaded?.Invoke(new List<DeckDTO>());
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("DeckInfoLoader: 收到卡组数据 - " + json);

            DeckListResponse response = JsonUtility.FromJson<DeckListResponse>(FixJsonArray(json));
            if (response != null && response.decks != null)
            {
                deckList = response.decks.FindAll(deck => deck.name != "Card Collection");
                onDecksLoaded?.Invoke(deckList);

            }
            else
            {
                Debug.LogWarning("DeckInfoLoader: 卡组解析失败或为空");
                onDecksLoaded?.Invoke(new List<DeckDTO>());
            }
        }
    }

    private string FixJsonArray(string json)
    {
        if (json.TrimStart().StartsWith("["))
        {
            return "{\"decks\":" + json + "}";
        }
        return json;
    }
}
