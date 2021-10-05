namespace Squads.CharacterElements
{
    [System.Serializable]
    public abstract class Upgrade
    {
        public abstract void SetDefaultValues();

        public virtual void LoadUpgradePreviewBeforeMatch() {}

        public virtual void ExecuteAtStartOfMatch(CharacterPrefab character) {}


    }
}
