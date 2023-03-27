using System.Collections;
using System.Collections.Generic;



public class PartyMemberData
{
    public string Name;     //이름
    public int HP;          //체력
    public int MP;          //마나
    public int St;          //힘-칼 기본 공격 위력 상승, 무릴 스킬 위력 소폭 상승
    public int Dx;          //기-총 기본 공격 위력 상승, 물리 스킬 위력 대폭 상승
    public int Ma;          //마-마법 스킬 위력 상승
    public int Ag;          //속-선제턴 확률, 행동순서, 명중률, 회피율 상승
    public int Lu;          //운-크리티컬, 회피율, 상태이상 성공률, 상태이상 회복속도, 아이템 드랍률 상승
    public AilmentType NowAilment;  //현재 걸려있는 상태이상-없을 경우 null, 상태이상은 중첩되지 않는다: 기존에 걸려있을 경우 새 상태이상이 덮어씌움

}

public class PartyDemonData : PartyMemberData
{
    public int ID;          //악마 식별 번호

}
