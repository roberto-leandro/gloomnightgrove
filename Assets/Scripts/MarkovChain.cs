using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkovChain
{
    private float[,] transitionMatrix;
    public float[,] TransitionMatrix { get { return transitionMatrix; } set { transitionMatrix = value; } }
    private int currentState;
    public int CurrentState { get { return currentState; } }

    public MarkovChain(float[,] transMatrix, int initialState) 
    {
        transitionMatrix = transMatrix;
        currentState = initialState;
    }

    public int getCurrentState()
    {
        return currentState;
    }

    public int generateNextState()
    {
        float randomFloat = Random.Range(0f, 1.0f);
        for(int i = 0; i < transitionMatrix.Length; i++)
        {
            if(randomFloat < transitionMatrix[currentState, i])
            {
                currentState = i;
                return i;
            }
            randomFloat = randomFloat - transitionMatrix[currentState, i];
        }
        return -1;
    }
}
