using System.Data;
using System.Security.Cryptography.X509Certificates;

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
                if ((int)obj.xo >= 0 && (int)obj.yo >= 0)
                {
                    Console.SetCursorPosition((int)obj.xo, (int)obj.yo);
                    Console.Write(" ");
                }
                if ((int)obj.x >= 0 && (int)obj.y >= 0)
                {
                    Console.SetCursorPosition((int)obj.x, (int)obj.y);
                    Console.Write(obj.sprite);
                }
            }
        }
    }
    public static class CollisionManager
    {
        public static void CheckCollisions(World world)
        {
            List<SpaceObject> objects = new List<SpaceObject>();
            objects.AddRange(world.spaceObjects);
            bool colDetected = false;
            for(int i = 0; i < objects.Count; i++)
            {
                for(int j = i + 1; j < objects.Count; j++)
                {
                    //TO DO: CHECK IF COLLISION WAS SKIPPED BETWEEN FRAMES
                    if((int)objects[j].x == (int)objects[i].x && (int)objects[j].y == (int)objects[i].y)
                    {
                        objects[j].OnCollision(objects[i]);
                        objects[i].OnCollision(objects[j]);
                        colDetected = true;
                    }
                }
            }
            if (colDetected)
            {
                Console.SetCursorPosition(0, 3);
                Console.WriteLine("Collision detected");
            }
            else
            {
                Console.SetCursorPosition(0, 3);
                Console.WriteLine("                               ");
            }
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            World world = GameManager.world;
            world.InstantiateObject(new SpaceShip(GameManager.world.worldHeight, GameManager.world.worldWidth/2));
            world.InstantiateObject(new EnemySpaceShip(GameManager.world.worldHeight/2, 11));
            while (true)
            {
                Input.UpdateInput();
                if (Input.GetInput() == 'i') world.InstantiateObject(new EnemySpaceShip(GameManager.world.worldHeight / 2, 11));
                world.UpdateObjects();
                CollisionManager.CheckCollisions(world);
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
            if ((int)spaceObject.x >= 0 && (int)spaceObject.y >= 0)
            {
                Console.SetCursorPosition((int)spaceObject.x, (int)spaceObject.y);
                Console.Write(" ");
            }
            if ((int)spaceObject.xo >= 0 && (int)spaceObject.yo >= 0)
            {
                Console.SetCursorPosition((int)spaceObject.xo, (int)spaceObject.yo);
                Console.Write(" ");
            }
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
            if((xo != 0 && vx !< 0) || (xo != GameManager.world.worldWidth && vx !> 0)) x = xo + (int)vx * GameManager.DELTA / 50;
            yo = y;
            if ((yo != 0 && vy! < 0) || (yo != GameManager.world.worldHeight && vy! > 0)) y = yo + (int)vy * GameManager.DELTA / 50;
        }

        public virtual void OnCollision(SpaceObject objectCollided)
        {

        }

        public virtual void Update()
        {
            Transform();
        }
    }

    public class Bullet : SpaceObject
    {
        public float bulletSpeed
        {
            get
            {
                return vy;
            }
            set
            {
                vy = value;
            }
        }
        public Bullet(int x, int y, int bulSpeed = -1) : base(x, y, "^") { bulletSpeed = bulSpeed;  vy = bulSpeed; }
        public Bullet(int x, int y, int bulSpeed = -1, string sprite = "^") : base(x, y, sprite) { bulletSpeed = bulSpeed; vy = bulSpeed; }

        public override void Update()
        {
            if(y <= 0 || y >= GameManager.world.worldHeight) GameManager.world.DestroyObject(this);
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
            Console.SetCursorPosition(0, 1);
            Console.WriteLine("                                       ");
            Console.WriteLine("x:" + x + "y:" + y);
        }
        public override void OnCollision(SpaceObject objectCollided)
        {
            if (objectCollided.GetType() == typeof(Bullet)) GameManager.world.DestroyObject(this);
        }
        public void Shoot()
        {
            GameManager.world.InstantiateObject(new Bullet((int)x, (int)y - 1, -2));
        }
    }
    public class EnemySpaceShip : SpaceObject
    {
        int shootingRate = 1;
        int shootingCountdown = 0;
        public EnemySpaceShip(int x, int y) : base(x, y, "Y") { }
        public override void Update()
        {
            Random rng = new Random();
            switch (rng.Next(0, 6))
            {
                case 0: vx = 0; vy = 0; break;
                case 1: vx = 1; vy = 0; break;
                case 2: vx = -1; vy = 0; break;
                case 3: vx = 0; vy = 1; break;
                case 4: vx = 0; vy = -1; break;
                case 5:
                    if (shootingCountdown == shootingRate) { Shoot(); shootingCountdown = 0; }
                    else shootingCountdown++;
                    break;

            }
            Transform();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("                                        ");
            Console.WriteLine("x:" + x + "y:" + y);
        }
        public override void OnCollision(SpaceObject objectCollided)
        {
            if (objectCollided.GetType() == typeof(Bullet)) GameManager.world.DestroyObject(this);
        }
        public void Shoot()
        {
            GameManager.world.InstantiateObject(new Bullet((int)x, (int)y + 2, 2, "*"));
        }
    }
}
