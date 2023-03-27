using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AilmentType
{
    Poison,     //독 - 전투에서 행동시 체력 감소, 필드에서 이동시 체력 감소/자동 회복X
    Sick,       //감기 - 전투에서 공격력 25% 하락, 회피율 0%로 하락, 턴 종료마다 5%의 확률로 타 아군 캐릭터에게 감염/자동 회복X
    Panic,      //혼란 - 조작 불가능, 마음대로 행동함 / 전투 중 및 전투 종료시 자동 회복
    Bind,       //결박 - 행동 불가능, 결박 상태의 악마에게 각종 대화시 플러스 효과 / 전투 중 및 전투 종료시 자동 회복
    Sleep,      //수면 - 행동 불가능, 피격시 25% 추가 대미지, 피격시 회복되지 않음 / 전투 중 및 전투 종료시 자동 회복
}

public class CharacterCondition
{
    
}
