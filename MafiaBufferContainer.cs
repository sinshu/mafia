using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using System;

namespace Mafia
{
    public class MafiaBufferContainer
    {
        public SecondaryBuffer Jump;
        public SecondaryBuffer Tiun;
        public SecondaryBuffer Coin;
        public SecondaryBuffer Switch;
        public SecondaryBuffer BoxMove;
        public SecondaryBuffer BoxFall;
        public SecondaryBuffer Lift;
        public SecondaryBuffer Spring;
        public SecondaryBuffer Fall;
        public SecondaryBuffer Ue;
        public SecondaryBuffer Hyakuyen;
        public SecondaryBuffer Stiana;

        public MafiaBufferContainer(Device device)
        {
            MafiaLoader loader = MafiaLoader.DefaultLoader;

            Jump = loader.GetBuffer(device, "jump.wav");
            Tiun = loader.GetBuffer(device, "tiun.wav");
            Coin = loader.GetBuffer(device, "coin.wav");
            Switch = loader.GetBuffer(device, "switch.wav");
            BoxMove = loader.GetBuffer(device, "boxmove.wav");
            BoxFall = loader.GetBuffer(device, "boxfall.wav");
            Lift = loader.GetBuffer(device, "lift.wav");
            Spring = loader.GetBuffer(device, "spring.wav");
            Fall = loader.GetBuffer(device, "fall.wav");
            Ue = loader.GetBuffer(device, "ue.wav");
            Hyakuyen = loader.GetBuffer(device, "hyakuyen.wav");
            Stiana = loader.GetBuffer(device, "stiana.wav");
        }

        public void Dispose()
        {
            if (Jump != null)
            {
                Jump.Dispose();
                Jump = null;
            }
            if (Tiun != null)
            {
                Tiun.Dispose();
                Tiun = null;
            }
            if (Coin != null)
            {
                Coin.Dispose();
                Coin = null;
            }
            if (Switch != null)
            {
                Switch.Dispose();
                Switch = null;
            }
            if (BoxMove != null)
            {
                BoxMove.Dispose();
                BoxMove = null;
            }
            if (BoxFall != null)
            {
                BoxFall.Dispose();
                BoxFall = null;
            }
            if (Lift != null)
            {
                Lift.Dispose();
                Lift = null;
            }
            if (Spring != null)
            {
                Spring.Dispose();
                Spring = null;
            }
            if (Fall != null)
            {
                Fall.Dispose();
                Fall = null;
            }
            if (Ue != null)
            {
                Ue.Dispose();
                Ue = null;
            }
            if (Hyakuyen != null)
            {
                Hyakuyen.Dispose();
                Hyakuyen = null;
            }
            if (Stiana != null)
            {
                Stiana.Dispose();
                Stiana = null;
            }
        }
    }
}
