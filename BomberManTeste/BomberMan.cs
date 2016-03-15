using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberManTeste
{
    public enum Direction { RIGHT , LEFT, DOWN, UP }



    class BomberMan
    {
        bool is_moving = false;
        Texture2D image;
        Vector2 position;
        Direction direction;
        int spriteWidht, spriteHeight;

        float v;//espaço a adicionar a esquerda do sokoban para ficar centrado
        float distance;//distancia percorrida entre zero e 64
        float spriteColum = 0f;
        public BomberMan(ContentManager content, Vector2 position)
        {
            this.position = position;
            this.direction = Direction.RIGHT;
            image = content.Load<Texture2D>("character3");

            spriteWidht = image.Width / 3;
            spriteHeight = image.Height / 4;

            v = (64 - spriteWidht) * 0.5f;
        }
        public Vector2 Position()
        {
            return position;
        }
        public void Update(GameTime gameTime)
        {
            if (is_moving)
            {
                distance += 3;
                spriteColum += 0.25f;
                if (distance >= 64)
                {
                    spriteColum = 0f;
                    distance = 0;
                    is_moving = false;
                    position = position + lastMovement;
                }
            }
        }

        public void Move(Vector2 movement)
        {

            lastMovement = movement;
            //as sprites da direita e para baixo estao trocadas
            if (movement.X == 1)
                direction = Direction.RIGHT;

            else if (movement.X == -1)
                direction = Direction.LEFT;

            else if (movement.Y == 1)
                direction = Direction.DOWN;

            else
                direction = Direction.UP;

            distance = 0f;
            is_moving = true;


        }
        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(image, position: new Vector2(position.X * 64 + v
                , position.Y * 64) + distanceToVector()
                , color: Color.White, sourceRectangle: new Rectangle(
                (int)spriteColum % 3 * spriteWidht,
                spriteHeight * (int)direction, spriteWidht, spriteHeight));
        }

        Vector2 distanceToVector()
        {
            switch (direction)
            {
                case Direction.RIGHT:
                    return Vector2.UnitX * distance;
                case Direction.LEFT:
                    return -Vector2.UnitX * distance;
                case Direction.DOWN:
                    return Vector2.UnitY * distance;
                case Direction.UP:
                    return -Vector2.UnitY * distance;


            }
            return Vector2.Zero;
        }

        public bool isMoving()
        {
            return is_moving;
        }


        public Vector2 lastMovement { get; set; }
    }
}
