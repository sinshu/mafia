using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using System;
using System.Windows.Forms;

namespace Mafia
{
    /// <summary>
    /// サウンドがどうのこうの。
    /// </summary>
    public class MafiaSound : IDisposable
    {
        private const int MAX_NUM_CHANNELS = 256;

        Device device = null;
        MafiaBufferContainer buffers;

        GameSoundChannel[] channels;
        int numChannels;

        int ticks;

        public MafiaSound(Form form)
        {
            try
            {
                device = new Device();
            }
            catch
            {
                return;
            }
            device.SetCooperativeLevel(form, CooperativeLevel.Priority);
            buffers = new MafiaBufferContainer(device);

            channels = new GameSoundChannel[MAX_NUM_CHANNELS];
            numChannels = 0;

            ticks = 0;
        }

        public void Play(SecondaryBuffer buffer, Thing thing)
        {
            // シュンスケ、nullかどうかチェックしなきゃならんとは何事だ
            if (device == null) return;
            if (numChannels == MAX_NUM_CHANNELS) return;
            channels[numChannels] = new GameSoundChannel(buffer.Clone(device), thing);
            channels[numChannels].Buffer.Pan = CalcPan(thing);
            channels[numChannels].Buffer.Volume = CalcVolume(thing);
            channels[numChannels].Buffer.Play(0, BufferPlayFlags.Default);
            numChannels++;
        }

        public void PlaySelectSound(SelectScene select)
        {
            if (device == null) return;
            select.PlaySound(this, buffers);
        }

        public void PlayGameSound(GameScene game)
        {
            if (device == null) return;

            ticks++;

            // 再生し終わったBufferをあぼーんする。
            for (int i = 0; i < numChannels; i++)
            {
                if (!channels[i].Buffer.Status.Playing)
                {
                    channels[i].Buffer.Dispose();
                    numChannels--;
                    for (int j = i; j < numChannels; j++)
                    {
                        channels[j] = channels[j + 1];
                    }
                }
            }

            if (ticks % 2 == 0)
            {
                for (int i = 0; i < numChannels; i++)
                {
                    channels[i].Buffer.Pan = CalcPan(channels[i].Thing);
                    channels[i].Buffer.Volume = CalcVolume(channels[i].Thing);
                }
            }

            game.ThingList.PlaySound(this, buffers);
        }

        public void StopSounds()
        {
            if (device == null) return;
            for (int i = 0; i < numChannels; i++)
            {
                channels[i].Buffer.Stop();
                channels[i].Buffer.Dispose();
            }
            numChannels = 0;
        }

        private int CalcPan(Thing thing)
        {
            if (thing == null)
            {
                return (int)Pan.Center;
            }
            double pan = (thing.CenterOnScreen.X - Mafia.SCREEN_WIDTH / 2) * 10000 / (Mafia.SCREEN_WIDTH * 4);
            if (Math.Abs(pan) > 10000)
            {
                pan = Math.Sign(pan) * 10000;
            }
            return (int)Math.Round(pan);
        }

        private int CalcVolume(Thing thing)
        {
            if (thing == null)
            {
                return (int)Volume.Max;
            }
            Vector v = thing.CenterOnScreen;
            if (0 < v.X && v.X < Mafia.SCREEN_WIDTH && 0 < v.Y && v.Y < Mafia.SCREEN_HEIGHT)
            {
                return 0;
            }

            double range1 = -v.X;
            double range2 = v.X - Mafia.SCREEN_WIDTH;
            double range3 = -v.Y;
            double range4 = v.Y - Mafia.SCREEN_HEIGHT;
            double range = Math.Max(Math.Max(range1, range2), Math.Max(range3, range4));

            int volume = (int)Math.Round(-range * 10000 / Mafia.SOUND_RANGE);
            if (volume < -10000) volume = -10000;
            if (volume > 0) volume = 0;

            return volume;
        }

        public void Dispose()
        {
            StopSounds();
            if (buffers != null)
            {
                buffers.Dispose();
                buffers = null;
            }
            if (device != null)
            {
                device.Dispose();
                device = null;
            }
        }
    }
}
