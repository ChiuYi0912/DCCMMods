using System;
using ModCore.Modules;
using ModCore.Events.Interfaces;
using ChiuYiUI.Core;
using dc.tool;
using dc.hxd.res;
using dc.hxd.fs;
using System.Diagnostics;

namespace ChiuYiUI.GameMechanics;

public class LevelMusic
{
    private CHIUYIMain _me;
    public LevelMusic(CHIUYIMain cHIUYIMain)
    {
        _me = cHIUYIMain;
    }

    public void HookInitialize()
    {
        Hook__MusicManager.get += Hook__MusicManager_get;
    }

    private Sound Hook__MusicManager_get(Hook__MusicManager.orig_get orig, dc.String musicName, dc.String folder)
    {
        //_ = PlayNeteaseSongAsync("https://music.163.com/#/song?id=2161222939");

        return orig(musicName, folder);
    }

    private async Task PlayNeteaseSongAsync(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo(url)
            {
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
