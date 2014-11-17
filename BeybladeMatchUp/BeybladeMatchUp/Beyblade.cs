using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BeybladeMatchUp
{
    class Beyblade
    {
                Texture2D animalTexture;
        byte animalType;
        byte mood, direction;
        int displacement;

        public int Displacement
        {
            get { return displacement; }
            set { displacement = value; }
        }

        public byte Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        bool active;

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        public byte Mood
        {
            get { return mood; }
            set { mood = value; }
        }

        //Constants make sure they are the same as games.cs

        const byte NONE = 0, RIGHT = 1, DOWN = 2, LEFT = 3, UP = 4, FALLING =5;
        const byte OK = 0, SLEEPY = 1, ANGRY = 2, SHOCK = 3;
        const byte MONKEY = 0, HIPPO = 1, LION = 2, PANDA = 3, ELEPHANT = 4, GIRAFE = 5, CROC = 6, BUNNY = 7;
        const int XOFFSET = 2, YOFFSET = 2, SPRITEOFFSET = 44, SPRITEWIDTH = 42, SPRITEBLOCKSIZE =168;
        
        public Beyblade(Texture2D animalsTexture, byte type)
        {
            animalType = type;
            animalTexture = animalsTexture;
            mood = OK;
            active = true;
        }

        public byte AnimalType
        {
            get { return animalType; }
            set { animalType = value; }
        }

        public void Draw(SpriteBatch spriteBacth,byte row, byte col)
        {
            if (active)
            {
                if (direction != NONE && displacement > 0)
                {
                    switch (direction)
                    {
                        case LEFT:
                            spriteBacth.Draw(animalTexture, new Rectangle(col * SPRITEOFFSET + XOFFSET + displacement * 2, row * SPRITEOFFSET + YOFFSET, SPRITEWIDTH, SPRITEWIDTH), new Rectangle(animalType * SPRITEBLOCKSIZE + (mood * SPRITEWIDTH), 0, SPRITEWIDTH, SPRITEWIDTH), Color.White);
                            break;
                        case RIGHT:
                            spriteBacth.Draw(animalTexture, new Rectangle(col * SPRITEOFFSET + XOFFSET - displacement * 2, row * SPRITEOFFSET + YOFFSET, SPRITEWIDTH, SPRITEWIDTH), new Rectangle(animalType * SPRITEBLOCKSIZE + (mood * SPRITEWIDTH), 0, SPRITEWIDTH, SPRITEWIDTH), Color.White);
                            break;
                        case UP:
                            spriteBacth.Draw(animalTexture, new Rectangle(col * SPRITEOFFSET + XOFFSET, row * SPRITEOFFSET + YOFFSET - displacement * 2, SPRITEWIDTH, SPRITEWIDTH), new Rectangle(animalType * SPRITEBLOCKSIZE + (mood * SPRITEWIDTH), 0, SPRITEWIDTH, SPRITEWIDTH), Color.White);
                            break;
                        case DOWN:
                            spriteBacth.Draw(animalTexture, new Rectangle(col * SPRITEOFFSET + XOFFSET, row * SPRITEOFFSET + YOFFSET + displacement * 2, SPRITEWIDTH, SPRITEWIDTH), new Rectangle(animalType * SPRITEBLOCKSIZE + (mood * SPRITEWIDTH), 0, SPRITEWIDTH, SPRITEWIDTH), Color.White);
                            break;
                        default:
                            break;
                    }
                    if (--displacement == 0)
                    {
                        direction = NONE;
                    }
                }
                else
                {
                    spriteBacth.Draw(animalTexture, new Rectangle(col * SPRITEOFFSET + XOFFSET, row * SPRITEOFFSET + YOFFSET, SPRITEWIDTH, SPRITEWIDTH), new Rectangle(animalType * SPRITEBLOCKSIZE + (mood * SPRITEWIDTH), 0, SPRITEWIDTH, SPRITEWIDTH), Color.White);
                } // end else
            } // end if
        } // end method

    }
    }

