using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoolatry
{
    public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";

    public static int PANEL_TO_BE_LOADED = 0;
    
    public const float PICKUP_SPAWNER_TIMER_MIN = 5f; // seconds
    public const float PICKUP_SPAWNER_TIMER_MAX = 10f; // seconds
    
    public const float PICKUP_HEALTH_PERCENT = .5f;
    public const float PICKUP_AMMO_PERCENT = .5f;


    //---------
    
    public const float CHICKY_HP_MAX = 100f;
    public const float CHICKY_SPEED_BASE = .3f;

    public const int CHICKY_AMMO_MAX = 320;
    public const int CHICKY_MAGAZINE_CAPACITY = 20;
    public const float CHICKY_RELOAD_TIMER = .3f; // seconds

    public const float CHICKY_SHOOTING_SPEED = 5f; // times within a second
    public const float CHICKY_SHOOTING_SPREAD = .5f;

    public const float CHICKY_PROJECTILE_SPEED = .3f;
    public const float CHICKY_PROJECTILE_RANGE = 50f;
    public const float CHICKY_PROJECTILE_DAMAGE = 5f;

    //---------
    
    public const float PIGGY_HP_MAX = 100f;
    public const float PIGGY_SPEED_BASE = .07f;

    public const int PIGGY_AMMO_MAX = 120;
    public const int PIGGY_MAGAZINE_CAPACITY = 6;
    public const float PIGGY_RELOAD_TIMER = .5f; // seconds

    public const float PIGGY_SHOOTING_SPEED = 3f; // times within a second
    public const float PIGGY_SHOOTING_SPREAD = .2f;

    public const float PIGGY_PROJECTILE_SPEED = .2f;
    public const float PIGGY_PROJECTILE_RANGE = 10f;
    public const float PIGGY_PROJECTILE_DAMAGE = 15f;
    
    //---------

    public enum PLAYER_CHARACTER {Chicky, Piggy};
    public enum BULLET_TARGET {Players,Enemies,All};
    public enum PICKUP_TYPE { Health, Ammo};
}
