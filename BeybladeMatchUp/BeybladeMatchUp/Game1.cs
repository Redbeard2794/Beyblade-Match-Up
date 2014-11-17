using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System.Windows;

namespace BeybladeMatchUp
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int ScreenWidth = 480;
        int ScreenHeight = 800;

        Texture2D animalsTexture, highLightTexture, elephantTexture, hourglassTexture1, hourglassTexture2, hourglassTexture3, hourglassTexture4, gameoverTexture, insertCoinTexture1, insertCoinTexture2, insertCoinTexture3, insertCoinTexture4, insertCoinTexture5, levelUpTexture, winTexture;

        SoundEffect backgroundMusic, beepSound, beeping, levelPlus, timeRunningOut;
        SoundEffectInstance backgroundMusicInstance, beepSoundInstance, beepingInstance, levelPlusInstance, timeRunningOutInstance;

        

        MouseState mouseState, previousMouseState;
        SpriteFont font;
        Player playerScore;
        Beyblade[,] theBoard;
        const int MaxRows = 8;
        const int MaxCols = 8;
        Random random = new Random();

        const byte SPLASH = 0, GAME = 1, PAUSE = 2, END = 3, Instructions = 4, levelUp = 5, Win = 6;//, LeaderBoard = 7; // used as gameMode values
        const byte MONKEY = 0, HIPPO = 1, LION = 2, PANDA = 3, ELEPHANT = 4, GIRAFE = 5, CROC = 6, BUNNY = 7; // used as animal type values
        const byte OK = 0, SLEEPY = 1, ANGRY = 2, SHOCK = 3; // used as animal mood values and offset in animal sprite sheet
        const byte NONE = 0, RIGHT = 1, DOWN = 2, LEFT = 3, UP = 4, FALLING = 5; // used to control animation for moving animals
        const int XOFFSET = 2, YOFFSET = 2, SPRITEOFFSET = 44, SPRITEWIDTH = 42, XMARGIN = 6; // used to control the layout the grid and drawing of animals

        bool selected = false;

        int noOfAnimals = 5;//6
        int currentX = -2, currentY = -2; // -2 used so that next valid animal won't be adjacent and trigger swap
        int gameMode = SPLASH;          // used in switch in update and draw      
        int splashTiming = 0;   // used to control animation of elephant
        int lastX1, lastX2, lastY1, lastY2; // store last animals swapped in case of no match
        int swapAnimationTime = 0; // used to  control the swapping animation

        string aMessage = "";//used to give a visual warning that time is running out
        int timing = 0;
        KeyboardState currentKeyboardState, previousKeyBoardState;//stores previous and current keyboard state
        bool match = false;//used to check whether there is a match or not
        int hourglassCounter = 0;//used to control the hourglass animation
        int coinTimer = 0;//used to control the sequence of images on the end screen
        int coinTiming = 0;//used to control the sequence of images on the end screen
        int levelTracker = 1;//used to store which level the player is on
        bool hintsActive = false;//used to check if the player currently has hints showing
        int hintTimer = 0;//used to reset shocked animals,i.e ones in possible matches to ok
        bool noMatch = false;//used to stop a move if there is no possible match
        const int noOfPlayers = 2;//max amount of players that can be stored in the leaderboard   (10)
        Player[] ScoreBoard = new Player[noOfPlayers];//array that stores the players details
        int noOfScores = 0;//used to determine which cell in the array is being used

        string name1;
        int score1;

        TouchCollection currentTouchCollection = new TouchCollection();
        TouchCollection previousTouchCollection;

        List<Vector2> clicks = new List<Vector2>();

        //timer for stopping double click]
        int doubleClickTimer;

        bool pauseMediaPlayer = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            theBoard = new Beyblade[MaxRows, MaxCols];
            playerScore = new Player();
            ScoreBoard[noOfScores] = playerScore;

            doubleClickTimer = 0;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("scorefont");

            animalsTexture = Content.Load<Texture2D>("ZooAnimals_strip42");
            highLightTexture = Content.Load<Texture2D>("highlight");
            elephantTexture = Content.Load<Texture2D>("elephantstage");
            hourglassTexture1 = Content.Load<Texture2D>("hourglass1");
            hourglassTexture2 = Content.Load<Texture2D>("hourglass2");
            hourglassTexture3 = Content.Load<Texture2D>("hourglass3");
            hourglassTexture4 = Content.Load<Texture2D>("hourglass4");
            gameoverTexture = Content.Load<Texture2D>("gameover");
            insertCoinTexture1 = Content.Load<Texture2D>("coin1");
            insertCoinTexture2 = Content.Load<Texture2D>("coin2");
            insertCoinTexture3 = Content.Load<Texture2D>("coin3");
            insertCoinTexture4 = Content.Load<Texture2D>("coin4");
            insertCoinTexture5 = Content.Load<Texture2D>("coin5");
            levelUpTexture = Content.Load<Texture2D>("levelup");
            winTexture = Content.Load<Texture2D>("win");

            backgroundMusic = Content.Load<SoundEffect>("beybladeSong");//wahwah
            backgroundMusicInstance = backgroundMusic.CreateInstance();
            beeping = Content.Load<SoundEffect>("sfx_beep");
            beepingInstance = beeping.CreateInstance();
            levelPlus = Content.Load<SoundEffect>("Wave2");
            levelPlusInstance = levelPlus.CreateInstance();
            timeRunningOut = Content.Load<SoundEffect>("timeRunningOut");
            timeRunningOutInstance = timeRunningOut.CreateInstance();


            beepSound = Content.Load<SoundEffect>("beep");
            beepSoundInstance = beepSound.CreateInstance();

            if (IsMediaPlayerBusy() == true)
            {
                backgroundMusicInstance.IsLooped = true;
                backgroundMusicInstance.Play();
            }
            //sets up the board at the start of the game
            for (int row = 0; row < MaxRows; row++)
            {
                for (int col = 0; col < MaxCols; col++)
                {
                    theBoard[row, col] = new Beyblade(animalsTexture, (byte)random.Next(noOfAnimals));
                }     // end inner for             
            } // end outer for  
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            previousTouchCollection = currentTouchCollection;
            currentTouchCollection = TouchPanel.GetState();

            doubleClickTimer++;
            if (doubleClickTimer > 2)
                doubleClickTimer = 0;

            switch (gameMode)
            {
                //shows the instructions for the game
                case Instructions:
                    InstructionsUpdate();
                    break;

                //if game is running
                case GAME:
                    GameUpdate(gameTime);
                    break;

                //when you complete a level
                case levelUp:
                    LevelUpUpdate();
                    break;

                //if game is over
                case END:
                    EndUpdate();
                    break;
                case Win:
                    WinUpdate();
                    break;

                //if elephant animation is running
                case SPLASH:
                    SplashUpdate();
                    break;

                //if game is paused
                case PAUSE:
                    previousKeyBoardState = currentKeyboardState;
                    currentKeyboardState = Keyboard.GetState();
                    PauseGame();
                    break;
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private void SplashUpdate()
        {
            splashTiming++;
            if (AnyClick() == true && doubleClickTimer ==0)
            {
                gameMode = Instructions;
                
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
        }

        private void InstructionsUpdate()
        {
            if (AnyClick() == true && doubleClickTimer == 0)
            {
                gameMode = GAME;
                
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                gameMode = SPLASH;
        }
        private void GameUpdate(GameTime gameTime)
        {
            //checkMouseInput();
            getClick(gameTime);
            if (swapAnimationTime > 0)
                swapAnimationTime--;

            //calls the method to check if there is a match of 3
            Match3();
            //for keeping track of the timer in the game
            //method for timer
            timing++;
            Timer();
            //calls the method to keep track of what level the player is on
            LevelTracking();


            //to pause the game
            //previousKeyBoardState = currentKeyboardState;
            //currentKeyboardState = Keyboard.GetState();
            //if (previousKeyBoardState.IsKeyDown(Keys.P) && currentKeyboardState.IsKeyUp(Keys.P))
            //{
            //    gameMode = PAUSE;
            //    beeping.Play();
            //}

            Hints();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                gameMode = Instructions;
        }
        private void LevelUpUpdate()
        {
            if (AnyClick() == true && doubleClickTimer == 0)
            {
                gameMode = GAME;
                playerScore.Timer = 100;
                playerScore.Hints = 3;
                levelTracker = levelTracker + 1;
                hintTimer = 0;
                for (int row = 0; row < MaxRows; row++)
                {
                    for (int col = 0; col < MaxCols; col++)
                    {
                        theBoard[row, col] = new Beyblade(animalsTexture, (byte)random.Next(noOfAnimals));
                    }     // end inner for             
                } // end outer for      
            }
        }
        private void EndUpdate()
        {
            coinTiming++;
            if (coinTiming == 30)
            {
                coinTimer++;
                coinTiming = 0;
            }
            if (AnyClick() == true && doubleClickTimer == 0)
            {
                gameMode = SPLASH;
                for (int row = 0; row < MaxRows; row++)
                {
                    for (int col = 0; col < MaxCols; col++)
                    {
                        theBoard[row, col] = new Beyblade(animalsTexture, (byte)random.Next(noOfAnimals));
                    }     // end inner for             
                } // end outer for      
                levelTracker = 1;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
        }
        private void WinUpdate()
        {
            if (AnyClick() == true && doubleClickTimer == 0)
            {
                gameMode = SPLASH;
                playerScore.Score = 0;
                playerScore.Timer = 100;
                playerScore.Hints = 3;
                levelTracker = 1;
                noOfScores++;
                noOfAnimals = 5;
                for (int row = 0; row < MaxRows; row++)
                {
                    for (int col = 0; col < MaxCols; col++)
                    {
                        theBoard[row, col] = new Beyblade(animalsTexture, (byte)random.Next(noOfAnimals));
                    }     // end inner for             
                } // end outer for 
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
        }
        void getClick(GameTime gameTime)
        {
            clicks.Clear();
            foreach (TouchLocation tl in currentTouchCollection)
            {
                if (tl.State == TouchLocationState.Pressed)
                    clicks.Add(new Vector2(tl.Position.X, tl.Position.Y));

                int newX = -1, newY = -1;

                newY = ((int)tl.Position.X - XOFFSET) / SPRITEOFFSET;
                newX = ((int)tl.Position.Y - YOFFSET) / SPRITEOFFSET;
                if (newX >= 0 && newX < 8)
                    if (newY >= 0 && newY < 8)
                    {
                        if (selected && newX == currentX && newY == currentY)
                        {
                            selected = false;
                            currentX = -2;
                            currentY = -2;
                        }
                        else
                        {
                            if ((Math.Abs(currentX - newX) == 1 && currentY == newY) || (Math.Abs(currentY - newY) == 1 && currentX == newX))
                            {
                                swap(currentX, currentY, newX, newY);
                                selected = false;
                                currentX = -2;
                                currentY = -2;// Make sure that next square clicked on is not adjacent to current  based on if statement above                             
                            }
                            else
                            {
                                if (selected)
                                {
                                    theBoard[currentX, currentY].Mood = OK;
                                }
                                currentX = newX;
                                currentY = newY;
                                theBoard[currentX, currentY].Mood = SLEEPY;
                                selected = true;
                            } // end else
                        } // end else
                    } // end if

            }
        }

        private bool AnyClick()
        {
            bool aPress = false;
            aPress = (currentTouchCollection.Count > 0);
            clicks.Clear();
            return aPress;
        }

        private void Hints()
        {
            //clicks.Clear();
            foreach (TouchLocation tl in currentTouchCollection)
            {
                if (tl.State == TouchLocationState.Pressed)
                    clicks.Add(new Vector2(tl.Position.X, tl.Position.Y));

                if (tl.Position.X > 370 && tl.Position.X < 420)
                {
                    if (tl.Position.Y > 230 && tl.Position.Y < 240)
                    {
                        ShowHints();
                    }
                }
            }
        }

        private void PauseGame()
        {
            //saves the score,timer and hints
            int pauseTimer = playerScore.Timer;
            int pauseScore = playerScore.Score;
            int pauseHints = playerScore.Hints;
            if (previousKeyBoardState.IsKeyDown(Keys.P) && currentKeyboardState.IsKeyUp(Keys.P))
            {
                beeping.Play();
                gameMode = GAME;
                //gives the player back the time,score and hints they had before they paused the game
                playerScore.Timer = pauseTimer;
                playerScore.Score = pauseScore;
                playerScore.Hints = pauseHints;
            }
        }
        private void ShowHints()
        {
            //if (previousKeyBoardState.IsKeyDown(Keys.H) && currentKeyboardState.IsKeyUp(Keys.H))
            //{
                if (playerScore.Hints > 0)
                {

                    playerScore.Hints -= 1;
                    HorizontalHints();//calls horizontal hints method
                    VerticalHints();//calls vertical hints method
                }
                else if (playerScore.Hints == 0)
                {
                    hintsActive = false;
                }
           // }
            if (hintsActive == true)
            {
                //sets animals in shock i.e any in a possible match back to ok after 5 seconds
                if (hintTimer >= 5)
                {
                    for (int cols = 0; cols < MaxCols; cols++)
                    {
                        for (int rows = 0; rows < MaxRows; rows++)
                        {
                            theBoard[rows, cols].Mood = OK;
                        }
                    }
                    hintsActive = false;
                    hintTimer = 0;
                }
            }
        }

        private void reverse()
        {
            beepSoundInstance.Play();
            swap(lastX1, lastY1, lastX2, lastY2);
        }


        private void swap(int currentX, int currentY, int newX, int newY)
        {
            Beyblade tempAnimal;

            //used later on to reverse last swap if no match occours
            lastX1 = currentX;
            lastX2 = newX;
            lastY1 = currentY;
            lastY2 = newY;
            theBoard[newX, newY].Mood = OK;
            theBoard[currentX, currentY].Mood = OK;
            if (newX < currentX)
            {
                theBoard[newX, newY].Direction = UP;
                theBoard[currentX, currentY].Direction = DOWN;

            }
            if (newX > currentX)
            {
                theBoard[newX, newY].Direction = DOWN;
                theBoard[currentX, currentY].Direction = UP;
            }
            if (newY > currentY)
            {
                theBoard[newX, newY].Direction = LEFT;
                theBoard[currentX, currentY].Direction = RIGHT;
            }
            if (newY < currentY)
            {
                theBoard[newX, newY].Direction = RIGHT;
                theBoard[currentX, currentY].Direction = LEFT;
            }
            tempAnimal = theBoard[currentX, currentY];
            theBoard[currentX, currentY].Displacement = 22; // 22 update ticks used to animate the swap movement.
            theBoard[currentX, currentY] = theBoard[newX, newY];
            theBoard[currentX, currentY].Displacement = 22;
            theBoard[newX, newY] = tempAnimal;            
        }

        private void Match3()
        {
            //when it selects an animal it checks the next 2 animals, hence the for loops are allowed to run 6 times as there are 8 animals in each row
            //checks horizontal matches
            for (int rows = 0; rows < MaxRows; rows++)
            {
                for (int cols = 0; cols < 6; cols++)
                {
                    if (theBoard[rows, cols].AnimalType == theBoard[rows, cols + 1].AnimalType && theBoard[rows, cols].AnimalType == theBoard[rows, cols + 2].AnimalType)
                    {
                        playerScore.IncreaseScore();//increases score
                        match = true;
                        if (match == true)
                        {
                            theBoard[rows, cols].AnimalType = (byte)random.Next(noOfAnimals);//changes animal to a new animal
                            RemoveTilesHorizontal(rows, cols);//calls the remove method
                            theBoard[rows, cols + 1].AnimalType = (byte)random.Next(noOfAnimals);
                            RemoveTilesHorizontal(rows, cols + 1);
                            theBoard[rows, cols + 2].AnimalType = (byte)random.Next(noOfAnimals);
                            RemoveTilesHorizontal(rows, cols + 2);
                        }//end inner if
                    }//end outer if
                    else match = false;
                    if (match == false)
                    {
                        noMatch = true;
                        //reverse(rows,cols,rows,cols+1);
                    }
                }//end inner for
            }//end outer for
            //checks vertical matches
            for (int cols = 0; cols < MaxCols; cols++)
            {
                for (int rows = 0; rows < 6; rows++)
                {
                    if (theBoard[rows, cols].AnimalType == theBoard[rows + 1, cols].AnimalType && theBoard[rows, cols].AnimalType == theBoard[rows + 2, cols].AnimalType)
                    {
                        playerScore.IncreaseScore();//increases score
                        match = true;
                        if (match == true)
                        {
                            theBoard[rows, cols].AnimalType = (byte)random.Next(noOfAnimals);//changes animal to a new animal
                            RemoveTilesVertical(rows, cols);//calls the remove method
                            theBoard[rows + 1, cols].AnimalType = (byte)random.Next(noOfAnimals);
                            RemoveTilesVertical(rows + 1, cols);
                            theBoard[rows + 2, cols].AnimalType = (byte)random.Next(noOfAnimals);
                            RemoveTilesVertical(rows + 2, cols);
                        }//end inner if
                    }//end outer if
                    else match = false;
                    if (match == false)
                        noMatch = true;
                }//end inner for
            }//end outer for
        }

        private void RemoveTilesHorizontal(int row, int col)
        {
            for (int row1 = row; row > 0; row--)
            {
                swap(row, col, row - 1, col);
            }
        }

        private void RemoveTilesVertical(int row, int col)
        {
            for (int row1 = row; row > 0; row--)
            {
                swap(row, col, row - 1, col);
            }
        }

        private void HorizontalHints()
        {
            //first possibilty = [rows,cols] [rows,cols+1] [rows-1,cols+2]
            //second possibilty = [rows,cols] [rows,cols+1] [rows+1,cols+2]
            //third possibilty = [rows,cols] [rows-1,cols+1] [rows,cols+2]
            //fourth possibilty = [rows,cols] [rows+1,cols+1] [rows,cols+2]
            //fifth possibilty = [rows,cols] [rows+1,cols+1] [rows+1,cols+2]
            //sixth possibilty = [rows,cols] [rows-1,cols+1] [rows-1,cols+2]
            //seventh possibilty = [rows,cols] [rows,cols+2] [rows,cols+3]
            //eighth possibilty = [rows,cols] [rows,cols+1] [rows,cols+3]
            hintsActive = true;
            for (int rows = 0; rows < MaxRows; rows++)
            {
                for (int cols = 0; cols < 6; cols++)
                {

                    if (rows > 0 && cols < 6)
                    {
                        //first possibility
                        if (theBoard[rows, cols].AnimalType == theBoard[rows, cols + 1].AnimalType && theBoard[rows, cols].AnimalType == theBoard[rows - 1, cols + 2].AnimalType)
                        {
                            theBoard[rows, cols].Mood = SHOCK;
                            theBoard[rows, cols + 1].Mood = SHOCK;
                            theBoard[rows - 1, cols + 2].Mood = SHOCK;
                        }
                        //third possibility
                        if (theBoard[rows, cols].AnimalType == theBoard[rows - 1, cols + 1].AnimalType && theBoard[rows, cols].AnimalType == theBoard[rows, cols + 2].AnimalType)
                        {
                            theBoard[rows, cols].Mood = SHOCK;
                            theBoard[rows - 1, cols + 1].Mood = SHOCK;
                            theBoard[rows, cols + 2].Mood = SHOCK;
                        }
                        //sixth possibility
                        if (theBoard[rows, cols].AnimalType == theBoard[rows - 1, cols + 1].AnimalType && theBoard[rows, cols].AnimalType == theBoard[rows - 1, cols + 2].AnimalType)
                        {
                            theBoard[rows, cols].Mood = SHOCK;
                            theBoard[rows - 1, cols + 1].Mood = SHOCK;
                            theBoard[rows - 1, cols + 2].Mood = SHOCK;
                        }
                    }
                    if (rows < 7 && cols < 7)
                    {
                        //second possibility
                        if (theBoard[rows, cols].AnimalType == theBoard[rows, cols + 1].AnimalType && theBoard[rows, cols].AnimalType == theBoard[rows + 1, cols + 2].AnimalType)
                        {
                            theBoard[rows, cols].Mood = SHOCK;
                            theBoard[rows, cols + 1].Mood = SHOCK;
                            theBoard[rows + 1, cols + 2].Mood = SHOCK;
                        }
                        //fourth possibility
                        if (theBoard[rows, cols].AnimalType == theBoard[rows + 1, cols + 1].AnimalType && theBoard[rows, cols].AnimalType == theBoard[rows, cols + 2].AnimalType)
                        {
                            theBoard[rows, cols].Mood = SHOCK;
                            theBoard[rows + 1, cols + 1].Mood = SHOCK;
                            theBoard[rows, cols + 2].Mood = SHOCK;
                        }
                        //fifth possibility
                        if (theBoard[rows, cols].AnimalType == theBoard[rows + 1, cols + 1].AnimalType && theBoard[rows, cols].AnimalType == theBoard[rows + 1, cols + 2].AnimalType)
                        {
                            theBoard[rows, cols].Mood = SHOCK;
                            theBoard[rows + 1, cols + 1].Mood = SHOCK;
                            theBoard[rows + 1, cols + 2].Mood = SHOCK;
                        }
                    }
                    if (cols < 5)
                    {
                        //seventh possibility
                        if (theBoard[rows, cols].AnimalType == theBoard[rows, cols + 2].AnimalType && theBoard[rows, cols].AnimalType == theBoard[rows, cols + 3].AnimalType)
                        {
                            theBoard[rows, cols].Mood = SHOCK;
                            theBoard[rows, cols + 2].Mood = SHOCK;
                            theBoard[rows, cols + 3].Mood = SHOCK;
                        }
                        //eighth possibility
                        if (theBoard[rows, cols].AnimalType == theBoard[rows, cols + 1].AnimalType && theBoard[rows, cols].AnimalType == theBoard[rows, cols + 3].AnimalType)
                        {
                            theBoard[rows, cols].Mood = SHOCK;
                            theBoard[rows, cols + 1].Mood = SHOCK;
                            theBoard[rows, cols + 3].Mood = SHOCK;
                        }
                    }
                }
            }
        }

        private void VerticalHints()
        {
            //first possibility = [rows,cols] [rows+1,cols] [rows+2,cols+1]
            //second possibility = [rows,cols] [rows+1,cols] [rows+2,cols-1]
            //third possibility = [rows,cols] [rows+1,cols+1] [rows+2,cols]
            //fourth possibility = [rows,cols] [rows+1,cols-1] [rows+2,cols]
            //fifth possibility = [rows,cols] [rows+1,cols+1] [rows+2,cols+1]
            //sixth possibility = [rows,cols] [rows+1,cols-1] [rows+2,cols-1]
            //seventh possibility = [rows,cols] [rows+2,cols] [rows+3,cols]
            //eighth possibility = [rows,cols] [rows+1,cols] [rows+3,cols]
            hintsActive = true;
            for (int cols = 0; cols < MaxCols; cols++)
            {
                for (int rows = 0; rows < 6; rows++)
                {

                    if (rows < 6 && cols < 7)
                    {
                        //first possibility
                        if (theBoard[rows, cols].AnimalType == theBoard[rows + 1, cols].AnimalType && theBoard[rows, cols].AnimalType == theBoard[rows + 2, cols + 1].AnimalType)
                        {
                            theBoard[rows, cols].Mood = SHOCK;
                            theBoard[rows + 1, cols].Mood = SHOCK;
                            theBoard[rows + 2, cols + 1].Mood = SHOCK;
                        }
                        //third possibility
                        if (theBoard[rows, cols].AnimalType == theBoard[rows + 1, cols + 1].AnimalType && theBoard[rows, cols].AnimalType == theBoard[rows + 2, cols].AnimalType)
                        {
                            theBoard[rows, cols].Mood = SHOCK;
                            theBoard[rows + 1, cols + 1].Mood = SHOCK;
                            theBoard[rows + 2, cols].Mood = SHOCK;
                        }
                        //fifth possibility
                        if (theBoard[rows, cols].AnimalType == theBoard[rows + 1, cols + 1].AnimalType && theBoard[rows, cols].AnimalType == theBoard[rows + 2, cols + 1].AnimalType)
                        {
                            theBoard[rows, cols].Mood = SHOCK;
                            theBoard[rows + 1, cols + 1].Mood = SHOCK;
                            theBoard[rows + 2, cols + 1].Mood = SHOCK;
                        }
                    }
                    if (rows < 6 && cols > 0)
                    {
                        //second possibility
                        if (theBoard[rows, cols].AnimalType == theBoard[rows + 1, cols].AnimalType && theBoard[rows, cols].AnimalType == theBoard[rows + 2, cols - 1].AnimalType)
                        {
                            theBoard[rows, cols].Mood = SHOCK;
                            theBoard[rows + 1, cols].Mood = SHOCK;
                            theBoard[rows + 2, cols - 1].Mood = SHOCK;
                        }
                        //fourth possibility
                        if (theBoard[rows, cols].AnimalType == theBoard[rows + 1, cols - 1].AnimalType && theBoard[rows, cols].AnimalType == theBoard[rows + 2, cols].AnimalType)
                        {
                            theBoard[rows, cols].Mood = SHOCK;
                            theBoard[rows + 1, cols - 1].Mood = SHOCK;
                            theBoard[rows + 2, cols].Mood = SHOCK;
                        }
                        //sixth possibility
                        if (theBoard[rows, cols].AnimalType == theBoard[rows + 1, cols - 1].AnimalType && theBoard[rows, cols].AnimalType == theBoard[rows + 2, cols - 1].AnimalType)
                        {
                            theBoard[rows, cols].Mood = SHOCK;
                            theBoard[rows + 1, cols - 1].Mood = SHOCK;
                            theBoard[rows + 2, cols - 1].Mood = SHOCK;
                        }
                    }
                    if (rows < 5)
                    {
                        //seventh possibility
                        if (theBoard[rows, cols].AnimalType == theBoard[rows + 2, cols].AnimalType && theBoard[rows, cols].AnimalType == theBoard[rows + 3, cols].AnimalType)
                        {
                            theBoard[rows, cols].Mood = SHOCK;
                            theBoard[rows + 2, cols].Mood = SHOCK;
                            theBoard[rows + 3, cols].Mood = SHOCK;
                        }
                        //eighth possibility
                        if (theBoard[rows, cols].AnimalType == theBoard[rows + 1, cols].AnimalType && theBoard[rows, cols].AnimalType == theBoard[rows + 3, cols].AnimalType)
                        {
                            theBoard[rows, cols].Mood = SHOCK;
                            theBoard[rows + 1, cols].Mood = SHOCK;
                            theBoard[rows + 3, cols].Mood = SHOCK;
                        }
                    }
                }
            }
        }

        private void LevelTracking()
        {
            //keeps track of what level the player is on and changes it when the player gets a certain score. Also adds a new animal on level 2 and 4
            //this is extra functionality
            if (levelTracker == 1)
                noOfAnimals = 5;//6
            if (levelTracker == 1 && playerScore.Score > 100)
            {
                levelPlusInstance.Play();
                //noOfAnimals += 1;
                gameMode = levelUp;
            }
            if (levelTracker == 2 && playerScore.Score > 200)
            {
                levelPlusInstance.Play();
                gameMode = levelUp;
            }
            if (levelTracker == 3 && playerScore.Score > 300)
            {
                levelPlusInstance.Play();
                //noOfAnimals += 1;
                gameMode = levelUp;
            }
            if (levelTracker == 4 && playerScore.Score > 400)
            {
                levelPlusInstance.Play();
                gameMode = levelUp;
            }
            if (levelTracker == 5 && playerScore.Score > 500)
            {
                levelPlusInstance.Play();
                gameMode = Win;
            }
        }

        private void Timer()
        {
            //controls the decrease in time available to the player and the timing for the hourglass animation
            if (timing == 50)
            {
                playerScore.DecreaseTimer();
                hourglassCounter++;
                hintTimer++;
                timing = 0;
                if (hourglassCounter == 4)
                    hourglassCounter = 0;
            }
            //controls a message that tells the player that time is running out
            if (playerScore.Timer == 15)
                aMessage = "Time";
            if (playerScore.Timer == 14)
                aMessage = "is";
            if (playerScore.Timer == 13)
                aMessage = "running";
            if (playerScore.Timer == 12)
                aMessage = "out!!!";
            if (playerScore.Timer == 11)
                aMessage = "";
            if (playerScore.Timer == 10)//plays a sound to give the player an audio warning that time is running out
                timeRunningOut.Play();
            //when time runs out sets gamemode to end and resets timer, score and hints
            if (playerScore.Timer == 0)
            {
                gameMode = END;
                playerScore.Timer = 100;
                playerScore.Score = 0;
                playerScore.Hints = 3;
                beepSoundInstance.Play();
            }
        }

        private bool IsMediaPlayerBusy()
        {
            if (!Microsoft.Xna.Framework.Media.MediaPlayer.GameHasControl)
            {
                MessageBoxResult result;
                result = MessageBox.Show("The media player is currently playing. Do you wish to stop it and continue?", "Continue", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    Microsoft.Xna.Framework.Media.MediaPlayer.Pause();
                    pauseMediaPlayer = true;
                }
                else
                {
                    pauseMediaPlayer = false;
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            //changes the background colour depending on what level the player is currently on
            if (levelTracker == 1)
                GraphicsDevice.Clear(Color.LimeGreen);
            if (levelTracker == 2)
                GraphicsDevice.Clear(Color.Coral);
            if (levelTracker == 3)
                GraphicsDevice.Clear(Color.DodgerBlue);
            if (levelTracker == 4)
                GraphicsDevice.Clear(Color.IndianRed);
            if (levelTracker == 5)
                GraphicsDevice.Clear(Color.Ivory);

            spriteBatch.Begin();
            switch (gameMode)
            {
                case Instructions:
                    //draws the instructions on the screen after the splash screen so the player can see what they have to do
                    spriteBatch.DrawString(font, "1) Match 3 beyblades in a row.", new Vector2(10, 10), Color.Gold);
                    spriteBatch.DrawString(font, "2) you have 100 seconds to play.", new Vector2(10, 40), Color.LemonChiffon);
                   // spriteBatch.DrawString(font, "3) Press 'P' to Pause the game.", new Vector2(10, 70), Color.DodgerBlue);
                   // spriteBatch.DrawString(font, "3) press it again to un-pause.", new Vector2(10, 100), Color.Pink);
                    spriteBatch.DrawString(font, "3) When you score a set amount,", new Vector2(10, 70), Color.Crimson);
                    spriteBatch.DrawString(font, "you go to the next level.", new Vector2(10, 100), Color.Crimson);
                    spriteBatch.DrawString(font, "4)Press 'hints' to see hints.", new Vector2(10, 130), Color.Chocolate);
                    spriteBatch.DrawString(font, "You have 3 of these per level.", new Vector2(10, 160), Color.Chocolate);
                    spriteBatch.DrawString(font, "They will show for 5 secs.", new Vector2(10, 190), Color.Chocolate);
                    spriteBatch.DrawString(font, "5) Enjoy!!", new Vector2(10, 220), Color.Orange);
                    break;

                case GAME:
                    //draws the board
                    for (byte row = 0; row < MaxRows; row++)
                    {
                        for (byte col = 0; col < MaxCols; col++)
                        {
                            theBoard[row, col].Draw(spriteBatch, row, col);
                        } // end inner for
                    } // end outer for
                    //draws the highlight around a selected animal
                    if (selected)
                    {
                        spriteBatch.Draw(highLightTexture, new Rectangle(currentY * SPRITEOFFSET + XOFFSET, currentX * SPRITEOFFSET + YOFFSET, SPRITEOFFSET, SPRITEOFFSET), Color.White);
                    }
                    //draws indicators that point the player towards their score,timer,hints and level
                    spriteBatch.DrawString(font, "Your Score:", new Vector2(360, 20), Color.Black);
                    spriteBatch.DrawString(font, "Time left:", new Vector2(370, 100), Color.Black);
                    spriteBatch.DrawString(font, "Hints:", new Vector2(370, 210), Color.Black);
                    spriteBatch.DrawString(font, "Level " + levelTracker, new Vector2(370, 280), Color.Cyan);
                    playerScore.Draw(spriteBatch, font);
                    //draws a message that warns the player time is running out
                    spriteBatch.DrawString(font, aMessage, new Vector2(600, 100), Color.Black);

                    //hourglass timer for the hourglass that turns each second
                    if (hourglassCounter == 0)
                        spriteBatch.Draw(hourglassTexture1, new Rectangle(400, 160, 30, 40), Color.White);
                    if (hourglassCounter == 1)
                        spriteBatch.Draw(hourglassTexture2, new Rectangle(400, 160, 30, 40), Color.White);
                    if (hourglassCounter == 2)
                        spriteBatch.Draw(hourglassTexture3, new Rectangle(400, 160, 30, 40), Color.White);
                    if (hourglassCounter == 3)
                        spriteBatch.Draw(hourglassTexture4, new Rectangle(400, 160, 30, 40), Color.White);

                    break;

                case levelUp:
                    //draws the level up screen when the player goes on to a new level
                    spriteBatch.Draw(levelUpTexture, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
                    break;
                case Win:
                    //draws the win screen when the player wins the game
                    spriteBatch.Draw(winTexture, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.DodgerBlue);
                    //spriteBatch.DrawString(font, "Press 'L' to see the leaderboard", new Vector2(20, 20), Color.White);
                    break;

                //if game is over
                case END:
                    //draws game over screen when the player runs out of time
                    spriteBatch.Draw(gameoverTexture, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
                    //draws the sequence of pictures that show up on the game over screen
                    if (coinTimer == 1)
                        spriteBatch.Draw(insertCoinTexture1, new Rectangle(22, 120, 40, 50), Color.White);
                    if (coinTimer == 2)
                        spriteBatch.Draw(insertCoinTexture2, new Rectangle(60, 120, 29, 50), Color.White);
                    if (coinTimer == 3)
                        spriteBatch.Draw(insertCoinTexture3, new Rectangle(100, 120, 30, 50), Color.White);
                    if (coinTimer == 4)
                        spriteBatch.Draw(insertCoinTexture4, new Rectangle(140, 120, 25, 50), Color.White);
                    if (coinTimer == 5)
                        spriteBatch.Draw(insertCoinTexture5, new Rectangle(180, 120, 50, 50), Color.White);
                    if (coinTimer == 6)
                        coinTimer = 0;

                    break;

                case PAUSE:
                    //draws the pause screen
                    spriteBatch.DrawString(font, "PAUSE", new Vector2(200, 150), Color.Red);
                    break;

                // starting animation
                case SPLASH:
                    spriteBatch.Draw(elephantTexture, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), new Rectangle(323, 1, 256, 160), Color.White);
                    if (splashTiming < 100)//102,.,316,.
                    {
                        spriteBatch.Draw(elephantTexture, new Rectangle(50, 81, 700, 214), new Rectangle(163, 99, 158, 94), Color.White);
                    }
                    if (splashTiming < 80)
                    {
                        spriteBatch.Draw(elephantTexture, new Rectangle(50, 81, 700, 214), new Rectangle(2, 99, 158, 94), Color.White);
                    }
                    if (splashTiming < 70)
                    {
                        spriteBatch.Draw(elephantTexture, new Rectangle(50, 81, 700, 214), new Rectangle(163, 2, 158, 94), Color.White);
                    }
                    if (splashTiming < 50)
                    {
                        spriteBatch.Draw(elephantTexture, new Rectangle(50, 81, 700, 214), new Rectangle(2, 2, 158, 94), Color.White);
                    }
                    if (splashTiming > 100)
                    {
                        splashTiming = 0;
                    }
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
