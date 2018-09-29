using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using gameObjects;
using System.Diagnostics;
using System.Threading;

namespace visual_game
{


    public partial class frmSolitaire : Form
    {
        public delegate void DelUpdateTextBox();
        public delegate void DelNewGame();
        public delegate void DelUCTTree(int tragectorys);
        public delegate void delMoveImages(int pileNumber, int index, cardView cardImage);

        Thread uctThread;

        Dictionary<Card, cardView> deck;
        TimeSpan ts;
        double avgSec;
        Stopwatch stopWatch;
        List<Move> moves;
        List<PictureBox> boxes;
        OldGame game;
        int autoGames;
        List<Bitmap> deckImages;
        int lastMoved;
        //UCTTree tree;
        bool started, autoPlay , master;
        bool threadAutoPlay;
        int win, loss, totalGames;
        public List<string> gameovers;
        public NewUCTPolicy uCTPolicy;
        public frmSolitaire()
        {
            InitializeComponent();
        }
        public void LoadImages()
        {
            //deck image red suites 0-23 24-51
            deckImages = new List<Bitmap>();
            Image image = visual_game.Properties.Resources.Deck_72x100x16;
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 13; x++)
                {
                    int xCut = (x * 72);
                    int yCut = y * 100;
                    deckImages.Add(new Bitmap(72, 100));
                    var graphics = Graphics.FromImage(deckImages[deckImages.Count - 1]);
                    graphics.DrawImage(image, new Rectangle(0, 0, 72, 100), new Rectangle(xCut, yCut, 72, 100), GraphicsUnit.Pixel);
                    graphics.Dispose();
                }
            }
            //field pile index 52
            deckImages.Add(new Bitmap(72, 100));
            var temp = Graphics.FromImage(deckImages[deckImages.Count - 1]);
            temp.DrawImage(image, new Rectangle(0, 0, 72, 100), new Rectangle(2 * 72, 4 * 100, 72, 100), GraphicsUnit.Pixel);
            temp.Dispose();
            //back image 53
            deckImages.Add(new Bitmap(72, 100));
            temp = Graphics.FromImage(deckImages[deckImages.Count - 1]);
            temp.DrawImage(image, new Rectangle(0, 0, 72, 100), new Rectangle(4 * 72, 4 * 100, 72, 100), GraphicsUnit.Pixel);
            temp.Dispose();

        }
        public void NewGame()
        {
            
            started = true;
            game = new OldGame();
            //tree = new UCTTree(game);
            lastMoved = 24;
            for (int pn = 0; pn < 12; pn++)
            {
                CardPile cp = game.gamePiles[pn];
                for (int i = 0; i < cp.Count; i++)
                {
                    MovePic(pn, i, deck[cp[i]]);
                }
            }
            lBoxMoves.Items.Clear();
            if(autoPlay||threadAutoPlay)
            {
                stopWatch.Restart();
            }
            LoadMoves();
            if (game.endOfGame())
            {
                started = false;
                autoPlay = false;
                if (game.lossCheck1())
                {
                    textBox2.Text = "Game Over 1";
                }
                else
                {
                    textBox2.Text = "Game Over 2";
                }
                textBox3.Text = game.ToString();
            }
            else
            {
                textBox2.Text = "Not over";
                textBox3.Text = "";
            }

        }
        public void UpdateText()
        {
            textBox1.Text = win.ToString() + "-" + totalGames.ToString() + " sec:" + avgSec.ToString();
        }
        public void AutoPlayThreadStarter()
        {
            if (!int.TryParse(AutoPlayBox.Text, out autoGames))
            {
                autoGames = 1;
            }
            AutoPlay(autoGames);

        }
        public void AutoPlay(int NumGames)
        {
            if(threadAutoPlay)
            {
                
                for (int i = 0; i < NumGames; i++)
                {
                    if (!started)
                    {
                        this.Invoke( new DelNewGame(NewGame));
                    }
                    while (!game.endOfGame())
                    {
                        if (started)
                        {
                            //test
                            List<Move> moves = game.FindMoveList();
                            //Move m = tree.bestMove(100, game);
                            //Action a = new UctPolicy().uctBestAction(game, 1000);
                            //Move m = new Move(game.cardsLocation[a.card].cardIndex, a.oldStack, a.newStack);
                            NewAction a = uCTPolicy.BestMove(game, 100);
                            Move m = new Move(game.cardsLocation[a.moveCard].cardIndex, a.fromPile, a.toPile);
                            game.makeMove(m);
                            if (m.from == 11)
                            {
                                lastMoved = m.index;
                            }
                            for (int pn = 0; pn < 12; pn++)
                            {
                                CardPile cp = game.gamePiles[pn];
                                for (int k = 0; k < cp.Count; k++)
                                {
                                    Invoke(new delMoveImages( MovePic),pn, k, deck[cp[k]]);
                                }
                            }
                        }
                    }
                    started = false;
                    stopWatch.Stop();
                    totalGames++;
                    ts = stopWatch.Elapsed;
                    avgSec = avgSec + ts.TotalSeconds;
                    if (totalGames > 1)
                    {
                        avgSec = avgSec / 2;
                    }
                    if (game.isWin())
                    {
                        win++;
                    }
                    else
                    {
                        loss++;
                    }
                    textBox1.Invoke(new DelUpdateTextBox(UpdateText));

                }
            }

        }
        public void NewGame(string gameString)
        {
            started = true;
            game = new OldGame(gameString);
            //tree = new UCTTree(game);
            lastMoved = 24;
            for (int pn = 0; pn < 12; pn++)
            {
                CardPile cp = game.gamePiles[pn];
                for (int i = 0; i < cp.Count; i++)
                {
                    MovePic(pn, i, deck[cp[i]]);
                }
            }
            lBoxMoves.Items.Clear();
            LoadMoves();
            if (game.endOfGame())
            {
                started = false;
                autoPlay = false;
                if (game.lossCheck1())
                {
                    textBox2.Text = "Game Over 1";
                }
                else
                {
                    textBox2.Text = "Game Over 2";
                }
                textBox3.Text = game.ToString();
            }
            else
            {
                textBox2.Text = "Not over";
                textBox3.Text = "";
            }

        }
        public void LoadMoves()
        {

                moves = game.FindMoveList();
                foreach (Move m in moves)
                {
                    lBoxMoves.Items.Add(m.ToString());
                }
            
        }
        public void MovePic(int pileNumber, int index, cardView cardImage)
        {
            if (pileNumber < 7)
            {
                if (game.hidden[pileNumber] > index)
                {
                    if (cardImage.flip)
                    {
                        cardImage.flipCard();
                    }

                    cardImage.display.Top = 285 + (10 * index);
                    cardImage.display.Left = 10 + (100 * pileNumber);

                    cardImage.display.BringToFront();
                }
                else
                {
                    if (!cardImage.flip)
                    {
                        cardImage.flipCard();
                    }
                    cardImage.display.Top = 285 + (35 * index) - (game.hidden[pileNumber] * 25);
                    cardImage.display.Left = 10 + (100 * pileNumber);

                    cardImage.display.BringToFront();
                }


            }
            else if (pileNumber < 11)
            {
                if (!cardImage.flip)
                {
                    cardImage.flipCard();
                }
                cardImage.display.Top = 160;
                cardImage.display.Left = 10 + ((pileNumber - 7) * 80);
                cardImage.display.BringToFront();
            }
            else
            {
                if (!cardImage.flip)
                {
                    cardImage.flipCard();
                }

                cardImage.display.Left = 10 + (15 * index);

                if (index % 3 == 2 || game.gamePiles[11].Count - 1 == index)
                {
                    cardImage.display.Top = 10;
                }
                else if ((lastMoved - 1) % 3 == (index % 3) && lastMoved - 1 <= index || lastMoved == 0 && index == 0)
                {
                    cardImage.display.Top = 30;
                }
                else
                {
                    cardImage.display.Top = 55;
                }
                cardImage.display.BringToFront();
            }
            cardImage.display.BringToFront();
        }
        public void MakeMove()
        {
            if(started)
            {
                //test
                List<Move> moves = game.FindMoveList();
                //Move m = tree.bestMove(100, game);
                Action a = new UctPolicy().uctBestAction(game, 100);
                Move m = new Move(game.cardsLocation[a.card].cardIndex, a.oldStack, a.newStack);
                game.makeMove(m);
                if (m.from == 11)
                {
                    lastMoved = m.index;
                }
                for (int pn = 0; pn < 12; pn++)
                {
                    CardPile cp = game.gamePiles[pn];
                    for (int i = 0; i < cp.Count; i++)
                    {
                        MovePic(pn, i, deck[cp[i]]);
                    }
                    lBoxMoves.Items.Clear();
                    LoadMoves();
                    if (game.endOfGame())
                    {
                        started = false;
                        autoPlay = false;
                        if (game.lossCheck1())
                        {
                            textBox2.Text = "Game Over 1";
                        }
                        else
                        {
                            textBox2.Text = "Game Over 2";
                        }
                        textBox3.Text = game.ToString();
                    }
                    else
                    {
                        textBox2.Text = "Not over";
                        textBox3.Text = "";
                    }
                }
            }

    }
        public void MakeMove(int index)
        {
            if (index > -1)
            {
                Move m = moves[index];
                game.makeMove(m);
                if (m.from == 11)
                {
                    lastMoved = m.index;
                }
                for (int pn = 0; pn < 12; pn++)
                {
                    CardPile cp = game.gamePiles[pn];
                    for (int i = 0; i < cp.Count; i++)
                    {
                        MovePic(pn, i, deck[cp[i]]);
                    }
                }
                lBoxMoves.Items.Clear();
                LoadMoves();
                if (game.endOfGame())
                {
                    started = false;
                    autoPlay = false;
                    if(game.lossCheck1())
                    {
                        textBox2.Text = "Game Over 1";
                    }
                    else
                    {
                        textBox2.Text = "Game Over 2";
                    }
                    textBox3.Text = game.ToString();

                }
                else
                {
                    textBox2.Text = "Not over";
                }
            }
        }    

        private void Form1_Load(object sender, EventArgs e)
        {
            uCTPolicy = new NewUCTPolicy();
            stopWatch = new Stopwatch();
            gameovers = new List<string>();
            avgSec = 0;
            autoGames = 1;
            autoPlay = false;
            master = autoPlay = false;
            win = 0;
            loss = 0;
            totalGames = 0;
            started = false;
            threadAutoPlay = false;
            moves = new List<gameObjects.Move>();
            lastMoved = 24;
            //load/split Image and pictureBoxes
            boxes = Controls.OfType<PictureBox>().ToList();
            LoadImages();
            // set cards to there images
            int gameZones = boxes.Count - deckImages.Count + 1;
            deck = new Dictionary<Card, cardView>();
            //suite
            int boxIndex = 63;
            int imageID = 0;
            for(int s =0; s < 4; s++)
            {
                for(int v = 0; v < 13; v++)
                {
                    Card tempCard = new Card(s, v);
                    if (s == 0)
                    {
                        imageID = v;
                    }
                    else if (s == 1)
                    {
                        imageID = v + 26;
                    }
                    else if (s == 2)
                    {
                        imageID = v + 13;
                    }
                    else
                    {
                        imageID = v + 39;
                    }
                    cardView tempView = new cardView(imageID.ToString(), boxes[boxIndex], deckImages[imageID], deckImages[53]);
                    deck.Add(tempCard, tempView);
                    tempView.display.BringToFront();

                    boxIndex--;
                }

                for(int i =0; i < 11; i++)
                {
                    boxes[boxIndex-i].Image = deckImages[52];
                }
                
            }
        }

        private void MasterBTN_Click(object sender, EventArgs e)
        {
            int i = lBoxMoves.SelectedIndex;
            if (i >= 0)
            {
                MakeMove(i);
            }
        }

        private void DecodeBtn_Click(object sender, EventArgs e)
        {
            NewGame(textBox1.Text);
        }

        private void frmSolitaire_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(uctThread != null && uctThread.IsAlive)
            {
                uctThread.Abort();
            }
            
        }

        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            if (master)
            {
                if (autoPlay && started)
                {
                    if(!game.isLoss())
                    {
                        MakeMove();
                        
                    }
                    else
                    {
                        autoPlay = false;
                    }
                    
                }
                else
                {

                    if (autoGames == 1)
                    {
                        master = false;
                    }

                    if (game != null && game.isWin())
                    {
                        stopWatch.Stop();
                        ts = stopWatch.Elapsed;
                        avgSec = avgSec + ts.TotalSeconds;

                        
                        textBox2.Text = "win";

                        win++;
                        totalGames++;
                        if (totalGames > 1)
                        {
                            avgSec = avgSec / 2;
                        }
                        textBox1.Text = win.ToString() + "-" + totalGames.ToString() +" sec:"+ avgSec.ToString();
                        autoGames--;
                        autoPlay = true;
                    }
                    else if (game != null)
                    {
                        stopWatch.Stop();
                        ts = stopWatch.Elapsed;
                        avgSec = avgSec + ts.TotalSeconds;
                        textBox2.Text = "loss";
                        loss++;
                        if (totalGames > 1)
                        {
                            avgSec = avgSec / 2;
                        }
                        gameovers.Add(game.ToString());
                        totalGames++;
                        textBox1.Text = win.ToString() + "-" + totalGames.ToString()+ " sec:" + avgSec.ToString(); avgSec.ToString();
                        autoGames--;
                        autoPlay = true;
                    }
                    NewGame();

                }
            }
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            NewGame();

        }
        private void btnMove_Click(object sender, EventArgs e)
        {
            if(!int.TryParse( AutoPlayBox.Text, out autoGames))
            {
                autoGames = 1;
            }
            threadAutoPlay = !threadAutoPlay;
            uctThread = new Thread(new ThreadStart(AutoPlayThreadStarter));
            uctThread.Name = "utc";
            uctThread.Start();
            //master = true;
            //autoPlay = true;
            btnMove.Enabled = false;
        }
        private void button1_Click(object sender, EventArgs e)
        {

            MakeMove();
        }

    }
}
