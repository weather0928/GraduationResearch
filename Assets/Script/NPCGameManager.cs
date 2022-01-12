using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class NPCGameManager : MonoBehaviour
{
    //�J�[�hPrefab�p�ϐ�
    [SerializeField] CardController cardPrefab; //�t�B�[���h�̃J�[�h�p
    [SerializeField] CardController handCardPrefab; //��D�̃J�[�h�p

    //NPC1�p�̕ϐ�
    [SerializeField] Transform NPC1Field;�@ //�����X�^�[�̏����ꏊ
    [SerializeField] GameObject NPC1ManaCost;�@//�}�i�R�X�g�̕\���ꏊ
    [SerializeField] Text NPC1HPText;�@//�v���C���[��HP�\���ꏊ
    [SerializeField] Transform NPC1HandCard;�@//��D��u���ꏊ
    List<int> NPCDeck1;�@//�f�b�L�Ǘ�
    int NPC1MAXMana, NPC1Mana;�@//�}�i�Ǘ�
    int NPC1HP;�@//HP�Ǘ�

    //NPC2�p�̕ϐ�
    [SerializeField] Transform NPC2Field;
    [SerializeField] GameObject NPC2ManaCost;
    [SerializeField] Text NPC2HPText;
    [SerializeField] Transform NPC2HandCard;
    List<int> NPCDeck2;
    int NPC2MAXMana, NPC2Mana;
    int NPC2HP;

    //�^�[���Ǘ��p�t���O
    //true��NPC1����s�Afalse��NPC2����s�i�����p�ɃC���X�y�N�^�[�ŕ\���j
    [SerializeField] bool isNPC1Turn;

    //�^�[����
    int turnCount;

    //�����I�����Ɏg����
    [SerializeField] GameObject gameEndText;�@//�����I�����̃e�L�X�g
    bool gameEndFlag; //�����I���t���O

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        Deck NPC1OriginalDeck = Resources.Load<Deck>("Deck/NPC1");
        Deck NPC2OriginalDeck = Resources.Load<Deck>("Deck/NPC2");

        NPCDeck1 = new List<int>(NPC1OriginalDeck.cardList.OrderBy(a => Guid.NewGuid()).ToList());
        NPCDeck2 = new List<int>(NPC2OriginalDeck.cardList.OrderBy(a => Guid.NewGuid()).ToList());

        for (int i = 0; i < 3; i++)
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

    //�J�[�h���h���[���鏈��
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

    //�����X�^�[����ɏo������
    void SummonCard(CardController sumonCard, Transform place)
    {
        CardController card = Instantiate(cardPrefab, place);
        card.Init(sumonCard.model.cardID);
    }

    //�^�[���؂�ւ�����
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

    //�^�[���؂�ւ��t���O����
    public void ChangeTurn()
    {
        if(gameEndFlag == false)
        {
            isNPC1Turn = !isNPC1Turn;
            TurnCalc();
        }
    }

    //NPC1�i��ʎ�O�j�̃^�[������
    IEnumerator NPC1Turn()
    {
        if (NPCDeck1.Count == 0) //�f�b�L��0���Ȃ畉���̏���������
        {
            GameEnd(false);
            yield break;
        }
        else //�f�b�L�ɃJ�[�h������΃h���[����
        {
            DrowCard(NPCDeck1, NPC1HandCard);
        }

        if(turnCount == 2) //��U�Ȃ����1�h���[
        {
            DrowCard(NPCDeck1, NPC1HandCard);
        }

        //NPC1�̃t�B�[���h�ɂ��郂���X�^�[�󋵂��擾���čU���\�ɂ���
        CardController[] NPC1FieldCardList = NPC1Field.GetComponentsInChildren<CardController>();
        SetAttackableFieldCard(NPC1FieldCardList, true);

        //��D�̃J�[�h�̏󋵂��擾
        CardController[] handCardList = NPC1HandCard.GetComponentsInChildren<CardController>();

        //�}�i�̃e�L�X�g���擾
        Text NPC1ManaText = NPC1ManaCost.transform.Find("Text").gameObject.GetComponent<Text>();

        //�����҂�
        yield return new WaitForSeconds(1f);

        //�U���\�ȃ����X�^�[������΍U��
        while (Array.Exists(NPC1FieldCardList, card => card.model.canAttack))
        {
            //�U���\�J�[�h���擾
            CardController[] NPC1CanAttackCardList = Array.FindAll(NPC1FieldCardList, card => card.model.canAttack);
            int totalPower = 0;

            //�U���\�J�[�h�̍��v�U���͂��v�Z
            for (int i = 0; i < NPC1CanAttackCardList.Length; i++)
            {
                totalPower += NPC1CanAttackCardList[i].model.power;
            }

            if (totalPower >= NPC2HP)�@//�U���ł���J�[�h�̍U���͍��v�l�������HP��荂����΁A���[�_�[�ɍU��
            {
                //�U������J�[�h��I��
                CardController attackCard = NPC1CanAttackCardList[0];
                AttackToLeader(attackCard, true);
            }
            else
            {
                //NPC2�̃t�B�[���h�̏󋵂��擾
                CardController[] NPC2FieldCardList = NPC2Field.GetComponentsInChildren<CardController>();

                //�U������J�[�h��I��
                CardController attackCard = NPC1CanAttackCardList[0];

                //�U���Ώۂ�I��
                if (NPC2FieldCardList.Length > 0) //����t�B�[���h�Ƀ����X�^�[������ꍇ
                {
                    for(int i = 0;i < NPC2FieldCardList.Length;i++)
                    {
                        //�����̍U�����郂���X�^�[�̍U���͂̒l����HP���Ⴂ����̃����X�^�[��������U��
                        if(attackCard.model.power >= NPC2FieldCardList[i].model.hp)
                        {
                            CardController defenceCard = NPC2FieldCardList[i];
                            CardBattle(attackCard, defenceCard);
                            break;
                        }
                        //�����̍U�����郂���X�^�[�̍U���͂̒l����HP���Ⴂ����̃����X�^�[�����Ȃ���΃��[�_�[�ɍU��
                        if (i == NPC2FieldCardList.Length - 1)
                        {
                            AttackToLeader(attackCard, true);
                        }
                    }
                }
                else //����t�B�[���h�Ƀ����X�^�[�����Ȃ���΁A���胊�[�_�[�ɍU��
                {
                    AttackToLeader(attackCard, true);
                }
            }

            //�����̃t�B�[���h�̏󋵂��Ď擾
            NPC1FieldCardList = NPC1Field.GetComponentsInChildren<CardController>();

            //�����҂�
            yield return new WaitForSeconds(1f);

            if (gameEndFlag == true)
            {
                yield break;
            }
        }

        if(gameEndFlag == false)
        {
            //NPC1�̃t�B�[���h�̏󋵂��Ď擾
            NPC1FieldCardList = NPC1Field.GetComponentsInChildren<CardController>();

            //�������郂���X�^�[�����߂ď���
            for (int i = 0; i < handCardList.Length; i++)
            {
                if (handCardList[i].model.cost <= NPC1Mana && NPC1FieldCardList.Length < 5)
                {
                    SummonCard(handCardList[i], NPC1Field);
                    NPC1Mana -= handCardList[i].model.cost;
                    Destroy(handCardList[i].gameObject);
                    NPC1ManaText.text = NPC1Mana + "/" + NPC1MAXMana;
                    NPC1FieldCardList = NPC1Field.GetComponentsInChildren<CardController>();
                    //�����҂�
                    yield return new WaitForSeconds(1f);
                }
            }

            ChangeTurn();
        }
    }

    //NPC2�i��ʉ��j�̃^�[������
    IEnumerator NPC2Turn()
    {
        if (NPCDeck2.Count == 0) //�f�b�L��0���Ȃ畉���̏���������
        {
            GameEnd(true);
            yield break;
        }
        else //�f�b�L�ɃJ�[�h������΃h���[����
        {
            DrowCard(NPCDeck2, NPC2HandCard);
        }

        if(turnCount == 2) //��U�Ȃ����1�h���[
        {
            DrowCard(NPCDeck2, NPC2HandCard);
        }

        CardController[] NPC2FieldCardList = NPC2Field.GetComponentsInChildren<CardController>();
        SetAttackableFieldCard(NPC2FieldCardList, true);

        CardController[] handCardList = NPC2HandCard.GetComponentsInChildren<CardController>();

        Text NPC2ManaText = NPC2ManaCost.transform.Find("Text").gameObject.GetComponent<Text>();

        //�����҂�
        yield return new WaitForSeconds(1f);

        //�U���\�ȃ����X�^�[������΍U��
        while (Array.Exists(NPC2FieldCardList, card => card.model.canAttack))
        {
            //�U���\�J�[�h���擾
            CardController[] NPC2CanAttackCardList = Array.FindAll(NPC2FieldCardList, card => card.model.canAttack);
            int totalPower = 0;

            //�U���\�J�[�h�̍��v�U���͂��v�Z
            for (int i = 0; i < NPC2CanAttackCardList.Length; i++)
            {
                totalPower += NPC2CanAttackCardList[i].model.power;
            }

            if (totalPower >= NPC1HP)�@//�U���ł���J�[�h�̍U���͍��v�l�������HP��荂����΁A���[�_�[�ɍU��
            {
                //�U������J�[�h��I��
                CardController attackCard = NPC2CanAttackCardList[0];
                AttackToLeader(attackCard, false);
            }
            else
            {
                //NPC1�̃t�B�[���h�̏󋵂��擾
                CardController[] NPC1FieldCardList = NPC1Field.GetComponentsInChildren<CardController>();

                //�U������J�[�h��I��
                CardController attackCard = NPC2CanAttackCardList[0];

                //�U���Ώۂ�I��
                if (NPC1FieldCardList.Length > 0) //����t�B�[���h�Ƀ����X�^�[������ꍇ
                {
                    for (int i = 0; i < NPC1FieldCardList.Length; i++)
                    {
                        //�����̍U�����郂���X�^�[�̍U���͂̒l����HP���Ⴂ����̃����X�^�[��������U��
                        if (attackCard.model.power >= NPC1FieldCardList[i].model.hp)
                        {
                            CardController defenceCard = NPC1FieldCardList[i];
                            CardBattle(attackCard, defenceCard);
                            break;
                        }
                        //�����̍U�����郂���X�^�[�̍U���͂̒l����HP���Ⴂ����̃����X�^�[�����Ȃ���΃��[�_�[�ɍU��
                        if (i == NPC1FieldCardList.Length - 1)
                        {
                            AttackToLeader(attackCard, false);
                        }
                    }
                }
                else //����t�B�[���h�Ƀ����X�^�[�����Ȃ���΁A���胊�[�_�[�ɍU��
                {
                    AttackToLeader(attackCard, false);
                }
            }

            //�����̃t�B�[���h�̏󋵂��Ď擾
            NPC2FieldCardList = NPC2Field.GetComponentsInChildren<CardController>();

            //�����҂�
            yield return new WaitForSeconds(1f);

            if (gameEndFlag == true)
            {
                yield break;
            }

        }

        if (gameEndFlag == false)
        {
            //�����̃t�B�[���h�̏󋵂��Ď擾
            NPC2FieldCardList = NPC2Field.GetComponentsInChildren<CardController>();

            for (int i = 0; i < handCardList.Length; i++)
            {
                if (handCardList[i].model.cost <= NPC2Mana && NPC2FieldCardList.Length < 5)
                {
                    SummonCard(handCardList[i], NPC2Field);
                    NPC2Mana -= handCardList[i].model.cost;
                    Destroy(handCardList[i].gameObject);
                    NPC2ManaText.text = NPC2Mana + "/" + NPC2MAXMana;
                    NPC2FieldCardList = NPC2Field.GetComponentsInChildren<CardController>();
                    //�����҂�
                    yield return new WaitForSeconds(1f);
                }
            }

            ChangeTurn();
        }
    }

    //�U���\����
    void SetAttackableFieldCard(CardController[] cardList, bool canAttack)
    {
        foreach (CardController card in cardList)
        {
            card.model.canAttack = canAttack;
        }
    }

    //�����X�^�[�ւ̍U������
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

    //���[�_�[�ւ̍U������
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
            GameEnd(false);
        }
        else if (NPC2HP <= 0)
        {
            GameEnd(true);
        }
    }

    //���[�_�[�̗͕̑\������
    void ShowLeaderHP()
    {
        NPC1HPText.text = NPC1HP.ToString();
        NPC2HPText.text = NPC2HP.ToString();
    }

    //�����I������
    void GameEnd(bool NPC1WinFlag)
    {
        gameEndText.SetActive(true);
        Text winnerText = gameEndText.GetComponent<Text>();

        if(NPC1WinFlag == true)
        {
            winnerText.text = "NPC1win";
        }
        else
        {
            winnerText.text = "NPC2win";
        }

        gameEndFlag = true;
    }
}
