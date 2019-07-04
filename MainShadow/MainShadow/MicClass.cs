using NAudio.Wave;
using System.Drawing;
using NAudio.Wave.SampleProviders;
namespace Shadow_player_
{
    internal class MicClass
    {
        private WaveInEvent recorder;
        private BufferedWaveProvider bufferedWaveProvider;
        private SavingWaveProvider savingWaveProvider;
        private WaveOut MicPlayer = new WaveOut();
        public bool isMute = false;
        public static Image MicEnableImage = MainShadow.Properties.Resources.mic;
        public static Image MicDisableImage = MainShadow.Properties.Resources.nonmic;
        private float volume;



        public MicClass()
        {
            recorder = new WaveInEvent();
            recorder.DataAvailable += RecorderOnDataAvailable;
            bufferedWaveProvider = new BufferedWaveProvider(recorder.WaveFormat);
            savingWaveProvider = new SavingWaveProvider(bufferedWaveProvider, "temp.wav");
            MicPlayer.Init(savingWaveProvider);
            MicStart();
        }
        public float Volume
        {
            get
            {
                return this.volume;
            }
            set
            {
                volume = value;
                savingWaveProvider.Volume = volume;
            }
        }
        private void RecorderOnDataAvailable(object sender, WaveInEventArgs waveInEventArgs)
        {
            bufferedWaveProvider.AddSamples(waveInEventArgs.Buffer, 0, waveInEventArgs.BytesRecorded);
        }

        // todo
        public void MicStart()
        { 
            if (WaveInEvent.DeviceCount != 0)
            {
                MicPlayer.Play();
                recorder.StartRecording();
            }

        }
        public void MicStop()
        {
            recorder.StopRecording();
            MicPlayer.Stop();
            savingWaveProvider.Dispose();
        }
        public void SetDelayMS(int DelayMSecond)
        {
            recorder.StopRecording();
            MicPlayer.Dispose();
            if (DelayMSecond > 11000)
                DelayMSecond = 11000;
            MicPlayer.DesiredLatency = DelayMSecond;
            MicPlayer.Init(savingWaveProvider);
            MicStart();
        }
    }



}