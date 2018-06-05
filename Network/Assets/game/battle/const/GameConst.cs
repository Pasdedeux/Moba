namespace common.game.battle.constant
{
    class GameConst
    {
        public static class Param
        {
            public const int BATTLE_FIELD = 100;
            public const int BATTLE_FIELD_UNIT = BATTLE_FIELD + 1;
            public const int BATTLE_FIELD_UNIT_ID = BATTLE_FIELD + 2;
            public const int BATTLE_FIELD_POSITION = BATTLE_FIELD + 3;

            public const int MOVE = 300;
            public const int MOVE_POSITION = MOVE + 1;
            public const int MOVE_DIR = MOVE + 2;
            public const int MOVE_SPEED = MOVE + 3;
        }
        public static class Cmd
        {
            public const int BATTLE_FIELD = 100;//战场
            public const int BATTLE_FIELD_ADD_UNIT = BATTLE_FIELD + 1;//向战场添加单位
            public const int BATTLE_FIELD_REMOVE_UNIT = BATTLE_FIELD + 2;//从战场移除单位

            public const int CHARACTOR = 200;
            public const int CHARACTOR_CRT = CHARACTOR + 1;

            public const int MOVE = 300;
            public const int MOVE_CHANGE = MOVE + 1;
            public const int MOVE_FLUSH = MOVE + 2;
        }
        public static class RsCode
        {

        }
    }
}
