using System;
using System.Diagnostics;
using System.Threading;

namespace Mafia
{
    /// <summary>
    /// FPS�^�C�}�[�B
    /// </summary>
    /// <remarks>
    /// �P���Ȏ��������琂�������𕹗p���Ȃ�������޽�Ȃ��ƂɂȂ邪�C�ɂ���ȁB
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
        /// ��1/FPS�b�ԑ҂B
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

            // �o�ߎ��ԂƗ��z�I�ȕ`��^�C�~���O�Ƃ̃Y����1�b�ȏ�ɂȂ�����ُ펖�Ԃ����������Ƃ��ă^�C�}�[�����Z�b�g����B
            // �ł�FPS�̑��肪�܂��I����ĂȂ�������iWait()��fps��Ăяo���ĂȂ���΁j���Z�b�g���Ȃ��B
            if (!(elapsed - target < new TimeSpan(10000000L)) && !(numTicks < fps))
            {
                Reset();
            }
        }

        /// <summary>
        /// �`�悷�ׂ��^�C�~���O���ǂ������擾����B
        /// </summary>
        /// <returns>true�Ȃ�`�悵��Afalse�Ȃ炷�ȁB</returns>
        /// <remarks>
        /// allowFrameSkip��false�̂Ƃ��͏��true��Ԃ��B
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
        /// FPS�^�C�}�[�����Z�b�g����B
        /// </summary>
        /// <remarks>
        /// �t�@�C���ǂݍ��ݓ��̏d��������������͂�����Ă񂾂ق����ǂ��B
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
        /// ���݂�FPS���擾����B
        /// </summary>
        /// <returns>FPS�B</returns>
        public int GetCurrentFps()
        {
            return (int)Math.Round(realFps);
        }
    }
}
