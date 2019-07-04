using NAudio.Wave;
using System.Drawing;
namespace Shadow_player_
{
    internal class DeviceWaveOut
    {
        private WaveOut waveOut = new WaveOut();
        public BoolImageStatus DeviceImageStatus = new BoolImageStatus();
        public bool isMute = false;
        public enum ImageName
        {
            DeviceVolumeOnImage = 0,
            DeviceVolumeOffImage,
        }
        public DeviceWaveOut()
        {
            DeviceImageStatus.SetImage((int)ImageName.DeviceVolumeOnImage, MainShadow.Properties.Resources.sound);
            DeviceImageStatus.SetImage((int)ImageName.DeviceVolumeOffImage, MainShadow.Properties.Resources.nosound);
        }
        public float Volume
        {
            set
            {
                waveOut.Volume = value;
            }
        }
        public Image GetImage(ImageName imageName)
        {
            return DeviceImageStatus.GetImage((int)imageName);
        }
    }
}