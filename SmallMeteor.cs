using SplashKitSDK;

public class SmallMeteor : Enemy
{
    public SmallMeteor(Window gameWindow, int index, int spawnDensity) : base(gameWindow)
    {
        Bitmap smallMeteor = new Bitmap("tinymeteor", "images/meteorBrown_tiny1.png");
        EnemySprite = SplashKit.CreateSprite("tinymeteor", smallMeteor);

        X = index * (-spawnDensity) - EnemySprite.Width;
        Y = SplashKit.Rnd(170, 340) - EnemySprite.Height / 2;
        Lives = 5;
    }

    public override void Draw()
    {
        base.Draw();
    }

    public override void Update()
    {
        base.Update();
    }

    public override int Lives
    {
        get { return base.Lives; }
        set { base.Lives = value; }
    }
}
