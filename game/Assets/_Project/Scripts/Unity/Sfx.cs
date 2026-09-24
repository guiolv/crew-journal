// CrewJournal — SFX com override por arquivo (Resources/Audio/sfx_<nome>).
// Sem arquivo = procedural. Auditoria game-feel #3.
using System;
using UnityEngine;

public static class Sfx
{
    const int Rate = 22050;
    const float Vol = 0.3f;
    static AudioSource src;

    public static bool muted
    {
        get { return PlayerPrefs.GetInt("cj_mute", 0) == 1; }
        set { PlayerPrefs.SetInt("cj_mute", value ? 1 : 0); PlayerPrefs.Save(); }
    }

    static AudioSource Src()
    {
        if (src != null) return src;
        GameObject go = new GameObject("Sfx");
        UnityEngine.Object.DontDestroyOnLoad(go);
        src = go.AddComponent<AudioSource>();
        src.volume = Vol;
        return src;
    }

    static void Play(float[] data)
    {
        if (muted) return;
        AudioClip clip = AudioClip.Create("sfx", data.Length, 1, Rate, false);
        clip.SetData(data, 0);
        Src().PlayOneShot(clip);
    }

    static bool PlayFile(string key)
    {
        if (muted) return true;
        AudioClip clip = Resources.Load<AudioClip>("Audio/sfx_" + key);
        if (clip == null) return false;
        Src().PlayOneShot(clip);
        return true;
    }

    static float[] Tone(float freq, float secs, float slideTo)
    {
        int n = Math.Max(1, (int)(Rate * secs));
        float[] d = new float[n];
        float phase = 0f;
        for (int i = 0; i < n; i++)
        {
            float t = (float)i / n;
            float f = freq + (slideTo - freq) * t;
            phase += 2f * (float)Math.PI * f / Rate;
            d[i] = (float)Math.Sin(phase) * (1f - t);
        }
        return d;
    }

    static float[] Mix(params float[][] parts)
    {
        int n = 0;
        for (int i = 0; i < parts.Length; i++) n += parts[i].Length;
        float[] d = new float[n];
        int k = 0;
        for (int i = 0; i < parts.Length; i++)
        {
            for (int j = 0; j < parts[i].Length; j++) d[k++] = parts[i][j] * 0.8f;
        }
        return d;
    }

    public static void Click()
    {
        if (!PlayFile("click")) Play(Tone(1250f, 0.06f, 900f));
    }

    public static void Coin()
    {
        if (!PlayFile("coin")) Play(Mix(Tone(950f, 0.09f, 950f), Tone(1420f, 0.14f, 1420f)));
    }

    public static void Hit()
    {
        if (PlayFile("hit")) return;
        System.Random rng = new System.Random();
        int n = (int)(Rate * 0.18f);
        float[] d = new float[n];
        for (int i = 0; i < n; i++)
        {
            float t = (float)i / n;
            float noise = (float)(rng.NextDouble() * 2.0 - 1.0) * (1f - t);
            float thump = (float)Math.Sin(2f * Math.PI * 90f * i / Rate) * (1f - t) * 0.7f;
            d[i] = (noise * 0.5f + thump) * 0.8f;
        }
        Play(d);
    }

    public static void Victory()
    {
        if (!PlayFile("victory")) Play(Mix(Tone(523f, 0.12f, 523f), Tone(659f, 0.12f, 659f), Tone(784f, 0.2f, 784f)));
    }

    public static void Defeat()
    {
        if (!PlayFile("defeat")) Play(Mix(Tone(330f, 0.15f, 300f), Tone(262f, 0.15f, 240f), Tone(196f, 0.25f, 150f)));
    }
}
