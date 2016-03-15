#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Linq;
using BomberManTeste;
#endregion

namespace BomberMan_trabalho
{
    //falta adicionar a classes das bombas 

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        char[,] board;
        int width, height;

        int size = 64;// tamanho das imagen usadas(largura,altura)
        Texture2D wall;
        Texture2D caixa;
        Texture2D bola;
        Texture2D beje;
        Texture2D pixel;

        bool win = false;



        BomberMan bomberman;


        float scale = 0.75f;//escala
        SpriteFont arialBlack20;

        int nrMovements = 0;
        int nrLevels = 3;
        int curLevel = 1;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.Black });

            LoadLevel();
            base.Initialize();
        }


        void LoadLevel()
        {
            board = readBomberMan(@"Content\level" + curLevel + ".sok");
            width = board.GetLength(0);
            height = board.GetLength(1);
            
            nrMovements = 0;
            
            bomberman = new BomberMan(Content, positionBomberMan());
            win = false;

            graphics.PreferredBackBufferHeight = (int)(scale * (30 + height * size));
            graphics.PreferredBackBufferWidth = (int)(scale * width * size);
            graphics.ApplyChanges();

        }


        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);
            wall = Content.Load<Texture2D>("wall");
            caixa = Content.Load<Texture2D>("caixa");
            beje = Content.Load<Texture2D>("stars2");
            bola = Content.Load<Texture2D>("bomba");

            arialBlack20 = Content.Load<SpriteFont>("simoes");//carregar a spritefont

        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // movementTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //sair se o jogo acabou
            if (isWin())
            {
                //mudança de mapas working
                if (curLevel < nrLevels)
                {
                    curLevel++;
                    LoadLevel();
                }

                else
                    win = true;
            }


            KeyboardState keys = Keyboard.GetState();
            if (keys.IsKeyDown(Keys.R))
            {
                if (win) curLevel = 1;
                LoadLevel();
                //da restart ao nivel
            }

            //alterar a velocidade do boneco
            if (!win && bomberman.isMoving() == false)
            {

                Vector2 movement = Vector2.Zero;
                if (keys.IsKeyDown(Keys.Down))
                    //baixo
                    movement = Vector2.UnitY;
                else if (keys.IsKeyDown(Keys.Up))
                    //cima
                    movement = -Vector2.UnitY;
                else if (keys.IsKeyDown(Keys.Left))
                    //esqerda
                    movement = -Vector2.UnitX;
                else if (keys.IsKeyDown(Keys.Right))
                    //direita
                    movement = Vector2.UnitX;



                if (movement != Vector2.Zero)
                {

                    Vector2 position = bomberman.Position();

                    if (isCrate(position + movement))
                    {
                        if (!isCrate(position + 2 * movement) &&
                            !iswall(position + 2 * movement))
                        {
                            moveCrate(position + movement, position + 2 * movement);
                            // position = position + movement;
                            bomberman.Move(movement);
                            nrMovements++;
                        }
                    }

                    else if (!iswall(position + movement))
                    {
                        //position = position + movement;
                        nrMovements++;//aki ja conta quanto ta andar contra a parede
                        bomberman.Move(movement);


                    }
                }

            }

            bomberman.Update(gameTime);
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(transformMatrix: Matrix.CreateScale(scale));

            spriteBatch.DrawString(arialBlack20, "Vidas : " + nrMovements, new Vector2(10, height * size - 5), Color.Yellow);
            //falta alterar isto ,com um vidas
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //criar imagem para fundo
                    spriteBatch.Draw(beje, new Vector2(x * size, y * size), Color.White);
                    switch (board[x, y])
                    {
                        case '#':
                            spriteBatch.Draw(wall, new Vector2(x * size, y * size),
                         Color.White);
                            break;
                        case '*':
                        case '$':
                            spriteBatch.Draw(caixa, new Vector2(x * size, y * size),
                                Color.White);
                            break;
                        case '.':
                            spriteBatch.Draw(bola, new Vector2(x * size, y * size),
                                Color.White);
                            break;
                        default:
                            break;
                    }
                }


            }
            bomberman.Draw(spriteBatch);
            spriteBatch.End();
            //win screen
            if (win)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(pixel, new Rectangle(0, 0, GraphicsDevice.Viewport.Width,
                    GraphicsDevice.Viewport.Height), new Color(Color.Black, 0.5f));

                Vector2 strSize = arialBlack20.MeasureString("YOU WIN");
                spriteBatch.DrawString(arialBlack20, "YOU WIN",
                    (new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height) - strSize) * 0.5f, Color.DarkCyan);
                spriteBatch.End();

            }




            base.Draw(gameTime);
        }




        void moveCrate(Vector2 origem, Vector2 destino)
        {
            //1.remover a origem
            if (board[(int)origem.X, (int)origem.Y] == '*')
                board[(int)origem.X, (int)origem.Y] = '.';
            else
                board[(int)origem.X, (int)origem.Y] = ' ';


            //2.colocar o destino
            if (board[(int)destino.X, (int)destino.Y] == '.')
                board[(int)destino.X, (int)destino.Y] = '*';
            else
                board[(int)destino.X, (int)destino.Y] = '$';
        }





        bool isCrate(Vector2 pos)
        {
            return (board[(int)pos.X, (int)pos.Y] == '$')
                || (board[(int)pos.X, (int)pos.Y] == '*');
        }



        bool iswall(Vector2 coord)
        {
            return board[(int)coord.X, (int)coord.Y] == '#';

        }



        /*
        *  # - Parede
        *  . - destino de caixa
        *  $ - caixa
        *  @ - Sokoban
        *  * - caixa no destino
        *  + - sokoban num destino
        *  espaco, caminho...
        */

        static char[,] readBomberMan(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
            int width = lines.Select(x => x.Length).Max();
            int height = lines.Length;

            char[,] board = new char[width, height];

            for (int line = 0; line < height; line++)
            {
                for (int c = 0; c < width; c++)
                {
                    board[c, line] = c < lines[line].Length ? lines[line][c] : ' ';
                }
            }
            return board;
        }
        Vector2 positionBomberMan()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (board[x, y] == '@')
                    {
                        board[x, y] = ' ';
                        return new Vector2(x, y);
                    }

                    else if (board[x, y] == '+')
                    {
                        board[x, y] = '.';
                        return new Vector2(x, y);
                    }
                }

            }
            return Vector2.Zero; //em principio nunca executado
        }
        bool isWin()
        {
            for (int x = 0; x < width; x++)
            {

                for (int y = 0; y < height; y++)
                {
                    if (board[x, y] == '.')
                        return false;
                }
            }
            return true;
        }


    }
}
