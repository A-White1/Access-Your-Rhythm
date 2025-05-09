using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Conductor : MonoBehaviour
{
    public float songBpm;
    public float[] notes;
    public float[] beatsShownInAdvance;

    public GameObject noteObject;
    public GameObject Canvas;

    public List<Vector2> spawnPositions;
    int nextIndex = 0;

    public float secPerBeat;
    public float songPosition;
    public float songPositionInBeats;
    public float dspSongTime;
    public float firstBeatOffset;
    public float beatsPerLoop;
    public int completedLoops = 0;
    public float loopPositionInAnalog;
    public float loopPositionInBeats;

    public AudioSource musicSource;

    public AudioSource soundFX;
    public AudioSource resultsSong;

    public AudioClip sfxCPU, sfxCheer, sfxBad, sfxOK, resultsSongBad, resultsSongGood, resultsSongGreat;

    public static Conductor instance;

    public int score = 0;
    public int okScore = 200;
    public int goodScore = 300;
    public int perfectScore = 500;

    public int multiplier;
    public int trackMultiplier;
    public int[] multiplierMilestones;

    public int completionNum;

    private int notesHit;
    private int notesTotal;
    private int okNotesHit;
    private int goodNotesHit;
    private int perfectNotesHit;
    private int missedNotesHit;

    public GameObject resultsScreen;
    public Text percentHitText, okText, goodText, perfectText, missText, rankText, finalScoreText;

    public Text scoreText;
    public Text multiplierText;
    public Text notesCountText;

    private bool cleared = false;

    SyncedAnimation neededScript;

    void Start()
    {
        resultsScreen.SetActive(false);
        neededScript = GameObject.FindGameObjectWithTag("Activator").GetComponent<SyncedAnimation>();
        musicSource = GetComponent<AudioSource>();
        secPerBeat = 60f / songBpm;
        dspSongTime = (float)AudioSettings.dspTime;
        musicSource.Play();
        scoreText.text = "Score: 0";
        notesCountText.text = "Notes Hit: " + notesHit + "/" + notesTotal;
        multiplier = 1;
    }

    void Update()
    {

        if (musicSource.isPlaying)
        {
            songPosition = (float)(AudioSettings.dspTime - dspSongTime - firstBeatOffset);
            songPositionInBeats = songPosition / secPerBeat;

            if (songPositionInBeats >= (completedLoops + 1) * beatsPerLoop)
                completedLoops++;
            loopPositionInBeats = songPositionInBeats - completedLoops * beatsPerLoop;
            loopPositionInAnalog = loopPositionInBeats / beatsPerLoop;

            if (completedLoops % 2 == 0 && !(completedLoops == 0) && cleared == false)
            {
                cleared = true;
                GameObject[] musicNotes = GameObject.FindGameObjectsWithTag("Hittable");
                foreach (GameObject noteInstance in musicNotes)
                    Destroy(noteInstance);
            }

            if (completedLoops == completionNum)
            {
                GameObject[] musicNotes = GameObject.FindGameObjectsWithTag("Hittable");
                foreach (GameObject noteInstance in musicNotes)
                    Destroy(noteInstance);

                musicSource.Stop();
                neededScript.EndLevel();

                resultsScreen.SetActive(true);

                float percentHit = (float)(System.Math.Round(((float)notesHit / (float)notesTotal) * 100, 2));

                percentHitText.text = notesHit + " (" + percentHit + "%)";
                okText.text = okNotesHit.ToString();
                goodText.text = goodNotesHit.ToString();
                perfectText.text = perfectNotesHit.ToString();
                missText.text = missedNotesHit.ToString();
                finalScoreText.text = score.ToString();

                string rank = "F";

                if (percentHit >= 40)
                {
                    soundFX.clip = sfxBad;
                    soundFX.Play();
                    resultsSong.clip = resultsSongBad;
                    resultsSong.PlayDelayed(soundFX.clip.length);
                    rank = "D";
                    if (percentHit >= 55)
                    {
                        soundFX.clip = sfxBad;
                        soundFX.Play();
                        resultsSong.clip = resultsSongBad;
                        resultsSong.PlayDelayed(soundFX.clip.length);
                        rank = "C";
                        if (percentHit >= 70)
                        {
                            soundFX.clip = sfxOK;
                            soundFX.Play();
                            resultsSong.clip = resultsSongGood;
                            resultsSong.PlayDelayed(soundFX.clip.length);
                            rank = "B";
                            if (percentHit >= 85)
                            {
                                soundFX.clip = sfxOK;
                                soundFX.Play();
                                resultsSong.clip = resultsSongGood;
                                resultsSong.PlayDelayed(soundFX.clip.length);
                                rank = "A";
                                if (percentHit >= 90)
                                {
                                    soundFX.clip = sfxCheer;
                                    soundFX.Play();
                                    resultsSong.clip = resultsSongGreat;
                                    resultsSong.PlayDelayed(soundFX.clip.length);
                                    rank = "S";
                                    if (percentHit >= 95)
                                    {
                                        soundFX.clip = sfxCheer;
                                        soundFX.Play();
                                        resultsSong.clip = resultsSongGreat;
                                        resultsSong.PlayDelayed(soundFX.clip.length);
                                        rank = "SS";
                                        if (percentHit >= 100)
                                        {
                                            soundFX.clip = sfxCheer;
                                            soundFX.Play();
                                            resultsSong.clip = resultsSongGreat;
                                            resultsSong.PlayDelayed(soundFX.clip.length);
                                            rank = "SSS";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    soundFX.clip = sfxBad;
                    soundFX.Play();
                    resultsSong.clip = resultsSongBad;
                    resultsSong.PlayDelayed(soundFX.clip.length);
                }

                rankText.text = rank;
                Invoke("ToMainMenu", 10);
            }

            if (!(completedLoops % 2 == 0) && cleared == true)
            {
                cleared = false;

            }

            if (nextIndex < notes.Length && notes[nextIndex] < songPositionInBeats + beatsShownInAdvance[nextIndex])
            {
                GameObject musicNote = Instantiate(noteObject, spawnPositions[nextIndex], Quaternion.identity, Canvas.transform);
                musicNote.transform.SetParent(Canvas.transform, false);
                musicNote.transform.localPosition = spawnPositions[nextIndex];
                nextIndex++;

                if (completedLoops % 2 == 0)
                {
                    soundFX.clip = sfxCPU;
                    soundFX.Play();
                }
            }
        }

        
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void Awake()
    {
        instance = this;
    }

    public void HitNote()
    {

        if (multiplier - 1 < multiplierMilestones.Length)
        {

            trackMultiplier++;


            if (multiplierMilestones[multiplier - 1] <= trackMultiplier)
            {
                trackMultiplier = 0;
                multiplier++;
                
            }

        }

        if (!(multiplier > 5))
        {
            multiplierText.text = "Score Multipiler : x" + multiplier;
        }

        scoreText.text = "Score: " + score;

        notesHit++;
        notesTotal++;

        notesCountText.text = "Notes Hit: " + notesHit + "/" + notesTotal;
    }

    public void MissedNote()
    {

        multiplier = 1;
        trackMultiplier = 0;

        multiplierText.text = "Score Multipiler : x" + multiplier;
        notesTotal++;

        notesCountText.text = "Notes Hit: " + notesHit + "/" + notesTotal;
        missedNotesHit++;
    }

    public void OkHit()
    {
        score += okScore * multiplier;
        HitNote();
        okNotesHit++;
    }
    public void GoodHit()
    {
        score += goodScore * multiplier;
        HitNote();
        goodNotesHit++;
    }

    public void PerfectHit()
    {
        score += perfectScore * multiplier;
        HitNote();
        perfectNotesHit++;
    }
}
