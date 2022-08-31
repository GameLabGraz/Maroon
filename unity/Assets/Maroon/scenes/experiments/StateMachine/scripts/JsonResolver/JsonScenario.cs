namespace Maroon.CSE.StateMachine.JsonData
{
    [System.Serializable]
    public class JsonScenario
    {
        public JsonFigure[] figures;
        public JsonFigure[] figuresToMove;
        public JsonDirection[] directions;
        public JsonEnemyMove[] enemyMoves;
        public string description;
        public string playerToPlay;
        public string destination;
        public bool mustHitAllEnemies;
    }
}
