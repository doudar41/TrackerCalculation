using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EnemyWeapon : RhythmWeapon
{
    [SerializeField]Enemy enemy;
    [SerializeField]Character target;
    int elementalDamageModifier = 0;
    [SerializeField] List<SequenceRow> rows = new List<SequenceRow>();
    [SerializeField] string FMODParameter = "PlayElementEnemy01";

    BeatUpdateSource beatSource;

    //QueuedRunesong runesongAtEndOfTurn = new QueuedRunesong();
    int attackCount = -1, beatCount=0;

    public UnityEvent stopPlayingRunesong;

    private void Start()
    {
        beatSource = FindAnyObjectByType<BeatUpdateSource>();

        GamesMainEvents.UpdateOnBeat += Attack;
        runesongAtEndOfTurn = new QueuedRunesong(target, enemy);
        foreach (SequenceRow s in rows)
        {
            trackerRowList.Add(s.rowEffect, s);
        }
        
    }
    public void GetElementalModifierFromPlayer(int amount)
    {
        elementalDamageModifier = amount;
    }

    public void Attack(string intFromFMODMarker)
    {
        
        int.TryParse(intFromFMODMarker, out int countParse);
        if (countParse % 2 == 0 && attackCount == -1) return;

        if(countParse % 2 != 0)
        {
            //beatSource.SetParameterToInstance(FMODParameter, 1);
            beatCount++;
            if (beatCount > GamesMainEvents.turnLength - enemy.ReturnCurrentSpeed())
            {
                List<Elements> elementsInColumn = new List<Elements>();
                int elementsCount = Random.Range(1, rows.Count + 1);

                var random = new System.Random();
                var numbers = TakeRandom(Enumerable.Range(0, rows.Count), elementsCount, random);

                foreach (int i in numbers)
                {
                    print(elementsCount + " " + rows[i].rowEffect + " " + rows.Count);
                }
                if (elementsCount > 1)
                {
                    foreach (int i in numbers)
                    {
                        DamageCalc(target, rows[i].rowEffect);
                        elementsInColumn.Add(rows[i].rowEffect);
                        runesongAtEndOfTurn.occupiedRuneSlotsinRow[rows[i].rowEffect] = runesongAtEndOfTurn.occupiedRuneSlotsinRow[rows[i].rowEffect] + 1;
                    }

                }
                else
                {
                    DamageCalc(target, rows[0].rowEffect);
                    elementsInColumn.Add(rows[0].rowEffect);
                    runesongAtEndOfTurn.occupiedRuneSlotsinRow[rows[0].rowEffect] = runesongAtEndOfTurn.occupiedRuneSlotsinRow[rows[0].rowEffect] + 1;
                }


                CheckElementalInColumn(elementsInColumn, beatSource, FMODParameter);

                attackCount++;
                if (attackCount > 7)
                {
                    StartCoroutine(OnFinishRuneSong(runesongAtEndOfTurn));
                    attackCount = -1;
                    stopPlayingRunesong.Invoke();
                    runesongAtEndOfTurn = new QueuedRunesong(target, enemy);
                }

                beatCount = 0;
            }
        }
        else
        {
            beatSource.SetParameterToInstance(FMODParameter, 0);
        }
    }


    public static List<int> TakeRandom (  IEnumerable<int> source, int count, System.Random random)
    {
        var list = new List<int>(count);
        int n = 1;
        foreach (var item in source)
        {
            if (list.Count < count)
            {
                list.Add(item);
            }
            else
            {
                int j = random.Next(n);
                if (j < count)
                {
                    list[j] = item;
                }
            }
            n++;
        }
        return list;
    }


}


