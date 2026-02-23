using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IRewindable
{
    protected Vector3 startingPosition;
        protected float health;
            protected bool isDead;

                public virtual void OnSnapshot(RewindFrame frame)
                    {
                            frame.Position = transform.position;
                                    frame.Health = health;
                                            frame.IsDead = isDead;
                                                }

                                                    public virtual void OnRewind(RewindFrame frame)
                                                        {
                                                                transform.position = frame.Position;
                                                                        health = frame.Health;
                                                                                isDead = frame.IsDead;
                                                                                    }
                                                                                    }
