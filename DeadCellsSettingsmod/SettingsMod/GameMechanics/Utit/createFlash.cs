using dc;
using dc.en;
using dc.pr;

namespace ChiuYiUI.GameMechanics.Utils
{
    public static class createFlash
    {
        public static void headCharge(double r, Hero hero, int furyColor, int baseColor)
        {
            Level currentLevel = Game.Class.ME.curLevel;


            currentLevel.fx.heroHeadCharge(hero, r, furyColor);


            double heroX = (hero.cx + hero.xr) * 24.0;
            double heroY = (hero.cy + hero.yr) * 24.0;
            double headY = heroY - (heroY - hero.get_headY()) * r;

            double offset = dc.Math.Class.random() * 90.0;
            double flashX = heroX + (10.0 + offset) * (Std.Class.random(2) * 2 - 1);
            FlashLight.Class.create(hero._level, flashX, headY, baseColor, 144.0, 0.35, 0.2, null);

            double lightningLength = 150.0 * (1.0 - r);
            double lightningOffset = 40.0 + dc.Math.Class.random() * (lightningLength - 40.0);
            double lightningX = heroX + lightningOffset * (Std.Class.random(2) * 2 - 1);

            currentLevel.fx.heroHeadLightnings(hero, lightningX, heroY, hero.get_headX(), headY, baseColor);
        }
    }
}