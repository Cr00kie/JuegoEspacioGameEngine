using System.Data;

namespace JuegoEspacio
{
    public static class GameManager
    {
        public const float DELTA = 50f;
        public static World world = new World(20, 20);
    }
    public static class Input
    {
        static char? input;
        public static void UpdateInput()
        {
            if (Console.KeyAvailable)
            {
                input = Console.ReadKey(true).KeyChar;
                while (Console.KeyAvailable) Console.ReadKey(true);
            }
            else
            {
                input = null;
            }
        }
        public static char? GetInput()
        {
            return input;
        }
    }
    public static class Renderer
    {
        public static void RenderWorld(World world)
        {
            List<SpaceObject> objects = world.spaceObjects;
            foreach(SpaceObject obj in objects)
            {
                Console.SetCursorPosition((int)obj.xo, (int)obj.yo);
                Console.Write(" ");
                Console.SetCursorPosition((int)obj.x, (int)obj.y);
                Console.Write(obj.sprite);
            }
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            World world = GameManager.world;
            SpaceShip ship = new SpaceShip(0, 0);
            world.InstantiateObject(ship);
            while (true)
            {
                Input.UpdateInput();
                world.UpdateObjects();
                Renderer.RenderWorld(world);
                Thread.Sleep((int)GameManager.DELTA);
            }

        }
    }
    public class World
    {
        public int worldWidth, worldHeight;
        public World(int wWitdht, int wHeight)
        {
            worldWidth = wWitdht;
            worldHeight = wHeight;
        }
        public List<SpaceObject> spaceObjects = new List<SpaceObject>();

        public void InstantiateObject(SpaceObject spaceObject)
        {
            spaceObjects.Add(spaceObject);
        }

        public void DestroyObject(SpaceObject spaceObject)
        {
            spaceObjects.Remove(spaceObject);
            //Delete de last drawing of it
            Console.SetCursorPosition((int)spaceObject.x, (int)spaceObject.y);
            Console.Write(" ");
        }

        public void UpdateObjects()
        {
            List<SpaceObject> objects = new List<SpaceObject>();
            objects.AddRange(spaceObjects);
            foreach(SpaceObject spaceObject in objects)
            {
                spaceObject.Update();
            }
        }
    }

    public class SpaceObject
    {
        public string sprite;
        public float x, y;
        public float xo, yo;
        public float vx, vy;

        public SpaceObject(int x = 0, int y = 0, string sprite = "X")
        {
            this.xo = x;
            this.x = x;
            this.yo = y;
            this.y = y;
            this.sprite = sprite;
        }

        public virtual void Transform()
        {
            // x = x0 + vx * t
            xo = x;
            if((xo != 0 && vx !< 0) || (xo != GameManager.world.worldWidth && vx !> 0)) x = xo + (int)vx * GameManager.DELTA / 100;
            yo = y;
            if ((yo != 0 && vy! < 0) || (yo != GameManager.world.worldHeight && vy! > 0)) y = yo + (int)vy * GameManager.DELTA / 100;
        }

        public virtual void Update()
        {
            Transform();
        }
    }

    public class Bullet : SpaceObject
    {
        public Bullet(int x, int y) : base(x, y, "^") { vy = -1; }

        public override void Update()
        {
            if(y == 0) GameManager.world.DestroyObject(this);

            else Transform();
        }
    }

    public class SpaceShip : SpaceObject
    {
        public SpaceShip(int x, int y) : base(x, y, "A") { }
        public override void Update()
        {
            char? input = Input.GetInput();
            //Get inputs and change variables
            if(input == null)
            {
                vx = 0;
                vy = 0;
            }
            else
            {
                switch (input)
                {
                    case 'w': vy = -1; vx = 0; break;
                    case 's': vy = 1; vx = 0; break;
                    case 'a': vy = 0; vx = -1; break;
                    case 'd': vy = 0; vx = 1; break;
                    case 'l': Shoot(); break;
                }
            }
            Transform();
        }
        public void Shoot()
        {
            GameManager.world.InstantiateObject(new Bullet((int)x, (int)y));
        }
    }
}
