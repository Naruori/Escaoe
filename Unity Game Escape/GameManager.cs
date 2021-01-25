using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[System.Serializable]
public class GameManager : MonoBehaviour
{

    public static bool isPause = false; //메뉴가 호출되면 true
    [SerializeField] private GameObject go_menuSet; //메뉴셋 오브젝트 
    [SerializeField] private SaveNLoad theSaveNLoad;
    private SoundManager soundManager;

	private void Start()
	{
        soundManager = GameObject.FindObjectOfType<SoundManager>();
	}
	void Update()
    {
        //게임 진행중 
        if (!isPause && go_menuSet)
        {
            Cursor.lockState = CursorLockMode.Locked; //마우스숨기기
            Cursor.visible = false;

            if (Input.GetButtonDown("Cancel"))
            {
                
                OpenMenu(); //메뉴창 생성
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None; //,마우스보이기
            Cursor.visible = true;
            if (Input.GetButtonDown("Cancel"))
            {
                
                CloseMenu(); //메뉴창 닫기
            }
        }
    }
    public void ChangeScene(string _sceneName)//씬 변경 함수
    {
        SceneManager.LoadScene(_sceneName);
        soundManager.PlayBGM(_sceneName);
	}
    
    public void OpenMenu()//메뉴 열었을 때
    {
        isPause = true;
        if(go_menuSet)
            go_menuSet.SetActive(true);
        
        Time.timeScale = 0f; // 정상속도
    }
    public void CloseMenu()// 메뉴 닫았을 때
    {
        isPause = false;
        if (go_menuSet)
            go_menuSet.SetActive(false);

        Time.timeScale = 1f; //일시정지
    }
    public void ClickSave()// 세이브 버튼 클릭
    {
        Debug.Log("세이브 버튼 클릭");
        soundManager.PlaySE("Button_Click");
        theSaveNLoad.SaveData();
	}
    public void ClickLoad()//로드 버튼 클릭
    {
        Debug.Log("로드 버튼 클릭");
        soundManager.PlaySE("Button_Click");
        theSaveNLoad.LoadData();
	}
    public void GameExit()//게임 종료
    {
        Debug.Log("Exit 클릭");
        soundManager.PlaySE("Button_Click");
        Application.Quit();
    }
}
