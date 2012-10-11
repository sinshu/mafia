using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using System;

namespace Mafia
{
    public struct GameSoundChannel
    {
        public SecondaryBuffer Buffer;
        public Thing Thing;

        public GameSoundChannel(SecondaryBuffer buffer, Thing thing)
        {
            Buffer = buffer;
            Thing = thing;
        }
    }
}
