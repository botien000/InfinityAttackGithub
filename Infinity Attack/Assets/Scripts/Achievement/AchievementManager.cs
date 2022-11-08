using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class AchievementManager : MonoBehaviour
{
    private string addressGetAchievementOwn = "http://localhost:3000/inventoryClient/post-achievements-own";
    private string addressUpdateGold = "http://localhost:3000/api/updateGoldUser";
    private string addressUpdateGem = "http://localhost:3000/api/updateGemUser";
    private string addressUpdateLevelAchievementOwn = "http://localhost:3000/inventoryClient/update-level-achievement-own";
    //private string addressUpdateChallengeAchievedByName = "http://localhost:3000/inventoryClient/update-challenge-achieved-by-name";
    private string addressUpdateAllChallengeAchievedAchievementByName = "http://localhost:3000/inventoryClient/update-all-challenge-achieved-achievement-by-name";

    private string addressGetCharacterOwn = "http://localhost:3000/inventoryClient/post-character-own";

    [SerializeField] private Achievement[] achievementOwnList;
    [SerializeField] private Character[] charList;
    private Sprite successSprite;
    private Sprite processSprite;
    private Sprite goldSprite;
    private Sprite gemSprite;
    private Sprite chest_closeSprite;

    private int characterown = 0, killenemy = 0, killboss = 0, singleplay = 0, multiplay = 0, addfriend = 0;
    void Start()
    {
        LoadSprite();
        LoadChallengeAchievedKillEnemy();
        LoadChallengeAchievedKillBoss();
        LoadChallengeAchievedSinglePlay();
        LoadChallengeAchievedMultiPlay();
        LoadChallengeAchievedAddFriend();
        //if (PlayerPrefs.HasKey("UID"))
        //{
        //    string userID = PlayerPrefs.GetString("UID");
        //    StartCoroutine(GetCharacterOwnData(addressGetCharacterOwn, userID));

        //}
        string userID = "6345a02f1d8f5da83dc48826";
        StartCoroutine(GetCharacterOwnData(addressGetCharacterOwn,userID));
    }

    private void Update()
    {
        //LoadChallengeAchievedKillEnemy();
        //LoadChallengeAchievedKillBoss();
        //LoadChallengeAchievedSinglePlay();
        //LoadChallengeAchievedMultiPlay();
        //LoadChallengeAchievedAddFriend();
    }
    private void LoadSprite()
    {
        goldSprite = Resources.Load<Sprite>("GiftQuest/Gold");
        gemSprite = Resources.Load<Sprite>("GiftQuest/Gem");
        chest_closeSprite = Resources.Load<Sprite>("GiftQuest/chest_close");
        successSprite = Resources.Load<Sprite>("Quest/process");
        processSprite = Resources.Load<Sprite>("Quest/success");
    }
    IEnumerator GetCharacterOwnData(string address, string userID)
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", userID);
        UnityWebRequest www = UnityWebRequest.Post(address, form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong: " + www.error);
        }
        else
        {
            string res = www.downloadHandler.text;
            AchievementCharacterOwn(res, userID);
            www.Dispose();
        }
    }

    void AchievementCharacterOwn(string rawResponse, string userID)
    {
        var _char = JsonConvert.DeserializeObject<Character[]>(rawResponse);
        charList = _char;
        characterown = charList.Length;
        StartCoroutine(UpdateAllChallengeAchievedAchievement(addressUpdateAllChallengeAchievedAchievementByName, characterown, userID));
    }

    IEnumerator UpdateAllChallengeAchievedAchievement(string address, int characterown, string userID)
    {
        LoadChallengeAchievedKillEnemy();
        LoadChallengeAchievedKillBoss();
        LoadChallengeAchievedSinglePlay();
        LoadChallengeAchievedMultiPlay();
        LoadChallengeAchievedAddFriend();
        string characterownName = "characterown";
        string killenemyName = "killenemy";
        string killbossName = "killboss";
        string singleplayName = "singleplay";
        string multiplayName = "multiplay";
        string addfriendName = "addfriend";
        WWWForm form = new WWWForm();
        form.AddField("characterown", characterownName);
        form.AddField("killenemy", killenemyName);
        form.AddField("killboss", killbossName);
        form.AddField("singleplay", singleplayName);
        form.AddField("multiplay", multiplayName);
        form.AddField("addfriend", addfriendName);

        form.AddField("challengeAchievedCharacterOwn", characterown);
        form.AddField("challengeAchievedKillEnemy", killenemy);
        form.AddField("challengeAchievedKillBoss", killboss);
        form.AddField("challengeAchievedSinglePlay", singleplay);
        form.AddField("challengeAchievedMultiPlay", multiplay);
        form.AddField("challengeAchievedAddFriend", addfriend);
        form.AddField("userID", userID);
        UnityWebRequest www = UnityWebRequest.Post(address, form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong: " + www.error);
        }
        else
        {
            GameObject[] aclone = GameObject.FindGameObjectsWithTag("Achievement");
            foreach (GameObject ao in aclone)
            {
                if (ao.name.Equals("Achievement(Clone)"))
                {
                    Destroy(ao);
                }
            }
            StartCoroutine(GetAchievementOwnData(addressGetAchievementOwn, userID));
        }
    }

    void ProcessServerResponse(string rawResponse)
    {
        var _achievement = JsonConvert.DeserializeObject<Achievement[]>(rawResponse);
        achievementOwnList = _achievement;
        GameObject g;
        GameObject item = transform.GetChild(0).gameObject;
        item.SetActive(true);
        for (int i = 0; i < achievementOwnList.Length; i++)
        {
            g = Instantiate(item, transform);
            string name = achievementOwnList[i].achievementID.name;
            string des = achievementOwnList[i].achievementLevelID.description;
            int challengeAchieved = achievementOwnList[i].challengeAchieved;
            int challenge = achievementOwnList[i].achievementLevelID.challenge;
            int gold = achievementOwnList[i].achievementLevelID.gold;
            int gem = achievementOwnList[i].achievementLevelID.gem;

            if (challengeAchieved < challenge)
            {
                g.transform.GetChild(0).GetComponent<Image>().sprite = processSprite;
                g.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = des;
                if(gem == 0)
                {
                    g.transform.GetChild(2).GetComponent<Image>().sprite = goldSprite;
                    g.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = gold+"";

                }
                else
                {
                    g.transform.GetChild(2).GetComponent<Image>().sprite = gemSprite;
                    g.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = gem+"";
                }
                g.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = challengeAchieved + "/" + challenge;
                g.transform.GetChild(5).GetComponent<Button>().interactable = false;
                g.transform.GetChild(5).GetComponent<Image>().sprite = chest_closeSprite;
                
            }
            else if (challengeAchieved >= challenge)
            {
                g.transform.GetChild(0).GetComponent<Image>().sprite = successSprite;
                g.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = des;
                if (gem == 0)
                {
                    g.transform.GetChild(2).GetComponent<Image>().sprite = goldSprite;
                    g.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = gold + "";

                }
                else
                {
                    g.transform.GetChild(2).GetComponent<Image>().sprite = gemSprite;
                    g.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = gem + "";
                }
                g.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = challengeAchieved + "/" + challenge;
                g.transform.GetChild(5).GetComponent<Button>().AddEventListener(i, Claim);
            }         
        }
        item.SetActive(false);
    }

    void Claim(int itemIndex)
    {
        Debug.Log("Claim achievement: " + itemIndex);
        Achievement achievement = achievementOwnList[itemIndex];
        int next_level = achievement.achievementLevelID.level + 1;
        string userID = achievement.userID._id.ToString();
        string achievementOwnID = achievement._id;
        string achievementID = achievement.achievementID._id;
        int goldAchievement = achievement.achievementLevelID.gold;
        int gemAchievement = achievement.achievementLevelID.gem;
        int gold = achievement.userID.gold;
        int gem = achievement.userID.gem;
        int gold_after_update = gold + goldAchievement;
        int gem_after_update = gem + gemAchievement;
        StartCoroutine(UpdateLevelAchievementOwn(addressUpdateLevelAchievementOwn, userID, achievementOwnID, next_level, achievementID, gold_after_update, gem_after_update));
    }

    IEnumerator UpdateLevelAchievementOwn(string address, string userID, string achievementOwnID, int next_level, string achievementID, int gold, int gem)
    {
        WWWForm form = new WWWForm();
        form.AddField("achievementOwnID", achievementOwnID);
        form.AddField("level", next_level);
        form.AddField("achievementID", achievementID);
        UnityWebRequest www = UnityWebRequest.Post(address, form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong: " + www.error);
        }
        else
        {
            StartCoroutine(updateGoldAfterUpdate(addressUpdateGold, userID, gold, gem));
        }
    }

    IEnumerator updateGoldAfterUpdate(string address, string userID, int gold, int gem)
    {
        WWWForm form = new WWWForm();
        form.AddField("_id", userID);
        form.AddField("gold", gold);

        UnityWebRequest www = UnityWebRequest.Post(address, form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong: " + www.error);
        }
        else
        {
            StartCoroutine(updateGemAfterUpdate(addressUpdateGem, userID, gem));
        }
    }

    IEnumerator updateGemAfterUpdate(string address, string userID, int gem)
    {
        WWWForm form = new WWWForm();
        form.AddField("_id", userID);
        form.AddField("gem", gem);

        UnityWebRequest www = UnityWebRequest.Post(address, form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong: " + www.error);
        }
        else
        {
            GameObject[] aclone = GameObject.FindGameObjectsWithTag("Achievement");
            foreach (GameObject ao in aclone)
            {
                if (ao.name.Equals("Achievement(Clone)"))
                {
                    Destroy(ao);
                }
            }
            StartCoroutine(GetAchievementOwnData(addressGetAchievementOwn, userID));
        }
    }
    IEnumerator GetAchievementOwnData(string address, string userID)
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", userID);
        UnityWebRequest www = UnityWebRequest.Post(address, form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong: " + www.error);
        }
        else
        {
            string res = www.downloadHandler.text;
            ProcessServerResponse(res);
            Debug.Log("Achievement res: " + res);
            www.Dispose();
        }
    }

    private void LoadChallengeAchievedKillEnemy()
    {
        killenemy = PlayerPrefs.GetInt("killenemy");
    }

    private void SaveChallengeAchievedKillEnemy(int killenemy)
    {
        PlayerPrefs.SetInt("killenemy", killenemy);
    }
    private void LoadChallengeAchievedKillBoss()
    {
        killboss = PlayerPrefs.GetInt("killboss");
    }

    private void SaveChallengeAchievedKillBoss(int killboss)
    {
        PlayerPrefs.SetInt("killboss", killboss);
    }
    private void LoadChallengeAchievedSinglePlay()
    {
        singleplay = PlayerPrefs.GetInt("singleplay");
    }

    private void SaveChallengeAchievedSinglePlay(int singleplay)
    {
        PlayerPrefs.SetInt("singleplay", singleplay);
    }
    private void LoadChallengeAchievedMultiPlay()
    {
        multiplay = PlayerPrefs.GetInt("multiplay");
    }

    private void SaveChallengeAchievedMultiPlay(int multiplay)
    {
        PlayerPrefs.SetInt("multiplay", multiplay);
    }
    private void LoadChallengeAchievedAddFriend()
    {
        addfriend = PlayerPrefs.GetInt("addfriend");
    }

    private void SaveChallengeAchievedAddFriend(int addfriend)
    {
        PlayerPrefs.SetInt("addfriend", addfriend);
    }
}