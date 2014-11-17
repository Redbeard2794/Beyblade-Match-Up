using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace BeybladeMatchUp
{
    class Player
    {
        int score;
        int timer;
        int hints;
        string name;

        public int Score
        {
            get { return score; }
            set { score = value; }
        }
        public int Timer
        {
            get { return timer; }
            set { timer = value; }
        }
        public int Hints
        {
            get { return hints; }
            set { hints = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Player()
        {
            name = "player";
            score = 0;
            timer = 100;
            hints = 3;
        }
        public void IncreaseScore()
        {
            score = score + 5;
        }
        public void DecreaseTimer()
        {
            timer = timer - 1;
        }
        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.DrawString(font, score.ToString(), new Vector2(400, 40), Color.Blue);
            if (timer > 1)
                spriteBatch.DrawString(font, timer.ToString() + " secs", new Vector2(395, 120), Color.MidnightBlue);

            else spriteBatch.DrawString(font, timer.ToString() + " sec", new Vector2(395, 120), Color.MidnightBlue);

            spriteBatch.DrawString(font, hints.ToString(), new Vector2(400, 230), Color.Red);
        }
    }
}
