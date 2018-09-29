using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace visual_game
{
    public class cardView
    {
        Bitmap face;
        Bitmap back;
        public bool flip;
        public PictureBox display;
        public string ID;
        public cardView()
        {
            face = null;
            back = null;
            ID = "--";
            display = null;
            flip = true;
        }
        public cardView(string id, PictureBox pictureBox, Bitmap cardFace, Bitmap cardBack)
        {
            ID = id;
            display = pictureBox;
            face = cardFace;
            back = cardBack;
            flip = true;
            display.Image = face;
        }

        public void flipCard()
        {
            if(flip)
            {
                display.Image = back;
                flip = false;
            }
            else
            {
                display.Image = face;
                flip = true;
            }
        }
        public override string ToString()
        {
            return ID;
        }

    }
}
