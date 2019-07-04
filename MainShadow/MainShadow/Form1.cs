using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using System.Timers;

namespace Shadow_player_
{
    public partial class ShadowPlayForm : Form
    {
        #region フィールド
        private MicClass micClass = new MicClass();
        private MusicClass musicClass = new MusicClass();
        private DeviceWaveOut deviceWaveOut = new DeviceWaveOut();
        private System.Windows.Forms.Timer MusicGetPositionTimer = new System.Windows.Forms.Timer();
        PlaybackState preState;

        #endregion
        public ShadowPlayForm()
        {
            InitializeComponent();
            InitializeSetting();
            /*
            MusicDataBinding.DataSource = new DataBindClass();
     
            MicDataBinding.DataSource = new DataBindClass();
            MicDelayBinding.DataSource = new DataBindClass();
            VolumeDataBinding.DataSource = new DataBindClass();
            */
            MusicVolumeChanger(80);
            MicVolumeChanger(80);
            AllVolumeChanger(80);
            //DelayTimeChanger(100);
        }

        private void InitializeSetting()
        {
            bool MicCanUse = WaveInEvent.DeviceCount != 0;
            if(MicCanUse)
                MicControlPicture.Image = MicClass.MicEnableImage;

            // 再生バーの設定
            MusicPositionBar.Maximum = 100;
            MusicPositionBar.MouseDown += delegate
            {
                preState = musicClass.MusicState;
                if (musicClass.MusicState is PlaybackState.Playing)
                    MusicPlayingMode();
                else
                    MusicPlayPicture.Image = musicClass.MusicWaveOut.GetImage(MusicWaveOut.ImageName.PlayingImage);
            };
            MusicPositionBar.MouseUp += delegate
            {
                if (preState == PlaybackState.Playing)
                    MusicPlayingMode();
                else
                    MusicStoppingMode();
            };
            MusicPositionBar.ValueChanged += delegate
            {
                if (MusicPositionBar.Focused)
                {
                    musicClass.MusicPosition = MusicPositionBar.Value;
                }
            };
            MusicGetPositionTimer.Tick += delegate
            {
                MusicPositionBar.Value = (int)musicClass.MusicPosition;
                if (musicClass.abRepeat.ABRepeatStatusData.Status)
                {
                    if (musicClass.MusicPosition > musicClass.abRepeat.BPosition)
                    {
                        musicClass.MusicPouse();
                        MusicGetPositionTimer.Stop();
                        //musicClass.abRepeat.ABRepeatStatusData.Status = false;
                        MusicPlayPicture.Image = musicClass.MusicWaveOut.GetImage(MusicWaveOut.ImageName.PlayingImage);
                    }
                }
                if (MusicPositionBar.Value == musicClass.MusicLength)
                    MusicStoppingMode();
            };
        }

        #region 音楽インポートボタン
        private void ImportMusicButton_Click(object sender, EventArgs e)
        {

        }
        #endregion
        #region 音楽再生ボタン
        private void MusicPlayPicture_Click(object sender, EventArgs e)
        {
            if(musicClass.canUse)
            {
                if (musicClass.MusicState is PlaybackState.Stopped || musicClass.MusicState is PlaybackState.Paused)
                {
                    MusicPlayingMode();
                }
                else if (musicClass.MusicState is PlaybackState.Playing)
                {
                    MusicStoppingMode();
                }
                musicClass.abRepeat.ABRepeatStatusData.Status = false;
            }
        }
        private void MusicPlayingMode()
        {
            MusicPlayPicture.Image = musicClass.MusicWaveOut.GetImage(MusicWaveOut.ImageName.PouseImage);
            musicClass.MusicPlay();
            MusicGetPositionTimer.Start();
            musicClass.abRepeat.APosition = musicClass.MusicPosition;
        }
        private void MusicStoppingMode()
        {
            MusicPlayPicture.Image = musicClass.MusicWaveOut.GetImage(MusicWaveOut.ImageName.PlayingImage);
            musicClass.MusicPouse();
            MusicGetPositionTimer.Stop();
            musicClass.abRepeat.BPosition = musicClass.MusicPosition;
            musicClass.SetDelayTime(musicClass.abRepeat.ABLength);
        }
        private void ABRepeatPicture_Click(object sender, EventArgs e)
        {
            if (musicClass.canUse)
            { 
                if(musicClass.abRepeat.ABRepeatStatusData.Status != true)
                    musicClass.abRepeat.BPosition = musicClass.MusicPosition;
                musicClass.abRepeat.ABRepeatStatusData.Status = true;
                MusicPlayPicture.Image = musicClass.MusicWaveOut.GetImage(MusicWaveOut.ImageName.PouseImage);
                MusicGetPositionTimer.Start();
                musicClass.MusicPlay(musicClass.abRepeat.APosition);
                musicClass.SetDelayTime(musicClass.abRepeat.ABLength);
                if (AutoDelayCheckBox.Checked)
                {
                    micClass.SetDelayMS(musicClass.delayTime);
                    MicDelayUpDown.Value = musicClass.delayTime;
                    TimeBar.Value = musicClass.delayTime;
                }
            }
        }
        #endregion
        private void InfinityPicture_Click(object sender, EventArgs e)
        {
            if(musicClass.canUse)
            {
                LoopClass.ImageName getImageName;
                if (musicClass.GetIsLoop())
                    getImageName = LoopClass.ImageName.NoInfinitImage;
                else
                    getImageName = LoopClass.ImageName.InfinitImage;

                InfinityPicture.Image = musicClass.LoopClass.GetImage(getImageName);
                musicClass.LoopClass.EnableLooping = !musicClass.LoopClass.EnableLooping;
            }
        }
        #region 音楽音量設定
        private void MusicVolumeBar_Scroll(object sender, EventArgs e)
        {
            MusicVolumeChanger(MusicVolumeBar.Value);
        }
        private void MusicSoundVolumeUpDown_ValueChanged(object sender, EventArgs e)
        {
            MusicVolumeChanger((int)MusicSoundVolumeUpDown.Value);
        }
        private void MusicVolumeChanger(int musicVolume)
        {
            MusicVolumeBar.Value = musicVolume;
            MusicSoundVolumeUpDown.Value = musicVolume;
            if (musicClass.isMute != true) // not mute
                musicClass.Volume = (float)musicVolume / 100;
        }
        private void SoundControlPicture_Click(object sender, EventArgs e)
        {
            MusicMute.ImageName getImageName = new MusicMute.ImageName();
            if (musicClass.isMute)
            {
                getImageName = MusicMute.ImageName.MusicOnSoundImage;
                musicClass.Volume = (float)MusicVolumeBar.Value / 100;
            }
            else
            {
                getImageName = MusicMute.ImageName.MusicOffSoundImage;
                musicClass.Volume = 0;
            }
            MusicSoundPicture.Image = musicClass.MusicMute.GetImage(getImageName);
            musicClass.isMute = !musicClass.isMute;

        }
        #endregion
        #region マイク音量設定
        private void MicBar_Scroll(object sender, EventArgs e)
        {
            MicVolumeChanger(MicBar.Value);

        }
        private void MicVolumeUpDown_ValueChanged(object sender, EventArgs e)
        {
            MicVolumeChanger((int)MicVolumeUpDown.Value);
        }

        private void MicVolumeChanger(int micVolume)
        {
            MicBar.Value = micVolume;
            MicVolumeUpDown.Value = micVolume;
            micClass.Volume = (float)micVolume / 100;
        }

        private void MicControlPicture_Click(object sender, EventArgs e)
        {
            if (micClass.isMute)
            {
                micClass.MicStart();
                MicControlPicture.Image = MicClass.MicEnableImage;
            }
            else
            {
                micClass.MicStop();
                MicControlPicture.Image = MicClass.MicDisableImage;
            }
            micClass.isMute = !micClass.isMute;

        }
        #endregion
        #region 出力デバイスの音量設定
        private void AllVolumeBar_Scroll(object sender, EventArgs e)
        {
            AllVolumeChanger(AllVolumeBar.Value);
        }
        private void AllVolumeUpDown1_ValueChanged(object sender, EventArgs e)
        {
            AllVolumeChanger((int)AllVolumeUpDown1.Value);
        }
        private void AllVolumeChanger(int allVolume)
        {
            if (deviceWaveOut.isMute != true) // not mute
                deviceWaveOut.Volume = (float)allVolume / 100;
            AllVolumeUpDown1.Value = allVolume;
            AllVolumeBar.Value = allVolume;
        }

        private void AllVolumePicture_Click(object sender, EventArgs e)
        {
            DeviceWaveOut.ImageName getImageName;
            if (deviceWaveOut.isMute)
            {
                getImageName = DeviceWaveOut.ImageName.DeviceVolumeOnImage;
                deviceWaveOut.Volume = (float)AllVolumeBar.Value / 100;
            }
            else
            {
                getImageName = DeviceWaveOut.ImageName.DeviceVolumeOffImage;
                deviceWaveOut.Volume = 0;
            }
            AllVolumePicture.Image = deviceWaveOut.GetImage(getImageName);
            deviceWaveOut.isMute = !deviceWaveOut.isMute;
        }
        #endregion
        #region 遅延設定
        private void TimeBar_Scroll(object sender, EventArgs e)
        {
            DelayTimeChanger(TimeBar.Value);
        }
        private void MicDelayUpDown_ValueChanged(object sender, EventArgs e)
        {
            DelayTimeChanger((int)MicDelayUpDown.Value);
        }

        #endregion
        private void MusicPositionBar_Scroll(object sender, EventArgs e) // mouse scroll, not value changed
        {
        }
        private void DelayTimeChanger(int MSecond)
        {
            TimeBar.Value = MSecond;
            MicDelayUpDown.Value = MSecond;
            micClass.SetDelayMS(MSecond);
        }
        #region tableLayout
        private void tableLayoutPanel5_Paint(object sender, PaintEventArgs e)
        {

        }
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }


        private void MusicTreeView_Click(object sender, EventArgs e)
        {

        }

        private void ShadowPlayForm_Load(object sender, EventArgs e)
        {

        }

        private void soundBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }


        #endregion

        private void MusicTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void 音楽ファイルToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openMusicEvent.ShowDialog() == DialogResult.OK)
            {

                string filename = Path.GetFileName(openMusicEvent.FileName);
                TreeNode musicFileTree = new TreeNode(filename);
                musicFileTree.Text = openMusicEvent.FileName;
                MusicTreeView.Nodes.Add(musicFileTree);
            }
        }

        private void MusicFolderImport_Click(object sender, EventArgs e)
        {

        }

        private void AutoDelayCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void MusicTreeView_Click(object sender, MouseEventArgs e)
        {

        }

        private void MusicPositionBar_Layout(object sender, LayoutEventArgs e)
        {

        }

        private void MusicTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (musicClass.haveMusic && musicClass.fileName == e.Node.Text)
                ;
            else
            {
                if (musicClass.haveMusic)
                    musicClass.MusicDispose();
                string fileNameExtend = Path.GetExtension(e.Node.Text);
                if (fileNameExtend == ".mp3")
                {
                    setupMusicPlay(e.Node.Text);
                }
            }
        }
        private void setupMusicPlay(string filePath)
        {
            musicClass.Init(filePath);
            MusicPositionBar.Maximum = (int)musicClass.MusicLength + MusicPositionBar.TickFrequency / 5;
            MusicGetPositionTimer.Interval = 100; 
            MusicPositionBar.Visible = true;
        }
    }
    public class DataBindClass
    {
        public DataBindClass()
        {
            this.subClass = new SubClass();
        }

        public string Tag { get; set; }
        public int Value { get; set; }

        public SubClass subClass { get; set; }

        public class SubClass
        {

        }
    }
}

