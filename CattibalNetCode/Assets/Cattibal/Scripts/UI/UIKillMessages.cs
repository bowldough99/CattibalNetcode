using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIKillMessages : MonoBehaviour
{
    public static UIKillMessages instance;
    public GameObject messagePrefab;
    public float messagebuffer = 50;

    List<GameObject> messageList = new List<GameObject>();

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            AddKillMessage("John", "Bob");
        }
    }

    public void AddStarveMessage(string victim)
    {
        QueueMessage(string.Format("Cat {0} starved!", victim));
    }

    public void AddKillMessage(string killer, string killee)
    {
        QueueMessage(string.Format("Cat {0} killed Cat {1}!", killer, killee));
    }

    public void QueueMessage(string message)
    {
        GameObject tempObj = Instantiate(messagePrefab);
        tempObj.GetComponent<TextMeshProUGUI>().text = message;

        tempObj.transform.SetParent(transform);

        for (int i = 0; i < messageList.Count; ++i)
        {
            messageList[i].transform.localPosition += new Vector3(0, -messagebuffer, 0);
        }
        messageList.Add(tempObj);
        tempObj.transform.localPosition = Vector3.zero;
        StartCoroutine(DecayMessage(tempObj, 3));
    }

    IEnumerator DecayMessage(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
        while(text.color.a > 0)
        {
            text.color -= new Color(0, 0, 0, 1) * Time.deltaTime;
            yield return null;
        }
        messageList.Remove(obj);
        Destroy(obj);
    }
}
