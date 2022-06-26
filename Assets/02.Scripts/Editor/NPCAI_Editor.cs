using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(NPC_AI))]
public class NPCAI_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        NPC_AI ai = (NPC_AI)target;

        EditorGUILayout.Space();

        ai.Target = EditorGUILayout.ObjectField("CurrentTarget", ai.Target, typeof(Collider), false) as Collider;
        EditorGUILayout.FloatField("타켓까지 거리", ai.TargetDist);
        ai.heightDamageTarget = EditorGUILayout.ObjectField("HighestDamageTarget", ai.heightDamageTarget, typeof(GameObject), false) as GameObject;

        EditorGUILayout.Space();

        ai.monsterType = (MonsterType)EditorGUILayout.EnumPopup("Monster Type", ai.monsterType);

        if (ai.monsterType == MonsterType.Melee)
        {
            ai.meleeAttackDist = EditorGUILayout.FloatField("Meele AttackDist", ai.meleeAttackDist);
            ai.meleeAttackMoveSpeed = EditorGUILayout.FloatField("Melee AttackMoveSpeed", ai.meleeAttackMoveSpeed);
            //ai.meleeAttackSpeed = EditorGUILayout.FloatField("Melee AttackSpeed", ai.meleeAttackSpeed);
        }

        if (ai.monsterType == MonsterType.Range)
        {
            //ai.projectile = EditorGUILayout.ObjectField("Projectile", ai.projectile, typeof(GameObject), false) as GameObject;
            //ai.firePos = EditorGUILayout.ObjectField("FirePos", ai.firePos, typeof(Transform), false) as Transform;

            //ai.rangeAttackDist = EditorGUILayout.FloatField("Range AttackDist", ai.rangeAttackDist);
            //ai.rangeAttackSpeed = EditorGUILayout.FloatField("Range AttackSpeed", ai.rangeAttackSpeed);
        }

        if (ai.monsterType == MonsterType.Boss)
        {
            ai.bossType = (BossType)EditorGUILayout.EnumPopup("Boss Type", ai.bossType);
            ai.miniBoss = EditorGUILayout.Toggle("MiniBoss", ai.miniBoss);

            ai.meleeAttackDist = EditorGUILayout.FloatField("Meele AttackDist", ai.meleeAttackDist);
            ai.meleeAttackMoveSpeed = EditorGUILayout.FloatField("Melee AttackMoveSpeed", ai.meleeAttackMoveSpeed);

            //ai.projectile = EditorGUILayout.ObjectField("Projectile", ai.projectile, typeof(GameObject), false) as GameObject;
            //ai.firePos = EditorGUILayout.ObjectField("FirePos", ai.firePos, typeof(Transform), false) as Transform;

            //ai.rangeAttackDist = EditorGUILayout.FloatField("Range AttackDist", ai.rangeAttackDist);
            //ai.rangeAttackSpeed = EditorGUILayout.FloatField("Range AttackSpeed", ai.rangeAttackSpeed);

            ai.jumpAttackCoolTime = EditorGUILayout.FloatField("Jump AttackCoolTime", ai.jumpAttackCoolTime);
            ai.wideAttackArea = EditorGUILayout.FloatField("WideAttackArea", ai.wideAttackArea);
            ai.wideAttackDamageMulti = EditorGUILayout.FloatField("WideAttackDamageMulti", ai.wideAttackDamageMulti);
        }

        if (ai.monsterType == MonsterType.SelfDestruct)
        {
            ai.Summoner = EditorGUILayout.ObjectField("Summoner", ai.Summoner, typeof(GameObject), false) as GameObject;
            ai.ExplosionRadius = EditorGUILayout.FloatField("ExplosionRadius", ai.ExplosionRadius);
            ai.DestructDamage = EditorGUILayout.IntField("DestructDamage", ai.DestructDamage);
        }

        EditorGUILayout.Space();

        ai.applyRootMotionSpeed = EditorGUILayout.Toggle("ApplyRootMotionSpeed", ai.applyRootMotionSpeed);
        ai.sensingDist = EditorGUILayout.FloatField("Sensing Dist", ai.sensingDist);
        ai.traceDist = EditorGUILayout.FloatField("Trace Dist", ai.traceDist);
        ai.moveBaseSpeed = EditorGUILayout.FloatField("BaseSpeed", ai.moveBaseSpeed);
        //ai.traceSpeed.SetBaseValue(ai.baseSpeed);
        ai.RotationSpeed = EditorGUILayout.FloatField("Rotation Speed", ai.RotationSpeed);
        ai.announceDist = EditorGUILayout.FloatField("announce Dist", ai.announceDist);

        EditorGUILayout.Space();

        ai.monsterState = (MonsterState)EditorGUILayout.EnumPopup("Monster State", ai.monsterState);

        EditorGUILayout.Space();

        ai.npcType = (NPC_Type)EditorGUILayout.EnumPopup("NPC Type", ai.npcType);

        if (ai.npcType == NPC_Type.Minion)
        {
            ai.Summoner = EditorGUILayout.ObjectField("Summoner", ai.Summoner, typeof(GameObject), false) as GameObject;
            ai.followSummonerDist = EditorGUILayout.FloatField("Follow SummonerDist", ai.followSummonerDist);
            ai.followSpeed = EditorGUILayout.FloatField("Follow Speed", ai.followSpeed);
        }

        EditorGUILayout.Space();

        ai.canWander = EditorGUILayout.Toggle("EnableWander", ai.canWander);

        if (ai.canWander == true)
        {
            ai.wanderDistanse = EditorGUILayout.FloatField("Wander Distanse", ai.wanderDistanse);
            ai.WanderMinWaitTime = EditorGUILayout.IntField("Wander MinWaitTime", ai.WanderMinWaitTime);
            ai.WanderMaxWaitTime = EditorGUILayout.IntField("Wander MaxWaitTime", ai.WanderMaxWaitTime);
            ai.wanderSpeed = EditorGUILayout.FloatField("Wander Speed", ai.wanderSpeed);
            ai.wanderLeastDst = EditorGUILayout.FloatField("wanderLeastDst", ai.wanderLeastDst);
        }

        EditorGUILayout.Space();

        ai.canPatrol = EditorGUILayout.Toggle("EnablePatrol", ai.canPatrol);

        if (ai.canPatrol == true)
        {
            ai.PatrolMinWaitTime = EditorGUILayout.IntField("Patrol MinWaitTime", ai.PatrolMinWaitTime);
            ai.PatrolMaxWaitTime = EditorGUILayout.IntField("Patrol MaxWaitTime", ai.PatrolMaxWaitTime);
        }

        EditorGUILayout.Space();

        ai.canFollow = EditorGUILayout.Toggle("EnableFollow", ai.canFollow);

        if (ai.canFollow == true)
        {
            ai.followTarget = EditorGUILayout.ObjectField("followTarget", ai.followTarget, typeof(GameObject), false) as GameObject;
            ai.FollowMinDist = EditorGUILayout.IntField("FollowMinDist", ai.FollowMinDist);
        }

        EditorGUILayout.Space();

        ai.FollowItToTheEnd = EditorGUILayout.Toggle("Follow It To The End", ai.FollowItToTheEnd);
        ai.highestDamageFirst = EditorGUILayout.Toggle("Highest Damage First", ai.highestDamageFirst);
        ai.canRotationWhileAttack = EditorGUILayout.Toggle("Can Rotation While Attack", ai.canRotationWhileAttack);

        EditorGUILayout.Space();

        ai.currentState = EditorGUILayout.TextField("CurrentState : ", ai.currentState);

        EditorGUILayout.Space();

        ai.unSyncPosition = EditorGUILayout.Toggle("UnSyncPositon : ", ai.unSyncPosition);

        if(ai.unSyncPosition)
        {
            ai.unSyncTraceSpeed = EditorGUILayout.FloatField("UnSyncTraceSpeed : ", ai.unSyncTraceSpeed);
            ai.unSyncRotationSpeed = EditorGUILayout.FloatField("UnSyncRotationSpeed : ", ai.unSyncRotationSpeed);
            ai.unSyncWanderSpeed = EditorGUILayout.FloatField("UnSyncWanderSpeed : ", ai.unSyncWanderSpeed);
            ai.unSynceAcceleration = EditorGUILayout.FloatField("UnSynceAcceleration : ", ai.unSynceAcceleration);
            EditorGUILayout.Space();
            ai.unSyncCurrentMoveSpeed = EditorGUILayout.FloatField("UnSynceCurrentMoveSpeed : ", ai.unSyncCurrentMoveSpeed);
        }

        if (GUI.changed)        //변경될시                    //커스텀 에디터에서는 SetDirty를 안해주면 저장이 안됨
        {
            EditorUtility.SetDirty(target);
        }

    }

    private void OnSceneGUI()
    {
        NPC_AI ai = (NPC_AI)target;

        Handles.color = Color.white;
        Handles.DrawWireArc(ai.transform.position, Vector3.up, Vector3.forward, 360, ai.sensingDist);

        Handles.color = Color.red;

        foreach (Collider coll in ai.collsList)
        {
            Handles.DrawLine(ai.transform.position + Vector3.up, coll.transform.position);
        }


        //Handles.color = Color.green;                                                  //원거리 공격 raycast 확인용으로 넣어놨는데 다른 코드를 찾아서 일단 지움
        //Handles.DrawLine(ai.firePos.transform.position, ai.Target.bounds.center);

    }
}

