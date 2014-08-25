using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Star
{
    class Sprite 
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 size;
        private Vector2 screenSize;
        private Vector2 velocity;
        private Vector2 previousPosition;
        private Random random = new Random();
        private Boolean isRandomMovement = false;

        public Texture2D Texture
        {
            get
            {
                return this.texture;
            }
        }

        public Vector2 Position
        {
            get { return this.position; }


            set { this.position = value; }
        }

        public Vector2 Size
        {
            get { return this.size; }


            set { this.size = value; }
        }

        public Vector2 Velocity
        {
            get { return this.velocity; }


            set { this.velocity = value; }
        }

        /*
        public Vector2 NAME
        {
            get { return this.var; }
            

            set { this.var = value;}
        }
        */
        //HIER PROPERTIES EINFUEGEN
        //SOUNDS EINFUEGEN

        public Sprite(Texture2D aTexture, Vector2 aPostion, Vector2 aSize, Vector2 aScreenSize)
        {
            this.velocity = new Vector2(0L, 0L);
            this.texture = aTexture;
            this.position = aPostion;
            this.previousPosition = aPostion;
            this.size = aSize;
            this.screenSize = aScreenSize;
        }

        public Boolean CollidesWith(Sprite other)
        {
            return (this.position.X + this.size.X > other.position.X // rechte kante von this ist rechts von others linker kante
                &&
                this.position.X < other.position.X + other.size.X // dieses Sprite ist nicht voellig rechts von other
                &&
                this.position.Y + this.size.Y > other.position.Y // untere kante ist unterhalb oberer kante von other
                &&
                this.position.Y < other.position.Y + other.size.Y); // dieses Sprite ist nicht voellig unterhalb von other
        }

        public void MoveBack()
        {
            this.position = this.previousPosition;
        }

        private Vector2 DetermineVelocity()
        {
            Vector2 result = this.velocity;
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                result.X = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X * 5;
                result.Y = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y * -5;
            }
            else
            {

                KeyboardState state = Keyboard.GetState();
                if (state.IsKeyDown(Keys.Up))
                {
                    //                if (this.velocity > 0)
                    //              {
                    // abwaerts, bremsen also...logarithmisch

                    // wie?
                    //            }

                    result.Y -= 5;
                }
                else if (state.IsKeyDown(Keys.Down))
                {
                    //          if (this.velocity < 0)
                    //        {
                    // aufwaerts, bremsen also...logarithmisch

                    // wie?
                    //      }

                    result.Y += 5;
                }
                else if (state.IsKeyDown(Keys.Right))
                {
                    result.X += 10;
                }
                else if (state.IsKeyDown(Keys.Left))
                {
                    result.X -= 10;
                }
            }

            return result;
        }


        public void RandomMovement()
        {
            this.isRandomMovement = true;
            if (this.velocity == Vector2.Zero)
            {
                this.velocity = new Vector2(this.random.Next(1000) % 11 - 5, this.random.Next(1000) % 11 - 5);
            }
            DoMove();
        }

        private void DoMove()
        {
            this.previousPosition = this.position;            

            this.position += this.velocity;

            if ((this.screenSize.Y < this.position.Y + this.size.Y)
              ||
                (0 > this.position.Y))
            {
                this.position.Y = 0 > this.position.Y ? 0 : this.screenSize.Y - this.size.Y;
                this.velocity.Y *= -1;

                if (this.isRandomMovement)
                {
                    this.velocity = new Vector2(this.random.Next(1000) % 40 - 20, this.random.Next(1000) % 40 - 20);

                    if (this.velocity.X == 0)
                        this.velocity.X = 10;
                    if (this.velocity.Y == 0)
                        this.velocity.Y = 10;
                }
            }

            if ((this.screenSize.X < this.position.X + this.size.X)
                ||
                (0 > this.position.X))
            {
                this.position.X = 0 > this.position.X ? 0 : this.screenSize.X - this.size.X;
                this.velocity.X *= -1;

                if (this.isRandomMovement)
                {
                    this.velocity = new Vector2(this.random.Next(1000) % 11 - 5, this.random.Next(1000) % 11 - 5);
                    if (this.velocity.X == 0)
                        this.velocity.X = 10;
                    if (this.velocity.Y == 0)
                        this.velocity.Y = 10;
                }
            }

        }



        public void Move()
        {         
            this.velocity = DetermineVelocity();
            DoMove();
        }

    }
}
