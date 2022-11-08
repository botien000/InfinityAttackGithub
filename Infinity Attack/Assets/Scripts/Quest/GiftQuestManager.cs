using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;
public class GiftQuestManager : MonoBehaviour
{
    private string addressGetGiftOwn = "http://localhost:3000/inventoryClient/post-gifts-own";
    private string addressUpdateGold = "http://localhost:3000/api/updateGoldUser";
    private string addressUpdateGem = "http://localhost:3000/api/updateGemUser";
    private string addressUpdateStatusGiftOwn = "http://localhost:3000/inventoryClient/update-status-gift-own";

    [SerializeField] private Gift[] giftOwnList;

    [SerializeField] private Slider missionProcess;
    [SerializeField] private TextMeshProUGUI giftText;
    [SerializeField] private Image avtGift;
    [SerializeField] private TextMeshProUGUI takeGiftText;
    [SerializeField] private Button takeGift;
    private int totalGift;
    private int moc1 = 0, moc2 = 0, moc3 = 0, moc4 = 0;
    private int clickGift;
    private Sprite goldSprite;
    private Sprite gemSprite;
    private Sprite chest_closeSprite;
    private Sprite chest_openSprite;
    void Start()
    {
        LoadTotal();
        LoadSprite();
        missionProcess.maxValue = 100;
        missionProcess.value = totalGift;
        StartCoroutine(GetGiftsOwnData(addressGetGiftOwn));
    }
    private void LoadSprite()
    {
        goldSprite = Resources.Load<Sprite>("GiftQuest/Gold");
        gemSprite = Resources.Load<Sprite>("GiftQuest/Gem");
        chest_closeSprite = Resources.Load<Sprite>("GiftQuest/chest_close");
        chest_openSprite = Resources.Load<Sprite>("GiftQuest/chest_open");
    }
    void Update()
    {
        LoadTotal();
        missionProcess.value = totalGift;
    }
    public void GiftClicked()
    {
        LoadClicked();
        if(moc1 == 1)
        {
            clickGift = 0;
        }
        else if(moc2 == 1)
        {
            clickGift = 1;
        }
        else if (moc3 == 1)
        {
            clickGift = 2;
        }
        else if (moc4 == 1)
        {
            clickGift = 3;
        }
        Debug.Log("Click Gift: " + clickGift);
        StartCoroutine(updateStatusGiftOwn(addressUpdateStatusGiftOwn, clickGift));
    }

    IEnumerator updateStatusGiftOwn(string address, int clickGift)
    {
        Gift _gift = giftOwnList[clickGift];
        string id = _gift._id;
        string userID = _gift.userID._id.ToString();
        int goldGift = _gift.giftID.gold;
        int gemGift = _gift.giftID.gem;
        int gold = _gift.userID.gold;
        int gem = _gift.userID.gem;

        int gold_after_update = gold + goldGift;
        int gem_after_update = gem + gemGift;
        WWWForm form = new WWWForm();
        form.AddField("giftOwnID", id);
        UnityWebRequest www = UnityWebRequest.Post(address, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong: " + www.error);
        }
        else
        {
            StartCoroutine(updateGoldAfterUpdate(addressUpdateGold, userID, gold_after_update, gem_after_update));
        }
    }

    IEnumerator updateGoldAfterUpdate(string address, string userID, int gold_after_update, int gem_after_update)
    {
        WWWForm form = new WWWForm();
        form.AddField("_id", userID);
        form.AddField("gold", gold_after_update);

        UnityWebRequest www = UnityWebRequest.Post(address, form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong: " + www.error);
        }
        else
        {
            StartCoroutine(updateGemAfterUpdate(addressUpdateGem, userID, gem_after_update));
        }
    }

    IEnumerator updateGemAfterUpdate(string address, string userID, int gem_after_update)
    {
        WWWForm form = new WWWForm();
        form.AddField("_id", userID);
        form.AddField("gem", gem_after_update);

        UnityWebRequest www = UnityWebRequest.Post(address, form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong: " + www.error);
        }
        else
        {
            GameObject[] hclone = GameObject.FindGameObjectsWithTag("GiftQuest");
            foreach (GameObject ho in hclone)
            {
                if (ho.name.Equals("Gift(Clone)"))
                {
                    Destroy(ho);
                }
            }
            StartCoroutine(GetGiftsOwnData(addressGetGiftOwn));
        }
    }
    void GiftOwnDataRespond(string rawResponse)
    {
        var _gift = JsonConvert.DeserializeObject<Gift[]>(rawResponse);
        giftOwnList = _gift;
        GameObject h;
        GameObject gift = transform.GetChild(0).gameObject;
        gift.SetActive(true);
        for (int i = 0; i < giftOwnList.Length; i++)
        {
            int status = giftOwnList[i].status;
            h = Instantiate(gift, transform);
            if(status == 0)
            {
                h.transform.GetComponent<Image>().sprite = chest_closeSprite;
                h.transform.GetComponent<Button>().interactable = true;
            }
            else if (status == 1)
            {
                h.transform.GetComponent<Image>().sprite = chest_openSprite;
                h.transform.GetComponent<Button>().interactable = false;
            }
            h.GetComponent<Button>().AddEventListener(i, ItemClicked);
        }

        gift.SetActive(false);
    }
    void ItemClicked(int itemIndex)
    {
        Gift gift = giftOwnList[itemIndex];
        int gold = gift.giftID.gold;
        int gem = gift.giftID.gem;
        string name = gift.giftID.name;
        if (PlayerPrefs.HasKey("totalGift"))
        {
            if(name == "moc1" && totalGift < 25)
            {
                takeGift.interactable = false;
                takeGiftText.color = Color.gray;
            }
            else if (name == "moc1" && totalGift >= 25)
            {
                takeGift.interactable = true;
                takeGiftText.color = Color.white;
            }
            else if (name == "moc2" && totalGift < 50)
            {
                takeGift.interactable = false;
                takeGiftText.color = Color.gray;
            }
            else if (name == "moc2" && totalGift >= 50)
            {
                takeGift.interactable = true;
                takeGiftText.color = Color.white;
            }
            else if (name == "moc3" && totalGift < 75)
            {
                takeGift.interactable = false;
                takeGiftText.color = Color.gray;
            }
            else if (name == "moc3" && totalGift >= 75)
            {
                takeGift.interactable = true;
                takeGiftText.color = Color.white;
            }
            else if (name == "moc4" && totalGift < 100)
            {
                takeGift.interactable = false;
                takeGiftText.color = Color.gray;
            }
            else if (name == "moc4" && totalGift == 100)
            {
                takeGift.interactable = true;
                takeGiftText.color = Color.white;
            }
        }

        if (gem == 0)
        {
            giftText.text = gold + "";
            avtGift.sprite = goldSprite;
        }
        else
        {
            giftText.text = gem + "";
            avtGift.sprite = gemSprite;
        }

        if(name == "moc1")
        {
            moc1 = 1;
            moc2 = 0;
            moc3 = 0;
            moc4 = 0;
        }
        else if (name == "moc2")
        {
            moc1 = 0;
            moc2 = 1;
            moc3 = 0;
            moc4 = 0;
        }
        else if (name == "moc3")
        {
            moc1 = 0;
            moc2 = 0;
            moc3 = 1;
            moc4 = 0;
        }
        else if (name == "moc4")
        {
            moc1 = 0;
            moc2 = 0;
            moc3 = 0;
            moc4 = 1;
        }
        SaveClick(moc1, moc2, moc3, moc4);
        Debug.Log("Gift name clicked: " + gift.giftID.name);
    }
    IEnumerator GetGiftsOwnData(string address)
    {
        //string userID = PlayerPrefs.GetString("uID");
        string userID = "6345a02f1d8f5da83dc48826";
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
            GiftOwnDataRespond(res);
            Debug.Log("Gift res: " + res);
            www.Dispose();
        }
    }

    private void LoadTotal()
    {
        totalGift = PlayerPrefs.GetInt("totalGift");
    }

    private void LoadClicked()
    {
        moc1 = PlayerPrefs.GetInt("moc1");
        moc2 = PlayerPrefs.GetInt("moc2");
        moc3 = PlayerPrefs.GetInt("moc3");
        moc4 = PlayerPrefs.GetInt("moc4");
    }

    private void SaveClick(int moc1, int moc2, int moc3, int moc4)
    {
        PlayerPrefs.SetInt("moc1", moc1);
        PlayerPrefs.SetInt("moc2", moc2);
        PlayerPrefs.SetInt("moc3", moc3);
        PlayerPrefs.SetInt("moc4", moc4);
    }

}