
using System.Drawing;
using System.Collections.Generic;
namespace Shadow_player_
{
    public class BoolImageStatus
    {
        private List<Image> Images = new List<Image>();
        public bool Status { get; set; }
        public Image GetImage(int num)
        {
            return Images[num];
        }
        public void SetImage(int num, Image image)
        {
            Images.Insert(num, image);
        }

        public void SetImages(List<int> nums, List<Image> images)
        {
            foreach (int i in nums)
            {
                Images.Insert(i, images[i]);
            }
        }
    }
}