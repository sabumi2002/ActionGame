using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerLogic : MonoBehaviour
{
    public int TotalItemCount; // 아이템 총 개수
    public int enemyCount;
    public int stage;
    public Text stageCountText;
    public Text PlayerCountText;
    void Awake()
    {
        //stageCountText.text = "/ " + TotalItemCount;
    }
    public void GetItem(int count)
    {
        //PlayerCountText.text = count.ToString();
    }
    private void OnTriggerEnter(Collider other)
    {
        /*
          if (other.gameObject.tag == "Player")
            SceneManager.LoadScene(stage);
        */
    }
}
