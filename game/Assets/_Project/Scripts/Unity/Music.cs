// CrewJournal — música em loop via Resources/Audio/music_<nome>.
// Mesma chave de mute do Sfx (cj_mute). Sem arquivo = silêncio, sem erro.
using UnityEngine;

public static class Music
{
    const float Vol = 0.35f;
    static AudioSource src;
    static string current = "";

    static AudioSource Src()
    {
        if (src != null) return src;
        GameObject go = new GameObject("Music");
        Object.DontDestroyOnLoad(go);
        src = go.AddComponent<AudioSource>();
        src.loop = true;
        src.volume = Vol;
        src.mute = Sfx.muted;
        return src;
    }

    public static void ApplyMute()
    {
        if (src != null) src.mute = Sfx.muted;
    }

    static void PlayKey(string key)
    {
        if (current == key) return;
        AudioClip clip = Resources.Load<AudioClip>("Audio/" + key);
        if (clip == null) return;
        current = key;
        AudioSource s = Src();
        s.clip = clip;
        s.Play();
    }

    public static void PlayMap()
    {
        PlayKey("music_map");
    }

    public static void PlayCombat()
    {
        PlayKey("music_combat");
    }

    public static void Stop()
    {
        current = "";
        if (src != null) src.Stop();
    }

    public static bool HasMusic(string key)
    {
        return Resources.Load<AudioClip>("Audio/" + key) != null;
    }
}
