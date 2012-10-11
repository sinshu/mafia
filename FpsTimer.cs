using System;
using System.Diagnostics;
using System.Threading;

namespace Mafia
{
    /// <summary>
    /// FPSタイマー。
    /// </summary>
    /// <remarks>
    /// 単純な実装だから垂直同期を併用しないとﾃﾗﾋﾄﾞｽなことになるが気にするな。
    /// </remarks>
    public class FpsTimer
    {
        private int fps;
        private bool allowFrameSkip;
        private int numTicks;
        private double realFps;
        private int numFrames;
        private bool shouldRender;
        private int numSkippedFrames;
        private TimeSpan measureStart;
        private Stopwatch stopwatch;

        public FpsTimer(int fps, bool allowFrameSkip)
        {
            this.fps = fps;
            this.allowFrameSkip = allowFrameSkip;
            numTicks = 0;
            realFps = -1;
            numFrames = 0;
            shouldRender = true;
            numSkippedFrames = 0;
            measureStart = new TimeSpan(0L);
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        /// <summary>
        /// 約1/FPS秒間待つ。
        /// </summary>
        public void Wait()
        {
            numTicks++;
            TimeSpan elapsed = stopwatch.Elapsed;
            TimeSpan target = new TimeSpan(10000000L * numTicks / fps);
            if (numTicks % fps == 0)
            {
                realFps = (double)numFrames / (elapsed - measureStart).TotalSeconds;
                numFrames = 0;
                measureStart = elapsed;
            }
            if (elapsed < target)
            {
                shouldRender = true;
                Thread.Sleep(target - elapsed);
            }
            else
            {
                shouldRender = false;
            }

            // 経過時間と理想的な描画タイミングとのズレが1秒以上になったら異常事態が発生したとしてタイマーをリセットする。
            // でもFPSの測定がまだ終わってなかったら（Wait()をfps回呼び出してなければ）リセットしない。
            if (!(elapsed - target < new TimeSpan(10000000L)) && !(numTicks < fps))
            {
                Reset();
            }
        }

        /// <summary>
        /// 描画すべきタイミングかどうかを取得する。
        /// </summary>
        /// <returns>trueなら描画しれ、falseならすな。</returns>
        /// <remarks>
        /// allowFrameSkipがfalseのときは常にtrueを返す。
        /// </remarks>
        public bool ShouldRender()
        {
            if (!allowFrameSkip || shouldRender)
            {
                numFrames++;
                numSkippedFrames = 0;
                return true;
            }
            else if (numSkippedFrames < 4)
            {
                numSkippedFrames++;
                return false;
            }
            numFrames++;
            numSkippedFrames = 0;
            return true;
        }

        /// <summary>
        /// FPSタイマーをリセットする。
        /// </summary>
        /// <remarks>
        /// ファイル読み込み等の重い処理をした後はこれを呼んだほうが良い。
        /// </remarks>
        public void Reset()
        {
            numTicks = 0;
            // realFps = -1;
            numFrames = 0;
            shouldRender = true;
            numSkippedFrames = 0;
            measureStart = new TimeSpan(0L);
            stopwatch.Reset();
            stopwatch.Start();
        }

        /// <summary>
        /// 現在のFPSを取得する。
        /// </summary>
        /// <returns>FPS。</returns>
        public int GetCurrentFps()
        {
            return (int)Math.Round(realFps);
        }
    }
}
