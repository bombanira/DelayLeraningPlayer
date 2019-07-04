using System.Drawing;
using NAudio.Wave;
using System.Timers;
namespace Shadow_player_
{
    public class MusicClass
    {
        public  MusicWaveOut MusicWaveOut = new MusicWaveOut();
        private AudioFileReader musicAudio;
        public  LoopClass LoopClass = new LoopClass();
        public MusicMute MusicMute = new MusicMute();
        public ABRepeat abRepeat = new ABRepeat();
        private bool defaultLoop = false; 
        private float volume = 0.8f;
        // AutoDelay
        public int delayTime { get; private set; }
        public float delayRatio = 1.1f;
        public bool DelayStatus;
        public void SetDelayTime(long length)
        {
            delayTime = (int)(length / (musicAudio.WaveFormat.AverageBytesPerSecond / 1000) * delayRatio);
        }
        // AutoDelay
        public bool canUse { get; private set; } = false; 
        public bool isMute = false;
        public string fileName { get; private set; }
        public System.Windows.Forms.BindingSource DeviceWatchSource = new System.Windows.Forms.BindingSource();
        public long MusicPosition {
            get
            {
                return musicAudio.Position;
            }
            set
            {
                musicAudio.Position = value;
            }
        }
        public long MusicLength
        {
            get { return musicAudio.Length; }
        }
        public bool haveMusic
        {
            get
            {
                bool result;
                if (musicAudio == null)
                    result = false;
                else
                    result = true;
                return result;
            }
        }

        public float Volume
        {
            set
            {
                volume = value;
                if (musicAudio != null)
                    VolumeChange();
            }
        }
        public PlaybackState MusicState
        {
            get
            {
                return MusicWaveOut.PlaybackState;
            }
        }

        public MusicClass()
        {
            SetIsLoop(false);
        }

        public void Init(string fileName)
        {
            this.fileName = fileName;
            musicAudio = new AudioFileReader(fileName);
            LoopClass.Init(musicAudio);
            MusicWaveOut.Init(LoopClass.Loopstream);
            LoopClass.EnableLooping = defaultLoop;
            canUse = true;
            VolumeChange();
        }

        public bool GetIsLoop()
        {
            bool MusicClassHasMusic = (musicAudio != null);
            if (MusicClassHasMusic)
                return LoopClass.EnableLooping;
            else
                return defaultLoop;
        }
        public void SetIsLoop(bool status)
        {
            bool MusicClassHasMusic = (musicAudio != null);
            if (MusicClassHasMusic)
                LoopClass.EnableLooping = status;
            defaultLoop = status;
        }

        private void VolumeChange()
        {
            musicAudio.Volume = volume;
        }

        public void MusicPlay()
        {
            if(MusicWaveOut != null)
                MusicWaveOut.Play();
        }
        public void MusicPlay(long position)
        {
            if (MusicWaveOut != null)
            {
                musicAudio.Position = position;
                MusicWaveOut.Play();
            }
        }

        public void MusicStop()
        {
            MusicWaveOut.Stop();
        }
        public void MusicPouse()
        {
            MusicWaveOut.Pause();
        }
        public void MusicDispose()
        {
            MusicWaveOut.Dispose();
        }
    }

    public class MusicMute
    {
        private BoolImageStatus muteStatusData = new BoolImageStatus();
        public enum ImageName
        {
            MusicOffSoundImage = 0,
            MusicOnSoundImage = 1
        }
        public MusicMute()
        {
            muteStatusData.SetImage((int)ImageName.MusicOffSoundImage, MainShadow.Properties.Resources.MusicOffSound);
            muteStatusData.SetImage((int)ImageName.MusicOnSoundImage, MainShadow.Properties.Resources.MusicOnSound);
        }
        public Image GetImage(ImageName imageName)
        {
            return muteStatusData.GetImage((int)imageName);
        }
    }
    public class MusicWaveOut: WaveOut
    {
        private BoolImageStatus musicStatusData = new BoolImageStatus();
        public enum ImageName
        {
            PlayingImage = 0,
            PouseImage,
        }
        public MusicWaveOut() : base()
        {
            musicStatusData.SetImage((int)ImageName.PlayingImage, MainShadow.Properties.Resources.play);
            musicStatusData.SetImage((int)ImageName.PouseImage, MainShadow.Properties.Resources.pouse);
        }
        public Image GetImage(ImageName imageName)
        {
            return musicStatusData.GetImage((int)imageName);
        }
    }

    public class LoopClass
    {
        private BoolImageStatus loopStatusData = new BoolImageStatus();
        public LoopStream Loopstream { get; private set; }
        public enum ImageName
        {
            NoInfinitImage = 0,
            InfinitImage,
        }
        public LoopClass()
        {
            loopStatusData.SetImage((int)ImageName.NoInfinitImage, MainShadow.Properties.Resources.NoInfinit);
            loopStatusData.SetImage((int)ImageName.InfinitImage, MainShadow.Properties.Resources.infinit);
        }
        public void Init(WaveStream sourceStream)
        {
            Loopstream = new LoopStream(sourceStream);
        }

        public bool EnableLooping
        {
            get
            {
                return loopStatusData.Status;
            }
            set
            {
                loopStatusData.Status = value;
                Loopstream.EnableLooping = loopStatusData.Status;
            }
        }

        public Image GetImage(ImageName imageName)
        {
            return loopStatusData.GetImage((int)imageName);
        }

    }

    public class ABRepeat
    {
        public long APosition { get; set; } = 0;
        public long BPosition { get; set; }
        public BoolImageStatus ABRepeatStatusData = new BoolImageStatus();
        public long ABLength { get { return BPosition - APosition; } }
        public enum ImageName
        {
            ABRepeatImage = 0
        }

        public ABRepeat()
        {
            ABRepeatStatusData.SetImage((int)ImageName.ABRepeatImage, MainShadow.Properties.Resources.ABRepeat);
            ABRepeatStatusData.Status = false;
        }

    }

}

