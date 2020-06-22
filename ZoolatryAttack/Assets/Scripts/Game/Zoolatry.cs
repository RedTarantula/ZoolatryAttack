using System;
using UnityEngine;

public static class Zoolatry
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

    public const float GUARD_BASE_HP_MAX = 0.04f;
    public const float GUARD_BASE_SPEED_BASE = 0.4f;

    public const int GUARD_BASE_MAGAZINE_CAPACITY = 6;
    public const float GUARD_BASE_RELOAD_TIMER = 1f;

    public const float GUARD_BASE_SHOOTING_SPEED = 1f; // times within a second
    public const float GUARD_BASE_SHOOTING_SPREAD = .3f;

    public const float GUARD_BASE_PROJECTILE_SPEED = .2f;
    public const float GUARD_BASE_PROJECTILE_RANGE = 8f;
    public const float GUARD_BASE_PROJECTILE_DAMAGE = 15f;

    public const float GUARD_BASE_DISTANCE_SHOOT_TARGET = .7f; // how close it must be to the target to start shooting at it
    public const float GUARD_BASE_DISTANCE_FOLLOW_TARGET = 2f; // how far the target must be for the guard to start chasing it again
    public const float GUARD_BASE_DISTANCE_LOSE_AGGRO = 10f;

    public const float GUARD_BASE_SCOUTING_DISTANCE = 15f;
    public const float GUARD_BASE_GUARDING_TIMER = 5f;
    //---------

    public const float GAME_GRAVITY = -9.81f;
    public const float GAME_FALL_MULTIPLIER = 2.5f;
    public const float GAME_GROUND_DISTANCE_TEST = 0.4f;

    //---------

    public enum PLAYER_CHARACTER { Chicky, Piggy };
    public enum GUARD_CHARACTER { Base };
    public enum BULLET_TARGET { Players, Enemies, All };
    public enum PICKUP_TYPE { Health, Ammo };
    public enum CAGE_TYPE { Small, Medium, Large };
    public enum GUARD_STATE { Scouting, Following, Guarding, Shooting, Capturing, Idle };

    /*  ==============
     *   GUARD STATES
     *  ==============
     *  
     * Scouting = walks around searching for the players
     * Following = follows the target
     * Guarding = stays in place looking for a target
     * Shooting = shooting towards the player
     * Capturing = moving cage to enemy vehicle
     */

    //---------

    [Serializable]
    public struct GuardVariables
    {
        public float healthMax;
        public float healthCurrent;
        public float moveSpeed;

        public int ammoMagazineSize;
        public int ammoLoaded;
        
        public float shootSpeed;
        public float reloadSpeed;

        public GameObject target;

        public float tgtDistShoot; // Distance for the guard to change from moving to shooting
        public float tgtDistFollow; // Distance for the guard to change from shooting to moving
        public float tgtDistAggro; // Distance for the guard to lose aggro on its target

        public float scoutDist;
        public float guardDuration;

        public GUARD_STATE state;

        public GuardVariables(GUARD_CHARACTER guard)
        {
            state = GUARD_STATE.Scouting;

            switch (guard)
            {
                default:
                    healthMax = GUARD_BASE_HP_MAX;
                    moveSpeed = GUARD_BASE_SPEED_BASE;
                    ammoMagazineSize = GUARD_BASE_MAGAZINE_CAPACITY;
                    shootSpeed = GUARD_BASE_SHOOTING_SPEED;
                    reloadSpeed = GUARD_BASE_RELOAD_TIMER;
                    tgtDistShoot = GUARD_BASE_DISTANCE_SHOOT_TARGET;
                    tgtDistFollow = GUARD_BASE_DISTANCE_FOLLOW_TARGET;
                    tgtDistAggro = GUARD_BASE_DISTANCE_LOSE_AGGRO;
                    scoutDist = GUARD_BASE_SCOUTING_DISTANCE;
                    guardDuration = GUARD_BASE_GUARDING_TIMER;
                    break;
            }

            healthCurrent = healthMax;
            ammoLoaded = ammoMagazineSize;
            target = null;
        }

        public float TgtDistance(Transform guardTf) { return Vector3.Distance(guardTf.position,target.transform.position); }
    }

    
    [Serializable]
    public struct PlayerVariables
    {
        public float healthMax;
        public float healthCurrent;
        public float moveSpeed;

        public int ammoCarryMax;
        public int ammoCarrying;

        public int ammoMagazineSize;
        public int ammoLoaded;
        
        public float shootSpeed;
        public float reloadSpeed;

        public PlayerVariables(PLAYER_CHARACTER character)
        {
            if(character == PLAYER_CHARACTER.Piggy)
            {
                healthMax = PIGGY_HP_MAX;
                healthCurrent = PIGGY_HP_MAX;
                moveSpeed = PIGGY_SPEED_BASE;

                ammoCarryMax = PIGGY_AMMO_MAX;
                ammoCarrying = PIGGY_AMMO_MAX;

                ammoMagazineSize = PIGGY_MAGAZINE_CAPACITY;
                ammoLoaded = PIGGY_MAGAZINE_CAPACITY;

                shootSpeed = PIGGY_SHOOTING_SPEED;
                reloadSpeed = PIGGY_RELOAD_TIMER;
            }
            else
            {
                healthMax = CHICKY_HP_MAX;
                healthCurrent = CHICKY_HP_MAX;
                moveSpeed = CHICKY_SPEED_BASE;

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
