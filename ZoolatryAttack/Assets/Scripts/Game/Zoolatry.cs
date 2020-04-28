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
    public const float CHICKY_SPEED_BASE = .1f;

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

    public const float GAME_GRAVITY = -9.81f;
    public const float GAME_FALL_MULTIPLIER = 2.5f;
    public const float GAME_GROUND_DISTANCE_TEST = 0.4f;

    //---------

    public enum PLAYER_CHARACTER { Chicky, Piggy };
    public enum BULLET_TARGET { Players, Enemies, All };
    public enum PICKUP_TYPE { Health, Ammo };
    public enum CAGE_TYPE { Small, Medium, Large };

    //---------

    public struct PlayerVariables
    {
        public float healthMax;
        public float healthCurrent;
        public float moveSpeed;

        public PlayerVariables(PLAYER_CHARACTER character)
        {
            if(character == PLAYER_CHARACTER.Piggy)
            {
                healthMax = PIGGY_HP_MAX;
                healthCurrent = PIGGY_HP_MAX;
                moveSpeed = PIGGY_SPEED_BASE;
            }
            else
            {
                healthMax = CHICKY_HP_MAX;
                healthCurrent = CHICKY_HP_MAX;
                moveSpeed = CHICKY_SPEED_BASE;
            }

        }
    }


    public struct PlayerGun
    {
        public int ammoCarryMax;
        public int ammoCarrying;

        public int ammoMagazineSize;
        public int ammoLoaded;
        
        public float shootSpeed;
        public float reloadSpeed;

        public PlayerGun(PLAYER_CHARACTER character)
        {
            if(character == PLAYER_CHARACTER.Piggy)
            {
                ammoCarryMax = PIGGY_AMMO_MAX;
                ammoCarrying = PIGGY_AMMO_MAX;

                ammoMagazineSize = PIGGY_MAGAZINE_CAPACITY;
                ammoLoaded = PIGGY_MAGAZINE_CAPACITY;

                shootSpeed = PIGGY_SHOOTING_SPEED;
                reloadSpeed = PIGGY_RELOAD_TIMER;
            }
            else
            {
                ammoCarryMax = CHICKY_AMMO_MAX;
                ammoCarrying = CHICKY_AMMO_MAX;

                ammoMagazineSize = CHICKY_MAGAZINE_CAPACITY;
                ammoLoaded = CHICKY_MAGAZINE_CAPACITY;

                shootSpeed = CHICKY_SHOOTING_SPEED;
                reloadSpeed = CHICKY_RELOAD_TIMER;
            }

        }
    }
}
