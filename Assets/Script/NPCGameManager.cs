using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class NPCGameManager : MonoBehaviour
{
    //カードPrefab用変数
    [SerializeField] CardController cardPrefab; //フィールドのカード用
    [SerializeField] CardController handCardPrefab; //手札のカード用

    //NPC1用の変数
    [SerializeField] Transform NPC1Field;　 //モンスターの召喚場所
    [SerializeField] GameObject NPC1ManaCost;　//マナコストの表示場所
    [SerializeField] Text NPC1HPText;　//プレイヤーのHP表示場所
    [SerializeField] Transform NPC1HandCard;　//手札を置く場所
    List<int> NPCDeck1;　//デッキ管理
    List<CardController> NPC1HandList;　//手札管理
    int NPC1MAXMana, NPC1Mana;　//マナ管理
    int NPC1HP;　//HP管理

    //NPC2用の変数
    [SerializeField] Transform NPC2Field;
    [SerializeField] GameObject NPC2ManaCost;
    [SerializeField] Text NPC2HPText;
    [SerializeField] Transform NPC2HandCard;
    List<int> NPCDeck2;
    List<CardController> NPC2HandList;
    int NPC2MAXMana, NPC2Mana;
    int NPC2HP;

    //ターン管理用フラグ
    //trueでNPC1が先行、falseでNPC2が先行（実験用にインスペクターで表示）
    [SerializeField] bool isNPC1Turn;

    //ターン数
    int turnCount;

    //試合終了時に使う物
    [SerializeField] GameObject gameEndText;　//試合終了時のテキスト
    bool gameEndFlag;　//試合終了フラグ

    void Start()
    {
        Deck NPC1OriginalDeck = Resources.Load<Deck>("Deck/NPC1");
        Deck NPC2OriginalDeck = Resources.Load<Deck>("Deck/NPC2");

        NPCDeck1 = new List<int>(NPC1OriginalDeck.cardList.OrderBy( a => Guid.NewGuid()).ToList());
        NPCDeck2 = new List<int>(NPC2OriginalDeck.cardList.OrderBy(a => Guid.NewGuid()).ToList());

        for (int i = 0;i < 3;i++)
        {
            DrowCard(NPCDeck1, NPC1HandCard);
            DrowCard(NPCDeck2, NPC2HandCard);
        }

        NPC1MAXMana = 0;
        NPC1Mana = 0;
        NPC2MAXMana = 0;
        NPC2Mana = 0;

        NPC1HP = 20;
        NPC2HP = 20;

        gameEndText.SetActive(false);

        gameEndFlag = false;

        turnCount = 0;

        TurnCalc();
    }

    //カードをドローする処理
    void DrowCard(List<int> deck,Transform hand)
    {
        if (deck.Count == 0)
        {
            return;
        }

        int cardID = deck[0];
        deck.RemoveAt(0);

        CardController[] handCardList = hand.GetComponentsInChildren<CardController>();

        if (handCardList.Length < 9)
        {
            CardController card = Instantiate(handCardPrefab, hand);
            card.Init(cardID);
        }
    }

    //モンスターを場に出す処理
    void SummonCard(CardController sumonCard, Transform place)
    {
        CardController card = Instantiate(cardPrefab, place);
        card.Init(sumonCard.model.cardID);
    }

    //ターン切り替え処理
    void TurnCalc()
    {
        if(gameEndFlag == false)
        {
            Image NPC1ManaImage = NPC1ManaCost.GetComponent<Image>();
            Text NPC1ManaText = NPC1ManaCost.transform.Find("Text").gameObject.GetComponent<Text>();
            Image NPC2ManaImage = NPC2ManaCost.GetComponent<Image>();
            Text NPC2ManaText = NPC2ManaCost.transform.Find("Text").gameObject.GetComponent<Text>();

            turnCount++;

            if (isNPC1Turn)
            {
                NPC1ManaImage.color = Color.yellow;
                NPC2ManaImage.color = Color.white;

                if (NPC1MAXMana < 10)
                {
                    NPC1MAXMana++;
                }
                NPC1Mana = NPC1MAXMana;

                NPC1ManaText.text = NPC1Mana + "/" + NPC1MAXMana;

                StartCoroutine(NPC1Turn());
            }
            else
            {
                NPC1ManaImage.color = Color.white;
                NPC2ManaImage.color = Color.yellow;

                if (NPC2MAXMana < 10)
                {
                    NPC2MAXMana++;
                }
                NPC2Mana = NPC2MAXMana;

                NPC2ManaText.text = NPC2Mana + "/" + NPC2MAXMana;

                StartCoroutine(NPC2Turn());
            }
        }
    }

    //ターン切り替えフラグ処理
    public void ChangeTurn()
    {
        if(gameEndFlag == false)
        {
            isNPC1Turn = !isNPC1Turn;
            TurnCalc();
        }
    }

    //NPC1（画面手前）のターン処理
    IEnumerator NPC1Turn()
    {
        //ドロー処理
        if(turnCount == 2) //後攻1ターン目なら追加で1ドロー
        {
            DrowCard(NPCDeck1, NPC1HandCard);
            Debug.Log("aaa");
        }

        if (NPCDeck1.Count == 0) //デッキが0枚なら負けの処理をする
        {
            GameEnd("NPC2");
            yield break;
        }
        else //デッキにカードがあればドローする
        {
            DrowCard(NPCDeck1, NPC1HandCard);
        }

        //NPC1のフィールドにいるモンスター状況を取得して攻撃可能にする
        CardController[] NPC1FieldCardList = NPC1Field.GetComponentsInChildren<CardController>();
        SetAttackableFieldCard(NPC1FieldCardList, true);

        //手札のカードの状況を取得
        CardController[] handCardList = NPC1HandCard.GetComponentsInChildren<CardController>();

        //マナのテキストを取得
        Text NPC1ManaText = NPC1ManaCost.transform.Find("Text").gameObject.GetComponent<Text>();

        //処理待ち
        yield return new WaitForSeconds(1f);

        //召喚するモンスターを決めて召喚
        for (int i = 0; i < handCardList.Length; i++)
        {
            if (handCardList[i].model.cost <= NPC1Mana && NPC1FieldCardList.Length < 5)
            {
                SummonCard(handCardList[i], NPC1Field);
                NPC1Mana -= handCardList[i].model.cost;
                Destroy(handCardList[i].gameObject);
                NPC1ManaText.text = NPC1Mana + "/" + NPC1MAXMana;
                //処理待ち
                yield return new WaitForSeconds(1f);
            }
        }

        //NPC1のフィールドの状況を再取得
        NPC1FieldCardList = NPC1Field.GetComponentsInChildren<CardController>();

        //攻撃可能なモンスターがいれば攻撃
        while (Array.Exists(NPC1FieldCardList, card => card.model.canAttack))
        {
            //攻撃可能カードを取得
            CardController[] NPC1CanAttackCardList = Array.FindAll(NPC1FieldCardList, card => card.model.canAttack);

            //NPC2のフィールドの状況を取得
            CardController[] NPC2FieldCardList = NPC2Field.GetComponentsInChildren<CardController>();

            //攻撃するカードを選択
            CardController attackCard = NPC1CanAttackCardList[0];

            //攻撃対象を選択
            if(NPC2FieldCardList.Length > 0) //相手フィールドにモンスターがいたら、1番古いモンスターに攻撃
            {
                CardController defenceCard = NPC2FieldCardList[0];
                CardBattle(attackCard, defenceCard);
            }
            else //相手フィールドにモンスターがいなければ、相手リーダーに攻撃
            {
                AttackToLeader(attackCard, true);
            }

            //処理待ち
            yield return new WaitForSeconds(1f);

            //自分のフィールドの状況を再取得
            NPC1FieldCardList = NPC1Field.GetComponentsInChildren<CardController>();
        }

        if(gameEndFlag == false)
        {
            ChangeTurn();
        }

    }

    //NPC2（画面奥）のターン処理
    IEnumerator NPC2Turn()
    {
        //ドロー処理
        if (turnCount == 2) //後攻1ターン目なら追加で1ドロー
        {
            DrowCard(NPCDeck2, NPC2HandCard);
        }

        if (NPCDeck2.Count == 0) //デッキが0枚なら負けの処理をする
        {
            GameEnd("NPC1");
            yield break;
        }
        else //デッキにカードがあればドローする
        {
            DrowCard(NPCDeck2, NPC2HandCard);
        }

        CardController[] NPC2FieldCardList = NPC2Field.GetComponentsInChildren<CardController>();
        SetAttackableFieldCard(NPC2FieldCardList, true);

        CardController[] handCardList = NPC2HandCard.GetComponentsInChildren<CardController>();

        Text NPC2ManaText = NPC2ManaCost.transform.Find("Text").gameObject.GetComponent<Text>();

        //処理待ち
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < handCardList.Length; i++)
        {
            if (handCardList[i].model.cost <= NPC2Mana && NPC2FieldCardList.Length < 5)
            {
                SummonCard(handCardList[i], NPC2Field);
                NPC2Mana -= handCardList[i].model.cost;
                Destroy(handCardList[i].gameObject);
                NPC2ManaText.text = NPC2Mana + "/" + NPC2MAXMana;
                //処理待ち
                yield return new WaitForSeconds(1f);
            }
        }

        //NPC2のフィールドの状況を再取得
        NPC2FieldCardList = NPC2Field.GetComponentsInChildren<CardController>();

        //攻撃可能なモンスターがいれば攻撃
        while (Array.Exists(NPC2FieldCardList, card => card.model.canAttack))
        {
            //攻撃可能カードを取得
            CardController[] NPC2CanAttackCardList = Array.FindAll(NPC2FieldCardList, card => card.model.canAttack);

            //NPC1のフィールドの状況を取得
            CardController[] NPC1FieldCardList = NPC1Field.GetComponentsInChildren<CardController>();

            //攻撃するカードを選択
            CardController attackCard = NPC2CanAttackCardList[0];

            //攻撃対象を選択
            if (NPC1FieldCardList.Length > 0) //相手フィールドにモンスターがいたら、1番古いモンスターに攻撃
            {
                CardController defenceCard = NPC1FieldCardList[0];
                CardBattle(attackCard, defenceCard);
            }
            else //相手フィールドにモンスターがいなければ、相手リーダーに攻撃
            {
                AttackToLeader(attackCard, false);
            }

            //処理待ち
            yield return new WaitForSeconds(1f);

            //自分のフィールドの状況を再取得
            NPC2FieldCardList = NPC2Field.GetComponentsInChildren<CardController>();
        }

        if (gameEndFlag == false)
        {
            ChangeTurn();
        }
    }

    //攻撃可能処理
    void SetAttackableFieldCard(CardController[] cardList, bool canAttack)
    {
        foreach (CardController card in cardList)
        {
            card.model.canAttack = canAttack;
        }
    }

    //モンスターへの攻撃処理
    void CardBattle(CardController attackCard, CardController defenceCard)
    {
        defenceCard.model.hp -= attackCard.model.power;
        attackCard.model.hp -= defenceCard.model.power;

        attackCard.model.canAttack = false;

        defenceCard.view.HPValue(defenceCard.model.hp);
        attackCard.view.HPValue(attackCard.model.hp);

        if(defenceCard.model.hp <= 0)
        {
            Destroy(defenceCard.gameObject);
        }
        if(attackCard.model.hp <= 0)
        {
            Destroy(attackCard.gameObject);
        }
    }

    //リーダーへの攻撃処理
    void AttackToLeader(CardController attackCard, bool isNPC1Card)
    {
        if(attackCard.model.canAttack == false)
        {
            return;
        }

        if(isNPC1Card == true)
        {
            NPC2HP -= attackCard.model.power;
        }
        else
        {
            NPC1HP -= attackCard.model.power;
        }

        attackCard.model.canAttack = false;
        ShowLeaderHP();

        if (NPC1HP <= 0)
        {
            GameEnd("NPC2");
        }
        else if (NPC2HP <= 0)
        {
            GameEnd("NPC1");
        }
    }

    //リーダーの体力表示処理
    void ShowLeaderHP()
    {
        NPC1HPText.text = NPC1HP.ToString();
        NPC2HPText.text = NPC2HP.ToString();
    }

    //試合終了処理
    void GameEnd(string winner)
    {
        gameEndText.SetActive(true);
        Text winnerText = gameEndText.GetComponent<Text>();
        winnerText.text = winner + "win";
        gameEndFlag = true;
    }
}
