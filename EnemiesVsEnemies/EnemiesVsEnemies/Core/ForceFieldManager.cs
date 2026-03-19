using dc;
using dc.en;
using dc.en.inter;
using ModCore.Modules;

namespace EnemiesVsEnemies.Core
{
    public class ForceFieldManager
    {
        private ForceField? currentForceField = null;
        private bool forceFieldActive = false;

        public ForceField? CurrentForceField => currentForceField;
        public bool IsForceFieldActive => forceFieldActive;

        public ForceField? SpawnForceField(bool openImmediately = false)
        {
            var hero = Game.Instance.HeroInstance;
            if (hero == null)
            {
                return null;
            }

            if (currentForceField != null)
            {
                CloseForceField();
            }

            currentForceField = new ForceField(hero._level, hero.cx, hero.cy, false);
            currentForceField.init();

            if (openImmediately)
            {
                OpenForceField();
            }

            return currentForceField;
        }

        public void OpenForceField()
        {
            if (currentForceField != null && !forceFieldActive)
            {
                currentForceField.open();
                forceFieldActive = true;
            }
        }

        public void CloseForceField()
        {
            if (currentForceField != null && forceFieldActive)
            {
                currentForceField.close(null);
                forceFieldActive = false;
            }
        }

        public void RemoveForceField()
        {
            CloseForceField();
            currentForceField = null;
            forceFieldActive = false;
        }


        public void ToggleForceField()
        {
            if (forceFieldActive)
            {
                CloseForceField();
            }
            else
            {
                OpenForceField();
            }
        }


        public void BeginBattleMode()
        {
            var hero = Game.Instance.HeroInstance;
            if (hero == null)
            {
                return;
            }

            hero.visible = false;
            hero.hasGravity = false;

            OpenForceField();
        }

        public void EndBattleMode()
        {
            var hero = Game.Instance.HeroInstance;
            if (hero == null)
            {
                return;
            }

            hero.visible = true;
            hero.hasGravity = true;
            RemoveForceField();
        }


        public bool IsInBattleMode()
        {
            var hero = Game.Instance.HeroInstance;
            if (hero == null)
            {
                return false;
            }

            return !hero.visible && !hero.hasGravity;
        }


        public void Reset()
        {
            RemoveForceField();

            var hero = Game.Instance.HeroInstance;
            if (hero != null)
            {
                hero.visible = true;
                hero.hasGravity = true;
            }
        }
    }
}