using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SaveNLoad : MonoBehaviour
{
    private PlayerMove thePlayer;// 플레이어 컨트롤러
    private Dialogue theDialogue;// 다이얼로그
    public void SaveData()// 데이터 저장
    {
        thePlayer = FindObjectOfType<PlayerMove>();// 플레이어 가져오기
        PlayerPrefs.SetFloat("PlayerPosX", thePlayer.transform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", thePlayer.transform.position.y);
        PlayerPrefs.SetFloat("PlayerPosZ", thePlayer.transform.position.z);

        PlayerPrefs.SetFloat("PlayerRotX", thePlayer.transform.eulerAngles.x);
        PlayerPrefs.SetFloat("PlayerRotY", thePlayer.transform.eulerAngles.y);
        PlayerPrefs.SetFloat("PlayerRotZ", thePlayer.transform.eulerAngles.z);
        

        PlayerPrefs.SetString("SceneName", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        Debug.Log("저장 완료");
	}

	public void LoadData()// 데이터 불러오기
    {
        if (!PlayerPrefs.HasKey("PlayerPosX"))
        {
            Debug.Log("저장된 데이터 없음");
            return;
        }
        thePlayer = FindObjectOfType<PlayerMove>();// 플레이어 가져오기
        if (SceneManager.GetActiveScene().name == "TowerOutside")
        {
            theDialogue = FindObjectOfType<Dialogue>(); 
            theDialogue.OnOff(false); 
        }
        // 플레이어 위치 가져오기
        float PosX = PlayerPrefs.GetFloat("PlayerPosX");
        float PosY = PlayerPrefs.GetFloat("PlayerPosY");
        float PosZ = PlayerPrefs.GetFloat("PlayerPosZ");

        // 플레이어 방향 가져오기
        float RotX = PlayerPrefs.GetFloat("PlayerRotX");
        float RotY = PlayerPrefs.GetFloat("PlayerRotY");
        float RotZ = PlayerPrefs.GetFloat("PlayerRotZ");

        thePlayer.transform.position = new Vector3(PosX, PosY, PosZ);// 플레이어 위치 대입
        thePlayer.transform.eulerAngles = new Vector3(RotX, RotY, RotZ);// 플레이어 방향 대입

        

        Debug.Log("로드데이터");
	}
}
