using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Digger
{
    public class Terrain : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand() {DeltaX = 0, DeltaY = 0};
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return conflictedObject is Player;
        }

        public int GetDrawingPriority()
        {
            return 1;
        }
        
        public string GetImageFileName()
        {
            return "Terrain.png";
        }
    }

    public class Player : ICreature
    {

        public CreatureCommand Act(int x, int y)
        {
            var moveCom = new CreatureCommand();
            var key = Game.KeyPressed;
            if (key == Keys.Up)
                if (y >= 1)
                    moveCom.DeltaY = -1;

            if (key == Keys.Down)
                if (y < Game.MapHeight - 1)
                    moveCom.DeltaY = 1;

            if (key == Keys.Left)
                if (x >= 1)
                    moveCom.DeltaX = -1;

            if (key == Keys.Right)
                if (x < Game.MapWidth - 1)
                    moveCom.DeltaX = 1;

            if (Game.Map[x + moveCom.DeltaX, y + moveCom.DeltaY] is Sack)
                return new CreatureCommand();

            return moveCom;
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return conflictedObject is Sack || conflictedObject is Monster;
        }

        public int GetDrawingPriority()
        {
            return 0;
        }
        
        public string GetImageFileName()
        {
            return "Digger.png";
        }
    }

    public class Sack : ICreature
    {
        int falling = 0;
        public CreatureCommand Act(int x, int y)
        {
            var moveCom = new CreatureCommand();
            if (y != Game.MapHeight - 1)
            {
                if (Game.Map[x, y + 1] == null || ((Game.Map[x, y + 1] is Player 
                    || Game.Map[x, y + 1] is Monster) && falling > 0))
                {
                    falling++;
                    moveCom.DeltaY = 1;
                    return moveCom;
                }
            }
            
            if (falling > 1)
            {
                moveCom.TransformTo = new Gold();
                falling = 0;
                moveCom.DeltaY = 0;
                return moveCom;
            }
            falling = 0;
            moveCom.DeltaY = 0;
            return new CreatureCommand();
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return false;
        }

        public int GetDrawingPriority()
        {
            return 1;
        }
        
        public string GetImageFileName()
        {
            return "Sack.png";
        }
    }

    public class Gold : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand();
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject is Player)
                Game.Scores += 10;
            return true;
        }

        public int GetDrawingPriority()
        {
            return 1;
        }
        
        public string GetImageFileName()
        {
            return "Gold.png";
        }
    }
    public class Monster : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            for (var width = 0; width < Game.MapWidth; width++)
                for (var heigth = 0; heigth < Game.MapHeight; heigth++)
                    if (Game.Map[width, heigth] is Player)
                    {
                        if (x.CompareTo(width) > 0)
                            if ((x - 1 >= 0) && !(Game.Map[x - 1, y] is Terrain)
                                && !(Game.Map[x - 1, y] is Sack) && !(Game.Map[x - 1, y] is Monster))
                                return new CreatureCommand { DeltaX = -1 };

                        if (x.CompareTo(width) < 0)
                            if ((x + 1 < Game.MapWidth) && !(Game.Map[x + 1, y] is Terrain)
                                && !(Game.Map[x + 1, y] is Sack) && !(Game.Map[x + 1, y] is Monster))
                                return new CreatureCommand { DeltaX = 1 };

                        if (y.CompareTo(heigth) < 0)
                            if ((y + 1 < Game.MapHeight) && !(Game.Map[x, y + 1] is Terrain)
                                && !(Game.Map[x, y + 1] is Sack) && !(Game.Map[x, y + 1] is Monster))
                                return new CreatureCommand { DeltaY = 1 };

                        if (y.CompareTo(heigth) > 0)
                            if ((y - 1 >= 0) && !(Game.Map[x, y - 1] is Terrain)
                                && !(Game.Map[x, y - 1] is Sack) && !(Game.Map[x, y - 1] is Monster))
                                return new CreatureCommand { DeltaY = -1 };
                    }
            return new CreatureCommand();
        }
        public bool DeadInConflict(ICreature conflictedObject)
        {
            return conflictedObject is Sack || conflictedObject is Monster;
        }

        public int GetDrawingPriority()
        {
            return 1;
        }

        public string GetImageFileName()
        {
            return "Monster.png";
        }
    }
}
