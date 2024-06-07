using SplashKitSDK;

public class UFO : Enemy
{
    public UFO(Window gameWindow, int index, int spawnDensity) : base(gameWindow)
    {
        Bitmap ufo = new Bitmap("ufo", "images/UFO/shipPink_manned.png");
        Bitmap ufoDamage = new Bitmap("ufodmg", "images/UFO/shipPink.png");
        Bitmap ufoDamage1 = new Bitmap("ufodmg1", "images/UFO/shipPink_damage1.png");
        Bitmap ufoDamage2 = new Bitmap("ufodmg2", "images/UFO/shipPink_damage2.png");

        EnemySprite = SplashKit.CreateSprite("UFO", ufo);
        EnemySprite.AddLayer(ufoDamage, "ufodmg");
        EnemySprite.AddLayer(ufoDamage1, "ufodmg1");
        EnemySprite.AddLayer(ufoDamage2, "ufodmg2");

        X = index * (-spawnDensity) - EnemySprite.Width;
        Y = SplashKit.Rnd(170, 340) - EnemySprite.Height / 2;
        Lives = 10; 
    }

    public override void Update()
    {
        base.Update();

        if (EnemySprite != null)
        {
            if (Lives < 8)
            {
                SplashKit.SpriteHideLayer(EnemySprite, 0);
                SplashKit.SpriteShowLayer(EnemySprite, 1);
            }

            if (Lives < 6)
            {
                SplashKit.SpriteHideLayer(EnemySprite, 1);
                SplashKit.SpriteShowLayer(EnemySprite, 2);
            }

            if (Lives < 3)
            {
                SplashKit.SpriteHideLayer(EnemySprite, 2);
                SplashKit.SpriteShowLayer(EnemySprite, 3);
            }
        }
    }
}
