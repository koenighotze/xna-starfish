
namespace Star.Util
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using System;
    using Star.Util.Debug;
    using Star.Util.Log;

    public class CornerStack
    {
        public enum POSITION
        {
            UPPER_LEFT = 0,
            UPPER_RIGHT,
            LOWER_LEFT,
            LOWER_RIGHT
        }

        private readonly ILogger logger = ILoggerFactory.getLogger(typeof(CornerStack));
        private readonly Random rand = new Random();
        private readonly Vector2[] corners = new Vector2[4];
        private readonly Rectangle bounds;        

        public CornerStack(Rectangle someBounds)
        {
            Assert.FailIfNull(someBounds, "someBounds");
            
            this.bounds = new Rectangle(someBounds.X, someBounds.Y, someBounds.Width, someBounds.Height);            

            Init();
        }

        private void Init()
        {            
            logger.Debug("Initializing corner stack...");
            
            this.corners[(int)POSITION.UPPER_LEFT] = new Vector2(this.bounds.Left, this.bounds.Top);
            this.corners[(int)POSITION.UPPER_RIGHT] = new Vector2(this.bounds.Right, this.bounds.Top);
            this.corners[(int)POSITION.LOWER_LEFT] = new Vector2(this.bounds.Left, this.bounds.Bottom);
            this.corners[(int)POSITION.LOWER_RIGHT] = new Vector2(this.bounds.Right, this.bounds.Bottom);

            logger.Debug("...finished initializing!");
        }


        public Vector2 GetCorner(POSITION aPosition)
        {
            Assert.FailIfNull(aPosition, "aPosition");

            return this.corners[(int)aPosition];
        }

        public Queue<Vector2> GetRandomCornerStack()
        {
            Queue<Vector2> result = new Queue<Vector2>();

            logger.Debug("Building random corner stack...");
            // TODO: Hier richtiges Shuffle!
            List<Vector2> tempCorners = new List<Vector2>(this.corners);
            do
            {
                int position = this.rand.Next(0, tempCorners.Count);

                result.Enqueue(tempCorners[position]);

                logger.Debug("...random stack at {0} is {1}", position, tempCorners[position]);

                tempCorners.RemoveAt(position);
            } while (0 != tempCorners.Count);            

            return result;
        }
    }
}
