using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // singletonで実装
    public static SoundManager Instance { get; private set; }

    // ゲーム中で使用する音声
    public AudioClip bgmTitle;      // タイトル画面用のBGM
    public AudioClip bgmStageSelect;    // ステージセレクト画面用のBGM
    public AudioClip bgmStagePlay;  // ステージプレイ中のBGM
    public AudioClip bgmGameClear;  // ゲームクリア時のBMG
    public AudioClip bgmGameOver;   // ゲームオーバー時のBGM
    public AudioClip seBreakBlock;  // ブロックが壊れた時のSE
    public AudioClip seBallContact; // ボールが何かに当たった時のSE
    public AudioClip seGameStart;   // ステージスタート時のSE
    public AudioClip seBallDrop;    // ボールが落下した時のSE
    public AudioClip seButtonClick; // ボタンをクリックしたときのSE

    private AudioSource bgm;    // BGMを流す用のAudioSource
    private AudioSource se;     // SEを流す用のAudioSource


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 自身にアタッチされているAudioSourceを読み込んで１つ目をSE、２つ目をＢＧＭとする
        AudioSource[] audioSource = GetComponents<AudioSource>();
        se = audioSource[0];
        bgm = audioSource[1];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bgm.loop = true;    // BGMのAudioSourceのループ再生を有効化
        se.loop = false;    // SEのAudioSourceのループ再生を無効化
    }

    // BGM再生用のメソッド
    public void PlayBGM(AudioClip clip)
    {
        // すでにBGMを再生する場合は、フェードアウトする
        if (bgm.isPlaying) StartCoroutine(FeedOut(bgm));
        StartCoroutine(FeedIn(bgm, clip));  // 指定された音声をフェードインで再生開始する
    }

    // SE再生用のメソッド
    public void PlaySE(AudioClip clip)
    {
        se.PlayOneShot(clip);
    }

    // 音声をフェードアウトする用のコルーチン
    private IEnumerator FeedOut(AudioSource source)
    {
        // 指定されたAudioSourceが再生中であることを確認する
        if (source.isPlaying)
        {
            int phaseNum = 10;
            float vol = source.volume;  // 現在の音量を保存
            float perVol = vol / phaseNum;    // 10段階で小さくするので、一回当たりに下げる音量を計算する
            // 10回に分けて、だんだんと音を小さくしていく
            for (int i = 0; i < phaseNum; i++)
            {
                source.volume -= perVol;
                yield return new WaitForSeconds(0.1f);
            }
            if (source.volume <= 0) source.volume = vol;    // フェードアウト終了時に元の音量に戻す
        }
    }

    // 音声をフェードインする用のコルーチン
    private IEnumerator FeedIn(AudioSource source, AudioClip clip)
    {
        int phaseNum = 10;
        float vol = source.volume;  // 現在の音量を保存
        float perVol = vol / phaseNum;    // 10段階で大きくするので、1段階の音量を計算する


        source.volume = 0;  // 開始時の音量を０に設定する
        source.clip = clip; // 指定された音声を設定する
        source.Play();     // 音声を再生する
        // 10回に分けて、だんだんと音を大きくしていく
        for (int i = phaseNum; i > 0; i--)
        {
            source.volume += perVol;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
