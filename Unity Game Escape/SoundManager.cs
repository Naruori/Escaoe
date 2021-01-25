using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Sound
{
    public string name;//곡의 이름
    public AudioClip clip;//곡
}
public class SoundManager : MonoBehaviour
{
    static public SoundManager instance;


    public AudioSource[] audioSourceEffects;// 효과음 오디오 소스
    public AudioSource audioSourceBgm;// 배경음 오디오 소스


    public string[] playSoundName;// 재생할 파일 이름

    public Sound[] effectSounds;// 효과음 리스트
    public Sound[] bgmSounds;// 배경음

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this.gameObject);

    }
    private void Start()
    {
        playSoundName = new string[audioSourceEffects.Length];// 효과음 오디오 소스 개수만큼 배열 길이선언
        PlayBGM(SceneManager.GetActiveScene().name);// 배경음 시작
    }

    public void SetMusicVolume(float volume)// 음악소리크기 설정
    {
        audioSourceBgm.volume = volume;
    }

    public void SetEffectVolume(float volume)// 버튼소리크기 설정
    {
		for (int i = 0; i < effectSounds.Length; i++)//모든 오디오 소스 Volume설정
        {
            audioSourceEffects[i].volume = volume;
		}
    }
    public bool IsPlaying(string _name)//재생중인지 확인하는 함수
	{
        for (int i = 0; i < effectSounds.Length; i++)
		{
            if (_name == effectSounds[i].name)
            {
                for (int j = 0; j < audioSourceEffects.Length; j++)
                {
                    if (audioSourceEffects[j].isPlaying)
                    {
                        if (audioSourceEffects[j].clip == effectSounds[i].clip)
                            return true;
                    }
                }
            }
        }
        return false;
	}

    public void PlaySE(string _name)// 새로운 효과음 재생
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            if (_name == effectSounds[i].name)// 효과음 이름으로 찾기
            {
                for (int j = 0; j < audioSourceEffects.Length; j++)// 실행하고 있지 않은 audioSource탐색
                {
                    if (!audioSourceEffects[j].isPlaying)// 실행하고 있지 않은 audioSource에 효과음 넣고 실행
                    {
                        playSoundName[j] = effectSounds[i].name;
                        audioSourceEffects[j].clip = effectSounds[i].clip;
                        audioSourceEffects[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 가용 AudioSource가 사용중입니다");
                return;
            }
        }
        Debug.Log(_name + "사운드가 SoundManager에 등록되지 않았습니다.");
    }

    public void StopAllSE()// 모든 효과음 정지
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }
    }

    public void StopSE(string _name)// 효과음 정지
    {
		for (int i = 0; i < audioSourceEffects.Length; i++)
		{
	    	if (audioSourceEffects[i].name == name)
			{
                if (audioSourceEffects[i].isPlaying)
                {
                    audioSourceEffects[i].Stop();
                    return;
                }
			}
		}
		Debug.Log("재생 중인" + _name + "사운드가 없습니다.");
    }

    public void PlayBGM(string _name)// 배경음 재생
    {
		for (int i = 0; i < bgmSounds.Length; i++)
		{
            if(_name == bgmSounds[i].name)
			{
                playSoundName[i] = bgmSounds[i].name;
                audioSourceBgm.clip = bgmSounds[i].clip;
                audioSourceBgm.Play();
                return;
            }
		}
	}
}