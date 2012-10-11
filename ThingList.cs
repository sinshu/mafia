using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Mafia
{
    public class ThingList
    {
        private List<Thing> list;

        public ThingList()
        {
            list = new List<Thing>();
        }

        public List<Thing>.Enumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public void ForEach(Action<Thing> action)
        {
            list.ForEach(action);
        }

        public bool Has(Thing thing)
        {
            return list.IndexOf(thing) != -1;
        }

        public void AddThing(Thing thing)
        {
            list.Add(thing);
        }

        public void AddThingList(ThingList things)
        {
            foreach (Thing thing in things)
            {
                list.Add(thing);
            }
        }

        public void Clear()
        {
            list.Clear();
        }

        public void Initialize()
        {
            foreach (Thing thing in list)
            {
                thing.Initialize();
            }
        }

        public void BeforeTick()
        {
            foreach (Thing thing in list)
            {
                thing.BeforeTick();
            }
        }

        public void Tick(GameInput input)
        {
            foreach (Thing thing in list)
            {
                thing.Tick(input);
            }
            RemoveThings();
        }

        public void AfterTick()
        {
            foreach (Thing thing in list)
            {
                thing.AfterTick();
            }
        }

        public void Draw(MafiaVideo video, IntVector camera)
        {
            foreach (Thing thing in list)
            {
                thing.Draw(video, camera);
            }
        }

        public void PlaySound(MafiaSound sound, MafiaBufferContainer buffers)
        {
            foreach (Thing thing in list)
            {
                thing.PlaySound(sound, buffers);
            }
        }

        public void MoveHorizontalBy(double d)
        {
            foreach (Thing thing in list)
            {
                thing.MoveHorizontalBy(d);
            }
        }

        public void MoveVerticalBy(double d)
        {
            foreach (Thing thing in list)
            {
                thing.MoveVerticalBy(d);
            }
        }

        public void RemoveThings(Predicate<Thing> match)
        {
            list.RemoveAll(match);
        }

        private void RemoveThings()
        {
            list.RemoveAll(isRemoved);
        }

        private static bool isRemoved(Thing thing)
        {
            return thing.Removed;
        }

        public int Count
        {
            get
            {
                return list.Count;
            }
        }
    }
}
